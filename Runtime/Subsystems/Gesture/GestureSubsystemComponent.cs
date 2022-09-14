// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using UnityEngine.XR.Management;
using UnityEngine.XR.InteractionSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// <para>
    /// Controls the lifecycle and configuration options for a Magic Leap gesture subsystem. There
    /// is only one active Magic Leap Gestures.  The event callbacks will inform code of when gesture events occur.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    public sealed class GestureSubsystemComponent : MonoBehaviour
    {
        /// <summary>
        /// Get the <c>MagicLeapGestureSubsystem</c> whose lifetime this component manages.
        /// </summary>
        public GestureSubsystem gestureSubsystem { get; private set; }

        /// <summary>
        /// This event is invoked whenever a <see cref="MagicLeapTouchpadGestureEvent"/> is received by the gestures subsystem.
        /// </summary>
        public event Action<GestureSubsystem.Extensions.TouchpadGestureEvent> onTouchpadGestureChanged;

        /// <summary>
        /// This event is invoked whenever a <see cref="ActivateGestureEvent"/> is received by the gestures subsystem.
        /// </summary>
        public event Action<ActivateGestureEvent> onActivate;

        [SerializeField]
        bool m_ControllerGesturesEnabled;

        public bool controllerGesturesEnabled
        {
            get => m_ControllerGesturesEnabled;
        }

        void OnEnable()
        {
            gestureSubsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<XRGestureSubsystem>() as GestureSubsystem;
        }

        void OnDisable()
        {
            gestureSubsystem = null;
        }

        void Update()
        {
            if (gestureSubsystem == null || !gestureSubsystem.running)
                return;

            gestureSubsystem.Update();

            if (onTouchpadGestureChanged != null)
            {
                foreach (var touchpadGestureEvent in gestureSubsystem.touchpadGestureEvents)
                    onTouchpadGestureChanged(touchpadGestureEvent);
            }

            if (onActivate != null)
            {
                foreach (var activateGestureEvent in gestureSubsystem.activateGestureEvents)
                    onActivate(activateGestureEvent);
            }
        }
    }
}
