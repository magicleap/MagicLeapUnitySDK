using Unity.Collections;

namespace MagicLeap.Android
{
    public static class JPEGUtility
    {
        public static byte[] ExtractJPEGData(NativePlane plane)
        {
            using (var data = plane.CopyToNativeArray(Allocator.Temp))
                return data.ToArray();
        }

    }
}
