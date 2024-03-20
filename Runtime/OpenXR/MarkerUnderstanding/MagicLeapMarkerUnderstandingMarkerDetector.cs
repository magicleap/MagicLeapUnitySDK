#if UNITY_OPENXR_1_9_0_OR_NEWER
using System;
using System.Linq;
using System.Collections.Generic;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapMarkerUnderstandingFeature
    {
        /// <summary>
        /// Used to detect data from a specified type of marker tracker based on specific settings.
        /// </summary>
        public class MarkerDetector
        {
            private ulong handle;
            private MarkerData[] data;
            private ulong[] markers;            
            private Dictionary<ulong, ulong> markerSpaces;

            /// <summary>
            /// The current settings associated with the marker detector.
            /// </summary>
            public MarkerDetectorSettings Settings { get; private set; }

            /// <summary>
            /// The current status of the readiness of the marker detector.
            /// </summary>
            public MarkerDetectorStatus Status { get; private set; }

            /// <summary>
            /// The data retrieved from the marker detector.
            /// </summary>
            /// <returns>A readonly collection of data retrieved from the marker detector.</returns>
            public IReadOnlyList<MarkerData> Data => Array.AsReadOnly<MarkerData>(data);

            private bool activeSnapshot;
            
            /// <summary>
            /// Creates a marker detector based on specific settings and initializes the data values.
            /// </summary>
            /// <param name="settings">The marker detector settings to be associated with the marker detector to be created.</param>
            internal MarkerDetector(MarkerDetectorSettings settings)
            {
                var resultCode = NativeBindings.MLCreateMarkerDetector(settings, out handle);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLCreateMarkerDetector));

                Settings = settings;
                Status = MarkerDetectorStatus.Pending;

                data = Array.Empty<MarkerData>();
                markers = Array.Empty<ulong>();
                markerSpaces = new();
            }

            /// <summary>
            /// Updates the status readiness of the marker detector and collects the current data values if it is in a ready state.
            /// </summary>
            internal void UpdateData()
            {
                Status = GetMarkerDetectorState();

                if (Status == MarkerDetectorStatus.Ready)
                {
                    activeSnapshot = false;
                    data = GetMarkersData();
                }
            }

            /// <summary>
            /// Destroys this marker detector and clears the associated data.
            /// </summary>
            internal void Destroy()
            {
                var resultCode = NativeBindings.MLDestroyMarkerDetector(handle);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLDestroyMarkerDetector));

                data = Array.Empty<MarkerData>();
                markers = Array.Empty<ulong>();
                markerSpaces.Clear();
            }

            /// <summary>
            /// Takes a snapshot of the active marker detector and gets the current status of it.
            /// </summary>
            /// <returns>The status of the marker detector, as either Pending, Ready, or Error.</returns>
            private MarkerDetectorStatus GetMarkerDetectorState()
            {
                if (!activeSnapshot)
                {
                    SnapshotMarkerDetector();
                    activeSnapshot = true;
                }
                
                var resultCode = NativeBindings.MLGetMarkerDetectorState(handle, out MarkerDetectorStatus status);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLGetMarkerDetectorState));
                return status;
            }

            /// <summary>
            /// Gets the relevant marker data of all markers associated with the active marker detector.
            /// </summary>
            /// <returns>An array representing all data retrieved from the active marker detector for all markers.</returns>
            private MarkerData[] GetMarkersData()
            {
                GetMarkers();

                MarkerData[] markersData = markers?.Select(GetMarkerData).ToArray();

                return markersData ?? Array.Empty<MarkerData>();
            }

            private void SnapshotMarkerDetector()
            {
                var resultCode = NativeBindings.MLSnapshotMarkerDetector(handle);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLSnapshotMarkerDetector));
            }

            private void GetMarkers()
            {
                // call first time to get marker count
                var resultCode = NativeBindings.MLGetMarkers(handle, out uint markerCount, null);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLGetMarkers));

                markers = new ulong[(int)markerCount];

                // call second time to get markers
                resultCode = NativeBindings.MLGetMarkers(handle, out markerCount, markers);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLGetMarkers));
            }

            private MarkerData GetMarkerData(ulong marker)
            {
                MarkerData markerData;

                markerData.MarkerLength = GetMarkerLength(marker);

                if (Settings.MarkerType == MarkerType.QR || Settings.MarkerType == MarkerType.Code128 || Settings.MarkerType == MarkerType.EAN13 || Settings.MarkerType == MarkerType.UPCA)
                {
                    markerData.ReprojectionErrorMeters = 0;
                    markerData.MarkerNumber = 0;
                    markerData.MarkerString = GetMarkerString(marker);
                }
                else
                {
                    markerData.ReprojectionErrorMeters = GetMarkerReprojectionError(marker);
                    markerData.MarkerNumber = GetMarkerNumber(marker);
                    markerData.MarkerString = null;
                }

                if (Settings.MarkerType == MarkerType.Aruco || Settings.MarkerType == MarkerType.QR || Settings.MarkerType == MarkerType.AprilTag)
                {
                    markerData.MarkerPose = CreateMarkerSpace(marker);
                }
                else
                {
                    markerData.MarkerPose = default;
                }

                return markerData;
            }

            private float GetMarkerReprojectionError(ulong marker)
            {
                var resultCode = NativeBindings.MLGetMarkerReprojectionError(handle, marker, out float reprojectionErrorMeters);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLGetMarkerReprojectionError));
                return reprojectionErrorMeters;
            }

            private float GetMarkerLength(ulong marker)
            {
                var resultCode = NativeBindings.MLGetMarkerLength(handle, marker, out float meters);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLGetMarkerLength));
                return meters;
            }

            private ulong GetMarkerNumber(ulong marker)
            {
                var resultCode = NativeBindings.MLGetMarkerNumber(handle, marker, out ulong number);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLGetMarkerNumber));
                return number;
            }

            private string GetMarkerString(ulong marker)
            {
                uint bufferSize = 100;
                char[] buffer = new char[bufferSize];

                var resultCode = NativeBindings.MLGetMarkerString(handle, marker, bufferSize, buffer);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLGetMarkerString));

                return new string(buffer);
            }

            private Pose CreateMarkerSpace(ulong marker)
            {
                Pose pose = default;

                // a marker space is not created if one already exists for that marker
                if (markerSpaces.TryGetValue(marker, out ulong space))
                {
                    MagicLeapFeature.NativeBindings.MLOpenXRGetUnityPoseForFeature(FeatureId, space, out  pose);
                    return pose;
                }
                var resultCode = NativeBindings.MLCreateMarkerSpace(handle, marker, out var xrSpace);
                Utils.DidXrCallSucceed(resultCode, nameof(NativeBindings.MLCreateMarkerSpace));

                markerSpaces.Add(marker, xrSpace);
                return pose;
            }
        }
    }
}
#endif