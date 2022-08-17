// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLWebViewYcbcrRenderer.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
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

            public Renderer(UnityEngine.ColorSpace colorSpace)
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                Initialize(colorSpace);
#endif
            }

            bool IHardwareBufferProvider.AcquireNextAvailableHwBuffer(out IntPtr hwBuffer)
            {
                if (!MagicLeapNativeBindings.MLHandleIsValid(WebViewHandle))
                {
                    Debug.LogError("failed to acquire frame buffer, invalid handle");
                    hwBuffer = IntPtr.Zero;
                    return false;
                }

#if UNITY_MAGICLEAP || UNITY_ANDROID
                MLResult.Code result = NativeBindings.MLWebViewAcquireNextAvailableFrame(WebViewHandle, out hwBuffer);

                bool acquired = MLResult.IsOK(result) && hwBuffer != IntPtr.Zero;
                if (acquired)
                {
                    acquiredFrameWebViewHandles.Enqueue(WebViewHandle);
                }

                return acquired;
#else
                hwBuffer = IntPtr.Zero;
                return false;
#endif
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

#if UNITY_MAGICLEAP || UNITY_ANDROID
                NativeBindings.MLWebViewReleaseFrame(currentWebViewHandle, hwBuffer);
#endif
            }

            bool IFrameTransformMatrixProvider.GetFrameTransformMatrix(float[] frameTransformMatColMajor)
            {
#if UNITY_ANDROID
                if (MLResult.DidNativeCallSucceed(
                    NativeBindings.MLWebViewGetFrameTransformMatrix(WebViewHandle, out MagicLeapNativeBindings.MLMat4f matrix),
                    nameof(NativeBindings.MLWebViewGetFrameTransformMatrix)))
                {
                    Array.Copy(matrix.MatrixColmajor, frameTransformMatColMajor, frameTransformMatColMajor.Length);
                }
#endif
                return true;
            }
        }
    }
}
