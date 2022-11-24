// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    public static partial class InputSubsystem
    {
        public static partial class Extensions
        {
            /// <summary>
            /// Try to get the <seealso cref="InputSubsystem.Extensions.MLEyes.State"/> from the given <seealso cref="InputDevice"/>.
            /// </summary>
            /// <param name="eyesDevice">The eye tracking device</param>
            /// <param name="state">The resulting Eye State</param>
            /// <returns>True if the resulting <seealso cref="InputSubsystem.Extensions.MLEyes.State"/> is valid. </returns>
            /// <remarks>
            /// This method expects that a Magic Leap Eye Tracking input device is used in query. (ie. has the EyeTracking <seealso cref="InputDeviceCharacteristics"/>.)
            /// Note, you can retrieve Magic Leap devices with specific characteristics using the <seealso cref="InputSubsystem.Utils.FindMagicLeapDevice"/> method.
            /// Once an eye tracking device is found and <seealso cref="InputDevice.isValid"/> is equal to true, you can get the <seealso cref="MLEyes.State"/> and access device specific features
            /// such as <seealso cref="InputSubsystem.Extensions.MLEyes.State.FixationConfidence"/>. For example:
            /// <code>
            /// InputDevice eyesDevice = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.EyeTracking | InputDeviceCharacteristics.TrackedDevice);
            /// if(eyesDevice.isValid){
            ///     InputSubsystem.Extensions.TryGetEyeTrackingState(eyesDevice, out MLEyes.State trackingState);
            ///     if(trackingState.FixationConfidence){
            ///         Debug.Log("FixationConfidence " + trackingState.FixationConfidence);
            ///     }
            /// }
            /// </code>
            /// </remarks>
            public static bool TryGetEyeTrackingState(InputDevice eyesDevice, out MLEyes.State state) => MLEyes.TryGetState(eyesDevice, out state);

            public static class MLEyes
            {
                /// <summary>
                /// Needs to be called in order to track eyes on the MagicLeap device.
                /// Required to be called only after the user has granted the eye tracking permission.
                /// </summary>
                public static void StartTracking()
                {
#if UNITY_ANDROID
                    MagicLeapXrProviderNativeBindings.StartEyeTracking();
#endif
                }

                public static void StopTracking()
                {
#if UNITY_ANDROID
                    MagicLeapXrProviderNativeBindings.StopEyeTracking();
#endif
                }

                public static bool TryGetState(InputDevice eyesDevice, out State state) => NativeBindings.TryGetState(eyesDevice, out state);

                public readonly struct State
                {
                    /// <summary>
                    ///     A quality metric confidence value 0.0 - 1.0 to indicate accuracy of fixation.
                    /// </summary>
                    public readonly float FixationConfidence;

                    /// <summary>
                    ///     A quality metric confidence value 0.0 - 1.0 to indicate accuracy of left eye center.
                    /// </summary>
                    public readonly float LeftCenterConfidence;

                    /// <summary>
                    ///     A quality metric confidence value 0.0 - 1.0 to indicate accuracy of right
                    ///     eye center.
                    /// </summary>
                    public readonly float RightCenterConfidence;

                    /// <summary>
                    ///     <c> true </c> if left eye is inside a blink. When not wearing the device,
                    ///     values can be arbitrary.
                    /// </summary>
                    public readonly bool LeftBlink;

                    /// <summary>
                    ///     <c> true </c> if right eye is inside a blink. When not wearing the device,
                    ///     values can be arbitrary.
                    /// </summary>
                    public readonly bool RightBlink;

                    /// <summary>
                    ///     Represents what eye tracking error (if any) is present.
                    /// </summary>
                    public readonly bool Error;

                    /// <summary>
                    ///     Timestamp for all the data fields in this struct
                    /// </summary>
                    public readonly MLTime Timestamp;

                    internal State(NativeBindings.MLEyeTrackingStateEx nativeState)
                    {
                        this.FixationConfidence = nativeState.VergenceConfidence;
                        this.LeftCenterConfidence = nativeState.LeftCenterConfidence;
                        this.RightCenterConfidence = nativeState.RightCenterConfidence;
                        this.LeftBlink = nativeState.LeftBlink;
                        this.RightBlink = nativeState.RightBlink;
                        this.Error = nativeState.Error == 1;
                        this.Timestamp = nativeState.Timestamp;
                    }
                }

                internal static class NativeBindings
                {
                    private static byte[] allocatedStateData = new byte[Marshal.SizeOf<NativeBindings.MLEyeTrackingStateEx>()];

                    public static bool TryGetState(InputDevice eyesDevice, out State state)
                    {
                        if (!eyesDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Eyes.TrackingState, allocatedStateData))
                            goto Failure;

                        try
                        {
                            IntPtr ptr = Marshal.AllocHGlobal(allocatedStateData.Length);
                            Marshal.Copy(allocatedStateData, 0, ptr, allocatedStateData.Length);
                            var nativeState = Marshal.PtrToStructure<NativeBindings.MLEyeTrackingStateEx>(ptr);
                            Marshal.FreeHGlobal(ptr);
                            state = new State(nativeState);
                            return true;
                        }

                        catch (Exception e)
                        {
                            Debug.LogError("TryGetTrackingState failed with the exception: " + e);
                            goto Failure;
                        }

                    Failure:
                        state = default;
                        return false;

                    }
                    [StructLayout(LayoutKind.Sequential)]
                    public readonly struct MLEyeTrackingStateEx
                    {
                        /// <summary>
                        ///     Struct version
                        /// </summary>
                        private readonly uint Version;

                        /// <summary>
                        ///     A quality metric confidence value 0.0 - 1.0 to indicate accuracy of fixation.
                        /// </summary>
                        public readonly float VergenceConfidence;

                        /// <summary>
                        ///     A quality metric confidence value 0.0 - 1.0 to indicate accuracy of left eye center.
                        /// </summary>
                        public readonly float LeftCenterConfidence;

                        /// <summary>
                        ///     A quality metric confidence value 0.0 - 1.0 to indicate accuracy of right
                        ///     eye center.
                        /// </summary>
                        public readonly float RightCenterConfidence;

                        /// <summary>
                        ///     <c> true </c> if left eye is inside a blink. When not wearing the device,
                        ///     values can be arbitrary.
                        /// </summary>
                        [MarshalAs(UnmanagedType.I1)]
                        public readonly bool LeftBlink;

                        /// <summary>
                        ///     <c> true </c> if right eye is inside a blink. When not wearing the device,
                        ///     values can be arbitrary.
                        /// </summary>
                        [MarshalAs(UnmanagedType.I1)]
                        public readonly bool RightBlink;

                        /// <summary>
                        ///     Represents what eye tracking error (if any) is present.
                        /// </summary>
                        public readonly uint Error;

                        /// <summary>
                        ///     Timestamp for all the data fields in this struct
                        /// </summary>
                        public readonly long Timestamp;
                    }
                }
            }

        }

    }
}
