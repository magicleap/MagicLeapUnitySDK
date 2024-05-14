using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using UnityEngine.Windows;

namespace UnityEditor.XR.MagicLeap
{
    static class MagicLeapProjectSettingsUIElementsRegister
    {
        private const string styleSheetPath = "Packages/com.magicleap.unitysdk/Editor/SettingsProviders/MagicLeapProjectSettings/MagicLeapProjectSettingsStyle.uss";

        [SettingsProvider]
        public static SettingsProvider CreateMagicLeapProjectSettingsProvider()
        {
            var provider = new SettingsProvider("MagicLeap/Audio", SettingsScope.Project)
            {
                label = "Audio",
                activateHandler = (searchContext, rootElement) =>
                {
                    var settings = MagicLeapProjectSettings.GetSerializedSettings();

                    var styleSheet = AssetDatabase.LoadAssetAtPath<StyleSheet>(styleSheetPath);
                    rootElement.styleSheets.Add(styleSheet);
                    var title = new Label()
                    {
                        text = "Audio"
                    };

                    title.AddToClassList("title");
                    rootElement.Add(title);

                    var properties = new VisualElement()
                    {
                        style =
                        {
                            flexDirection = FlexDirection.Column
                        }
                    };
                    properties.AddToClassList("property-list");
                    rootElement.Add(properties);

                    properties.Add(new PropertyField(settings.FindProperty("enableMLAudio")));

                    rootElement.Bind(settings);
                },

                keywords = new HashSet<string>(new[] { "magic leap", "audio", })
            };

            return provider;
        }
    }

    public class MagicLeapProjectSettingsProvider : SettingsProvider
    {
        private SerializedObject magicLeapProjectSettings;

        const string magicLeapProjectSettingsPath = "Assets/MagicLeapSettings.asset";

        public MagicLeapProjectSettingsProvider(string path, SettingsScope scopes, IEnumerable<string> keywords = null) : base(path, scopes, keywords)
        { }

        public static bool IsSettingsAvailable() => File.Exists(magicLeapProjectSettingsPath);

        public override void OnActivate(string searchContext, VisualElement rootElement)
        {
            magicLeapProjectSettings = MagicLeapProjectSettings.GetSerializedSettings();
        }

        public override void OnGUI(string searchContext)
        {
            EditorGUILayout.PropertyField(magicLeapProjectSettings.FindProperty("enableMLAudio"));
            magicLeapProjectSettings.ApplyModifiedPropertiesWithoutUndo();
        }

        [SettingsProvider]
        public static SettingsProvider CreateMyCustomSettingsProvider()
        {
            if (IsSettingsAvailable())
            {
                var provider = new MagicLeapProjectSettingsProvider("Project/MagicLeap", SettingsScope.Project);

                provider.keywords = GetSearchKeywordsFromGUIContentProperties<Styles>();
                return provider;
            }

            return null;
        }
    }
}
