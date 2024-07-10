using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

using static UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.MLEyes.NativeBindings;

namespace MagicLeap.OpenXR.LegacySupport
{
    internal static class MLEyeTracking
    {
        private static ulong handle;
        private static MLEyeTrackingStaticData staticData;
        private static MLEyeTrackingStateEx state;

        public static void Start()
        {
            MLResult.DidNativeCallSucceed(NativeBindings.MLEyeTrackingCreate(out handle));
            state = MLEyeTrackingStateEx.Init();
        }

        public static void Stop()
        {
            MLResult.DidNativeCallSucceed(NativeBindings.MLEyeTrackingDestroy(handle));
        }

        public static bool TryGetState(out InputSubsystem.Extensions.MLEyes.State eyeTrackingState)
        {
            eyeTrackingState = default;
            if (!MLResult.DidNativeCallSucceed(NativeBindings.MLEyeTrackingGetStateEx(handle, out state)))
            {
                return false;
            }
            eyeTrackingState = new InputSubsystem.Extensions.MLEyes.State(state);
            return true;
        }

        public static void GetStaticData(out InputSubsystem.Extensions.MLEyes.StaticData eyeTrackingStaticData)
        {
            MLResult.DidNativeCallSucceed(NativeBindings.MLEyeTrackingGetStaticData(handle, out staticData));
            eyeTrackingStaticData = new()
            {
                Vergence = staticData.vergence,
                LeftCenter = staticData.left_center,
                RightCenter = staticData.right_center
            };
        }

        private abstract class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeTrackingCreate(out ulong handle);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeTrackingDestroy(ulong handle);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeTrackingGetStaticData(ulong handle, out MLEyeTrackingStaticData data);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLEyeTrackingGetStateEx(ulong handle, out MLEyeTrackingStateEx state);
        }
    }
}
