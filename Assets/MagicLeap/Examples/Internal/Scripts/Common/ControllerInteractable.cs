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

namespace MagicLeap
{
    /// <summary>
    /// Allows a Controller component to interact, manipulate, and optionally throw this GameObject.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    public class ControllerInteractable : MonoBehaviour
    {
        private Rigidbody _rigidbody;

        public Action OnHitBegan;
        public Action OnHitEnded;
        public Action OnTouchBegan;
        public Action OnTouchEnded;
        public Action OnPressed;
        public Action OnReleased;

        /// <summary>
        /// Rigidbody gettor.
        /// </summary>
        public Rigidbody Rigidbody
        {
            get
            {
                return _rigidbody;
            }
        }

        /// <summary>
        /// Initializes variables
        /// </summary>
        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.useGravity = false;
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
        }

        /// <summary>
        /// Invokes the OnHit event.
        /// </summary>
        public void Hit()
        {
            if (OnHitBegan != null)
            {
                OnHitBegan();
            }
        }

        /// <summary>
        /// Invokes the OnHitEnded event.
        /// </summary>
        public void HitEnded()
        {
            if (OnHitEnded != null)
            {
                OnHitEnded();
            }
        }

        /// <summary>
        /// Updates the touch status of the ControllerInteractable and invokes the OnTouchBegan/OnTouchEnded events.
        /// </summary>
        /// <param name="active">Whether or not the user is actively touching this object.</param>
        public void Touching(bool active)
        {
            if (active)
            {
                if (OnTouchBegan != null)
                {
                    OnTouchBegan();
                }
            }
            else
            {
                if (OnTouchEnded != null)
                {
                    OnTouchEnded();
                }
            }
        }

        /// <summary>
        /// Grab and parent the ControllerInteractable also invokes the OnPressed event.
        /// </summary>
        public void Pressed()
        {
            if (OnPressed != null)
            {
                // Invoke the OnPressed event.
                OnPressed();
            }
        }

        /// <summary>
        /// Release the ControllerInteractable and invoke the OnRelease event.
        /// </summary>
        /// <param name="velocity">The velocity to apply to this object's rigidbody</param>
        public void Release()
        {
            if (OnReleased != null)
            {
                OnReleased();
            }
        }
    }
}
