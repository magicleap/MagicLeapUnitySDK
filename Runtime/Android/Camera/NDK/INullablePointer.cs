namespace MagicLeap
{
    using System;
    using System.Diagnostics;

    public interface INullablePointer : IDisposable
    {
        bool IsNull { get; }
    }

    internal interface IReferenceCountedPointer
    {
        void Acquire();
        void Release();
    }

    public static class NullablePointerExtensions
    {
        [Conditional("DEVELOPMENT_BUILD"), Conditional("ENABLE_UNITY_COLLECTIONS_CHECKS")]
        public static void CheckNullAndThrow<T>(this T self) where T : unmanaged, INullablePointer
        {
            if (self.IsNull)
                throw new NullReferenceException();
        }
    }

    internal static class ReferenceCounterPtrExtensions
    {
        public static T Clone<T>(this T self) where T : unmanaged, IReferenceCountedPointer
        {
            self.Acquire();
            return self;
        }
    }
}
