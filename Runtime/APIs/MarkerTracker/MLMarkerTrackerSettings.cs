// %BANNER_BEGIN% 
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMarkerTrackerSettings.cs" company="Magic Leap">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
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
            ///     Aruco dictionary to use.
            /// </summary>
            public readonly ArucoDictionaryName ArucoDicitonary;

            /// <summary>
            ///     Aruco marker size to use (in meters).
            /// </summary>
            public readonly float ArucoMarkerSize;

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

            public Settings(bool enableMarkerScanning, FPSHint fpsHint, MarkerType markerTypes, float qRCodeSize, ArucoDictionaryName arucoDicitonary, float arucoMarkerSize)
            {
                this.EnableMarkerScanning = enableMarkerScanning;
                this.FPSHint = fpsHint;
                this.ArucoDicitonary = arucoDicitonary;
                this.ArucoMarkerSize = arucoMarkerSize;
                this.QRCodeSize = qRCodeSize;
                this.MarkerTypes = markerTypes;

            }
            public static Settings Create(bool enableMarkerScanning = true, MarkerType markerTypes = MarkerType.All, float qRCodeSize = 0.1f, ArucoDictionaryName arucoDicitonary = ArucoDictionaryName.DICT_5X5_100, float arucoMarkerSize = 0.1f, FPSHint fpsHint = FPSHint.Medium) =>
                new Settings(enableMarkerScanning, fpsHint, markerTypes, qRCodeSize, arucoDicitonary, arucoMarkerSize);

            
            public override string ToString()
            {
                return $"{this.MarkerTypes}, {this.ArucoDicitonary}, {this.QRCodeSize}, {this.ArucoMarkerSize}, {this.EnableMarkerScanning}";
            }
        }
    }
}
