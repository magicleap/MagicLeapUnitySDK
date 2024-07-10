// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System;
using System.Collections.Generic;
using System.Linq;
using MagicLeap.OpenXR.Constants;
using MagicLeap.OpenXR.Futures;
using MagicLeap.OpenXR.Spaces;
using MagicLeap.OpenXR.Subsystems;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.LowLevel;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features.SpatialAnchors
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Spatial Anchors Storage",
        Desc = "Expand Spatial Anchors to allow to Publish, Delete, and update on Localized Maps.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new[] { BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = ExtensionName
    )]
#endif // UNITY_EDITOR
    public class MagicLeapSpatialAnchorsStorageFeature : MagicLeapOpenXRFeatureBase
    {
        public const string FeatureId = "com.magicleap.openxr.feature.ml2_spatialanchorstorage";
        private const string ExtensionName = "XR_ML_spatial_anchors_storage XR_EXT_future XR_KHR_locate_spaces";

        /// <summary>
        /// Receive an ulong AnchorId and a string anchorMapPositionId for the published anchor.
        /// </summary>
        public event Action<ulong, string> OnPublishComplete;
        /// <summary>
        /// Receive a list of string anchorMapPositionIds of anchors returned from the Query of the Localized Map.
        /// </summary>
        public event Action<List<string>> OnQueryComplete;
        /// <summary>
        /// Receive a list of string anchorMapPositionIds of anchors that have been deleted from the Localized Map.
        /// </summary>
        public event Action<List<string>> OnDeletedComplete;
        /// <summary>
        /// Receive a list of string anchorMapPositionIds of anchors that have had their expiration updated on the Localized Map.
        /// </summary>
        public event Action<List<string>> OnUpdateExpirationCompleted;
        /// <summary>
        /// Receive the created anchor's Pose, the anchor's ulong AnchorId, the anchor's string anchorMapPositionId for the localized Map, and the XResult of the request to create the anchor.
        /// </summary>
        public event Action<Pose, ulong, string, XrResult>  OnCreationCompleteFromStorage;

        private MagicLeapSpatialAnchorsFeature anchorsFeature;

        private bool startedSpatialAnchorStorage;

        private MagicLeapSpatialAnchorsStorageNativeFunctions spatialAnchorsStorageNativeFunctions;
        private SpacesNativeFunctions locateSpacesNativeFunctions;

        private MLXrAnchorSubsystem activeSubsystem;

        private ulong storageHandle;

        private struct AnchorsStorageData
        {
            internal int Count;
            internal List<string> Uuid;
        }

        private struct AnchorsStoragePendingPublish
        {
            internal int Count;
            internal List<ulong> AnchorIds;
        }

        private readonly HashSet<ulong> pendingStorageAnchors = new HashSet<ulong>();
        private readonly Dictionary<ulong, AnchorsStorageData> pendingStorageAnchorsData = new Dictionary<ulong, AnchorsStorageData>();

        private readonly HashSet<ulong> pendingPublishRequests = new HashSet<ulong>();
        private readonly Dictionary<ulong, AnchorsStoragePendingPublish> pendingStorageAnchorsPublishData = new Dictionary<ulong, AnchorsStoragePendingPublish>();

        private readonly HashSet<ulong> pendingQueries = new HashSet<ulong>();

        private readonly HashSet<ulong> pendingDeleteRequests = new HashSet<ulong>();
        private readonly Dictionary<ulong, AnchorsStorageData> pendingStorageAnchorsDeleteData = new Dictionary<ulong, AnchorsStorageData>();

        private readonly HashSet<ulong> pendingUpdateRequests = new HashSet<ulong>();
        private readonly Dictionary<ulong, AnchorsStorageData> pendingStorageAnchorsUpdateData = new Dictionary<ulong, AnchorsStorageData>();

        protected override bool UsesExperimentalExtensions => true;

        private struct AnchorsStorageUpdateType
        { }

        protected override bool OnInstanceCreate(ulong xrInstance)
        {
            var exts = ExtensionName.Split(' ');
            foreach (var ext in exts)
            {
                if (!OpenXRRuntime.IsExtensionEnabled(ext))
                {
                    Debug.LogError($"{ext} is not enabled. Disabling {nameof(MagicLeapSpatialAnchorsStorageFeature)}");
                    return false;
                }
            }
            
            var updateSystem = new PlayerLoopSystem
            {
                subSystemList = Array.Empty<PlayerLoopSystem>(),
                type = typeof(AnchorsStorageUpdateType),
                updateDelegate = AnchorsStoragePlayerLoop,
            };
            var playerLoop = PlayerLoop.GetCurrentPlayerLoop();
            if (!PlayerLoopUtil.InstallIntoPlayerLoop(ref playerLoop, updateSystem, PlayerLoopUtil.InstallPath))
                throw new Exception("Unable to install Spatial Anchors Storage Update delegate into player loop!");

            PlayerLoop.SetPlayerLoop(playerLoop);

            bool instanceCreateResult = base.OnInstanceCreate(xrInstance);

            spatialAnchorsStorageNativeFunctions = CreateNativeFunctions<MagicLeapSpatialAnchorsStorageNativeFunctions>();
            locateSpacesNativeFunctions = CreateNativeFunctions<SpacesNativeFunctions>();

            return instanceCreateResult;
        }

        public bool CreateSpatialAnchorsFromStorage(List<string> anchorMapPositionIds)
        {
            if (!startedSpatialAnchorStorage)
            {
                return false;
            }

            if (anchorMapPositionIds.Count == 0)
            {
                Debug.LogError("CreateSpatialAnchors sent an empty list of AnchorMapPositionIds.");
                return false;
            }

            if(anchorsFeature == null)
            {
                anchorsFeature = OpenXRSettings.Instance.GetFeature<MagicLeapSpatialAnchorsFeature>();
            }

            if (!anchorsFeature.enabled)
            {
                Debug.LogError("CreateSpatialAnchors requires an active MagicLeapSpatialAnchorsFeature OpenXRFeature.");
                return false;
            }

            unsafe
            {
                NativeArray<XrUUID> anchorUuidsToCreate = new NativeArray<XrUUID>(anchorMapPositionIds.Count, Allocator.Temp);

                for (int i = 0; i < anchorUuidsToCreate.Length; ++i)
                {
                    anchorUuidsToCreate[i] = new XrUUID(anchorMapPositionIds[i]);
                }

                var createInfo = new XrSpatialAnchorsCreateInfoFromUuids
                {
                    Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsCreateInfoFromUUIDs,
                    Storage = storageHandle,
                    UuidCount = (uint)anchorMapPositionIds.Count,
                    Uuids = (XrUUID*)anchorUuidsToCreate.GetUnsafePtr()
                };

                var baseHeader = (XrSpatialAnchorsCreateInfoBaseHeader*)(&createInfo);

                XrResult resultCode = anchorsFeature.SpatialAnchorsNativeFunctions.XrCreateSpatialAnchorsAsync(AppSession, baseHeader, out ulong pendingFuture);
                bool result = Utils.DidXrCallSucceed(resultCode, nameof(anchorsFeature.SpatialAnchorsNativeFunctions.XrCreateSpatialAnchorsAsync));

                if(!result)
                {
                    Debug.LogError("CreateSpatialAnchors failed to send request to create anchors from AnchorMapPositionId list.");
                }
                else
                {
                    pendingStorageAnchors.Add(pendingFuture);

                    var savedPendingData = new AnchorsStorageData
                    {
                        Count = anchorMapPositionIds.Count,
                        Uuid = anchorMapPositionIds
                    };

                    pendingStorageAnchorsData.Add(pendingFuture, savedPendingData);
                }

                return result;
            }
        }

        /// <summary>
        /// Determine the maximum number of anchors to accept their completion status each update.
        /// </summary>
        /// <param name="position">The position of the center of the query</param>
        /// <param name="radius">The radius of the search area in meters.</param>
        public bool QueryStoredSpatialAnchors(Vector3 position, float radius)
        {
            if (!startedSpatialAnchorStorage)
            {
                return false;
            }

            unsafe
            {
                var queryInfo = new XrSpatialAnchorsQueryInfoRadius
                {
                    Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsQueryInfoRadius,
                    Center = position.InvertZ(),
                    Radius = radius,
                    Time = NextPredictedDisplayTime,
                    BaseSpace = AppSpace
                };

                var baseHeader = (XrSpatialAnchorsQueryInfoBaseHeader*)(&queryInfo);

                XrResult resultCode = spatialAnchorsStorageNativeFunctions.XrQuerySpatialAnchorsAsync(storageHandle, baseHeader, out ulong pendingFuture);
                bool result = Utils.DidXrCallSucceed(resultCode, nameof(spatialAnchorsStorageNativeFunctions.XrQuerySpatialAnchorsAsync));

                if (!result)
                {
                    Debug.LogError("CreateSpatialAnchors failed to send request to create anchors from AnchorMapPositionId list.");
                }
                else
                {
                    pendingQueries.Add(pendingFuture);
                }

                return result;
            }
        }

        /// <summary>
        /// Publish local anchors to Spatial Anchor Storage using the MagicLeap Anchor Id.
        /// </summary>
        /// <param name="anchorIds">The list of AnchorIds to publish. These were assigned in the OnCreationComplete event in MagicLeapSpatialAnchorsFeature.</param>
        /// <param name="expiration">The time in seconds since epoch after which these anchors may: expire. Use 0 for permanent anchors. The system may retain them longer.</param>
        public bool PublishSpatialAnchorsToStorage(List<ulong> anchorIds, ulong expiration)
        {
            if (!startedSpatialAnchorStorage)
            {
                return false;
            }

            if (anchorIds.Count == 0)
            {
                Debug.LogError("PublishSpatialAnchorsToStorage sent an empty list of AnchorIds.");
                return false;
            }

            unsafe
            {
                using NativeArray<ulong> anchorsToPublish = new NativeArray<ulong>(anchorIds.ToArray(), Allocator.Temp);

                var publishInfo = new XrSpatialAnchorsPublishInfo
                {
                    Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsPublishInfo,
                    AnchorCount = (uint)anchorIds.Count,
                    Anchors = (ulong*)anchorsToPublish.GetUnsafePtr(),
                    Expiration = expiration
                };

                XrResult resultCode = spatialAnchorsStorageNativeFunctions.XrPublishSpatialAnchorsAsync(storageHandle, &publishInfo, out ulong pendingFuture);
                bool result = Utils.DidXrCallSucceed(resultCode, nameof(anchorsFeature.SpatialAnchorsNativeFunctions.XrCreateSpatialAnchorsAsync));

                if (!result)
                {
                    Debug.LogError("PublishSpatialAnchorsToStorage failed to send request to publish anchors from AnchorId list.");
                    return false;
                }

                pendingPublishRequests.Add(pendingFuture);

                var savedPendingData = new AnchorsStoragePendingPublish
                {
                    Count = anchorIds.Count,
                    AnchorIds = anchorIds
                };

                pendingStorageAnchorsPublishData.Add(pendingFuture, savedPendingData);

                return true;
            }
        }

        /// <summary>
        /// Publish local anchors to Spatial Anchor Storage using ARAnchors. Will return false if XRAnchorSubsystem is not loaded.
        /// </summary>
        /// <param name="anchors">The list of ARAnchors to publish. TrackingState must be Tracking to be valid for publish. Will be ignored if not.</param>
        /// <param name="expiration">The time in seconds since epoch after which these anchors may: expire. Use 0 for permanent anchors. The system may retain them longer.</param>
        public bool PublishSpatialAnchorsToStorage(List<ARAnchor> anchors, ulong expiration)
        {
            if (!startedSpatialAnchorStorage)
            {
                return false;
            }

            if(activeSubsystem == null)
            {
                return false;
            }

            List<ulong> anchorIds = new List<ulong>();

            foreach (ARAnchor anchor in anchors)
            {
                if(anchor.trackingState != TrackingState.Tracking)
                {
                    continue;
                }

                ulong anchorid = activeSubsystem.GetAnchorId(anchor);
                if (anchorid != 0u)
                {
                    anchorIds.Add(anchorid);
                }
                else
                {
                    Debug.LogWarning($"PublishSpatialAnchorsToStorage is unable to locate an ID for {anchor.trackableId} and it will be excluded from the list to publish.");
                }
            }

            return PublishSpatialAnchorsToStorage(anchorIds, expiration);
        }

        [Obsolete("DeleteStoredSpatialAnchor is obsolete. Use DeleteStoredSpatialAnchors instead.", false)]
        public bool DeleteStoredSpatialAnchor(List<string> anchorMapPositionIds)
        {
            return DeleteStoredSpatialAnchors(anchorMapPositionIds);
        }

        /// <summary>
        /// Delete published anchors from Spatial Anchor Storage.
        /// </summary>
        /// <param name="anchorMapPositionIds">The list of AnchorMapPositionIds to Delete. These were assigned in the OnPublishComplete event in MagicLeapSpatialAnchorsStorageFeature.</param>
        public bool DeleteStoredSpatialAnchors(List<string> anchorMapPositionIds)
        {
            if (!startedSpatialAnchorStorage)
            {
                return false;
            }

            if (anchorMapPositionIds.Count == 0)
            {
                Debug.LogError("DeleteStoredSpatialAnchor sent an empty list of AnchorMapPositionIds.");
                return false;
            }

            unsafe
            {
                NativeArray<XrUUID> anchorUuidsToDelete = new NativeArray<XrUUID>(anchorMapPositionIds.Count, Allocator.Temp);

                for (int i = 0; i < anchorUuidsToDelete.Length; ++i)
                {
                    anchorUuidsToDelete[i] = new XrUUID(anchorMapPositionIds[i]);
                }

                var deleteInfo = new XrSpatialAnchorsDeleteInfo
                {
                    Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsDeleteInfo,
                    UuidCount = (uint)anchorMapPositionIds.Count,
                    Uuids = (XrUUID*)anchorUuidsToDelete.GetUnsafePtr()
                };

                XrResult resultCode = spatialAnchorsStorageNativeFunctions.XrDeleteSpatialAnchorsAsync(storageHandle, &deleteInfo, out ulong pendingFuture);
                bool result = Utils.DidXrCallSucceed(resultCode, nameof(spatialAnchorsStorageNativeFunctions.XrDeleteSpatialAnchorsAsync));

                if(!result)
                {
                    Debug.LogError("CreateSpatialAnchors failed to send request to create anchors from AnchorMapPositionId list.");
                    return false;
                }
                
                pendingDeleteRequests.Add(pendingFuture);

                var savedDeleteData = new AnchorsStorageData
                {
                    Count = anchorMapPositionIds.Count,
                    Uuid = anchorMapPositionIds
                };

                pendingStorageAnchorsDeleteData.Add(pendingFuture, savedDeleteData);
                
                return true;
            }
        }

        public bool DeleteStoredSpatialAnchors(List<ARAnchor> anchors)
        {
            if (!startedSpatialAnchorStorage)
            {
                return false;
            }

            if (activeSubsystem == null)
            {
                return false;
            }

            List<string> anchorMapPositionIds = new List<string>();

            foreach (ARAnchor anchor in anchors)
            {
                if (anchor.trackingState != TrackingState.Tracking)
                {
                    continue;
                }

                string anchorMapPositionId = activeSubsystem.GetAnchorMapPositionId(anchor);
                if (!String.IsNullOrEmpty(anchorMapPositionId))
                {
                    anchorMapPositionIds.Add(anchorMapPositionId);
                }
                else
                {
                    Debug.LogWarning($"DeleteStoredSpatialAnchor is unable to locate an AnchorMapPositionId for {anchor.trackableId} and it will be excluded from the list of anchors to be deleted.");
                }
            }

            return DeleteStoredSpatialAnchors(anchorMapPositionIds);
        }

        /// <summary>
        /// Update the expiration time for published anchors in Spatial Anchor Storage.
        /// </summary>
        /// <param name="anchorMapPositionIds">The list of AnchorMapPositionIds to Delete. These were assigned in the OnPublishComplete event in MagicLeapSpatialAnchorsStorageFeature.</param>
        /// <param name="expiration">The time in seconds since epoch after which these anchors may: expire. The system may retain them longer.</param>
        [Obsolete("UpdateExpirationonStoredSpatialAnchor will be deprecated. Use UpdateExpirationForStoredSpatialAnchor.")]
        public bool UpdateExpirationonStoredSpatialAnchor(List<string> anchorMapPositionIds, ulong expiration) => UpdateExpirationForStoredSpatialAnchor(anchorMapPositionIds, expiration);

        /// <summary>
        /// Update the expiration time for published anchors in Spatial Anchor Storage.
        /// </summary>
        /// <param name="anchorMapPositionIds">The list of AnchorMapPositionIds to Delete. These were assigned in the OnPublishComplete event in MagicLeapSpatialAnchorsStorageFeature.</param>
        /// <param name="expiration">The time in seconds since epoch after which these anchors may: expire. The system may retain them longer.</param>
        public bool UpdateExpirationForStoredSpatialAnchor(List<string> anchorMapPositionIds, ulong expiration)
        {
            if (!startedSpatialAnchorStorage)
            {
                return false;
            }

            if (anchorMapPositionIds.Count == 0)
            {
                Debug.LogError($"{nameof(UpdateExpirationForStoredSpatialAnchor)} sent an empty list of AnchorMapPositionIds.");
                return false;
            }

            unsafe
            {
                NativeArray<XrUUID> anchorUuidsToUpdate = new NativeArray<XrUUID>(anchorMapPositionIds.Count, Allocator.Temp);

                for (int i = 0; i < anchorUuidsToUpdate.Length; ++i)
                {
                    anchorUuidsToUpdate[i] = new XrUUID(anchorMapPositionIds[i]);
                }

                var updateInfo = new XrSpatialAnchorsUpdateExpirationInfo
                {
                    Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsUpdateExpirationInfo,
                    UuidCount = (uint)anchorMapPositionIds.Count,
                    Uuids = (XrUUID*)anchorUuidsToUpdate.GetUnsafePtr(),
                    Expiration = expiration
                };

                XrResult resultCode = spatialAnchorsStorageNativeFunctions.XrUpdateSpatialAnchorsExpirationAsync(storageHandle, &updateInfo, out ulong pendingFuture);
                bool result = Utils.DidXrCallSucceed(resultCode, nameof(spatialAnchorsStorageNativeFunctions.XrUpdateSpatialAnchorsExpirationAsync));

                if (!result)
                {
                    Debug.LogError($"{nameof(UpdateExpirationForStoredSpatialAnchor)} failed to send request to update anchors expiration from AnchorMapPositionId list.");
                    return false;
                }

                pendingUpdateRequests.Add(pendingFuture);

                var savedUpdateData = new AnchorsStorageData
                {
                    Count = anchorMapPositionIds.Count,
                    Uuid = anchorMapPositionIds
                };

                pendingStorageAnchorsUpdateData.Add(pendingFuture, savedUpdateData);

                return true;
            }
        }

        public bool UpdateExpirationForStoredSpatialAnchor(List<ARAnchor> anchors, ulong expiration)
        {
            if (!startedSpatialAnchorStorage)
            {
                return false;
            }

            if (activeSubsystem == null)
            {
                return false;
            }

            List<string> anchorMapPositionIds = new List<string>();

            foreach (ARAnchor anchor in anchors)
            {
                if (anchor.trackingState != TrackingState.Tracking)
                {
                    continue;
                }

                string anchorMapPositionId = activeSubsystem.GetAnchorMapPositionId(anchor);
                if (!String.IsNullOrEmpty(anchorMapPositionId))
                {
                    anchorMapPositionIds.Add(anchorMapPositionId);
                }
                else
                {
                    Debug.LogWarning($"UpdateExpirationForStoredSpatialAnchor is unable to locate an AnchorMapPositionId for {anchor.trackableId} and will be excluded from the list to delete.");
                }
            }

            return UpdateExpirationForStoredSpatialAnchor(anchorMapPositionIds, expiration);
        }

        private void AnchorsStoragePlayerLoop()
        {
            if(!startedSpatialAnchorStorage)
            {
                unsafe
                {
                    var createInfo = new XrSpatialAnchorsCreateStorageInfo
                    {
                        Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsCreateStorageInfo
                    };

                    XrResult createResult = spatialAnchorsStorageNativeFunctions.XrCreateSpatialAnchorsStorage(AppSession, &createInfo, out storageHandle);

                    if (!Utils.DidXrCallSucceed(createResult, nameof(spatialAnchorsStorageNativeFunctions.XrCreateSpatialAnchorsStorage)))
                    {
                        return;
                    }

                    startedSpatialAnchorStorage = true;
                }
            }

            if(activeSubsystem == null)
            {
                var activeLoader = XRGeneralSettings.Instance.Manager.activeLoader;
                activeSubsystem = activeLoader.GetLoadedSubsystem<XRAnchorSubsystem>() as MLXrAnchorSubsystem;
                if (activeSubsystem == null) return;
            }

            if (pendingStorageAnchors.Count > 0)
            {
                HashSet<ulong> readyStorageAnchors = new HashSet<ulong>();
                HashSet<ulong> failedStorageAnchors = new HashSet<ulong>();

                foreach (ulong future in pendingStorageAnchors)
                {
                    spatialAnchorsStorageNativeFunctions.PollFuture(future, out XrFutureState pollState, out XrResult futureResult);

                    if (futureResult == (XrResult)MLXrResult.FutureInvalidEXT)
                    {
                        failedStorageAnchors.Add(future);
                        Debug.LogError($"CreateSpatialAnchorsFromStorage request failed with an invalid XrFuture.");
                        continue;
                    }

                    if(pollState == XrFutureState.Ready)
                    {
                        readyStorageAnchors.Add(future);
                    }
                }

                pendingStorageAnchors.ExceptWith(failedStorageAnchors);

                foreach (ulong failedFuture in failedStorageAnchors)
                {
                    pendingStorageAnchorsData.Remove(failedFuture);
                }

                pendingStorageAnchors.ExceptWith(readyStorageAnchors);

                unsafe
                {
                    if (readyStorageAnchors.Count > 0)
                    {
                        foreach (ulong future in readyStorageAnchors)
                        {
                            int count = pendingStorageAnchorsData[future].Count;

                            NativeArray<ulong> allSpaces = new NativeArray<ulong>(count, Allocator.Temp);

                            var completionInfo = new XrCreateSpatialAnchorsCompletion
                            {
                                Type = XrSpatialAnchorsStructTypes.XrTypeCreateSpatialAnchorsCompletion,
                                SpaceCount = (uint)count,
                                Spaces = (ulong*)allSpaces.GetUnsafePtr()
                            };

                            XrResult resultCode = anchorsFeature.SpatialAnchorsNativeFunctions.XrCreateSpatialAnchorsComplete(AppSession, future, out completionInfo);
                            if (!Utils.DidXrCallSucceed(resultCode, nameof(anchorsFeature.SpatialAnchorsNativeFunctions.XrCreateSpatialAnchorsComplete)))
                            {
                                pendingStorageAnchorsData.Remove(future);
                                continue;
                            }

                            XrPose[] poses = locateSpacesNativeFunctions.LocateSpaces(AppSpace, NextPredictedDisplayTime, AppSession, allSpaces.ToArray());

                            for (int i = 0; i < count; i++)
                            {
                                Pose unityPose = XrPose.GetUnityPose(poses[i]);
                                ulong anchorId = allSpaces[i];
                                string anchorMapPositionId = pendingStorageAnchorsData[future].Uuid[i];

                                activeSubsystem.AddStorageAnchorToSubsystem(unityPose, anchorId, anchorMapPositionId);
                                OnCreationCompleteFromStorage?.Invoke(unityPose, anchorId, anchorMapPositionId, completionInfo.FutureResult);
                            }

                            pendingStorageAnchorsData.Remove(future);
                        }
                    }
                }
            }

            if (pendingPublishRequests.Count > 0)
            {
                HashSet<ulong> readyPublishAnchors = new HashSet<ulong>();
                HashSet<ulong> failedPublishAnchors = new HashSet<ulong>();

                foreach (ulong future in pendingPublishRequests)
                {
                    spatialAnchorsStorageNativeFunctions.PollFuture(future, out XrFutureState pollState, out XrResult futureResult);

                    if (futureResult == (XrResult)MLXrResult.FutureInvalidEXT)
                    {
                        failedPublishAnchors.Add(future);
                        Debug.LogError($"PublishSpatialAnchorsToStorage request failed with an invalid XrFuture.");
                        continue;
                    }

                    if (pollState == XrFutureState.Ready)
                    {
                        readyPublishAnchors.Add(future);
                    }
                }

                pendingPublishRequests.ExceptWith(failedPublishAnchors);

                foreach (ulong failedFuture in failedPublishAnchors)
                {
                    pendingStorageAnchorsPublishData.Remove(failedFuture);
                }

                pendingPublishRequests.ExceptWith(readyPublishAnchors);

                unsafe
                {
                    if(readyPublishAnchors.Count > 0)
                    {
                        foreach (ulong future in readyPublishAnchors)
                        {
                            int count = pendingStorageAnchorsPublishData[future].Count;

                            using NativeArray<XrUUID> completedUuids = new NativeArray<XrUUID>(count, Allocator.Temp);

                            var completionInfo = new XrSpatialAnchorsPublishCompletion
                            {
                                Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsPublishCompletion,
                                UuidCount = (uint)count,
                                Uuids = (XrUUID*)completedUuids.GetUnsafePtr()
                            };

                            XrResult resultCode = spatialAnchorsStorageNativeFunctions.XrPublishSpatialAnchorsComplete(storageHandle, future, out completionInfo);
                            if (!Utils.DidXrCallSucceed(resultCode, nameof(spatialAnchorsStorageNativeFunctions.XrPublishSpatialAnchorsComplete)))
                            {
                                pendingStorageAnchorsPublishData.Remove(future);
                                continue;
                            }

                            if (completedUuids.Length != pendingStorageAnchorsPublishData[future].Count)
                            {
                                Debug.LogError($"PublishSpatialAnchorsToStorage completed with an Unexpected number of results.");
                            }

                            for (int i = 0; i < completedUuids.Length; i++)
                            {
                                ulong anchorId = pendingStorageAnchorsPublishData[future].AnchorIds[i];
                                string anchorMapPositionId = completedUuids[i].ToString();
                                activeSubsystem.PublishLocalAnchor(anchorId, anchorMapPositionId);
                                OnPublishComplete?.Invoke(anchorId, anchorMapPositionId);
                            }

                            pendingStorageAnchorsPublishData.Remove(future);
                        }
                    }
                }
            }

            if(pendingQueries.Count > 0)
            {
                HashSet<ulong> readyQueryAnchors = new HashSet<ulong>();
                HashSet<ulong> failedQueryAnchors = new HashSet<ulong>();

                foreach (ulong future in pendingQueries)
                {
                    spatialAnchorsStorageNativeFunctions.PollFuture(future, out XrFutureState pollState, out XrResult futureResult);

                    if (futureResult == (XrResult)MLXrResult.FutureInvalidEXT)
                    {
                        failedQueryAnchors.Add(future);
                        Debug.LogError($"QueryStoredSpatialAnchors request failed with an invalid XrFuture.");
                        continue;
                    }

                    if (pollState == XrFutureState.Ready)
                    {
                        readyQueryAnchors.Add(future);
                    }
                }

                pendingQueries.ExceptWith(failedQueryAnchors);
                pendingQueries.ExceptWith(readyQueryAnchors);

                unsafe
                {
                    if (readyQueryAnchors.Count > 0)
                    {
                        foreach (ulong future in readyQueryAnchors)
                        {
                            var completion = new XrSpatialAnchorsQueryCompletion
                            {
                                Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsQueryCompletion,
                                UuidCapacityInput = 0,
                                Uuids = default
                            };

                            XrResult resultCode = spatialAnchorsStorageNativeFunctions.XrQuerySpatialAnchorsComplete(storageHandle, future, out completion);
                            if (!Utils.DidXrCallSucceed(resultCode, nameof(spatialAnchorsStorageNativeFunctions.XrQuerySpatialAnchorsComplete)))
                            {
                                continue;
                            }

                            uint count = completion.UuidCountOutput;
                            List<string> returnedUuids = new List<string>();

                            if (count > 0)
                            {
                                using NativeArray<XrUUID> completedUuids = new NativeArray<XrUUID>((int)count, Allocator.Temp);

                                completion.UuidCapacityInput = count;
                                completion.Uuids = (XrUUID*)completedUuids.GetUnsafePtr();

                                resultCode = spatialAnchorsStorageNativeFunctions.XrQuerySpatialAnchorsComplete(storageHandle, future, out completion);
                                if (!Utils.DidXrCallSucceed(resultCode, nameof(spatialAnchorsStorageNativeFunctions.XrQuerySpatialAnchorsComplete)))
                                {
                                    continue;
                                }

                                foreach (XrUUID uuid in completedUuids)
                                {
                                    returnedUuids.Add(uuid.ToString());
                                }

                            }

                            OnQueryComplete?.Invoke(returnedUuids);
                        }
                    }
                }
            }

            if(pendingDeleteRequests.Count > 0)
            {
                HashSet<ulong> readyDeleteAnchors = new HashSet<ulong>();
                HashSet<ulong> failedDeleteAnchors = new HashSet<ulong>();

                foreach (ulong future in pendingDeleteRequests)
                {
                    spatialAnchorsStorageNativeFunctions.PollFuture(future, out XrFutureState pollState, out XrResult futureResult);

                    if (futureResult == (XrResult)MLXrResult.FutureInvalidEXT)
                    {
                        failedDeleteAnchors.Add(future);
                        Debug.LogError($"DeleteStoredSpatialAnchor request failed with an invalid XrFuture.");
                        continue;
                    }

                    if (pollState == XrFutureState.Ready)
                    {
                        readyDeleteAnchors.Add(future);
                    }
                }

                pendingDeleteRequests.ExceptWith(failedDeleteAnchors);

                foreach (ulong failedFuture in failedDeleteAnchors)
                {
                    pendingStorageAnchorsDeleteData.Remove(failedFuture);
                }

                pendingDeleteRequests.ExceptWith(readyDeleteAnchors);

                unsafe
                {
                    if(readyDeleteAnchors.Count > 0)
                    {
                        foreach (ulong future in readyDeleteAnchors)
                        {
                            var completion = new XrSpatialAnchorsDeleteCompletion
                            {
                                Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsDeleteCompletion
                            };

                            XrResult resultCode = spatialAnchorsStorageNativeFunctions.XrDeleteSpatialAnchorsComplete(storageHandle, future, out completion);
                            if (!Utils.DidXrCallSucceed(resultCode, nameof(spatialAnchorsStorageNativeFunctions.XrDeleteSpatialAnchorsComplete)))
                            {
                                pendingStorageAnchorsDeleteData.Remove(future);
                                continue;
                            }

                            if (Utils.DidXrCallSucceed(completion.FutureResult, nameof(spatialAnchorsStorageNativeFunctions.XrDeleteSpatialAnchorsComplete)))
                            {
                                activeSubsystem.DeleteStoredAnchors(pendingStorageAnchorsDeleteData[future].Uuid);
                                OnDeletedComplete?.Invoke(pendingStorageAnchorsDeleteData[future].Uuid);
                            }

                            pendingStorageAnchorsDeleteData.Remove(future);
                        }
                    }
                }
            }

            if(pendingUpdateRequests.Count > 0)
            {
                HashSet<ulong> readyUpdateAnchors = new HashSet<ulong>();
                HashSet<ulong> failedUpdateAnchors = new HashSet<ulong>();

                foreach (ulong future in pendingUpdateRequests)
                {
                    spatialAnchorsStorageNativeFunctions.PollFuture(future, out XrFutureState pollState, out XrResult futureResult);

                    if (futureResult == (XrResult)MLXrResult.FutureInvalidEXT)
                    {
                        failedUpdateAnchors.Add(future);
                        Debug.LogError($"UpdateExpirationForStoredSpatialAnchor request failed with an invalid XrFuture.");
                        continue;
                    }

                    if (pollState == XrFutureState.Ready)
                    {
                        readyUpdateAnchors.Add(future);
                    }
                }

                pendingUpdateRequests.ExceptWith(failedUpdateAnchors);

                foreach (ulong failedFuture in failedUpdateAnchors)
                {
                    pendingStorageAnchorsUpdateData.Remove(failedFuture);
                }

                pendingUpdateRequests.ExceptWith(readyUpdateAnchors);

                unsafe
                {
                    if (readyUpdateAnchors.Count > 0)
                    {
                        foreach (ulong future in readyUpdateAnchors)
                        {
                            var completion = new XrSpatialAnchorsUpdateExpirationCompletion
                            {
                                Type = XrSpatialAnchorsStorageStructTypes.XrTypeSpatialAnchorsUpdateExpirationCompletion
                            };

                            XrResult resultCode = spatialAnchorsStorageNativeFunctions.XrUpdateSpatialAnchorsExpirationComplete(storageHandle, future, out completion);
                            if (!Utils.DidXrCallSucceed(resultCode, nameof(spatialAnchorsStorageNativeFunctions.XrUpdateSpatialAnchorsExpirationComplete)))
                            {
                                pendingStorageAnchorsUpdateData.Remove(future);
                                continue;
                            }

                            if (Utils.DidXrCallSucceed(completion.FutureResult, nameof(spatialAnchorsStorageNativeFunctions.XrUpdateSpatialAnchorsExpirationComplete)))
                            {
                                OnUpdateExpirationCompleted?.Invoke(pendingStorageAnchorsUpdateData[future].Uuid);
                            }

                            pendingStorageAnchorsUpdateData.Remove(future);
                        }
                    }
                }
            }
        }

        protected override void OnSessionDestroy(ulong xrSession)
        {
            if (startedSpatialAnchorStorage)
            {
                unsafe
                {
                    XrResult createResult = spatialAnchorsStorageNativeFunctions.XrDestroySpatialAnchorsStorage(storageHandle);
                    if (Utils.DidXrCallSucceed(createResult, nameof(spatialAnchorsStorageNativeFunctions.XrDestroySpatialAnchorsStorage)))
                    {
                        startedSpatialAnchorStorage = false;
                    }
                }
            }
            base.OnSessionDestroy(AppSession);
        }
#if UNITY_EDITOR
        protected override IEnumerable<Type> DependsOn => base.DependsOn.Append(typeof(MagicLeapSpatialAnchorsFeature));
#endif
    }
}
