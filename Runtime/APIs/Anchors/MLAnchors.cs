// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disabling deprecated warning for the internal project
#pragma warning disable 618

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
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
    public partial class MLAnchors : MLAutoAPISingleton<MLAnchors>
    {
        /// <summary>
        /// LocalizationStatus
        /// </summary>
        public enum LocalizationStatus
        {
            /// <summary>
            ///  The device is not currently localized.
            /// </summary>
            NotLocalized,

            /// <summary>
            ///  The device has localized successfully.
            /// </summary>
            Localized,

            /// <summary>
            ///  A localization attempt is currently in progress.
            /// </summary>
            LocalizationPending,

            /// <summary>
            ///  The last localization attempt failed.
            /// </summary>
            LocalizationFailed,

        };

        /// <summary>
        /// The current mapping mode, set via settings.
        /// </summary>
        public enum MappingMode
        {
            /// <summary>
            ///	Using on-device mapping .
            /// </summary>
            OnDevice,

            /// <summary>
            ///  Using cloud-based mapping.
            /// </summary>
            ARCloud,
        };

        /// <summary>
        /// The quality of the local space around the anchor. 
        /// This can change over time as the user continues to scan the space.
        /// </summary>
        public enum Quality
        {
            /// <summary>
            /// Low quality, this anchor can be expected to move significantly.
            /// </summary>
            Low,

            /// <summary>
            /// Medium quality, this anchor may move slightly.
            /// </summary>
            Medium,

            /// <summary>
            /// High quality, this anchor is stable and suitable for digital content attachment.
            /// </summary>
            High,
        }

        /// <summary>
        ///  Maximum size for the name of the space in the #MLSpatialAnchorLocalizationInfo structure.
        /// </summary>
        public const uint MaxSpaceNameLength = 64;

        protected override MLResult.Code StartAPI()
        {
            if (!MLResult.DidNativeCallSucceed(MLPermissions.CheckPermission(MLPermission.SpatialAnchors).Result, nameof(StartAPI)))
            {
                MLPluginLog.Error($"{nameof(MLAnchors)} requires missing permission {MLPermission.SpatialAnchors}");
                return MLResult.Code.PermissionDenied;
            }
            return MLAnchors.NativeBindings.MLSpatialAnchorTrackerCreate(out this.Handle);
        }

        protected override MLResult.Code StopAPI() => MLAnchors.NativeBindings.MLSpatialAnchorTrackerDestroy(this.Handle);

        public static MLResult GetLocalizationInfo(out LocalizationInfo info) => MLResult.Create(Instance.GetLocalizationInformation(out info));

        public partial class Request : MLRequest<Request.Params, Request.Result>
        {
            private uint resultsCount;

            public Request()
            {
                handle = MagicLeapNativeBindings.InvalidHandle;
            }

            public override MLResult Start(Params queryParams)
            {
                Dispose(true);
                parameters = queryParams;
                var q = MLAnchors.Instance.CreateQuery(parameters, out handle, out resultsCount);
                var mlResult = MLResult.Create(q);
                return mlResult;
            }

            public MLResult Start(Params parameters, out uint resultsCount)
            {
                var mlResult = this.Start(parameters);
                resultsCount = this.resultsCount;
                return mlResult;
            }

            public override MLResult TryGetResult(out Result result) => this.TryGetResult(0, (int)this.resultsCount - 1, out result);

            public MLResult TryGetResult(int firstIndex, int lastIndex, out Result result)
            {
                if (this.resultsCount == 0)
                {
                    result = new Result(new NativeBindings.MLSpatialAnchor[0]);
                    return MLResult.Create(MLResult.Code.Ok);
                }

                int size = lastIndex - firstIndex + 1;
                var anchors = new NativeBindings.MLSpatialAnchor[size];
                var resultCode = MLAnchors.Instance.GetQueryResult(this.parameters, this.handle, (uint)firstIndex, (uint)lastIndex, anchors);
                MLResult mlResult = MLResult.Create(resultCode);
                result = new Result(anchors);
                return mlResult;
            }

            protected override void Dispose(bool disposing)
            {
                if (MagicLeapNativeBindings.MLHandleIsValid(this.handle))
                {
                    MLResult.Code resultCode = MLAnchors.Instance.DestroyQuery(this.handle);
                    if (MLResult.IsOK(resultCode))
                        this.handle = MagicLeapNativeBindings.InvalidHandle;
                    else
                        Debug.LogError("Failed to destroy anchor query: " + resultCode);
                }
            }
        }

        public partial class Request
        {
            /// <summary>
            /// A collection of filters for Spatial Anchor queries.  Filters that have been set will be combined via logical
            /// conjunction.  E.  g.  results must match the ids filter AND fall within the radius constraint when both have been set.  This struct
            /// must be initialized by calling #MLSpatialAnchorQueryFilterInit before use.
            /// </summary>
            public struct Params
            {
                /// <summary>
                /// The center point of where a spatial query will originate.
                /// </summary>
                public readonly Vector3 Center;

                /// <summary>
                /// The radius in meters used for a spatial query, relative to the specified center.  Only anchors inside this radius will
                /// be returned.  Set to 0 for unbounded results.
                /// </summary>
                public readonly float Radius;

                /// <summary>
                /// A list of Spatial Anchors to query for.
                /// Only the array of anchors or the array of strings will be used for querying.
                /// </summary>
                public readonly Anchor[] Anchors;

                /// <summary>
                /// A list of Spatial Anchors Ids query for.
                /// Only the array of anchors or the array of strings will be used for querying.
                /// </summary>
                public readonly string[] AnchorIds;

                /// <summary>
                /// The upper bound of expected results.  Set to 0 for unbounded results.
                /// </summary>
                public readonly uint MaxResults;

                /// <summary>
                /// Indicate whether the results will be returned sorted by distance from center.  Sorting results by distance will incur a
                /// performance penalty.
                /// </summary>
                public readonly bool Sorted;

                public Params(Vector3 Center, float Radius, uint MaxResults, bool Sorted)
                {
                    this.Center = Center;
                    this.Radius = Radius;
                    this.Anchors = null;
                    this.AnchorIds = null;
                    this.MaxResults = MaxResults;
                    this.Sorted = Sorted;
                }

                public Params(Vector3 Center, float Radius, Anchor[] Anchors, uint MaxResults, bool Sorted)
                {
                    this.Center = Center;
                    this.Radius = Radius;
                    this.Anchors = Anchors;
                    this.AnchorIds = null;
                    this.MaxResults = MaxResults;
                    this.Sorted = Sorted;
                }

                public Params(Vector3 Center, float Radius, string[] AnchorIds, uint MaxResults, bool Sorted)
                {
                    this.Center = Center;
                    this.Radius = Radius;
                    this.Anchors = null;
                    this.AnchorIds = AnchorIds;
                    this.MaxResults = MaxResults;
                    this.Sorted = Sorted;
                }
            }
        }

        public partial class Request
        {
            public readonly struct Result
            {
                internal Result(NativeBindings.MLSpatialAnchor[] nativeAnchors)
                {
                    var anchors = new Anchor[nativeAnchors.Length];

                    for (int i = 0; i < nativeAnchors.Length; ++i)
                        anchors[i] = new Anchor(nativeAnchors[i]);

                    this.anchors = anchors;
                }
                public readonly Anchor[] anchors;
            }
        }

        public readonly struct Anchor
        {
            /// <summary>
            /// Creates a new anchor based on a given pose that can expire after a predefined amount of time. The anchor is only valid when MLResult.Code.Ok is returned.
            /// </summary>
            /// <param name="pose">Pose to base the anchor on.</param>
            /// <param name="expirationSeconds">
            /// Length of time before anchor expires. Can be a range from 0 to DateTime.MaxValue - DateTime.UtcNow. Passing a value of 0 creates an indefinite duration.</param>
            /// <param name="anchor">Anchor being created.</param>
            /// <returns>
            /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successful.
            /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if a parameter is invalid.
            /// MLResult.Result will be <c>MLResult.Code.AnchorsInsufficientMapping</c> if the space has not been sufficiently mapped to allow this operation.
            /// MLResult.Result will be <c>MLResult.Code.AnchorsInvalidExpirationTimestamp</c> if the provided expiration suggestion was not valid.
            /// MLResult.Result will be <c>MLResult.Code.AnchorsMaxAnchorLimitReached</c> if the maximum number of anchors for the current space has been reached.
            /// MLResult.Result will be <c>MLResult.Code.AnchorsMinDistanceThresholdExceeded</c> if the minimum distance between anchors was not met.
            /// </returns>
            public static MLResult Create(Pose pose, long expirationSeconds, out Anchor anchor)
            {
                anchor = new Anchor();

                if (expirationSeconds < 0)
                    return MLResult.Create(MLResult.Code.InvalidParam,
                        "The expirationSeconds parameter was a negative number and should be positive or 0.");

                double unixTimestamp = 0;
                if (expirationSeconds > 0)
                {
                    var maxExpirationSeconds = (long)DateTime.MaxValue.Subtract(DateTime.UtcNow).TotalSeconds;
                    var clampedExpirationSeconds = Math.Clamp(expirationSeconds, 0, maxExpirationSeconds);

                    unixTimestamp = (DateTime.UtcNow.AddSeconds(clampedExpirationSeconds)).Subtract(DateTime.UnixEpoch).TotalSeconds;
                }

                var createInfo = new NativeBindings.MLSpatialAnchorCreateInfo(pose, (ulong)unixTimestamp);
                var resultCode = MLAnchors.Instance.CreateAnchor(createInfo, out NativeBindings.MLSpatialAnchor nativeAnchor);
                anchor = new Anchor(nativeAnchor);
                return MLResult.Create(resultCode);
            }

            public static MLResult DeleteAnchorWithId(string anchorId)
            {
                var resultCode = MLAnchors.Instance.DeleteAnchor(anchorId);
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// The anchor's unique ID. This is a unique identifier for a single Spatial Anchor that is generated and managed by the
            /// Spatial Anchor system. The ID is created when MLSpatialAnchorCreate is called.
            /// </summary>
            public string Id => this.id.ToString();

            /// <summary>
            /// The ID of the space that this anchor belongs to. This is only relevant if IsPersisted is true.
            /// </summary>
            public string SpaceId => this.spaceId.ToString();

            /// <summary>
            /// The quality of the local space that this anchor occupies. This value may change over time.
            /// </summary>
            public string Quality => this.quality.ToString();

            /// <summary>
            /// Pose.
            /// </summary>
            public readonly Pose Pose;

            /// <summary>
            /// The suggested expiration time for this anchor represented in seconds since the Unix epoch.  This is implemented as an
            /// expiration timestamp in the future after which the associated anchor should be considered no longer valid and may be
            /// removed by the Spatial Anchor system.
            /// </summary>
            public readonly ulong ExpirationTimeStamp;

            /// <summary>
            /// Indicates whether or not the anchor has been persisted via a call to #MLSpatialAnchorPublish.
            /// </summary>
            public readonly bool IsPersisted;

            /// <summary>
            /// The anchor's unique ID. This is a unique identifier for a single Spatial Anchor that is generated and managed by the
            /// Spatial Anchor system. The ID is created when MLSpatialAnchorCreateSpatialAnchor is called.
            /// </summary>
            internal readonly NativeBindings.MLUUIDBytes id;

            /// <summary>
            /// The ID of the space that this anchor belongs to. This is only relevant if IsPersisted is true.
            /// </summary>
            internal readonly NativeBindings.MLUUIDBytes spaceId;

            /// <summary>
            /// The cfuid of the anchor.
            /// </summary>
            internal readonly MagicLeapNativeBindings.MLCoordinateFrameUID cfuid;

            /// <summary>
            /// The quality of the local space that this anchor occupies. This value may change over time.
            /// </summary>
            internal readonly Quality quality;

            public MLResult Publish()
            {
                var resultCode = MLAnchors.Instance.PublishAnchor(this);
                return MLResult.Create(resultCode);
            }

            public MLResult Update(long newExpirationTimeStamp)
            {
                if (newExpirationTimeStamp < 0)
                    return MLResult.Create(MLResult.Code.InvalidParam,
                        "The expirationSeconds parameter was a negative number and should be positive or 0.");

                double unixTimestamp = 0;
                if (newExpirationTimeStamp > 0)
                {
                    var maxExpirationSeconds = (long)DateTime.MaxValue.Subtract(DateTime.UtcNow).TotalSeconds;
                    var clampedExpirationSeconds = Math.Clamp(newExpirationTimeStamp, 0, maxExpirationSeconds);

                    unixTimestamp = (DateTime.UtcNow.AddSeconds(clampedExpirationSeconds)).Subtract(DateTime.UnixEpoch).TotalSeconds;
                }

                var resultCode = MLAnchors.Instance.UpdateAnchor(this, (ulong)unixTimestamp);
                return MLResult.Create(resultCode);
            }

            public MLResult Delete()
            {
                var resultCode = MLAnchors.Instance.DeleteAnchor(this);
                return MLResult.Create(resultCode);
            }

            internal Anchor(NativeBindings.MLSpatialAnchor nativeAnchor)
            {
                this.id = nativeAnchor.Id;
                this.spaceId = nativeAnchor.SpaceId;
                this.cfuid = nativeAnchor.Cfuid;
                MagicLeapXrProviderNativeBindings.GetUnityPose(nativeAnchor.Cfuid, out this.Pose);
                this.ExpirationTimeStamp = nativeAnchor.ExpirationTimeStamp;
                this.IsPersisted = nativeAnchor.IsPersisted;
                this.quality = nativeAnchor.Quality;
            }

            public static bool operator ==(Anchor one, Anchor two)
            {
                return one.id == two.id;
            }

            public static bool operator !=(Anchor one, Anchor two)
            {
                return !(one == two);
            }

            public override bool Equals(object obj)
            {
                if (obj is Anchor)
                {
                    var rhs = (Anchor)obj;
                    return this == rhs;
                }

                return false;
            }

            public override int GetHashCode()
            {
                return this.Id.GetHashCode();
            }

            public override string ToString() => $"id: {Id},\nPose: {Pose}, \nQuality: {Quality}, \nExpirationTimeStamp: {ExpirationTimeStamp},\nIsPersisted: {IsPersisted},\nSpaceId: {SpaceId}";
        }

        /// <summary>
        /// A structure containing information about the device's localization state.
        /// </summary>
        public readonly struct LocalizationInfo
        {
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
            public readonly string SpaceName;

            /// <summary>
            /// If localized, this will contain the name of the current space.
            /// </summary>
            public readonly string SpaceId => this.spaceId.ToString();

            /// <summary>
            /// If localized, this will contain the pose info of
            ///	the target space's origin relative to the world origin.
            /// </summary>
            public readonly Pose SpaceOrigin;

            /// <summary>
            /// If localized, this will contain the unique ID of the current space.
            /// </summary>
            internal readonly NativeBindings.MLUUIDBytes spaceId;

            /// <summary>
            /// If localized, this will contain the identifier of the transform of
            ///	the target space's origin relative to the world origin.
            /// </summary>
            internal readonly MagicLeapNativeBindings.MLCoordinateFrameUID spaceOrigin;

            public LocalizationInfo(NativeBindings.MLSpatialAnchorLocalizationInfo nativeInfo)
            {
                this.LocalizationStatus = nativeInfo.LocalizationStatus;
                this.MappingMode = nativeInfo.MappingMode;
                this.SpaceName = nativeInfo.SpaceName;
                this.spaceId = nativeInfo.SpaceId;
                this.spaceOrigin = nativeInfo.TargetSpaceOrigin;

                IntPtr snapshot = IntPtr.Zero;
                MagicLeapNativeBindings.MLTransform transform = new MagicLeapNativeBindings.MLTransform();
                MagicLeapNativeBindings.MLCoordinateFrameUID cfuid = nativeInfo.TargetSpaceOrigin;
                MagicLeapNativeBindings.MLPerceptionGetSnapshot(ref snapshot);
                MagicLeapNativeBindings.MLSnapshotGetTransform(snapshot, ref cfuid, ref transform);

                this.SpaceOrigin.position = transform.Position.ToVector3();
                this.SpaceOrigin.rotation = MLConvert.ToUnity(transform.Rotation);

                MagicLeapNativeBindings.MLPerceptionReleaseSnapshot(snapshot);
            }

            public override string ToString() => $"LocalizationStatus: {this.LocalizationStatus},\nMappingMode: {this.MappingMode},\nSpaceName: {this.SpaceName},\nSpaceId: {this.SpaceId}, \nSpaceOriginId: {this.spaceOrigin}, \nSpaceOrigin: {this.SpaceOrigin}";

        }
    }

}

