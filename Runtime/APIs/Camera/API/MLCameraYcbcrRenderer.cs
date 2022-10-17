// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_MAGICLEAP || UNITY_ANDROID
using UnityEngine.XR.MagicLeap.Native;
#endif

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLCamera
    {
        public class Renderer : YcbcrRenderer, YcbcrRenderer.INativeBufferProvider
        {
            /// <summary>
            /// Buffer received from Camera Preview.
            /// </summary>
#if UNITY_MAGICLEAP || UNITY_ANDROID
            public ulong PreviewBuffer = MagicLeapNativeBindings.InvalidHandle;
#else
            public ulong PreviewBuffer;

#endif
            public Renderer()
            {
                Initialize();
            }

            bool INativeBufferProvider.AcquireNextAvailableBuffer(out ulong nativeBufferHandle)
            {
                nativeBufferHandle = PreviewBuffer;
#if UNITY_MAGICLEAP || UNITY_ANDROID
                return MagicLeapNativeBindings.MLHandleIsValid(PreviewBuffer);
#else
                return false;

#endif
            }

            void INativeBufferProvider.ReleaseBuffer(ulong nativeBufferHandle)
            { }
        }
    }
}
