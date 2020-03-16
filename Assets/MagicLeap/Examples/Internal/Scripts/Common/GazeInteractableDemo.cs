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

namespace MagicLeap
{
    /// <summary>
    /// Adds a visual effect to a GazeInteractable component on all actionable events.
    /// </summary>
    [RequireComponent(typeof(GazeInteractable))]
    public class GazeInteractableDemo : MonoBehaviour
    {
        [SerializeField, Tooltip("This is the primitive that should be scaled for the selection effect.")]
        private Primitive _primitive = null;

        private GazeInteractable _interactable = null;

        /// <summary>
        /// Initializes variables and sets callbacks.
        /// </summary>
        void Awake()
        {
            // references:
            _interactable = GetComponent<GazeInteractable>();

            // Register Events
            _interactable.OnGazeBegan += HandleOnGazeBegan;
            _interactable.OnGazeEnded += HandleOnGazeEnded;
            _interactable.OnGazePressed += HandleOnGazePressed;
            _interactable.OnGazeReleased += HandleOnGazeReleased;
        }

        /// <summary>
        /// Unregister callback handlers
        /// </summary>
        void OnDestroy()
        {
            // Unregister Events
            _interactable.OnGazeBegan -= HandleOnGazeBegan;
            _interactable.OnGazeEnded -= HandleOnGazeEnded;
            _interactable.OnGazePressed -= HandleOnGazePressed;
            _interactable.OnGazeReleased -= HandleOnGazeReleased;
        }

        /// <summary>
        /// When cursor is on the object
        /// </summary>
        private void HandleOnGazeBegan()
        {
            _primitive.ChangeEmission(new Color(0.106f, 0.447f, 0.576f));
        }

        /// <summary>
        /// When cursor is no longer on the object
        /// </summary>
        private void HandleOnGazeEnded()
        {
            _primitive.ResetEmission();
        }

        /// <summary>
        /// When the button in the controller is pressed
        /// </summary>
        private void HandleOnGazePressed()
        {
            _primitive.Expand();
        }

        /// <summary>
        /// When the button in the controller is released
        /// </summary>
        private void HandleOnGazeReleased()
        {
            _primitive.Contract();
        }
    }
}
