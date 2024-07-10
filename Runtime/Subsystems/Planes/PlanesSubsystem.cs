// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disabling deprecated warning for the internal project
#pragma warning disable 618

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// The Magic Leap implementation of the <c>XRPlaneSubsystem</c>. Do not create this directly.
    /// Use <c>PlanesSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    public sealed partial class PlanesSubsystem : XRPlaneSubsystem
    {
        public static partial class Extensions
        {
            public static Extensions.PlanesQuery Query
            {
                get => query;
                set
                {
                    QuerySet = true;
                    query = value;
                }
            }

            internal static bool QuerySet
            {
                get;
                private set;
            }

            private static Extensions.PlanesQuery query = default;

            public struct PlanesQuery
            {
                /// <summary>
                /// The flags to apply to this query.
                /// </summary>
                public PlanesSubsystem.Extensions.MLPlanesQueryFlags Flags;

                /// <summary>
                /// The center of the bounding box which defines where planes extraction should occur.
                /// </summary>
                public Vector3 BoundsCenter;

                /// <summary>
                /// The rotation of the bounding box where planes extraction will occur.
                /// </summary>
                public Quaternion BoundsRotation;

                /// <summary>
                /// The size of the bounding box where planes extraction will occur.
                /// </summary>
                public Vector3 BoundsExtents;

                /// <summary>
                /// The maximum number of results that should be returned.
                /// </summary>
                public uint MaxResults;

                /// <summary>
                /// The minimum area (in squared meters) of planes to be returned. This value
                /// cannot be lower than 0.04 (lower values will be capped to this minimum).
                /// </summary>
                public float MinPlaneArea;
            }
        }

#if UNITY_2020_2_OR_NEWER
        MagicLeapProvider magicLeapProvider => (MagicLeapProvider)provider;
#else
        MagicLeapProvider magicLeapProvider;

        protected override Provider CreateProvider()
        {
            magicLeapProvider = new MagicLeapProvider();
            return magicLeapProvider;
        }
#endif

        internal const ulong planeTrackableIdSalt = 0xf52b75076e45ad88;

        class MagicLeapProvider : Provider
        {
            internal Extensions.PlanesQuery defaultPlanesQuery
            {
                get
                {
                    if (Extensions.QuerySet)
                    {
                        return Extensions.Query;
                    }
                    else
                    {
                        return new Extensions.PlanesQuery
                        {
                            Flags = _defaultQueryFlags,
                            BoundsCenter = Vector3.zero,
                            BoundsRotation = Quaternion.identity,
                            BoundsExtents = Vector3.one * 20f,
                            MaxResults = _maxResults,
                            MinPlaneArea = 0.25f
                        };
                    }
                }
            }

            ulong _planesTracker = Native.MagicLeapNativeBindings.InvalidHandle;
            ulong _QueryHandle = Native.MagicLeapNativeBindings.InvalidHandle;

            uint _maxResults = 4;
            uint _lastNumResults;
            uint _previousLastNumResults = 0;

            Extensions.MLPlanesQueryFlags _defaultQueryFlags = Extensions.MLPlanesQueryFlags.Polygons | Extensions.MLPlanesQueryFlags.Semantic_All;

            Dictionary<TrackableId, BoundedPlane> _planes = new Dictionary<TrackableId, BoundedPlane>();

            Extensions.MLPlaneBoundariesList _boundariesList;

            Extensions.MLPlanesQueryFlags _requestedPlaneDetectionMode;
            Extensions.MLPlanesQueryFlags _currentPlaneDetectionMode;

            // todo: 2019-05-22: Unity.Collections.NativeHashMap would be better
            // but introduces another package dependency. Probably not worth it
            // for just this one thing, but if it becomes a dependency, we should
            // switch to using the NativeHashMap (or NativeHashSet if it exists).
            static HashSet<TrackableId> currentSet = new HashSet<TrackableId>();

            public MagicLeapProvider() { }

            public override PlaneDetectionMode requestedPlaneDetectionMode
            {
                get => _requestedPlaneDetectionMode.ToPlaneDetectionMode();
                set
                {
                    _requestedPlaneDetectionMode = value.ToMLQueryFlags();
                    _defaultQueryFlags = _requestedPlaneDetectionMode | Extensions.MLPlanesQueryFlags.Polygons | Extensions.MLPlanesQueryFlags.Semantic_All;
                }
            }

            public override PlaneDetectionMode currentPlaneDetectionMode => _currentPlaneDetectionMode.ToPlaneDetectionMode();

            private bool CreateClient()
            {
                if (_planesTracker != Native.MagicLeapNativeBindings.InvalidHandle)
                {
                    // client already created
                    return true;
                }

                if (!MLPermissions.CheckPermission(MLPermission.SpatialMapping).IsOk)
                {
                    // permission denied
                    return false;
                }

                var result = NativeBindings.MLPlanesCreate(out _planesTracker);
                if (!MLResult.IsOK(result))
                {
                    Debug.LogError($"Failed to start planes subsystem, reason: {result}");
                    return false;
                }

                if (_boundariesList.valid)
                {
                    Debug.LogError($"Restarting the plane subsystem with an existing boundaries list.");
                }
                _boundariesList = Extensions.MLPlaneBoundariesList.Create();

                return true;
            }

            public override void Start()
            {
                // Don't create client right away as permission request will not be approved yet

                SubsystemFeatures.SetFeatureRequested(Feature.PlaneTracking, true);
            }

            public override void Stop()
            {
                if (_planesTracker != Native.MagicLeapNativeBindings.InvalidHandle)
                {
                    if (_boundariesList.valid)
                    {
                        NativeBindings.MLPlanesReleaseBoundariesList(_planesTracker, ref _boundariesList);
                        _boundariesList = Extensions.MLPlaneBoundariesList.Create();
                    }

                    NativeBindings.MLPlanesDestroy(_planesTracker);
                    _planesTracker = Native.MagicLeapNativeBindings.InvalidHandle;
                }

                SubsystemFeatures.SetFeatureRequested(Feature.PlaneTracking, false);
                _QueryHandle = Native.MagicLeapNativeBindings.InvalidHandle;
            }

            public override void Destroy() { }

            public unsafe PlaneBoundaryCollection GetAllBoundariesForPlane(in TrackableId trackableId)
            {
                if (!_planes.TryGetValue(trackableId, out BoundedPlane plane))
                    return default;

                // MLPlaneBoundaries is an array of boundaries, so planeBoundariesArray represents an array of MLPlaneBoundaries
                // which is itself an array of boundaries.
                var planeBoundariesArray = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Extensions.MLPlaneBoundaries>(
                    _boundariesList.plane_boundaries,
                    (int)_boundariesList.plane_boundaries_count,
                    Allocator.None);

#if UNITY_EDITOR
                var safetyHandle = AtomicSafetyHandle.Create();
                NativeArrayUnsafeUtility.SetAtomicSafetyHandle(ref planeBoundariesArray, safetyHandle);
#endif

                // Find the plane boundaries with the given trackable id
                foreach (var planeBoundaries in planeBoundariesArray)
                {
                    var id = new TrackableId(planeBoundaries.id, planeTrackableIdSalt);
                    if (id == trackableId)
                    {
                        return new PlaneBoundaryCollection(planeBoundaries, plane.pose);
                    }
                }

                return default;
            }

            public unsafe override void GetBoundary(
                TrackableId trackableId,
                Allocator allocator,
                ref NativeArray<Vector2> convexHullOut)
            {
                var boundaries = GetAllBoundariesForPlane(in trackableId);
                if (boundaries.count > 0)
                {
                    // TODO 2019-05-21: handle multiple boundaries?
                    using (var polygon = boundaries[0].GetPolygon(Allocator.TempJob))
                    {
                        ConvexHullGenerator.Giftwrap(polygon, allocator, ref convexHullOut);
                        return;
                    }
                }
                else
                {
                    if (_planes.TryGetValue(trackableId, out BoundedPlane plane))
                    {
                        float halfHeight = plane.height * 0.5f;
                        float halfWidth = plane.width * 0.5f;

                        var calculatedBoundaries = new NativeArray<Vector2>(4, Allocator.Temp);
                        calculatedBoundaries[0] = new Vector2(halfWidth, halfHeight);
                        calculatedBoundaries[1] = new Vector2(-halfWidth, halfHeight);
                        calculatedBoundaries[2] = new Vector2(-halfWidth, -halfHeight);
                        calculatedBoundaries[3] = new Vector2(halfWidth, -halfHeight);

                        ConvexHullGenerator.Giftwrap(calculatedBoundaries, allocator, ref convexHullOut);
                        return;
                    }
                }

                CreateOrResizeNativeArrayIfNecessary<Vector2>(0, allocator, ref convexHullOut);
            }


            public unsafe override TrackableChanges<BoundedPlane> GetChanges(BoundedPlane defaultPlane, Allocator allocator)
            {
                if (_planesTracker == Native.MagicLeapNativeBindings.InvalidHandle)
                {
                    if (!CreateClient())
                    {
                        return default;
                    }
                }

                if (_QueryHandle == Native.MagicLeapNativeBindings.InvalidHandle)
                {
                    _QueryHandle = BeginNewQuery();
                    return default;
                }
                else
                {
                    // Get results
                    var mlPlanes = new NativeArray<Extensions.MLPlane>((int)_maxResults, Allocator.TempJob);
                    if (_boundariesList.valid)
                    {
                        NativeBindings.MLPlanesReleaseBoundariesList(_planesTracker, ref _boundariesList);
                    }
                    _boundariesList = Extensions.MLPlaneBoundariesList.Create();

                    try
                    {
                        var result = NativeBindings.MLPlanesQueryGetResultsWithBoundaries(
                            _planesTracker, _QueryHandle,
                            (Extensions.MLPlane*)mlPlanes.GetUnsafePtr(), out uint numResults, ref _boundariesList);

                        switch (result)
                        {
                            case MLResult.Code.Ok:
                                {
                                    _previousLastNumResults = _lastNumResults;
                                    _lastNumResults = numResults;
                                    _QueryHandle = BeginNewQuery();

                                    using (var uPlanes = new NativeArray<BoundedPlane>((int)numResults, Allocator.TempJob))
                                    {
                                        // Generate unique tracker IDs based on number of planes with the same plane id (inner planes)
                                        var planeIdCount = new Dictionary<ulong, ulong>(); ;
                                        var planeTrackableIds = new NativeArray<TrackableId>(mlPlanes.Length, Allocator.TempJob);

                                        for (int i = 0; i < mlPlanes.Length; ++i)
                                        {
                                            var planeId = mlPlanes[i].id;
                                            if (!planeIdCount.TryGetValue(planeId, out ulong count))
                                            {
                                                count = 0;
                                            }
                                            var trackableId = new TrackableId(planeId, planeTrackableIdSalt + count);
                                            planeIdCount[planeId] = ++count;
                                            planeTrackableIds[i] = trackableId;
                                        }

                                        // Perform Unity plane conversion
                                        new CopyPlaneResultsJob
                                        {
                                            planeTrackableIds = planeTrackableIds,
                                            planesIn = mlPlanes,
                                            planesOut = uPlanes
                                        }.Schedule((int)numResults, 1).Complete();

                                        planeTrackableIds.Dispose();

                                        // Update plane states
                                        var added = new NativeFixedList<BoundedPlane>((int)numResults, Allocator.Temp);
                                        var updated = new NativeFixedList<BoundedPlane>((int)numResults, Allocator.Temp);
                                        var removed = new NativeFixedList<TrackableId>((int)_previousLastNumResults, Allocator.Temp);

                                        currentSet.Clear();
                                        for (int i = 0; i < numResults; ++i)
                                        {
                                            var uPlane = uPlanes[i];
                                            var trackableId = uPlane.trackableId;
                                            currentSet.Add(trackableId);

                                            if (_planes.ContainsKey(trackableId))
                                            {
                                                updated.Add(uPlane);
                                            }
                                            else
                                            {
                                                added.Add(uPlane);
                                            }

                                            _planes[trackableId] = uPlane;
                                        }

                                        // Look for removed planes
                                        foreach (var kvp in _planes)
                                        {
                                            var trackableId = kvp.Key;
                                            if (!currentSet.Contains(trackableId))
                                            {
                                                removed.Add(trackableId);
                                            }
                                        }

                                        foreach (var trackableId in removed)
                                        {
                                            _planes.Remove(trackableId);
                                        }

                                        using (added)
                                        using (updated)
                                        using (removed)
                                        {
                                            var changes = new TrackableChanges<BoundedPlane>(
                                                added.Length,
                                                updated.Length,
                                                removed.Length,
                                                allocator);

                                            added.CopyTo(changes.added);
                                            updated.CopyTo(changes.updated);
                                            removed.CopyTo(changes.removed);

                                            return changes;
                                        }
                                    }
                                }
                            case MLResult.Code.Pending:
                                {
                                    return default;
                                }
                            default:
                                {
                                    _QueryHandle = BeginNewQuery();
                                    return default;
                                }
                        }
                    }
                    finally
                    {
                        mlPlanes.Dispose();
                    }
                }
            }

            ulong BeginNewQuery()
            {
                // We hit the max, so increase for next time
                if (!Extensions.QuerySet && _maxResults == _lastNumResults)
                {
                    _maxResults = _maxResults * 3 / 2;
                }

                var query = new Extensions.MLPlanesQuery(defaultPlanesQuery);
                if (Extensions.QuerySet)
                {
                    _maxResults = query.max_results;
                }

                ulong queryHandle;
                var result = NativeBindings.MLPlanesQueryBegin(_planesTracker, in query, out queryHandle);

                if (!MLResult.IsOK(result))
                {
                    return Native.MagicLeapNativeBindings.InvalidHandle;
                }

                _currentPlaneDetectionMode = _requestedPlaneDetectionMode;

                return queryHandle;
            }
        }


#if UNITY_XR_MAGICLEAP_PROVIDER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        static void RegisterDescriptor()
        {
            XRPlaneSubsystemDescriptor.Create(new XRPlaneSubsystemDescriptor.Cinfo
            {
                id = MagicLeapXrProvider.PlanesSubsystemId,
                providerType = typeof(MagicLeapProvider),
                subsystemTypeOverride = typeof(PlanesSubsystem),
                supportsVerticalPlaneDetection = true,
                supportsArbitraryPlaneDetection = true,
                supportsBoundaryVertices = true,
                supportsClassification = true
            });
        }

        internal class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLPlanesCreate(out ulong planes_tracker);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLPlanesDestroy(ulong planes_tracker);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLPlanesQueryBegin(ulong planes_tracker, in Extensions.MLPlanesQuery query, out ulong request_handle);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern unsafe MLResult.Code MLPlanesQueryGetResultsWithBoundaries(ulong planes_tracker, ulong planes_query, Extensions.MLPlane* out_results, out uint out_num_results, ref Extensions.MLPlaneBoundariesList out_boundaries);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLPlanesReleaseBoundariesList(ulong planes_tracker, ref Extensions.MLPlaneBoundariesList plane_boundaries);
        }
    }
}
