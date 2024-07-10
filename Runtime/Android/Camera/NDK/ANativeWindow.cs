// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace MagicLeap.Android.NDK.NativeWindow
{
    using System;

    public struct ANativeWindow : INullablePointer
    {
        private IntPtr value;

        public bool IsNull => value == IntPtr.Zero;

        void IDisposable.Dispose() {}
    }
}
