namespace MagicLeap.Android
{
    using System.Runtime.CompilerServices;
    using Unity.Collections;
    using Unity.Jobs;
    using UnityEngine;
    using UnityEngine.XR.MagicLeap.Unsafe;

    public static class YUVUtility
    {
        private const float kRedCoefficient = 1.370705f;
        private const float kGreenCoefficientV = 0.698001f;
        private const float kGreenCoefficientU = 0.337633f;
        private const float kBlueCoefficient = 1.732446f;

        private struct YuvToRgbJob : IJobFor
        {
            public NativePlane Y;
            public NativePlane U;
            public NativePlane V;

            public ImageDimensions Dimensions;

            public NativeArray<Color32> RgbData;

            public void Execute(int index)
            {
                (int x, int y) = Dimensions.GetCoordinatesFromIndex(index);

                int yIndex = CalculateYIndex(x, y, Y.PixelStride, Y.RowStride);
                int uvIndex = CalculateUVIndex(x, y, U.PixelStride, U.RowStride);

                byte rawY = Y.GetDataAtOffset<byte>(yIndex);
                byte rawU = U.GetDataAtOffset<byte>(uvIndex);
                byte rawV = V.GetDataAtOffset<byte>(uvIndex);

                RgbData[index] = ProcessPixel(rawY, rawU, rawV);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateYIndex(int x, int y, int pixelStride, int rowStride)
            => (y * rowStride) + (x * pixelStride);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int CalculateUVIndex(int x, int y, int pixelStride, int rowStride)
        {
            int uvx = x / 2;
            int uvy = y / 2;

            return (uvy * rowStride) + (uvx * pixelStride);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private static byte ClampToByte(int value) => (byte)Mathf.Clamp(value, 0, 255);

        public static JobHandle ConvertPlanesToRGBAsync(Allocator allocator, NativeYUVPlanes yuvPlanes, out NativeArray<Color32> outRgb)
        {
            outRgb = yuvPlanes.Dimensions.CreateNativeArray<Color32>(allocator);

            var job = new YuvToRgbJob
            {
                Y = yuvPlanes.YPlane,
                U = yuvPlanes.UPlane,
                V = yuvPlanes.VPlane,
                Dimensions = yuvPlanes.Dimensions,
                RgbData = outRgb,
            };

            return job.ScheduleParallel(yuvPlanes.Dimensions.Size, yuvPlanes.Dimensions.Width, default);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Color32 ProcessPixel(byte rawY, byte rawU, byte rawV)
        {
            int y = rawY & 0xFF;
            int u = (rawU & 0xFF) - 128;
            int v = (rawV & 0xFF) - 128;

            int r = (int)(y + kRedCoefficient * v);
            int g = (int)(y - (kGreenCoefficientV * v) - (kGreenCoefficientU * u));
            int b = (int)(y + kBlueCoefficient * u);

            return new Color32
            {
                a = 255,
                r = ClampToByte(r),
                g = ClampToByte(g),
                b = ClampToByte(b)
            };
        }
    }
}
