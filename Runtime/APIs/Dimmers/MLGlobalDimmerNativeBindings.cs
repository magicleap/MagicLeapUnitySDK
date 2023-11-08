using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public static partial class MLGlobalDimmer
    {
        private class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// Sets the GlobalDimmer value via the Magic Leap XR Plugin
            /// </summary>
            /// <param name="dimmerValue">Level of opacity, between 0 and 1, of global dimmer.</param>
            [DllImport(UnityMagicLeapDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void UnityMagicLeap_RenderingSetGlobalDimmerValue(float dimmerValue);
        }
    }
}
