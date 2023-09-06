// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    /// <summary>
    ///     The Magic Leap implementation of the <c>XRPlaneSubsystem</c>. Do not create this directly.
    ///     Use <c>PlanesSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    public sealed partial class MLXrPlaneSubsystem : XRPlaneSubsystem
    {
        private static PlanesQuery query;
        public static PlanesQuery Query
        {
            get => query;
            set
            {
                QuerySet = true;
                query = value;
            }
        }

        private static bool QuerySet { get; set; }

        public struct PlanesQuery
        {
            /// <summary>
            ///     The flags to apply to this query.
            /// </summary>
            public MLPlanesQueryFlags Flags;

            /// <summary>
            ///     The center of the bounding box which defines where planes extraction should occur.
            /// </summary>
            public Vector3 BoundsCenter;

            /// <summary>
            ///     The rotation of the bounding box where planes extraction will occur.
            /// </summary>
            public Quaternion BoundsRotation;

            /// <summary>
            ///     The size of the bounding box where planes extraction will occur.
            /// </summary>
            public Vector3 BoundsExtents;

            /// <summary>
            ///     The maximum number of results that should be returned.
            /// </summary>
            public uint MaxResults;

            /// <summary>
            ///     The minimum area (in squared meters) of planes to be returned. This value
            ///     cannot be lower than 0.04 (lower values will be capped to this minimum).
            /// </summary>
            public float MinPlaneArea;
        }

        private const ulong PlaneTrackableIdSalt = 0xf52b75076e45ad88;

        private class MagicLeapProvider : Provider
        {
            private readonly HashSet<TrackableId> _currentSet = new();
            private readonly Dictionary<TrackableId, BoundedPlane> _planes = new();
            private readonly Dictionary<TrackableId, PlaneBoundary> _boundariesTable = new();
            private MLPlanesQueryFlags _currentPlaneDetectionMode;
            private MLPlanesQueryFlags _defaultQueryFlags = MLPlanesQueryFlags.AllOrientations | MLPlanesQueryFlags.SemanticAll;
            private uint _lastNumResults;
            private uint _maxResults = 10;
            private ulong _planesTracker = MagicLeapNativeBindings.InvalidHandle;
            private uint _previousLastNumResults;
            private MLPlanesQueryFlags _requestedPlaneDetectionMode;
            private ScanState _currentScanState = ScanState.Stopped;

            private PlanesQuery DefaultPlanesQuery
            {
                get
                {
                    if (QuerySet)
                    {
                        return Query;
                    }

                    return new PlanesQuery
                    {
                        Flags = _defaultQueryFlags,
                        BoundsCenter = Vector3.zero,
                        BoundsRotation = Quaternion.identity,
                        BoundsExtents = Vector3.one * 20f,
                        MaxResults = _maxResults,
                        MinPlaneArea = 10,
                    };
                }
            }

            public override PlaneDetectionMode requestedPlaneDetectionMode
            {
                get => _requestedPlaneDetectionMode.ToPlaneDetectionMode();
                set
                {
                    _requestedPlaneDetectionMode = value.ToMLXrQueryFlags();
                    _defaultQueryFlags = _requestedPlaneDetectionMode | MLPlanesQueryFlags.SemanticAll;
                }
            }

            public override PlaneDetectionMode currentPlaneDetectionMode => _currentPlaneDetectionMode.ToPlaneDetectionMode();
            
            private bool CreateClient()
            {
                if (_planesTracker != MagicLeapNativeBindings.InvalidHandle)
                {
                    return true;
                }

                if (!MLPermissions.CheckPermission(MLPermission.SpatialMapping).IsOk)
                {
                    return false;
                }

                var result = NativeBindings.MLOpenXRCreatePlaneTracker(out _planesTracker);
                if (!MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLOpenXRCreatePlaneTracker)))
                {
                    return false;
                }
                
                _boundariesTable.Clear();
                return true;
            }

            public override void Start()
            {
                SubsystemFeatures.SetFeatureRequested(Feature.PlaneTracking, true);
            }

            public override void Stop()
            {
                if (_planesTracker != MagicLeapNativeBindings.InvalidHandle)
                {
                    NativeBindings.MLOpenXRDestroyPlaneTracker(_planesTracker);
                    _planesTracker = MagicLeapNativeBindings.InvalidHandle;
                    _boundariesTable.Clear();
                }
                
                SubsystemFeatures.SetFeatureRequested(Feature.PlaneTracking, false);
            }

            public override void Destroy()
            {
            }

            private PlaneBoundary GetBoundaryOfPlane(in TrackableId trackableId)
            {
                if (!_planes.TryGetValue(trackableId, out _))
                {
                    return default;
                }

                return !_boundariesTable.TryGetValue(trackableId, out var planeBoundary) ? default : planeBoundary;
            }

            public override void GetBoundary(TrackableId trackableId, Allocator allocator, ref NativeArray<Vector2> convexHullOut)
            {
                var boundary = GetBoundaryOfPlane(in trackableId);
                if (boundary?.PolygonVertexCount > 0)
                {
                    using var polygon = boundary.GetPolygon(Allocator.TempJob);
                    ConvexHullGenerator.GiftWrap(polygon, allocator, ref convexHullOut);
                    return;
                }

                if (_planes.TryGetValue(trackableId, out var plane))
                {
                    var halfHeight = plane.height * 0.5f;
                    var halfWidth = plane.width * 0.5f;

                    var calculatedBoundaries = new NativeArray<Vector2>(4, Allocator.Temp);
                    calculatedBoundaries[0] = new Vector2(halfWidth, halfHeight);
                    calculatedBoundaries[1] = new Vector2(-halfWidth, halfHeight);
                    calculatedBoundaries[2] = new Vector2(-halfWidth, -halfHeight);
                    calculatedBoundaries[3] = new Vector2(halfWidth, -halfHeight);

                    ConvexHullGenerator.GiftWrap(calculatedBoundaries, allocator, ref convexHullOut);
                    return;
                }

                CreateOrResizeNativeArrayIfNecessary(0, allocator, ref convexHullOut);
            }
            
            public override unsafe TrackableChanges<BoundedPlane> GetChanges(BoundedPlane defaultPlane, Allocator allocator)
            {
                if (_planesTracker == MagicLeapNativeBindings.InvalidHandle)
                {
                    if (!CreateClient())
                    {
                        return default;
                    }
                }
                
                if (_currentScanState == ScanState.Stopped)
                {
                    BeginNewQuery();
                }

                //If query has already begun, track the changes
                if (_currentScanState != ScanState.Scanning)
                {
                    return default;
                }
                
                //Check the state to make sure we are done scanning
                var stateResult = NativeBindings.MLGetPlaneDetectionState(_planesTracker, out var scanState);
                if (!MLResult.DidNativeCallSucceed(stateResult, nameof(NativeBindings.MLGetPlaneDetectionState)))
                {
                    return default;
                }
                switch (scanState)
                {
                    case XrTypes.MLXrPlaneDetectionState.Done:
                        break;
                    case XrTypes.MLXrPlaneDetectionState.Pending:
                    case XrTypes.MLXrPlaneDetectionState.None:
                        return default;
                    case XrTypes.MLXrPlaneDetectionState.Error:
                    case XrTypes.MLXrPlaneDetectionState.Fatal:
                    default:
                        _currentScanState = ScanState.Stopped;
                        throw new InvalidOperationException($"An error occured when querying state of the plane detection: {scanState}");
                }
                

                //Successfully scanned, so get the results
                //Get the plane results first
                var location = new XrTypes.MLXrPlaneDetectorLocations();
                var result = NativeBindings.MLGetPlaneDetections(_planesTracker, out location);
                if (!MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLGetPlaneDetections)))
                {
                    _currentScanState = ScanState.Stopped;
                    return default;
                }
                var planeTrackableIds = new NativeArray<TrackableId>((int)location.planeLocationCountOutput, Allocator.TempJob);
                if (location.planeLocationCountOutput > 0)
                {
                    //Now we have the count so we can assign the locations
                    location.planeLocations = (XrTypes.MLXrPlaneDetectorLocation*)new NativeArray<XrTypes.MLXrPlaneDetectorLocation>((int)location.planeLocationCountOutput, Allocator.TempJob).GetUnsafePtr();
                    location.planeLocationCapacityInput = location.planeLocationCountOutput;
                    
                    result = NativeBindings.MLGetPlaneDetections(_planesTracker, out location);

                    if (!MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLGetPlaneDetections)))
                    {
                        _currentScanState = ScanState.Stopped;
                        return default;
                    }
                    
                    //Now we have all the locations. So we can start creating the boundary
                    _boundariesTable.Clear();
                    var planeCountTable = new Dictionary<ulong, ulong>();
                    for (var i = 0; i < location.planeLocationCountOutput; i++)
                    {
                        var planeLocation = location.planeLocations[i];
                        var planeBoundary = new PlaneBoundary();
                        //Get polygon
                        var polygonBuffer = new XrTypes.MLXrPlaneDetectorPolygonBuffer();
                        result = NativeBindings.MLGetPlanePolygonBuffer(_planesTracker, planeLocation.planeId, 0, out polygonBuffer);
                        if (!MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLGetPlanePolygonBuffer)))
                        {
                            _currentScanState = ScanState.Stopped;
                            return default;
                        }
                        
                        polygonBuffer.vertices = (Vector2*)new NativeArray<Vector2>((int)polygonBuffer.vertexCountOutput, Allocator.TempJob).GetUnsafePtr();
                        polygonBuffer.vertexCapacityInput = polygonBuffer.vertexCountOutput;

                        result = NativeBindings.MLGetPlanePolygonBuffer(_planesTracker, planeLocation.planeId, 0, out polygonBuffer);

                        if (!MLResult.DidNativeCallSucceed(result, $"{nameof(NativeBindings.MLGetPlanePolygonBuffer)} Querying Length"))
                        {
                            _currentScanState = ScanState.Stopped;
                            return default;
                        }
                        planeBoundary.Polygon = polygonBuffer;
                        if (planeLocation.polygonBufferCount > 0)
                        {
                            var holeBuffers = new List<XrTypes.MLXrPlaneDetectorPolygonBuffer>();
                            for (uint holeIndex = 1; holeIndex < planeLocation.polygonBufferCount; holeIndex++)
                            {
                                var holeBuffer = new XrTypes.MLXrPlaneDetectorPolygonBuffer();
                                result = NativeBindings.MLGetPlanePolygonBuffer(_planesTracker, planeLocation.planeId, holeIndex, out holeBuffer);
                                if (!MLResult.DidNativeCallSucceed(result, $"{nameof(NativeBindings.MLGetPlanePolygonBuffer)} Querying Holes Length"))
                                {
                                    return default;
                                }
                                holeBuffer.vertices = (Vector2*)new NativeArray<Vector2>((int)holeBuffer.vertexCountOutput, Allocator.TempJob).GetUnsafePtr();
                                holeBuffer.vertexCapacityInput = holeBuffer.vertexCountOutput;
                                result = NativeBindings.MLGetPlanePolygonBuffer(_planesTracker, planeLocation.planeId, holeIndex, out holeBuffer);
                                if (!MLResult.DidNativeCallSucceed(result, $"{nameof(NativeBindings.MLGetPlanePolygonBuffer)} Getting Holes"))
                                {
                                    return default;
                                }
                                holeBuffers.Add(holeBuffer);
                            }

                            planeBoundary.Holes = new NativeArray<XrTypes.MLXrPlaneDetectorPolygonBuffer>(holeBuffers.ToArray(), Allocator.TempJob);
                        }
                        
                        if (!planeCountTable.TryGetValue(planeLocation.planeId, out var count))
                        {
                            count = 0;
                        }
                        var trackableId = new TrackableId(planeLocation.planeId, PlaneTrackableIdSalt + count);
                        planeCountTable[planeLocation.planeId] = ++count;
                        _boundariesTable[trackableId] = planeBoundary;
                        planeTrackableIds[i] = trackableId;
                    }
                }


                _previousLastNumResults = _lastNumResults;
                _lastNumResults = location.planeLocationCountOutput;
                _currentScanState = ScanState.Stopped;
                using var uPlanes = new NativeArray<BoundedPlane>((int)location.planeLocationCountOutput, Allocator.TempJob);
                // Perform Unity plane conversion
                new CopyPlaneResultsJob { PlaneTrackableIds = planeTrackableIds, PlanesIn = location.planeLocations, PlanesOut = uPlanes }.Schedule((int)location.planeLocationCountOutput, 1).Complete();
                planeTrackableIds.Dispose();

                // Update plane states
                var added = new NativeFixedList<BoundedPlane>((int)location.planeLocationCountOutput, Allocator.Temp);
                var updated = new NativeFixedList<BoundedPlane>((int)location.planeLocationCountOutput, Allocator.Temp);
                var removed = new NativeFixedList<TrackableId>((int)_previousLastNumResults, Allocator.Temp);

                _currentSet.Clear();
                for (var i = 0; i < location.planeLocationCountOutput; ++i)
                {
                    var uPlane = uPlanes[i];
                    var trackableId = uPlane.trackableId;
                    _currentSet.Add(trackableId);

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
                    if (!_currentSet.Contains(trackableId))
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
                    var changes = new TrackableChanges<BoundedPlane>(added.Length, updated.Length, removed.Length, allocator);
                    added.CopyTo(changes.added);
                    updated.CopyTo(changes.updated);
                    removed.CopyTo(changes.removed);
                    return changes;
                }
            }

            private unsafe void BeginNewQuery()
            {
                _maxResults = QuerySet switch
                {
                    // We hit the max, so increase for next time
                    false when _maxResults == _lastNumResults => _maxResults * 3 / 2,
                    true => DefaultPlanesQuery.MaxResults,
                    _ => _maxResults
                };
                
                DefaultPlanesQuery.Flags.ToMLXrOrientationsAndSemanticTypes(out var orientationValues, out var semanticValues);

                var orientationsArray = new NativeArray<XrTypes.MLXrPlaneDetectorOrientation>(orientationValues.Count, Allocator.TempJob);
                orientationsArray.CopyFrom(orientationValues.ToArray());

                var semanticArray = new NativeArray<XrTypes.MLXrPlaneDetectorSemanticType>(semanticValues.Count, Allocator.TempJob);
                semanticArray.CopyFrom(semanticValues.ToArray());
                
                var beginInfo = new XrTypes.MLXrPlaneDetectorBeginInfo
                {
                    orientationCount = (uint)orientationsArray.Length,
                    orientations = (XrTypes.MLXrPlaneDetectorOrientation*)orientationsArray.GetUnsafePtr(),
                    semanticTypeCount = (uint)semanticArray.Length,
                    semanticTypes = (XrTypes.MLXrPlaneDetectorSemanticType*)semanticArray.GetUnsafePtr(),
                    maxPlanes = _maxResults,
                    minArea = DefaultPlanesQuery.MinPlaneArea,
                    boundingBoxPose = new XrTypes.MLXrPose { position = DefaultPlanesQuery.BoundsCenter, rotation = DefaultPlanesQuery.BoundsRotation },
                    boundingBoxExtents = DefaultPlanesQuery.BoundsExtents
                };

                var result = NativeBindings.MLBeginPlaneDetection(_planesTracker, in beginInfo);

                if (!MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLBeginPlaneDetection)))
                {
                    return;
                }
                
                _currentScanState = ScanState.Scanning;
                _currentPlaneDetectionMode = _requestedPlaneDetectionMode;
            }
            
            private enum ScanState
            {
                Scanning,
                Stopped
            }
        }

#if UNITY_OPENXR_1_7_0_OR_NEWER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void RegisterDescriptor()
        {
            Debug.Log("Planes: Registering Planes Subsystem");
            XRPlaneSubsystemDescriptor.Create(new XRPlaneSubsystemDescriptor.Cinfo
            {
                id = MagicLeapXrProvider.PlanesSubsystemId,
                providerType = typeof(MagicLeapProvider),
                subsystemTypeOverride = typeof(MLXrPlaneSubsystem),
                supportsVerticalPlaneDetection = true,
                supportsArbitraryPlaneDetection = true,
                supportsBoundaryVertices = true,
                supportsClassification = true
            });
        }
#endif

    }
}
