using MagicLeap.Android.NDK.Camera.Metadata;
using System.Collections.Generic;

namespace MagicLeap.Android
{
    /// <summary>
    /// Three Camera Devices are supported: Camera Id 0, Camera Id 1, Camera Id 3. <br/>
    /// Camera Id 0 and 1 support Camera_Only capture.<br/>
    /// Camera Id 3 supports Mixed_Reality capture, Virtual_Only capture, Camera_Only capture.<br/>
    /// Camera Id 0 and Camera Id 1 support a total of 3 streams. Streams are dynamically allocated between Camera Id 0 and Camera Id 1.<br/>
    /// Camera Id 0 and Camera Id 3 are conflicting devices, a higher priority application using Camera Id 3 will evict client using Camera Id 0.<br/>
    /// Camera Id 3(Mixed Reality Camera) only supports 1 stream.
    /// </summary>
    public static class MagicLeapCameras
    {
        private const uint VENDOR = (uint)acamera_metadata_section_start.ACAMERA_VENDOR_START;

        public static readonly Dictionary<VideoCaptureMode, string[]> DevicesForCaptureMode = new()
        {
            { VideoCaptureMode.MixedReality, new string[] { MixedRealityCamera } },
            { VideoCaptureMode.VirtualOnly, new string[] { MixedRealityCamera } },
            { VideoCaptureMode.CameraOnly, new string [] { MainCamera, CVCamera, MixedRealityCamera } }
        };

        public static readonly Dictionary<string, VideoCaptureMode[]> SupportedModesForDevice = new()
        {
            { "0", new VideoCaptureMode[] { VideoCaptureMode.CameraOnly } },
            { "1", new VideoCaptureMode[] { VideoCaptureMode.CameraOnly } },
            { "3", new VideoCaptureMode[] { VideoCaptureMode.CameraOnly, VideoCaptureMode.VirtualOnly, VideoCaptureMode.MixedReality } }
        };

        public enum VideoCaptureMode : byte
        {
            MixedReality = 0,
            VirtualOnly = 1,
            CameraOnly = 2
        }

        /// <summary>
        /// ID for the device most commonly used for regular main camera capture.
        /// </summary>
        public const string MainCamera = "0";

        /// <summary>
        /// ID for the device most commonly reserved for use with CVCamera operations.
        /// </summary>
        public const string CVCamera = "1";

        /// <summary>
        /// ID for the device that supports virtual-only or mixed reality camera capture.
        /// </summary>
        public const string MixedRealityCamera = "3";

        /// <summary>
        /// MagicLeap specific extensions to Camera Metadata.
        /// The metadata tags can be used with NDK apis to get and set camera metadata.
        /// </summary>
        public enum MetadataTags : uint
        {
            #region Tags for camera id 0 and camera id 1
            /// <summary>
            /// Can be used to force apply camera settings.
            /// Camera Id 0 and Camera Id 1 share the same camera hardware resources. It is recommended that applications try to use as much as possible
            /// the default template metadata otherwise metadata properties from one camera can affect the other.<br/>
            /// When both cameras are streaming, request metadata settings for both cameras are merged and then applied. While merging, the metadata
            /// settings from Camera Id 0 take precedence over Camera Id 1. The Force Apply mode setting can be used to override this.<br/>
            /// If Camera Id 1 Request metadata has force apply mode on, the Camera Id 1 metadata settings take precedence over Camera Id 0 properties.<br/>
            /// The tag is available in both Capture Request and Capture Result metadata.<br/>
            /// The tag has data type uint8 and can be set to 1 to force apply the camera settings.<br/>
            /// The name for this tag is "com.amd.control.forceapply" which can be used with android java camera API.<br/>
            /// </summary>
            ML_CONTROL_CAMERA_FORCEAPPLY_MODE = VENDOR + 0x1,

            /// <summary>
            /// Can be used to configure special effects.
            /// The tag is available in both Capture Request and Capture Result metadata.<br/>
            /// The tag has data type uint8 and can be set to the following values for configuring effect modes:<br/>
            /// 0 - Off, 1 - Grayscale, 2 - Negative, 3 - Sepia, 4 - Color Selection, 5 - Sharpen, 6 - Emboss, 7 - Sketch.<br/>
            /// The name for this tag is "com.amd.control.effectmode" which can be used with android java camera API.
            /// </summary>
            ML_CONTROL_CAMERA_EFFECT_MODE = VENDOR + 0x2,

            /// <summary>
            /// Can be used to limit the max exposure time selected by auto exposure algorithm.<br/>
            /// The tag can be used to limit exposure time in a high motion environment to reduce motion blur.<br/>
            /// The auto exposure algorithm uses max time of 16ms/20ms in 60Hz/50Hz env, any value less than 16ms/20ms can be used.<br/>
            /// The tag is available in both Capture Request and Capture Result metadata.<br/>
            /// The tag has data type int64 and can be used to set max exposure time in nanoseconds.<br/>
            /// The name for this tag is "com.amd.control.app_exposure_time_upper_limit" which can be used with android java camera API.
            /// </summary>
            ML_CONTROL_CAMERA_APP_EXPOSURE_UPPER_TIME_LIMIT = VENDOR + 0x4,

            /// <summary>
            /// Can be used to query no. of camera intrinsics in Capture Result Metadata.<br/>
            /// The tag can be used to query no. of intrinsics which is the no. of streams configured for camera.<br/>
            /// The tag is available in Capture Result metadata.<br/>
            /// The tag has data type int32.<br/>
            /// The name for this tag is "com.amd.control.num_intrinsics" which can be used with android java camera API.
            /// </summary>
            ML_CONTROL_CAMERA_NUM_INTRINSICS = VENDOR + 0x1B,

            /// <summary>
            /// Can be used to query the camera intrinsics in Capture Result Metadata.<br/>
            /// The tag can be used to query intrinsics for camera.<br/>
            /// The tag is available in Capture Result metadata.<br/>
            /// The tag has data type float and returns an array of floats which can be interpreted as:<br/>
            /// num_intrinsics* {width, height, focal_length_x, focal_length_y, principal_point_x, principal_point_y, field_of_view, k1, k2, p1, p2, k3 }.<br/>
            /// The name for this tag is "com.amd.control.intrinsics" which can be used with android java camera API.
            /// </summary>
            ML_CONTROL_CAMERA_INTRINSICS = VENDOR + 0x1C,
            #endregion

            #region Tags for camera id 3 (Mixed Reality Capture)
            /// <summary>
            /// Can be used to control tint for virtual and camera frames composition.<br/>
            /// The tag is available in Capture Request and Capture Result metadata.<br/>
            /// The tag has data type float and can be set between 0.0 to 1.0 to control the tint of mixed reality content.<br/>
            /// The name for this tag is "com.ml.control.tint" which can be used with android java camera API.
            /// </summary>
            ML_CONTROL_MRCAMERA_TINT = VENDOR + 0x103,

            /// <summary>
            /// Can be used to control alpha for virtual and camera frames composition.<br/>
            /// The tag is available in Capture Request and Capture Result metadata.<br/>
            /// The tag has data type float and can be set between -1.0 to 1.0 to control the opacity/transparency of virtual content.<br/>
            /// The name for this tag is "com.ml.control.alpha_bias" which can be used with android java camera API.
            /// </summary>
            ML_CONTROL_MRCAMERA_ALPHA_BIAS = VENDOR + 0x104,

            /// <summary>
            /// Can be used to set the capture mode for mixed reality capture.<br/>
            /// The tag is available in Capture Request and Capture Result metadata.<br/>
            /// The tag has data type uint8_t and can be set to 0 for Mixed_Reality capture or 1 for Virtual_Only capture.<br/>
            /// The name for this tag is "com.ml.control.capture_mode" which can be used with android java camera API.
            /// </summary>
            ML_CONTROL_MRCAMERA_CAPTURE_MODE = VENDOR + 0x105,

            /// <summary>
            /// Can be used to query no. of camera intrinsics in Capture Result Metadata.<br/>
            /// The tag can be used to query no. of intrinsics which is the no. of streams configured for camera.<br/>
            /// The tag is available in Capture Result metadata.<br/>
            /// The tag has data type int32.<br/>
            /// The name for this tag is "com.ml.control.num_intrinsics" which can be used with android java camera API.
            /// </summary>
            ML_CONTROL_MRCAMERA_NUM_INTRINSICS = VENDOR + 0x112,

            /// <summary>
            /// can be used to query the camera intrinsics in Capture Result Metadata.<br/>
            /// The tag can be used to query intrinsics for camera.<br/>
            /// The tag is available in Capture Result metadata.<br/>
            /// The tag has data type float and returns an array of floats which can be interpreted as:<br/>
            /// num_intrinsics* {width, height, focal_length_x, focal_length_y, principal_point_x, principal_point_y, field_of_view, k1, k2, p1, p2, k3}.<br/>
            /// The name for this tag is "com.ml.control.intrinsics" which can be used with android java camera API.
            /// </summary>
            ML_CONTROL_MRCAMERA_INTRINSICS = VENDOR + 0x113
            #endregion
        }
    }
}
