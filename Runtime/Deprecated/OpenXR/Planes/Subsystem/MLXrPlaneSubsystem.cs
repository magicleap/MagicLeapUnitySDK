// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
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
using UnityEngine.XR.OpenXR.NativeTypes;
#pragma warning disable CS0618 // Type or member is obsolete
using static UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapPlanesNativeTypes;
#pragma warning restore CS0618 // Type or member is obsolete
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapPlanesTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    /// <summary>
    ///     The Magic Leap implementation of the <c>XRPlaneSubsystem</c>. Do not create this directly.
    ///     Use <c>PlanesSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve, Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Subsystems")]
    public sealed partial class MLXrPlaneSubsystem : XRPlaneSubsystem
    {
        private static PlanesQuery QueryInternal;
        public static PlanesQuery Query
        {
            get => QueryInternal;
            set
            {
                QuerySet = true;
                QueryInternal = value;
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
        
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapProvider")]
        
        private class MagicLeapProvider : Provider
        {
            private readonly HashSet<TrackableId> currentSet = new();
            private readonly Dictionary<TrackableId, BoundedPlane> planes = new();
            private readonly Dictionary<TrackableId, PlaneBoundary> boundariesTable = new();
            private MLPlanesQueryFlags currentPlaneDetectionModeInternal;
            private MLPlanesQueryFlags defaultQueryFlags = MLPlanesQueryFlags.AllOrientations | MLPlanesQueryFlags.SemanticAll;
            private uint lastNumResults;
            private uint maxResults = 10;
            private ulong planesTracker = Utils.XrConstants.NullHandle;
            private uint previousLastNumResults;
            private MLPlanesQueryFlags requestedPlaneDetectionModeInternal;
            private ScanState currentScanState = ScanState.Stopped;

            private MagicLeapPlanesNativeFunctions nativeFunctions;
            private MagicLeapPlanesFeature planesFeature;

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
                unsafe
                {
                    if (planesTracker != Utils.XrConstants.NullHandle)
                    {
                        return true;
                    }

                    if (!Permissions.CheckPermission(MLPermission.SpatialMapping))
                    {
                        return false;
                    }

                    var createInfo = new XrPlaneDetectorCreateInfo
                    {
                        Type = XrPlaneStructTypes.PlaneDetectorCreateInfo,
                        Flags = XrPlaneDetectorFlags.XrPlaneDetectorEnableContourBit
                    };

                    var result = nativeFunctions.XrCreatePlaneDetector(planesFeature.AppSession, in createInfo, out planesTracker);
                    if (!Utils.DidXrCallSucceed(result, nameof(nativeFunctions.XrCreatePlaneDetector)))
                    {
                        return false;
                    }
                    boundariesTable.Clear();
                    return true;
                }
            }

            public override void Start()
            {
                SubsystemFeatures.SetFeatureRequested(Feature.PlaneTracking, true);
                planesFeature = OpenXRSettings.Instance.GetFeature<MagicLeapPlanesFeature>();
                nativeFunctions = planesFeature.PlanesNativeFunctions;
            }

            public override void Stop()
            {
                currentScanState = ScanState.Stopped;
                if (planesTracker != Utils.XrConstants.NullHandle)
                {
                    unsafe
                    {
                        nativeFunctions.XrDestroyPlaneDetector(planesTracker);
                        planesTracker = Utils.XrConstants.NullHandle;
                        boundariesTable.Clear();
                    }
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

                return boundariesTable.GetValueOrDefault(trackableId);
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
                if(planesFeature.SessionState != XrSessionState.Focused)
                {
                    return default;
                }
                
                if (planesTracker == Utils.XrConstants.NullHandle)
                {
                    CreateClient();
                    return default;
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
                var stateResult = nativeFunctions.XrGetPlaneDetectionState(planesTracker, out var scanState);
                if (!Utils.DidXrCallSucceed(stateResult, nameof(nativeFunctions.XrGetPlaneDetectionState)))
                {
                    return default;
                }
                switch (scanState)
                {
                    case XrPlaneDetectionState.Done:
                        break;
                    case XrPlaneDetectionState.Pending:
                    case XrPlaneDetectionState.None:
                        return default;
                    case XrPlaneDetectionState.Error:
                    case XrPlaneDetectionState.Fatal:
                    default:
                        currentScanState = ScanState.Stopped;
                        var message = $"[MLXrPlaneSubsystem] Plane detection state: {scanState.ToString().ToUpper()}";
                        if (!Application.isEditor)
                        {
                            throw new ApplicationException(message);
                        }

                        Debug.LogWarning(message);
                        return default;

                }
                

                //Successfully scanned, so get the results
                //Get the plane results first
                var location = new XrPlaneDetectorLocations()
                {
                    Type = XrPlaneStructTypes.PlaneDetectorLocations
                };

                var getInfo = new XrPlaneDetectorGetInfo
                {
                    Type = XrPlaneStructTypes.PlaneDetectorGetInfo,
                    Space = planesFeature.AppSpace,
                    Time = planesFeature.NextPredictedDisplayTime
                };

                var result = nativeFunctions.XrGetPlaneDetections(planesTracker, in getInfo, out location);
                if (!Utils.DidXrCallSucceed(result, nameof(nativeFunctions.XrGetPlaneDetections)))
                {
                    currentScanState = ScanState.Stopped;
                    return default;
                }
                var planeTrackableIds = new NativeArray<TrackableId>((int)location.PlaneLocationCountOutput, Allocator.TempJob);
                if (location.PlaneLocationCountOutput > 0)
                {
                    //Now we have the count so we can assign the locations
                    var locationArray = new NativeArray<XrPlaneDetectorLocation>((int)location.PlaneLocationCountOutput, Allocator.Temp);
                    location.PlaneLocations = (XrPlaneDetectorLocation*)locationArray.GetUnsafePtr();
                    NativeCopyUtility.FillArrayWithValue(locationArray, new XrPlaneDetectorLocation
                    {
                        Type = XrPlaneStructTypes.PlaneDetectorLocation,
                    });
                    location.PlaneLocationCapacityInput = location.PlaneLocationCountOutput;

                    result = nativeFunctions.XrGetPlaneDetections(planesTracker, in getInfo, out location);

                    if (!Utils.DidXrCallSucceed(result, nameof(nativeFunctions.XrGetPlaneDetections)))
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
                        var polygonBuffer = new XrPlaneDetectorPolygonBuffer()
                        {
                            Type = XrPlaneStructTypes.PlaneDetectorPolygonBuffer,
                            VertexCapacityInput =  0,
                            VertexCountOutput = 0,
                            Vertices = null
                        };
                        
                        result = nativeFunctions.XrGetPlanePolygonBuffer(planesTracker, planeLocation.PlaneId, 0, out polygonBuffer);
                        if (!Utils.DidXrCallSucceed(result, nameof(nativeFunctions.XrGetPlanePolygonBuffer)))
                        {
                            currentScanState = ScanState.Stopped;
                            return default;
                        }
                        
                        polygonBuffer.Vertices = (Vector2*)new NativeArray<Vector2>((int)polygonBuffer.VertexCountOutput, Allocator.Temp).GetUnsafePtr();
                        polygonBuffer.VertexCapacityInput = polygonBuffer.VertexCountOutput;

                        result = nativeFunctions.XrGetPlanePolygonBuffer(planesTracker, planeLocation.PlaneId, 0, out polygonBuffer);

                        if (!Utils.DidXrCallSucceed(result, $"{nameof(nativeFunctions.XrGetPlanePolygonBuffer)} Querying Length"))
                        {
                            currentScanState = ScanState.Stopped;
                            return default;
                        }
                        planeBoundary.Polygon = polygonBuffer;
                        if (planeLocation.PolygonBufferCount > 0)
                        {
                            var holeBuffers = new List<XrPlaneDetectorPolygonBuffer>();
                            for (uint holeIndex = 1; holeIndex < planeLocation.PolygonBufferCount; holeIndex++)
                            {
                                var holeBuffer = new XrPlaneDetectorPolygonBuffer()
                                {
                                    Type = XrPlaneStructTypes.PlaneDetectorPolygonBuffer
                                };
                                
                                result = nativeFunctions.XrGetPlanePolygonBuffer(planesTracker, planeLocation.PlaneId, holeIndex, out holeBuffer);
                                if (!Utils.DidXrCallSucceed(result, $"{nameof(nativeFunctions.XrGetPlanePolygonBuffer)} Querying Holes Length"))
                                {
                                    return default;
                                }
                                holeBuffer.Vertices = (Vector2*)new NativeArray<Vector2>((int)holeBuffer.VertexCountOutput, Allocator.Temp).GetUnsafePtr();
                                holeBuffer.VertexCapacityInput = holeBuffer.VertexCountOutput;
                                result = nativeFunctions.XrGetPlanePolygonBuffer(planesTracker, planeLocation.PlaneId, holeIndex, out holeBuffer);
                                if (!Utils.DidXrCallSucceed(result, $"{nameof(nativeFunctions.XrGetPlanePolygonBuffer)} Getting Holes"))
                                {
                                    return default;
                                }
                                holeBuffers.Add(holeBuffer);
                            }

                            planeBoundary.Holes = new NativeArray<XrPlaneDetectorPolygonBuffer>(holeBuffers.ToArray(), Allocator.Temp);
                        }
                        
                        if (!planeCountTable.TryGetValue(planeLocation.PlaneId, out var count))
                        {
                            count = 0;
                        }
                        var trackableId = new TrackableId(planeLocation.PlaneId, PlaneTrackableIdSalt + count);
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
                maxResults = QuerySet switch
                {
                    // We hit the max, so increase for next time
                    false when maxResults == lastNumResults => maxResults * 3 / 2,
                    true => DefaultPlanesQuery.MaxResults,
                    _ => maxResults
                };

                DefaultPlanesQuery.Flags.ToMLXrOrientationsAndSemanticTypes(out var orientationValues, out var semanticValues);

                var orientationsArray = new NativeArray<XrPlaneDetectorOrientation>(orientationValues.Count, Allocator.Temp);
                orientationsArray.CopyFrom(orientationValues.ToArray());

                var semanticArray = new NativeArray<XrPlaneDetectorSemanticTypes>(semanticValues.Count, Allocator.Temp);
                semanticArray.CopyFrom(semanticValues.ToArray());
                
                var beginInfo = new XrPlaneDetectorBeginInfo
                {
                    Type = XrPlaneStructTypes.PlaneDetectorBeginInfo,
                    Space = planesFeature.AppSpace,
                    Time = planesFeature.NextPredictedDisplayTime,
                    OrientationCount = (uint)orientationsArray.Length,
                    Orientations = (XrPlaneDetectorOrientation*)orientationsArray.GetUnsafePtr(),
                    SemanticTypesCount = (uint)semanticArray.Length,
                    SemanticTypes = (XrPlaneDetectorSemanticTypes*)semanticArray.GetUnsafePtr(),
                    MaxPlanes = maxResults,
                    MinArea = DefaultPlanesQuery.MinPlaneArea,
                    BoundingBoxPose = new XrPose { Position = DefaultPlanesQuery.BoundsCenter.ConvertBetweenUnityOpenXr(), Rotation = DefaultPlanesQuery.BoundsRotation.ConvertBetweenUnityOpenXr() },
                    BoundingBoxExtents = DefaultPlanesQuery.BoundsExtents,
                };

                var result = nativeFunctions.XrBeginPlaneDetection(planesTracker, in beginInfo);

                if (!Utils.DidXrCallSucceed(result, nameof(nativeFunctions.XrBeginPlaneDetection)))
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

        [Obsolete]
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
#if UNITY_EDITOR
                supportsBoundaryVertices = false,
#else
                supportsBoundaryVertices = true,
#endif
                supportsClassification = true
            });
        }

    }
}
