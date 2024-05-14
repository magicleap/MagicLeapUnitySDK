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

using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop;
using UnityEngine.XR.OpenXR.NativeTypes;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    /// <summary>
    ///     The Magic Leap implementation of the <c>XRAnchorSubsystem</c>. Do not create this directly.
    ///     Use <c>XRAnchorSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    public sealed partial class MLXrAnchorSubsystem : XRAnchorSubsystem
    {
        public enum AnchorConfidence
        {
            NotFound = 0,
            Low = 1,
            Medium = 2,
            High = 3
        }

        internal struct AnchorCompletionStatus
        {
            internal Pose Pose;
            internal ulong Id;
            internal XrUUID AnchorStorageId;
            internal byte FromStorage;
            internal XrResult Result;
        }

        public AnchorConfidence GetAnchorConfidence(ARAnchor arAnchor)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorConfidence)} without \"Start\"ing the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;

            return magicLapProvider.GetAnchorConfidence(arAnchor.trackableId);
        }

        public ulong GetAnchorId(ARAnchor arAnchor)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorConfidence)} without \"Start\"ing the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.GetAnchorId(arAnchor.trackableId);
        }

        public Pose GetAnchorPoseFromID(ulong anchorId)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorConfidence)} without \"Start\"ing the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.GetAnchorPoseFromID(anchorId);
        }

        private class MagicLeapProvider : Provider
        {
            private MagicLeapSpatialAnchorsFeature anchorsFeature = null;

            private Camera mainCamera;

            private const ulong AnchorTrackableIdSalt = 0xf52b75076e45ad88;

            private int anchorHashCounter = 0;
            private TrackableId GenerateTrackableId()
            {
                int hash = anchorHashCounter++;
                hash = hash.GetHashCode();
                var trackableId = new TrackableId((ulong)hash, AnchorTrackableIdSalt);
                return trackableId;
            }

            private struct AnchorsSubsystemUpdateType
            { }

            private struct AnchorSubsystemAnchorData
            {
                public ulong AnchorId;
                public XRAnchor AnchorObject;
            }

            private List<AnchorSubsystemAnchorData> currentAnchors = new List<AnchorSubsystemAnchorData>();
            private List<XRAnchor> addedAnchors = new List<XRAnchor>();
            private List<XRAnchor> updatedAnchors = new List<XRAnchor>();
            private List<TrackableId> deletedAnchors = new List<TrackableId>();


            public MagicLeapProvider()
            {
            }

            public override void Start()
            {
                if (OpenXRRuntime.IsExtensionEnabled("XR_ML_spatial_anchors"))
                {
                    mainCamera = Camera.main;

                    anchorsFeature = OpenXRSettings.Instance.GetFeature<MagicLeapSpatialAnchorsFeature>();

                    if (!anchorsFeature)
                    {
                        Debug.LogError("XRAnchorSubsystem failed to retrieve the MagicLeapSpatialAnchorsFeature and will not function properly.");
                    }
                }
                else
                {
                    Debug.LogError($"XR_ML_spatial_anchors is not enabled and is required for XRAnchorSubsystem");
                }
            }

            public override void Stop()
            {
            }

            public override void Destroy()
            {
            }

            public override unsafe TrackableChanges<XRAnchor> GetChanges(XRAnchor defaultAnchor, Allocator allocator)
            {

                int completedLength = 0;

                unsafe
                {
                    if (anchorsFeature)
                    {
                        MagicLeapSpatialAnchorsFeature.NativeBindings.MLOpenXRGetSpatialAnchorCompletionCount(&completedLength);

                        if (completedLength > 0)
                        {
                            using NativeArray<AnchorCompletionStatus> completed = new NativeArray<AnchorCompletionStatus>(completedLength, Allocator.Temp);

                            MagicLeapSpatialAnchorsFeature.NativeBindings.MLOpenXRCheckSpatialAnchorCompletion((AnchorCompletionStatus*)completed.GetUnsafePtr(), &completedLength);

                            if (completedLength > 0)
                            {
                                // NativeSlice contrains the number of values to the actual amount of anchors that have completed creation.
                                NativeSlice<AnchorCompletionStatus> finalComplete = new NativeSlice<AnchorCompletionStatus>(completed, 0, completedLength);

                                foreach (AnchorCompletionStatus status in finalComplete)
                                {
                                    bool found = false;
                                    for (int i = 0; i < currentAnchors.Count; i++)
                                    {
                                        if (currentAnchors[i].AnchorObject.pose == status.Pose)
                                        {
                                            AnchorSubsystemAnchorData UpdatedAnchor = new AnchorSubsystemAnchorData();
                                            UpdatedAnchor.AnchorId = status.Id;
                                            UpdatedAnchor.AnchorObject = new XRAnchor(currentAnchors[i].AnchorObject.trackableId, status.Pose, TrackingState.Tracking, IntPtr.Zero);
                                            currentAnchors[i] = UpdatedAnchor;

                                            updatedAnchors.Add(UpdatedAnchor.AnchorObject);
                                            found = true;
                                            break;
                                        }
                                    }

                                    if (!found)
                                    {
                                        Debug.LogWarning("XRAnchorSubsystem: Anchor created outside of the Anchor Manager at " + status.Pose.ToString());
                                        TrackableId newId = GenerateTrackableId();

                                        AnchorSubsystemAnchorData newAnchor = new AnchorSubsystemAnchorData();
                                        newAnchor.AnchorId = status.Id;
                                        newAnchor.AnchorObject = new XRAnchor(newId, status.Pose, TrackingState.Tracking, IntPtr.Zero);

                                        currentAnchors.Add(newAnchor);

                                        addedAnchors.Add(newAnchor.AnchorObject);
                                    }
                                }
                            }
                        }
                    }
                }


                var changes = new TrackableChanges<XRAnchor>(
                        addedAnchors.Count,
                        updatedAnchors.Count,
                        deletedAnchors.Count,
                        allocator);

                changes.added.CopyFrom(addedAnchors.ToArray());
                changes.updated.CopyFrom(updatedAnchors.ToArray());
                changes.removed.CopyFrom(deletedAnchors.ToArray());

                addedAnchors.Clear();
                updatedAnchors.Clear();
                deletedAnchors.Clear();

                return changes;
            }

            public override unsafe bool TryAddAnchor(Pose pose, out XRAnchor xrAnchor)
            {
                bool result = false;
                xrAnchor = default;

                if (anchorsFeature)
                {
                    var resultCode = MagicLeapSpatialAnchorsFeature.NativeBindings.MLOpenXRCreateSpatialAnchor(pose);
                    result = Utils.DidXrCallSucceed(resultCode, nameof(MagicLeapSpatialAnchorsFeature.NativeBindings.MLOpenXRCreateSpatialAnchor));

                    if (!result)
                    {
                        Debug.LogError("XRAnchorSubsystem: Error creating Anchor at " + pose.ToString());
                    }
                    else
                    {
                        TrackableId newId = GenerateTrackableId();

                        AnchorSubsystemAnchorData newAnchor = new AnchorSubsystemAnchorData();
                        newAnchor.AnchorId = 0u;
                        newAnchor.AnchorObject = new XRAnchor(newId, pose, TrackingState.None, IntPtr.Zero);

                        xrAnchor = newAnchor.AnchorObject;

                        currentAnchors.Add(newAnchor);

                        addedAnchors.Add(newAnchor.AnchorObject);
                    }
                }

                return result;
            }

            public override bool TryRemoveAnchor(TrackableId trackableId)
            {
                bool anchorFound = false;
                int foundIndex = 0; 
                for (int i = 0; i < currentAnchors.Count; i++)
                {
                    if (currentAnchors[i].AnchorObject.trackableId == trackableId)
                    {
                        anchorFound = true;
                        foundIndex = i;
                        break;
                    }
                }

                if (anchorFound)
                {
                    bool deleteResult = false;

                    unsafe
                    {
                        var anchorId = currentAnchors[foundIndex].AnchorId;
                        var resultCode = MagicLeapSpatialAnchorsFeature.NativeBindings.MLOpenXRDeleteLocalSpatialAnchor(&anchorId);
                        deleteResult = Utils.DidXrCallSucceed(resultCode, nameof(MagicLeapSpatialAnchorsFeature.NativeBindings.MLOpenXRCreateSpatialAnchor));
                    }

                    if (!deleteResult)
                    {
                        Debug.LogError("XRAnchorSubsystem: Error Deleting local Anchor " + trackableId.ToString());
                    }
                    else
                    {
                        deletedAnchors.Add(currentAnchors[foundIndex].AnchorObject.trackableId);
                        currentAnchors.RemoveAt(foundIndex);
                    }


                    if (deleteResult)
                    {
                        return true;
                    }
                }

                Debug.LogError("XRAnchorSubsystem: Failed Deleting Anchor from Subsystem " + trackableId.ToString());
                return false;
            }

            public AnchorConfidence GetAnchorConfidence(TrackableId xrAnchorId)
            {
                for (int i = 0; i < currentAnchors.Count; i++)
                {
                    if (currentAnchors[i].AnchorObject.trackableId == xrAnchorId)
                    {
                        uint anchorCon = 0;
                        var result = MagicLeapSpatialAnchorsFeature.NativeBindings.MLOpenXRGetSpatialAnchorConfidence(currentAnchors[i].AnchorId, out anchorCon);

                        if (result != XrResult.Success)
                        {
                            Debug.LogError("XRAnchorSubsystem: GetAnchorConfidence failed at " + currentAnchors[i].AnchorId.ToString() + " with result: " + result);
                        }

                        return (AnchorConfidence)anchorCon;
                    }
                }

                return AnchorConfidence.NotFound;

            }

            public ulong GetAnchorId(TrackableId xrAnchorId)
            {
                for (int i = 0; i < currentAnchors.Count; i++)
                {
                    if (currentAnchors[i].AnchorObject.trackableId == xrAnchorId)
                    {
                        return currentAnchors[i].AnchorId;
                    }
                }

                return 0u;
            }

            public Pose GetAnchorPoseFromID(ulong anchorId)
            {
                unsafe
                {
                    Pose returnedPose = default;
                    MagicLeapSpatialAnchorsFeature.NativeBindings.MLOpenXRGetSpatialAnchorPose(&anchorId, &returnedPose);
                    return returnedPose;
                }
            }
        }

        public static void RegisterDescriptor()
        {
            Debug.Log("XRAnchorSubsystem: Registering Anchors Subsystem");
            XRAnchorSubsystemDescriptor.Create(new XRAnchorSubsystemDescriptor.Cinfo
            {
                id = MagicLeapXrProvider.AnchorSubsystemId,
                providerType = typeof(MagicLeapProvider),
                subsystemTypeOverride = typeof(MLXrAnchorSubsystem)
            });
        }
        
    }
}
#endif
