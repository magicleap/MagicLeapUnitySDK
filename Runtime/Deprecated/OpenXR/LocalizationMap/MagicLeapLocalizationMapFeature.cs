// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapLocalizationMapNativeTypes;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRNativeDelegates;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif // UNITY_EDITOR

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.LocalizationMaps")]
    public partial class MagicLeapLocalizationMapFeature : MagicLeapOpenXRFeatureWithInterception<MagicLeapLocalizationMapFeature>
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_localizationmap";
        public const string ExtensionName = "XR_ML_localization_map";
        
        private MagicLeapLocalizationMapNativeFunctions nativeFunctions;

        private XrEventDataLocalizationChanged localizationEventData;
        
        public static event Action<LocalizationEventData> OnLocalizationChangedEvent;
        
        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(ExtensionName))
            {
                var result = base.OnInstanceCreate(xrInstance);
                if (result)
                {
                    nativeFunctions = MagicLeapNativeFunctionsBase.Create<MagicLeapLocalizationMapNativeFunctions>(InstanceProcAddr, AppInstance);
                }
                return result;
            }
            Debug.LogError($"{ExtensionName} is not enabled. Disabling {nameof(MagicLeapLocalizationMapFeature)}");
            return false;
        }

        protected override void MarkFunctionsToIntercept()
        {
            InterceptPollEvent = true;
        }

        internal override XrResult OnPollEvent(ulong instance, IntPtr eventBuffer, XrPollEvent origPollEvent)
        {
            var result = base.OnPollEvent(instance, eventBuffer, origPollEvent);
            if (result != XrResult.Success)
            {
                return result;
            }
            unsafe
            {
                var eventData = (XrEventBuffer*)eventBuffer;
                if (eventData->Type != (ulong)XrLocalizationMapStructTypes.XrTypeEventDataLocalizationChanged)
                {
                    return result;
                }

                fixed (XrEventDataLocalizationChanged* target = &localizationEventData)
                {
                    UnsafeUtility.MemCpy(target, eventBuffer.ToPointer(), sizeof(XrEventDataLocalizationChanged));
                }
                OnLocalizationChangedEvent?.Invoke(new LocalizationEventData(localizationEventData));
            }

            return result;
        }
        public XrResult EnableLocalizationEvents(bool enableEvents)
        {
            unsafe
            {
                var localizationEnableInfo = new XrLocalizationEnableEventsInfo()
                {
                    Type = XrLocalizationMapStructTypes.XrTypeLocalizationEnableEventsInfo,
                    Enabled = (enableEvents) ? 1U : 0
                };
                return nativeFunctions.XrEnableLocalizationEvents(AppSession, in localizationEnableInfo);
            }
        }

        public XrResult GetLocalizationMapsList(out LocalizationMap[] maps)
        {
            maps = null;
            unsafe
            {
                var baseHeader = new XrLocalizationMapQueryInfoBaseHeader();
                var baseHeaderAddr = &baseHeader;
                var resultCode = nativeFunctions.XrQueryLocalizationMaps(AppSession, in baseHeaderAddr, 0, out var mapCount, null);
                if (resultCode != XrResult.Success)
                {
                    return resultCode;
                }

                var nativeArray = new NativeArray<XrLocalizationMap>((int)mapCount, Allocator.Temp);
                var defaultLocalizationMap = new XrLocalizationMap
                {
                    Type = XrLocalizationMapStructTypes.XrTypeLocalizationMap,
                };
                NativeCopyUtility.FillArrayWithValue(nativeArray, defaultLocalizationMap);
                resultCode = nativeFunctions.XrQueryLocalizationMaps(AppSession, in baseHeaderAddr, mapCount, out mapCount, (XrLocalizationMap*) nativeArray.GetUnsafePtr());
                if (resultCode != XrResult.Success)
                {
                    return resultCode;
                }
                maps = nativeArray.Select(element => new LocalizationMap(element)).ToArray();
            }
            return XrResult.Success;
        }

        public XrResult RequestMapLocalization(string mapId)
        {
            var xrUuid = new XrUUID(mapId);
            unsafe
            {
                var requestInfo = new XrMapLocalizationRequestInfo
                {
                    Type = XrLocalizationMapStructTypes.XrTypeMapLocalizationRequestInfo,
                    MapUUID = xrUuid
                };
                var xrResult = nativeFunctions.XrRequestMapLocalization(AppSession, in requestInfo);
                Utils.DidXrCallSucceed(xrResult, nameof(nativeFunctions.XrRequestMapLocalization));
                return xrResult;
            }
        }
        
        [Obsolete("ExportLocalizatioMap will be deprecated. Use ExportLocalizationMap instead.")]
        public XrResult ExportLocalizatioMap(string mapId, out byte[] mapData) => ExportLocalizationMap(mapId, out mapData);

        public XrResult ExportLocalizationMap(string mapId, out byte[] mapData)
        {
            // create map handle
            mapData = Array.Empty<byte> ();
            var mapUuid = new XrUUID(mapId);

            unsafe
            {
                var resultCode = nativeFunctions.XrCreateExportedLocalizationMap(AppSession, in mapUuid, out var mapHandle);
                if (!Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrCreateExportedLocalizationMap)))
                {
                    return resultCode;
                }
                // Create exported map data
                uint mapDataSize = 0;
                resultCode = nativeFunctions.XrGetExportedLocalizationMapData(mapHandle, mapDataSize, out mapDataSize, null);

                if (mapDataSize <= 0)
                {
                    return resultCode;
                }

                var nativeMapData = new NativeArray<byte>((int)mapDataSize, Allocator.Temp);
                resultCode = nativeFunctions.XrGetExportedLocalizationMapData(mapHandle, mapDataSize, out mapDataSize, (byte*)nativeMapData.GetUnsafePtr());
                if (!Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetExportedLocalizationMapData)))
                {
                    return resultCode;
                }

                mapData = nativeMapData.ToArray();
                // destroy map handle
                resultCode = nativeFunctions.XrDestroyExportedLocalizationMap(mapHandle);
                Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrDestroyExportedLocalizationMap));
                return resultCode;
            }
        }

        public XrResult ImportLocalizationMap(byte[] mapData, out string mapId)
        {
            unsafe
            {
                mapId = default;
                var mapDataArray = new NativeArray<byte>(mapData, Allocator.Temp);
                var importInfo = new XrLocalizationMapImportInfo
                {
                    Type = XrLocalizationMapStructTypes.XrTypeLocalizationMapImportInfo,
                    Size = (uint)mapDataArray.Length,
                    Data = (byte*)mapDataArray.GetUnsafePtr(),
                };
                var resultCode = nativeFunctions.XrImportLocalizationMap(AppSession, in importInfo, out var mapUUID);
                if (!Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrImportLocalizationMap)))
                {
                    return resultCode;
                }

                mapId = mapUUID.ToString();
                return resultCode;
            }
        }

        public bool GetLatestLocalizationMapData(out LocalizationEventData data)
        {
            data = new LocalizationEventData();
            if (localizationEventData.Type != XrLocalizationMapStructTypes.XrTypeEventDataLocalizationChanged)
            {
                return false;
            }
            data = new LocalizationEventData(localizationEventData);
            return true;
        }
    }
}
