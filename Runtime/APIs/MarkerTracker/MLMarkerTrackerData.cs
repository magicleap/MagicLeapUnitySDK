// %BANNER_BEGIN% 
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMarkerTrackerMarkerData.cs" company="Magic Leap">
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
using System.Text;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLMarkerTracker
    {
        /// <summary>
        ///     Represents the different marker types supported by the API
        /// </summary>
        [Flags]
        public enum MarkerType
        {
            /// <summary>
            ///     Represents no marker
            /// </summary>
            None = 0,

            /// <summary>
            ///     QR code of Model 1 or 2
            /// </summary>
            QR = 1,

            /// <summary>
            ///     Aruco marker and AprilTag
            /// </summary>
            Aruco_April = 2,

            /// <summary>
            ///     EAN-13 (experimental)
            /// </summary>
            EAN_13 = 4,

            /// <summary>
            ///     UPC-A (experimental)
            /// </summary>
            UPC_A = 8,

            /// <summary>
            ///     All supported markers
            /// </summary>
            All = 0x3FFFFFFF
        }

        /// <summary>
        /// Supported pre-defined ArUco dictionary and AprilTags.
        /// Marker Tracker supports pre-defined ArUco dictionary and AprilTags.
        /// ArUco dictionaries can be looked up and markers can be generated for them here:
        ///  http://chev.me/arucogen/
        /// </summary>
        public enum ArucoDictionaryName
        {
            /// <summary>
            /// Default value. Nothing will be detected.
            /// </summary>
            NotInitialized = -1,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            DICT_4X4_50 = 0,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            DICT_4X4_100,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            DICT_4X4_250,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            DICT_4X4_1000,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            DICT_5X5_50,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            DICT_5X5_100,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            DICT_5X5_250,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            DICT_5X5_1000,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            DICT_6X6_50,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            DICT_6X6_100,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            DICT_6X6_250,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            DICT_6X6_1000,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            DICT_7X7_50,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            DICT_7X7_100,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            DICT_7X7_250,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            DICT_7X7_1000,

            /// <summary>
            /// 5 by 5 pixel ArUco marker dictionary with 1024 IDs 
            /// </summary>
            DICT_ARUCO_ORIGINAL,

            /// <summary>
            /// 4x4 bits, minimum hamming distance between any two codes = 5, 30 codes 
            /// </summary>
            DICT_APRILTAG_16h5,

            /// <summary>
            /// 5x5 bits, minimum hamming distance between any two codes = 9, 35 codes
            /// </summary>
            DICT_APRILTAG_25h9,

            /// <summary>
            /// 6x6 bits, minimum hamming distance between any two codes = 10, 2320 codes
            /// </summary>
            DICT_APRILTAG_36h10,

            /// <summary>
            /// 6x6 bits, minimum hamming distance between any two codes = 11, 587 codes
            /// </summary>
            DICT_APRILTAG_36h11,
        }

        /// <summary>
        /// Used to hint to the back-end the max frames per second
        /// that should be analyzed.This is set in the
        /// MLMarkerTrackerSettings structure and this setting
        /// applies to all enabled trackers.
        /// CPU load is a combination of enabled detector types and
        /// FpsHint. More detectors and a higher FPS will
        /// result in a higher CPU load. High CPU load can affect the
        /// performance of your system.        
        /// </summary>
        public enum FPSHint
        {
            Low,
            Medium,
            High,
            Max
        }

        public readonly struct ArucoData
        {
            /// <summary>
            ///     Dictionary used by the Aruco Marker.
            /// </summary>
            public readonly ArucoDictionaryName Dictionary;

            /// <summary>
            ///     Type selector for the structure.
            /// </summary>
            public readonly uint Id;

#if UNITY_MAGICLEAP || UNITY_ANDROID
            internal ArucoData(NativeBindings.MLMarkerTrackerDecodedArucoData nativeArucoData)
            {
                this.Dictionary = nativeArucoData.Dictionary;
                this.Id = nativeArucoData.Id;
            }
#endif
            public override string ToString() =>
                $"Id: {Id}\nDictionaryName: {this.Dictionary}";
        }

        public readonly struct BinaryData
        {
            public readonly byte[] Data;

#if UNITY_MAGICLEAP || UNITY_ANDROID
            internal BinaryData(byte[] data)
            {
                this.Data = data;
            }
#endif
            public override string ToString() => $"Data: {this.Data}";
        }

        /// <summary>
        ///     Marker data as returned by the Magic Leap SDK
        /// </summary>
        public readonly struct MarkerData
        {
            /// <summary>
            ///     The type of marker that was detected.
            /// </summary>
            public readonly MarkerType Type;

            /// <summary>
            ///     Aruco decoded data (if any exists).
            /// </summary>
            public readonly ArucoData ArucoData;

            /// <summary>
            ///     Binary decoded data (if any exists).
            /// </summary>
            public readonly BinaryData BinaryData;

            /// <summary>
            ///     The world-space position and rotation of the marker.
            /// </summary>
            public readonly Pose Pose;

            /// <summary>
            ///     The reprojection error of this QR code detection in degrees.
            ///
            ///     The reprojection error is only useful when tracking QR codes. A high
            ///     reprojection error means that the estimated pose of the QR code doesn't match
            ///     well with the 2D detection on the processed video frame and thus the pose might
            ///     be inaccurate. The error is given in degrees, signifying by how much either
            ///     camera or QR code would have to be moved or rotated to create a perfect
            ///     reprojection. The further away your QR code is, the smaller this reprojection
            ///     error will be for the same displacement error of the code.
            /// </summary>
            public readonly float ReprojectionError;

#if UNITY_MAGICLEAP || UNITY_ANDROID
            internal MarkerData(MarkerType Type, NativeBindings.MLMarkerTrackerDecodedArucoData NativeArucoData, byte[] BinaryDataResult, Pose Pose, float ReprojectionError)
            {
                this.Type = Type;
                this.ArucoData = new ArucoData(NativeArucoData);
                this.BinaryData = new BinaryData(BinaryDataResult);
                this.Pose = Pose;
                this.ReprojectionError = ReprojectionError;
            }
#endif
            public override string ToString()
            {
                string toString;
                switch(this.Type)
                {
                    case MarkerType.Aruco_April:
                        toString = $"\nType: {Enum.GetName(typeof(MLMarkerTracker.MarkerType), this.Type)}\nReprojection Error: {this.ReprojectionError}\n Data:\n {this.ArucoData}";
                        break;

                    case MarkerType.QR:
                    case MarkerType.EAN_13:
                    case MarkerType.UPC_A:
                        toString = $"\nType: {Enum.GetName(typeof(MLMarkerTracker.MarkerType), this.Type)}\nReprojection Error: {this.ReprojectionError}\n Data Length:\n{this.BinaryData.Data.Length} bytes";
                        break;

                    default:
                        toString = "Marker has no type.";
                        break;
                }
                return toString;
            }
              

        }
    }
}
