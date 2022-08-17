using System.Collections.Generic;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.XR.MagicLeap;

namespace UnityEditor.XR.MagicLeap
{
    internal static class MLPreferencesTabProvider
    {
        private static readonly string SdkPathEditorPrefsKey = "MagicLeapSDKRoot";
        private static readonly string PathToUIAsset = "Packages/com.magicleap.unitysdk/Editor/SettingsProviders/Preferences/MLPreferencesTab.uxml";

        private static TextField sdkInputField;
        private static Button sdkPathBrowseBtn;
        private static VisualElement sdkPathHelpContainer;
        private static HelpBox sdkPathHelp;

#if UNITY_2022_2_OR_NEWER
        [SettingsProvider]
#endif
        public static SettingsProvider CreateMLPreferencesTabProvider()
        {
            var provider = new SettingsProvider("Preferences/External Tools/Magic Leap", SettingsScope.User)
            {
                label = "Magic Leap",

                // activateHandler is called when the user clicks on the Settings item in the Settings window.
                activateHandler = (searchContext, rootElement) =>
                {
                    var visualTree = AssetDatabase.LoadAssetAtPath<VisualTreeAsset>(PathToUIAsset);

                    visualTree.CloneTree(rootElement);

                    sdkInputField = rootElement.Q<TextField>("SDKPath-Input");
                    sdkInputField.value = GetSavedSDKPath();
                    sdkInputField.RegisterValueChangedCallback(SdkPathChanged);

                    sdkPathBrowseBtn = rootElement.Q<Button>("SDKPath-Button");
                    sdkPathBrowseBtn.clicked += SdkPathBrowseBtn_clicked;

                    sdkPathHelpContainer = rootElement.Q<VisualElement>("SDKPath-Help");

                    sdkPathHelp = new HelpBox("testing help message", HelpBoxMessageType.None);
                    sdkPathHelpContainer.Add(sdkPathHelp);
                    sdkPathHelpContainer.visible = false;

                    CheckPathValid(sdkInputField.value);
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Magic", "Leap", "SDK", "MLSDK", "Lumin" })
            };

            return provider;
    }

        private static void SdkPathChanged(ChangeEvent<string> textFieldChangedEvt)
        {
            if (CheckPathValid(textFieldChangedEvt.newValue))
            {
                SaveNewSDKPath(textFieldChangedEvt.newValue);
            }
        }

        private static void SdkPathBrowseBtn_clicked()
        {
            var startingPath = Path.IsPathFullyQualified(sdkInputField.value) ? Path.GetFullPath(sdkInputField.value) : Directory.GetCurrentDirectory();
            if (!Directory.Exists(startingPath))
                startingPath = "";
            string path = EditorUtility.OpenFolderPanel("Select Magic Leap SDK", startingPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                sdkInputField.value = path;
            }
        }

        private static bool CheckPathValid(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                sdkPathHelp.text = "Magic Leap SDK path not set!";
                sdkPathHelp.messageType = HelpBoxMessageType.Error;
                sdkPathHelpContainer.visible = true;
                return false;
            }
            else
            {
                var manifest = Path.Combine(path, ".metadata", "sdk.manifest");
                bool isValid = File.Exists(manifest);
                if (isValid)
                {
                    sdkPathHelpContainer.visible = false;
                    return true;
                }
                else
                {
                    sdkPathHelp.text = "Selected path does not contain a valid Magic Leap SDK!";
                    sdkPathHelp.messageType = HelpBoxMessageType.Error;
                    sdkPathHelpContainer.visible = true;
                    return false;
                }
            }
        }

        private static string GetSavedSDKPath()
        {
            var path = EditorPrefs.GetString(SdkPathEditorPrefsKey, "");
            if (!string.IsNullOrEmpty(path))
                return path;

            // don't break for users who spent time developing for "Relish" platform
            string relishSdkPath = EditorPrefs.GetString("RelishSDKRoot", null);
            if (!string.IsNullOrEmpty(relishSdkPath))
                return relishSdkPath;

            return EditorPrefs.GetString("LuminSDKRoot", "");
        }

        private static void SaveNewSDKPath(string path)
        {
            if (CheckPathValid(path))
            {
                EditorPrefs.SetString(SdkPathEditorPrefsKey, path);
            }
        }
    }
}
