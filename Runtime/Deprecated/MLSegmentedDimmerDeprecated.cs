using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLSegmentedDimmer
    {
        [Obsolete("SetEnabled will be removed in a future release! Use EnableRendererFeature() and DisableRendererFeature() to toggle the configuration on your URP asset.")]
        public static void SetEnabled(bool enabled)
        {
            // Setting the RendererFeature to "disabled" doesn't actually do anything due
            // to a limitation with URP.
        }
    }
}
