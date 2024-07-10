namespace MagicLeap.Android.NDK.NativeWindow
{
    using System;
    using System.Runtime.InteropServices;

    internal static class NativeWindowNativeBindings
    {
        public const string kLibraryName = "libnativewindow";

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AHardwareBuffer_acquire(AHardwareBuffer buffer);

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AHardwareBuffer_allocate(ref AHardwareBuffer.Description description,
            out AHardwareBuffer buffer);

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AHardwareBuffer_describe(AHardwareBuffer buffer,
            out AHardwareBuffer.Description description);

#if ANDROID_API_LEVEL_31_OR_NEWER
        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AHardwareBuffer_getId(AHardwareBuffer buffer, out ulong outId);
#endif

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AHardwareBuffer_isSupported(ref AHardwareBuffer.Description description);

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AHardwareBuffer_lock(AHardwareBuffer buffer, AHardwareBuffer.Usage usage,
            int fence, ARect* rect, out void* virtualAddress);

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AHardwareBuffer_lockAndGetInfo(AHardwareBuffer buffer,
            AHardwareBuffer.Usage usage, int fence, ARect* rect, out void* outVirtualAddress, out int outBytesPerPixel,
            out int outBytesPerStride);

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AHardwareBuffer_lockPlanes(AHardwareBuffer buffer, AHardwareBuffer.Usage usage,
            int fenceFd, ARect* rect, out AHardwareBuffer.Planes outPlanes);

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AHardwareBuffer_recvHandleFromUnixSocket(int socketFd, out AHardwareBuffer buffer);

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AHardwareBuffer_release(AHardwareBuffer buffer);

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern int AHardwareBuffer_sendHandleToUnixSocket(AHardwareBuffer buffer, int socketFd);

        [DllImport(kLibraryName, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe int AHardwareBuffer_unlock(AHardwareBuffer buffer, int* fence);
    }
}
