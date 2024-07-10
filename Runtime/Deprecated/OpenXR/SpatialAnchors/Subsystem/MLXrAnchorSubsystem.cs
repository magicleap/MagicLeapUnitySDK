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
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using MagicLeapSpatialAnchorsNativeTypes;
    /// <summary>
    ///     The Magic Leap implementation of the <c>XRAnchorSubsystem</c>. Do not create this directly.
    ///     Use <c>XRAnchorSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Subsystems")]
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
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorId)} without \"Start\"ing the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.GetAnchorId(arAnchor.trackableId);
        }

        public Pose GetAnchorPoseFromID(ulong anchorId)
        {
            if (!running)
                throw new InvalidOperationException($"Can't call {nameof(GetAnchorPoseFromID)} without \"Start\"ing the anchor subsystem!");

            var magicLapProvider = provider as MagicLeapProvider;
            return magicLapProvider.GetAnchorPoseFromID(anchorId);
        }

        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MagicLeapProvider")]

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

            private HashSet<ulong> pendingFutures = new HashSet<ulong>();
            private HashSet<ulong> readyFutures = new HashSet<ulong>();

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
                                if (futureResult == (XrResult)Utils.MLXrResult.FutureInvalidEXT)
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
                                    if (currentAnchors[i].AnchorObject.pose == currentPose)
                                    {
                                        var updatedAnchor = new AnchorSubsystemAnchorData();
                                        updatedAnchor.AnchorId = currentSpace;
                                        updatedAnchor.AnchorObject = new XRAnchor(currentAnchors[i].AnchorObject.trackableId, currentPose, TrackingState.Tracking, IntPtr.Zero);
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

            public Pose GetAnchorPoseFromID(ulong anchorId)
            {
                return anchorsFeature.SpaceInfoNativeFunctions.GetUnityPose(anchorId, anchorsFeature.AppSpace, anchorsFeature.NextPredictedDisplayTime);
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
