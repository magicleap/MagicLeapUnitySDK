// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_OPENXR_1_9_0_OR_NEWER
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif // UNITY_EDITOR

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using static MagicLeapLocalizationMapFeature.NativeBindings;

#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Localization Maps",
        Desc = "Import/Export and manage localization maps.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = ExtensionName
    )]
#endif // UNITY_EDITOR
    public partial class MagicLeapLocalizationMapFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_localizationmap";
        public const string ExtensionName = "XR_ML_localization_map";
        const uint LocalizationMapNameSize = 64;

        #region Callbacks
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        internal delegate void OnEventDataLocalizationChangedCallback(ref XrEventDataLocalizationChangedML eventData);

        public delegate void OnLocalizationMapChangedDelegate(LocalizationEventData data);
        private static event OnLocalizationMapChangedDelegate OnLocalizationChanged = delegate { };
        public static event OnLocalizationMapChangedDelegate OnLocalizationChangedEvent
        {
            add => OnLocalizationChanged += value;
            remove => OnLocalizationChanged -= value;
        }
        #endregion
        
        #region Enums
        public enum LocalizationMapState
        {
            NotLocalized,
            Localized,
            LocalizationPending,
            SleepingBeforeRetry,
        }

        public enum LocalizationMapType
        {
            OnDevice = 0,
            Cloud = 1,
        }

        public enum LocalizationMapConfidence
        {
            Poor = 0,
            Fair = 1,
            Good = 2,
            Excellent = 3
        }

        public enum LocalizationMapErrorFlags
        {
            UnknownBit = 1,
            OutOfMappedAreaBit = 2,
            LowFeatureCountBit = 4,
            ExcessiveMotionBit = 8,
            LowLightBit = 16,
            HeadposeBit = 32
        }
        #endregion

        #region Structs
        public struct LocalizationMap
        {
            public string Name;

            public string MapUUID;

            public LocalizationMapType MapType;

            internal LocalizationMap(XrLocalizationMapML map)
            {
                Name = XrLocalizationMapML.GetName(map);
                MapUUID = XrLocalizationMapML.GetMapUuid(map);
                MapType = map.MapType;
            }
        }

        public struct LocalizationEventData
        {
            public LocalizationMapState State;

            public LocalizationMap Map;

            public LocalizationMapConfidence Confidence;

            public LocalizationMapErrorFlags[] Errors;

            internal LocalizationEventData(XrEventDataLocalizationChangedML data)
            {
                State = (LocalizationMapState)data.State;
                Confidence = (LocalizationMapConfidence)data.Confidence;
                Map = new LocalizationMap(data.Map);
                var errors = new List<LocalizationMapErrorFlags>();
                foreach (LocalizationMapErrorFlags flag in Enum.GetValues(typeof(LocalizationMapErrorFlags)))
                {
                    if ((data.ErrorFlags & (ulong)flag) != 0)
                        errors.Add(flag);
                }
                Errors = errors.ToArray();
            }
        }
        #endregion

        #region Methods
        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            if (OpenXRRuntime.IsExtensionEnabled(ExtensionName))
            {
                return base.OnInstanceCreate(xrInstance);
            }
            Debug.LogError($"{ExtensionName} is not enabled. Disabling {nameof(MagicLeapLocalizationMapFeature)}");
            return false;
        }

        protected override string GetFeatureId() => FeatureId;

        public XrResult EnableLocalizationEvents(bool enableEvents)
        {
            return MLOpenXREnableLocalizationEvents(enableEvents);
        }

        public XrResult GetLocalizationMapsList(out LocalizationMap[] maps)
        {
            NativeArray<XrLocalizationMapML> nativeArray = new();
            maps = null;
            unsafe
            {
                var resultCode = MLOpenXRQueryLocalizationMaps(0, out uint mapCount, (XrLocalizationMapML*)nativeArray.GetUnsafePtr());
                if (resultCode != XrResult.Success)
                    return resultCode;

                nativeArray = new NativeArray<XrLocalizationMapML>((int)mapCount, Allocator.Temp);
                for (int i = 0; i < nativeArray.Length; i++)
                {
                    nativeArray[i] = XrLocalizationMapML.Create();
                }
                resultCode = MLOpenXRQueryLocalizationMaps(mapCount, out mapCount, (XrLocalizationMapML*)nativeArray.GetUnsafePtr());
                if (resultCode != XrResult.Success)
                    return resultCode;
            }
            maps = nativeArray.Select(element => new LocalizationMap(element)).ToArray();
            return XrResult.Success;
        }

        public XrResult RequestMapLocalization(string mapId)
        {
            XrUUID xrUuid = new XrUUID(mapId);
            return MLOpenXRRequestMapLocalization(xrUuid);
        }

        public XrResult ExportLocalizatioMap(string mapId, out byte[] mapData)
        {
            // create map handle
            ulong mapHandle;
            mapData = Array.Empty<byte> ();
            XrUUID mapUuid = new XrUUID(mapId);
            var resultCode = MLOpenXRCreateExportedLocalizationMap(in mapUuid, out mapHandle);

            // Create exported map data
            uint mapDataSize = 0;
            resultCode = MLOpenXRGetExportedLocalizationMapData(mapHandle, mapDataSize, out mapDataSize, mapData);

            if (mapDataSize > 0)
            {
                mapData = new byte[mapDataSize];
                MLOpenXRGetExportedLocalizationMapData(mapHandle, mapDataSize, out mapDataSize, mapData);

                // destroy map handle
                resultCode = MLOpenXRDestroyExportedLocalizationMap(mapHandle);
            }
            return resultCode;
        }

        public XrResult ImportLocalizationMap(byte[] mapData, out string mapId)
        {
            var requestInfo = new XrLocalizationMapImportInfoML(mapData);
            var resultCode = MLOpenXRImportLocalizationMap(ref requestInfo, out XrUUID xrUUID);
            mapId = xrUUID.ToString();
            return resultCode;
        }

        public bool GetLatestLocalizationMapData(out LocalizationEventData data)
        {
            data = new();
            XrEventDataLocalizationChangedML localizationData;
            localizationData.Map = XrLocalizationMapML.Create();

            var resultCode = MLOpenXRGetLocalizationMapData(out localizationData);
            if (resultCode == true)
                data = new LocalizationEventData(localizationData);
            return resultCode;
        }
        #endregion
    }
}
#endif // UNITY_OPENXR_1_9_0_OR_NEWER
