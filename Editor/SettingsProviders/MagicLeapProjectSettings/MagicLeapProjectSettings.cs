using UnityEngine;

namespace UnityEditor.XR.MagicLeap
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
