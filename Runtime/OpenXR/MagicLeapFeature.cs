using System.Collections.Generic;
using UnityEditor;

#if UNITY_OPENXR_1_4_0_OR_NEWER
#if UNITY_EDITOR
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    /// <summary>
    /// Enables the Magic Leap OpenXR Loader for Android, and modifies the AndroidManifest to be compatible with ML2.
    /// </summary>
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap Support",
        Desc="Necessary to deploy a Magic Leap compatible app.",
        Company = "Magic Leap",
        Version = "0.53.2",
        BuildTargetGroups = new []{ BuildTargetGroup.Android },
        CustomRuntimeLoaderBuildTargets = new []{ BuildTarget.Android },
        FeatureId = featureId
    )]
#endif
    public class MagicLeapFeature : OpenXRFeature
    {
        /// <summary>
        /// The feature id string. This is used to give the feature a well known id for reference.
        /// </summary>
        public const string featureId = "com.magicleap.openxr.feature.magicleap";

        [SerializeField]
        private bool enableMLAudio = false;

        public bool IsMLAudioEnabled => enableMLAudio;
    }
}
#endif
