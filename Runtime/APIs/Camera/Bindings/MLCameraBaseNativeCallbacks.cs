// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLCameraBase
    {
        internal partial class NativeBindings
        {
            /// <summary> 
            /// Helper funcction for marshaling the intrinsic calibration parameters. 
            /// </summary>
            private static MLCamera.IntrinsicCalibrationParameters? CreateIntrinsicParameters(IntPtr intrinsicsPointer)
            {
                if (intrinsicsPointer != IntPtr.Zero)
                {
                    MLCameraIntrinsicCalibrationParameters intrinsics = Marshal.PtrToStructure<MLCameraIntrinsicCalibrationParameters>(intrinsicsPointer);
                    return new MLCamera.IntrinsicCalibrationParameters(intrinsics.Width, intrinsics.Height, MLConvert.ToUnity(intrinsics.FocalLength), MLConvert.ToUnity(intrinsics.PrincipalPoint), intrinsics.FOV, intrinsics.Distortion);
                }
                return null;
            }

            [AOT.MonoPInvokeCallback(typeof(DeviceAvailabilityStatusDelegate))]
            private static void OnDeviceAvailableCallback(ref MLCameraDeviceAvailabilityInfo info)
            {
                // TODO : revive
                // MLThreadDispatch.ScheduleMain(() =>
                // {
                //     // If OnDeviceAvailableCallback was called in succession before the re-connect sequence was completed,
                //     // multiple instances of this lambda would be queued. So use wasDeviceDisconnected to gate
                //     // any attempts at reconnection.
                //     if (!Instance.wasDeviceDisconnected) return;

                //     // Tear everything down and set flags for what was enabled while doing so.
                //     // We don't necessarily need to stop any ongoing capture, because it has
                //     // already been stopped internally on the platform. We just need to
                //     // call MLCameraDisconnect() and have the correct Unity side flags to be
                //     // set so we know what to resume on reconnection.
                //     // Without this teardown, ml_camera's state assumes its stil connected to
                //     // the camera and proceeds with a successful MLCameraConnect() but fails on
                //     // all other functions.
                //     DidNativeCallSucceed(Instance.Pause( /* flagsOnly */ true).Result, "MLCamera.Pause()");

                //     // Build everything back up as it was. Keep retrying connection on every OnDeviceAvailableCallback until it succeeds.
                //     Instance.wasDeviceDisconnected = !Instance.Resume( /* retry */ true).IsOk;
                // });

                MLThreadDispatch.Call(info.CamId, internalOnDeviceAvailable);
            }

            [AOT.MonoPInvokeCallback(typeof(DeviceAvailabilityStatusDelegate))]
            private static void OnDeviceUnavailableCallback(ref MLCameraDeviceAvailabilityInfo info)
            {
                MLThreadDispatch.Call(info.CamId, internalOnDeviceUnavailable);
            }

            [AOT.MonoPInvokeCallback(typeof(OnDeviceIdleDelegate))]
            private static void OnDeviceIdleCallback(IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;
                MLThreadDispatch.Call(camera.OnDeviceIdle);
            }

            [AOT.MonoPInvokeCallback(typeof(OnDeviceStreamingDelegate))]
            private static void OnDeviceStreamingCallback(IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;
                MLThreadDispatch.Call(camera.OnDeviceStreaming);
            }

            [AOT.MonoPInvokeCallback(typeof(OnDeviceDisconnectedDelegate))]
            private static void OnDeviceDisconnectCallback(MLCamera.DisconnectReason reason, IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;
                MLThreadDispatch.Call<MLCamera.DisconnectReason>(reason, camera.OnDeviceDisconnected);
            }

            [AOT.MonoPInvokeCallback(typeof(OnErrorDataCallback))]
            private static void OnDeviceErrorCallback(MLCamera.ErrorType error, IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;
                MLThreadDispatch.Call<MLCamera.ErrorType>(error, camera.OnDeviceError);
            }

            [AOT.MonoPInvokeCallback(typeof(OnCaptureFailedDelegate))]
            private static void OnCaptureFailCallback(ref MLCameraResultExtras extra, IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;
                MLCamera.IntrinsicCalibrationParameters? lambdaIntrinsics = CreateIntrinsicParameters(extra.Intrinsics);
                MLCamera.ResultExtras lambdaExtra = new MLCamera.ResultExtras(extra.FrameNumber, extra.VcamTimestamp, lambdaIntrinsics);
                MLThreadDispatch.Call<MLCamera.ResultExtras>(lambdaExtra, camera.OnCaptureFailed);
            }

            [AOT.MonoPInvokeCallback(typeof(OnCaptureAbortedDelegate))]
            private static void OnCaptureAbortCallback(IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;
                MLThreadDispatch.Call(camera.OnCaptureAborted);
            }

            [AOT.MonoPInvokeCallback(typeof(OnCaptureCompletedDelegate))]
            private static void OnCaptureCompleteCallback(ulong metadataHandle, ref MLCameraResultExtras extra, IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;

                if (camera != null)
                {
                    if (camera.previousCaptureTimestamp != 0)
                    {
                        camera.currentFPS = 1000f / Math.Abs(DateTime.Now.Millisecond - camera.previousCaptureTimestamp);
                    }

                    camera.previousCaptureTimestamp = DateTime.Now.Millisecond;
                    MLCamera.IntrinsicCalibrationParameters? lambdaIntrinsics = CreateIntrinsicParameters(extra.Intrinsics);
                    var lambdaExtra = new MLCamera.ResultExtras(extra.FrameNumber, extra.VcamTimestamp, lambdaIntrinsics);
                    MLThreadDispatch.Call(new MLCamera.Metadata(metadataHandle), lambdaExtra, camera.OnCaptureCompleted);
                }
            }

            [AOT.MonoPInvokeCallback(typeof(OnImageBufferAvailableDelegate))]
            private static void OnImageBufferAvailableCallback(ref MLCameraOutput output, ulong metadataHandle,
                                                               ref MLCameraResultExtras extra, IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;

                if (camera.OnRawImageAvailable?.GetInvocationList().Length > 0)
                {
                    MLCamera.IntrinsicCalibrationParameters? lambdaIntrinsics = CreateIntrinsicParameters(extra.Intrinsics);
                    MLCamera.ResultExtras lambdaExtra = new MLCamera.ResultExtras(extra.FrameNumber, extra.VcamTimestamp, lambdaIntrinsics);
                    // always marshal image data to managed memory in case of image captures since there isnt really
                    // a use case for providing the unmanaged memory ptr, specially when that ptr is invalidated
                    // once this callback exits.
                    // TODO : revisit to see if we wanna use the circular buffer here. How frequently would image captures be taken
                    // to warrant the use of a circular buffer?
                    MLThreadDispatch.Call<MLCamera.CameraOutput, MLCamera.ResultExtras, MLCamera.Metadata>(output.CreateFrameInfo(copyToManagedMemory: true), lambdaExtra, new MLCamera.Metadata(metadataHandle), camera.OnRawImageAvailable);
                }
            }

            [AOT.MonoPInvokeCallback(typeof(OnVideoBufferAvailableDelegate))]
            private static void OnVideoBufferAvailableCallback(ref MLCameraOutput output, ulong metadataHandle,
                                                               ref MLCameraResultExtras extra, IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;

                bool shouldCopyToManaged = camera.OnRawVideoFrameAvailable?.GetInvocationList().Length > 0;
                MLCamera.IntrinsicCalibrationParameters? lambdaIntrinsics = CreateIntrinsicParameters(extra.Intrinsics);
                MLCamera.ResultExtras lambdaExtra = new MLCamera.ResultExtras(extra.FrameNumber, extra.VcamTimestamp, lambdaIntrinsics);

                if (shouldCopyToManaged)
                {
                    for (int i = 0; i < output.PlaneCount; ++i)
                    {
                        if (camera.byteArrays[i] == null || camera.byteArrays[i].Length != output.Planes[i].Size)
                        {
                            camera.byteArrays[i] = new byte[output.Planes[i].Size];
                        }
                    }
                }
                MLCamera.CameraOutput frameInfo = output.CreateFrameInfo(shouldCopyToManaged, shouldCopyToManaged ? camera.byteArrays : null);
                camera.OnRawVideoFrameAvailable_NativeCallbackThread?.Invoke(frameInfo, lambdaExtra, new MLCamera.Metadata(metadataHandle));

                if (shouldCopyToManaged)
                {
                    MLThreadDispatch.Call(frameInfo, lambdaExtra, new MLCamera.Metadata(metadataHandle), camera.OnRawVideoFrameAvailableInternal);
                }
            }

            [AOT.MonoPInvokeCallback(typeof(OnPreviewBufferAvailableDelegate))]
            private static void OnPreviewBufferAvailableCallback(ulong bufferHandle, ulong metadataHandle,
                                                                 ref MLCameraResultExtras extra, IntPtr data)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(data);
                var camera = gcHandle.Target as MLCameraBase;

                if (camera == null)
                {
                    return;
                }

                MLCamera.IntrinsicCalibrationParameters? lambdaIntrinsics = CreateIntrinsicParameters(extra.Intrinsics);
                MLCamera.ResultExtras lambdaExtra = new MLCamera.ResultExtras(extra.FrameNumber, extra.VcamTimestamp, lambdaIntrinsics);
                if (camera.previewRenderer != null)
                {
                    camera.previewRenderer.PreviewBuffer = bufferHandle;
                }

                MLThreadDispatch.Call(camera.GLPluginEvent);

                MLThreadDispatch.Call(new MLCamera.Metadata(metadataHandle), lambdaExtra, camera.OnPreviewBufferAvailable);
            }
        }
    }
}
