// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
#if UNITY_OPENXR_1_9_0_OR_NEWER

using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport;

namespace OpenXR.PointCloud
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Subsystems")]
    public class MagicLeapOpenXRPointCloudSubsystem : XRPointCloudSubsystem
    {
#if !UNITY_XR_MAGICLEAP_PROVIDER
        public static void RegisterDescriptor()
        {
            Debug.Log("Registering Point Cloud Subsystem");
            XRPointCloudSubsystemDescriptor.RegisterDescriptor(new XRPointCloudSubsystemDescriptor.Cinfo
            {
                id = MagicLeapXrProvider.PointCloudSubsystemId,
                providerType = typeof(MagicLeapProvider),
                subsystemTypeOverride = typeof(MagicLeapOpenXRPointCloudSubsystem),
                supportsFeaturePoints = true,
                supportsConfidence = true,
                supportsUniqueIds = false
            });
        }
#endif
        private struct XRPointCloudCreationJob : IJobParallelFor
        {
            [ReadOnly] public NativeArray<TrackableId> InputIds;
            [ReadOnly] public XRPointCloud DefaultPointCloud;
            [WriteOnly] public NativeList<XRPointCloud>.ParallelWriter Output;

            public void Execute(int index)
            {
                var idToProcess = InputIds[index];
                var cloud = new XRPointCloud(idToProcess, DefaultPointCloud.pose, TrackingState.Tracking, IntPtr.Zero);
                Output.AddNoResize(cloud);
            }
        }

        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapProvider")]

        private class MagicLeapProvider : Provider
        {
            private MagicLeapMeshingFeature.FrameMeshInfo currentFrameInfo;

            private IntPtr previousMeshIds = IntPtr.Zero;
            private int previousMeshCount;

            public override void Start()
            {
                base.Start();
                MagicLeapOpenXRPointCloudNativeBindings.MLOpenXRBeginPointCloudDetection();
            }

            public override void Stop()
            {
                base.Stop();
                MagicLeapOpenXRPointCloudNativeBindings.MLOpenXRStopPointCloudDetection();
                MagicLeapOpenXRPointCloudNativeBindings.MLOpenXRGetCurrentMeshes(ref previousMeshIds, out previousMeshCount);
            }


            public override TrackableChanges<XRPointCloud> GetChanges(XRPointCloud defaultPointCloud, Allocator allocator)
            {
                unsafe
                {
                    //If previous Ids exists, then we have to add that as removed
                    if (previousMeshIds != IntPtr.Zero && previousMeshCount > 0)
                    {
                        var previousIds = (TrackableId*)previousMeshIds;
                        var previousRemovedIds = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TrackableId>(previousIds, previousMeshCount, Allocator.None);
                        var previousResult =  new TrackableChanges<XRPointCloud>(null, 0, null, 0, previousRemovedIds.GetUnsafePtr(), previousMeshCount, defaultPointCloud, sizeof(XRPointCloud), allocator);
                        previousMeshIds = IntPtr.Zero;
                        return previousResult;
                    }
                    //Fetch frame info
                    MagicLeapXrMeshingNativeBindings.MLOpenXRGetCurrentFrameMeshData(out currentFrameInfo);

                    var addedIds = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TrackableId>(currentFrameInfo.addedIds, (int)currentFrameInfo.addedCount, Allocator.None);
                    var updatedIds = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TrackableId>(currentFrameInfo.updatedIds, (int)currentFrameInfo.updatedCount, Allocator.None);
                    var removedIds = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<TrackableId>(currentFrameInfo.removedIds, (int)currentFrameInfo.removedCount, Allocator.None);
                    
                    using var addedPointClouds = new NativeList<XRPointCloud>((int)currentFrameInfo.addedCount, Allocator.TempJob);
                    using var updatedPointClouds = new NativeList<XRPointCloud>((int)currentFrameInfo.updatedCount, Allocator.TempJob);

                    new XRPointCloudCreationJob
                    {
                        InputIds = addedIds,
                        DefaultPointCloud = defaultPointCloud,
                        Output = addedPointClouds.AsParallelWriter()
                    }.Schedule(addedIds.Length, 10).Complete();

                    new XRPointCloudCreationJob
                    {
                        InputIds = updatedIds,
                        DefaultPointCloud = defaultPointCloud,
                        Output = updatedPointClouds.AsParallelWriter()
                    }.Schedule(updatedIds.Length, 10).Complete();
                    
                    var result = new TrackableChanges<XRPointCloud>(addedPointClouds.GetUnsafePtr(), addedPointClouds.Length, updatedPointClouds.GetUnsafePtr(), updatedPointClouds.Length, removedIds.GetUnsafePtr(), removedIds.Length, defaultPointCloud, sizeof(XRPointCloud), allocator);
                    return result;
                }
            }

            public override XRPointCloudData GetPointCloudData(TrackableId trackableId, Allocator allocator)
            {
                unsafe
                {
                    var positions = (Vector3*)IntPtr.Zero;
                    var confidence = (float*)IntPtr.Zero;
                    var normals = (Vector3*)IntPtr.Zero;

                    var result = MagicLeapXrMeshingNativeBindings.MLOpenXRAcquireMeshData(in trackableId, ref positions, out var positionCount, ref normals, out _, ref confidence, out var confidenceCount);
                    if (!result)
                    {
                        return default;
                    }

                    var pointCloudData = new XRPointCloudData();

                    var positionsArray = new NativeArray<Vector3>(NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<Vector3>(positions, positionCount, Allocator.None), Allocator.Temp);
                    var confidenceArray = new NativeArray<float>(NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<float>(confidence, confidenceCount, Allocator.None), Allocator.Temp);

                    pointCloudData.confidenceValues = new NativeArray<float>(confidenceArray.Length, allocator);
                    pointCloudData.positions = new NativeArray<Vector3>(positionsArray.Length, allocator);
                    new XRCopyPointCloudDataJob
                    {
                        VertexInput = positionsArray,
                        ConfidenceInput = confidenceArray,
                        VertexOutput = pointCloudData.positions,
                        ConfidenceOutput = pointCloudData.confidenceValues
                    }.Schedule().Complete();

                    return pointCloudData;
                }
            }

            private struct XRCopyPointCloudDataJob : IJob
            {
                [ReadOnly] public NativeArray<Vector3> VertexInput;
                [ReadOnly] public NativeArray<float> ConfidenceInput;

                [WriteOnly] public NativeArray<Vector3> VertexOutput;
                [WriteOnly] public NativeArray<float> ConfidenceOutput;

                public void Execute()
                {
                    VertexOutput.CopyFrom(VertexInput);
                    ConfidenceOutput.CopyFrom(ConfidenceInput);
                }
            }
        }
    }
}
#endif
