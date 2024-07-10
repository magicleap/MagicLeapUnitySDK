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
using System.Text;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.XrTime")]
    public sealed class XrTime
    {
        public long Value { get; private set; }
        internal XrTime(long val) { Value = val; }

        public static implicit operator long(XrTime xrTime) => xrTime.Value;
        public static implicit operator XrTime(long val) => new(val);
        public override string ToString() => Value.ToString();
    }

    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.FunctionPrototypes")]

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
            StringBuilder idString = new StringBuilder(20);

            for (int i = 0; i < 16; i++)
            {
                idString.AppendFormat("{0:x2}", this.Data[i]);
            }

            foreach (int i in hyphenIndices)
                idString.Insert(i, "-");

            return idString.ToString();
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

    internal unsafe struct FeatureLifecycleNativeListener
    {
        internal delegate* unmanaged [Cdecl] <ulong, IntPtr, void> OnInstanceCreated;
        internal delegate* unmanaged [Cdecl] <ulong, void> OnInstanceDestroyed;
        internal delegate* unmanaged [Cdecl] <ulong, void> OnSessionCreate;
        internal delegate* unmanaged [Cdecl] <ulong, void> OnSessionDestroy;
        internal delegate* unmanaged [Cdecl] <ulong, void> OnAppSpaceChanged;
        internal delegate* unmanaged [Cdecl] <long, void> OnPredictedDisplayTimeChanged;
        
        public void InstanceCreated(ulong instance, IntPtr hookAddr)
        {
            if (OnInstanceCreated != null)
            {
                OnInstanceCreated(instance, hookAddr);
            }
        }

        public void InstanceDestroyed(ulong instance)
        {
            if (OnInstanceDestroyed != null)
            {
                OnInstanceDestroyed(instance);
            }
        }

        public void SessionCreated(ulong session)
        {
            if (OnSessionCreate != null)
            {
                OnSessionCreate(session);
            }
        }

        public void SessionDestroyed(ulong session)
        {
            if (OnSessionDestroy != null)
            {
                OnSessionDestroy(session);
            }
        }

        public void AppSpaceChanged(ulong space)
        {
            if (OnAppSpaceChanged != null)
            {
                OnAppSpaceChanged(space);
            }
        }

        public void PredictedDisplayTimeChanged(long time)
        {
            if (OnPredictedDisplayTimeChanged != null)
            {
                OnPredictedDisplayTimeChanged(time);
            }
        }
    }
}
#endif
