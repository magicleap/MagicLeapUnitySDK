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

using System.Threading.Tasks;
#if PLATFORM_LUMIN
using static UnityEngine.XR.MagicLeap.MLWebRTC.VideoSink.Frame;
#endif

namespace UnityEngine.XR.MagicLeap
{
    public class MLCameraVideoSource : MLWebRTC.AppDefinedVideoSource
    {
#if PLATFORM_LUMIN
        private CircularBuffer<ImagePlane[]> imagePlanesBuffer = CircularBuffer<ImagePlane[]>.Create(new ImagePlane[(int)NativeImagePlanesLength.YUV_420_888], new ImagePlane[(int)NativeImagePlanesLength.YUV_420_888], new ImagePlane[(int)NativeImagePlanesLength.YUV_420_888]);

        private MLCamera.CaptureSettings captureSettings;

        private bool isCapturing = false;

        private MLCameraVideoSource(MLCamera.CaptureSettings settings, string trackId)
            : base(trackId)
        {
            this.captureSettings = settings;
        }

        public static MLCameraVideoSource CreateLocal(MLCamera.CaptureSettings settings, out MLResult result, string trackId = "")
        {
            MLCameraVideoSource mlCameraVideoSource = new MLCameraVideoSource(settings, trackId);

            result = MLWebRTC.AppDefinedVideoSource.InitializeLocal(mlCameraVideoSource);
            mlCameraVideoSource.StartCapture();
            return mlCameraVideoSource;
        }
#endif

        private MLCameraVideoSource()
            : base(string.Empty)
        {
        }

        protected override void OnSourceSetEnabled(bool enabled)
        {
            if(enabled)
            {
                this.StartCapture();
            }
            else
            {
                this.StopCapture();
            }
        }

        protected override void OnSourceDestroy()
        {
            this.StopCapture();
        }

        private void StartCapture()
        {
#if PLATFORM_LUMIN
            if (!isCapturing)
            {
                MLPrivileges.RequestPrivileges(MLPrivileges.Id.CameraCapture);
                MLCamera.Connect();
                MLCamera.PrepareCapture(MLCamera.CaptureType.VideoRaw, ref this.captureSettings);
                MLCamera.OnRawVideoFrameAvailableYUV_NativeCallbackThread += OnMLCameraFrameYUV;
                MLCamera.StartRawVideoCapture();
                isCapturing = true;
            }
#endif
        }

        private void StopCapture()
        {
#if PLATFORM_LUMIN
            if (isCapturing)
            {
                if (MLCamera.IsStarted)
                {
                    MLCamera.StopVideoCapture();
                    MLCamera.Disconnect();
                }
                MLCamera.OnRawVideoFrameAvailableYUV_NativeCallbackThread -= OnMLCameraFrameYUV;
                isCapturing = false;
            }
#endif
        }

#if PLATFORM_LUMIN
        private void OnMLCameraFrameYUV(MLCamera.ResultExtras results, MLCamera.YUVFrameInfo frameInfo, MLCamera.FrameMetadata metadata)
        {
            PushYUVFrame(results, frameInfo, metadata);
        }

        private void PushYUVFrame(MLCamera.ResultExtras results, MLCamera.YUVFrameInfo frameInfo, MLCamera.FrameMetadata metadata)
        {
            MLCamera.YUVBuffer buffer;
            MLWebRTC.VideoSink.Frame.ImagePlane[] imagePlaneArray = imagePlanesBuffer.Get();
            for (int i = 0; i < imagePlaneArray.Length; ++i)
            {
                switch (i)
                {
                    case 0:
                    {
                        buffer = frameInfo.Y;
                        break;
                    }
                    case 1:
                    {
                        buffer = frameInfo.U;
                        break;
                    }
                    case 2:
                    {
                        buffer = frameInfo.V;
                        break;
                    }
                    default:
                    {
                        buffer = new MLCamera.YUVBuffer();
                        break;
                    }
                }

                imagePlaneArray[i] = MLWebRTC.VideoSink.Frame.ImagePlane.Create(buffer.Width, buffer.Height, buffer.Stride, buffer.BytesPerPixel, buffer.Size, buffer.DataPtr);
            }

            MLWebRTC.VideoSink.Frame frame = MLWebRTC.VideoSink.Frame.Create((ulong)results.RequestId, results.VcamTimestampUs, imagePlaneArray, MLWebRTC.VideoSink.Frame.OutputFormat.YUV_420_888);

            _ = this.PushFrameAsync(frame);
        }
#endif
    }
}
