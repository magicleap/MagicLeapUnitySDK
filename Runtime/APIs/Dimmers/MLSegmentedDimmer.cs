using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;
#if URP_14_0_0_OR_NEWER
using UnityEngine.Rendering.Universal;
using SegmentedDimmerFeature = URP.SegmentedDimmer.SegmentedDimmer;
#endif

namespace UnityEngine.XR.MagicLeap
{
    public class MLSegmentedDimmer
    {
#if URP_14_0_0_OR_NEWER
        /// <summary>
        /// The actual URP feature that represents the Segmented Dimmer to Unity
        /// </summary>
        private static SegmentedDimmerFeature dimmerInstance;
        private static SegmentedDimmerFeature DimmerInstance
        {
            get
            {
                if(dimmerInstance == null)
                {
                    var urp = GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset;
                    if(urp != null)
                    {
                        dimmerInstance = urp.GetRendererFeature<SegmentedDimmerFeature>() as SegmentedDimmerFeature;
                    }
                }
                return dimmerInstance;
            }
        }
#endif

        /// <summary>
        /// Does the Universal Render Pipeline contain a Segmented Dimmer feature in its renderers
        /// </summary>
#if URP_14_0_0_OR_NEWER
        public static bool Exists => DimmerInstance != null;
#else
        public static bool Exists => false;
#endif

        /// <summary>
        /// Is the Segmented Dimmer feature enabled in the render pipeline
        /// </summary>
        public static bool IsEnabled
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    return DimmerInstance.settings.isEnabled;
                }
#endif
                return false;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    DimmerInstance.settings.isEnabled = value;
                }
#endif
            }
        }

        /// <summary>
        /// LayerMask for filtering the rendering of the dimmer by URP. 
        /// </summary>
        public static LayerMask LayerMask
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    return DimmerInstance.settings.layerMask;
                }
#endif
                return -1;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    DimmerInstance.settings.layerMask = value;
                }
#endif
            }
        }

        /// <summary>
        /// Clear value for the Segmented Dimmer texture. Can be between 0 and 1.
        /// </summary>
        public static float ClearValue
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    return DimmerInstance.settings.clearValue;
                }
#endif
                return -1;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    if (value < 0)
                    {
                        DimmerInstance.settings.clearValue = 0;
                    }
                    else if (value > 1)
                    {
                        DimmerInstance.settings.clearValue = 1;
                    }
                    else
                    {
                        DimmerInstance.settings.clearValue = value;
                    }
                }
#endif
            }
        }

        /// <summary>
        /// Should the render pipeline override any materials on dimmer meshes and apply a fully opaque shader instead
        /// </summary>
        public static bool IgnoreMaterials
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    return DimmerInstance.settings.overrideMaterial;
                }
#endif
                return false;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    DimmerInstance.settings.overrideMaterial = value;
                }
#endif
            }
        }

        /// <summary>
        /// Using the full rsolution will try to render the mask directly in the alpha channel of the color target, if possible.
        /// </summary>
        public static bool UseFullResolution
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    return DimmerInstance.settings.useFullResolution;
                }
#endif
                return false;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    DimmerInstance.settings.useFullResolution = value;
                }
#endif
            }
        }

        /// <summary>
        /// Only applicable if <see cref="UseFullResolution"/> is false. Width of the render target to use for the dimmer.
        /// </summary>
        public static int RenderTargetWidth
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    return DimmerInstance.settings.renderTextureSize.x;
                }
#endif
                return -1;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    DimmerInstance.settings.renderTextureSize.x = value;
                }
#endif
            }
        }

        /// <summary>
        /// Only applicable if <see cref="UseFullResolution"/> is false. Height of the render target to use for the dimmer.
        /// </summary>
        public static int RenderTargetHeight
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    return DimmerInstance.settings.renderTextureSize.y;
                }
#endif
                return -1;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (DimmerInstance != null)
                {
                    DimmerInstance.settings.renderTextureSize.y = value;
                }
#endif
            }
        }
    }
}
