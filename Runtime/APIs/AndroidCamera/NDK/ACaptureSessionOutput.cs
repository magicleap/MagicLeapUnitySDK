namespace MagicLeap.Android.NDK.Camera
{
    using System;
    using NativeWindow;

    using static CameraNativeBindings;

    public struct ACaptureSessionOutput : INullablePointer
    {
        private IntPtr value;

        public bool IsNull => value == IntPtr.Zero;

        public void Dispose()
        {
            if (!IsNull)
                ACaptureSessionOutput_free(this);

            value = IntPtr.Zero;
        }

        public static bool TryCreate(ANativeWindow window, out ACaptureSessionOutput output)
        {
            var result = ACaptureSessionOutput_create(window, out output);
            result.CheckReturnValueAndThrow();
            return result == CameraStatus.Ok;
        }
    }
}
