// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_OPENXR_1_9_0_OR_NEWER
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
        internal const ulong StructType = 1000135000;
#pragma warning disable 0414
        private ulong type;
        internal void* Next;
        internal float FocusDistance;
        internal FrameInfoFlags Flags;
#pragma warning restore 0414

        internal static FrameEndInfo Init()
            => new FrameEndInfo
            {
                type = StructType,
                Next = null,
                FocusDistance = 0.0f,
                Flags = FrameInfoFlags.None,
            };
    }

    internal static class FunctionPrototypes
    {
#if PLATFORM_STANDALONE_WIN || UNITY_EDITOR_WIN
        private const CallingConvention callConv = CallingConvention.StdCall;
#else
        private const CallingConvention callConv = CallingConvention.Cdecl;
#endif
        [UnmanagedFunctionPointer(callConv, CharSet = CharSet.Ansi)]
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
        internal const ulong StructType = 1000136000;

#pragma warning disable 0414
        private ulong type;
        internal void* Next;
        internal float DimmerValue;
        internal GlobalDimmerFlags Flags;
#pragma warning restore 0414

        internal static GlobalDimmerFrameEndInfo Init()
            => new GlobalDimmerFrameEndInfo()
            {
                type = StructType,
                Next = null,
                DimmerValue = 0.0f,
                Flags = GlobalDimmerFlags.Disabled,
            };
    }

    internal struct Result
    {
#pragma warning disable 0414
        private long value;
#pragma warning restore 0414

        public bool ActuallySucceeded => value == 0;
        public bool Failed => value < 0;
        public bool Succeeded => value >= 0;
    }

    internal unsafe struct XrUUID
    {
        private static readonly int[] hyphenIndices = new int[] { 8, 13, 18, 23 };

        public fixed byte Data[16];

        public override string ToString()
        {
            string idString = string.Empty;

            for (int i = 0; i < 16; i++)
            {
                idString += string.Format("{0:x2}", this.Data[i]);
            }

            foreach (int i in hyphenIndices)
                idString = idString.Insert(i, "-");

            return idString;
        }

        internal XrUUID(string id)
        {
            id = id.Replace("-", string.Empty);
            fixed (byte* b = this.Data)
            {
                StringToByteArray(id, b);
            }
        }

        private static void StringToByteArray(string hex, byte* bytes)
        {
            int numChars = hex.Length;
            for (int i = 0; i < numChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
    }
}
#endif
