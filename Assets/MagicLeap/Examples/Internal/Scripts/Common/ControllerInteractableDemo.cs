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
    /// Applies a visual effect on a ControllerInteractable object when touched.
    /// </summary>
    [RequireComponent(typeof(ControllerInteractable))]
    public class ControllerInteractableDemo : MonoBehaviour
    {
        [SerializeField, Tooltip("This is the primitive that should be scaled for the selection effect.")]
        private Primitive _primitive = null;

        private ControllerInteractable _interactable = null;

        private void Awake()
        {
            _interactable = GetComponent<ControllerInteractable>();

            // event register
            _interactable.OnHitBegan += HandleOnHitBegan;
            _interactable.OnHitEnded += HandleOnHitEnded;
            _interactable.OnTouchBegan += HandleOnTouchingBegan;
            _interactable.OnTouchEnded += HandleOnTouchEnded;
            _interactable.OnPressed += HandleOnPressed;
            _interactable.OnReleased += HandleOnReleased;
        }

        private void OnDestroy()
        {
            // event unregister
            _interactable.OnHitBegan -= HandleOnHitBegan;
            _interactable.OnHitEnded -= HandleOnHitEnded;
            _interactable.OnTouchBegan -= HandleOnTouchingBegan;
            _interactable.OnTouchEnded -= HandleOnTouchEnded;
            _interactable.OnPressed -= HandleOnPressed;
            _interactable.OnReleased -= HandleOnReleased;
        }

        private void HandleOnHitBegan()
        {
            _primitive.ChangeEmission(new Color(0.106f, 0.447f, 0.576f));
        }

        private void HandleOnHitEnded()
        {
            _primitive.ResetEmission();
        }

        private void HandleOnTouchingBegan()
        {
            _primitive.ChangeEmission(new Color(0.106f, 0.447f, 0.576f));
        }

        private void HandleOnTouchEnded()
        {
            _primitive.ResetEmission();
        }

        private void HandleOnPressed()
        {
            _primitive.Expand();
        }

        private void HandleOnReleased()
        {
            _primitive.Contract();
        }
    }
}
