// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace MagicLeap.Android.NDK.Camera
{
    using System;
    using NativeWindow;

    using static CameraNativeBindings;

    public struct ACameraOutputTarget : INullablePointer
    {
        private IntPtr value;

        public bool IsNull => value == IntPtr.Zero;

        public void Dispose()
        {
            if (!IsNull)
                ACameraOutputTarget_free(this);

            value = IntPtr.Zero;
        }

        public static bool TryCreate(ANativeWindow window, out ACameraOutputTarget outTarget)
        {
            window.CheckNullAndThrow();

            var result = ACameraOutputTarget_create(window, out outTarget);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }
    }
}
