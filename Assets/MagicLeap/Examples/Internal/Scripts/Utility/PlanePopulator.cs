// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.UI;
using MagicLeap.Core;
using MagicLeap.Core.StarterKit;

namespace MagicLeap
{
    /// <summary>
    /// PlanesPopulator is a component designed to hook into the MLPlanesBehavior component
    /// and use the planes information to intelligently place pre-defined content
    /// into the scene.
    /// </summary>
    [RequireComponent(typeof(MLPlanesBehavior))]
    public class PlanePopulator : MonoBehaviour
    {
        public enum SurfaceType
        {
            Ceiling = MLPlanes.QueryFlags.SemanticCeiling,
            Floor = MLPlanes.QueryFlags.SemanticFloor,
            Wall = MLPlanes.QueryFlags.SemanticWall
        }

        [SerializeField, Tooltip("The cursor that should be used for object placement.")]
        public MLRaycastVisualizer Cursor;

        [SerializeField, Tooltip("The current placement location status.")]
        public Text StatusLabel;

        [SerializeField]
        [Tooltip("The array of ContentObject prefabs that can be placed onto the detected planes.")]
        public ContentObject[] ObjectsToPlace;

        // Planes component
        private MLPlanesBehavior _planes;

        // All instances placed
        private List<GameObject> _placedContent = new List<GameObject>();

        // Map of surface types and prefabs
        private Dictionary<uint, List<ContentObject>> _sortedContentObjects = new Dictionary<uint, List<ContentObject>>();

        // Main camera
        private Camera _mainCamera;

        //Checks if cursor position is valid or not
        private bool _isValid;

        // Stores the type of surface the cursor is positioned at
        private SurfaceType _type = 0;

        // Current object being placed
        private GameObject _obj;

        // Type of surface of current object being placed
        private SurfaceType _objType = 0;

        private float _currentAngle;

        // Holds if it's the first time objects being placed or not
        private bool _firstPlacement;

        private MLInput.Controller _controller;

        /// <summary>
        /// Initializes all required components and data for this
        /// script to use during runtime. Also does error checking
        /// to make sure all required components have been enabled.
        /// </summary>
        private void Start()
        {
            _planes = gameObject.GetComponent<MLPlanesBehavior>();
            _mainCamera = Camera.main;

            _type = SurfaceType.Floor;
            _isValid = false;

            _currentAngle = 0.0f;
            _firstPlacement = true;

            if (_planes == null)
            {
                Debug.LogError("MLPlanesBehavior component cannot be found when initializing PlanesPopulator. Disabling component.");
                this.enabled = false;
                return;
            }

            if (Cursor == null)
            {
                Debug.LogError("Cursor reference was not set when initializing PlanesPopulator. Disabling component.");
                this.enabled = false;
                return;
            }

            if (Cursor.raycast == null)
            {
                Debug.LogError("Cursor.raycast reference was not set when initializing PlanesPopulator. Disabling component.");
                this.enabled = false;
                return;
            }

            if (StatusLabel == null)
            {
                Debug.LogError("Status Label reference was not set when initializing PlanesPopulator. Disabling component.");
                this.enabled = false;
                return;
            }

            #if PLATFORM_LUMIN
            MLResult result = MLInput.Start();
            if (!result.IsOk)
            {
                Debug.LogError("Controller reference was not set when initializing PlanesPopulator. Disabling component.");
                this.enabled = false;
                return;
            }
            #endif

            SortContent();

            #if PLATFORM_LUMIN
            _controller = MLInput.GetController(MLInput.Hand.Left);
            MLInput.OnControllerButtonDown += HandleOnButtonPressed;
            #endif

            Cursor.raycast.OnRaycastResult += OnRaycastResult;

            Cursor.gameObject.SetActive(false);
            StatusLabel.gameObject.SetActive(false);

        }

        /// <summary>
        /// Detects the type of surface the cursor is at.
        /// </summary>
        private void OnRaycastResult(MLRaycast.ResultState state, MLRaycastBehavior.Mode mode, Ray ray, RaycastHit hit, float confidence)
        {
            if (_firstPlacement == true)
            {
                return;
            }

            else
            {
                _isValid = true;
                float dot = Vector3.Dot(-Cursor.transform.forward, Vector3.up);

                //are we oriented like a table/floor or a wall?
                if (dot > .5f || dot < -.5f)
                {
                    if (dot < 0)
                    {
                        _type = SurfaceType.Floor;
                    }
                    else if (dot > 0)
                    {
                        _type = SurfaceType.Ceiling;
                    }

                }
                else
                {
                    _type = SurfaceType.Wall;
                }
            }

            if (StatusLabel != null)
            {
                StatusLabel.text = "Valid Plane " + _type.ToString();
                StatusLabel.color = Color.green;
            }
            if (StatusLabel != null)
            {
                StatusLabel.text = "Invalid Location";
                StatusLabel.color = Color.red;
            }

            UpdatePad();
            UpdateObject();
        }

        /// <summary>
        /// Cleans up all placed objects and clears the sortedContent dictionary.
        /// Also unregisters all events from the MagicLeap device.
        /// </summary>
        private void OnDestroy()
        {
            CleanContent();
            _sortedContentObjects.Clear();

            #if PLATFORM_LUMIN
            MLInput.OnControllerButtonDown -= HandleOnButtonPressed;
            MLInput.Stop();
            #endif
        }

        #if PLATFORM_LUMIN
        /// <summary>
        /// Based on the passed in SurfaceType, returns a list of ContentObjects
        /// that meet the same flag requirements.
        /// </summary>
        /// <param name="flag">The SurfaceType to be checked</param>
        private ContentObject[] GetCandidateObjects(MLPlanes.Plane plane)
        {
            var candidates = new List<ContentObject>();
            var flags = System.Enum.GetValues(typeof(SurfaceType)) as SurfaceType[];

            // Get all of the ContentObject refs that match these flags
            for (int i = 0; i < flags.Length; ++i)
            {
                if (MLPlanesStarterKit.DoesPlaneHaveFlag(plane, (MLPlanes.QueryFlags)flags[i]))
                {
                    candidates.AddRange(_sortedContentObjects[(uint)flags[i]]);
                }
            }

            return candidates.ToArray();
        }
        #endif

        /// <summary>
        /// Loops through all currently detected planes and randomly chooses objects
        /// to place out of the pool of objects that have met the criteria of fitting
        /// on said plane.
        /// </summary>
        private void PlaceContent()
        {
            CleanContent();

            #if PLATFORM_LUMIN
            foreach (MLPlanes.Plane plane in _planes.PlanesResult)
            {
                ContentObject[] candidates = GetCandidateObjects(plane);

                if (candidates.Length == 0)
                {
                    continue;
                }

                // Now select a random object from the candidate list.
                int candidateIndex = Random.Range(0, candidates.Length);
                ContentObject prefabReference = candidates[candidateIndex];

                // Save a reference to the local bounds of our new obj
                Bounds bounds = prefabReference.LocalBounds;

                // Instantiate candidate as a game object
                GameObject newObject = GameObject.Instantiate(prefabReference.gameObject);

                // Add the new object to the placedContent list
                _placedContent.Add(newObject);

                // Center the new object on the plane.
                newObject.transform.position = plane.Center;

                // If the object is on the wall, rotate it
                // to the correct wall orientation.
                if (MLPlanesStarterKit.DoesPlaneHaveFlag(plane, (MLPlanes.QueryFlags)SurfaceType.Wall))
                {
                    newObject.transform.rotation = Quaternion.LookRotation(plane.Rotation * Vector3.back);
                }
                // If the object is on the floor, rotate it to face
                // the main camera. This isn't required but it's a nice touch.
                if (MLPlanesStarterKit.DoesPlaneHaveFlag(plane, (MLPlanes.QueryFlags)SurfaceType.Floor))
                {
                    Vector3 lookPos = _mainCamera.transform.position - plane.Center;
                    lookPos.y = 0;
                    Quaternion rotation = Quaternion.LookRotation(lookPos.normalized);
                    newObject.transform.rotation = rotation;
                }
            }
            #endif
        }

        /// <summary>
        /// Destroys all objects that have been placed down by this
        /// component and ensures this component's memory footprint
        /// is minimalized.
        /// </summary>
        private void CleanContent()
        {
            for (int i = 0; i < _placedContent.Count; ++i)
            {
                GameObject.Destroy(_placedContent[i]);
            }
            _placedContent.Clear();
        }

        /// <summary>
        /// Sorts the array of ContentObjects into the _sortedContentObjects
        /// Dictionary based on their semantic flags.
        /// </summary>
        private void SortContent()
        {
            _sortedContentObjects.Add((int)SurfaceType.Wall, new List<ContentObject>());
            _sortedContentObjects.Add((int)SurfaceType.Floor, new List<ContentObject>());
            _sortedContentObjects.Add((int)SurfaceType.Ceiling, new List<ContentObject>());
            for (int i = 0; i < ObjectsToPlace.Length; ++i)
            {
                if ((SurfaceType.Wall & ObjectsToPlace[i].Flags) == SurfaceType.Wall)
                {
                    _sortedContentObjects[(uint)SurfaceType.Wall].Add(ObjectsToPlace[i]);
                }
                if ((SurfaceType.Floor & ObjectsToPlace[i].Flags) == SurfaceType.Floor)
                {
                    _sortedContentObjects[(uint)SurfaceType.Floor].Add(ObjectsToPlace[i]);
                }
                if ((SurfaceType.Ceiling & ObjectsToPlace[i].Flags) == SurfaceType.Ceiling)
                {
                    _sortedContentObjects[(uint)SurfaceType.Ceiling].Add(ObjectsToPlace[i]);
                }
            }
        }

        /// <summary>
        /// Updates objects visibility and position.
        /// </summary>
        void UpdateObject()
        {
            if (_objType != _type)
            {
                if (_obj != null)
                {
                    Destroy(_obj);
                }
            }

            if (_obj == null)
            {
                List<ContentObject> candidates = new List<ContentObject>();
                candidates.AddRange(_sortedContentObjects[(uint)(_type)]);
                int candidateIndex = Random.Range(0, candidates.Count - 1);
                ContentObject prefabReference = candidates[candidateIndex];

                _obj = GameObject.Instantiate(prefabReference.gameObject);
                _objType = _type;

                _currentAngle = 0.0f;
            }

            if ((uint)(_objType) == (uint)(SurfaceType.Wall))
            {
                _obj.transform.rotation = Quaternion.AngleAxis(_currentAngle, Cursor.transform.rotation * Vector3.forward) * Quaternion.LookRotation(Cursor.transform.rotation * Vector3.forward);
            }
            else
            {
                _obj.transform.rotation = Quaternion.AngleAxis(_currentAngle, _obj.transform.up);
            }

            _obj.transform.position = Cursor.transform.position;
        }

        /// <summary>
        /// Updates the preview object rotation based on touchpad position
        /// </summary>
        private void UpdatePad()
        {
            #if PLATFORM_LUMIN
            if (_controller.Touch1Active == true && _controller.CurrentTouchpadGesture.Type == MLInput.Controller.TouchpadGesture.GestureType.RadialScroll)
            {
                _currentAngle = _controller.CurrentTouchpadGesture.Angle * Mathf.Rad2Deg;
            }
            #endif
        }

        /// <summary>
        /// Handles the event for when the controller's Bumper is pressed.
        /// </summary>
        private void HandleOnButtonPressed(byte controllerid, MLInput.Controller.Button button)
        {
            if (button == MLInput.Controller.Button.Bumper)
            {
                if (_firstPlacement == true)
                {
                    PlaceContent();

                    PlanesVisualizer pv = GetComponent<PlanesVisualizer>();
                    if (pv != null)
                    {
                        Destroy(pv);
                    }

                    if (_planes != null)
                    {
                        Destroy(_planes);
                    }

                    _firstPlacement = false;
                    Cursor.gameObject.SetActive(true);
                    StatusLabel.gameObject.SetActive(true);
                }

                else if (_isValid == true)
                {
                    _placedContent.Add(_obj);
                    _obj = null;
                    _isValid = false;
                }
            }
        }
    }
}
