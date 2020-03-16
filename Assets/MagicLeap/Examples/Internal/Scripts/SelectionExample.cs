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

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;
using MagicLeap.Core;

namespace MagicLeap
{
    public class SelectionExample : MonoBehaviour
    {
        public enum RaycastMode
        {
            Controller,
            Head,
            Eyes
        }

        [SerializeField, Tooltip("Text to print current raycast mode on.")]
        private Text _statusLabel = null;

        [SerializeField, Tooltip("MLRaycastBehavior component for controller")]
        private MLRaycastBehavior _raycastController = null;

        [SerializeField, Tooltip("MLRaycastBehavior component for head")]
        private MLRaycastBehavior _raycastHead = null;

        [SerializeField, Tooltip("MLRaycastBehavior component for eyes")]
        private MLRaycastBehavior _raycastEyes = null;

        private RaycastMode _raycastMode = RaycastMode.Controller;
        private int _modeCount = System.Enum.GetNames(typeof(RaycastMode)).Length;

        /// <summary>
        /// Validate all required components and sets event handlers.
        /// </summary>
        void Start()
        {
            #if PLATFORM_LUMIN
            MLResult result = MLInput.Start();
            if (!result.IsOk)
            {
                Debug.LogError("Error SelectionExample starting MLInput, disabling script.");
                enabled = false;
                return;
            }
            #endif

            if (_statusLabel == null)
            {
                Debug.LogError("SelectionExample._statusLabel is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_raycastController == null)
            {
                Debug.LogError("SelectionExample._raycastController is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_raycastHead == null)
            {
                Debug.LogError("SelectionExample._raycastHead is not set, disabling script.");
                enabled = false;
                return;
            }

            if (_raycastEyes == null)
            {
                Debug.LogError("SelectionExample._raycastEyes is not set, disabling script.");
                enabled = false;
                return;
            }

            #if PLATFORM_LUMIN
            MLInput.OnControllerButtonDown += OnButtonDown;
            #endif

            UpdateRaycastMode();
        }

        /// <summary>
        /// Cleans up the component.
        /// </summary>
        void OnDestroy()
        {
            #if PLATFORM_LUMIN
            MLInput.OnControllerButtonDown -= OnButtonDown;
            MLInput.Stop();
            #endif
        }

        /// <summary>
        /// Updates type of raycast and enables correct cursor.
        /// </summary>
        private void UpdateRaycastMode()
        {
            // Default all objects to inactive and then set active to the appropriate ones.
            _raycastController.gameObject.SetActive(false);
            _raycastController.transform.parent.gameObject.SetActive(false);

            _raycastHead.gameObject.SetActive(false);
            _raycastEyes.gameObject.SetActive(false);

            switch (_raycastMode)
            {
                case RaycastMode.Controller:
                {
                    _raycastController.gameObject.SetActive(true);
                    _raycastController.transform.parent.gameObject.SetActive(true);
                    break;
                }
                case RaycastMode.Head:
                {
                    _raycastHead.gameObject.SetActive(true);
                    break;
                }
                case RaycastMode.Eyes:
                {
                    _raycastEyes.gameObject.SetActive(true);
                    break;
                }
            }
        }

        /// <summary>
        /// Updates Status Label with latest data.
        /// </summary>
        private void UpdateStatusText()
        {
            _statusLabel.text = string.Format("Raycast Mode: {0}", _raycastMode.ToString());
        }

        /// <summary>
        /// Handles the event for button down and cycles the raycast mode.
        /// </summary>
        /// <param name="controller_id">The id of the controller.</param>
        /// <param name="button">The button that is being pressed.</param>
        private void OnButtonDown(byte controller_id, MLInput.Controller.Button button)
        {
            if (button == MLInput.Controller.Button.Bumper)
            {
                _raycastMode = (RaycastMode)((int)(_raycastMode + 1) % _modeCount);
                UpdateRaycastMode();
                UpdateStatusText();
            }
        }
    }
}

