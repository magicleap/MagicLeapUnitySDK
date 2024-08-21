// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    public partial class MLCameraBase
    {
        /// <summary>
        /// MLCameraMetadata Summary placeholder.
        /// </summary>
        public partial class Metadata
        {

            /// <summary>
            /// Gets sensor sensitivity.
            /// </summary>
            private MLResult.Code InternalGetSensorSensitivityRequestMetadata(out int OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetSensorSensitivityRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorSensitivityRequestMetadata));
                return result;
            }

            /// <summary>
            /// Sets AE lock.
            /// </summary>
            private MLResult.Code InternalSetControlAELock(ControlAELock Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlAELock(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAELock));
                return result;
            }

            /// <summary>
            /// Gets AE Max Regions.
            /// </summary>
            private MLResult.Code InternalGetControlAEMaxRegions(out int OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAEMaxRegions(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEMaxRegions));
                return result;
            }

            /// <summary>
            /// Gets AE mode.
            /// </summary>
            private MLResult.Code InternalGetControlAEModeResultMetadata(out ControlAEMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAEModeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEModeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets AE antibanding mode.
            /// </summary>
            private MLResult.Code InternalGetControlAEAntibandingModeResultMetadata(out ControlAEAntibandingMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAEAntibandingModeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEAntibandingModeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets AE compensation step.
            /// </summary>
            private MLResult.Code InternalGetControlAECompensationStep(out Rational OutData)
            {
                int sizeOfMLCameraMetadataRational = Marshal.SizeOf(typeof(Rational));
                IntPtr outDataPointer = Marshal.AllocHGlobal(sizeOfMLCameraMetadataRational);
                var result = NativeBindings.MLCameraMetadataGetControlAECompensationStep(Handle, outDataPointer);
                OutData = Marshal.PtrToStructure<Rational>(outDataPointer);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAECompensationStep));
                Marshal.FreeHGlobal(outDataPointer);

                return result;
            }

            /// <summary>
            /// Gets color correction transform.
            /// </summary>
            private MLResult.Code InternalGetColorCorrectionTransformRequestMetadata(out Rational[][] OutData)
            {
                const int outDataRowSize = 3;
                const int outDataColSize = 3;
                int sizeOfMLCameraMetadataRational = Marshal.SizeOf(typeof(NativeBindings.MLCameraMetadataRational));
                int arraySize = sizeOfMLCameraMetadataRational * outDataRowSize * outDataColSize;
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(arraySize));
                var result = NativeBindings.MLCameraMetadataGetColorCorrectionTransformRequestMetadata(Handle, ptr);
                OutData = new Rational[outDataColSize][];
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionTransformRequestMetadata));

                if (result == MLResult.Code.Ok)
                {
                    for (int i = 0; i < outDataColSize; ++i)
                    {
                        OutData[i] = new Rational[outDataRowSize];
                        for (int j = 0; j < outDataRowSize; ++j)
                        {
                            OutData[i][j] = ConvertRational(Marshal.PtrToStructure<NativeBindings.MLCameraMetadataRational>(ptr));
                            ptr += sizeOfMLCameraMetadataRational;
                        }
                    }
                }

                Marshal.FreeHGlobal(ptr);
                return result;
            }

            /// <summary>
            /// Gets sensor info sensitivity range.
            /// </summary>
            private MLResult.Code InternalGetSensorInfoSensitivityRange(out int[] OutData)
            {
                OutData = new int[2];
                var result = NativeBindings.MLCameraMetadataGetSensorInfoSensitivityRange(Handle, OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorInfoSensitivityRange));
                return result;
            }

            /// <summary>
            /// Sets Effect mode.
            /// </summary>
            private MLResult.Code InternalSetControlEffectMode(ControlEffectMode Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlEffectMode(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlEffectMode));
                return result;
            }

            /// <summary>
            /// Sets color correction aberration.
            /// </summary>
            private MLResult.Code InternalSetColorCorrectionAberrationMode(ColorCorrectionAberrationMode Data)
            {
                var result = NativeBindings.MLCameraMetadataSetColorCorrectionAberrationMode(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetColorCorrectionAberrationMode));
                return result;
            }

            /// <summary>
            /// Gets color correction aberration modes.
            /// </summary>
            private MLResult.Code InternalGetColorCorrectionAvailableAberrationModes(
                out ColorCorrectionAberrationMode[] OutData)
            {
                const int sizeOfMLCameraMetadataColorCorrectionAberrationMode = sizeof(ColorCorrectionAberrationMode);
                var result = NativeBindings.MLCameraMetadataGetColorCorrectionAvailableAberrationModes(Handle,
                    out IntPtr outDataPointer,
                    out int outCount);
                OutData = new ColorCorrectionAberrationMode[outCount];
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionAvailableAberrationModes));
                if (result == MLResult.Code.Ok)
                {
                    for (int i = 0; i < outCount; ++i)
                    {
                        OutData[i] = (ColorCorrectionAberrationMode)Marshal.ReadInt32(outDataPointer);
                        outDataPointer += sizeOfMLCameraMetadataColorCorrectionAberrationMode;
                    }
                }

                Marshal.FreeHGlobal(outDataPointer);
                return result;
            }

            /// <summary>
            /// Gets color correction aberration.
            /// </summary>
            private MLResult.Code InternalGetColorCorrectionAberrationModeRequestMetadata(
                out ColorCorrectionAberrationMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets AE lock.
            /// </summary>
            private MLResult.Code InternalGetControlAELockAvailable(out ControlAELock OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAELockAvailable(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAELockAvailable));
                return result;
            }

            /// <summary>
            /// Gets jpeg GPS coordinates.
            /// </summary>
            private MLResult.Code InternalGetJpegGPSCoordinatesRequestMetadata(out double[] OutData)
            {
                OutData = new double[3];
                var result = NativeBindings.MLCameraMetadataGetJpegGPSCoordinatesRequestMetadata(Handle, OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegGPSCoordinatesRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets Effect modes.
            /// </summary>
            private MLResult.Code InternalGetControlAvailableEffectModes(out ControlEffectMode[] OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAvailableEffectModes(Handle, out OutData, out int OutCount);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAvailableEffectModes));
                return result;
            }

            /// <summary>
            /// Gets Scene modes.
            /// </summary>
            private MLResult.Code InternalGetControlAvailableSceneModes(out ControlSceneMode[] OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAvailableSceneModes(Handle, out OutData, out int OutCount);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAvailableSceneModes));
                return result;
            }

            /// <summary>
            /// Gets AE modes.
            /// </summary>
            private MLResult.Code InternalGetControlAEAvailableModes(out ControlAEMode[] OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAEAvailableModes(Handle, out OutData, out int OutCount);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEAvailableModes));
                return result;
            }

            /// <summary>
            /// Gets AE compensation range.
            /// </summary>
            private MLResult.Code InternalGetControlAECompensationRange(out int[] OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAECompensationRange(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAECompensationRange));
                return result;
            }

            /// <summary>
            /// Gets AWB modes.
            /// </summary>
            private MLResult.Code InternalGetControlAWBAvailableModes(out ControlAWBMode[] OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAWBAvailableModes(Handle, out OutData, out int OutCount);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBAvailableModes));
                return result;
            }

            /// <summary>
            /// Gets AE regions.
            /// </summary>
            private MLResult.Code InternalGetControlAERegionsRequestMetadata(out int[][] OutData)
            {
                const int maxColSize = 3;
                const int rowSize = 5;
                const int arraySize = sizeof(int) * maxColSize * rowSize;
                IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(arraySize));
                var result = NativeBindings.MLCameraMetadataGetControlAERegionsRequestMetadata(Handle, ptr, out int OutCount);
                OutData = new int[OutCount][];
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAERegionsRequestMetadata));

                if (result == MLResult.Code.Ok)
                {
                    for (int i = 0; i < OutCount; ++i)
                    {
                        OutData[i] = new int[rowSize];
                        for (int j = 0; j < rowSize; ++j)
                        {
                            OutData[i][j] = Marshal.ReadInt32(ptr);
                            ptr += sizeof(int);
                        }
                    }
                }

                Marshal.FreeHGlobal(ptr);
                return result;
            }

            /// <summary>
            /// Gets Available Modes.
            /// </summary>
            private MLResult.Code InternalGetControlAvailableModes(out ControlMode[] OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAvailableModes(Handle, out OutData, out int OutCount);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAvailableModes));
                return result;
            }

            /// <summary>
            /// Sets color correction transform.
            /// </summary>
            private MLResult.Code InternalSetColorCorrectionTransform(Rational[][] Data)
            {
                int elementSize = Marshal.SizeOf<NativeBindings.MLCameraMetadataRational>();
                const int rowSize = 3;
                const int colSize = 3;
                int dataSize = elementSize * rowSize * colSize;
                NativeBindings.MLCameraMetadataRational[][] inputData = ConvertRational(Data);
                IntPtr ptr = Marshal.AllocHGlobal(dataSize);
                for (int i = 0; i < inputData.Length; ++i)
                {
                    for (int j = 0; j < inputData[i].Length; ++j)
                    {
                        IntPtr hptr = ptr + (i * rowSize + j) * elementSize;
                    }
                }
                var result = NativeBindings.MLCameraMetadataSetColorCorrectionTransform(Handle, ptr);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetColorCorrectionTransform));
                Marshal.FreeHGlobal(ptr);
                return result;
            }

            /// <summary>
            /// Gets AWB lock.
            /// </summary>
            private MLResult.Code InternalGetControlAWBLockAvailable(out ControlAWBLock OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAWBLockAvailable(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBLockAvailable));
                return result;
            }

            /// <summary>
            /// Gets sensor info active array sizes.
            /// </summary>
            private MLResult.Code InternalGetSensorInfoActiveArraySize(out int[] OutData)
            {
                OutData = new int[4];
                var result = NativeBindings.MLCameraMetadataGetSensorInfoActiveArraySize(Handle, OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorInfoActiveArraySize));
                return result;
            }

            /// <summary>
            /// Gets color correction mode.
            /// </summary>
            private MLResult.Code InternalGetColorCorrectionModeRequestMetadata(out ColorCorrectionMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetColorCorrectionModeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionModeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets sensor orientation degree.
            /// </summary>
            private MLResult.Code InternalGetSensorOrientation(out int OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetSensorOrientation(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorOrientation));
                return result;
            }

            /// <summary>
            /// Gets color correction gains.
            /// </summary>
            private MLResult.Code InternalGetColorCorrectionGainsRequestMetadata(out float[] OutData)
            {
                float[] outData = new float[4];
                var result = NativeBindings.MLCameraMetadataGetColorCorrectionGainsRequestMetadata(Handle, outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionGainsRequestMetadata));
                OutData = outData;
                return result;
            }

            /// <summary>
            /// Gets color correction transform.
            /// </summary>
            private MLResult.Code InternalGetColorCorrectionTransformResultMetadata(out Rational[][] OutData)
            {
                const int colSize = 3;
                const int rowSize = 3;
                int elementSize = Marshal.SizeOf(typeof(NativeBindings.MLCameraMetadataRational));
                IntPtr ptr = Marshal.AllocHGlobal(colSize * rowSize * elementSize);
                var result = NativeBindings.MLCameraMetadataGetColorCorrectionTransformResultMetadata(Handle, ptr);
                OutData = new Rational[colSize][];
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionTransformResultMetadata));

                if (result == MLResult.Code.Ok)
                {
                    for (int i = 0; i < colSize; ++i)
                    {
                        OutData[i] = new Rational[rowSize];
                        for (int j = 0; j < rowSize; ++j)
                        {
                            NativeBindings.MLCameraMetadataRational element = Marshal.PtrToStructure<NativeBindings.MLCameraMetadataRational>(ptr);
                            OutData[i][j] = ConvertRational(element);
                            ptr += elementSize;
                        }
                    }
                }

                Marshal.FreeHGlobal(ptr);
                return result;
            }

            /// <summary>
            /// Gets AE antibanding mode.
            /// </summary>
            private MLResult.Code InternalGetControlAEAntibandingModeRequestMetadata(out ControlAEAntibandingMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAEAntibandingModeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEAntibandingModeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets AE exposure compensation.
            /// </summary>
            private MLResult.Code InternalGetControlAEExposureCompensationRequestMetadata(out int OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAEExposureCompensationRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEExposureCompensationRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets AE lock.
            /// </summary>
            private MLResult.Code InternalGetControlAELockRequestMetadata(out ControlAELock OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAELockRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAELockRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets AE mode.
            /// </summary>
            private MLResult.Code InternalGetControlAEModeRequestMetadata(out ControlAEMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAEModeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEModeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets AWB lock.
            /// </summary>
            private MLResult.Code InternalGetControlAWBLockRequestMetadata(out ControlAWBLock OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAWBLockRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBLockRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets AWB mode.
            /// </summary>
            private MLResult.Code InternalGetControlAWBModeRequestMetadata(out ControlAWBMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAWBModeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBModeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets mode.
            /// </summary>
            private MLResult.Code InternalGetControlModeRequestMetadata(out ControlMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlModeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlModeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets Scene mode.
            /// </summary>
            private MLResult.Code InternalGetControlSceneModeRequestMetadata(out ControlSceneMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlSceneModeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlSceneModeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets sensor exposure time.
            /// </summary>
            private MLResult.Code InternalGetSensorExposureTimeRequestMetadata(out long OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetSensorExposureTimeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorExposureTimeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Effect mode.
            /// </summary>
            private MLResult.Code InternalGetControlEffectModeRequestMetadata(out ControlEffectMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlEffectModeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlEffectModeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets AE lock.
            /// </summary>
            private MLResult.Code InternalGetControlAELockResultMetadata(out ControlAELock OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAELockResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAELockResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets exposure time upper limit.
            /// </summary>
            private MLResult.Code InternalGetControlExposureUpperTimeLimitRequestMetadata(out long OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlExposureUpperTimeLimitRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlExposureUpperTimeLimitRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets jpeg timestamp.
            /// </summary>
            private MLResult.Code InternalGetJpegGPSTimestampRequestMetadata(out long OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetJpegGPSTimestampRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegGPSTimestampRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets jpeg thumbnail size.
            /// </summary>
            private MLResult.Code InternalGetJpegThumbnailSizeRequestMetadata(out JpegThumbnailSize OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetJpegThumbnailSizeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegThumbnailSizeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Gets jpeg quality.
            /// </summary>
            private MLResult.Code InternalGetJpegQualityRequestMetadata(out byte OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetJpegQualityRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegQualityRequestMetadata));
                return result;
            }

            /// <summary>
            /// Sets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
            /// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
            /// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
            /// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
            /// Main Camera metadata properties.
            /// </summary>
            private MLResult.Code InternalSetControlForceApplyMode(ControlForceApplyMode Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlForceApplyMode(Handle, Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlForceApplyMode));
                return result;
            }

            /// <summary>
            /// Gets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
            /// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
            /// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
            /// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
            /// Main Camera metadata properties.
            /// </summary>
            private MLResult.Code InternalGetControlForceApplyModeRequestMetadata(out ControlForceApplyMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlForceApplyModeRequestMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlForceApplyModeRequestMetadata));
                return result;
            }

            /// <summary>
            /// Sets color correction mode.
            /// </summary>
            private MLResult.Code InternalSetColorCorrectionMode(ColorCorrectionMode Data)
            {
                var result = NativeBindings.MLCameraMetadataSetColorCorrectionMode(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetColorCorrectionMode));
                return result;
            }

            /// <summary>
            /// Gets color correction gains.
            /// </summary>
            private MLResult.Code InternalGetColorCorrectionGainsResultMetadata(out float[] OutData)
            {
                float[] outData = new float[4];
                var result = NativeBindings.MLCameraMetadataGetColorCorrectionGainsResultMetadata(Handle, outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionGainsResultMetadata));
                OutData = outData;
                return result;
            }

            /// <summary>
            /// Sets color correction gains.
            /// </summary>
            private MLResult.Code InternalSetColorCorrectionGains(float[] Data)
            {
                var result = NativeBindings.MLCameraMetadataSetColorCorrectionGains(Handle, Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetColorCorrectionGains));
                return result;
            }

            /// <summary>
            /// Sets AE antiband mode.
            /// </summary>
            private MLResult.Code InternalSetControlAEAntibandingMode(ControlAEAntibandingMode Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlAEAntibandingMode(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAEAntibandingMode));
                return result;
            }

            /// <summary>
            /// Sets AE exposure compensation.
            /// </summary>
            private MLResult.Code InternalSetControlAEExposureCompensation(int Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlAEExposureCompensation(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAEExposureCompensation));
                return result;
            }

            /// <summary>
            /// Sets AE mode.  MLCameraMetadataControlAEMode_Off is not supported if camera is configured for 15FPS
            /// </summary>
            private MLResult.Code InternalSetControlAEMode(ControlAEMode Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlAEMode(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAEMode));
                return result;
            }

            /// <summary>
            /// set AE regions.
            /// </summary>
            private MLResult.Code InternalSetControlAERegions(int[][] Data)
            {
                const int colSize = 3;
                const int rowSize = 5;
                const int elementSize = sizeof(int);
                IntPtr ptr = Marshal.AllocHGlobal(elementSize * colSize * rowSize);
                for (int i = 0; i < Data.Length; ++i)
                {
                    for (int j = 0; j < rowSize; ++j)
                    {
                        Marshal.WriteInt32(ptr, (i * rowSize + j) * elementSize, Data[i][j]);
                    }
                }
                int count = Data.Length;
                var result = NativeBindings.MLCameraMetadataSetControlAERegions(Handle, ptr, count);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAERegions));

                Marshal.FreeHGlobal(ptr);
                return result;
            }

            /// <summary>
            /// Sets AWB lock.
            /// </summary>
            private MLResult.Code InternalSetControlAWBLock(ControlAWBLock Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlAWBLock(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAWBLock));
                return result;
            }

            /// <summary>
            /// Sets AWB mode.
            /// </summary>
            private MLResult.Code InternalSetControlAWBMode(ControlAWBMode Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlAWBMode(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAWBMode));
                return result;
            }

            /// <summary>
            /// Sets mode.
            /// </summary>
            private MLResult.Code InternalSetControlMode(ControlMode Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlMode(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlMode));
                return result;
            }

            /// <summary>
            /// Sets Scene mode.
            /// </summary>
            private MLResult.Code InternalSetControlSceneMode(ControlSceneMode Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlSceneMode(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlSceneMode));
                return result;
            }

            /// <summary>
            /// Gets AWB lock.
            /// </summary>
            private MLResult.Code InternalGetControlAWBLockResultMetadata(out ControlAWBLock OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAWBLockResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBLockResultMetadata));
                return result;
            }

            /// <summary>
            /// Sets sensor exposure time.
            /// </summary>
            private MLResult.Code InternalSetSensorExposureTime(long Data)
            {
                var result = NativeBindings.MLCameraMetadataSetSensorExposureTime(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetSensorExposureTime));
                return result;
            }

            /// <summary>
            /// Sets sensor sensitivity.
            /// </summary>
            private MLResult.Code InternalSetSensorSensitivity(int Data)
            {
                var result = NativeBindings.MLCameraMetadataSetSensorSensitivity(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetSensorSensitivity));
                return result;
            }

            /// <summary>
            /// Sets exposure time upper limit.
            /// </summary>
            private MLResult.Code InternalSetControlExposureUpperTimeLimit(long Data)
            {
                var result = NativeBindings.MLCameraMetadataSetControlExposureUpperTimeLimit(Handle, Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlExposureUpperTimeLimit));
                return result;
            }

            /// <summary>
            /// Sets jpeg GPS coordinates.
            /// </summary>
            private MLResult.Code InternalSetJpegGPSCoordinates(double[] Data)
            {
                var result = NativeBindings.MLCameraMetadataSetJpegGPSCoordinates(Handle, Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetJpegGPSCoordinates));
                return result;
            }

            /// <summary>
            /// Sets jpeg timestamp.
            /// </summary>
            private MLResult.Code InternalSetJpegGPSTimestamp(long Data)
            {
                var result = NativeBindings.MLCameraMetadataSetJpegGPSTimestamp(Handle, Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetJpegGPSTimestamp));
                return result;
            }

            /// <summary>
            /// Sets jpeg thumbnail size.
            /// </summary>
            private MLResult.Code InternalSetJpegThumbnailSize(JpegThumbnailSize Data)
            {
                var result = NativeBindings.MLCameraMetadataSetJpegThumbnailSize(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetJpegThumbnailSize));
                return result;
            }

            /// <summary>
            /// Sets jpeg quality.
            /// </summary>
            private MLResult.Code InternalSetJpegQuality(byte Data)
            {
                var result = NativeBindings.MLCameraMetadataSetJpegQuality(Handle, ref Data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetJpegQuality));
                return result;
            }

            /// <summary>
            /// Gets color correction.
            /// </summary>
            private MLResult.Code InternalGetColorCorrectionModeResultMetadata(out ColorCorrectionMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetColorCorrectionModeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionModeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets color correction aberration.
            /// </summary>
            private MLResult.Code InternalGetColorCorrectionAberrationModeResultMetadata(out ColorCorrectionAberrationMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetColorCorrectionAberrationModeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets AE exposure compensation.
            /// </summary>
            private MLResult.Code InternalGetControlAEExposureCompensationResultMetadata(out int OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAEExposureCompensationResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEExposureCompensationResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets AE regions.
            /// </summary>
            private MLResult.Code InternalGetControlAERegionsResultMetadata(out int[][] OutData)
            {
                const int colSize = 3;
                const int rowSize = 5;
                const int elementSize = sizeof(int);
                IntPtr ptr = Marshal.AllocHGlobal(colSize * rowSize * elementSize);
                var result = NativeBindings.MLCameraMetadataGetControlAERegionsResultMetadata(Handle, ptr, out int OutCount);
                OutData = new int[OutCount][];
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAERegionsResultMetadata));

                if (result == MLResult.Code.Ok)
                {
                    for (int i = 0; i < OutCount; ++i)
                    {
                        OutData[i] = new int[rowSize];
                        for (int j = 0; j < rowSize; ++j)
                        {
                            OutData[i][j] = Marshal.ReadInt32(ptr);
                            ptr += elementSize;
                        }
                    }
                }

                Marshal.FreeHGlobal(ptr);
                return result;
            }

            /// <summary>
            /// Gets AE target FPS range.
            /// </summary>
            private MLResult.Code InternalGetControlAETargetFPSRangeResultMetadata(out int[] OutData)
            {
                int[] outData = new int[2];
                var result = NativeBindings.MLCameraMetadataGetControlAETargetFPSRangeResultMetadata(Handle, outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAETargetFPSRangeResultMetadata));
                OutData = outData;
                return result;
            }

            /// <summary>
            /// Gets AE state.
            /// </summary>
            private MLResult.Code InternalGetControlAEStateResultMetadata(out ControlAEState OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAEStateResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAEStateResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets AWB state.
            /// </summary>
            private MLResult.Code InternalGetControlAWBStateResultMetadata(out ControlAWBState OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAWBStateResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBStateResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets AWB mode.
            /// </summary>
            private MLResult.Code InternalGetControlAWBModeResultMetadata(out ControlAWBMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlAWBModeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAWBModeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets mode.
            /// </summary>
            private MLResult.Code InternalGetControlModeResultMetadata(out ControlMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlModeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlModeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets Scene mode.
            /// </summary>
            private MLResult.Code InternalGetControlSceneModeResultMetadata(out ControlSceneMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlSceneModeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlSceneModeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets sensor exposure time.
            /// </summary>
            private MLResult.Code InternalGetSensorExposureTimeResultMetadata(out long OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetSensorExposureTimeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorExposureTimeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets sensor sensitivity.
            /// </summary>
            private MLResult.Code InternalGetSensorSensitivityResultMetadata(out int OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetSensorSensitivityResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorSensitivityResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets frame captured timestamp.
            /// </summary>
            private MLResult.Code InternalGetSensorTimestampResultMetadata(out long OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetSensorTimestampResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorTimestampResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets sensor frame duration.
            /// </summary>
            private MLResult.Code InternalGetSensorFrameDurationResultMetadata(out long OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetSensorFrameDurationResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetSensorFrameDurationResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets Effect mode.
            /// </summary>
            private MLResult.Code InternalGetControlEffectModeResultMetadata(out ControlEffectMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlEffectModeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlEffectModeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets exposure time upper limit.
            /// </summary>
            private MLResult.Code InternalGetControlExposureUpperTimeLimitResultMetadata(out long OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlExposureUpperTimeLimitResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlExposureUpperTimeLimitResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets jpeg GPS coordinates.
            /// </summary>
            private MLResult.Code InternalGetJpegGPSCoordinatesResultMetadata(out double[] OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetJpegGPSCoordinatesResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegGPSCoordinatesResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets jpeg timestamp.
            /// </summary>
            private MLResult.Code InternalGetJpegGPSTimestampResultMetadata(out long OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetJpegGPSTimestampResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegGPSTimestampResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets jpeg thumbnail size.
            /// </summary>
            private MLResult.Code InternalGetJpegThumbnailSizeResultMetadata(out JpegThumbnailSize OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetJpegThumbnailSizeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegThumbnailSizeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets force apply mode.  Main camera and CV Camera share the same camera hardware resources.  When both the cameras are
            /// streaming, request metadata properties for both cameras are merged and then applied.  While merging, the metadata
            /// properties from Main Camera take precedence over CV camera metadata properties.  The force apply mode property can be used to
            /// override this.  If CV Camera metadata has force apply mode on, the CV Camera metadata properties take precedence over
            /// Main Camera metadata properties.
            /// </summary>
            private MLResult.Code InternalGetControlForceApplyModeResultMetadata(out ControlForceApplyMode OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetControlForceApplyModeResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlForceApplyModeResultMetadata));
                return result;
            }

            /// <summary>
            /// Gets jpeg quality.
            /// </summary>
            private MLResult.Code InternalGetJpegQualityResultMetadata(out byte OutData)
            {
                var result = NativeBindings.MLCameraMetadataGetJpegQualityResultMetadata(Handle, out OutData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetJpegQualityResultMetadata));
                return result;
            }

            private static Rational ConvertRational(NativeBindings.MLCameraMetadataRational rational)
            {
                Rational result = new Rational();
                result.Numerator = rational.Numerator;
                result.Denominator = rational.Denominator;
                return result;
            }

            private static NativeBindings.MLCameraMetadataRational ConvertRational(Rational rational)
            {
                NativeBindings.MLCameraMetadataRational result = new NativeBindings.MLCameraMetadataRational();
                result.Numerator = rational.Numerator;
                result.Denominator = rational.Denominator;
                return result;
            }

            private static NativeBindings.MLCameraMetadataRational[] ConvertRational(Rational[] rational)
            {
                NativeBindings.MLCameraMetadataRational[] result = new NativeBindings.MLCameraMetadataRational[rational.Length];
                for (int i = 0; i < rational.Length; ++i)
                {
                    result[i] = ConvertRational(rational[i]);
                }
                return result;
            }

            private static NativeBindings.MLCameraMetadataRational[][] ConvertRational(Rational[][] rational)
            {
                NativeBindings.MLCameraMetadataRational[][] result = new NativeBindings.MLCameraMetadataRational[rational.Length][];
                for (int i = 0; i < rational.Length; ++i)
                {
                    result[i] = ConvertRational(rational[i]);
                }
                return result;
            }

            private MLResult.Code InternalGetControlAFAvailableModes(out ControlAFMode[] outData)
            {
                outData = new ControlAFMode[0];
                var result = NativeBindings.MLCameraMetadataGetControlAFAvailableModes(Handle, out outData, out int resultCount);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAFAvailableModes));
                return result;
            }

            private MLResult.Code InternalGetLensInfoHyperfocalDistance(out float outData)
            {
                outData = 0f;
                var result = NativeBindings.MLCameraMetadataGetLensInfoHyperfocalDistance(Handle, out outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetLensInfoHyperfocalDistance));
                return result;
            }

            private MLResult.Code InternalGetLensInfoMinimumFocusDistance(out float outData)
            {
                outData = 0f;
                var result = NativeBindings.MLCameraMetadataGetLensInfoMinimumFocusDistance(Handle, out outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetLensInfoMinimumFocusDistance));
                return result;
            }

            private MLResult.Code InternalGetLensInfoMinimumFocusDistanceIncrement(out float outData)
            {
                outData = 0f;
                var result = NativeBindings.MLCameraMetadataGetLensInfoMinimumFocusDistanceIncrement(Handle, out outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetLensInfoMinimumFocusDistanceIncrement));
                return result;
            }

            private MLResult.Code InternalGetControlAFModeRequestMetadata(out ControlAFMode outData)
            {
                outData = ControlAFMode.Off;
                var result = NativeBindings.MLCameraMetadataGetControlAFModeRequestMetadata(Handle, out outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAFModeRequestMetadata));
                return result;
            }

            private MLResult.Code InternalGetControlAFTriggerRequestMetadata(out ControlAFTrigger outData)
            {
                outData = ControlAFTrigger.Cancel;
                var result = NativeBindings.MLCameraMetadataGetControlAFTriggerRequestMetadata(Handle, out outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAFTriggerRequestMetadata));
                return result;
            }

            private MLResult.Code InternalGetControlAFDistanceRangeRequestMetadata(out float min, out float max)
            {
                min = 0f;
                max = 0f;
                var result = NativeBindings.MLCameraMetadataGetAFDistanceRangeRequestMetadata(Handle, out float[] outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetAFDistanceRangeRequestMetadata));
                if (outData.Length == 2 && result == MLResult.Code.Ok)
                {
                    min = outData[0];
                    max = outData[1];
                }
                return result;
            }

            private MLResult.Code InternalGetLensFocusDistanceRequestMetadata(out float outData)
            {
                outData = 0f;
                var result = NativeBindings.MLCameraMetadataGetLensFocusDistanceRequestMetadata(Handle, out outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetLensFocusDistanceRequestMetadata));
                return result;
            }

            private MLResult.Code InternalSetControlAFMode(ControlAFMode mode)
            {
                var result = NativeBindings.MLCameraMetadataSetControlAFMode(Handle, in mode);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAFMode));
                return result;
            }

            private MLResult.Code InternalSetControlAFTrigger(ControlAFTrigger trigger)
            {
                var result = NativeBindings.MLCameraMetadataSetControlAFTrigger(Handle, in trigger);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetControlAFTrigger));
                return result;
            }

            private MLResult.Code InternalSetAFDistanceRange(float min, float max)
            {
                float[] values = new float[] { min, max };
                var result = NativeBindings.MLCameraMetadataSetAFDistanceRange(Handle, in values);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetAFDistanceRange));
                return result;
            }

            private MLResult.Code InternalSetLensFocusDistance(float distance)
            {
                var result = NativeBindings.MLCameraMetadataSetLensFocusDistance(Handle, in distance);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataSetLensFocusDistance));
                return result;
            }

            private MLResult.Code InternalGetControlAFModeResultMetadata(out ControlAFMode data)
            {
                data = ControlAFMode.Off;
                var result = NativeBindings.MLCameraMetadataGetControlAFModeResultMetadata(Handle, out data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAFModeResultMetadata));
                return result;
            }

            private MLResult.Code InternalGetControlAFTriggerResultMetadata(out ControlAFTrigger data)
            {
                data = ControlAFTrigger.Idle;
                var result = NativeBindings.MLCameraMetadataGetControlAFTriggerResultMetadata(Handle, out data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAFTriggerResultMetadata));
                return result;
            }

            private MLResult.Code InternalGetControlAFDistanceRangeResultMetadata(out float min, out float max)
            {
                min = 0f;
                max = 0f;
                var result = NativeBindings.MLCameraMetadataGetAFDistanceRangeResultMetadata(Handle, out float[] outData);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetAFDistanceRangeResultMetadata));
                if (outData.Length == 2 && result == MLResult.Code.Ok)
                {
                    min = outData[0];
                    max = outData[1];
                }
                return result;
            }

            private MLResult.Code InternalGetControlAFStateResultMetadata(out ControlAFState data)
            {
                data = ControlAFState.Inactive;
                var result = NativeBindings.MLCameraMetadataGetControlAFStateResultMetadata(Handle, out data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAFStateResultMetadata));
                return result;
            }

            private MLResult.Code InternalGetControlAFSceneChangeResultMetadata(out ControlAFSceneChange data)
            {
                data = ControlAFSceneChange.NotDetected;
                var result = NativeBindings.MLCameraMetadataGetControlAFSceneChangeResultMetadata(Handle, out data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetControlAFSceneChangeResultMetadata));
                return result;
            }

            private MLResult.Code InternalGetLensFocusDistanceResultMetadata(out float data)
            {
                data = 0f;
                var result = NativeBindings.MLCameraMetadataGetLensFocusDistanceResultMetadata(Handle, out data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetLensFocusDistanceResultMetadata));
                return result;
            }

            private MLResult.Code InternalGetLensStateResultMetadata(out LensState data)
            {
                data = LensState.Stationary;
                var result = NativeBindings.MLCameraMetadataGetLensStateResultMetadata(Handle, out data);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLCameraMetadataGetLensStateResultMetadata));
                return result;
            }
        }
    }
}
