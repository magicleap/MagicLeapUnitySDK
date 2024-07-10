namespace MagicLeap.Android.NDK.Camera
{
    using System;
    using Unity.Collections.LowLevel.Unsafe;
    public unsafe struct ACameraCaptureFailure : INullablePointer
    {
        private struct Data
        {
            public long frameNumber;
            public int reason;
            public int sequenceId;
            public byte wasCaptured;
        }

        [NativeDisableUnsafePtrRestriction]
        private Data* data;

        public long FrameNumber
        {
            get
            {
                this.CheckNullAndThrow();
                return data->frameNumber;
            }
        }

        public int Reason
        {
            get
            {
                this.CheckNullAndThrow();
                return data->reason;
            }
        }

        public int SequenceId
        {
            get
            {
                this.CheckNullAndThrow();
                return data->sequenceId;
            }
        }

        public bool WasCaptured
        {
            get
            {
                this.CheckNullAndThrow();
                return data->wasCaptured != 0;
            }
        }

        public bool IsNull => data == null;

        void IDisposable.Dispose()
            => throw new InvalidOperationException(
                "This object doesn't need to be disposed, and boxing it into an IDisposable simply to dispose it wastes GC memory");
    }
}
