// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "IHardwareBufferProvider.cs" company="Magic Leap, Inc">
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

namespace UnityEngine.XR.MagicLeap
{
    public partial class YcbcrRenderer
    {
        public interface IHardwareBufferProvider
        {
            internal abstract bool AcquireNextAvailableHwBuffer(out IntPtr hwBuffer);

            internal abstract void ReleaseHwBuffer(IntPtr hwBuffer);
        }
    }
}
