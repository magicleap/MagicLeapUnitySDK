// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLCameraYcbcrRenderer.cs" company="Magic Leap, Inc">
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
            public Renderer(ColorSpace colorSpace)
            {
                Initialize(colorSpace);
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
