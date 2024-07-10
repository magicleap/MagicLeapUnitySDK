using System;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MarkerUnderstandingNativeTypes
    {
        internal enum XrMarkerUnderstandingStructTypes : ulong
        {
            XrTypeSystemMarkerUnderstandingProperties = 1000138000U,
            XrTypeMarkerDetectorCreateInfo = 1000138001U,
            XrTypeMarkerDetectorArucoInfo = 1000138002U,
            XrTypeMarkerDetectorSizeInfo = 1000138003U,
            XrTypeMarkerDetectorAprilTagInfo = 1000138004U,
            XrTypeMarkerDetectorCustomProfileInfo = 1000138005U,
            XrTypeMarkerDetectorSnapshotInfo = 1000138006U,
            XrTypeMarkerDetectorState = 1000138007U,
            XrTypeMarkerSpaceCreateInfo = 1000138008U
        }

        internal enum XrMarkerDetectorProfile
        {
            Default,
            Speed,
            Accuracy,
            SmallTargets,
            LargeFOV,
            Custom
        }

        internal enum XrMarkerType
        {
            Aruco,
            AprilTag,
            QR,
            EAN13,
            UPCA,
            CODE_128,
        }

        internal enum XrMarkerArucoDict
        {
            DICT_4X4_50= 0,
            DICT_4X4_100= 1,
            DICT_4X4_250= 2,
            DICT_4X4_1000= 3,
            DICT_5X5_50= 4,
            DICT_5X5_100= 5,
            DICT_5X5_250= 6,
            DICT_5X5_1000= 7,
            DICT_6X6_50= 8,
            DICT_6X6_100= 9,
            DICT_6X6_250= 10,
            DICT_6X6_1000= 11,
            DICT_7X7_50= 12,
            DICT_7X7_100= 13,
            DICT_7X7_250= 14,
            DICT_7X7_1000= 15,
        }
        
        internal enum XrAprilTagType
        {
            /// <summary>
            /// 4x4 bits, minimum hamming distance between any two codes = 5, 30 codes 
            /// </summary>
            Dictionary_16H5 = 0,

            /// <summary>
            /// 5x5 bits, minimum hamming distance between any two codes = 9, 35 codes
            /// </summary>
            Dictionary_25H9,

            /// <summary>
            /// 6x6 bits, minimum hamming distance between any two codes = 10, 2320 codes
            /// </summary>
            Dictionary_36H10,

            /// <summary>
            /// 6x6 bits, minimum hamming distance between any two codes = 11, 587 codes
            /// </summary>
            Dictionary_36H11
        }
        
                /// <summary>
        /// Used to hint to the back-end the max frames per second
        /// that should be analyzed. This is set in the
        /// CustomProfileSettings structure and this setting
        /// applies to all enabled trackers.
        /// 
        /// CPU load is a combination of enabled detector types,
        /// FpsHint and ResolutionHint. More detectors with a higher FPS
        /// and resolution hints will result in a higher CPU load. High CPU load can affect the
        /// performance of your system.        
        /// </summary>
        internal enum XrMarkerDetectorFPS
        {
            Low = 0,
            Medium,
            High,
            Max
        }

        /// <summary>
        /// The MarkerDetectorResolution enum values are
        /// used to hint to the back-end the resolution
        /// that should be used. This is set in the
        /// CustomProfileSettings structure and this setting
        /// currently only applies to the QR, UPC and EAN detectors.
        /// 
        /// CPU load is a combination of enabled detector types,
        /// MarkerDetectorFPS and MarkerDetectorResolution. More detectors and a higher
        /// fps and resolution hints will result in a higher CPU load.
        /// High CPU load can affect the performance of your system.
        /// </summary>
        internal enum XrMarkerDetectorResolution
        {
            Low = 0,
            Medium,
            High
        }

        /// <summary>
        /// The MarkerDetectorCamera enum values are
        /// used to hint to the camera
        /// that should be used. This is set in the
        /// CustomProfileSettings structure and this setting
        /// currently only applies to the aruco detectors.
        /// 
        /// RGB camera has higher resolution than world cameras and are better suited
        /// for use cases where the target to be tracked is small or needs to be detected
        /// from far.
        /// 
        /// World cameras make use of multiple world cameras to improve accuracy and
        /// increase the FoV for detection.
        /// from far.
        /// </summary>
        internal enum XrMarkerDetectorCamera
        {
            /// <summary>
            /// Single RGB Camera.
            /// </summary>
            RGB = 0,

            /// <summary>
            /// One or more world cameras.
            /// </summary>
            World
        }

        /// <summary>
        ///     The Aruco/April tag detector comes with several corner refinement methods.
        ///     Choosing the right corner refinement method has an impact on the accuracy and
        ///     speed trade-off that comes with each detection pipeline.
        ///     Corner refinement only applies to Aruco and April tags, not QR codes.
        /// </summary>
        internal enum XrMarkerDetectorCornerRefineMethod
        {
            /// <summary>
            /// No refinement, may have inaccurate corners.
            /// </summary>
            None = 0,

            /// <summary>
            /// Corners have subpixel coordinates. 
            /// High detection rate, very fast, reasonable accuracy.
            /// </summary>
            Subpix,

            /// <summary>
            /// High detection rate, fast, reasonable accuracy. 
            /// </summary>
            Contour,

            /// <summary>
            /// Reasonable detection rate, slowest, but very accurate.
            /// </summary>
            AprilTag
        }

        /// <summary>
        /// In order to improve performance, the detectors don't always run on the full
        /// frame. Full frame analysis is however necessary to detect new markers that
        /// weren't detected before. Use this option to control how often the detector may
        /// detect new markers and its impact on tracking performance.
        /// </summary>
        internal enum XrMarkerDetectorFullAnalysisInterval
        {

            /// <summary>
            /// Detector analyzes every frame fully.
            /// </summary>
            Max = 0,

            /// <summary>
            /// Detector analyzes frame fully very often.
            /// </summary>
            Fast,

            /// <summary>
            /// Detector analyzes frame fully a few times per second.
            /// </summary>
            Medium,

            /// <summary>
            /// Detector analyzes frame fully about every second.
            /// </summary>
            Slow
        }

        internal enum XrMarkerDetectorStatus
        {
            Pending, 
            Ready,
            Error
        }


        internal struct XrMarkerDetectorCreateInfo
        {
            internal XrMarkerUnderstandingStructTypes Type;
            internal IntPtr Next;
            internal XrMarkerDetectorProfile Profile;
            internal XrMarkerType MarkerType;
        }

        internal struct XrMarkerDetectorArucoInfo
        {
            internal XrMarkerUnderstandingStructTypes Type;
            internal IntPtr Next;
            internal XrMarkerArucoDict ArucoDict;
        }

        internal struct XrMarkerDetectorSizeInfo
        {
            internal XrMarkerUnderstandingStructTypes Type;
            internal IntPtr Next;
            internal float MarkerLength;
        }

        internal struct XrMarkerDetectorAprilTagInfo
        {
            internal XrMarkerUnderstandingStructTypes Type;
            internal IntPtr Next;
            internal XrAprilTagType AprilTagType;
        }

        internal struct XrMarkerDetectorCustomProfile
        {
            internal XrMarkerUnderstandingStructTypes Type;
            internal IntPtr Next;
            internal XrMarkerDetectorFPS FPSHint;
            internal XrMarkerDetectorResolution ResolutionHint;
            internal XrMarkerDetectorCamera CameraHint;
            internal XrMarkerDetectorCornerRefineMethod CornerRefineMethod;
            internal XrBool32 UseEdgeRefinement;
            internal XrMarkerDetectorFullAnalysisInterval FullAnalysisIntervalHint;
        }

        internal struct XrMarkerDetectorSnapshotInfo
        {
            internal XrMarkerUnderstandingStructTypes Type;
            internal IntPtr Next;
        }

        internal struct XrMarkerDetectorState
        {
            internal XrMarkerUnderstandingStructTypes Type;
            internal IntPtr Next;
            internal XrMarkerDetectorStatus Status;
        }

        internal struct XrMarkerSpaceCreateInfo
        {
            internal XrMarkerUnderstandingStructTypes Type;
            internal IntPtr Next;
            internal ulong MarkerDetector;
            internal ulong Marker;
            internal XrPose PoseInMarkerSpace;
        }

        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MarkerUnderstandingNativeExtensions")]

        internal static class MarkerUnderstandingNativeExtensions
        {
            internal static void ConvertCustomProfile(this XrMarkerDetectorCustomProfile profile,  MagicLeapMarkerUnderstandingFeature.CustomProfileSettings settings)
            {
                profile.UseEdgeRefinement = settings.UseEdgeRefinement;
                profile.CameraHint = (XrMarkerDetectorCamera)settings.CameraHint;
                profile.ResolutionHint = (XrMarkerDetectorResolution)settings.ResolutionHint;
                profile.CornerRefineMethod = (XrMarkerDetectorCornerRefineMethod)settings.CornerRefinement;
                profile.FPSHint = (XrMarkerDetectorFPS)settings.FPSHint;
                profile.FullAnalysisIntervalHint = (XrMarkerDetectorFullAnalysisInterval)settings.AnalysisInterval;
            }
        }

        internal struct XrMarkerDetectorInfoContainer
        {
            internal XrMarkerDetectorCreateInfo CreateInfo;
            internal XrMarkerDetectorArucoInfo ArucoInfo;
            internal XrMarkerDetectorSizeInfo SizeInfo;
            internal XrMarkerDetectorAprilTagInfo AprilTagInfo;
            internal XrMarkerDetectorCustomProfile CustomInfo;
            
            internal static XrMarkerDetectorInfoContainer Create()
            {
                var result = new XrMarkerDetectorInfoContainer();
                result.CreateInfo.Type = XrMarkerUnderstandingStructTypes.XrTypeMarkerDetectorCreateInfo;
                result.ArucoInfo.Type = XrMarkerUnderstandingStructTypes.XrTypeMarkerDetectorArucoInfo;
                result.SizeInfo.Type = XrMarkerUnderstandingStructTypes.XrTypeMarkerDetectorSizeInfo;
                result.AprilTagInfo.Type = XrMarkerUnderstandingStructTypes.XrTypeMarkerDetectorAprilTagInfo;
                result.CustomInfo.Type = XrMarkerUnderstandingStructTypes.XrTypeMarkerDetectorCustomProfileInfo;
                return result;
            }
        }
    }
}
