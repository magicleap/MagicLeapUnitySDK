// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2023) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;
    using static UnityEngine.XR.MagicLeap.Native.MagicLeapNativeBindings;

    public partial class MLSpace
    {
        /// <summary>
        ///  Maximum size for the name of a Magic Leap Space.
        /// </summary>
        public const uint MaxSpaceNameLength = 64;

        /// <summary>
        /// A structure containing settings for the space manager.
        /// This structure must be initialized by calling #MLSpaceManagerSettingsInit before use.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Settings
        {
            /// <summary>
            /// Version of this settings.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Initializes default values for MLSpaceManagerSettings.
            /// </summary>
            public static Settings Create(uint version = 1)
            {
                return new Settings
                {
                    Version = version,
                };
            }
        }

        /// <summary>
        /// A structure containing information about a Magic Leap Space.
        /// This structure must be initialized by calling #MLSpaceInit before use.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct Space
        {
            /// <summary>
            /// Version of the structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Name of the Space.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)MaxSpaceNameLength)]
            public string SpaceName;

            /// <summary>
            /// Unique ID of the Space.
            /// </summary>
            public MLUUID SpaceId;

            /// <summary>
            /// Type of the Space.
            /// </summary>
            public Type SpaceType;

            /// <summary>
            /// Initializes default values for MLSpace.
            /// </summary>
            public static Space Create(uint version = 1u)
            {
                return new Space
                {
                    Version = version,
                };
            }

        }

        /// <summary>
        ///  A structure containing list of #MLSpace.
        ///  This structure must be initialized by calling #MLSpaceListInit before use.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public readonly struct SpaceList
        {
            /// <summary>
            /// Version of the structure.
            /// </summary>
            public readonly uint Version;

            /// <summary>
            /// Number of Magic Leap Spaces.
            /// </summary>
            public readonly uint SpaceCount;

            /// <summary>
            /// List of Magic Leap Spaces.
            /// </summary>
            public readonly IntPtr Spaces;

            /// <summary>
            /// Initializes default values for MLSpaceList.
            /// </summary>
            SpaceList(uint version = 1u)
            {
                Version = version;
                SpaceCount = 0;
                Spaces = default;
            }
        }

        /// <summary>
        /// A collection of filters for Magic Leap Spaces.
        /// This structure must be initialized by calling #MLSpaceQueryFilterInit before use.
        /// There is no support for filters at this time.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SpaceFilter
        {
            /// <summary>
            /// Version of the structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Initializes the default values for a query filter (currently not supported).
            /// </summary>
            public static SpaceFilter Create(uint version = 1u)
            {
                return new SpaceFilter
                {
                    Version = version,
                };
            }
        }

        /// <summary>
        /// A collection of parameters to be used for localization request.
        /// This structure must be initialized by calling #MLSpaceLocalizationInfoInit before use.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SpaceLocalizationInfo
        {
            /// <summary>
            /// Version of the structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// #MLUUID of the Space into which the device attempts to localize into.
            /// </summary>
            public MLUUID SpaceId;

            /// <summary>
            /// Initializes the default values for localization info.
            /// </summary>
            public static SpaceLocalizationInfo Create(uint version = 1u)
            {
                return new SpaceLocalizationInfo
                {
                    Version = version,
                };
            }
        }

        /// <summary>
        /// A structure containing information about the device's localization state.
        /// This structure must be initialized by calling #MLSpaceLocalizationResultInit before usebefore use.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SpaceLocalizationResult
        {
            /// <summary>
            /// Version of the structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// The localization status at the time this structure was returned.
            /// </summary>
            public readonly Status LocalizationStatus;

            /// <summary>
            /// Space information.
            /// If localized(#MLSpaceLocalizationStatus_Localized) this will contain valid Space information.
            /// If not localized this field should be ignored.
            /// </summary>
            public readonly Space Space;

            /// <summary>
            /// Target space's origin relative to world origin. 
            /// If localized this will contain the identifier of the transform of the target space's origin relative to the world origin. 
            /// If not localized this will be null.
            /// </summary>
            public readonly NativeBindings.MLCoordinateFrameUID TargetSpaceOrigin;

            /// <summary>
            /// The confidence level of this localization result.
            /// </summary>
            public readonly LocalizationConfidence ConfidenceOfLocalization;

            /// <summary>
            /// Represents a bitmask of LocalizationErrorFlag.
            /// </summary>
            public readonly uint Error;

            /// <summary>
            /// Initialize default values for #MLSpaceLocalizationResult.
            /// </summary>
            public static SpaceLocalizationResult Create(uint version = 3u)
            {
                return new SpaceLocalizationResult
                {
                    Version = version,
                };
            }
        }

        /// <summary>
        /// A structure containing information about the device's localization state.
        /// </summary>
        public struct LocalizationResult
        {
            /// <summary>
            /// The localization status at the time this structure was returned.
            /// </summary>
            public Status LocalizationStatus;

            /// <summary>
            /// Space information.
            /// If localized(#MLSpaceLocalizationStatus_Localized) this will contain valid Space information.
            /// If not localized this field should be ignored.
            /// </summary>
            public Space Space;

            /// <summary>
            /// Target space's origin relative to world origin. 
            /// If localized this will contain the identifier of the transform of the target space's origin relative to the world origin. 
            /// If not localized this will be null.
            /// </summary>
            public NativeBindings.MLCoordinateFrameUID TargetSpaceOrigin;

            /// <summary>
            /// The confidence level of this localization result.
            /// </summary>
            public LocalizationConfidence ConfidenceOfLocalization;

            /// <summary>
            /// Represents a bitmask of LocalizationErrorFlag.
            /// </summary>
            public LocalizationErrorFlag Error;

            internal static LocalizationResult CreateFromSpaceLocalizationResult(SpaceLocalizationResult spaceLocalizationResult)
            {
                var localizedResult = new LocalizationResult
                {
                    ConfidenceOfLocalization = spaceLocalizationResult.ConfidenceOfLocalization, 
                    LocalizationStatus = spaceLocalizationResult.LocalizationStatus, 
                    Space = spaceLocalizationResult.Space, 
                    TargetSpaceOrigin = spaceLocalizationResult.TargetSpaceOrigin,
                    Error = (LocalizationErrorFlag) spaceLocalizationResult.Error,
                };
                return localizedResult;
            }
        }

        /// <summary>
        /// A structure containing callbacks for events related to the Space.
        /// This structure must be initialized by calling #MLSpaceCallbacksInit before use.
        /// Application can unregister(stop receiving callbacks) at any time by setting
        /// the corresponding callback to NULL.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SpaceCallbacks
        {
            /// <summary>
            /// Version of the structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// This callback will be invoked whenever there is an update to the localization status.
            /// Localization events can occur when the application requests for localization
            /// via #MLSpaceRequestLocalization or due to other events outside the control of
            /// the app such as head tracking failure, localization loss.
            /// </summary>
            public MLSpaceDelegate OnLocalizationChangedCallbacks;

            /// <summary>
            /// Initialize default values for #SpaceCallbacks.
            /// </summary>
            public static SpaceCallbacks Create(uint version = 1u)
            {
                return new SpaceCallbacks
                {
                    Version = version,
                    OnLocalizationChangedCallbacks = LocalizationChanged,
                };
            }
        }

        /// <summary>
        /// A structure containing information needed to import Magic Leap Space.
        /// This structure must be initialized by calling #MLSpaceImportInfoInit before use.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SpaceImportInfo
        {
            /// <summary>
            /// Version of the structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// Binary data size in bytes.
            /// </summary>
            public ulong Size;

            /// <summary>
            /// Binary data obtained from #MLSpaceExportSpace. 
            /// </summary>
            public IntPtr Data;

            /// <summary>
            /// Initialize default values for #SpaceImportInfo.
            /// </summary>
            public static SpaceImportInfo Create(uint version = 1u)
            {
                return new SpaceImportInfo
                {
                    Version = version,
                    Size = 0u,
                    Data = IntPtr.Zero,
                };
            }
        }

        /// <summary>
        /// A structure containing information about the imported Space.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SpaceImportOutData
        {
            /// <summary>
            /// Unique ID of the imported Space.
            /// </summary>
            public MLUUID SpaceId;
        }

        /// <summary>
        /// A structure containing information about the Space export settings.
        /// This structure must be initialized by calling #MLSpaceExportInfoInit before use.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SpaceExportInfo
        {
            /// <summary>
            /// Version of the structure.
            /// </summary>
            public uint Version;

            /// <summary>
            /// #MLUUID of the Space that needs to be exported.
            /// </summary>
            public MLUUID SpaceId;

            /// <summary>
            /// Initialize default values for #SpaceExportInfo.
            /// </summary>
            public static SpaceExportInfo Create(uint version = 1u)
            {
                return new SpaceExportInfo
                {
                    Version = version,
                };
            }
        }

        /// <summary>
        /// A structure containing information about the exported Space.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct SpaceExportOutData
        {
            /// <summary>
            /// Binary data size in bytes.
            /// </summary>
            public readonly ulong Size;

            /// <summary>
            /// Space data.
            /// This is binary data and typically does not include a terminating null
            /// character.
            /// </summary>
            public IntPtr Data;


            public static SpaceExportOutData Create()
            {
                return new SpaceExportOutData()
                {
                    Data = IntPtr.Zero,
                };
            }

            public static byte[] GetData(SpaceExportOutData data)
            {
                if (data.Data == IntPtr.Zero)
                    return null;
                byte[] bytes = MLConvert.MarshalUnmanagedArray<byte>(data.Data, (int)data.Size);
                return bytes;
            }
        }

        /// <summary>
        /// Space data used when importing a space.
        /// </summary>
        public struct SpaceData
        {
            /// <summary>
            /// Size of space data in bytes.
            /// </summary>
            public ulong Size;

            /// <summary>
            /// Binary space data.
            /// </summary>
            public byte[] Data;
        }

        /// <summary>
        /// Space data returned when we successfully import space.
        /// </summary>
        public struct SpaceInfo
        {
            public MLUUID SpaceId;
        }
    }
}

