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
    using System.Diagnostics;
    using System.Runtime.InteropServices;

    public unsafe struct ACameraIdList : INullablePointer
    {
        private struct Data
        {
            public int NumCameras;
            public byte** CameraIds;
        }

        private Data* data;

        public bool IsNull => data == null;

        public int NumCameras
        {
            get
            {
                this.CheckNullAndThrow();
                return data->NumCameras;
            }
        }

        public string CameraAt(int index)
        {
            this.CheckNullAndThrow();
            CheckIndexAndThrow(index);
            return Marshal.PtrToStringAnsi(new IntPtr(data->CameraIds[index]));
        }

        public byte* CameraAtNonAlloc(int index)
        {
            this.CheckNullAndThrow();
            CheckIndexAndThrow(index);
            return data->CameraIds[index];
        }

        public void Dispose()
        {
            if (!IsNull)
                CameraNativeBindings.ACameraManager_deleteCameraIdList(this);

            data = null;
        }

        [Conditional("DEVELOPMENT_BUILD")]
        private void CheckIndexAndThrow(int index)
        {
            if (index < 0 || index >= NumCameras)
                throw new IndexOutOfRangeException($"camera index must be between 0 and {NumCameras - 1}, inclusive");
        }
    }
}
