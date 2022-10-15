// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Utilities;
#if UNITY_MAGICLEAP || UNITY_ANDROID
using UnityEngine.XR.MagicLeap.Native;
#endif
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XR;


namespace UnityEngine.XR.MagicLeap
{
    public static partial class InputSubsystem
    {
        public static partial class Extensions
        {
            public static partial class Controller
            {
                /// <summary>
                /// Exposed callback for controller <seealso cref="NativeBindings.OnTrigger"/> trigger event.
                /// </summary>
                public static bool AttachTriggerListener(Action<ushort, MLInputControllerTriggerEvent, float> triggerCallback)
                {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                    if (!_callbacksSet)
                    {
                        var result = SetCallbacks();

                        if (result != MLResult.Code.Ok)
                            return false;

                        _callbacksSet = true;
                    }

                    _onTriggerEvent += triggerCallback;
                    return _callbacksSet;
#else
                    return false;
#endif
                }

                public static void RemoveTriggerListener(Action<ushort, MLInputControllerTriggerEvent, float> triggerCallback)
                {
                    _onTriggerEvent -= triggerCallback;
                }

                private static event Action<ushort, MLInputControllerTriggerEvent, float> _onTriggerEvent;

#if UNITY_MAGICLEAP || UNITY_ANDROID
                /// <summary>
                /// Sets the callbacks for controller input events.
                /// </summary>
                private static MLResult SetCallbacks()
                {
                    var callbacks = NativeBindings.MLInputControllerCallbacksEx.Create();
                    var result = NativeBindings.MLInputSetControllerCallbacksEx(MagicLeapXrProviderNativeBindings.GetInputHandle(), ref callbacks,
                        IntPtr.Zero);
                    MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLInputSetControllerCallbacksEx));

                    return MLResult.Create(result);
                }
#endif

                /// <summary>
                /// The maximum number of controller touchpad touches that are supported.
                /// </summary>
                public const uint MaxControllerTouchpadTouches = 2;

                /// <summary>
                /// Flag used to mark whenever callbacks for controller inputs are already set. 
                /// </summary>
                private static bool _callbacksSet;

                /// <summary>
                /// The calibration accuracy levels for controller.
                /// </summary>
                public enum CalibrationAccuracy : uint
                {
                    /// <summary>
                    /// The calibration accuracy is bad.
                    /// </summary>
                    Bad = 0,

                    /// <summary>
                    /// The calibration accuracy is low.
                    /// </summary>
                    Low = 1,

                    /// <summary>
                    /// The calibration accuracy is medium.
                    /// </summary>
                    Medium = 2,

                    /// <summary>
                    /// The calibration accuracy is high.
                    /// </summary>
                    High = 3,
                }


                public enum MLInputControllerType
                {
                    None,
                    Device
                }

                public enum MLInputControllerButton
                {
                    None,
                    Bumper,
                    Menu,
                    Count
                }

                /// <summary>
                ///  Trigger events types.
                /// </summary>
				public enum MLInputControllerTriggerEvent
                {

                    /// <summary>
                    /// This is used when trigger is pulled down, and the normalized value is > 0.
                    /// </summary>
                    Pull,

                    /// <summary>
                    /// /This is used when trigger is fully released, and the normalized value is 0.
                    /// </summary>
                    Release,

                    /// <summary>
                    /// This is used when trigger is pulled and released within a short duration.
                    /// </summary>
                    Click,

                    /// <summary>
                    /// This is used when trigger is pulled and held for a longer duration.
                    /// </summary>
                    Hold
                }

#if UNITY_MAGICLEAP || UNITY_ANDROID
                internal class NativeBindings : MagicLeapNativeBindings
                {
                    /// <summary>
                    /// Links to MLInputConnectedDevicesList in ml_input.h.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLInputControllerState
                    {
                        /// <summary>
                        /// Current touch position (x,y) and force (z). Position is in the [-1.0,1.0] range and force is in the [0.0,1.0] range.
                        /// </summary>
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MaxControllerTouchpadTouches)]
                        public MagicLeapNativeBindings.MLVec3f[] TouchesPositionAndForce;

                        /// <summary>
                        /// Normalized trigger value [0.0,1.0]
                        /// </summary>
                        public float TriggerNormalized;

                        /// <summary>
                        /// Button states.
                        /// </summary>
                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MLInputControllerButton.Count)]
                        public byte[] ButtonStates;

                        [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)MaxControllerTouchpadTouches)]
                        public byte[] IsTouchesActive;

                        public byte IsConnected;

                        public TouchpadGesture.MLInputControllerTouchpadGesture TouchpadGesture;

                        public TouchpadGesture.State TouchpadGestureState;

                        public MLInputControllerType Type;

                        public byte HardwareIndex;
                    }

                    /// <summary>
                    /// Callback structure for <see cref="NativeBindings.OnTouchpadGestureEnd"/>.
                    /// </summary>
                    public delegate void OnTouchpadGestureEndDelegate(ushort controllerId, IntPtr touchpadGesture, IntPtr data);

                    /// <summary>
                    /// Callback structure for <see cref="NativeBindings.OnTouchpadGesture"/>.
                    /// </summary>
                    public delegate void OnTouchpadGestureDelegate(ushort controllerId, IntPtr touchpadGesture, IntPtr data);

                    /// <summary>
                    /// Callback structure for <see cref="NativeBindings.OnTouchpadGestureContinue"/>.
                    /// </summary>
                    public delegate void OnTouchpadGestureContinueDelegate(ushort controllerId, IntPtr touchpadGesture, IntPtr data);

                    /// <summary>
                    /// Callback structure for <see cref="NativeBindings.OnButtonDown"/>.
                    /// </summary>
                    public delegate void OnButtonDownDelegate(ushort controllerId, MLInputControllerButton button, IntPtr data);

                    /// <summary>
                    /// Callback structure for <see cref="NativeBindings.OnButtonUp"/>.
                    /// </summary>
                    public delegate void OnButtonUpDelegate(ushort controllerId, MLInputControllerButton button, IntPtr data);

                    /// <summary>
                    /// Callback structure for <see cref="NativeBindings.OnButtonClick"/>.
                    /// </summary>
                    public delegate void OnButtonClickDelegate(ushort controllerId, MLInputControllerButton button, IntPtr data);

                    /// <summary>
                    /// Callback structure for <see cref="NativeBindings.OnTrigger"/>.
                    /// </summary>
                    public delegate void OnTriggerDelegate(ushort controllerId, MLInputControllerTriggerEvent @event, float depth, IntPtr data);

                    /// <summary>
                    /// Callback structure for <see cref="NativeBindings.OnConnect"/>.
                    /// </summary>
                    public delegate void OnConnectDelegate(ushort controllerId, IntPtr data);

                    /// <summary>
                    /// Callback structure for <see cref="NativeBindings.OnDisconnect"/>.
                    /// </summary>
                    public delegate void OnDisconnectDelegate(ushort controllerId, IntPtr data);


                    /// <summary>
                    /// A structure containing information about the connected devices.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLInputConnectedDevicesList
                    {
                        /// <summary>
                        /// Version of this structure.
                        /// </summary>
                        public uint Version;

                        /// <summary>
                        /// Number of connected controllers.
                        /// </summary>
                        public uint ConnectedControllerCount;

                        /// <summary>
                        /// Pointer to the array of connected controller IDs.
                        /// </summary>
                        public IntPtr ConnectedControllerIds;

                    };

                    /// <summary>
                    /// A structure containing callbacks for input controller events.  The final parameter to all the callbacks is a void *,
                    /// which will point to whatever payload data the user provides in MLInputSetControllerCallbacksEx.  Individual callbacks which
                    /// are not required by the client can be NULL.  This structure must be initialized by calling
                    /// MLInputControllerCallbacksExInit() before use.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLInputControllerCallbacksEx
                    {
                        /// <summary>
                        /// Version of this structure.
                        /// </summary>
                        public uint Version;

                        /// <summary>
                        /// This callback will be invoked whenever a touch gesture is detected.  This callback will be called for both discrete and
                        /// continuous gestures.
                        /// </summary>
                        public OnTouchpadGestureDelegate OnTouchpadGesture;

                        /// <summary>
                        /// This callback will be invoked whenever a continuation of a touch gesture is detected.  This callback will be called only
                        /// for continuous gestures.
                        /// </summary>
                        public OnTouchpadGestureContinueDelegate OnTouchpadGestureContinue;

                        /// <summary>
                        /// This callback will be invoked whenever a continuous touch gesture ends.  This callback will be called only for
                        /// continuous gestures.
                        /// </summary>
                        public OnTouchpadGestureEndDelegate OnTouchpadGestureEnd;

                        /// <summary>
                        /// This callback will be invoked whenever a controller button is pressed.  This callback will be called only for discrete
                        /// gestures.
                        /// </summary>
                        public OnButtonDownDelegate OnButtonDown;

                        /// <summary>
                        /// This callback will be invoked whenever a controller button is released.
                        /// </summary>
                        public OnButtonUpDelegate OnButtonUp;

                        /// <summary>
                        /// This callback will be invoked whenever a controller button is pressed and released within a short duration.
                        /// </summary>
                        public OnButtonClickDelegate OnButtonClick;

                        /// <summary>
                        /// This callback will be invoked whenever a controller trigger state is changed.
                        /// </summary>
                        public OnTriggerDelegate OnTrigger;

                        /// <summary>
                        /// This callback will be invoked whenever a controller is connected.
                        /// </summary>
                        public OnConnectDelegate OnConnect;

                        /// <summary>
                        /// This callback will be invoked whenever a controller is disconnected.
                        /// </summary>
                        public OnDisconnectDelegate OnDisconnect;

                        /// <summary>
                        /// Create and return an initialized version of this struct.
                        /// </summary>
                        public static MLInputControllerCallbacksEx Create()
                        {
                            return new MLInputControllerCallbacksEx
                            {
                                Version = 3,
                                OnTrigger = NativeBindings.OnTrigger
                            };
                        }
                    };

                    /// <summary>
                    /// Sets the callbacks for controller input events.
                    /// </summary>
                    [DllImport(MLInputDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLInputSetControllerCallbacksEx(ulong Handle, ref MLInputControllerCallbacksEx Callbacks,
                        IntPtr UserData);

                    /// <summary>
                    /// Gets the device IDs of all connected devices.
                    /// </summary>
                    [DllImport(MLInputDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLInputGetConnectedDevices(ulong Handle, IntPtr InoutDevices);

                    /// <summary>
                    /// Releases the contents of #MLInputConnectedDevicesList populated by #MLInputGetConnectedDevices.
                    /// </summary>
                    [DllImport(MLInputDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLInputReleaseConnectedDevicesList(ulong Handle, IntPtr Devices);


                    /// <summary>
                    /// This callback will be invoked whenever a controller trigger state is changed.
                    /// </summary>
                    [AOT.MonoPInvokeCallback(typeof(OnTriggerDelegate))]
                    private static void OnTrigger(ushort controllerId, MLInputControllerTriggerEvent triggerEvent, float depth, IntPtr data)
                    {
                        _onTriggerEvent?.Invoke(controllerId, triggerEvent, depth);
                    }
                }
#endif
            }
        }
    }
}
