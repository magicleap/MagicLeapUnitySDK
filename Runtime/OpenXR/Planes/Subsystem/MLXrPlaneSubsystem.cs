// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_OPENXR_1_9_0_OR_NEWER

using MagicLeap.Android;
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
                querySet = true;
                query = value;
            }
        }

        private static bool querySet { get; set; }

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

        private const ulong planeTrackableIdSalt = 0xf52b75076e45ad88;

        private class MagicLeapProvider : Provider
        {
            private readonly HashSet<TrackableId> currentSet = new();
            private readonly Dictionary<TrackableId, BoundedPlane> planes = new();
            private readonly Dictionary<TrackableId, PlaneBoundary> boundariesTable = new();
            private MLPlanesQueryFlags currentPlaneDetectionModeInternal;
            private MLPlanesQueryFlags defaultQueryFlags = MLPlanesQueryFlags.AllOrientations | MLPlanesQueryFlags.SemanticAll;
            private uint lastNumResults;
            private uint maxResults = 10;
            private ulong planesTracker = MagicLeapNativeBindings.InvalidHandle;
            private uint previousLastNumResults;
            private MLPlanesQueryFlags requestedPlaneDetectionModeInternal;
            private ScanState currentScanState = ScanState.Stopped;

            private PlanesQuery defaultPlanesQuery
            {
                get
                {
                    if (querySet)
                    {
                        return Query;
                    }

                    return new PlanesQuery
                    {
                        Flags = defaultQueryFlags,
                        BoundsCenter = Vector3.zero,
                        BoundsRotation = Quaternion.identity,
                        BoundsExtents = Vector3.one * 20f,
                        MaxResults = maxResults,
                        MinPlaneArea = 10,
                    };
                }
            }

            public override PlaneDetectionMode requestedPlaneDetectionMode
            {
                get => requestedPlaneDetectionModeInternal.ToPlaneDetectionMode();
                set
                {
                    requestedPlaneDetectionModeInternal = value.ToMLXrQueryFlags();
                    defaultQueryFlags = requestedPlaneDetectionModeInternal | MLPlanesQueryFlags.SemanticAll;
                }
            }

            public override PlaneDetectionMode currentPlaneDetectionMode => currentPlaneDetectionModeInternal.ToPlaneDetectionMode();
            
            private bool CreateClient()
            {
                if (planesTracker != MagicLeapNativeBindings.InvalidHandle)
                {
                    return true;
                }

                if (!Permissions.CheckPermission(MLPermission.SpatialMapping))
                {
                    return false;
                }

                var result = NativeBindings.MLOpenXRCreatePlaneTracker(out planesTracker);
                if (!Utils.DidXrCallSucceed(result, nameof(NativeBindings.MLOpenXRCreatePlaneTracker)))
                {
                    return false;
                }
                
                boundariesTable.Clear();
                return true;
            }

            public override void Start()
            {
                SubsystemFeatures.SetFeatureRequested(Feature.PlaneTracking, true);
            }

            public override void Stop()
            {
                currentScanState = ScanState.Stopped;
                if (planesTracker != MagicLeapNativeBindings.InvalidHandle)
                {
                    NativeBindings.MLOpenXRDestroyPlaneTracker(planesTracker);
                    planesTracker = MagicLeapNativeBindings.InvalidHandle;
                    boundariesTable.Clear();
                }
                
                SubsystemFeatures.SetFeatureRequested(Feature.PlaneTracking, false);
            }

            public override void Destroy()
            {
            }

            private PlaneBoundary GetBoundaryOfPlane(in TrackableId trackableId)
            {
                if (!planes.TryGetValue(trackableId, out _))
                {
                    return default;
                }

                return !boundariesTable.TryGetValue(trackableId, out var planeBoundary) ? default : planeBoundary;
            }

            public override void GetBoundary(TrackableId trackableId, Allocator allocator, ref NativeArray<Vector2> convexHullOut)
            {
                var boundary = GetBoundaryOfPlane(in trackableId);
                if (boundary?.PolygonVertexCount > 0)
                {
                    using var polygon = boundary.GetPolygon(Allocator.Temp);
                    ConvexHullGenerator.GiftWrap(polygon, allocator, ref convexHullOut);
                    return;
                }

                if (planes.TryGetValue(trackableId, out var plane))
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
                if (planesTracker == MagicLeapNativeBindings.InvalidHandle)
                {
                    if (!CreateClient())
                    {
                        return default;
                    }
                }
                
                if (currentScanState == ScanState.Stopped)
                {
                    BeginNewQuery();
                }

                //If query has already begun, track the changes
                if (currentScanState != ScanState.Scanning)
                {
                    return default;
                }
                
                //Check the state to make sure we are done scanning
                var stateResult = NativeBindings.MLGetPlaneDetectionState(planesTracker, out var scanState);
                if (!Utils.DidXrCallSucceed(stateResult, nameof(NativeBindings.MLGetPlaneDetectionState)))
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
                        currentScanState = ScanState.Stopped;
                        string message = $"[MLXrPlaneSubsystem] Plane detection state: {scanState.ToString().ToUpper()}";
                        if (Application.isEditor)
                        {
                            Debug.LogWarning(message);
                            return default;
                        }
                        else
                        {
                            throw new ApplicationException(message);
                        }
                }
                

                //Successfully scanned, so get the results
                //Get the plane results first
                var location = new XrTypes.MLXrPlaneDetectorLocations();
                var result = NativeBindings.MLGetPlaneDetections(planesTracker, out location);
                if (!Utils.DidXrCallSucceed(result, nameof(NativeBindings.MLGetPlaneDetections)))
                {
                    currentScanState = ScanState.Stopped;
                    return default;
                }
                var planeTrackableIds = new NativeArray<TrackableId>((int)location.PlaneLocationCountOutput, Allocator.Temp);
                if (location.PlaneLocationCountOutput > 0)
                {
                    //Now we have the count so we can assign the locations
                    location.PlaneLocations = (XrTypes.MLXrPlaneDetectorLocation*)new NativeArray<XrTypes.MLXrPlaneDetectorLocation>((int)location.PlaneLocationCountOutput, Allocator.Temp).GetUnsafePtr();
                    location.PlaneLocationCapacityInput = location.PlaneLocationCountOutput;
                    
                    result = NativeBindings.MLGetPlaneDetections(planesTracker, out location);

                    if (!Utils.DidXrCallSucceed(result, nameof(NativeBindings.MLGetPlaneDetections)))
                    {
                        currentScanState = ScanState.Stopped;
                        return default;
                    }
                    
                    //Now we have all the locations. So we can start creating the boundary
                    boundariesTable.Clear();
                    var planeCountTable = new Dictionary<ulong, ulong>();
                    for (var i = 0; i < location.PlaneLocationCountOutput; i++)
                    {
                        var planeLocation = location.PlaneLocations[i];
                        var planeBoundary = new PlaneBoundary();
                        //Get polygon
                        var polygonBuffer = new XrTypes.MLXrPlaneDetectorPolygonBuffer();
                        result = NativeBindings.MLGetPlanePolygonBuffer(planesTracker, planeLocation.PlaneId, 0, out polygonBuffer);
                        if (!Utils.DidXrCallSucceed(result, nameof(NativeBindings.MLGetPlanePolygonBuffer)))
                        {
                            currentScanState = ScanState.Stopped;
                            return default;
                        }
                        
                        polygonBuffer.Vertices = (Vector2*)new NativeArray<Vector2>((int)polygonBuffer.VertexCountOutput, Allocator.Temp).GetUnsafePtr();
                        polygonBuffer.VertexCapacityInput = polygonBuffer.VertexCountOutput;

                        result = NativeBindings.MLGetPlanePolygonBuffer(planesTracker, planeLocation.PlaneId, 0, out polygonBuffer);

                        if (!Utils.DidXrCallSucceed(result, $"{nameof(NativeBindings.MLGetPlanePolygonBuffer)} Querying Length"))
                        {
                            currentScanState = ScanState.Stopped;
                            return default;
                        }
                        planeBoundary.Polygon = polygonBuffer;
                        if (planeLocation.PolygonBufferCount > 0)
                        {
                            var holeBuffers = new List<XrTypes.MLXrPlaneDetectorPolygonBuffer>();
                            for (uint holeIndex = 1; holeIndex < planeLocation.PolygonBufferCount; holeIndex++)
                            {
                                var holeBuffer = new XrTypes.MLXrPlaneDetectorPolygonBuffer();
                                result = NativeBindings.MLGetPlanePolygonBuffer(planesTracker, planeLocation.PlaneId, holeIndex, out holeBuffer);
                                if (!Utils.DidXrCallSucceed(result, $"{nameof(NativeBindings.MLGetPlanePolygonBuffer)} Querying Holes Length"))
                                {
                                    return default;
                                }
                                holeBuffer.Vertices = (Vector2*)new NativeArray<Vector2>((int)holeBuffer.VertexCountOutput, Allocator.Temp).GetUnsafePtr();
                                holeBuffer.VertexCapacityInput = holeBuffer.VertexCountOutput;
                                result = NativeBindings.MLGetPlanePolygonBuffer(planesTracker, planeLocation.PlaneId, holeIndex, out holeBuffer);
                                if (!Utils.DidXrCallSucceed(result, $"{nameof(NativeBindings.MLGetPlanePolygonBuffer)} Getting Holes"))
                                {
                                    return default;
                                }
                                holeBuffers.Add(holeBuffer);
                            }

                            planeBoundary.Holes = new NativeArray<XrTypes.MLXrPlaneDetectorPolygonBuffer>(holeBuffers.ToArray(), Allocator.Temp);
                        }
                        
                        if (!planeCountTable.TryGetValue(planeLocation.PlaneId, out var count))
                        {
                            count = 0;
                        }
                        var trackableId = new TrackableId(planeLocation.PlaneId, planeTrackableIdSalt + count);
                        planeCountTable[planeLocation.PlaneId] = ++count;
                        boundariesTable[trackableId] = planeBoundary;
                        planeTrackableIds[i] = trackableId;
                    }
                }


                previousLastNumResults = lastNumResults;
                lastNumResults = location.PlaneLocationCountOutput;
                currentScanState = ScanState.Stopped;
                using var uPlanes = new NativeArray<BoundedPlane>((int)location.PlaneLocationCountOutput, Allocator.TempJob);
                // Perform Unity plane conversion
                new CopyPlaneResultsJob { PlaneTrackableIds = planeTrackableIds, PlanesIn = location.PlaneLocations, PlanesOut = uPlanes }.Schedule((int)location.PlaneLocationCountOutput, 1).Complete();
                planeTrackableIds.Dispose();

                // Update plane states
                var added = new NativeFixedList<BoundedPlane>((int)location.PlaneLocationCountOutput, Allocator.Temp);
                var updated = new NativeFixedList<BoundedPlane>((int)location.PlaneLocationCountOutput, Allocator.Temp);
                var removed = new NativeFixedList<TrackableId>((int)previousLastNumResults, Allocator.Temp);

                currentSet.Clear();
                for (var i = 0; i < location.PlaneLocationCountOutput; ++i)
                {
                    var uPlane = uPlanes[i];
                    var trackableId = uPlane.trackableId;
                    currentSet.Add(trackableId);

                    if (planes.ContainsKey(trackableId))
                    {
                        updated.Add(uPlane);
                    }
                    else
                    {
                        added.Add(uPlane);
                    }

                    planes[trackableId] = uPlane;
                }

                // Look for removed planes
                foreach (var kvp in planes)
                {
                    var trackableId = kvp.Key;
                    if (!currentSet.Contains(trackableId))
                    {
                        removed.Add(trackableId);
                    }
                }

                foreach (var trackableId in removed)
                {
                    planes.Remove(trackableId);
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
                maxResults = querySet switch
                {
                    // We hit the max, so increase for next time
                    false when maxResults == lastNumResults => maxResults * 3 / 2,
                    true => defaultPlanesQuery.MaxResults,
                    _ => maxResults
                };

                defaultPlanesQuery.Flags.ToMLXrOrientationsAndSemanticTypes(out var orientationValues, out var semanticValues);

                var orientationsArray = new NativeArray<XrTypes.MLXrPlaneDetectorOrientation>(orientationValues.Count, Allocator.Temp);
                orientationsArray.CopyFrom(orientationValues.ToArray());

                var semanticArray = new NativeArray<XrTypes.MLXrPlaneDetectorSemanticType>(semanticValues.Count, Allocator.Temp);
                semanticArray.CopyFrom(semanticValues.ToArray());
                
                var beginInfo = new XrTypes.MLXrPlaneDetectorBeginInfo
                {
                    OrientationCount = (uint)orientationsArray.Length,
                    Orientations = (XrTypes.MLXrPlaneDetectorOrientation*)orientationsArray.GetUnsafePtr(),
                    SemanticTypeCount = (uint)semanticArray.Length,
                    SemanticTypes = (XrTypes.MLXrPlaneDetectorSemanticType*)semanticArray.GetUnsafePtr(),
                    MaxPlanes = maxResults,
                    MinArea = defaultPlanesQuery.MinPlaneArea,
                    BoundingBoxPose = new Pose { position = defaultPlanesQuery.BoundsCenter, rotation = defaultPlanesQuery.BoundsRotation },
                    BoundingBoxExtents = defaultPlanesQuery.BoundsExtents
                };

                var result = NativeBindings.MLBeginPlaneDetection(planesTracker, in beginInfo);

                if (!Utils.DidXrCallSucceed(result, nameof(NativeBindings.MLBeginPlaneDetection)))
                {
                    return;
                }
                
                currentScanState = ScanState.Scanning;
                currentPlaneDetectionModeInternal = requestedPlaneDetectionModeInternal;
            }
            
            private enum ScanState
            {
                Scanning,
                Stopped
            }
        }
        
        public static void RegisterDescriptor()
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

    }
}

#endif
