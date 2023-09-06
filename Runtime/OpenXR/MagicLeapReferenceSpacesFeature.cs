#if UNITY_OPENXR_1_7_0_OR_NEWER
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Reference Spaces Support",
        Desc="Support for additional XR reference spaces supported by Magic Leap 2",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new []{ BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = featureId,
        OpenxrExtensionStrings = "XR_MSFT_unbounded_reference_space " +
                                 "XR_EXT_local_floor "
    )]
#endif
    public class MagicLeapReferenceSpacesFeature : MagicLeapOpenXRFeatureBase
    {
        public const string featureId = "com.magicleap.openxr.feature.reference_spaces";
    }
}
#endif
