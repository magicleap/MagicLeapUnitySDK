// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2023) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_OPENXR_1_7_0_OR_NEWER
using UnityEngine;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport;

namespace UnityEditor.XR.OpenXR.Features.MagicLeapSupport
{
    public static class SegmentedDimmerMenuItem
    {
        [MenuItem("GameObject/XR/Magic Leap Segmented Dimmer")]
        public static void AddSegmentedDimmerToScene()
        {
            var mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError($"Could not find Main Camera! Before adding a Segmented Dimmer to your scene, you must add a Camera and tag it \"MainCamera\"");
                return;
            }

            var prefab = AssetDatabase.LoadAssetAtPath<DimmerCameraControl>("Packages/com.magicleap.unitysdk/Runtime/Tools/Prefabs/Segmented Dimmer.prefab");
            var instance = GameObject.Instantiate(prefab.gameObject, mainCamera.transform);
            instance.name = "Segmented Dimmer";
            Selection.objects = new Object[] { instance };
        }
    }
}
#endif
