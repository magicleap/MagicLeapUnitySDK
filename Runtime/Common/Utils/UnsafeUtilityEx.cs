using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.MagicLeap.Unsafe
{
    internal static unsafe class UnsafeUtilityEx
    {
        public static T* Malloc<T>(Allocator allocator) where T : unmanaged
            => (T*)UnsafeUtility.Malloc(sizeof(T), UnsafeUtility.AlignOf<T>(), allocator);

        public static T* MallocTracked<T>(Allocator allocator, int callstacksToSkip) where T : unmanaged
            => (T*)UnsafeUtility.MallocTracked(sizeof(T), UnsafeUtility.AlignOf<T>(), allocator, callstacksToSkip);
    }
}
