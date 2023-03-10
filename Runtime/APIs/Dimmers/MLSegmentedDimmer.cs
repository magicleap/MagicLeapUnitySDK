using System;
using UnityEngine.Rendering;
#if URP_14_0_0_OR_NEWER
using UnityEngine.Rendering.Universal;
using SegmentedDimmerFeature = URP.SegmentedDimmer.SegmentedDimmer;
#endif

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLSegmentedDimmer
    {
#if URP_14_0_0_OR_NEWER
        /// <summary>
        /// The actual URP feature that represents the Segmented Dimmer to Unity
        /// </summary>
        private static SegmentedDimmerFeature segmentedDimmerFeature;
        private static SegmentedDimmerFeature SegmentedDimmerFeature
        {
            get
            {
                if(segmentedDimmerFeature == null)
                {
                    var urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                    if(urp != null)
                    {
                        segmentedDimmerFeature = urp.GetRendererFeature<SegmentedDimmerFeature>() as SegmentedDimmerFeature;
                    }
                }
                return segmentedDimmerFeature;
            }
        }
#endif

        private static int defaultLayer = -1;

        /// <summary>
        /// Turn on the ability to display Segmented Dimmer in your scenes.
        /// This instructs MLGraphics to provide frames using the AlphaBlend blend mode.
        /// </summary>
        public static void Activate()
        {
            MLGraphicsHooks.RequestAlphaBlendFrameRendering(true);
        }

        /// <summary>
        /// Turn off the ability to display Segmented Dimmer in your scenes.
        /// This reverts to accepting frames with the default blend mode from MLGraphics.
        /// </summary>
        public static void Deactivate()
        {
            MLGraphicsHooks.RequestAlphaBlendFrameRendering(false);
        }

        /// <summary>
        /// Does the Universal Render Pipeline contain a Segmented Dimmer feature in its renderers
        /// </summary>
#if URP_14_0_0_OR_NEWER
        public static bool Exists => SegmentedDimmerFeature != null;
#else
        public static bool Exists => false;
#endif

        /// <summary>
        /// Returns the first available Unity Layer specified in the LayerMask for the SegmentedDimmer Feature. This will be the default layer SegmentedDimmer mesh objects
        /// will be assigned to when first configured.
        /// </summary>
        /// <returns></returns>
        public static int GetDefaultLayer()
        {
#if URP_14_0_0_OR_NEWER
            if (defaultLayer >= 0)
            {
                return defaultLayer;
            }
            for (int i = 0; i < 32; i++)
            {
                if (SegmentedDimmerFeature.settings.layerMask == (SegmentedDimmerFeature.settings.layerMask | (1 << i)))
                {
                    defaultLayer = i;
                    break;
                }
            }
#endif
            return defaultLayer;
        }

        
    }
}
