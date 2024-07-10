using System;

namespace MagicLeap.OpenXR.Features.LocalizationMaps
{
    internal enum XrLocalizationMapStructTypes : ulong
    {
        XrTypeLocalizationMap = 1000139000U,
        XrTypeEventDataLocalizationChanged = 1000139001U,
        XrTypeMapLocalizationRequestInfo = 1000139002U,
        XrTypeLocalizationMapImportInfo = 1000139003U,
        XrTypeLocalizationEnableEventsInfo = 1000139004U
    }

    internal enum XrLocalizationMapType
    {
        Device,
        Cloud
    }

    internal enum XrLocalizationMapState : uint
    {
        NotLocalized,
        Localized,
        LocalizationPending,
        LocalizationPendingBeforeRetry
    }

    internal enum XrLocalizationMapConfidence : uint
    {
        Poor,
        Fair,
        Good,
        Excellent
    }

    [Flags]
    internal enum XrLocalizationMapErrorFlags : ulong
    {
        XrLocalizationMapErrorUnknown = 0x00000001,
        XrLocalizationMapErrorOutOfMappedArea = 0x00000002,
        XrLocalizationMapErrorLowFeatureCount = 0x00000004,
        XrLocalizationMapErrorExcessiveMotion = 0x00000008,
        XrLocalizationMapErrorLowLight = 0x00000010,
        XrLocalizationMapErrorHeadPose = 0x00000020
    }


    internal unsafe struct XrLocalizationMap
    {
        internal const int NameLength = 64;
        internal XrLocalizationMapStructTypes Type;
        internal IntPtr Next;
        internal fixed byte Name[NameLength];
        internal XrUUID MapUUID;
        internal XrLocalizationMapType MapType;
    }

    internal struct XrEventDataLocalizationChanged
    {
        internal XrLocalizationMapStructTypes Type;
        internal IntPtr Next;
        internal ulong Session;
        internal XrLocalizationMapState MapState;
        internal XrLocalizationMap LocalizationMap;
        internal XrLocalizationMapConfidence Confidence;
        internal XrLocalizationMapErrorFlags ErrorFlags;
    }

    internal struct XrLocalizationMapQueryInfoBaseHeader
    {
        internal XrLocalizationMapStructTypes Type;
        internal IntPtr Next;
    }

    internal struct XrMapLocalizationRequestInfo
    {
        internal XrLocalizationMapStructTypes Type;
        internal IntPtr Next;
        internal XrUUID MapUUID;
    }

    internal unsafe struct XrLocalizationMapImportInfo
    {
        internal XrLocalizationMapStructTypes Type;
        internal IntPtr Next;
        internal uint Size;
        internal byte* Data;
    }

    internal struct XrLocalizationEnableEventsInfo
    {
        internal XrLocalizationMapStructTypes Type;
        internal IntPtr Next;
        internal uint Enabled;
    }
}
