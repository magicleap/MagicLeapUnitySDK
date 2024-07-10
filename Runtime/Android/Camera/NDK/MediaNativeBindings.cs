using System.Runtime.InteropServices;
using MagicLeap.Android.NDK.NativeWindow;

namespace MagicLeap.Android.NDK.Media
{
    internal static class MediaNativeBindings
    {
        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AImage_delete(AImage image);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImage_getCropRect(AImage image, out ARect cropRect);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImage_getFormat(AImage image, out MediaFormat format);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImage_getHardwareBuffer(AImage image, out AHardwareBuffer buffer);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImage_getHeight(AImage image, out int height);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImage_getNumberOfPlanes(AImage image, out int numPlanes);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern unsafe MediaStatus AImage_getPlaneData(AImage image, int planeIdx, out byte* data,
            out int dataLength);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImage_getPlanePixelStride(AImage image, int planeIdx, out int pixelStride);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImage_getPlaneRowStride(AImage image, int planeIdx, out int rowStride);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImage_getTimestamp(AImage image, out long timestampNs);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImage_getWidth(AImage image, out int width);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_acquireLatestImage(AImageReader reader, out AImage image);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_acquireNextImage(AImageReader reader, out AImage image);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern void AImageReader_delete(AImageReader reader);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_getFormat(AImageReader reader, out MediaFormat format);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_getHeight(AImageReader reader, out int height);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_getMaxImages(AImageReader reader, out int maxImages);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_getWidth(AImageReader reader, out int width);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_getWindow(AImageReader reader, out ANativeWindow window);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_new(int width, int height, MediaFormat format, int maxImages,
            out AImageReader reader);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_newWithUsage(int width, int height, MediaFormat format,
            AHardwareBuffer.Usage usage, int maxImages, out AImageReader reader);

        [DllImport(MediaConstants.kMediaLibrary, CallingConvention = CallingConvention.Cdecl)]
        public static extern MediaStatus AImageReader_setImageListener(AImageReader reader,
            ref AImageReader.ImageListener callback);
    }
}
