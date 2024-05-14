// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using Unity.Collections;
using UnityEngine.Serialization;
using UnityEngine.XR.MagicLeap.Unsafe;

namespace MagicLeap.Android.NDK.Camera
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Collections.LowLevel.Unsafe;

    using static CameraNativeBindings;

    public struct ACameraMetadata : INullablePointer
    {
        public struct Entry
        {
            // [StructLayout(LayoutKind.Explicit, Size = 8)]
            // public unsafe struct ConstUnionPointer
            // {
            //     [FieldOffset(0)]
            //     public readonly byte*     u8;
            //     [FieldOffset(0)]
            //     public readonly int*      i32;
            //     [FieldOffset(0)]
            //     public readonly float*    f32;
            //     [FieldOffset(0)]
            //     public readonly long*     i64;
            //     [FieldOffset(0)]
            //     public readonly double*   f64;
            //     [FieldOffset(0)]
            //     public readonly Rational* r;
            // }

            [StructLayout(LayoutKind.Explicit, Size = 8)]
            public unsafe struct UnionPointer
            {
                [FieldOffset(0)]
                [NativeDisableUnsafePtrRestriction]
                public byte*     U8;
                [FieldOffset(0)]
                [NativeDisableUnsafePtrRestriction]
                public int*      I32;
                [FieldOffset(0)]
                [NativeDisableUnsafePtrRestriction]
                public float*    F32;
                [FieldOffset(0)]
                [NativeDisableUnsafePtrRestriction]
                public long*     I64;
                [FieldOffset(0)]
                [NativeDisableUnsafePtrRestriction]
                public double*   F64;
                [FieldOffset(0)]
                [NativeDisableUnsafePtrRestriction]
                public Rational* R;
            }

            public struct ReadOnly
            {
                public readonly uint Tag;
                public readonly Type Type;
                public readonly uint Count;
                public readonly UnionPointer Data;
            }

            public enum Type : byte
            {
                Byte = 0,
                Int32 = 1,
                Float = 2,
                Int64 = 3,
                Double = 4,
                Rational = 5,
                NumTypes,
            }

            public uint Tag;
            public Type type;
            public uint Count;
            public UnionPointer Data;
        }

        public struct Rational
        {
            public int Numerator;
            [FormerlySerializedAs("denominator")] public int Denominator;
        }

        private IntPtr value;

        public bool IsNull => value == IntPtr.Zero;

        public void Dispose()
        {
            if (!IsNull)
                ACameraMetadata_free(this);
        }

        public unsafe NativeArray<uint> GetAllMetadataTags()
        {
            uint* data = null;
            int length = 0;
            var result = ACameraMetadata_getAllTags(this, out length, out data);
            result.CheckReturnValueAndThrow();
            if (result != CameraStatus.Ok)
                return default;

            var array = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<uint>(data, length, Allocator.None);
#if ENABLE_UNITY_COLLECTIONS_CHECKS
            NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref array, UnsafeUtilityEx.CreateAtomicSafetyHandleForAllocator(Allocator.None));
#endif
            return array;
        }

        public bool TryGetConstEntry(Metadata.Tags tag, out Entry.ReadOnly outEntry)
        {
            var result = ACameraMetadata_getConstEntry(this, (uint)tag, out outEntry);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }
    }
}
