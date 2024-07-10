using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;
using static UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.MLHeadTracking.NativeBindings;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MLHeadTracking")]
    internal partial class MLHeadTracking
    {
        private static ulong handle = MagicLeapNativeBindings.InvalidHandle;
        private static MLHeadTrackingStaticData staticData;
        private static MLHeadTrackingStateEx state;
        private static ulong mapEvents;

        private static void CreateHandle()
        {
            if(handle == MagicLeapNativeBindings.InvalidHandle)
            {
                MLResult.DidNativeCallSucceed(NativeBindings.MLHeadTrackingCreate(out handle));
                state = default;
                staticData = default;
            }
        }

        public static ulong Handle
        {
            get
            {
                CreateHandle();
                return handle;
            }
        }

        public static bool IsAvailable()
        {
            CreateHandle();
            return handle != MagicLeapNativeBindings.InvalidHandle;
        }

        public static bool TryGetStateEx(out InputSubsystem.Extensions.MLHeadTracking.StateEx headTrackingState)
        {
            headTrackingState = default;
            CreateHandle();

            if (!MLResult.DidNativeCallSucceed(NativeBindings.MLHeadTrackingGetStateEx(handle, out state)))
                return false;

            headTrackingState = new InputSubsystem.Extensions.MLHeadTracking.StateEx(state);
            return true;
        }

        public static bool GetStaticData(out MagicLeapNativeBindings.MLCoordinateFrameUID outUID)
        {
            outUID = MagicLeapNativeBindings.MLCoordinateFrameUID.EmptyFrame;
            CreateHandle();

            if (!MLResult.DidNativeCallSucceed(NativeBindings.MLHeadTrackingGetStaticData(handle, out staticData)))
                return false;

            outUID = staticData.coord_frame_head;
            return true;
        }

        public static bool TryGetMapEvents(out InputSubsystem.Extensions.MLHeadTracking.MapEvents outMapEvents)
        {
            outMapEvents = default;
            CreateHandle();
            
            if (!MLResult.DidNativeCallSucceed(NativeBindings.MLHeadTrackingGetMapEvents(handle, out mapEvents)))
                return false;
            
            outMapEvents = (InputSubsystem.Extensions.MLHeadTracking.MapEvents)mapEvents;
            return (outMapEvents != 0);
        }

        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.NativeBindings")]

        internal class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLHeadTrackingCreate(out ulong handle);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLHeadTrackingDestroy(ulong handle);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLHeadTrackingGetStateEx(ulong handle, out MLHeadTrackingStateEx state);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLHeadTrackingGetStaticData(ulong handle, out MLHeadTrackingStaticData data);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLHeadTrackingGetMapEvents(ulong handle, out ulong mapEvents);
        }
    }
}
