namespace MagicLeap.Android
{
    using System;
    using System.Runtime.InteropServices;
    using Unity.Jobs;
    using NDK.Camera;
    using Unity.Collections;

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

        /// <summary>
        /// Set a metadata value for the capture request in the form of one or more <c>bytes</c>.
        /// </summary>
        /// <param name="tag">Enumeration for the metadata tag, e.g. a member of <see cref="NDK.Camera.Metadata.Tags"/> or <see cref="MagicLeapCameras.MetadataTags"/></param>
        /// <param name="values">Array containing all of the desired values to assign to the metadata tag. The correct number of values depends on the metadata tag.</param>
        /// <returns></returns>
        public bool TrySetMetadata(uint tag, byte[] values)
        {
            var data = new NativeArray<byte>(values.Length, Allocator.Temp);
            for (int i = 0; i < values.Length; i++)
                data[i] = values[i];
            return nativeRequest.TrySetMetadataEntry(tag, data);
        }

        /// <summary>
        /// Set a metadata value for the capture request in the form of one or more <c>ints</c>.
        /// </summary>
        /// <param name="tag">Enumeration for the metadata tag, e.g. a member of <see cref="NDK.Camera.Metadata.Tags"/> or <see cref="MagicLeapCameras.MetadataTags"/></param>
        /// <param name="values">Array containing all of the desired values to assign to the metadata tag. The correct number of values depends on the metadata tag.</param>
        /// <returns></returns>
        public bool TrySetMetadata(uint tag, int[] values)
        {
            var data = new NativeArray<int>(values.Length, Allocator.Temp);
            for (int i = 0; i < values.Length; i++)
                data[i] = values[i];
            return nativeRequest.TrySetMetadataEntry(tag, data);
        }

        /// <summary>
        /// Set a metadata value for the capture request in the form of one or more <c>longs</c>.
        /// </summary>
        /// <param name="tag">Enumeration for the metadata tag, e.g. a member of <see cref="NDK.Camera.Metadata.Tags"/> or <see cref="MagicLeapCameras.MetadataTags"/></param>
        /// <param name="values">Array containing all of the desired values to assign to the metadata tag. The correct number of values depends on the metadata tag.</param>
        /// <returns></returns>
        public bool TrySetMetadata(uint tag, long[] values)
        {
            var data = new NativeArray<long>(values.Length, Allocator.Temp);
            for (int i = 0; i < values.Length; i++)
                data[i] = values[i];
            return nativeRequest.TrySetMetadataEntry(tag, data);
        }

        /// <summary>
        /// Set a metadata value for the capture request in the form of one or more <c>doubles</c>.
        /// </summary>
        /// <param name="tag">Enumeration for the metadata tag, e.g. a member of <see cref="NDK.Camera.Metadata.Tags"/> or <see cref="MagicLeapCameras.MetadataTags"/></param>
        /// <param name="values">Array containing all of the desired values to assign to the metadata tag. The correct number of values depends on the metadata tag.</param>
        /// <returns></returns>
        public bool TrySetMetadata(uint tag, double[] values)
        {
            var data = new NativeArray<double>(values.Length, Allocator.Temp);
            for (int i = 0; i < values.Length; i++)
                data[i] = values[i];
            return nativeRequest.TrySetMetadataEntry(tag, data);
        }

        /// <summary>
        /// Set a metadata value for the capture request in the form of one or more <c>floats</c>.
        /// </summary>
        /// <param name="tag">Enumeration for the metadata tag, e.g. a member of <see cref="NDK.Camera.Metadata.Tags"/> or <see cref="MagicLeapCameras.MetadataTags"/></param>
        /// <param name="values">Array containing all of the desired values to assign to the metadata tag. The correct number of values depends on the metadata tag.</param>
        /// <returns></returns>
        public bool TrySetMetadata(uint tag, float[] values)
        {
            var data = new NativeArray<float>(values.Length, Allocator.Temp);
            for (int i = 0; i < values.Length; i++)
                data[i] = values[i];
            return nativeRequest.TrySetMetadataEntry(tag, data);
        }

        /// <summary>
        /// Set a metadata value for the capture request in the form of one or more <see cref="ACameraMetadata.Rational"/>.
        /// </summary>
        /// <param name="tag">Enumeration for the metadata tag, e.g. a member of <see cref="NDK.Camera.Metadata.Tags"/> or <see cref="MagicLeapCameras.MetadataTags"/></param>
        /// <param name="values">Array containing all of the desired values to assign to the metadata tag. The correct number of values depends on the metadata tag.</param>
        /// <returns></returns>
        public bool TrySetMetadata(uint tag, ACameraMetadata.Rational[] values)
        {
            var data = new NativeArray<ACameraMetadata.Rational>(values.Length, Allocator.Temp);
            for (int i = 0; i < values.Length; i++)
                data[i] = values[i];
            return nativeRequest.TrySetMetadataEntry(tag, data);
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
