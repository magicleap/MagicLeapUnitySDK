using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLGlobalDimmer
    {
        private class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// Sets new global dimmer value, between 0 and 1.
            /// </summary>
            /// <param name="dimmerValue">Level of opacity, between 0 and 1, of global dimmer.</param>
            [DllImport(UnityMagicLeapDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void UnityMagicLeap_RenderingSetGlobalDimmerValue(float dimmerValue);
        }
    }
}
