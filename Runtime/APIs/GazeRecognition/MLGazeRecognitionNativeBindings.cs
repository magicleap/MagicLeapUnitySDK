// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System.Runtime.InteropServices;

    public partial class MLGazeRecognition
    {
        /// <summary>
        /// See ml_gaze_recognition.h for additional comments.
        /// </summary>
        internal class NativeBindings : Native.MagicLeapNativeBindings
        {
            /// <summary>
            /// Static information about the Gaze Recognition system.  Populate with MLGazeRecognitionGetStaticData().
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLGazeRecognitionStaticData
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// The maximum value for the height of the eye_left and eye_right vector.
                /// </summary>
                public readonly float EyeHeightMax;

                /// <summary>
                /// The maximum value for the width of the eye_left and eye_right vector.
                /// </summary>
                public readonly float EyeWidthMax;

                /// <summary>
                /// Location of the 3D vergence point, intersection of 3D gaze vectors.
                /// </summary>
                public readonly NativeBindings.MLCoordinateFrameUID Vergence;

                public MLGazeRecognitionStaticData(uint setVersion)
                {
                    Version = setVersion;
                    Vergence = MLCoordinateFrameUID.EmptyFrame;
                    EyeHeightMax = 0;
                    EyeWidthMax = 0;
                }
            };

            /// <summary>
            /// Information about the state of the Gaze Recognition system.  This structure must be initialized by calling
            /// MLGazeRecognitionStateInit() before use.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public readonly struct MLGazeRecognitionState
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public readonly uint Version;

                /// <summary>
                /// The timestamp accociated with all data fields in this struct, giving the headset clock.
                /// </summary>
                public readonly long Timestamp;

                /// <summary>
                /// Represents what gaze Recognition error (if any) is present.
                /// </summary>
                public readonly Error Error;

                /// <summary>
                /// Represents what known gaze Recognition behavior is present.
                /// </summary>
                public readonly Behavior Behavior;

                /// <summary>
                /// A vector for eye-in-skull position of left eye, even if right eye is closed
                /// </summary>
                public readonly MLVec2f EyeLeft;

                /// <summary>
                /// A vector for eye-in-skull position of right eye, even if left eye is closed.
                /// </summary>
                public readonly MLVec2f EyeRight;

                /// <summary>
                ///  Metadata field for onset of the current behavior, in seconds. Onset applies to all behaviors and marks the time when the current behavior began.
                /// </summary>
                public readonly float OnsetS;

                /// <summary>
                /// Metadata field for duration of the current behavior, in seconds. Duration applies to all gaze types.
                /// </summary>
                public readonly float DurationS;

                /// <summary>
                /// Metadata field for velocity of the current movement, in degrees per second.  Velocity field applies to saccades and
                /// pursuit, otherwise NaN
                /// </summary>
                public readonly float VelocityDegps;

                /// <summary>
                /// Metadata field for amplitude of the current movement, which is eye-position displacement in degrees of visual angle. Amplitude applies to saccades and pursuit, 
                /// otherwise NaN
                /// </summary>
                public readonly float AmplitudeDeg;

                /// <summary>
                /// Metadata field for direction of the current movement, in radial degrees (0-360).  Direction field applies to saccades
                /// and pursuit, otherwise NaN
                /// </summary>
                public readonly float DirectionRadial;

                public MLGazeRecognitionState(uint setVersion)
                {
                    Version = setVersion;
                    Timestamp = 0;
                    Error = Error.None;
                    Behavior = Behavior.Unknown;
                    EyeLeft.X = 0.0f;
                    EyeLeft.Y = 0.0f;
                    EyeRight.X = 0.0f;
                    EyeRight.Y = 0.0f;
                    OnsetS = 0.0f;
                    DurationS = 0.0f;
                    VelocityDegps = 0.0f;
                    AmplitudeDeg = 0.0f;
                    DirectionRadial = 0.0f;
                }
            };

            /// <summary>
            /// Create Gaze Recognition.
            /// </summary>
            /// <param name="handle">A pointer to an #MLHandle which will contain a handle to Gaze Recognition.
            /// If this operation fails, out_handle will be #ML_INVALID_HANDLE.
            /// </param>
            /// <returns>
            /// <c>MLResult_InvalidParam</c> The out_handle parameter was not valid (null).
            /// <c>MLResult_Ok</c> Gaze Recognition was successfully created.
            /// <c>MLResult_UnspecifiedFaiure</c> Gaze Recognition was not created successfully.
            /// <c>MLResult_PermissionDenied</c>
            /// </returns>
            [DllImport(MLGazeRecognitionDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLGazeRecognitionCreate(out ulong handle);

            /// <summary>
            /// Destroy Gaze Recognition.
            /// </summary>
            /// <param name="handle"> A handle to Gaze Recognition created by MLGazeRecognitionCreate()</param>
            /// <returns>
            /// <c>MLResult_Ok</c> The Gaze Recognition was successfully destroyed.
            /// <c>MLResult_UnspecifiedFailure</c> The Gaze Recognition was not successfully destroyed.
            /// <c>MLResult_InvalidParam</c> The Gaze Recognition handle was not valid.
            /// </returns>
            [DllImport(MLGazeRecognitionDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLGazeRecognitionDestroy(ulong handle);

            /// <summary>
            /// Get information about the user's gaze.
            /// </summary>
            /// <param name="handle">A handle to Gaze Recognition created byvMLGazeRecognitionCreate().</param>
            /// <param name="state">Information about the gaze.</param>
            /// <returns>
            ///	<c>MLResult_InvalidParam</c> The state parameter was not valid (null).
            /// <c>MLResult_Ok</c> gaze Recognition static was successfully received.
            /// <c>MLResult_UnspecifiedFailure</c> Failed to receive gaze Recognition state data.
            /// </returns>
            [DllImport(MLGazeRecognitionDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLGazeRecognitionGetState(ulong handle, ref MLGazeRecognitionState state);

            /// <summary>
            /// Get static information about Gaze Recognition.
            /// </summary>
            /// <param name="handle">A handle to Gaze Recognition created by MLGazeRecognitionCreate().</param>
            /// <param name="data">Target to populate the data about Gaze Recognition..</param>
            /// <returns>
            /// <c>MLResult_InvalidParam</c> The data parameter was not valid (null).
            /// <c>MLResult_Ok gaze Recognition</c> data was successfully received.
            /// <c>MLResult_UnspecifiedFailure</c> Failed to receive gaze Recognition static data.
            /// </returns>
            [DllImport(MLGazeRecognitionDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLGazeRecognitionGetStaticData(ulong handle, ref MLGazeRecognitionStaticData data);
        }
    }
}
