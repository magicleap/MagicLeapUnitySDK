// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLDepthCamera
    {
        private class NativeBindings : MagicLeapNativeBindings
        {
            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraSettings
            {
                /// <summary>
                /// Version of this structure
                /// </summary>
                public uint Version;

                /// <summary>
                /// The system may not be able to service all the requested streams at any
                /// given time. This parameter is treated as a hint and data will be
                /// provided for the requested streams if available.
                /// </summary>
                public uint Streams;

                /// <summary>
                /// Controls for each of the depth camera streams.
                /// Only controls for streams enabled via streams field will be read.
                /// Use MLDepthCamera.FrameType enumeration for indexing.
                /// </summary>
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = FrameTypeCount)]
                public StreamConfig[] StreamConfig;
                public static MLDepthCameraSettings Init()
                {
                    MLDepthCameraSettings settings = new MLDepthCameraSettings();

                    settings.Version = 2;
                    settings.Streams = (uint)Stream.LongRange;

                    StreamConfig[] config = new StreamConfig[2];

                    int i = (int)FrameType.LongRange;
                    config[i].Flags = (uint)CaptureFlags.DepthImage;
                    config[i].Exposure = 1600;
                    config[i].FrameRateConfig = FrameRate.FPS_5;

                    i = (int)FrameType.ShortRange;
                    config[i].Flags = (uint)CaptureFlags.DepthImage;
                    config[i].Exposure = 375;
                    config[i].FrameRateConfig = FrameRate.FPS_5;

                    settings.StreamConfig = config;

                    return settings;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraIntrinsics
            {
                public const int MaxDistortionCoefficients = 5;

                public uint Width;

                public uint Height;

                public MLVec2f FocalLength;

                public MLVec2f PrincipalPoint;

                public float FoV;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = MaxDistortionCoefficients)]
                public double[] Distortion;

                public static MLDepthCameraIntrinsics Init(Intrinsics managed)
                {
                    var intrinsics = new MLDepthCameraIntrinsics
                    {
                        Width = managed.Width,
                        Height = managed.Height,
                        FocalLength = new MLVec2f() { X = managed.FocalLength.x, Y = managed.FocalLength.y },
                        PrincipalPoint = new MLVec2f() { X = managed.PrincipalPoint.x, Y = managed.PrincipalPoint.y },
                        FoV = managed.FoV,
                        Distortion = managed.GetDistortionList().ToArray()
                    };
                    return intrinsics;
                }

                public static Intrinsics ToManaged(MLDepthCameraIntrinsics nativeIntrinsics)
                {
                    return new Intrinsics()
                    {
                        Width = nativeIntrinsics.Width,
                        Height = nativeIntrinsics.Height,
                        FocalLength = MLConvert.ToUnity(nativeIntrinsics.FocalLength),
                        PrincipalPoint = MLConvert.ToUnity(nativeIntrinsics.PrincipalPoint),
                        FoV = nativeIntrinsics.FoV,
                        Distortion = new DistortionCoefficients(nativeIntrinsics.Distortion)
                    };
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraFrameBuffer
            {
                public uint Width;
                public uint Height;
                public uint Stride;
                public uint BytesPerUnit;
                public uint Size;
                public IntPtr Data;

                public static MLDepthCameraFrameBuffer Init(FrameBuffer? managed)
                {
                    var result = new MLDepthCameraFrameBuffer();
                    if (managed != null)
                    {
                        result.Width = managed.Value.Width;
                        result.Height = managed.Value.Height;
                        result.Stride = managed.Value.Stride;
                        result.BytesPerUnit = managed.Value.BytesPerUnit;
                        result.Size = (managed.Value.Data != null) ? (uint)managed.Value.Data.Length : 0u;
                        if (managed.Value.Data != null)
                        {
                            Marshal.Copy(managed.Value.Data, 0, result.Data, managed.Value.Data.Length);
                        }
                        return result;
                    }
                    else
                    {
                        result.Data = IntPtr.Zero;
                        return result;
                    }
                }
            }

            /// <summary>
            /// Structure to encapsulate output data for each camera sensor.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraFrame
            {
                /// <summary>
                /// A 64bit integer to index the frame number associated with this frame.
                /// </summary>
                public long FrameNumber;

                /// <summary>
                /// Frame timestamp specifies the time at which the frame was captured.
                /// </summary>
                public long FrameTimestamp;

                /// <summary>
                /// Depth camera stream used for capturing this frame.
                /// </summary>
                public FrameType FrameType;

                /// <summary>
                /// Depth camera pose in the world co-ordinate system.
                /// </summary>
                public MLTransform CameraPose;

                /// <summary>
                /// Camera intrinsic parameters.
                /// </summary>
                public MLDepthCameraIntrinsics Intrinsics;

                /// <summary>
                /// Depth image.
                /// </summary>
                public IntPtr DepthImageFrameBufferPtr;

                /// <summary>
                /// Confidence score.
                /// </summary>
                public IntPtr ConfidenceBufferFrameBufferPtr;

                /// <summary>
                /// Depth flags.
                /// </summary>
                public IntPtr DepthFlagsBufferFrameBufferPtr;

                /// <summary>
                /// Ambient raw depth image.
                /// The illuminator in the sensor is modulated with a system determined frequency.
                /// This is the raw sensor data captured when the illuminator is off.
                /// </summary>
                public IntPtr AmbientRawDepthImageFrameBufferPtr;

                /// <summary>
                /// Raw depth image.
                /// The illuminator in the sensor is modulated with a system determined frequency.
                /// This is the raw sensor data captured when the illuminator is on.
                /// </summary>
                public IntPtr RawDepthImageFrameBufferPtr;

                public static MLDepthCameraFrame Init(Data managed)
                {
                    var data = new MLDepthCameraFrame();
                    data.FrameNumber = managed.FrameNumber;
                    data.FrameTimestamp = managed.FrameTimestamp;
                    data.FrameType = managed.FrameType;
                    data.CameraPose = new MLTransform()
                    {
                        Position = MLConvert.FromUnity(managed.Position),
                        Rotation = MLConvert.FromUnity(managed.Rotation)
                    };
                    data.Intrinsics = MLDepthCameraIntrinsics.Init(managed.Intrinsics);

                    var depthMapPlaneInfo = MLDepthCameraFrameBuffer.Init(managed.DepthImage);
                    Marshal.StructureToPtr(depthMapPlaneInfo, data.DepthImageFrameBufferPtr, true);

                    var confidenceMapPlaneInfo = MLDepthCameraFrameBuffer.Init(managed.ConfidenceBuffer);
                    Marshal.StructureToPtr(confidenceMapPlaneInfo, data.ConfidenceBufferFrameBufferPtr, true);

                    var depthFlagsPlaneInfo = MLDepthCameraFrameBuffer.Init(managed.DepthFlagsBuffer);
                    Marshal.StructureToPtr(depthFlagsPlaneInfo, data.DepthFlagsBufferFrameBufferPtr, true);

                    var aiFlagsPlaneInfo = MLDepthCameraFrameBuffer.Init(managed.AmbientRawDepthImage);
                    Marshal.StructureToPtr(aiFlagsPlaneInfo, data.AmbientRawDepthImageFrameBufferPtr, true);

                    var rawDepthImageInfo = MLDepthCameraFrameBuffer.Init(managed.RawDepthImage);
                    Marshal.StructureToPtr(rawDepthImageInfo, data.RawDepthImageFrameBufferPtr, true);

                    return data;
                }
            }

            /// <summary>
            /// Structure to encapsulate output data for each camera stream. 
            /// This structure must be initialized before use.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraData
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Number of camera frames populated.
                /// </summary>
                public byte FrameCount;

                /// <summary>
                /// Camera frame data. The number of frames is specified by frame_count.
                /// </summary>
                public IntPtr Frames;

                public static MLDepthCameraData Init()
                {
                    var data = new MLDepthCameraData();
                    data.Version = 3;
                    return data;
                }
            }

            /// <summary>
            /// Structure to encapsulate a possible configuration for a single stream.
            /// Can be used to understand possible values for a specific StreamConfig element in MLDepthCameraSettings.
            /// The capabilities supported by the depth camera can be queried with MLDepthCameraGetCapabilities().
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraStreamCapability
            {
                /// <summary>
                /// Stream for which this capability can be applied.
                /// </summary>
                public Stream Stream;

                /// <summary>
                /// Minimum sensor exposure in microseconds.
                /// </summary>
                public uint MinExposure;

                /// <summary>
                /// Maximum sensor exposure in microseconds.
                /// </summary>
                public uint MaxExposure;

                /// <summary>
                /// Frame rate.
                /// </summary>
                public FrameRate FrameRateCapability;
            }

            /// <summary>
            /// Structure to encapsulate a possible set of streams configuration.
            /// Such set describes a possible way of setting StreamConfig in MLDepthCameraSettings
            /// and may hold multiple MLDepthCameraStreamCapability for different streams.
            /// Whole StreamCapabilities array describe a single configuration. Using only part
            /// of it is not recommended. The capabilities supported by the depth camera can be queried
            /// with MLDepthCameraGetCapabilities().
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraCapability
            {
                /// <summary>
                /// Size of StreamCapabilities array.
                /// </summary>
                public byte Size;

                /// <summary>
                /// Array of MLDepthCameraStreamCapability elements.
                /// </summary>
                public IntPtr StreamCapabilities;
            }

            /// <summary>
            /// Structure to encapsulate a list of possible stream configurations. 
            /// User should be able to use any of the capabilities from the list. 
            /// They are grouped in usable sets of configurations - one set may contain
            /// different MLDepthCameraStreamCapability for different streams.
            /// For more information see MLDepthCameraCapability.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraCapabilityList
            {
                /// <summary>
                /// Size of Capabilities array.
                /// </summary>
                public byte Size;

                /// <summary>
                /// Array of MLDepthCameraCapability elements.
                /// </summary>
                public IntPtr Capabilities;
            }

            /// <summary>
            /// Structure to encapsulate camera capabilities filtering.
            /// The capabilities supported by the depth camera can be queried
            /// with MLDepthCameraGetCapabilities().
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLDepthCameraCapabilityFilter
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// Streams for which capabilities will be filtered.
                /// Can hold multiple Stream values.
                /// </summary>
                public uint Streams;

                public static MLDepthCameraCapabilityFilter Init()
                {
                    var filter = new MLDepthCameraCapabilityFilter();
                    filter.Version = 1;
                    return filter;
                }
            }

            /// <summary>
            /// Connect to depth camera.
            /// </summary>
            /// <param name="settings">A pointer to MLDepthCameraSettings structure.</param>
            /// <param name="handle">A pointer to camera handle to be used in later APIs.</param>
            /// <returns>
            /// <c>MLResult_InvalidParam</c>: One of the parameters is invalid. <para />
            /// <c>MLResult_LicenseError</c>: Necessary license is missing. <para />
            /// <c>MLResult_Ok</c>: Connected to camera device(s) successfully. <para />
            /// <c>MLResult_PermissionDenied</c>: Necessary permission is missing. <para />
            /// <c>MLResult_UnspecifiedFailure</c>: The operation failed with an unspecified error. <para />
            /// </returns>
            /// <remarks>
            /// apilevel: 29 <para />
            /// permissions: com.magicleap.permission.DEPTH_CAMERA (protection level: dangerous)
            /// </remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraConnect(in MLDepthCameraSettings settings, out ulong handle);

            /// <summary>
            /// Update the depth camera settings.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLDepthCameraConnect.</param>
            /// <param name="settings">Pointer to MLDepthCameraSettings.</param>
            /// <returns>
            /// <c>MLResult_InvalidParam</c>: Invalid handle. <para />
            /// <c>MLResult_Ok</c>: Settings updated successfully. <para />
            /// <c>MLResult_UnspecifiedFailure</c>: Failed due to internal error. <para />
            /// </returns>
            /// <remarks>
            /// apilevel: 29 <para />
            /// permissions: None
            /// </remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraUpdateSettings(ulong handle, in MLDepthCameraSettings settings);

            /// <summary>
            /// Query the depth camera stream capabilities.
            /// Retrieves the capabilities supported by the depth camera based on the provided filter.
            /// After each call, a corresponding call to MLDepthCameraReleaseCapabilities should be made
            /// to release allocated resources.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLDepthCameraConnect.</param>
            /// <param name="filter">Pointer to initialized MLDepthCameraCapabilityFilter structure. Used for filtering.</param>
            /// <param name="outCaps">Pointer to initialized MLDepthCameraCapabilityList structure.</param>
            /// <returns>
            /// <c>MLResult_InvalidParam</c>: One of the parameters is invalid. <para />
            /// <c>MLResult_Ok</c>: API call completed successfully. <para />
            /// </returns>
            /// <remarks>
            /// apilevel: 29 <para />
            /// permissions: None
            /// </remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraGetCapabilities(ulong handle, ref MLDepthCameraCapabilityFilter filter, out MLDepthCameraCapabilityList outCaps);

            /// <summary>
            /// Release resources allocated with MLDepthCameraGetCapabilities.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLDepthCameraConnect.</param>
            /// <param name="outCaps">Pointer to MLDepthCameraCapabilityList filled by the call to MLDepthCameraGetCapabilities.</param>
            /// <returns>
            /// <c>MLResult_InvalidParam</c>: One of the parameters is invalid. <para />
            /// <c>MLResult_Ok</c>: API call completed successfully. <para />
            /// </returns>
            /// <remarks>
            /// apilevel: 29 <para />
            /// permissions: None
            /// </remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraReleaseCapabilities(ulong handle, ref MLDepthCameraCapabilityList outCaps);

            /// <summary>
            /// Poll for Frames.
            /// Returns MLDepthCameraData with this latest data when available. The memory is
            /// owned by the system. Application should copy the data it needs to cache it
            /// and then release the memory by calling MLDepthCameraReleaseCameraData.
            /// This is a blocking call. API is not thread safe.
            /// If there are no new depth data frames within the timeoutMs duration then the
            /// API will return MLResult_Timeout.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLDepthCameraConnect.</param>
            /// <param name="timeoutMs">Timeout in milliseconds.</param>
            /// <param name="data">Depth camera data. Should be an initialized MLDepthCameraData object.</param>
            /// <returns>
            /// <c>MLResult_InvalidParam</c>: Invalid handle. <para />
            /// <c>MLResult_Ok</c>: Depth camera data fetched successfully. <para />
            /// <c>MLResult_Timeout</c>: Returned because no new frame available at this time. <para />
            /// <c>MLResult_UnspecifiedFailure</c>: Failed due to internal error. <para />
            /// </returns>
            /// <remarks>
            /// apilevel: 29 <para />
            /// permissions: None
            /// </remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraGetLatestDepthData(ulong handle, ulong timeoutMs, out MLDepthCameraData data);

            /// <summary>
            /// Releases specified MLDepthCameraData object. 
            /// This function should be called exactly once for each successfull call to
            /// MLDepthCameraGetLatestCameraData.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLDepthCameraConnect.</param>
            /// <param name="data">Pointer to a valid MLDepthCameraData object.</param>
            /// <returns>
            /// <c>MLResult_Ok</c>: Successfully released depth camera data. <para />
            /// <c>MLResult_InvalidParam</c>: data parameter was not valid (null). <para />
            /// <c>MLResult_UnspecifiedFailure</c>: Failed due to internal error. <para />
            /// </returns>
            /// <remarks>
            /// apilevel: 29 <para />
            /// permissions: None
            /// </remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraReleaseDepthData(ulong handle, ref MLDepthCameraData data);

            /// <summary>
            /// Disconnect from depth camera.
            /// This will disconnect from the depth camera.
            /// </summary>
            /// <param name="handle">Camera handle obtained from MLDepthCameraConnect.</param>
            /// <returns>
            /// <c>MLResult_InvalidParam</c>: Invalid handle. <para />
            /// <c>MLResult_Ok</c>: Disconnected camera successfully. <para />
            /// <c>MLResult_UnspecifiedFailure</c>: Failed to disconnect camera. <para />
            /// </returns>
            /// <remarks>
            /// apilevel: 29 <para />
            /// permissions: None
            /// </remarks>
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLDepthCameraDisconnect(ulong handle);
        }
    }
}
