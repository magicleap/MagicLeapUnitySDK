using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLSegmentedDimmer
    {
        [Obsolete("This property will be removed in a future release. Instead, use SetEnabled() to turn on or off SegmentedDimmer meshes in your scene at runtime.")]
        public static bool IsEnabled
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    return Feature.settings.isEnabled;
                }
#endif
                return false;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    Feature.settings.isEnabled = value;
                }
#endif
            }
        }

        [Obsolete("This property is not functional and will be removed in a future release. The LayerMask must be configured on the URP asset before building!")]
        public static LayerMask LayerMask
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    return Feature.settings.layerMask;
                }
#endif
                return -1;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    Feature.settings.layerMask = value;
                }
#endif
            }
        }

        [Obsolete("This property is not functional and will be removed in a future release. The clear value must be configured on the URP asset before building!")]
        public static float ClearValue
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    return Feature.settings.clearValue;
                }
#endif
                return -1;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    if (value < 0)
                    {
                        Feature.settings.clearValue = 0;
                    }
                    else if (value > 1)
                    {
                        Feature.settings.clearValue = 1;
                    }
                    else
                    {
                        Feature.settings.clearValue = value;
                    }
                }
#endif
            }
        }

        [Obsolete("This property is not functional and will be removed in a future release, and the option in URP been deprecated by Unity.")]
        public static bool IgnoreMaterials
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    return Feature.settings.overrideMaterial;
                }
#endif
                return false;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    Feature.settings.overrideMaterial = value;
                }
#endif
            }
        }

        [Obsolete("This property is not functional and will be removed in a future release. The \"use full resolution\" option must be set on the URP asset before building!")]
        public static bool UseFullResolution
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    return Feature.settings.useFullResolution;
                }
#endif
                return false;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    Feature.settings.useFullResolution = value;
                }
#endif
            }
        }

        [Obsolete("This property is not functional and will be removed in a future release. The X value dimension must be set on the URP asset before building!")]
        public static int RenderTargetWidth
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    return Feature.settings.renderTextureSize.x;
                }
#endif
                return -1;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    Feature.settings.renderTextureSize.x = value;
                }
#endif
            }
        }

        [Obsolete("This property is not functional and will be removed in a future release. The Y value dimension must be set on the URP asset before building!")]
        public static int RenderTargetHeight
        {
            get
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    return Feature.settings.renderTextureSize.y;
                }
#endif
                return -1;
            }
            set
            {
#if URP_14_0_0_OR_NEWER
                if (Feature != null)
                {
                    Feature.settings.renderTextureSize.y = value;
                }
#endif
            }
        }
    }
}
