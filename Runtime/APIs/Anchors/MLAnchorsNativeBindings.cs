// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%


namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using Native;

    /// <summary>
    ///	Spatial Anchor management is closely tied to the selected mapping mode on the device. The modes are
    ///	mutually exclusive and affect the functionality of these APIs. 
    ///	The available mapping modes are:
    ///	
    ///	On-Device Mode - A persistent mode in which anchors are persisted locally and will be available
    ///	in multiple sessions when localized to the same space in which they were published.
    ///	
    /// AR Cloud Mode - A persistent mode in which anchors are persisted in the cloud environment and
    /// will be available in multiple sessions to devices that are localized to the same space in which
    /// they were published.
    /// 
    /// </summary>
    public partial class MLAnchors
    {
        /// <summary>
        /// See ml_spatial_anchor.h for additional comments.
        /// </summary>
        public class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// A structure representing a user-defined Spatial Anchor.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLSpatialAnchor
            {
                /// <summary>
                /// Version of the structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// The anchor's unique ID. This is a unique identifier for a single Spatial Anchor that is generated and managed by the
                /// Spatial Anchor system. The ID is created when MLSpatialAnchorCreate is called.
                /// </summary>
                public readonly MLUUIDBytes Id;

                /// <summary>
                /// The coordinate frame identifier of the Spatial Anchor.  This should be passed to MLSnapshotGetTransform to get the
                /// anchor's transform.  The anchor's transform is set when the anchor is created but may be updated later by the Spatial Anchor
                /// system based on factors such as quality of the mapped space and subsequent localizations.
                /// </summary>
                public readonly NativeBindings.MLCoordinateFrameUID Cfuid;

                /// <summary>
                /// The suggested expiration time for this anchor represented in seconds since the Unix epoch. This is implemented as an
                /// expiration timestamp in the future after which the associated anchor should be considered no longer valid and may be
                /// removed by the Spatial Anchor system  based on factors such as quality of the mapped space and subsequent localizations.
                /// </summary>
                public readonly ulong ExpirationTimeStamp;

                /// <summary>
                /// Indicates whether or not the anchor has been persisted via a call to #MLSpatialAnchorPublish.
                /// </summary>
                public readonly bool IsPersisted;

                /// <summary>
                /// The ID of the space that this anchor belongs to. This is only relevant if IsPersisted is true.
                /// </summary>
                public readonly MLUUIDBytes SpaceId;

                /// <summary>
                /// The quality of the local space that this anchor occupies. This value may change over time.
                /// </summary>
                public readonly Quality Quality;

                public MLSpatialAnchor(MLAnchors.Anchor anchor)
                {
                    this.Version = 2;
                    this.Id = anchor.id;
                    this.Cfuid = anchor.cfuid;
                    this.ExpirationTimeStamp = anchor.ExpirationTimeStamp;
                    this.IsPersisted = anchor.IsPersisted;
                    this.SpaceId = anchor.spaceId;
                    this.Quality = anchor.quality;
                }

                public MLSpatialAnchor(MLAnchors.Anchor anchor, ulong expirationTimeStamp)
                {
                    this.Version = 2;
                    this.Id = anchor.id;
                    this.Cfuid = anchor.cfuid;
                    this.ExpirationTimeStamp = expirationTimeStamp;
                    this.IsPersisted = anchor.IsPersisted;
                    this.SpaceId = anchor.spaceId;
                    this.Quality = anchor.quality;
                }
            };

            /// <summary>
            /// A structure used to populate anchor creation info when creating a new Spatial Anchor.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLSpatialAnchorCreateInfo
            {
                /// <summary>
                /// Version of the structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// The desired transform of the new Spatial Anchor.  The anchor's transform is set when the anchor is created but may be
                /// updated later by the Spatial Anchor system based on factors such as quality of the mapped space and subsequent localizations.
                /// </summary>
                public readonly NativeBindings.MLTransform Transform;

                /// <summary>
                /// The suggested expiration time for this anchor represented in seconds since the Unix epoch.  This is implemented as an
                /// expiration timestamp in the future after which the associated anchor should be considered no longer valid and may be
                /// removed by the Spatial Anchor system.
                /// If the timestamp is set to zero in this struct, it will default to one year from when the anchor is created.
                /// </summary>
                public readonly ulong ExpirationTimestamp;

                public MLSpatialAnchorCreateInfo(Pose Pose, ulong ExpirationTimestamp)
                {
                    this.Version = 1;
                    this.Transform = new NativeBindings.MLTransform();
                    this.Transform.Position = MLConvert.FromUnity(Pose.position);
                    this.Transform.Rotation = MLConvert.FromUnity(Pose.rotation);
                    this.ExpirationTimestamp = ExpirationTimestamp;
                }
            };

            /// <summary>
            /// A collection of filters for Spatial Anchor queries.  Filters that have been set will be combined via logical
            /// conjunction.  E.  g.  results must match the ids filter AND fall within the radius constraint when both have been set.  This struct
            /// must be initialized by calling #MLSpatialAnchorQueryFilterInit before use.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLSpatialAnchorQueryFilter
            {
                /// <summary>
                /// Version of the structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// The center point of where a spatial query will originate.
                /// </summary>
                public readonly MLVec3f Center;

                /// <summary>
                /// The radius in meters used for a spatial query, relative to the specified center.  Only anchors inside this radius will
                /// be returned. Set to 0 for unbounded results.
                /// </summary>
                public readonly float RadiusM;

                /// <summary>
                /// A list of Spatial Anchor IDs to query for.
                /// </summary>
                public readonly IntPtr Ids;

                /// <summary>
                /// The number of IDs provided.
                /// </summary>
                public readonly uint IdsCount;

                /// <summary>
                /// The upper bound of expected results. Set to 0 for unbounded results.
                /// </summary>
                public readonly uint MaxResults;

                /// <summary>
                /// Indicate whether the results will be returned sorted by distance from center.  Sorting results by distance will incur a
                /// performance penalty.
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public readonly bool Sorted;

                // Used by the anchor subsystem.
                internal MLSpatialAnchorQueryFilter(Vector3 Center, float RadiusM, IntPtr Ids, uint IdsCount, uint MaxResults, bool Sorted)
                {
                    this.Version = 1;
                    this.Center = MLConvert.FromUnity(Center);
                    this.RadiusM = RadiusM;
                    this.Ids = Ids;
                    this.IdsCount = IdsCount;
                    this.MaxResults = MaxResults;
                    this.Sorted = Sorted;
                }

                internal MLSpatialAnchorQueryFilter(Request.Params requestParams)
                {
                    this.Version = 1;
                    this.Center = MLConvert.FromUnity(requestParams.Center);
                    this.RadiusM = requestParams.Radius;

                    this.Ids = IntPtr.Zero;
                    this.IdsCount = 0;

                    if (requestParams.Anchors != null)
                        MarshalArrayToPtr(requestParams.Anchors, out this.Ids, out this.IdsCount);

                    else if (requestParams.AnchorIds != null)
                        MarshalArrayToPtr(requestParams.AnchorIds, out this.Ids, out this.IdsCount);

                    this.MaxResults = requestParams.MaxResults;
                    this.Sorted = requestParams.Sorted;
                }

                internal static void MarshalArrayToPtr<T>(T[] array, out IntPtr ids, out uint idsCount)
                {
                    IntPtr arrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MLUUIDBytes>() * array.Length);
                    IntPtr walkPtr = arrayPtr;
                    foreach (var element in array)
                    {
                        if (element is Anchor)
                            Marshal.StructureToPtr((element as Anchor?)?.id, walkPtr, false);

                        else if (element is string)
                            Marshal.StructureToPtr(new MLUUIDBytes(element as string), walkPtr, false);

                        walkPtr = new IntPtr(walkPtr.ToInt64()) + Marshal.SizeOf<MLUUIDBytes>();
                    }

                    ids = arrayPtr;
                    idsCount = (uint)array.Length;
                }
            }

            /// <summary>
            /// A structure containing information about the device's localization state.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLSpatialAnchorLocalizationInfo
            {
                /// <summary>
                /// Version of the structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// The localization status at the time this structure was returned.
                /// </summary>
                public readonly LocalizationStatus LocalizationStatus;

                /// <summary>
                /// The current mapping mode.
                /// </summary>
                public readonly MappingMode MappingMode;

                /// <summary>
                /// If localized, this will contain the name of the current space.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValTStr, SizeConst = (int)MaxSpaceNameLength)]
                public readonly string SpaceName;

                /// <summary>
                /// If localized, this will contain the unique ID of the current space.
                /// </summary>
                public readonly NativeBindings.MLUUIDBytes SpaceId;

                /// <summary>
                /// If localized, this will contain the identifier of the transform of
                ///	the target space's origin relative to the world origin.
                /// </summary>
                public readonly MagicLeapNativeBindings.MLCoordinateFrameUID TargetSpaceOrigin;

                public static MLSpatialAnchorLocalizationInfo Create() => new MLSpatialAnchorLocalizationInfo(2);

                public MLSpatialAnchorLocalizationInfo(uint version)
                {
                    this.Version = version;
                    this.LocalizationStatus = default;
                    this.MappingMode = default;
                    this.SpaceName = default;
                    this.SpaceId = default;
                    this.TargetSpaceOrigin = default;
                }
            };

            /// <summary>
            /// Create a new local Spatial Anchor at the desired location.  On success, out_anchor will be returned with the desired
            /// transform and a newly generated ID.
            /// Any unpublished anchor will be lost if the Headpose session is lost. See #MLHeadTrackingGetMapEvents for more details.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorCreate(ulong handle, in MLSpatialAnchorCreateInfo createInfo, out MLSpatialAnchor anchor);

            /// <summary>
            /// Create a Spatial Anchor tracker.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorTrackerCreate(out ulong handle);

            /// <summary>
            /// Destroy a previously created Spatial Anchor tracker.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorTrackerDestroy(ulong handle);

            /// <summary>
            /// Publish an existing local Spatial Anchor to the persistent backend.  Depending on the currently selected mapping mode,
            /// this can store the anchor locally or in the cloud. This call will fail if the device is not localized to a space.
            ///  This call will fail if the device is not localized to a space.
            /// Any unpublished anchor will be lost if the Headpose session is lost.See #MLHeadTrackingGetMapEvents for more details
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorPublish(ulong handle, MLUUIDBytes anchorId);

            /// <summary>
            /// Delete an existing Spatial Anchor.  If successful, this will delete the anchor from persistent storage based on the
            /// currently selected mapping mode.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorDelete(ulong handle, MLUUIDBytes anchorId);

            /// <summary>
            /// Update a Spatial Anchor's properties.  The only property that can currently be updated is the expirationTimeStamp.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorUpdate(ulong handle, in MLSpatialAnchor anchor);

            /// <summary>
            /// Create a new query for Spatial Anchors in the current space. It is the responsibility of the caller to call
            /// #MLSpatialAnchorQueryDestroy with the query handle returned from this function after the results are no longer needed.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorQueryCreate(ulong handle, in MLSpatialAnchorQueryFilter queryFilter, out ulong queryHandle, out uint resultsCount);

            /// <summary>
            /// Destroy a previously created query handle and release its associated resources.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorQueryDestroy(ulong handle, ulong queryHandle);

            /// <summary>
            /// Get the result of a previous Spatial Anchor query.  Putting index bounds on the results allows the caller to only
            /// receive a subset of the total number of results generated by the query.  This is useful as a form of pagination in the case of
            /// a large number of anchors in the current space. Indexing is zero-based so if there are N results in the query, then it
            /// is required that 0 <= first_index <= last_index < N.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorQueryGetResult(ulong handle, ulong queryHandle, uint firstIndex, uint lastIndex, [In, Out] MLSpatialAnchor[] results);

            /// <summary>
            /// Get the current localization status of the device.
            /// </summary>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLSpatialAnchorGetLocalizationInfo(ulong handle, ref MLSpatialAnchorLocalizationInfo localizationInfo);
        }
    }
}
