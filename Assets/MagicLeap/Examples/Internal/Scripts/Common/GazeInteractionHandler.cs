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
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// This class handles GazeInteractable events based on input.
    /// </summary>
    public class GazeInteractionHandler : MonoBehaviour
    {
        // Last object hit by raycast
        private Transform _lastHit;
        // Object trigger was pressed on
        private Transform _lastPressed;

        private bool _triggerPressed = false;
        private bool _triggerPreviousPressed = false;

        /// <summary>
        /// Initializes variables.
        /// </summary>
        void Start()
        {
            _lastHit = null;
            _lastPressed = null;

            #if PLATFORM_LUMIN
            MLResult result = MLInput.Start();
            if (!result.IsOk)
            {
                Debug.LogError("Error GazeInteractableHandler starting MLInput, disabling script.");
                enabled = false;
                return;
            }

            MLInput.OnTriggerDown += OnTriggerDown;
            MLInput.OnTriggerUp += OnTriggerUp;
            #endif
        }

        /// <summary>
        /// Cleans up the component.
        /// </summary>
        void OnDestroy()
        {
            #if PLATFORM_LUMIN
            MLInput.OnTriggerUp -= OnTriggerUp;
            MLInput.OnTriggerDown -= OnTriggerDown;
            MLInput.Stop();
            #endif
        }

        /// <summary>
        /// Handles raycast hit collision start on an object
        /// </summary>
        /// <param name="trans"> Transform starting interaction with </param>
        private void InteractionBegin(Transform trans)
        {
            GazeInteractable interactable = trans.GetComponent<GazeInteractable>();

            if (interactable == null)
            {
                return;
            }

            interactable.GazeBegin();
        }

        /// <summary>
        /// Handles raycast hit collision end on an object
        /// </summary>
        /// <param name="trans"> Transform finishing interaction with </param>
        private void InteractionEnd(Transform trans)
        {
            GazeInteractable interactable = trans.GetComponent<GazeInteractable>();

            if (interactable == null)
            {
                return;
            }

            interactable.GazeEnd();
        }

        /// <summary>
        /// Handles controller trigger press behavior on an object
        /// </summary>
        /// <param name="trans"> Transform being pressed </param>
        private void InteractionPress(Transform trans)
        {
            GazeInteractable interactable = trans.GetComponent<GazeInteractable>();

            if (interactable == null)
            {
                return;
            }

            interactable.Press();
        }

        /// <summary>
        /// Handles controller trigger release behavior on an object
        /// </summary>
        /// <param name="trans"> Transform being released </param>
        private void InteractionRelease(Transform trans)
        {
            GazeInteractable interactable = trans.GetComponent<GazeInteractable>();

            if (interactable == null)
            {
                return;
            }

            interactable.Release();
        }

        /// <summary>
        /// Handles controller trigger click behavior on an object
        /// </summary>
        /// <param name="trans"> Transform being clicked </param>
        private void InteractionClick(Transform trans)
        {
            GazeInteractable interactable = trans.GetComponent<GazeInteractable>();

            if (interactable == null)
            {
                return;
            }

            interactable.Click();
        }

        private void HandleInput()
        {
            // Detect the user's press input.
            if (_triggerPressed && !_triggerPreviousPressed)
            {
                InteractionPress(_lastHit);

                _triggerPreviousPressed = true;
                _lastPressed = _lastHit;
            }

            // Detect the user's release input.
            else if (!_triggerPressed && _triggerPreviousPressed)
            {
                InteractionRelease(_lastPressed);

                // Ensure the object was previously pressed and call the click method.
                if (_lastPressed == _lastHit)
                {
                    InteractionClick(_lastHit);
                }

                _triggerPreviousPressed = false;
                _lastPressed = null;
            }
        }

        /// <summary>
        /// Aplies the correct object interactions based on the raycast results.
        /// </summary>
        /// <param name="state"> The state of the raycast result.</param>
        /// <param name="result"> Contains the info on the result of the raycast</param>
        public void OnRaycastHit(MLRaycast.ResultState state, RaycastHit result, float confidence)
        {
            // Detect if raycast hit same object, new object or none
            if (_lastHit != result.transform || _lastHit == null)
            {
                if (_lastHit != null)
                {
                    if (_triggerPressed)
                    {
                        InteractionRelease(_lastPressed);
                        _lastPressed = null;
                        _triggerPreviousPressed = false;
                    }

                    InteractionEnd(_lastHit);
                }

                _lastHit = result.transform;

                if (_lastHit != null)
                {
                    InteractionBegin(_lastHit);
                }
                else
                {
                    return;
                }
            }

            HandleInput();
        }

        /// <summary>
        /// Handles the event for trigger down and turns on _triggerOrAppPressed flag.
        /// </summary>
        /// <param name="controller_id">The id of the controller.</param>
        /// <param name="value">The button that is being pressed.</param>
        private void OnTriggerDown(byte controller_id, float value)
        {
            _triggerPressed = true;
        }

        /// <summary>
        /// Handles the event for trigger up and turns off _triggerOrAppPressed flag.
        /// </summary>
        /// <param name="controller_id"></param>
        /// <param name="value"></param>
        private void OnTriggerUp(byte controller_id, float value)
        {
            _triggerPressed = false;
        }
    }
}
