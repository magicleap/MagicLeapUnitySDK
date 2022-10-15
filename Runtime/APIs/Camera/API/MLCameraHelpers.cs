using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLCamera
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
    }
}
