// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLMarkerTracker
    {
        [Serializable]
        public struct TrackerSettings
        {
            /// <summary>
            ///     If <c> true </c>, Marker Scanner will detect markers and track QR codes.
            ///     Marker Scanner should be disabled when app is paused and enabled when app
            ///     resumes. When enabled, Marker Scanner will gain access to the camera and start
            ///     scanning markers. When disabled Marker Scanner will release the camera and
            ///     stop scanning markers. Internal state of the scanner will be maintained.
            /// </summary>
            public bool EnableMarkerScanning;

            /// <summary>
            ///     The marker types that are enabled for this scanner. Enable markers by
            ///     combining any number of <c> MarkerType </c> flags using '|' (bitwise 'or').
            /// </summary>
            public MarkerType MarkerTypes;

            /// <summary>
            ///     Aruco dictionary to use.
            /// </summary>
            public ArucoDictionaryName ArucoDicitonary;

            /// <summary>
            ///     Aruco marker size to use (in meters).
            /// </summary>
            public float ArucoMarkerSize;

            /// <summary>
            ///     The physical size of the QR code that shall be tracked (in meters). The physical size is
            ///     important to know, because once a QR code is detected we can only determine its
            ///     3D position when we know its correct size. The size of the QR code is given in
            ///     meters and represents the length of one side of the square code(without the
            ///     outer margin). Min size: As a rule of thumb the size of a QR code should be at
            ///     least a 10th of the distance you intend to scan it with a camera device. Higher
            ///     version markers with higher information density might need to be larger than
            ///     that to be detected reliably. Max size: Our camera needs to see the whole
            ///     marker at once. If it's too large, we won't detect it.
            /// </summary>
            public float QRCodeSize;

            /// <summary>
            ///     Tracker profile to be used.
            /// </summary>
            public Profile TrackerProfile;

            /// <summary>
            ///     The custom tracker profile to be used if the TrackerProfile member is set to Custom.
            /// </summary>
            public CustomProfile CustomTrackerProfile;

            internal TrackerSettings(bool enableMarkerScanning, MarkerType markerTypes, float qRCodeSize, ArucoDictionaryName arucoDicitonary, float arucoMarkerSize, Profile profile, CustomProfile customProfile)
            {
                EnableMarkerScanning = enableMarkerScanning;
                ArucoDicitonary = arucoDicitonary;
                ArucoMarkerSize = arucoMarkerSize;
                QRCodeSize = qRCodeSize;
                MarkerTypes = markerTypes;
                TrackerProfile = profile;
                CustomTrackerProfile = customProfile;
            }

            public static TrackerSettings Create(bool enableMarkerScanning = false, MarkerType markerTypes = MarkerType.None, float qRCodeSize = 0.1f, ArucoDictionaryName arucoDicitonary = ArucoDictionaryName.DICT_5X5_100, float arucoMarkerSize = 0.1f, Profile profile = Profile.Default, CustomProfile customProfile = default) =>
                new TrackerSettings(enableMarkerScanning, markerTypes, qRCodeSize, arucoDicitonary, arucoMarkerSize, profile, customProfile);

            public override string ToString() => $" {this.EnableMarkerScanning}, {this.MarkerTypes}, {this.ArucoDicitonary}, {this.QRCodeSize}, {this.ArucoMarkerSize},  {this.TrackerProfile},  {this.CustomTrackerProfile}";
            
            [Serializable]
            public struct CustomProfile
            {
                /// <summary>
                ///     A hint to the back-end the max frames per second hat should be analyzed.
                /// </summary>
                public FPSHint FPSHint;

                /// <summary>
                ///     A hint to the back-end the resolution that should be used.
                /// </summary>
                public ResolutionHint ResolutionHint;

                /// <summary>
                ///     A hint to the back-end for what cameras should be used.
                /// </summary>
                public CameraHint CameraHint;

                /// <summary>
                ///     In order to improve performance, the detectors don't always run on the full
                ///     frame.Full frame analysis is however necessary to detect new markers that
                ///     weren't detected before. Use this option to control how often the detector may
                ///     detect new markers and its impact on tracking performance.
                /// </summary>
                public FullAnalysisIntervalHint FullAnalysisIntervalHint;

                /// <summary>
                ///     This option provides control over corner refinement methods and a way to
                ///     balance detection rate, speed and pose accuracy. Always available and
                ///     applicable for Aruco and April tags.
                /// </summary>
                public CornerRefineMethod CornerRefineMethod;

                /// <summary>
                ///     Run refinement step that uses marker edges to generate even more accurate
                ///     corners, but slow down tracking rate overall by consuming more compute.
                ///     Aruco/April tags only.
                /// </summary>
                public bool UseEdgeRefinement;

                internal CustomProfile(FPSHint fpsHint, ResolutionHint resolutionHint, CameraHint cameraHint, FullAnalysisIntervalHint fullAnalysisIntervalHint, CornerRefineMethod cornerRefineMethod, bool useEdgeRefinement)
                {
                    FPSHint = fpsHint;
                    ResolutionHint = resolutionHint;
                    CameraHint = cameraHint;
                    FullAnalysisIntervalHint = fullAnalysisIntervalHint;
                    CornerRefineMethod = cornerRefineMethod;
                    UseEdgeRefinement = useEdgeRefinement;
                }

                public static CustomProfile Create(FPSHint fpsHint = FPSHint.Medium, ResolutionHint resolutionHint = ResolutionHint.Low, CameraHint cameraHint = CameraHint.RGB, FullAnalysisIntervalHint fullAnalysisIntervalHint = FullAnalysisIntervalHint.Medium, CornerRefineMethod cornerRefineMethod = CornerRefineMethod.None, bool useEdgeRefinement = false) =>
                    new CustomProfile(fpsHint, resolutionHint, cameraHint, fullAnalysisIntervalHint, cornerRefineMethod, useEdgeRefinement);

                public override string ToString() => $"{this.FPSHint}, {this.ResolutionHint}, {this.CameraHint}, {this.FullAnalysisIntervalHint}, {this.CornerRefineMethod}, {this.UseEdgeRefinement}";
            }
        }

    }
}
