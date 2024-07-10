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
using UnityEngine;

namespace MagicLeap.Settings
{
    public class MagicLeapProjectSettings : ScriptableObject
    {
        const string defaultAssetPath = "Assets/MagicLeapSettings.asset";

        [SerializeField, HideInInspector]
        private bool enableMLAudio;

        [SerializeField, HideInInspector]
        private int permissionsMinApiLevel = 29;

        public bool EnableMLAudio 
        { 
            get => enableMLAudio;
            set => ApplyBoolValue(nameof(enableMLAudio), value);
        }

        public int PermissionsMinApiLevel
        {
            get => permissionsMinApiLevel;
            set => ApplyIntValue(nameof(permissionsMinApiLevel), value);
        }

        public static MagicLeapProjectSettings Instance => GetOrCreateSettings();

        internal static MagicLeapProjectSettings GetOrCreateSettings()
        {
            var settings = AssetDatabase.LoadAssetAtPath<MagicLeapProjectSettings>(defaultAssetPath);
            if (settings == null)
            {
                settings = CreateInstance<MagicLeapProjectSettings>();
                settings.enableMLAudio = true;
                AssetDatabase.CreateAsset(settings, defaultAssetPath);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal static SerializedObject GetSerializedSettings()
        {
            return new SerializedObject(GetOrCreateSettings());
        }

        private void ApplyBoolValue(string propertyName, bool value)
        {
            var settings = GetSerializedSettings();
            settings.FindProperty(propertyName).boolValue = value;
            settings.ApplyModifiedPropertiesWithoutUndo();
        }

        private void ApplyIntValue(string propertyName, int value)
        {
            var settings = GetSerializedSettings();
            settings.FindProperty(propertyName).intValue = value;
            settings.ApplyModifiedPropertiesWithoutUndo();
        }
    }
}
