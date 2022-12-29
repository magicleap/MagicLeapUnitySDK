// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLWebView
    {
        public class Renderer : YcbcrRenderer, YcbcrRenderer.IHardwareBufferProvider, YcbcrRenderer.IFrameTransformMatrixProvider
        {
            /// <summary>
            /// WebView handle.
            /// </summary>
            public ulong WebViewHandle = MagicLeapNativeBindings.InvalidHandle;

            private Queue<ulong> acquiredFrameWebViewHandles = new Queue<ulong>();

            public Renderer()
            {
                Initialize();
            }

            bool IHardwareBufferProvider.AcquireNextAvailableHwBuffer(out IntPtr hwBuffer)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(WebViewHandle))
                {
                    Debug.LogError("failed to acquire frame buffer, invalid handle");
                    hwBuffer = IntPtr.Zero;
                    return false;
                }

                MLResult.Code result = NativeBindings.MLWebViewAcquireNextAvailableFrame(WebViewHandle, out hwBuffer);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLWebViewAcquireNextAvailableFrame));
                bool acquired = MLResult.IsOK(result) && hwBuffer != IntPtr.Zero;
                if (acquired)
                {
                    acquiredFrameWebViewHandles.Enqueue(WebViewHandle);
                }

                return acquired;
            }

            void IHardwareBufferProvider.ReleaseHwBuffer(IntPtr hwBuffer)
            {
                if (acquiredFrameWebViewHandles.Count == 0)
                {
                    return;
                }

                ulong currentWebViewHandle = acquiredFrameWebViewHandles.Dequeue();
                if (!MagicLeapNativeBindings.MLHandleIsValid(currentWebViewHandle))
                {
                    Debug.LogError("failed to release frame buffer, invalid handle");
                    return;
                }

                NativeBindings.MLWebViewReleaseFrame(currentWebViewHandle, hwBuffer);
            }

            bool IFrameTransformMatrixProvider.GetFrameTransformMatrix(float[] frameTransformMatColMajor)
            {
                if (MLResult.DidNativeCallSucceed(
                    NativeBindings.MLWebViewGetFrameTransformMatrix(WebViewHandle, out MagicLeapNativeBindings.MLMat4f matrix),
                    nameof(NativeBindings.MLWebViewGetFrameTransformMatrix)))
                {
                    Array.Copy(matrix.MatrixColmajor, frameTransformMatColMajor, frameTransformMatColMajor.Length);
                }

                return true;
            }
        }
    }
}
