// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "INativeBufferProvider.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    public partial class YcbcrRenderer
    {
        public interface INativeBufferProvider
        {
            internal abstract bool AcquireNextAvailableBuffer(out ulong nativeBufferHandle);

            internal abstract void ReleaseBuffer(ulong nativeBufferHandle);
        }
    }
}
