using System;
using System.Collections.Generic;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.MarkerUnderstanding
{
    /// <summary>
    /// Used to detect data from a specified type of marker tracker based on specific settings.
    /// </summary>
    public class MarkerDetector
    {
        private ulong handle;
        private MarkerData[] data;
        private List<ulong> markers;
        private readonly Dictionary<ulong, ulong> markerSpaces;

        /// <summary>
        /// The current settings associated with the marker detector.
        /// </summary>
        public MarkerDetectorSettings Settings { get; }

        /// <summary>
        /// The current status of the readiness of the marker detector.
        /// </summary>
        public MarkerDetectorStatus Status { get; private set; }

        /// <summary>
        /// The data retrieved from the marker detector.
        /// </summary>
        /// <returns>A readonly collection of data retrieved from the marker detector.</returns>
        public IReadOnlyList<MarkerData> Data => Array.AsReadOnly(data);

        private bool activeSnapshot;

        private MarkerUnderstandingNativeFunctions NativeFunctions { get; }

        private MagicLeapMarkerUnderstandingFeature MarkerUnderstandingFeature { get; }

        /// <summary>
        /// Creates a marker detector based on specific settings and initializes the data values.
        /// </summary>
        /// <param name="settings">The marker detector settings to be associated with the marker detector to be created.</param>
        /// <param name="nativeFunctions">The native OpenXR function pointers</param>
        /// <param name="markerUnderstandingFeature">The MagicLeapMarkerUnderstandingFeature reference</param>
        internal MarkerDetector(MarkerDetectorSettings settings, MarkerUnderstandingNativeFunctions nativeFunctions, MagicLeapMarkerUnderstandingFeature markerUnderstandingFeature)
        {
            NativeFunctions = nativeFunctions;
            MarkerUnderstandingFeature = markerUnderstandingFeature;
            Settings = settings;
            var resultCode = CreateMarkerDetectorInternal();
            var success = Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrCreateMarkerDetector));

            Settings = settings;
            Status = success ? MarkerDetectorStatus.Pending : MarkerDetectorStatus.Error;

            data = Array.Empty<MarkerData>();
            markers = new List<ulong>();
            markerSpaces = new();
        }

        private unsafe XrResult CreateMarkerDetectorInternal()
        {
            var infoContainer = XrMarkerDetectorInfoContainer.Create();

            infoContainer.CreateInfo.MarkerType = (XrMarkerType)Settings.MarkerType;
            infoContainer.CreateInfo.Profile = (XrMarkerDetectorProfile)Settings.MarkerDetectorProfile;

            ref var createInfo = ref infoContainer.CreateInfo;
            ref var arucoInfo = ref infoContainer.ArucoInfo;
            ref var aprilInfo = ref infoContainer.AprilTagInfo;
            ref var customInfo = ref infoContainer.CustomInfo;
            ref var sizeInfo = ref infoContainer.SizeInfo;

            var markerType = createInfo.MarkerType;

            var chainStart = &infoContainer.CreateInfo.Next;
            var currentChain = chainStart;

            if (createInfo.Profile == XrMarkerDetectorProfile.Custom)
            {
                customInfo.ConvertCustomProfile(Settings.CustomProfileSettings);
                *currentChain = new IntPtr(&infoContainer.CustomInfo);
                currentChain = &infoContainer.CustomInfo.Next;
            }

            var shouldAppendSizeInfo = false;
            var length = 0f;

            if (markerType == XrMarkerType.Aruco)
            {
                arucoInfo.ArucoDict = (XrMarkerArucoDict)Settings.ArucoSettings.ArucoType;
                shouldAppendSizeInfo = !Settings.ArucoSettings.EstimateArucoLength;
                length = Settings.ArucoSettings.ArucoLength;
                *currentChain = new IntPtr(&infoContainer.ArucoInfo);
                currentChain = &infoContainer.ArucoInfo.Next;
            }

            if (markerType == XrMarkerType.AprilTag)
            {
                aprilInfo.AprilTagType = (XrAprilTagType)Settings.AprilTagSettings.AprilTagType;
                shouldAppendSizeInfo = !Settings.AprilTagSettings.EstimateAprilTagLength;
                length = Settings.AprilTagSettings.AprilTagLength;

                *currentChain = new IntPtr(&infoContainer.AprilTagInfo);
                currentChain = &infoContainer.AprilTagInfo.Next;
            }

            if (markerType == XrMarkerType.QR)
            {
                shouldAppendSizeInfo = !Settings.QRSettings.EstimateQRLength;
                length = Settings.QRSettings.QRLength;
            }

            if (shouldAppendSizeInfo)
            {
                sizeInfo.MarkerLength = length;
                *currentChain = new IntPtr(&infoContainer.SizeInfo);
            }

            var xrResult = NativeFunctions.XrCreateMarkerDetector(MarkerUnderstandingFeature.AppSession, in infoContainer.CreateInfo, out handle);
            Utils.DidXrCallSucceed(xrResult, nameof(NativeFunctions.XrCreateMarkerDetector));
            return xrResult;
        }

        /// <summary>
        /// Updates the status readiness of the marker detector and collects the current data values if it is in a ready state.
        /// </summary>
        internal void UpdateData()
        {
            Status = GetMarkerDetectorState();

            if (Status != MarkerDetectorStatus.Ready)
            {
                return;
            }

            activeSnapshot = false;
            data = GetMarkersData();
        }

        /// <summary>
        /// Destroys this marker detector and clears the associated data.
        /// </summary>
        internal unsafe void Destroy()
        {
            var resultCode = NativeFunctions.XrDestroyMarkerDetector(handle);
            Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrDestroyMarkerDetector));

            data = Array.Empty<MarkerData>();
            markers = new List<ulong>();
            markerSpaces.Clear();
        }

        /// <summary>
        /// Takes a snapshot of the active marker detector and gets the current status of it.
        /// </summary>
        /// <returns>The status of the marker detector, as either Pending, Ready, or Error.</returns>
        private unsafe MarkerDetectorStatus GetMarkerDetectorState()
        {
            if (!activeSnapshot)
            {
                SnapshotMarkerDetector();
            }

            var xrMarkerState = new XrMarkerDetectorState
            {
                Type = XrMarkerUnderstandingStructTypes.XrTypeMarkerDetectorState,
            };
            var resultCode = NativeFunctions.XrGetMarkerDetectorState(handle, out xrMarkerState);
            Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrGetMarkerDetectorState));
            return (MarkerDetectorStatus)xrMarkerState.Status;
        }

        /// <summary>
        /// Gets the relevant marker data of all markers associated with the active marker detector.
        /// </summary>
        /// <returns>An array representing all data retrieved from the active marker detector for all markers.</returns>
        private MarkerData[] GetMarkersData()
        {
            GetMarkers();

            var markersData = new MarkerData[markers.Count];
            for (var i = 0; i < markers.Count; ++i)
            {
                markersData[i] = GetMarkerData(markers[i]);
            }

            if (markers.Count == 0)
            {
                // clear cached data
                markerSpaces.Clear();
            }

            return markersData;
        }

        private unsafe void SnapshotMarkerDetector()
        {
            var snapshotInfo = new XrMarkerDetectorSnapshotInfo
            {
                Type = XrMarkerUnderstandingStructTypes.XrTypeMarkerDetectorSnapshotInfo,
            };
            var resultCode = NativeFunctions.XrSnapshotMarkerDetector(handle, ref snapshotInfo);
            if (Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrSnapshotMarkerDetector)))
            {
                activeSnapshot = true;
            }
        }

        private unsafe void GetMarkers()
        {
            markers.Clear();

            // call first time to get marker count
            var resultCode = NativeFunctions.XrGetMarkers(handle, 0, out var markerCount, null);

            if (!Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrGetMarkers)))
            {
                return;
            }


            // call second time to get markers
            var markersArray = new NativeArray<ulong>((int)markerCount, Allocator.Temp);
            resultCode = NativeFunctions.XrGetMarkers(handle, markerCount, out markerCount, (ulong*)markersArray.GetUnsafePtr());
            foreach (var marker in markersArray)
            {
                if (marker == 0)
                {
                    continue;
                }

                markers.Add(marker);
            }

            Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrGetMarkers));
        }

        private MarkerData GetMarkerData(ulong marker)
        {
            if (marker == 0)
            {
                return default;
            }

            MarkerData markerData;

            markerData.MarkerLength = GetMarkerLength(marker);

            if (Settings.MarkerType == MarkerType.QR || Settings.MarkerType == MarkerType.Code128 || Settings.MarkerType == MarkerType.EAN13 || Settings.MarkerType == MarkerType.UPCA)
            {
                markerData.ReprojectionErrorMeters = 0;
                markerData.MarkerNumber = null;
                markerData.MarkerString = GetMarkerString(marker);
            }
            else
            {
                markerData.ReprojectionErrorMeters = GetMarkerReprojectionError(marker);
                markerData.MarkerNumber = GetMarkerNumber(marker);
                markerData.MarkerString = null;
            }

            markerData.MarkerPose = Settings.MarkerType is MarkerType.Aruco or MarkerType.QR or MarkerType.AprilTag ? CreateMarkerSpace(marker) : null;
            return markerData;
        }

        private unsafe float GetMarkerReprojectionError(ulong marker)
        {
            var resultCode = NativeFunctions.XrGetMarkerReprojectionError(handle, marker, out var reprojectionError);
            Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrGetMarkerReprojectionError));
            return reprojectionError;
        }

        private unsafe float GetMarkerLength(ulong marker)
        {
            var resultCode = NativeFunctions.XrGetMarkerLength(handle, marker, out var meters);
            Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrGetMarkerLength));
            return meters;
        }

        private unsafe ulong GetMarkerNumber(ulong marker)
        {
            var resultCode = NativeFunctions.XrGetMarkerNumber(handle, marker, out var number);
            Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrGetMarkerNumber));
            return number;
        }

        private unsafe string GetMarkerString(ulong marker)
        {
            var resultCode = NativeFunctions.XrGetMarkerString(handle, marker, 0, out var markerLength, null);
            if (!Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrGetMarkerString)))
            {
                return "";
            }

            var byteArray = new NativeArray<byte>((int)markerLength, Allocator.Temp);
            resultCode = NativeFunctions.XrGetMarkerString(handle, marker, markerLength, out markerLength, (byte*)byteArray.GetUnsafePtr());
            if (!Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrGetMarkerString)))
            {
                return "";
            }

            var result = Encoding.UTF8.GetString((byte*)byteArray.GetUnsafePtr(), (int)markerLength).Trim('\0');
            return result;
        }

        private unsafe Pose? CreateMarkerSpace(ulong marker)
        {
            // a marker space is not created if one already exists for that marker
            if (markerSpaces.TryGetValue(marker, out var space))
            {
                var pose = NativeFunctions.GetUnityPose(space, MarkerUnderstandingFeature.AppSpace, MarkerUnderstandingFeature.NextPredictedDisplayTime);
                return pose;
            }
            
            var markerCreateSpaceInfo = new XrMarkerSpaceCreateInfo
            {
                Type = XrMarkerUnderstandingStructTypes.XrTypeMarkerSpaceCreateInfo,
                MarkerDetector = handle,
                Marker = marker,
                PoseInMarkerSpace = XrPose.GetFromPose(new Pose(Vector3.zero, Quaternion.identity))
            };

            var resultCode = NativeFunctions.XrCreateMarkerSpace(MarkerUnderstandingFeature.AppSession, in markerCreateSpaceInfo, out var xrSpace);
            if (Utils.DidXrCallSucceed(resultCode, nameof(NativeFunctions.XrCreateMarkerSpace)))
            {
                markerSpaces.Add(marker, xrSpace);
            }
            return null;
        }
    }
}
