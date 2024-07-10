// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLWorldCamera.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disabling deprecated warning for the internal project
#pragma warning disable 618

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Magic Leap 2 has three world cameras which it uses for environment tracking.
    /// The three cameras area located on the left, center, and right side of the
    /// headset. This API will provide a way to query for the frames from these world
    /// cameras, at this point the app will not be able to configure the world camera parameters.
    /// 
    /// This is an experimental API which may be modified or removed without
    /// any prior notice.
    /// </summary>
    public partial class MLWorldCamera : MLAutoAPISingleton<MLWorldCamera>
    {
        /// <summary>
        /// Enumeration of all the available world camera sensors.
        /// </summary>
        [Flags]
        public enum CameraId
        {
            /// <summary>
            ///  Left World camera.
            /// </summary>
            Left = 1 << 0,

            /// <summary>
            ///  Right World camera.
            /// </summary>
            Right = 1 << 1,

            /// <summary>
            ///  Center World camera.
            /// </summary>
            Center = 1 << 2,

            /// <summary>
            ///  All World cameras.
            /// </summary>
            All = Left | Right | Center,
        };

        /// <summary>
        /// Enumeration of world camera modes.
        /// </summary>
        [Flags]
        public enum Mode
        {
            /// <summary>
            ///  None.
            /// </summary>
            Unknown = 0,

            /// <summary>
            /// Low exposure mode.
            /// This mode is currently only available when the controller is being tracked.
            /// </summary>
            LowExposure = 1 << 0,

            /// <summary>
            /// Normal exposure mode.
            /// </summary>
            NormalExposure = 1 << 1
        };

        public Settings CurrentSettings { get; private set; }

        public bool IsConnected { get; private set; }

        private bool connectionPaused;

        protected override MLResult.Code StartAPI() => MLResult.Code.Ok;

        protected override MLResult.Code StopAPI() => Disconnect().Result;

        /// <summary>
        /// Connect to world cameras.
        /// </summary>
        public MLResult Connect(in Settings settings) => MLResult.Create(InternalConnect(in settings));

        /// <summary>
        /// Update the world camera settings.
        /// </summary>
        public MLResult UpdateSettings(in Settings settings) => MLResult.Create(InternalUpdateSettings(in settings));

        /// <summary>
        /// Poll for Frames.  Returns #MLWorldCameraData with this latest data when available.  The memory is owned by the system.
        /// Application should copy the data it needs to cache it and then release the memory by calling
        /// #MLWorldCameraReleaseCameraData.  This is a blocking call.  API is not thread safe.  If there are no new world camera data frames for a given
        /// duration (duration determined by the system) then the API will return MLResult_Timeout.  To Do : Are there any other
        /// meaningful return codes that we need to consider.  Say something like MLResult_ResourceNotAvailble for cases where the world
        /// camera is not ready yet or is not generating any data because its been turned off.
        /// </summary>
        public MLResult GetLatestWorldCameraData(out Frame[] data, uint timeOutMs = 1) => MLResult.Create(InternalGetLatestWorldCameraData(timeOutMs, out data));

        /// <summary>
        /// Disconnect from world camera.  This will disconnect from all the world camera currently connected.
        /// </summary>
        public MLResult Disconnect() => MLResult.Create(InternalDisconnect());

        /// <summary>
        /// Connect to world cameras.
        /// </summary>
        private MLResult.Code InternalConnect(in Settings settings)
        {
            if (IsConnected)
                return MLResult.Code.Ok;

            var nativeSettings = new NativeBindings.MLWorldCameraSettings(in settings);
            var resultCode = NativeBindings.MLWorldCameraConnect(in nativeSettings, out Handle);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWorldCameraConnect)))
            {
                IsConnected = true;
                CurrentSettings = settings;
            }
            return resultCode;
        }

        /// <summary>
        /// Update the world camera settings.
        /// </summary>
        private MLResult.Code InternalUpdateSettings(in Settings settings)
        {
            var nativeSettings = new NativeBindings.MLWorldCameraSettings(in settings);
            var resultCode = NativeBindings.MLWorldCameraUpdateSettings(Handle, in nativeSettings);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWorldCameraUpdateSettings)))
                CurrentSettings = settings;

            return resultCode;
        }

        /// <summary>
        /// Poll for Frames.  Returns #MLWorldCameraData with this latest data when available.  The memory is owned by the system.
        /// Application should copy the data it needs to cache it and then release the memory by calling
        /// #MLWorldCameraReleaseCameraData.  This is a blocking call.  API is not thread safe.  If there are no new world camera data frames for a given
        /// duration (duration determined by the system) then the API will return MLResult_Timeout.  To Do : Are there any other
        /// meaningful return codes that we need to consider.  Say something like MLResult_ResourceNotAvailble for cases where the world
        /// camera is not ready yet or is not generating any data because its been turned off.
        /// </summary>
        private MLResult.Code InternalGetLatestWorldCameraData(uint timeOutMs, out Frame[] data)
        {
            var nativeData = new NativeBindings.MLWorldCameraData(1);
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(nativeData));
            Marshal.StructureToPtr(nativeData, ptr, false);
            var resultCode = NativeBindings.MLWorldCameraGetLatestWorldCameraData(Handle, timeOutMs, ref ptr);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWorldCameraGetLatestWorldCameraData), TimeOutPredicate);
            if (!MLResult.IsOK(resultCode))
            {
                data = new Frame[0];
                return resultCode;
            }

            nativeData = Marshal.PtrToStructure<NativeBindings.MLWorldCameraData>(ptr);
            data = MarshalFrames(in nativeData);
            resultCode = NativeBindings.MLWorldCameraReleaseCameraData(Handle, ptr);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWorldCameraReleaseCameraData));
            Marshal.FreeHGlobal(ptr);

            return resultCode;
        }

        /// <summary>
        /// Disconnect from world camera.  This will disconnect from all the world camera currently connected.
        /// </summary>
        private MLResult.Code InternalDisconnect()
        {
            if (!IsConnected)
                return MLResult.Code.Ok;

            var resultCode = NativeBindings.MLWorldCameraDisconnect(Handle);
            IsConnected = !MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLWorldCameraDisconnect));
            return resultCode;
        }

        private Frame[] MarshalFrames(in NativeBindings.MLWorldCameraData data)
        {
            var frames = new Frame[data.FrameCount];
            IntPtr walkPtr = data.Frames;
            for (int i = 0; i < data.FrameCount; ++i)
            {
                var nativeFrame = Marshal.PtrToStructure<NativeBindings.MLWorldCameraFrame>(walkPtr);
                frames[i] = new Frame(nativeFrame);
                walkPtr = new IntPtr(walkPtr.ToInt64() + Marshal.SizeOf<NativeBindings.MLWorldCameraFrame>());
            }

            return frames;
        }

        private bool TimeOutPredicate(MLResult.Code code) => code == MLResult.Code.Ok || code == MLResult.Code.Timeout;

        protected override void OnApplicationPause(bool pauseStatus)
        {
            base.OnApplicationPause(pauseStatus);

            if (pauseStatus)
            {
                if (IsConnected)
                {
                    connectionPaused = true;
                    Disconnect();
                }
            }

            else
            {
                if (connectionPaused)
                {
                    connectionPaused = false;
                    if (MLPermissions.CheckPermission(MLPermission.Camera).IsOk)
                    {
                        var settings = CurrentSettings;
                        Connect(in settings);
                    }
                }
            }
        }
    }
}

