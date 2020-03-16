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
    /// Receives interaction events from a Gaze object.
    /// </summary>
    [RequireComponent(typeof(Collider))]
    public class GazeInteractable : MonoBehaviour
    {
        public Action OnGazeBegan;
        public Action OnGazeEnded;
        public Action OnGazePressed;
        public Action OnGazeReleased;
        public Action OnGazeClicked;

        /// <summary>
        /// This event is triggered when this object has focus from a GazeSelector.
        /// </summary>
        /// 
        public void GazeBegin()
        {
            // Invoke the inspector events.
            if (OnGazeBegan != null)
            {
                OnGazeBegan();
            }
        }

        /// <summary>
        /// This event is triggered when this object loses focus from a GazeSelector.
        /// </summary>
        public void GazeEnd()
        {
            // Invoke the inspector events.
            if (OnGazeEnded != null)
            {
                OnGazeEnded();
            }
        }

        /// <summary>
        /// This event is triggered when the user input press event occurs while having focus.
        /// </summary>
        public void Press()
        {
            if (OnGazePressed != null)
            {
                OnGazePressed();
            }
        }

        /// <summary>
        /// This event is triggered when the user input release event occurs, focus is not guaranteed.
        /// </summary>
        public void Release()
        {
            if (OnGazeReleased != null)
            {
                OnGazeReleased();
            }
        }

        /// <summary>
        /// This event is triggered when the user input click event occurs while having focus.
        /// </summary>
        public void Click()
        {
            if (OnGazeClicked != null)
            {
                OnGazeClicked();
            }
        }
    }
}
