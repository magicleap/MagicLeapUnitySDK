using System.Diagnostics;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap.Unsafe
{
    using System;
    using Unity.Collections;
    using Unity.Collections.LowLevel.Unsafe;
    internal static unsafe class UnsafeUtilityEx
    {
        public static T* Calloc<T>(Allocator allocator, T initialValue = default) where T : unmanaged
        {
            var ptr = Malloc<T>(allocator);
            *ptr = initialValue;
            return ptr;
        }

        public static T[] GetManagedArray<T>(IntPtr buffer, int length) where T : unmanaged
        {
            return GetManagedArray((T*)buffer, length);
        }
        

        public static T[] GetManagedArray<T>(T* buffer, int length) where T: unmanaged
        {
            var result = new T[length];
            for (int i = 0; i < length; i++)
            {
                result[i] = buffer[i];
            }
            return result;
        }

        public static T* CallocTracked<T>(Allocator allocator, T initialValue = default, int callstacksToSkip = 1)
            where T : unmanaged
        {
            var ptr = MallocTracked<T>(allocator, callstacksToSkip);
            *ptr = initialValue;
            return ptr;
        }

        public static T* Malloc<T>(Allocator allocator) where T : unmanaged
            => (T*)UnsafeUtility.Malloc(sizeof(T), UnsafeUtility.AlignOf<T>(), allocator);

        public static T* MallocTracked<T>(Allocator allocator, int callstacksToSkip = 1) where T : unmanaged
            => (T*)UnsafeUtility.MallocTracked(sizeof(T), UnsafeUtility.AlignOf<T>(), allocator, callstacksToSkip);

#if ENABLE_UNITY_COLLECTIONS_CHECKS
        public static AtomicSafetyHandle CreateAtomicSafetyHandleForAllocator(Allocator allocator)
        {
            switch (allocator)
            {
                case Allocator.Invalid:
                    throw new InvalidOperationException("Cannot create safety handle for invalid allocator");
                case Allocator.Temp:
                    return AtomicSafetyHandle.GetTempMemoryHandle();
                default:
                    return AtomicSafetyHandle.Create();
            }
        }
#endif

        public static U* PunRefTypeUnchecked<T, U>(ref T obj) where T : unmanaged where U : unmanaged
            => (U*)UnsafeUtility.AddressOf(ref UnsafeUtility.As<T, U>(ref obj));

        public static U* PunType<T, U>(T* obj) where T : unmanaged where U : unmanaged
        {
            CheckTypeSizeAndThrow<T,U>();

            return PunTypeUnchecked<T, U>(obj);
        }
        public static T* PunType<T>(void* obj, int expectedSize) where T : unmanaged
        {
            CheckTypeSizeAndThrow<T>(expectedSize);

            return PunTypeUnchecked<T>(obj);
        }

        public static U* PunTypeUnchecked<U>(void* obj) where U : unmanaged
            => (U*)UnsafeUtility.AddressOf(ref UnsafeUtility.AsRef<U>(obj));

        public static U* PunTypeUnchecked<T, U>(T* obj) where T: unmanaged where U : unmanaged
            => (U*)UnsafeUtility.AddressOf(ref UnsafeUtility.AsRef<U>(obj));

        public static string FormatAddress(void* ptr)
        {
            var i64 = new IntPtr(ptr).ToInt64();
            return $"0x{i64:X}";
        }

        [Conditional("DEVELOPMENT_BUILD")]
        public static void CheckTypeSizeAndThrow<T, U>() where T : unmanaged where U : unmanaged
        {
            var szT = sizeof(T);
            var szU = sizeof(U);
            if (szT != szU)
                throw new InvalidOperationException(
                    $"Type size mismatch! sizeof({typeof(T).Name}) = {szT}, sizeof({typeof(U).FullName}) = {szU}");
        }

        [Conditional("DEVELOPMENT_BUILD")]
        public static void CheckTypeSizeAndThrow<T>(int expectedTypeSize) where T : unmanaged
        {
            var szT = sizeof(T);
            if (szT != expectedTypeSize)
                throw new InvalidOperationException(
                    $"Type size mismatch! Expected sizeof({typeof(T).FullName}) to be {expectedTypeSize}, was instead {szT}");
        }

    }
}
