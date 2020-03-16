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

using System;
using UnityEngine;
using System.Collections;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap
{
    /// <summary>
    /// Class that handles triggering events for visual effects based on
    /// collision of assigned object with virtual objects
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class CollisionInteractabilityHandler : MonoBehaviour
    {
        private ControllerInteractable _hit;
        private ControllerInteractable _lastHit;

        private Collider _collider;

        #if PLATFORM_LUMIN
        private MLInput.Controller _controller;
        #endif

        private bool _isTriggerPressed = false;
        private bool _isHitting = false;

        /// <summary>
        /// Initializes variables.
        /// </summary>
        void Start()
        {
            #if PLATFORM_LUMIN
            MLResult result = MLInput.Start();
            if (!result.IsOk)
            {
                Debug.LogError("Error starting MLInput, disabling script.");
                enabled = false;
                return;
            }
            _controller = MLInput.GetController(MLInput.Hand.Left);
            #endif

            _collider = GetComponent<Collider>();
        }

        /// <summary>
        /// Stop MLInput.
        /// </summary>
        private void OnDestroy()
        {
            #if PLATFORM_LUMIN
            MLInput.Stop();
            #endif
        }

        /// <summary>
        /// Handle Trigger press
        /// </summary>
        void Update()
        {
            // Capture the current trigger value.
            bool pressed = false;

            #if PLATFORM_LUMIN
            pressed = _controller.TriggerValue > 0.8f;
            #endif

            if (!_isTriggerPressed && pressed)
            {
                _isTriggerPressed = true;

                // If an object is currently within the Controller trigger.
                // State: Selection
                if (_lastHit != null)
                {
                    // Notify the last ControllerInteractable
                    _lastHit.Pressed();
                }
                else if (!_isHitting)
                {
                    // If an object is currently not within the Controller trigger.
                    // State: Collision
                    _isHitting = true;

                    // Enable Collision
                    _collider.isTrigger = false;
                }
            }
            else if (_isTriggerPressed && !pressed)
            {
                _isTriggerPressed = false;

                if (_lastHit != null)
                {
                    // Notify the last ControllerInteractable
                    _lastHit.Release();
                }
                else if (_isHitting)
                {
                    _isHitting = false;

                    // Disable Collision
                    _collider.isTrigger = true;
                }
            }
        }

        /// <summary>
        /// Handles the event for when the trigger enters.
        /// </summary>
        /// <param name="collider">The collider that entered the trigger.</param>
        private void OnTriggerEnter(Collider collider)
        {
            ControllerInteractable interactable = collider.GetComponent<ControllerInteractable>();
            if (interactable != null)
            {
                _hit = interactable;
                _hit.Touching(true);

                if (_lastHit != _hit)
                {
                    _lastHit = _hit;
                }
            }
        }

        /// <summary>
        /// Handles the event for when the trigger exits.
        /// </summary>
        /// <param name="collider">The collider that exited the trigger.</param>
        private void OnTriggerExit(Collider collider)
        {
            ControllerInteractable interactable = collider.GetComponent<ControllerInteractable>();
            if (interactable != null)
            {
                _hit = interactable;
                _hit.Touching(false);

                if (_lastHit != null)
                {
                    _lastHit.Release();
                }

                if (_lastHit == _hit)
                {
                    _lastHit = null;
                }
            }
        }

        /// <summary>
        /// Handles the event for when a collision occurs.
        /// </summary>
        /// <param name="collision">The collision information.</param>
        private void OnCollisionEnter(Collision collision)
        {
            ControllerInteractable interactable = collision.gameObject.GetComponent<ControllerInteractable>();
            if (interactable != null)
            {
                interactable.Hit();
            }
        }

        /// <summary>
        /// Handles the event for when a collision stops.
        /// </summary>
        /// <param name="collision">The collision information.</param>
        private void OnCollisionExit(Collision collision)
        {
            ControllerInteractable interactable = collision.gameObject.GetComponent<ControllerInteractable>();
            if (interactable != null)
            {
                interactable.OnHitEnded();
            }
        }
    }
}
