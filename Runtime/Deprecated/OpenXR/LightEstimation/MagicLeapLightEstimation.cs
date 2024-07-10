#if UNITY_OPENXR_1_9_0_OR_NEWER
using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.XR.OpenXR.NativeTypes;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapLightEstimationFeature
    {
            private const int EstimateQueryThreshold = 2000; // 2 seconds

            private ulong lightEstimation;
            private long lastEstimateQueryTimeNanoSeconds;
            private bool lightEstimationReady;
            private DateTime lastUpdateTime;

            public bool LightEstimationCreated { get; private set; }

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
                
                ulong lightEstimationEstimate = CreateLightEstimationEstimate();
                
                lastUpdateTime = DateTime.Now;

                estimateData.CubeMap = GetHDRCubemapData(lightEstimationEstimate);
                estimateData.DirectionalLight = GetMainDirectionalLight(lightEstimationEstimate);
                estimateData.HarmonicsCoefficients = GetSphericalHarmonics(lightEstimationEstimate);
                estimateData.TimeStampNanoSeconds = GetTimestamp(lightEstimationEstimate);

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
                Cubemap cubemap = new Cubemap(faceDim, TextureFormat.RGBA32, false);

                int numBytes = faceDim * faceDim * 4;

                for (int i = 0; i < 6; i++)
                {                    
                    byte[] pixelBytes = new byte[numBytes];

                    // convert the values in the float array for the given face into a byte array
                    for (int j = 0; j < numBytes; j++)
                    {
                        pixelBytes[j] = (byte)(rawPixelsArray[i * numBytes + j] * 255);
                    }

                    pixelBytes = MLTextureUtils.FlipPixelsVertically(pixelBytes, faceDim, faceDim);

                    CubemapFace cubemapFace;

                    switch (i)
                    {
                        case 0:
                            cubemapFace = CubemapFace.PositiveX;
                            break;
                        case 1:
                            cubemapFace = CubemapFace.NegativeX;
                            break;
                        case 2:
                            cubemapFace = CubemapFace.PositiveY;
                            break;
                        case 3:
                            cubemapFace = CubemapFace.NegativeY;
                            break;
                        case 4:
                            cubemapFace = CubemapFace.PositiveZ;
                            break;
                        case 5:
                            cubemapFace = CubemapFace.NegativeZ;
                            break;
                        default:
                            cubemapFace = CubemapFace.Unknown;
                            break;
                    }

                    cubemap.SetPixelData(pixelBytes, 0, cubemapFace);
                }

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
                lightOutput.Direction = mainDirectionalLight.Direction;
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
#endif
