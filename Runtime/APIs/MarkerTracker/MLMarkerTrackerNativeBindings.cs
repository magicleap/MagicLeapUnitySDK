// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

// Disabling deprecated warning for the internal project
#pragma warning disable 618

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using System.Threading.Tasks;
    using UnityEngine.XR.MagicLeap.Native;


    public sealed partial class MLMarkerTracker
    {
        /// <summary>
        ///     Creates the MLMarkerTracker API instance on the native side. 
        ///     This must be called before any other native call.
        /// </summary>
        /// <param name="settings"> The initial settings for the marker scanner to use. </param>
        /// <returns> MLResult indicating the success or failure of the operation. </returns>
        /// <exception cref="System.DllNotFoundException" />
        /// <exception cref="System.EntryPointNotFoundException" />
        private static MLResult.Code MLMarkerTrackerCreate(TrackerSettings settings)
        {
            var nativeSettings = new NativeBindings.MLMarkerTrackerSettings(settings);
            if (!MLPermissions.CheckPermission(MLPermission.MarkerTracking).IsOk)
            {
                Debug.LogError($"Unable to create MLMarkerTracker because the permission {MLPermission.MarkerTracking} has not been granted.");
                return MLResult.Code.PermissionDenied;
            }
            MLResult.Code resultCode = NativeBindings.MLMarkerTrackerCreate(nativeSettings, out Instance.Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMarkerTrackerCreate));
            return resultCode;
        }

        /// <summary>
        ///     Marshal binary data struct into string.
        /// </summary>
        /// <returns> 
        ///     A string representing the binary data.
        /// </returns>
        /// 
        private static byte[] MarshalBinaryData(IntPtr binaryDataStructPtr)
        {
            var binaryDataStruct = Marshal.PtrToStructure<NativeBindings.MLMarkerTrackerDecodedBinaryData>(binaryDataStructPtr);

            // the actual binary data is at an offset of the binary data struct
            IntPtr offsetPtr = IntPtr.Add(binaryDataStructPtr, Marshal.SizeOf(binaryDataStruct));
            byte[] bytes = new byte[binaryDataStruct.Size];
            Marshal.Copy(offsetPtr, bytes, 0, bytes.Length);
            return bytes;
        }

        /// <summary>
        ///     Poll the native API for marker scanner results.
        /// </summary>
        /// <returns> 
        ///     An array of MarkerData that contains the results
        ///     that the scanner has collected since the last call to this function. This array
        ///     may be empty if there are no new results.
        /// </returns>
        private static MarkerData[] MLMarkerTrackerGetResults()
        {
            var scannerResults = new NativeBindings.MLMarkerTrackerResultArray(1);
            var resultCode = NativeBindings.MLMarkerTrackerGetResult(Instance.Handle, ref scannerResults);

            // get results from native api
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMarkerTrackerGetResult)))
            {
                var managedResults = new MarkerData[((int)scannerResults.Count)];
                for (ulong i = 0; i < scannerResults.Count.ToUInt64(); i++)
                {
                    // marshal native array into native structs
                    long address = scannerResults.Detections.ToInt64() + (Marshal.SizeOf<IntPtr>() * (int)i);
                    NativeBindings.MLMarkerTrackerResult detectedResult = Marshal.PtrToStructure<NativeBindings.MLMarkerTrackerResult>(Marshal.ReadIntPtr(new IntPtr(address)));
                    Pose pose = Pose.identity;
                    if (detectedResult.IsValidPose)
                    {
                        resultCode = MagicLeapXrProviderNativeBindings.GetUnityPose(detectedResult.CoordinateFrameUID, out pose);
                        if (MLResult.IsOK(resultCode))
                        {
                            // Update marker pose to be rotated 180 degrees on it's z axis to accommodate  for how the marker face is interpreted by the capi.
                            // Without this update the marker positions will be upside down.
                            pose = new Pose(pose.position, pose.rotation * Quaternion.AngleAxis(180, Vector3.forward));
                        }
                        else
                            Debug.LogError($"Marker Scanner could not get pose data for coordinate frame id '{detectedResult.CoordinateFrameUID}'");
                    }

                    var decodedDataType = Marshal.PtrToStructure<NativeBindings.MLMarkerTrackerDecodedTypedData>(detectedResult.DecodedData.Data);
                    NativeBindings.MLMarkerTrackerDecodedArucoData arucoData = default;
                    byte[] binaryData = null;
                    var markerType = MarkerType.None;
                    switch (decodedDataType.Type)
                    {
                        case NativeBindings.DecodedDataType.Aruco:
                            arucoData = Marshal.PtrToStructure<NativeBindings.MLMarkerTrackerDecodedArucoData>(detectedResult.DecodedData.Data);
                            markerType = MarkerType.Aruco_April;
                            break;
                        case NativeBindings.DecodedDataType.QR:
                            binaryData = MarshalBinaryData(detectedResult.DecodedData.Data);
                            markerType = MarkerType.QR;
                            break;
                        case NativeBindings.DecodedDataType.EAN_13:
                            binaryData = MarshalBinaryData(detectedResult.DecodedData.Data);
                            markerType = MarkerType.EAN_13;
                            break;
                        case NativeBindings.DecodedDataType.UPC_A:
                            binaryData = MarshalBinaryData(detectedResult.DecodedData.Data);
                            markerType = MarkerType.UPC_A;
                            break;
                    }

                    managedResults[i] =

                        new MarkerData
                        (
                            markerType,
                            arucoData,
                            binaryData,
                            pose,
                            detectedResult.ReprojectionError
                        );

                }
                if (scannerResults.Count.ToUInt64() > 0)
                {
                    // release native memory so results can be polled again
                    if (MLResult.DidNativeCallSucceed(NativeBindings.MLMarkerTrackerReleaseResult(ref scannerResults), nameof(NativeBindings.MLMarkerTrackerReleaseResult)))
                        return managedResults;
                    else
                    {
                        MLPluginLog.Error($"MLMarkerTracker.NativeBindings.MLMarkerTrackerReleaseResult failed when trying to release the results' memory. Reason: {MLResult.CodeToString(resultCode)}");
                        return managedResults;
                    }
                }
                else
                {
                    return managedResults;
                }

            }
            else
            {
                MLPluginLog.Error($"MLMarkerTracker.MLMarkerTrackerGetResult failed to obtain a result. Reason: {resultCode}");
                return new MarkerData[0];
            }
        }

        private static Task<MLResult> MLMarkerTrackerSettingsUpdate(TrackerSettings settings)
        {
            var handle = Instance.Handle;
            var nativeSettings = new NativeBindings.MLMarkerTrackerSettings(settings);
            var resultCode = NativeBindings.MLMarkerTrackerUpdateSettings(handle, in nativeSettings);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMarkerTrackerUpdateSettings));
            return MLResult.Create(resultCode);
        }

        internal class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            ///     Data type of the decoded marker data.
            /// </summary>
            public enum DecodedDataType
            {
                None,
                /// <summary>
                /// This covers Aruco and AprilTag
                /// </summary>
                Aruco,
                QR,
                EAN_13,
                UPC_A
            }

            /// <summary>
            ///     Create a Marker Scanner. Requires CameraCapture, LowLatencyLightwear priveledges.
            /// </summary>
            /// <param name="settings">
            ///     List of settings of type <c> MLMarkerTrackerSettings </c> that configure the scanner.
            /// </param>
            /// <param name="handle">
            ///     A pointer to an <c> MLHandle </c> to the newly created Marker Scanner. If this
            ///     operation fails, handle will be <c> ML_INVALID_HANDLE </c>.
            /// </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c>: Failed to create Marker Scanner due to invalid
            ///     out_handle. <c> MLResult_Ok Successfully </c>: created Marker Scanner. <c>
            ///     MLResult_PermissionDenied Failed </c>: to create scanner due to lack of
            ///     permission(s). <c> MLResult_UnspecifiedFailure </c>: Failed to create the Marker
            ///     Scanner due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMarkerTrackerCreate(in MLMarkerTrackerSettings settings, out ulong handle);

            /// <summary>
            ///     Destroy a Marker Scanner. Requires CameraCapture, LowLatencyLightwear priveleges.
            /// </summary>
            /// <param name="scannerHandle"> MLHandle to the Marker Scanner created by MLMarkerTrackerCreate(). </param>
            /// <returns>
            ///     <c> MLResult_Ok </c>: Successfully destroyed the Marker Scanner.\n <c>
            ///     MLResult_UnspecifiedFailure </c>: Failed to destroy the scanner due to an
            ///     internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMarkerTrackerDestroy(ulong scannerHandle);

            /// <summary>
            ///     brief Get the results for Marker Scanning.= This function can be used to poll
            ///     results from the scanner. This will allocate memory for the results array that
            ///     will have to be freed later.
            /// </summary>
            /// <param name="scanner_handle">
            ///     <c> MLHandle </c> to the Marker Scanner created by MLMarkerTrackerCreate().
            /// </param>
            /// <param name="data">
            ///     out_data Pointer to an array of pointers to MLMarkerTrackerResult. The content
            ///     will be freed by the MLMarkerTrackerReleaseResult.
            /// </param>
            /// <returns>
            ///     MLResult_InvalidParam Failed to return detection data due to invalid out_data.
            /// </returns>
            /// <returns> MLResult_Ok Successfully fetched and returned all detections. </returns>
            /// \retval MLResult_UnspecifiedFailure Failed to return detections due to an internal error.
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMarkerTrackerGetResult(ulong scanner_handle, ref MLMarkerTrackerResultArray data);

            /// <summary>
            ///     Release the resources for the results array.
            /// </summary>
            /// <param name="data">The list of detections to be freed.</param>
            /// <returns>
            ///     MLResult_InvaldParam Failed to free structure due to invalid data.
            ///     MLResult_Ok Successfully freed data structure.
            ///     MLResult_UnspecifiedFailure Failed to free data due to an internal error.
            /// </returns>
            ///
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMarkerTrackerReleaseResult(ref MLMarkerTrackerResultArray data);

            /// <summary>
            ///     Update the Marker Scanner with new settings. Requires CameraCapture,
            ///     LowLatencyLightwear priveledges.
            /// </summary>
            /// <param name="scanner_handle"> MLHandle to the Marker Scanner created by MLArucoScannerCreate(). </param>
            /// <param name="scanner_settings"> List of new Marker Scanner settings. </param>
            /// <returns>
            ///     <c> MLResult_InvalidParam </c>: Failed to update the settings due to invalid
            ///     scanner_settings. <c> MLResult_Ok Successfully </c>: updated the Marker Scanner
            ///     settings. <c> MLResult_PermissionDenied </c>: Failed to update the settings due
            ///     to lack of permission(s). <c> MLResult_UnspecifiedFailure </c>: Failed to update
            ///     the settings due to an internal error.
            /// </returns>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMarkerTrackerUpdateSettings(ulong scanner_handle, in MLMarkerTrackerSettings scanner_settings);

            /// <summary>
            ///     Different Marker Decoders will produce different data. Use this
            ///     structure to find what the data structure is.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLMarkerTrackerDecodedTypedData
            {
                /// <summary>
                ///     Type selector for the structure.
                /// </summary>
                public readonly DecodedDataType Type;
            }

            /// <summary>
            ///      Aruco decoded data.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLMarkerTrackerDecodedArucoData
            {
                /// <summary>
                ///     Type selector for the structure.
                /// </summary>
                public readonly DecodedDataType Type;

                /// <summary>
                ///     Dictionary used by the Aruco Marker.
                /// </summary>
                public readonly ArucoDictionaryName Dictionary;

                /// <summary>
                ///     Type selector for the structure.
                /// </summary>
                public readonly uint Id;
            }

            /// <summary>
            ///      Aruco decoded data.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLMarkerTrackerDecodedBinaryData
            {
                /// <summary>
                ///     Type selector for the structure.
                /// </summary>
                public readonly DecodedDataType Type;

                /// <summary>
                ///     Binary data size.
                /// </summary>
                public readonly uint Size;
            }

            /// <summary>
            ///     Represents the decoded data encoded in the marker. Markers can encode binary
            ///     data, strings, URLs and more. This struct represents the decoded data read from
            ///     a marker.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLMarkerTrackerDecodedData
            {
                /// <summary>
                ///      Data field contents depends on the selected detector.
                ///      The Data's Type field indicates which structure this actually contains.
                /// </summary>
                public readonly IntPtr Data;

                /// <summary>
                ///     Length of the decoded data.
                /// </summary>
                public readonly uint Size;

                public override string ToString() => $"DataSize: {Size}"; // -1 is for null terminated C strings
            }

            /// <summary>
            ///     A list of these results will be returned by the Marker Scanner, after
            ///     processing a video frame succesfully.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLMarkerTrackerResult
            {
                /// <summary>
                ///     The data which was encoded in the marker.
                /// </summary>
                public readonly MLMarkerTrackerDecodedData DecodedData;

                /// <summary>
                ///     The type of marker that was detected.
                /// </summary>
                public readonly MarkerType Type;

                /// <summary>
                ///     This indicates if coord_frame_marker holds a valid pose.
                ///     If false do not use the CoordinateFrameUID.
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public readonly bool IsValidPose;

                /// <summary>
                ///     MLCoordinateFrameUID of the QR code. This FrameUID is only useful if the
                ///     marker is of type #MLMarkerTypeQR This should be passed to the
                ///     MLSnapshotGetTransform() function to get the 6 DOF pose of the QR code. Any
                ///     marker that isn't a QR code will have an invalid FrameUID here.
                /// </summary>
                public readonly MLCoordinateFrameUID CoordinateFrameUID;

                /// <summary>
                ///     The reprojection error of this QR code detection in degrees.
                ///
                ///     The reprojection error is only useful when tracking QR codes. A high
                ///     reprojection error means that the estimated pose of the QR code doesn't
                ///     match well with the 2D detection on the processed video frame and thus the
                ///     pose might be inaccurate. The error is given in degrees, signifying by how
                ///     much either camera or QR code would have to be moved or rotated to create a
                ///     perfect reprojection. The further away your QR code is, the smaller this
                ///     reprojection error will be for the same displacement error of the code.
                /// </summary>
                public readonly float ReprojectionError;

                public override string ToString() => $"{DecodedData}\nType: {Enum.GetName(typeof(MarkerType), Type)}\nCoordFrameID: {CoordinateFrameUID}\nReproj Error: {ReprojectionError}";
            }

            /// <summary>
            ///     An array of all the detection results from the marker scanning.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLMarkerTrackerResultArray
            {
                public readonly uint Version;

                /// <summary>
                ///     Pointer to an array of pointers for MLMarkerResult.
                /// </summary>
                public readonly IntPtr Detections;

                /// <summary>
                ///     Number of markers being tracked.
                /// </summary>
                public readonly UIntPtr Count;

                public MLMarkerTrackerResultArray(uint version)
                {
                    Version = version;
                    Detections = IntPtr.Zero;
                    Count = UIntPtr.Zero;
                }
            }

            /// <summary>
            ///     When creating the Marker Scanner, this list of settings needs to be passed to
            ///     configure the scanner properly.The estimated poses will only be correct if the
            ///     marker length has been set correctly.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly partial struct MLMarkerTrackerSettings
            {
                /// <summary>
                ///     Version of the struct.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                ///     If true, Marker Tracker will detect and track the enabled marker types.
                ///     Marker Tracker should be disabled when app is paused and enabled when app resumes.
                ///     When enabled, Marker Tracker will gain access to the camera and start analysing camera frames.
                ///     When disabled Marker Tracker will release the camera and stop tracking markers.
                ///     Internal state of the tracker will be maintained.
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public readonly bool EnableMarkerScanning;

                /// <summary>
                ///     The marker types that are enabled for this scanner. Enable markers by
                ///     combining any number of MLMarkerType flags using '|' (bitwise 'or').
                /// </summary>
                public readonly uint EnabledDetectorTypes;

                /// <summary>
                ///     Aruco Dictionary or April Tag name from which markers shall be tracked.
                /// </summary>
                public readonly ArucoDictionaryName ArucoDicitonary;

                /// <summary>
                ///     The physical size of the Aruco marker that shall be tracked.
                ///
                ///     The physical size is important to know, because once a Aruco marker is detected
                ///     we can only determine its 3D position when we know its correct size. The
                ///     size of the Aruco marker is given in meters and represents the length of one side
                ///     of the square marker(without the outer margin).
                ///
                ///     Min size: As a rule of thumb the size of a Aruco marker should be at least a 10th
                ///     of the distance you intend to scan it with a camera device. Higher version
                ///     markers with higher information density might need to be larger than that
                ///     to be detected reliably.
                ///
                ///     Max size: Our camera needs to see the whole marker at once. If it's too
                ///     large, we won't detect it.
                /// </summary>
                public readonly float ArucoMarkerSize;

                /// <summary>
                ///     The physical size of the QR code that shall be tracked.
                ///
                ///     The physical size is important to know, because once a QR code is detected
                ///     we can only determine its 3D position when we know its correct size. The
                ///     size of the QR code is given in meters and represents the length of one side
                ///     of the square code(without the outer margin).
                ///
                ///     Min size: As a rule of thumb the size of a QR code should be at least a 10th
                ///     of the distance you intend to scan it with a camera device. Higher version
                ///     markers with higher information density might need to be larger than that
                ///     to be detected reliably.
                ///
                ///     Max size: Our camera needs to see the whole marker at once. If it's too
                ///     large, we won't detect it.
                /// </summary>
                public readonly float QRCodeSize;

                /// <summary>
                ///     Tracker profile to be used.
                /// </summary>
                public readonly Profile TrackerProfile;

                /// <summary>
                ///     Custom tracker profile to be used if the TrackerProfile member is the Custom value (see MLMarkerTracker.Profile enum).
                /// </summary>
                public readonly MLMarkerTrackerCustomProfile CustomTrackerProfile;


                /// <summary>
                ///     Sets the native structures from the user facing properties.
                /// </summary>
                public MLMarkerTrackerSettings(TrackerSettings settings)
                {
                    this.Version = 6;
                    this.EnableMarkerScanning = settings.EnableMarkerScanning;
                    this.EnabledDetectorTypes = (uint)settings.MarkerTypes;
                    this.ArucoDicitonary = settings.ArucoDicitonary;
                    this.ArucoMarkerSize = settings.ArucoMarkerSize;
                    this.QRCodeSize = settings.QRCodeSize;
                    this.TrackerProfile = settings.TrackerProfile;
                    this.CustomTrackerProfile = new MLMarkerTrackerCustomProfile(settings.CustomTrackerProfile);
                }
            }

            /// <summary>
            ///     Marker Tracker system provides a set of standard tracking profiles (see MLMarkerTracker.Profile enum)
            ///     to configure the tracker settings. This is the structure that defines a custom tracker profile.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLMarkerTrackerCustomProfile
            {
                /// <summary>
                ///     Hint used for all detectors.
                /// </summary>
                public readonly FPSHint FPSHint;

                /// <summary>
                ///     The resolution hint for all detectors.
                /// </summary>
                public readonly ResolutionHint ResolutionHint;

                /// <summary>
                ///     The camera hint for all detectors.
                /// </summary>
                public readonly CameraHint CameraHint;

                /// <summary>
                ///     This option provides control over corner refinement methods and a way to
                ///     balance detection rate, speed and pose accuracy. Always available and
                ///     applicable for Aruco and April tags.
                /// </summary>
                public readonly CornerRefineMethod CornerRefineMethod;

                /// <summary>
                ///     Run refinement step that uses marker edges to generate even more accurate
                ///     corners, but slow down tracking rate overall by consuming more compute.
                ///     Aruco/April tags only.
                /// </summary>
                [MarshalAs(UnmanagedType.I1)]
                public readonly bool UseEdgeRefinement;

                /// <summary>
                ///     In order to improve performance, the detectors don't always run on the full
                ///     frame.Full frame analysis is however necessary to detect new markers that
                ///     weren't detected before. Use this option to control how often the detector may
                ///     detect new markers and its impact on tracking performance.
                /// </summary>
                public readonly FullAnalysisIntervalHint FullAnalysisIntervalHint;

                /// <summary>
                ///     Sets the native structures from the user facing properties.
                /// </summary>
                public MLMarkerTrackerCustomProfile(TrackerSettings.CustomProfile customProfile)
                {
                    this.FPSHint = customProfile.FPSHint;
                    this.ResolutionHint = customProfile.ResolutionHint;
                    this.CameraHint = customProfile.CameraHint;
                    this.CornerRefineMethod = customProfile.CornerRefineMethod;
                    this.UseEdgeRefinement = customProfile.UseEdgeRefinement;
                    this.FullAnalysisIntervalHint = customProfile.FullAnalysisIntervalHint;
                }
            }
        }
    }
}
