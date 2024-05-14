// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using Unity.Collections;

namespace MagicLeap.Android
{
    using System;
    using System.Threading;

    using NDK.Camera;
    using NDK.Media;

    using UnityEngine;
    using UnityEngine.XR.MagicLeap;

    using Metadata = NDK.Camera.Metadata;

    public static class AndroidCameraManager
    {
        private class TemporaryManagerScope : IDisposable
        {
            private ACameraManager _cameraManager;

            public ACameraManager CameraManager => _cameraManager;

            public TemporaryManagerScope()
            {
                _cameraManager = GetOrCreateManager();
            }

            public void Dispose()
            {
                ReleaseManager();
                _cameraManager = default;
            }

            public static implicit operator ACameraManager(TemporaryManagerScope scope)
                => scope._cameraManager;
        }

        private const int kMaxImages = 4;

        private static string[] s_CameraIdList = null;
        private static ACameraManager s_Manager = default;
        private static int s_RefCount = 0;

        public static string[] GetCameraIdList()
        {
            if (s_CameraIdList != null)
                return s_CameraIdList;

            using var scope = new TemporaryManagerScope();
            var mgr = scope.CameraManager;
            if (!mgr.TryGetCameraIds(out var cameraIdListPtr))
                throw new Exception("failed to retrieve camera ids from camera manager");

            using (cameraIdListPtr)
            {
                var cameraList = new string[cameraIdListPtr.NumCameras];
                for (var i = 0; i < cameraList.Length; ++i)
                    cameraList[i] = cameraIdListPtr.CameraAt(i);
                s_CameraIdList = cameraList;
            }

            return s_CameraIdList;
        }



        public static bool TryCreateAndroidCamera(string cameraId, StreamConfiguration config, out AndroidCamera outCamera)
        {

            outCamera = null;

            if (string.IsNullOrEmpty(cameraId))
                throw new ArgumentNullException(nameof(cameraId));

            using var scope = new TemporaryManagerScope();
            ACameraManager manager = scope;

            if (!manager.TryOpenCamera(cameraId, out var cameraPtr))
                return false;

            if (!AImageReader.TryCreate(config.Width, config.Height, config.Format, kMaxImages, out var imageReaderPtr))
                goto cleanupCameraPtr;

            outCamera = new AndroidCamera(cameraPtr, imageReaderPtr);

            return true;

            cleanupCameraPtr:
            {
                cameraPtr.Dispose();
            }

            return false;
        }

        public static bool TryGetAvailableStreamConfigurations(string cameraId, out NativeArray<StreamConfiguration> outConfigs)
        {
            outConfigs = default;

            if (!TryGetCameraMetadata(cameraId, out var metadata))
                return false;

            if (!metadata.TryGetConstEntry(Metadata.Tags.ACAMERA_SCALER_AVAILABLE_STREAM_CONFIGURATIONS, out var entry))
                return false;

            var outputConfigCount = 0;
            unsafe
            {
                for (var i = 0; i < entry.Count; i += 4)
                {
                    if (entry.Data.I32[i + 3] == 0)
                        outputConfigCount++;
                }
            }

            outConfigs = new NativeArray<StreamConfiguration>(outputConfigCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            int lastIdx = 0;

            unsafe
            {
                for (var i = 0; i < entry.Count; i += 4)
                {
                    if (entry.Data.I32[i + 3] != 0)
                        continue;
                    MediaFormat format = (MediaFormat)entry.Data.I32[i + 0];
                    int width = entry.Data.I32[i + 1];
                    int height = entry.Data.I32[i + 2];

                    outConfigs[lastIdx++] = new StreamConfiguration(format, width, height);
                }
            }

            return true;
        }

        public static bool TryGetCameraDimensions(string cameraId, out int width, out int height)
        {
            width = 0;
            height = 0;

            if (string.IsNullOrEmpty(cameraId))
                throw new ArgumentNullException(nameof(cameraId));

            using var scope = new TemporaryManagerScope();
            ACameraManager manager = scope;

            if (!manager.TryGetCameraMetadata(cameraId, out var metadata))
                return false;

            return TryGetCameraDimensionsInternal(metadata, out width, out height);
        }

        internal static bool TryGetCameraDimensionsInternal(ACameraMetadata metadata, out int width, out int height)
        {
            width = 0;
            height = 0;

            if (!metadata.TryGetConstEntry(Metadata.Tags.ACAMERA_SCALER_AVAILABLE_STREAM_CONFIGURATIONS, out var entry))
                return false;

            unsafe
            {
                for (int i = 0; i < entry.Count; i += 4)
                {
                    // only interested in output streams.
                    int input = entry.Data.I32[i + 3];
                    if (input != 0)
                        continue;

                    // only interested in PRIVATE data format.
                    MediaFormat format = (MediaFormat)entry.Data.I32[i + 0];
                    if (format == MediaFormat.Private)
                    {
                        width = entry.Data.I32[i + 1];
                        height = entry.Data.I32[i + 2];

                        return true;
                    }
                }
            }

            return false;
        }

        public static bool TryGetCameraMetadata(string cameraId, out ACameraMetadata outMetadata)
        {
            outMetadata = default;

            if (string.IsNullOrEmpty(cameraId))
                throw new ArgumentNullException(nameof(cameraId));

            using var scope = new TemporaryManagerScope();
            ACameraManager manager = scope;

            return manager.TryGetCameraMetadata(cameraId, out outMetadata);
        }

        internal static ACameraManager GetOrCreateManager()
        {
            var refCount = Interlocked.Increment(ref s_RefCount);
            if (refCount == 1)
                s_Manager = ACameraManager.Create();

            return s_Manager;
        }

        internal static void ReleaseManager()
        {
            var refCount = Interlocked.Decrement(ref s_RefCount);
            if (refCount == 0)
                s_Manager.Dispose();
        }
    }

    internal sealed class AndroidCameraLog : MLPluginLog.ScopedLog
    {
        public AndroidCameraLog(string scopeName, bool showStackTrace = false)
        : base($"AndroidCamera::{scopeName}", showStackTrace)
        { }

        protected override void LogInternal(string message, LogType logType = LogType.Log)
        {
#if ENABLE_ML_CAMERA_TRACING
            base.LogInternal(message, logType);
#endif
        }

        public static void LogOnce(string scope, string message, LogType logType = LogType.Log, bool showStackTrace = false)
        {
#if ENABLE_ML_CAMERA_TRACING
            UnityEngine.Debug.LogFormat(logType, showStackTrace ? LogOption.None : LogOption.NoStacktrace, null, $"[{scope}]: {message}");
#endif
        }
    }
}
