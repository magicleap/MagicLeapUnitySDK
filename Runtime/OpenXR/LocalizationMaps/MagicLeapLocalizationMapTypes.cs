using System;
using System.Linq;
using System.Text;

namespace MagicLeap.OpenXR.Features.LocalizationMaps
{
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
        HeadPoseBit = 32
    }

    public enum LocalizationMapState
    {
        NotLocalized,
        Localized,
        LocalizationPending,
        SleepingBeforeRetry
    }

    public enum LocalizationMapType
    {
        OnDevice = 0,
        Cloud = 1
    }

    public struct LocalizationMap
    {
        public string Name;

        public string MapUUID;

        public LocalizationMapType MapType;

        internal LocalizationMap(XrLocalizationMap map)
        {
            unsafe
            {
                MapUUID = map.MapUUID.ToString();
                Name = Encoding.UTF8.GetString(map.Name, XrLocalizationMap.NameLength).Trim('\0');
                MapType = (LocalizationMapType)map.MapType;
            }
        }
    }

    public struct LocalizationEventData
    {
        public LocalizationMapState State;

        public LocalizationMap Map;

        public LocalizationMapConfidence Confidence;

        public LocalizationMapErrorFlags[] Errors;

        internal LocalizationEventData(XrEventDataLocalizationChanged data)
        {
            State = (LocalizationMapState)data.MapState;
            Confidence = (LocalizationMapConfidence)data.Confidence;
            Map = new LocalizationMap(data.LocalizationMap);
            Errors = ((XrLocalizationMapErrorFlags[])Enum.GetValues(typeof(XrLocalizationMapErrorFlags))).Where(flag => data.ErrorFlags.HasFlag(flag)).Select(flag => (LocalizationMapErrorFlags)flag).ToArray();
        }
    }
}
