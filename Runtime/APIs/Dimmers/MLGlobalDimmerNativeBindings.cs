using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public static partial class MLGlobalDimmer
    {
        private class NativeBindings : MagicLeapNativeBindings
        {
            public const int XR_TYPE_GLOBAL_DIMMER_FRAME_END_INFO_ML = 1000136000;

            [StructLayout(LayoutKind.Sequential)]
            public readonly struct XrGlobalDimmerFrameEndInfoML
            {
                public readonly int Type;
                public readonly IntPtr Next;
                public readonly float DimmerValue;
                public readonly long EnabledFlag;

                public XrGlobalDimmerFrameEndInfoML(float dimmerValue, bool enabled)
                {
                    Type = XR_TYPE_GLOBAL_DIMMER_FRAME_END_INFO_ML;
                    Next = IntPtr.Zero;
                    DimmerValue = dimmerValue;
                    EnabledFlag = enabled ? 1L : 0L;
                }
            }

            /// <summary>
            /// Sets the GlobalDimmer value via the Magic Leap XR Plugin
            /// </summary>
            /// <param name="dimmerValue">Level of opacity, between 0 and 1, of global dimmer.</param>
            [DllImport(UnityMagicLeapDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void UnityMagicLeap_RenderingSetGlobalDimmerValue(float dimmerValue);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLOpenXRSetGlobalDimmerFrameEndInfoParams(XrGlobalDimmerFrameEndInfoML globalDimmerFrameEndInfoML);
        }
    }
}
