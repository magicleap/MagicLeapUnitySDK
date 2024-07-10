namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
    {
        /// <summary>
        /// The base class for all metadata types. <see cref="MagicLeapPixelSensorFeature.GetSensorData"/> will return an array of instances of the base class type
        /// Use the MetadataType to determine and cast to the corresponding metadata needed
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorMetaData")]
        public abstract class PixelSensorMetaData
        {
            /// <summary>
            /// The type of the metadata that this instance holds
            /// </summary>
            public abstract PixelSensorMetaDataType MetaDataType { get; }
        }
        
        /// <summary>
        /// The exposure time metadata
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorExposureTime")]
        public class PixelSensorExposureTime : PixelSensorMetaData
        {
            /// <summary>
            /// The Exposure time in milliseconds
            /// </summary>
            public float ExposureTime { get; internal set; }

            public override PixelSensorMetaDataType MetaDataType => PixelSensorMetaDataType.ExposureTime;

            public override string ToString()
            {
                return $"Metadata:: Exposure Time {ExposureTime}";
            }
        }
        
        /// <summary>
        /// The Analog gain metadata holder
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorAnalogGain")]
        public class PixelSensorAnalogGain : PixelSensorMetaData
        {
            /// <summary>
            /// Analog gain in capturing a frame. Higher gain is useful in low light conditions but may introduce noise.
            /// </summary>
            public uint AnalogGain { get; internal set; }

            public override PixelSensorMetaDataType MetaDataType => PixelSensorMetaDataType.AnalogGain;
        }
        
        /// <summary>
        /// The Digital gain metadata holder
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorDigitalGain")]
        public class PixelSensorDigitalGain : PixelSensorMetaData
        {
            /// <summary>
            /// The digital gain in capturing a frame.Higher gain is useful in low light conditions but may introduce noise
            /// </summary>
            public uint DigitalGain { get; internal set; }

            public override PixelSensorMetaDataType MetaDataType => PixelSensorMetaDataType.DigitalGain;
        }
        
        /// <summary>
        /// The Pinhole camera model intrinsics metadata
        /// <para>Specifies the camera intrinsics and distortion co-efficients for a pinhole camera model.</para>
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorPinholeIntrinsics")]
        public class PixelSensorPinholeIntrinsics : PixelSensorMetaData
        {
            /// <summary>
            /// Focal length in pixels
            /// </summary>
            public Vector2 FocalLength { get; internal set; }
            
            /// <summary>
            /// The principal point in pixels
            /// </summary>
            public Vector2 PrincipalPoint { get; internal set; }
            
            /// <summary>
            /// The horizontal (x) and vertical (y) field of view in degrees.
            /// </summary>
            public Vector2 FOV { get; internal set; }
            
            /// <summary>
            /// Distortion coefficients. These coefficients are in the following order: [k1, k2, p1, p2, k3]
            /// </summary>
            public double[] Distortion { get; internal set; } = new double[5];

            public override PixelSensorMetaDataType MetaDataType => PixelSensorMetaDataType.PinholeCameraModel;
        }

        /// <summary>
        /// The Fisheye intrinsics camera model metadata.
        /// <para>Specifies the camera matrix and distortion co-efficients for a Magic Leap’s fisheye camera model.</para>
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorFisheyeIntrinsics")]
        public class PixelSensorFisheyeIntrinsics : PixelSensorMetaData
        {
            /// <summary>
            /// The Focal length in pixels
            /// </summary>
            public Vector2 FocalLength { get; internal set; }
            
            /// <summary>
            /// The principal point in pixels
            /// </summary>
            public Vector2 PrincipalPoint { get; internal set; }
            
            /// <summary>
            /// The horizontal and vertical field of view in degrees
            /// </summary>
            public Vector2 FOV { get; internal set; }
            
            /// <summary>
            /// The tangential distortion coefficients. These coefficients are in the following order: [k1, k2, k3, k4].
            /// </summary>
            public double[] RadialDistortion { get; internal set; } = new double[5];
            
            /// <summary>
            /// The radial distortion coefficients. These coefficients are in the following order: [p1, p2]
            /// </summary>
            public double[] TangentialDistortion { get; internal set; } = new double[2];

            public override PixelSensorMetaDataType MetaDataType => PixelSensorMetaDataType.FishEyeCameraModel;
        }
        
        /// <summary>
        /// The Depth Frame Illumination metadata
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorDepthFrameIllumination")]
        public class PixelSensorDepthFrameIllumination : PixelSensorMetaData
        {
            /// <summary>
            /// The Illumination type used to capture a frame
            /// </summary>
            public PixelSensorDepthFrameIlluminationType IlluminationType { get; internal set; }
            public override PixelSensorMetaDataType MetaDataType => PixelSensorMetaDataType.DepthFrameIllumination;
        }
        
        /// <summary>
        /// The Depth Confidence Buffer metadata
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorDepthConfidenceBuffer")]
        public class PixelSensorDepthConfidenceBuffer : PixelSensorMetaData
        {
            /// <summary>
            /// The frame associated with the depth confidence data.
            /// <para>Each pixel in the frame will have an associated confidence value. See <see cref="PixelSensorFrameType"/></para>
            /// </summary>
            public PixelSensorFrame Frame;
            public override PixelSensorMetaDataType MetaDataType => PixelSensorMetaDataType.DepthConfidenceBuffer;
        }
        
        /// <summary>
        /// The Depth Flag Buffer metadata
        /// </summary>
        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.PixelSensorDepthFlagBuffer")]
        public class PixelSensorDepthFlagBuffer : PixelSensorMetaData
        {
            /// <summary>
            /// The frame associated with the depth flags
            /// <para>Each pixel holds the flag information. See <see cref="PixelSensorDepthFlags"/> for the possible values</para>
            /// </summary>
            public PixelSensorFrame Frame;
            public override PixelSensorMetaDataType MetaDataType => PixelSensorMetaDataType.DepthFlagBuffer;
        }
    }
}
