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
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLCameraBase
    {
        public static StreamCapability[] GetImageStreamCapabilitiesForCamera(MLCamera camera, params CaptureType[] captureTypes)
        {
            var result = camera.GetStreamCapabilities(out StreamCapabilitiesInfo[] streamCapabilitiesInfo);

            if (!result.IsOk)
            {
                return new StreamCapability[0];
            }

            List<StreamCapability> streamCapabilities = new List<StreamCapability>();

            for (int i = 0; i < streamCapabilitiesInfo.Length; i++)
            {
                foreach (var streamCap in streamCapabilitiesInfo[i].StreamCapabilities)
                {
                    foreach (var type in captureTypes)
                    {
                        if (streamCap.CaptureType == type)
                        {
                            streamCapabilities.Add(streamCap);
                        }
                    }
                }
            }

            return streamCapabilities.ToArray();
        }

        public static bool TryGetBestFitStreamCapabilityFromCollection(StreamCapability[] streamCapabilities, int width, int height, CaptureType captureType, out StreamCapability streamCapability)
        {
            int bestFitIndex = -1;
            int minScore = int.MaxValue;
            streamCapability = default;

            for (int i = 0; i < streamCapabilities.Length; ++i)
            {
                StreamCapability capability = streamCapabilities[i];
                if (capability.CaptureType == captureType)
                {
                    int score = Mathf.Abs(capability.Width - width + (capability.Height - height));
                    if (score < minScore)
                    {
                        minScore = score;
                        bestFitIndex = i;
                    }
                }
            }

            if (bestFitIndex == -1)
                return false;

            streamCapability = streamCapabilities[bestFitIndex];
            return true;
        }

        public static bool IsCaptureTypeSupported(MLCamera camera, CaptureType captureType)
        {
            var caps = GetImageStreamCapabilitiesForCamera(camera, captureType);
            foreach (var cap in caps)
            {
                if (cap.CaptureType == captureType)
                {
                    return true;
                }
            }
            return false;
        }

        public static void FlipFrameVertically(ref CameraOutput frame)
        {
            for(int i = 0; i < frame.Planes.Length; i++)
            {
                var flippedArray = new byte[frame.Planes[i].Data.Length];
                
                int width = (frame.Format == OutputFormat.YUV_420_888) ? (int)frame.Planes[i].Stride : (int)frame.Planes[i].Width;
                int height = (int)frame.Planes[i].Height;
                int bytesPerPixel = (int)frame.Planes[i].BytesPerPixel;
                int stride = (int)frame.Planes[i].Stride;

                for (int y = 0; y < height; y++)
                {
                    int x = height - y - 1;
                    Buffer.BlockCopy(frame.Planes[i].Data, y * stride, flippedArray, x * stride, stride);
                }

                frame.Planes[i].Data = flippedArray;
            }
        }
    }
}
