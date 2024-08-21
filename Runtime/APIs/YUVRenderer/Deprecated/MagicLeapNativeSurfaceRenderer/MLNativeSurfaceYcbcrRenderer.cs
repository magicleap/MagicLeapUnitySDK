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
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    [Obsolete("MLNativeSurfaceYcbcrRenderer will be deprecated. Please use YCbCrHardwareBufferRenderer instead")]
    public class MLNativeSurfaceYcbcrRenderer : YcbcrRenderer, YcbcrRenderer.INativeBufferProvider, YcbcrRenderer.IFrameTransformMatrixProvider
    {
        public MLNativeSurface Surface { get; private set; }
        private int isNewFrameAvailable;
        
        public MLNativeSurfaceYcbcrRenderer(uint width, uint height)
        {
            Initialize();
            // 3 buffer count because MLGraphics uses 3 buffer count
            Surface = new MLNativeSurface(MLNativeSurface.PixelFormat.Rgb888, 3, width, height);
            Surface.OnFrameAvailable += OnFrameAvailable;
            OnCleanupComplete += OnResourceCleanupComplete;
        }

        private void OnResourceCleanupComplete()
        {
            Surface.OnFrameAvailable -= OnFrameAvailable;
            Surface.Destroy();
        }

        private void OnFrameAvailable()
        {
            isNewFrameAvailable = 1;
        }
        
        bool IFrameTransformMatrixProvider.GetFrameTransformMatrix(float[] frameTransformMatColMajor)
        {
            if (!Surface.GetFrameTransformMatrix(frameTransformMatColMajor).IsOk)
            {
                return false;
            }

            MLConvert.FlipTransformMatrixVertically(frameTransformMatColMajor);

            return true;
        }
        
        bool INativeBufferProvider.AcquireNextAvailableBuffer(out ulong nativeBufferHandle)
        {
            nativeBufferHandle = MagicLeapNativeBindings.InvalidHandle;
            // This needs to be an atomic operation because OnFrameAvailable is called on a different thread than this func.
            var acquireNewFrame = (System.Threading.Interlocked.Exchange(ref isNewFrameAvailable, 0) == 1);
            var result = MLResult.Create(MLResult.Code.UnspecifiedFailure);
            if (!acquireNewFrame)
            {
                return result.IsOk;
            }
            result = Surface.AcquireNextAvailableFrame(out nativeBufferHandle);
            return result.IsOk;
        }

        void INativeBufferProvider.ReleaseBuffer(ulong nativeBufferHandle)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(Surface.Handle) || !MagicLeapNativeBindings.MLHandleIsValid(nativeBufferHandle))
            {
                return;
            }
            Surface.ReleaseFrame(nativeBufferHandle);
        }
        
        ulong INativeBufferProvider.NativeSurfaceHandle => Surface.Handle;
    }
}
