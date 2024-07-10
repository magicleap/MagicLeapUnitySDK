using UnityEngine;

namespace MagicLeap.OpenXR.Features.MarkerUnderstanding
{

        /// <summary>
        /// Represents the different tracker profiles used to optimize marker tracking in difference use cases.
        /// </summary>
        public enum MarkerDetectorProfile
        {
            /// <summary>
            /// Generic tracker profile.
            /// Tracker profile that covers standard use cases. If this does not fit the
            /// needs of the application try the other profiles.
            /// </summary>
            Default = 0,

            /// <summary>
            /// Use this profile to reduce the compute load and increase detection/tracker speed.
            /// This can result poor poses.
            /// </summary>
            Speed,

            /// <summary>
            /// Use this profile to optimize for accurate marker poses. 
            /// This can cause increased load on the compute.
            /// </summary>
            Accuracy,

            /// <summary>
            /// Use this profile to optimize for markers that are small or for larger
            /// markers that need to detected from far.
            /// </summary>
            SmallTargets,

            /// <summary>
            /// Use this profile to be able to detect markers across a larger Field Of View.
            /// Marker Tracker system will attempt to use multiple cameras to detect the markers.
            /// </summary>
            LargeFOV,

            /// <summary>
            /// Application can define a custom tracker profiler.
            /// </summary>
            Custom,
        }

        /// <summary>
        /// Represents the different marker types supported by the API
        /// </summary>
        public enum MarkerType
        {
            Aruco = 0,
            AprilTag,
            QR,
            EAN13,
            UPCA,
            Code128
        }

        /// <summary>
        /// The current status of the readiness of the marker tracker.
        /// </summary>
        public enum MarkerDetectorStatus
        {
            Pending = 0,
            Ready,
            Error
        }

        /// <summary>
        /// Supported pre-defined ArUco dictionaries.
        /// Marker Understanding supports pre-defined ArUco dictionaries.
        /// ArUco dictionaries can be looked up and markers can be generated for them here:
        ///  http://chev.me/arucogen/
        /// </summary>
        public enum ArucoType
        {
            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            Dictionary_4x4_50 = 0,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            Dictionary_4x4_100,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            Dictionary_4x4_250,

            /// <summary>
            /// 4 by 4 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            Dictionary_4x4_1000,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            Dictionary_5x5_50,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            Dictionary_5x5_100,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            Dictionary_5x5_250,

            /// <summary>
            /// 5 by 5 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            Dictionary_5x5_1000,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            Dictionary_6x6_50,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            Dictionary_6x6_100,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            Dictionary_6x6_250,

            /// <summary>
            /// 6 by 6 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            Dictionary_6x6_1000,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 50 IDs.
            /// </summary>
            Dictionary_7x7_50,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 100 IDs.
            /// </summary>
            Dictionary_7x7_100,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 250 IDs.
            /// </summary>
            Dictionary_7x7_250,

            /// <summary>
            /// 7 by 7 pixel <c>ArUco</c> marker dictionary with 1000 IDs.
            /// </summary>
            Dictionary_7x7_1000,
        }

        /// <summary>
        /// Supported pre-defined AprilTag dictionaries.
        /// Marker Understanding supports pre-defined AprilTag dictionaries.
        /// AprilTag dictionaries can be looked up and markers can be generated for them here:
        ///  http://chev.me/arucogen/
        /// </summary>
        public enum AprilTagType
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
        public enum MarkerDetectorFPS
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
        public enum MarkerDetectorResolution
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
        public enum MarkerDetectorCamera
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
        public enum MarkerDetectorCornerRefineMethod
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
        public enum MarkerDetectorFullAnalysisInterval
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

        /// <summary>
        /// The data retrieved from a marker detector.
        /// </summary>
        public struct MarkerData
        {
            /// <summary>
            /// <para>The reprojection error is only useful when tracking <see cref="MarkerType.QR"/> codes. A high
            /// reprojection error means that the estimated pose of the QR code doesn't match
            /// well with the 2D detection on the processed video frame and thus the pose might
            /// be inaccurate. The error is given in degrees, signifying by how much either
            /// camera or QR code would have to be moved or rotated to create a perfect
            /// reprojection. The further away your QR code is, the smaller this reprojection
            /// error will be for the same displacement error of the code.</para>
            /// </summary>
            public float ReprojectionErrorMeters;

            /// <summary>
            /// The estimated length of the marker in meters.
            /// </summary>
            public float MarkerLength;

            /// <summary>
            /// The number data retrieved from the marker. Only applies to <see cref="MarkerType.Aruco"/> and <see cref="MarkerType.AprilTag"/>.
            /// </summary>
            public ulong? MarkerNumber;

            /// <summary>
            /// The string data obtained from the marker. Only applies to <see cref="MarkerType.QR"/>, <see cref="MarkerType.Code128"/>, <see cref="MarkerType.EAN13"/>, and <see cref="MarkerType.UPCA"/>.
            /// </summary>
            public string MarkerString;

            /// <summary>
            /// The position and rotation data of the marker. Only applies to <see cref="MarkerType.Aruco"/>, <see cref="MarkerType.AprilTag"/>, and <see cref="MarkerType.QR"/>.
            /// </summary>
            public Pose? MarkerPose;
        }
        
        /// <summary>
        /// All of the settings associated with the marker tracker.
        /// </summary>
        public struct MarkerDetectorSettings
        {
            /// <summary>
            /// The marker type to be associated with the marker tracker.
            /// </summary>
            public MarkerType MarkerType;

            /// <summary>
            /// The type of tracker profile to be associated with the marker tracker.
            /// </summary>
            public MarkerDetectorProfile MarkerDetectorProfile;

            /// <summary>
            /// The custom settings to be applied to the marker tracker. This is only applicable when a custom type is selected for the MarkerDetectorProfile.
            /// </summary>
            public CustomProfileSettings CustomProfileSettings;

            /// <summary>
            /// The settings associated with the Aruco marker type to be applied to the marker tracker. This only applies if the MarkerType is Aruco.
            /// </summary>
            public ArucoSettings ArucoSettings;

            /// <summary>
            /// The settings associated with the AprilTag marker type to be applied to the marker tracker. This only applies if the MarkerType is AprilTag.
            /// </summary>
            public AprilTagSettings AprilTagSettings;

            /// <summary>
            /// The settings associated with the QR marker type to be applied to the marker tracker. This only applies if the MarkerType is QR.
            /// </summary>
            public QRSettings QRSettings;
        }

        /// <summary>
        /// The custom settings applied to the marker tracker when a custom profile is used.
        /// </summary>
        public struct CustomProfileSettings
        {
            /// <summary>
            /// A hint to the back-end the max frames per second hat should be analyzed.
            /// </summary>
            public MarkerDetectorFPS FPSHint;

            /// <summary>
            /// A hint to the back-end the resolution that should be used.
            /// </summary>
            public MarkerDetectorResolution ResolutionHint;

            /// <summary>
            /// A hint to the back-end for what cameras should be used.
            /// </summary>
            public MarkerDetectorCamera CameraHint;

            /// <summary>
            /// This option provides control over corner refinement methods and a way to
            /// balance detection rate, speed and pose accuracy. Always available and
            /// applicable for Aruco and April tags.
            /// </summary>
            public MarkerDetectorCornerRefineMethod CornerRefinement;

            /// <summary>
            /// Run refinement step that uses marker edges to generate even more accurate
            /// corners, but slow down tracking rate overall by consuming more compute.
            /// Aruco/April tags only.
            /// </summary>
            public bool UseEdgeRefinement;

            /// <summary>
            /// In order to improve performance, the detectors don't always run on the full
            /// frame. Full frame analysis is however necessary to detect new markers that
            /// weren't detected before. Use this option to control how often the detector may
            /// detect new markers and its impact on tracking performance.
            /// </summary>
            public MarkerDetectorFullAnalysisInterval AnalysisInterval;
        }

        /// <summary>
        /// Settings associated with the Aruco marker type.
        /// </summary>
        public struct ArucoSettings
        {
            /// <summary>
            /// Whether the length of the Aruco marker will be estimated automatically.
            /// </summary>
            public bool EstimateArucoLength;

            /// <summary>
            /// The estimated length of the Aruco marker in meters.
            /// </summary>
            public float ArucoLength;

            /// <summary>
            /// The type of pre-defined Aruco dictionary.
            /// </summary>
            public ArucoType ArucoType;
        }

        /// <summary>
        /// Settings associated with the AprilTag marker type.
        /// </summary>
        public struct AprilTagSettings
        {
            /// <summary>
            /// Whether the length of the AprilTag marker will be estimated automatically.
            /// </summary>
            public bool EstimateAprilTagLength;

            /// <summary>
            /// The estimated length of the AprilTag marker in meters.
            /// </summary>
            public float AprilTagLength;

            /// <summary>
            /// The type of pre-defined AprilTag dictionary.
            /// </summary>
            public AprilTagType AprilTagType;
        }

        /// <summary>
        /// Settings associated with the QR marker type.
        /// </summary>
        public struct QRSettings
        {
            /// <summary>
            /// Whether the length of the QR marker will be estimated automatically.
            /// </summary>
            public bool EstimateQRLength;

            /// <summary>
            /// The estimated length of the QR marker in meters.
            /// </summary>
            public float QRLength;
        }      
}
