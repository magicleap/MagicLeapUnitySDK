#if UNITY_OPENXR_1_7_0_OR_NEWER
using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop
{
    internal enum BlendMode : ulong
    {
        Additive = 2,
        AlphaBlend = 3 
    }
    
    [Flags]
    internal enum FrameInfoFlags : ulong
    {
        None = 0,
        Protected = 1 << 0,
        Vignette = 1 << 1,
    }
    
    // By default, [StructLayout(LayoutKind.Sequential)] is applied to structs
    internal unsafe struct FrameEndInfo
    {
        // OpenXR structure type constant (from OpenXR headers)
        internal const ulong kStructType = 1000135000;
#pragma warning disable 0414
        private ulong type;
        internal void* next;
        internal float focusDistance;
        internal FrameInfoFlags flags;
#pragma warning restore 0414

        internal static FrameEndInfo Init()
            => new FrameEndInfo
            {
                type = kStructType,
                next = null,
                focusDistance = 0.0f,
                flags = FrameInfoFlags.None,
            };
    }

    internal static class FunctionPrototypes
    {
#if PLATFORM_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const CallingConvention kCallConv = CallingConvention.StdCall;
#else
        private const CallingConvention kCallConv = CallingConvention.Cdecl;
#endif
        [UnmanagedFunctionPointer(kCallConv, CharSet = CharSet.Ansi)]
        public delegate Result XRGetInstanceProcAddr(ulong instance, string name, ref IntPtr function);
    }

    [Flags]
    internal enum GlobalDimmerFlags : ulong
    {
        Disabled = 0,
        Enabled = 1,
    }

    // By default, [StructLayout(LayoutKind.Sequential)] is applied to structs
    internal unsafe struct GlobalDimmerFrameEndInfo
    {
        // OpenXR structure type constant (from OpenXR headers)
        internal const ulong kStructType = 1000136000;

#pragma warning disable 0414
        private ulong type;
        internal void* next;
        internal float dimmerValue;
        internal GlobalDimmerFlags flags;
#pragma warning restore 0414

        internal static GlobalDimmerFrameEndInfo Init()
            => new GlobalDimmerFrameEndInfo()
            {
                type = kStructType,
                next = null,
                dimmerValue = 0.0f,
                flags = GlobalDimmerFlags.Disabled,
            };
    }

    internal struct Result
    {
#pragma warning disable 0414
        private long value;
#pragma warning restore 0414

        public bool actuallySucceeded => value == 0;
        public bool failed => value < 0;
        public bool succeeded => value >= 0;
    }
}
#endif
