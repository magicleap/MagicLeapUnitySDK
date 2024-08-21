using System;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.MagicLeap.Native;

namespace MagicLeap.OpenXR.Features.EyeTracker
{
    public partial class MagicLeapEyeTrackerFeature
    {
        internal const int SpecVersion = 1;

        private ulong eyeTracker;

        public unsafe void CreateEyeTracker()
        {
            Utils.OpenXRStructHelpers<XrEyeTrackerCreateInfo>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerCreateInfo, out var eyeTrackerCreateInfo);

            var resultCode = nativeFunctions.XrCreateEyeTracker(AppSession, in eyeTrackerCreateInfo, out eyeTracker);
            Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrCreateEyeTracker));
        }

        public unsafe void DestroyEyeTracker()
        {
            var resultCode = nativeFunctions.XrDestroyEyeTracker(eyeTracker);
            Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrDestroyEyeTracker));
        }

        public EyeTrackerData GetEyeTrackerData()
        {
            EyeTrackerData eyeTrackerData = new EyeTrackerData();

            eyeTrackerData.StaticData = GetStaticData();
            eyeTrackerData.GeometricData = GetGeometricData();
            eyeTrackerData.PupilData = GetPupilData();
            eyeTrackerData.GazeBehaviorData = GetGazeBehavior();

            return eyeTrackerData;
        }

        private unsafe StaticData GetStaticData()
        {
            Utils.OpenXRStructHelpers<XrEyeTrackerStaticDataGetInfo>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerStaticDataGetInfo, out var staticDataGetInfo);
            Utils.OpenXRStructHelpers<XrEyeTrackerStaticData>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerStaticData, out var eyeTrackerStaticData);

            var resultCode = nativeFunctions.XrGetEyeTrackerStaticData(eyeTracker, in staticDataGetInfo, ref eyeTrackerStaticData);
            Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetEyeTrackerStaticData));

            var staticData = new StaticData();
            staticData.Result = resultCode;
            staticData.EyeWidthMax = eyeTrackerStaticData.EyeWidthMax;
            staticData.EyeHeightMax = eyeTrackerStaticData.EyeHeightMax;

            return staticData;
        }

        private unsafe GeometricData[] GetGeometricData()
        {
            Utils.OpenXRStructHelpers<XrEyeTrackerGeometricDataGetInfo>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerGeometricDataGetInfo, out var geometricDataGetInfo);

            var nativeGeometricData = new NativeArray<XrEyeTrackerGeometricData>(2, Allocator.Temp);

            Utils.OpenXRStructHelpers<XrEyeTrackerGeometricData>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerGeometricData, out var leftGeometricData);
            nativeGeometricData[0] = leftGeometricData;

            Utils.OpenXRStructHelpers<XrEyeTrackerGeometricData>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerGeometricData, out var rightGeometricData);
            nativeGeometricData[1] = rightGeometricData;

            var resultCode = nativeFunctions.XrGetEyeTrackerGeometricData(eyeTracker, in geometricDataGetInfo, 2, out uint outputCount, (XrEyeTrackerGeometricData*)nativeGeometricData.GetUnsafePtr());
            Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetEyeTrackerGeometricData));

            var geometricData = new GeometricData[2];

            for (int i = 0; i < 2; i++)
            {
                geometricData[i].Result = resultCode;
                geometricData[i].Eye = (Eye)nativeGeometricData[i].Eye;
                geometricData[i].Time = nativeGeometricData[i].Time;
                geometricData[i].Valid = nativeGeometricData[i].Valid;
                geometricData[i].EyeOpenness = nativeGeometricData[i].EyeOpenness;
                geometricData[i].EyeInSkullPosition = nativeGeometricData[i].EyeInSkullPosition;
            }

            return geometricData;
        }

        private unsafe PupilData[] GetPupilData()
        {
            Utils.OpenXRStructHelpers<XrEyeTrackerPupilDataGetInfo>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerPupilDataGetInfo, out var pupilDataGetInfo);

            var nativePupilData = new NativeArray<XrEyeTrackerPupilData>(2, Allocator.Temp);

            Utils.OpenXRStructHelpers<XrEyeTrackerPupilData>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerPupilData, out var leftNativePupilData);
            nativePupilData[0] = leftNativePupilData;

            Utils.OpenXRStructHelpers<XrEyeTrackerPupilData>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerPupilData, out var rightNativePupilData);
            nativePupilData[1] = rightNativePupilData;

            var resultCode = nativeFunctions.XrGetEyeTrackerPupilData(eyeTracker, in pupilDataGetInfo, 2, out uint outputCount, (XrEyeTrackerPupilData*)nativePupilData.GetUnsafePtr());
            Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetEyeTrackerPupilData));

            PupilData[] pupilData = new PupilData[2];

            for (int i = 0; i < 2; i++)
            {
                pupilData[i].Result = resultCode;
                pupilData[i].Eye = (Eye)nativePupilData[i].Eye;
                pupilData[i].Time = nativePupilData[i].Time;
                pupilData[i].Valid = nativePupilData[i].Valid;
                pupilData[i].PupilDiameter = nativePupilData[i].PupilDiameter;
            }

            return pupilData;
        }

        private unsafe GazeBehavior GetGazeBehavior()
        {
            Utils.OpenXRStructHelpers<XrEyeTrackerGazeBehaviorGetInfo>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerGazeBehaviorGetInfo, out var gazeGetInfo);
            Utils.OpenXRStructHelpers<XrEyeTrackerGazeBehaviorMetaData>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerGazeBehaviorMetaData, out var gazeMetaDataNative);

            Utils.OpenXRStructHelpers<XrEyeTrackerGazeBehavior>.Create(XrEyeTrackerStructTypes.XrTypeEyeTrackerGazeBehavior, out var gazeDataNative);
            gazeDataNative.Next = (IntPtr)(&gazeMetaDataNative);

            var resultCode = nativeFunctions.XrGetEyeTrackerGazeBehavior(eyeTracker, in gazeGetInfo, ref gazeDataNative);
            Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetEyeTrackerGazeBehavior));

            gazeMetaDataNative = Marshal.PtrToStructure<XrEyeTrackerGazeBehaviorMetaData>(gazeDataNative.Next);

            var gazeMetaData = new GazeBehaviorMetaData
            {
                Valid = gazeMetaDataNative.Valid,
                Amplitude = gazeMetaDataNative.Amplitude,
                Direction = gazeMetaDataNative.Direction,
                Velocity = gazeMetaDataNative.Velocity
            };

            var gazeData = new GazeBehavior
            {
                Result = resultCode,
                MetaData = gazeMetaData,
                Time = gazeDataNative.Time,
                Valid = gazeDataNative.Valid,
                GazeBehaviorType = (GazeBehaviorType)gazeDataNative.GazeBehaviorType,
                OnsetTime = gazeDataNative.OnsetTime,
                Duration = gazeDataNative.Duration
            };

            return gazeData;
        }
    }
}
