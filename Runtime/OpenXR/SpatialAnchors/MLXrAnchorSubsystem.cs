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
using MagicLeap.OpenXR.Constants;
using MagicLeap.OpenXR.Futures;
using MagicLeap.OpenXR.Features.SpatialAnchors;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Subsystems
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

        internal struct AnchorStoragePending
        {
            internal Pose Pose;
            internal ulong Id;
            internal string mapPositionId;
        }

        public AnchorConfidence GetAnchorConfidence(ARAnchor arAnchor)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorConfidence)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;

            return magicLapProvider.GetAnchorConfidence(arAnchor.trackableId);
        }

        public ulong GetAnchorId(ARAnchor arAnchor)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorId)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.GetAnchorId(arAnchor.trackableId);
        }

        public string GetAnchorMapPositionId(ARAnchor arAnchor)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorMapPositionId)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.GetAnchorMapPositionId(arAnchor.trackableId);
        }

        [Obsolete("GetAnchorPoseFromID is obsolete. Use GetAnchorPoseFromId instead.", false)]
        public Pose GetAnchorPoseFromID(ulong anchorId)
        {
            return GetAnchorPoseFromId(anchorId);
        }

        public Pose GetAnchorPoseFromId(ulong anchorId)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorPoseFromId)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.GetAnchorPoseFromID(anchorId);
        }

        public Pose GetAnchorPose(ARAnchor arAnchor)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorPose)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.GetAnchorPose(arAnchor.trackableId);
        }

        public bool IsStoredAnchor(ARAnchor arAnchor)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(IsStoredAnchor)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.IsStoredAnchor(arAnchor.trackableId);
        }

        public TrackableId GetTrackableIdFromMapPositionId(string mapPositionId)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetTrackableIdFromMapPositionId)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.GetTrackableIdFromMapPositionId(mapPositionId);
        }

        internal void AddStorageAnchorToSubsystem(Pose pose, ulong anchorId, string anchorMapPositionId)
        {
            if(!running)
                throw new InvalidOperationException($"Can't call {nameof(AddStorageAnchorToSubsystem)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            magicLapProvider.AddPendingStorageAnchor(pose, anchorId, anchorMapPositionId);
        }

        internal void PublishLocalAnchor(ulong anchorId, string anchorMapPositionId)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(PublishLocalAnchor)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            magicLapProvider.PublishLocalAnchor(anchorId, anchorMapPositionId);
        }

        internal void DeleteStoredAnchors(List<string> anchorMapPositionIds)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(DeleteStoredAnchors)} without Starting the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            foreach (string anchorMapPositionId in anchorMapPositionIds)
            {
                magicLapProvider.DeleteStoredAnchor(anchorMapPositionId);
            }
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
                public string AnchorMapPositionId;
                public XRAnchor AnchorObject;
                public ulong AnchorFuture;
            }

            private HashSet<ulong> pendingFutures = new HashSet<ulong>();
            private HashSet<ulong> readyFutures = new HashSet<ulong>();
            private List<TrackableId> pendingDeleteStoredAnchors = new List<TrackableId>();

            private List<AnchorSubsystemAnchorData> currentAnchors = new List<AnchorSubsystemAnchorData>();
            private List<XRAnchor> addedAnchors = new List<XRAnchor>();
            private List<XRAnchor> updatedAnchors = new List<XRAnchor>();
            private List<TrackableId> deletedAnchors = new List<TrackableId>();

            private List<AnchorStoragePending> pendingStorageAnchors = new List<AnchorStoragePending>();

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
                unsafe
                {
                    if (anchorsFeature)
                    {
                        if (pendingFutures.Count > 0)
                        {
                            HashSet<ulong> toRemove = new HashSet<ulong>();

                            foreach (ulong future in pendingFutures)
                            {
                                anchorsFeature.SpatialAnchorsNativeFunctions.PollFuture(future, out XrFutureState pollState, out XrResult futureResult);
                                if (futureResult == (XrResult)MLXrResult.FutureInvalidEXT)
                                {
                                    toRemove.Add(future);
                                }

                                if (pollState == XrFutureState.Ready)
                                {
                                    readyFutures.Add(future);
                                    toRemove.Add(future);
                                }
                            }

                            pendingFutures.ExceptWith(toRemove);

                            toRemove.Clear();
                        }

                        foreach (ulong future in readyFutures)
                        {
                            // Completion From Pose only has one result.
                            ulong currentSpace;
                            var completionData = new XrCreateSpatialAnchorsCompletion
                            {
                                Type = XrSpatialAnchorsStructTypes.XrTypeCreateSpatialAnchorsCompletion,
                                SpaceCount = 1,
                                Spaces = &currentSpace
                            };

                            XrResult resultCode = anchorsFeature.SpatialAnchorsNativeFunctions.XrCreateSpatialAnchorsComplete(anchorsFeature.AppSession, future, out completionData);
                            bool result = Utils.DidXrCallSucceed(resultCode, nameof(anchorsFeature.SpatialAnchorsNativeFunctions.XrCreateSpatialAnchorsAsync));
                            if (result)
                            {
                                Pose currentPose = GetAnchorPoseFromID(currentSpace);
                                bool found = false;
                                for (int i = 0; i < currentAnchors.Count; i++)
                                {
                                    if (currentAnchors[i].AnchorFuture == future)
                                    {
                                        var updatedAnchor = new AnchorSubsystemAnchorData
                                        {
                                            AnchorId = currentSpace,
                                            AnchorObject = new XRAnchor(currentAnchors[i].AnchorObject.trackableId, currentPose, TrackingState.Tracking, IntPtr.Zero),
                                            AnchorFuture = Values.InvalidHandle
                                        };
                                        currentAnchors[i] = updatedAnchor;
                                        updatedAnchors.Add(updatedAnchor.AnchorObject);
                                        found = true;
                                        break;
                                    }
                                }

                                if (!found)
                                {
                                    Debug.LogWarning("XRAnchorSubsystem: Anchor created outside of the Anchor Manager at " + currentPose.ToString());
                                    TrackableId newId = GenerateTrackableId();

                                    AnchorSubsystemAnchorData newAnchor = new AnchorSubsystemAnchorData();
                                    newAnchor.AnchorId = currentSpace;
                                    newAnchor.AnchorObject = new XRAnchor(newId, currentPose, TrackingState.Tracking, IntPtr.Zero);

                                    currentAnchors.Add(newAnchor);

                                    addedAnchors.Add(newAnchor.AnchorObject);
                                }
                            }
                        }
                        readyFutures.Clear();
                    }
                }

                // Add Anchors From Storage
                foreach (AnchorStoragePending storedAnchor in pendingStorageAnchors)
                {
                    AnchorSubsystemAnchorData existingAnchor = currentAnchors.Find(anchor => anchor.AnchorId == storedAnchor.Id);

                    if (existingAnchor.AnchorId != 0u)
                    {
                        // Sanity check for the subsystem tracking an anchor and not knowing it is stored.
                        if (string.IsNullOrEmpty(existingAnchor.AnchorMapPositionId))
                        {
                            int index = currentAnchors.FindIndex(a => a.AnchorObject.trackableId == existingAnchor.AnchorObject.trackableId);

                            if (index != -1)
                            {
                                var updatedAnchor = new AnchorSubsystemAnchorData();
                                updatedAnchor.AnchorId = existingAnchor.AnchorId;
                                updatedAnchor.AnchorMapPositionId = storedAnchor.mapPositionId;
                                updatedAnchor.AnchorObject = new XRAnchor(currentAnchors[index].AnchorObject.trackableId, currentAnchors[index].AnchorObject.pose, TrackingState.Tracking, IntPtr.Zero);
                                currentAnchors[index] = updatedAnchor;
                                updatedAnchors.Add(existingAnchor.AnchorObject);
                            }         
                        }
                        continue;
                    }

                    TrackableId newId = GenerateTrackableId();

                    AnchorSubsystemAnchorData newAnchor = new AnchorSubsystemAnchorData();
                    newAnchor.AnchorId = storedAnchor.Id;
                    newAnchor.AnchorMapPositionId = storedAnchor.mapPositionId;
                    newAnchor.AnchorObject = new XRAnchor(newId, storedAnchor.Pose, TrackingState.Tracking, IntPtr.Zero);

                    currentAnchors.Add(newAnchor);

                    addedAnchors.Add(newAnchor.AnchorObject);
                }

                pendingStorageAnchors.Clear();


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
                    var createInfo = new XrSpatialAnchorsCreateInfoFromPose
                    {
                        Type = XrSpatialAnchorsStructTypes.XrTypeSpatialAnchorsCreateInfoFromPose,
                        BaseSpace = anchorsFeature.AppSpace,
                        PoseInBaseSpace = XrPose.GetFromPose(pose),
                        Time = anchorsFeature.NextPredictedDisplayTime
                    };

                    var baseHeader = (XrSpatialAnchorsCreateInfoBaseHeader*)(&createInfo);

                    XrResult resultCode = anchorsFeature.SpatialAnchorsNativeFunctions.XrCreateSpatialAnchorsAsync(anchorsFeature.AppSession, baseHeader, out ulong pendingFuture);
                    result = Utils.DidXrCallSucceed(resultCode, nameof(anchorsFeature.SpatialAnchorsNativeFunctions.XrCreateSpatialAnchorsAsync));
                    
                    if (!result)
                    {
                        Debug.LogError("XRAnchorSubsystem: Error creating Anchor at " + pose.ToString());
                    }
                    else
                    {
                        pendingFutures.Add(pendingFuture);
                        TrackableId newId = GenerateTrackableId();

                        AnchorSubsystemAnchorData newAnchor = new AnchorSubsystemAnchorData();
                        newAnchor.AnchorId = 0u;
                        newAnchor.AnchorObject = new XRAnchor(newId, pose, TrackingState.None, IntPtr.Zero);
                        newAnchor.AnchorFuture = pendingFuture;
                        xrAnchor = newAnchor.AnchorObject;

                        currentAnchors.Add(newAnchor);

                        addedAnchors.Add(newAnchor.AnchorObject);
                    }
                }

                return result;
            }

            public override bool TryRemoveAnchor(TrackableId trackableId)
            {
                // Stored Anchor is being deleted from the localization map and subsystem.
                if (pendingDeleteStoredAnchors.Remove(trackableId))
                {
                    return true;
                }

                // Stored Anchor is being removed from subsystem only.
                if (IsStoredAnchor(trackableId))
                {
                    AnchorSubsystemAnchorData foundAnchor = currentAnchors.Find(anchor => anchor.AnchorObject.trackableId == trackableId);

                    deletedAnchors.Add(trackableId);
                    currentAnchors.Remove(foundAnchor);
                    return true;
                }

                // Local Anchor is being removed from subsystem.
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
                    if(anchorsFeature.AppSession == default)
                    {
                        deletedAnchors.Add(currentAnchors[foundIndex].AnchorObject.trackableId);
                        return true;
                    }

                    bool deleteResult = false;

                    unsafe
                    {
                        ulong anchorId = currentAnchors[foundIndex].AnchorId;
                        XrResult resultCode = anchorsFeature.SpaceInfoNativeFunctions.XrDestroySpace(anchorId);
                        deleteResult = Utils.DidXrCallSucceed(resultCode, nameof(anchorsFeature.SpaceInfoNativeFunctions.XrDestroySpace));
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
                        ulong anchorId = GetAnchorId(xrAnchorId);
                        if(anchorId == 0u)
                        {
                            return AnchorConfidence.NotFound;
                        }
                        unsafe
                        {
                            bool result = false;
                            uint anchorCon = 0;
                            var anchorState = new XrSpatialAnchorState
                            {
                                Type = XrSpatialAnchorsStructTypes.XrTypeSpatialAnchorState
                            };

                            XrResult resultCode = anchorsFeature.SpatialAnchorsNativeFunctions.XrGetSpatialAnchorState(anchorId, out anchorState);
                            result = Utils.DidXrCallSucceed(resultCode, nameof(anchorsFeature.SpatialAnchorsNativeFunctions.XrGetSpatialAnchorState));

                            if (!result)
                            {
                                Debug.LogError("XRAnchorSubsystem: GetAnchorConfidence failed at " + currentAnchors[i].AnchorId.ToString() + " with result: " + resultCode);
                            }
                            else
                            {
                                // MLXrAnchorSubsystem adds a confidence value of NotFound.
                                anchorCon = ((uint)anchorState.Confidence + 1);
                            }

                            return (AnchorConfidence)anchorCon;
                        }                       
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

            public string GetAnchorMapPositionId(TrackableId xrAnchorId)
            {
                AnchorSubsystemAnchorData foundAnchor = currentAnchors.Find(anchor => anchor.AnchorObject.trackableId == xrAnchorId);
                return foundAnchor.AnchorMapPositionId;
            }

            public Pose GetAnchorPoseFromID(ulong anchorId)
            {
                return anchorsFeature.SpaceInfoNativeFunctions.GetUnityPose(anchorId, anchorsFeature.AppSpace, anchorsFeature.NextPredictedDisplayTime);
            }

            public Pose GetAnchorPose(TrackableId xrAnchorId)
            {
                AnchorSubsystemAnchorData foundAnchor = currentAnchors.Find(anchor => anchor.AnchorObject.trackableId == xrAnchorId);

                if(foundAnchor.AnchorId == 0u)
                {
                    Debug.LogError("XRAnchorSubsystem: GetAnchorPose failed to find the requested Anchor in the subsystem.");
                    return Pose.identity;
                }

                return anchorsFeature.SpaceInfoNativeFunctions.GetUnityPose(foundAnchor.AnchorId, anchorsFeature.AppSpace, anchorsFeature.NextPredictedDisplayTime);
            }

            public bool IsStoredAnchor(TrackableId xrAnchorId)
            {
                AnchorSubsystemAnchorData foundAnchor = currentAnchors.Find(anchor => anchor.AnchorObject.trackableId == xrAnchorId);

                return !string.IsNullOrEmpty(foundAnchor.AnchorMapPositionId);
            }

            public TrackableId GetTrackableIdFromMapPositionId(string mapPositionId)
            {
                AnchorSubsystemAnchorData foundAnchor = currentAnchors.Find(anchor => anchor.AnchorMapPositionId == mapPositionId);

                return foundAnchor.AnchorObject.trackableId;
            }

            internal void AddPendingStorageAnchor(Pose pose, ulong anchorId, string anchorMapPositionId)
            {
                AnchorStoragePending newStorageAnchor = new AnchorStoragePending
                {
                    Pose = pose,
                    Id = anchorId,
                    mapPositionId = anchorMapPositionId
                };

                pendingStorageAnchors.Add(newStorageAnchor);
            }

            internal void PublishLocalAnchor(ulong anchorId, string anchorMapPositionId)
            {
                for (int i = 0; i < currentAnchors.Count; i++)
                {
                    if (currentAnchors[i].AnchorId == anchorId)
                    {
                        var updatedAnchor = new AnchorSubsystemAnchorData();
                        updatedAnchor.AnchorId = anchorId;
                        updatedAnchor.AnchorMapPositionId = anchorMapPositionId;
                        updatedAnchor.AnchorObject = new XRAnchor(currentAnchors[i].AnchorObject.trackableId, currentAnchors[i].AnchorObject.pose, TrackingState.Tracking, IntPtr.Zero);
                        currentAnchors[i] = updatedAnchor;

                        updatedAnchors.Add(updatedAnchor.AnchorObject);
                        break;
                    }
                }
            }

            internal void DeleteStoredAnchor(string anchorMapPositionId)
            {
                AnchorSubsystemAnchorData foundAnchor = currentAnchors.Find(anchor => anchor.AnchorMapPositionId == anchorMapPositionId);

                if(foundAnchor.AnchorId != 0u)
                {
                    pendingDeleteStoredAnchors.Add(foundAnchor.AnchorObject.trackableId);
                    deletedAnchors.Add(foundAnchor.AnchorObject.trackableId);
                    currentAnchors.Remove(foundAnchor);
                }
                else
                {
                    Debug.LogError("XRAnchorSubsystem: Failed to find the requested Anchor in the subsystem to delete.");
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
