// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEditor;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Features
{
    [CustomEditor(typeof(MagicLeapRenderingExtensionsFeature))]
    public class MagicLeapRenderingExtensionsFeatureEditor : Editor
    {
        private SerializedProperty protectedSurfaceProperty;
        private SerializedProperty vignetteProperty;
        private SerializedProperty blendModeProperty;
        private SerializedProperty globalDimmerProperty;
        private SerializedProperty globalDimmerValueProperty;

        private enum SupportedBlendModes
        {
            Additive = XrEnvironmentBlendMode.Additive,
            AlphaBlend = XrEnvironmentBlendMode.AlphaBlend
        }

        private void OnEnable()
        {
            blendModeProperty = serializedObject.FindProperty("defaultBlendModeSelection");
            protectedSurfaceProperty = serializedObject.FindProperty("UseProtectedSurface");
            vignetteProperty = serializedObject.FindProperty("UseVignetteMode");
            globalDimmerProperty = serializedObject.FindProperty("GlobalDimmerEnabled");
            globalDimmerValueProperty = serializedObject.FindProperty("GlobalDimmerValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            blendModeProperty.intValue = (int)(SupportedBlendModes)EditorGUILayout.EnumPopup("Blend Mode", (SupportedBlendModes)blendModeProperty.intValue);
            protectedSurfaceProperty.boolValue = EditorGUILayout.Toggle("Protected Surface", protectedSurfaceProperty.boolValue);
            vignetteProperty.boolValue = EditorGUILayout.Toggle("Vignette", vignetteProperty.boolValue);
            globalDimmerProperty.boolValue = EditorGUILayout.Toggle("Enable Global Dimmer", globalDimmerProperty.boolValue);

            var dimmerEnabled = globalDimmerProperty.boolValue;
            globalDimmerValueProperty.floatValue = EditorGUILayout.Slider("Global Dimmer Value", dimmerEnabled ? globalDimmerValueProperty.floatValue : 0f, 0f, 1f);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
