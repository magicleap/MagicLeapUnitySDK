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
        public readonly struct Settings
        {
            /// <summary>
            ///     If <c> true </c>, Marker Scanner will detect markers and track QR codes.
            ///     Marker Scanner should be disabled when app is paused and enabled when app
            ///     resumes. When enabled, Marker Scanner will gain access to the camera and start
            ///     scanning markers. When disabled Marker Scanner will release the camera and
            ///     stop scanning markers. Internal state of the scanner will be maintained.
            /// </summary>
            public readonly bool EnableMarkerScanning;

            /// <summary>
            ///     A hint to the back-end the max frames per second hat should be analyzed.
            /// </summary>
            public readonly FPSHint FPSHint;

            /// <summary>
            ///     A hint to the back-end the resolution that should be used.
            /// </summary>
            public readonly ResolutionHint ResolutionHint;

            /// <summary>
            ///     In order to improve performance, the detectors don't always run on the full
            ///     frame.Full frame analysis is however necessary to detect new markers that
            ///     weren't detected before. Use this option to control how often the detector may
            ///     detect new markers and its impact on tracking performance.
            /// </summary>
            public readonly FullAnalysisIntervalHint FullAnalysisIntervalHint;

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
            public readonly bool UseEdgeRefinement;

            /// <summary>
            ///     Aruco dictionary to use.
            /// </summary>
            public readonly ArucoDictionaryName ArucoDicitonary;

            /// <summary>
            ///     Aruco marker size to use (in meters).
            /// </summary>
            public readonly float ArucoMarkerSize;

            /// <summary>
            ///     Determines which camera to use for aruco marker tracking.
            ///     0 uses the world cameras and 1 uses the RGB camera.
            /// </summary>
            public readonly int ArucoCamera;

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
            public readonly float QRCodeSize;

            /// <summary>
            ///     The marker types that are enabled for this scanner. Enable markers by
            ///     combining any number of <c> MarkerType </c> flags using '|' (bitwise 'or').
            /// </summary>
            public readonly MarkerType MarkerTypes;

            internal Settings(bool enableMarkerScanning, FPSHint fpsHint, ResolutionHint resolutionHint, FullAnalysisIntervalHint fullAnalysisIntervalHint, CornerRefineMethod cornerRefineMethod, bool useEdgeRefinement, MarkerType markerTypes, float qRCodeSize, ArucoDictionaryName arucoDicitonary, float arucoMarkerSize, int arucoCamera)
            {
                this.EnableMarkerScanning = enableMarkerScanning;
                this.FPSHint = fpsHint;
                this.ResolutionHint = resolutionHint;
                this.FullAnalysisIntervalHint = fullAnalysisIntervalHint;
                this.CornerRefineMethod = cornerRefineMethod;
                this.UseEdgeRefinement = useEdgeRefinement;
                this.ArucoDicitonary = arucoDicitonary;
                this.ArucoMarkerSize = arucoMarkerSize;
                this.QRCodeSize = qRCodeSize;
                this.MarkerTypes = markerTypes;
                this.ArucoCamera = arucoCamera;

            }

            public static Settings Create(bool enableMarkerScanning = true, MarkerType markerTypes = MarkerType.All, float qRCodeSize = 0.1f, ArucoDictionaryName arucoDicitonary = ArucoDictionaryName.DICT_5X5_100, float arucoMarkerSize = 0.1f, int arucoCamera = 0, FPSHint fpsHint = FPSHint.Medium, ResolutionHint resolutionHint = ResolutionHint.Low, FullAnalysisIntervalHint fullAnalysisIntervalHint = FullAnalysisIntervalHint.Medium, CornerRefineMethod cornerRefineMethod = CornerRefineMethod.None, bool useEdgeRefinement = false) =>
                new Settings(enableMarkerScanning, fpsHint, resolutionHint, fullAnalysisIntervalHint, cornerRefineMethod, useEdgeRefinement, markerTypes, qRCodeSize, arucoDicitonary, arucoMarkerSize, arucoCamera);

            
            public override string ToString() => $"{this.MarkerTypes}, {this.ArucoDicitonary}, {this.QRCodeSize}, {this.ArucoMarkerSize}, {this.EnableMarkerScanning}";

        }
    }
}
