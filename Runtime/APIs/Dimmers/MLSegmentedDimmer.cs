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
        private static SegmentedDimmerFeature urpFeature;
        private static SegmentedDimmerFeature Feature
        {
            get
            {
                if(urpFeature == null)
                {
                    var urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                    if(urp != null)
                    {
                        urpFeature = urp.GetRendererFeature<SegmentedDimmerFeature>() as SegmentedDimmerFeature;
                    }
                }
                return urpFeature;
            }
        }
#endif

        private static int defaultLayer = -1;

        /// <summary>
        /// Turn on the ability to display Segmented Dimmer in your scenes by requesting support from the ML Graphics API
        /// </summary>
        public static void Activate()
        {
            MLGraphicsHooks.RequestAlphaBlendFrameRendering(true);
        }

        /// <summary>
        /// Inform the ML Graphics API to turn off the ability to display Segmented Dimmer
        /// </summary>
        public static void Deactivate()
        {
            MLGraphicsHooks.RequestAlphaBlendFrameRendering(false);
        }

        /// <summary>
        /// Does the Universal Render Pipeline contain a Segmented Dimmer feature in its renderers
        /// </summary>
#if URP_14_0_0_OR_NEWER
        public static bool Exists => Feature != null;
#else
        public static bool Exists => false;
#endif

        /// <summary>
        /// Enables or disables all Mesh Renderers in the current scene which are on a SegmentedDimmer layer. <br/>
        /// WARNING: This is expensive and it is recommended not to use it often! 
        /// </summary>
        /// <param name="enabled"></param>
        public static void SetEnabled(bool enabled)
        {
            var renderers = GameObject.FindObjectsOfType<MeshRenderer>();
            foreach(var r in renderers)
            {
                if((Feature.settings.layerMask & (1 << r.gameObject.layer)) != 0)
                {
                    r.enabled = enabled;
                }
            }
        }        

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
                if (Feature.settings.layerMask == (Feature.settings.layerMask | (1 << i)))
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
