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

    using static CameraNativeBindings;

    public struct ACaptureSessionOutputContainer : INullablePointer
    {
        private IntPtr value;

        public bool IsNull => value == IntPtr.Zero;

        public void Dispose()
        {
            if (!IsNull)
                ACaptureSessionOutputContainer_free(this);

            value = IntPtr.Zero;
        }

        public bool TryAddOutput(ACaptureSessionOutput output)
        {
            this.CheckNullAndThrow();
            output.CheckNullAndThrow();
            var result = ACaptureSessionOutputContainer_add(this, output);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public static bool TryCreate(out ACaptureSessionOutputContainer container)
        {
            var result = ACaptureSessionOutputContainer_create(out container);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }

        public bool TryRemoveOutput(ACaptureSessionOutput output)
        {
            this.CheckNullAndThrow();
            output.CheckNullAndThrow();
            var result = ACaptureSessionOutputContainer_remove(this, output);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }
    }
}
