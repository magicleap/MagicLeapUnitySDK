namespace MagicLeap.Android
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Jobs;
    using NDK.Camera;

    public sealed class CaptureRequest : IDisposable
    {
        public delegate JobHandle OnFrameAvailable(AndroidCamera camera, CaptureRequest request,
            NativeImage image, IntPtr context);

        // parent camera object.
        private AndroidCamera camera;
        // native request pointer.
        private ACaptureRequest nativeRequest;
        // optional user context pointer.
        private IntPtr context;
        // processing delegate.
        private OnFrameAvailable onFrameAvailable;

        internal ACaptureRequest NativeRequest => nativeRequest;


        internal CaptureRequest(AndroidCamera camera, ACaptureRequest nativeRequest,
            OnFrameAvailable onFrameAvailable, IntPtr context = default)
        {
            this.camera = camera ?? throw new ArgumentNullException(nameof(camera));
            if (nativeRequest.IsNull)
                throw new ArgumentNullException(nameof(nativeRequest));
            this.nativeRequest = nativeRequest;
            this.context = context;
            this.onFrameAvailable = onFrameAvailable ?? throw new ArgumentNullException(nameof(onFrameAvailable));

            if (this.nativeRequest.IsNull)
                throw new ArgumentException($"'{nameof(nativeRequest)}' must be a valid native pointer");

            var weakHandle = GCHandle.Alloc(this, GCHandleType.Weak);
            this.nativeRequest.UserContext = GCHandle.ToIntPtr(weakHandle);
        }

        ~CaptureRequest()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (nativeRequest.IsNull)
                    throw new ObjectDisposedException("object is already disposed");

                var handle = GCHandle.FromIntPtr(nativeRequest.UserContext);
                if (handle.IsAllocated)
                    handle.Free();
            }

            camera = null;
            context = IntPtr.Zero;
            onFrameAvailable = null;

            if (!nativeRequest.IsNull)
                nativeRequest.Dispose();
            nativeRequest = default;
        }

        internal JobHandle ProcessNewFrame(NativeImage image)
            => onFrameAvailable(camera, this, image, context);

        internal static bool TryGetFromIntPtr(IntPtr ptr, out CaptureRequest outCaptureRequest)
        {
            outCaptureRequest = null;

            if (ptr == IntPtr.Zero)
                return false;

            var handle = GCHandle.FromIntPtr(ptr);
            if (!handle.IsAllocated)
                return false;

            outCaptureRequest = (CaptureRequest)handle.Target;
            return true;
        }
    }
}
