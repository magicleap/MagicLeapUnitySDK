// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using MagicLeap.OpenXR.LegacySupport;

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

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
                    if (MLDevice.IsOpenXRLoaderActive())
                    {
                        MLEyeTracking.Start();
                    }
                    else
                    {
                        MagicLeapXrProviderNativeBindings.StartEyeTracking();
                    }
                }

                public static void StopTracking()
                {
                    if (MLDevice.IsOpenXRLoaderActive())
                    {
                        MLEyeTracking.Stop();
                    }
                    else
                    {
                        MagicLeapXrProviderNativeBindings.StopEyeTracking();
                    }
                }

                public static bool TryGetState(InputDevice eyesDevice, out State state)
                {
                    if (MLDevice.IsOpenXRLoaderActive())
                    {
                        return MLEyeTracking.TryGetState(out state);
                    }
                    else
                    {
                        return NativeBindings.TryGetTrackingState(eyesDevice, out state);
                    }
                }

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
                    
                    /// <summary>
                    /// Openness of the left eye<br/>
                    /// Output is 0.0 - 1.0, with 0.0 being fully closed and 1.0 fully open.
                    /// </summary>
                    public readonly float LeftEyeOpenness;

                    /// <summary>
                    /// Openness of the right eye<br/>
                    /// Output is 0.0 - 1.0, with 0.0 being fully closed and 1.0 fully open.
                    /// </summary>
                    public readonly float RightEyeOpenness;

                    internal State(NativeBindings.MLEyeTrackingStateEx nativeState)
                    {
                        this.FixationConfidence = nativeState.VergenceConfidence;
                        this.LeftCenterConfidence = nativeState.LeftCenterConfidence;
                        this.RightCenterConfidence = nativeState.RightCenterConfidence;
                        this.LeftBlink = nativeState.LeftBlink;
                        this.RightBlink = nativeState.RightBlink;
                        this.Error = nativeState.Error == 1;
                        this.Timestamp = nativeState.Timestamp;
                        this.LeftEyeOpenness = nativeState.LeftEyeOpenness;
                        this.RightEyeOpenness = nativeState.RightEyeOpenness;
                    }
                }

                public struct StaticData 
                {
                    public MagicLeapNativeBindings.MLCoordinateFrameUID Vergence;

                    public MagicLeapNativeBindings.MLCoordinateFrameUID LeftCenter;

                    public MagicLeapNativeBindings.MLCoordinateFrameUID RightCenter;

                    public Pose GetVergencePose()
                    {
                        MLResult.DidNativeCallSucceed(MagicLeapXrProviderNativeBindings.GetUnityPose(Vergence, out Pose result));
                        return result;
                    }

                    public Pose GetLeftEyeCenterPose()
                    {
                        MLResult.DidNativeCallSucceed(MagicLeapXrProviderNativeBindings.GetUnityPose(LeftCenter, out Pose result));
                        return result;
                    }

                    public Pose GetRightEyeCenterPose()
                    {
                        MLResult.DidNativeCallSucceed(MagicLeapXrProviderNativeBindings.GetUnityPose(RightCenter, out Pose result));
                        return result;
                    }
                }

                public struct DeviceCenteredEyeData
                {
                    public Pose LeftEye;

                    public Pose RightEye;
                      
                    public Pose Vergence;
                }

                public static void GetStaticData(out StaticData staticData)
                {
#if UNITY_OPENXR_1_9_0_OR_NEWER
                    MLEyeTracking.GetStaticData(out staticData);
#else
                    staticData = new();
                    var eyeHandle = MagicLeapXrProviderNativeBindings.GetEyeTrackerHandle();
                    if (!MagicLeapNativeBindings.MLHandleIsValid(eyeHandle))
                    {
                        Debug.LogError("Eye handle invalid");
                        return;
                    }

                    NativeBindings.MLEyeTrackingStaticData data = new();
                    var result = MLResult.Create(NativeBindings.MLEyeTrackingGetStaticData(eyeHandle, ref data));
                    if (!result.IsOk)
                    {
                        Debug.LogError($"GetStaticData failed: {result}");
                        return;
                    }

                    staticData.Vergence = data.vergence;
                    staticData.LeftCenter = data.left_center;
                    staticData.RightCenter = data.right_center;
                    return;
#endif
                }

                public static void GetEyeDataInDeviceCoords(out DeviceCenteredEyeData deviceCenteredEyeData)
                {
                    deviceCenteredEyeData = new();
                    MagicLeapNativeBindings.MLSnapshotStaticData snapshotStaticData = MagicLeapNativeBindings.MLSnapshotStaticData.Init();
                    IntPtr snap = IntPtr.Zero;

                    MLResult result;
                    result = MLResult.Create(MagicLeapNativeBindings.MLSnapshotGetStaticData(ref snapshotStaticData));
                    if (!result.IsOk)
                    {
                        Debug.LogError($"MLEyes::MLSnapshotGetStaticData failed: {result}");
                        return;
                    }

                    result = MLResult.Create(MagicLeapNativeBindings.MLPerceptionGetSnapshot(ref snap));
                    if (!result.IsOk)
                    {
                        Debug.LogError($"MLEyes::MLPerceptionGetSnapshot failed: {result}");
                        return;
                    }

                    MagicLeapNativeBindings.MLPose leftEyeCenter = new();
                    MagicLeapNativeBindings.MLPose rightEyeCenter = new();
                    MagicLeapNativeBindings.MLTransform head = new();
                    MagicLeapNativeBindings.MLTransform vergence = new();

                    MLHeadTracking.GetStaticData(out MagicLeapNativeBindings.MLCoordinateFrameUID headUID);

                    result = MLResult.Create(MagicLeapNativeBindings.MLSnapshotGetTransform(snap, ref headUID, ref head));
                    if (!result.IsOk)
                    {
                        Debug.LogError($"MLEyes::MLSnapshotGetTransform (Device UID) failed: {result}");
                        return;
                    }

                    StaticData eyeStaticData;
                    GetStaticData(out eyeStaticData);

                    result = MLResult.Create(MagicLeapNativeBindings.MLSnapshotGetTransform(snap, ref eyeStaticData.Vergence, ref vergence));
                    if (!result.IsOk)
                    {
                        Debug.LogError($"MLEyes::MLSnapshotGetTransform (vergence UID) failed: {result}");
                        return;
                    }

                    result = MLResult.Create(MagicLeapNativeBindings.MLSnapshotGetPoseInBase(snap, ref headUID, ref eyeStaticData.LeftCenter, ref leftEyeCenter));
                    if (!result.IsOk)
                    {
                        Debug.LogError($"MLEyes::MLSnapshotGetPoseInBase(leftEye) failed: {result}");
                        return;
                    }

                    result = MLResult.Create(MagicLeapNativeBindings.MLSnapshotGetPoseInBase(snap, ref headUID, ref eyeStaticData.RightCenter, ref rightEyeCenter));
                    if (!result.IsOk)
                    {
                        Debug.LogError($"MLEyes::MLSnapshotGetPoseInBase(rightEye) failed: {result}");
                        return;
                    }

                    deviceCenteredEyeData.LeftEye = new Pose(MLConvert.ToUnity(leftEyeCenter.Transform.Position), MLConvert.ToUnity(leftEyeCenter.Transform.Rotation));
                    deviceCenteredEyeData.RightEye = new Pose(MLConvert.ToUnity(rightEyeCenter.Transform.Position), MLConvert.ToUnity(rightEyeCenter.Transform.Rotation));
                    deviceCenteredEyeData.Vergence = new Pose(MLConvert.ToUnity(vergence.Position), MLConvert.ToUnity(vergence.Rotation));

                    MagicLeapNativeBindings.MLPerceptionReleaseSnapshot(snap);
                }

                internal static class NativeBindings
                {
                    private static byte[] allocatedStateData = new byte[Marshal.SizeOf<NativeBindings.MLEyeTrackingStateEx>()];

                    public static bool TryGetTrackingState(InputDevice eyesDevice, out State state)
                    {
                        if (!eyesDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Eyes.TrackingState, allocatedStateData))
                        {
                            state = default;
                            return false;
                        }

                        IntPtr ptr = Marshal.AllocHGlobal(allocatedStateData.Length);
                        Marshal.Copy(allocatedStateData, 0, ptr, allocatedStateData.Length);
                        var nativeState = Marshal.PtrToStructure<NativeBindings.MLEyeTrackingStateEx>(ptr);
                        Marshal.FreeHGlobal(ptr);
                        state = new State(nativeState);
                        return true;
                    }
                    
                    [StructLayout(LayoutKind.Sequential)]
                    public readonly struct MLEyeTrackingStateEx
                    {
                        public static MLEyeTrackingStateEx Init() => new (version: 2);
                        
                        /// <summary>
                        ///     Struct version
                        /// </summary>
                        public readonly uint Version;

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

                        /// <summary>
                        /// Openness of the left eye<br/>
                        /// Output is 0.0 - 1.0, with 0.0 being fully closed and 1.0 fully open.
                        /// </summary>
                        public readonly float LeftEyeOpenness;

                        /// <summary>
                        /// Openness of the right eye<br/>
                        /// Output is 0.0 - 1.0, with 0.0 being fully closed and 1.0 fully open.
                        /// </summary>
                        public readonly float RightEyeOpenness;

                        private MLEyeTrackingStateEx(uint version)
                        {
                            Version = version;
                            #region state defaults
                            VergenceConfidence = 0;
                            LeftCenterConfidence = 0;
                            RightCenterConfidence = 0;
                            LeftBlink = false;
                            RightBlink = false;
                            Error = 0;
                            Timestamp = 0;
                            LeftEyeOpenness = 0;
                            RightEyeOpenness = 0;
                            #endregion
                        }
                    }

                    /// <summary>
                    /// Gets static information about the eye tracker
                    /// </summary>
                    /// <param name="handle"> A handle to an Eye Tracker retrievced from GetEyeTrackerHandle()</param>
                    /// <param name="outData"> Target to populate the data about the eye tracker</param>
                    /// <returns>
                    /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if the out_data param was not valid (null).
                    /// MLResult.Result will be <c>MLResult.Code.Ok</c> if eye tracking static data was successfully received.
                    /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to retrieve eye tracking static data.
                    /// </returns>
                    [DllImport(MagicLeapNativeBindings.MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLEyeTrackingGetStaticData(ulong handle, ref MLEyeTrackingStaticData outData);

                    /// <summary>
                    /// Static information about the eye tracking. Populate with MLEyeTrackingGetStaticData
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLEyeTrackingStaticData
                    {
                        /// <summary>
                        /// Location of the 3D vergence point.
                        /// </summary>
                        public MagicLeapNativeBindings.MLCoordinateFrameUID vergence;

                        /// <summary>
                        /// Left center of the eye.
                        /// </summary>
                        public MagicLeapNativeBindings.MLCoordinateFrameUID left_center;

                        /// <summary>
                        /// Right center of the eye.
                        /// </summary>
                        public MagicLeapNativeBindings.MLCoordinateFrameUID right_center;
                    }
                }
            }
        }
    }
}
