using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLGlobalDimmer
    {
        /// <summary>
        /// Set the manual value for the global dimmer Updates are reflected in
        /// the next client submitted frame. Global dimmer update through this
        /// func is applicable only if auto dimmer is disabled. Any smooth ramping
        /// from auto dimmer to application final dimmer should be handled by the
        /// application itself. Since the dimmer value set through this func is
        /// reflected only in the next client submitted frame, the smooth ramp
        /// stepping interval is equal to current application frame rate
        /// </summary>
        /// <param name="dimmerValue">dimmer value in valid range [0.0 to 1.0].
        /// 0.0 corresponds no global dimming while 1.0 corresponds to max global
        /// dimming.</param>
        public static MLResult SetValue(float dimmerValue)
        {
            float clampedValue = Mathf.Clamp(dimmerValue, 0.0f, 1.0f);
            NativeBindings.UnityMagicLeap_RenderingSetGlobalDimmerValue(clampedValue);
            return MLResult.Create(MLResult.Code.Ok);
        }
    }
}
