// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLNativeSurfaceYcbcrRenderer.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLNativeSurfaceYcbcrRenderer : YcbcrRenderer, YcbcrRenderer.INativeBufferProvider, YcbcrRenderer.IFrameTransformMatrixProvider
    {
        public MLNativeSurface Surface { get; private set; }
        private int isNewFrameAvailable;

        public MLNativeSurfaceYcbcrRenderer(UnityEngine.ColorSpace colorSpace, uint width, uint height)
        {
            Initialize(colorSpace);
            // 3 buffer count because MLGraphics uses 3 buffer count
            Surface = new MLNativeSurface(MLNativeSurface.PixelFormat.Rgb888, 3, width, height);
            Surface.OnFrameAvailable += OnFrameAvailable;
        }

        private void OnFrameAvailable()
        {
            isNewFrameAvailable = 1;
        }

        bool INativeBufferProvider.AcquireNextAvailableBuffer(out ulong nativeBufferHandle)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            // This needs to be an atomic operation because OnFrameAvailable is called on a different thread than this func.
            bool acquireNewFrame = (System.Threading.Interlocked.Exchange(ref isNewFrameAvailable, 0) == 1);
            if (acquireNewFrame)
            {
                MLResult result = Surface.AcquireNextAvailableFrame(out nativeBufferHandle);
                return result.IsOk;
            }
            nativeBufferHandle = 0;
            return false;
#else
            nativeBufferHandle = 0;
            return false;
#endif
        }

        void INativeBufferProvider.ReleaseBuffer(ulong nativeBufferHandle)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            Surface.ReleaseFrame(nativeBufferHandle);
#endif
        }

        bool IFrameTransformMatrixProvider.GetFrameTransformMatrix(float[] frameTransformMatColMajor)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (Surface.GetFrameTransformMatrix(frameTransformMatColMajor).IsOk)
            {
                Native.MLConvert.FlipTransformMatrixVertically(frameTransformMatColMajor);

                return true;
            }
#endif
            return false;
        }
    }
}
