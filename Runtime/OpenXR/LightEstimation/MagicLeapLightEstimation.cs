// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System;
using System.Threading.Tasks;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features.LightEstimation
{
    public partial class MagicLeapLightEstimationFeature
    {
        private const int EstimateQueryThreshold = 2000; // 2 seconds
        private const int NumChannels = 4; // RGBA
        private const int FaceCount = 6;
        private const int CubeMapBatchSize = 10;

        internal const int TotalHarmonics = 27;

        internal const int SpecVersion = 1;

        private ulong lightEstimation;
        private long lastEstimateQueryTimeNanoSeconds;
        private bool lightEstimationReady;
        private DateTime lastUpdateTime;

        public bool LightEstimationCreated { get; private set; }

        [BurstCompile]
        private struct ConvertAndFlipJob : IJobParallelFor
        {
            [NativeDisableParallelForRestriction]
            [ReadOnly] public NativeSlice<float> InputPixels;
            [NativeDisableParallelForRestriction]
            [WriteOnly] public NativeSlice<float> OutputPixels;
            public int FaceDim;

            public void Execute(int i)
            {
                int floatIndex = i * 4; // 4 channels (RGBA)
                int x = i % FaceDim;
                int y = i / FaceDim;
                int flippedIndex = (FaceDim - 1 - y) * FaceDim + x;
                int outputIndex = flippedIndex * 4;

                // Direct assignment without additional conversion
                OutputPixels[outputIndex] = InputPixels[floatIndex];
                OutputPixels[outputIndex + 1] = InputPixels[floatIndex + 1];
                OutputPixels[outputIndex + 2] = InputPixels[floatIndex + 2];
                OutputPixels[outputIndex + 3] = InputPixels[floatIndex + 3];
            }
        }

        /// <summary>
        /// Creates a light estimation to enable light estimates to be created for a given cubemap resolution.
        /// </summary>
        /// <param name="cubemapFaceResolution">The width/height resolution of the face of the cubemap to be used.</param>
        public void CreateLightEstimation(HDRCubemapFaceResolution cubemapFaceResolution)
        {
            if (LightEstimationCreated)
            {
                DestroyLightEstimation();
            }

            var createInfo = new XrLightEstimationCreateInfo
            {
                Type = XrLightEstimationStructTypes.XrTypeLightEstimationCreateInfoML,
                Next = default,
                BaseSpace = AppSpace,
                Time = NextPredictedDisplayTime,
                CubemapFaceResolution = (XrLightEstimationHDRCubemapFaceResolution)cubemapFaceResolution
            };

            unsafe 
            {
                var resultCode = nativeFunctions.XrCreateLightEstimation(AppSession, ref createInfo, out lightEstimation);
                bool didSucceed = Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrCreateLightEstimation));

                if (didSucceed)
                {
                    LightEstimationCreated = true;

                    DelayReadiness();
                }
            }
        }

        /// <summary>
        /// Destroys the active light estimation so light estimates can no longer be created.
        /// </summary>
        public void DestroyLightEstimation()
        {        
            unsafe 
            {             
                var resultCode = nativeFunctions.XrDestroyLightEstimation(lightEstimation);
                bool didSucceed = Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrDestroyLightEstimation));
                
                if (didSucceed)
                {
                    lightEstimation = 0;
                    LightEstimationCreated = false;
                    lightEstimationReady = false;
                    lastUpdateTime = DateTime.MinValue;
                }
            }
        }       

        /// <summary>
        /// Gets the last time that the light estimation state was updated.
        /// </summary>
        /// <returns>The time of last update in nanoseconds.</returns>
        public long GetLastUpdateTime()
        {
            var state = new XrLightEstimationState
            {
                Type = XrLightEstimationStructTypes.XrTypeLightEstimationStateML,
                Next = default,
                LastUpdate = default
            };

            unsafe
            {
                var resultCode = nativeFunctions.XrGetLightEstimationState(lightEstimation, ref state);
                Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetLightEstimationState));
            }

            return state.LastUpdate;
        }          

        /// <summary>
        /// Determines if enough time has passed for a light estimation estimate to obtain data.
        /// </summary>
        /// <returns>The readiness of the light estimation estimate.</returns>
        public bool CheckEstimationEstimateReadiness()
        {
            if (!lightEstimationReady)
            {
                return false;
            }

            // allow first time creating estimate to occur if it did not occur for the estimation already
            if (lastUpdateTime == DateTime.MinValue)
            {
                return true;
            }

            TimeSpan elapsed = DateTime.Now - lastUpdateTime;

            return elapsed.TotalMilliseconds >= EstimateQueryThreshold;
        }

        /// <summary>
        /// Creates a light estimation estimate, obtains the light estimate data, then destroys the light estimate.
        /// </summary>
        /// <returns>The data obtained from a light estimation estimate.</returns>
        public EstimateData GetLightEstimationEstimateData()
        {
            EstimateData estimateData;

            var lastUpdate = GetLastUpdateTime();
            if (lastUpdate == 0)
            {
                return default;
            }
            ulong lightEstimationEstimate = CreateLightEstimationEstimate();

            lastUpdateTime = DateTime.Now;

            estimateData.CubeMap = GetHDRCubemapData(lightEstimationEstimate);
            estimateData.DirectionalLight = GetMainDirectionalLight(lightEstimationEstimate);
            estimateData.TimeStampNanoSeconds = GetTimestamp(lightEstimationEstimate);
            estimateData.HarmonicsCoefficients = GetSphericalHarmonics(lightEstimationEstimate);

            DestroyLightEstimationEstimate(lightEstimationEstimate);

            return estimateData;
        }         

        /// <summary>
        /// Gets a Unity Cubemap object from a light estimate's cubemap data.
        /// </summary>
        /// <param name="rawPixelsArray">The float array of pixels obtained from a light estimation estimate.</param>
        /// <param name="faceDim">The desired width/height dimension of each face of the cubemap.</param>
        /// <returns>A cubemap object from the light estimate data in a format usable by Unity.</returns>
        public Cubemap GetEstimateCubemap(float[] rawPixelsArray, int faceDim)
        {
            Cubemap cubemap = new Cubemap(faceDim, TextureFormat.RGBAFloat, false, true);

            int numPixels = faceDim * faceDim;         

            NativeArray<float> allInputPixels = new NativeArray<float>(rawPixelsArray, Allocator.TempJob);
            NativeArray<float> allOutputPixels = new NativeArray<float>(numPixels * NumChannels * FaceCount, Allocator.TempJob);
            NativeArray<JobHandle> jobHandles = new NativeArray<JobHandle>(FaceCount, Allocator.Temp);

            for (int faceIndex = 0; faceIndex < FaceCount; faceIndex++)
            {
                int baseIndex = faceIndex * numPixels * NumChannels;
                NativeSlice<float> inputSlice = new NativeSlice<float>(allInputPixels, baseIndex, numPixels * NumChannels);
                NativeSlice<float> outputSlice = new NativeSlice<float>(allOutputPixels, baseIndex, numPixels * NumChannels);

                var conversionJob = new ConvertAndFlipJob
                {
                    InputPixels = inputSlice,
                    OutputPixels = outputSlice,
                    FaceDim = faceDim
                };

                jobHandles[faceIndex] = conversionJob.Schedule(numPixels, CubeMapBatchSize);
            }

            JobHandle.CompleteAll(jobHandles);

            for (int faceIndex = 0; faceIndex < FaceCount; faceIndex++)
            {
                int baseIndex = faceIndex * numPixels * NumChannels;
                NativeSlice<float> outputSlice = new NativeSlice<float>(allOutputPixels, baseIndex, numPixels * NumChannels);
                cubemap.SetPixelData(outputSlice.ToArray(), 0, (CubemapFace)faceIndex);
            }

            allInputPixels.Dispose();
            allOutputPixels.Dispose();
            cubemap.Apply();

            return cubemap;
        } 

        private unsafe ulong CreateLightEstimationEstimate()
        {
            var resultCode = nativeFunctions.XrCreateLightEstimationEstimate(lightEstimation, out ulong lightEstimationEstimate);
            Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrCreateLightEstimationEstimate));

            return lightEstimationEstimate;
        }                   

        private unsafe HDRCubemapData GetHDRCubemapData(ulong lightEstimationEstimate)
        {
            var hdrCubemap = new XrLightEstimationHDRCubemap
            {
                Type = XrLightEstimationStructTypes.XrTypeLightEstimationHDRCubemapML,
                Next = default,
                PixelCountInput = default,
                PixelCountOutput = default,
                Pixels = null,
                FaceDim = default                    
            };

            // call first time to get pixelCountOutput
            var resultCode = nativeFunctions.XrGetLightEstimationHDRCubemap(lightEstimationEstimate, ref hdrCubemap);
            bool didSucceed = Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetLightEstimationHDRCubemap));

            if (!didSucceed)
            {
                return default;
            }

            int pixelsArraySize = (int)hdrCubemap.PixelCountOutput;
            hdrCubemap.PixelCountInput = hdrCubemap.PixelCountOutput;
            hdrCubemap.Pixels = (float*)new NativeArray<float>(pixelsArraySize, Allocator.Temp).GetUnsafePtr();

            // call second time to get pixels data
            resultCode = nativeFunctions.XrGetLightEstimationHDRCubemap(lightEstimationEstimate, ref hdrCubemap);
            didSucceed = Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetLightEstimationHDRCubemap));

            if (!didSucceed)
            {
                return default;
            }

            HDRCubemapData outputData;
            outputData.Pixels = new float[pixelsArraySize];
            outputData.FaceDimension = hdrCubemap.FaceDim;

            for (int i = 0; i < pixelsArraySize; i++)
            {
                outputData.Pixels[i] = hdrCubemap.Pixels[i];
            }

            return outputData;
        }

        private MainDirectionalLight GetMainDirectionalLight(ulong lightEstimationEstimate)
        {
            var mainDirectionalLight = new XrLightEstimationMainDirectionalLight
            {
                Type = XrLightEstimationStructTypes.XrTypeLightEstimationMainDirectionalLightML,
                Next = default,
                Direction = default,
                Color = default                  
            };
            
            unsafe 
            {
                var resultCode = nativeFunctions.XrGetLightEstimationMainDirectionalLight(lightEstimationEstimate, ref mainDirectionalLight);
                Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetLightEstimationMainDirectionalLight));
            }

            MainDirectionalLight lightOutput;
            lightOutput.Direction = new Vector3(mainDirectionalLight.Direction.x, mainDirectionalLight.Direction.y, -mainDirectionalLight.Direction.z);
            lightOutput.Color = mainDirectionalLight.Color;

            return lightOutput;
        }

        private unsafe float[] GetSphericalHarmonics(ulong lightEstimationEstimate)
        {
            float[] outputArray = new float[TotalHarmonics];

            var sphericalHarmonics = new XrLightEstimationSphericalHarmonics();
            sphericalHarmonics.Type = XrLightEstimationStructTypes.XrTypeLightEstimationSphericalHarmonicsML;
            sphericalHarmonics.Next = default;  

            var resultCode = nativeFunctions.XrGetLightEstimationSphericalHarmonics(lightEstimationEstimate, ref sphericalHarmonics);
            Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetLightEstimationSphericalHarmonics));

            for (int i = 0; i < TotalHarmonics; i++)
            {
                outputArray[i] = sphericalHarmonics.HarmonicsCoefficients[i];
            }

            return outputArray;
        }

        private unsafe long GetTimestamp(ulong lightEstimationEstimate)
        {
            var resultCode = nativeFunctions.XrGetLightEstimationTimestamp(lightEstimationEstimate, out long lastTimestampNanoSeconds);
            Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrGetLightEstimationTimestamp));

            return lastTimestampNanoSeconds;
        }

        private unsafe void DestroyLightEstimationEstimate(ulong lightEstimationEstimate)
        {        
            var resultCode = nativeFunctions.XrDestroyLightEstimationEstimate(lightEstimationEstimate);
            bool didSucceed = Utils.DidXrCallSucceed(resultCode, nameof(nativeFunctions.XrDestroyLightEstimationEstimate));
            
            if (didSucceed)
            {
                lightEstimationEstimate = 0;
            }
        }            

        private async void DelayReadiness()
        {
            await Task.Delay(EstimateQueryThreshold);
            lightEstimationReady = true;
        }
    }
}
