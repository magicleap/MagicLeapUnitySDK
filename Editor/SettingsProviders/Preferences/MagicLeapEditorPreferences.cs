using System.Collections.Generic;
using UnityEngine.UIElements;
using System.IO;
using UnityEngine.XR.MagicLeap;
using System.Diagnostics;
using System.Threading.Tasks;
using System;

namespace UnityEditor.XR.MagicLeap
{
    internal static class MagicLeapEditorPreferences
    {
        private static readonly string SdkPathEditorPrefsKey = "MagicLeapSDKRoot";
        private static readonly string UserCustomZIBackendPath = "ZI_User_Selected_ZI_Runtime";
        private static readonly string LabdriverFoundBackendPath = "ZI_Labdriver_Found_ZI_Runtime";
        private static readonly string ToggleZIPathEnableOverride = "ZI_Path_Enable_Override";

        private static readonly string PathToUIAsset = "Packages/com.magicleap.unitysdk/Editor/SettingsProviders/Preferences/MagicLeapEditorPreferences.uxml";

        private static TextField sdkInputField;
        private static TextField ziInputField;
        private static Button sdkPathBrowseBtn;
        private static Button ziPathBrowseBtn;
        private static VisualElement sdkPathHelpContainer;
        private static VisualElement ziPathHelpContainer;
        private static HelpBox sdkPathHelp;
        private static HelpBox ziPathHelp;
        private static Toggle ziPathOverrideToggle;

        private static bool labdriverRunning = false;
        private static int progressId = -1;

        private static string mlsdkPath = "";
        private static string ziRuntimePath;
        private static string labdriverResultPath = "";
        private static bool enableOverrideZiPath;
        private static bool usingLabdriverFoundPath = false;

        public static string ZeroIterationRuntimePath => ziRuntimePath;

        public static bool RunningLabdriver => labdriverRunning;

        public static event Action<string> ZIRuntimePathChangeEvt;

        [InitializeOnLoad]
        class PreferencesLoader
        {
            static PreferencesLoader()
            {
                var path = GetSavedSDKPath();
                if (Directory.Exists(path))
                {
                    mlsdkPath = path;
                }
                enableOverrideZiPath = EditorPrefs.GetBool(ToggleZIPathEnableOverride);
                if (enableOverrideZiPath)
                {
                    path = EditorPrefs.GetString(UserCustomZIBackendPath);
                    if (Directory.Exists(path))
                    {
                        ziRuntimePath = path;
                    }
                }
                else
                {
                    LocateZIRuntimeFromMLSDK();
                }

                EditorApplication.update += () =>
                {
                    if (!string.IsNullOrEmpty(labdriverResultPath))
                    {
                        SessionState.SetString(LabdriverFoundBackendPath, labdriverResultPath);
                        if (ziRuntimePath != labdriverResultPath)
                        {
                            ziRuntimePath = labdriverResultPath;
                            ZIRuntimePathChangeEvt?.Invoke(ziRuntimePath);
                        }
                        labdriverResultPath = "";
                    }
                    if (labdriverRunning)
                    {
                        Progress.Report(progressId, 1f, "Searching for latest ML App Sim installation...");
                    }
                    else
                    {
                        progressId = Progress.Remove(progressId);
                    }
                };
            }
        }

        [SettingsProvider]
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

                    sdkInputField = rootElement.Q<TextField>("MLSDK-Input");
                    ziInputField = rootElement.Q<TextField>("ZI-Input");
                    sdkPathBrowseBtn = rootElement.Q<Button>("MLSDK-Browse");
                    ziPathBrowseBtn = rootElement.Q<Button>("ZI-Browse");
                    ziPathOverrideToggle = rootElement.Q<Toggle>("ZI-Override");
                    sdkPathHelpContainer = rootElement.Q<VisualElement>("SDKPath-Help");
                    ziPathHelpContainer = rootElement.Q<VisualElement>("ZIPath-Help");


                    sdkPathHelp = new HelpBox("", HelpBoxMessageType.None);
                    sdkPathHelpContainer.Add(sdkPathHelp);
                    sdkPathHelpContainer.visible = false;

                    ziPathHelp = new HelpBox("", HelpBoxMessageType.Info);
                    ziPathHelpContainer.Add(ziPathHelp);

                    sdkInputField.SetValueWithoutNotify(mlsdkPath);
                    sdkInputField.RegisterValueChangedCallback(SdkPathChanged);

                    bool enableOverride = EditorPrefs.GetBool(ToggleZIPathEnableOverride, false);
                    ziPathOverrideToggle.value = enableOverride;
                    ziInputField.SetEnabled(enableOverride);
                    ziPathBrowseBtn.SetEnabled(enableOverride);
                    ziPathOverrideToggle.RegisterValueChangedCallback(ZIOverrideToggleChanged);

                    ziInputField.RegisterValueChangedCallback(ZIPathChanged);
                    ziInputField.value = ziRuntimePath;
                    sdkPathBrowseBtn.clicked += SdkPathBrowseBtn_clicked;
                    ziPathBrowseBtn.clicked += ZIPathBrowseBtn_clicked;
                    
                    if(progressId != -1)
                    {
                        progressId = Progress.Remove(progressId);
                    }
                },

                // This is behavior for updating the UI of the window iff the Preferences window is open
                inspectorUpdateHandler = () =>
                {
                    if (!labdriverRunning)
                    {
                        CheckPathValidZI(ziRuntimePath);
                        if (ziRuntimePath != ziInputField.value)
                        {
                            ziInputField.value = ziRuntimePath;

                            if (!string.IsNullOrEmpty(ziRuntimePath))
                            {
                                if (!Directory.Exists(ziRuntimePath))
                                {
                                    SetZIHelpBoxStatusLocateFailed();
                                }
                                else
                                {
                                    SetZIHelpBoxStatusLocateSuccess();
                                }
                            }
                        }
                    }
                },

                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "Magic", "Leap", "SDK", "MLSDK", "Lumin" }),
            };

            return provider;
        }

        private static void ZIOverrideToggleChanged(ChangeEvent<bool> evt)
        {
            enableOverrideZiPath = evt.newValue; 
            EditorPrefs.SetBool(ToggleZIPathEnableOverride, enableOverrideZiPath);
            ziInputField.SetEnabled(enableOverrideZiPath);
            ziPathBrowseBtn.SetEnabled(enableOverrideZiPath);
            if (!enableOverrideZiPath)
            {
                if(!usingLabdriverFoundPath)
                {
                    ziInputField.SetValueWithoutNotify("");
                    ziRuntimePath = "";
                }
                ziPathHelp.text = "Searching for ML App Sim runtime...";
                ziPathHelp.messageType = HelpBoxMessageType.None;
                LocateZIRuntimeFromMLSDK();
            }
        }

        private static void ZIPathBrowseBtn_clicked()
        {
            string startingPath;
            if (string.IsNullOrEmpty(ziInputField.value))
            {
                startingPath = Directory.GetCurrentDirectory();
            }
            else
            {
                startingPath = Path.IsPathFullyQualified(ziInputField.value) ? Path.GetFullPath(ziInputField.value) : Directory.GetCurrentDirectory();
            }

            if(!Directory.Exists(startingPath))
            {
                startingPath = "";
            }
            string path = EditorUtility.OpenFolderPanel("Select ML App Sim Runtime Install Path", startingPath, "");
            if(!string.IsNullOrEmpty(path))
            {
                ziInputField.value = path;
            }
        }

        private static void ZIPathChanged(ChangeEvent<string> textFieldChangedEvt)
        {
            if (ziPathOverrideToggle.value)
            {
                usingLabdriverFoundPath = false;
                ziRuntimePath = textFieldChangedEvt.newValue;
                if (CheckPathValidZI(textFieldChangedEvt.newValue))
                {
                    ZIRuntimePathChangeEvt?.Invoke(ziRuntimePath);
                    EditorPrefs.SetString(UserCustomZIBackendPath, textFieldChangedEvt.newValue);
                }
            }

            MagicLeapEditorPreferencesProvider.OnZIPathChanged?.Invoke(textFieldChangedEvt.newValue);
        }

        private static void SdkPathChanged(ChangeEvent<string> textFieldChangedEvt)
        {
            usingLabdriverFoundPath = false;
            if (CheckPathValidSDK(textFieldChangedEvt.newValue))
            {
                SaveNewSDKPath(textFieldChangedEvt.newValue);
            }

            MagicLeapEditorPreferencesProvider.OnSDKPathChanged?.Invoke(textFieldChangedEvt.newValue);
        }

        private static void SdkPathBrowseBtn_clicked()
        {
            var startingPath = Path.IsPathFullyQualified(sdkInputField.value) ? Path.GetFullPath(sdkInputField.value) : Directory.GetCurrentDirectory();
            if (!Directory.Exists(startingPath))
            {
                startingPath = "";
            }
            string path = EditorUtility.OpenFolderPanel("Select Magic Leap SDK", startingPath, "");
            if (!string.IsNullOrEmpty(path))
            {
                sdkInputField.value = path;
            }
        }

        private static bool CheckPathValidSDK(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                if (!ziPathOverrideToggle.value)
                {
                    SetZIHelpBoxStatusLocateFailed();
                    ziInputField.SetValueWithoutNotify("");
                }
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
                    if (!ziPathOverrideToggle.value)
                    {
                        SetZIHelpBoxStatusLocateFailed();
                        ziInputField.SetValueWithoutNotify("");
                    }
                    return false;
                }
            }
        }

        private static bool CheckPathValidZI(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                SetZIHelpBoxStatusLocateFailed();
                return false;
            }
            else
            {
                var dll = Path.Combine(path, "zif", "scripting", "csharp", "com.magicleap.zif.dll");
                var bin = Path.Combine(path, "bin");
                var lib = Path.Combine(path, "lib");
                if(File.Exists(dll) && Directory.Exists(bin) && Directory.Exists(lib))
                {
                    SetZIHelpBoxStatusLocateSuccess();
                    return true;
                }
                else
                {
                    ziPathHelp.text = "Path is not a valid ML App Sim Runtime!";
                    ziPathHelp.messageType = HelpBoxMessageType.Error;
                    return false;
                }
            }
        }

        private static string GetSavedSDKPath()
        {
            var path = EditorPrefs.GetString(SdkPathEditorPrefsKey, "");
            if (!string.IsNullOrEmpty(path))
            {
                return path;
            }

            // fallback to edge case where developer previously used a Relish build of Unity
            return EditorPrefs.GetString("RelishSDKRoot", "");
        }

        private static void SaveNewSDKPath(string path)
        {
            SessionState.EraseString(LabdriverFoundBackendPath);
            EditorPrefs.SetString(SdkPathEditorPrefsKey, path);
            mlsdkPath = path;
            if (!ziPathOverrideToggle.value)
            {
                if(!usingLabdriverFoundPath)
                {
                    ziInputField.SetValueWithoutNotify("");
                    ziRuntimePath = "";
                }
                ziPathHelp.text = "Searching for ML App Sim runtime...";
                ziPathHelp.messageType = HelpBoxMessageType.None;
                LocateZIRuntimeFromMLSDK();
            }
        }

        private static void LocateZIRuntimeFromMLSDK()
        {
            var path = SessionState.GetString(LabdriverFoundBackendPath, "");
            if(Directory.Exists(path))
            {
                if (path != ziRuntimePath)
                {
                    ziRuntimePath = path;
                    ZIRuntimePathChangeEvt?.Invoke(ziRuntimePath);
                }
            }

            if(string.IsNullOrEmpty(ziRuntimePath) || !Directory.Exists(ziRuntimePath))
            {
                if(mlsdkPath.EndsWith("LAYOUT"))
                {
                    path = Path.Combine(mlsdkPath, "VirtualDevice");
                    if(path != ziRuntimePath)
                    {
                        ziRuntimePath = path;
                        ZIRuntimePathChangeEvt?.Invoke(ziRuntimePath);
                    }
                    usingLabdriverFoundPath = false;
                }
                else
                {
                    if (!labdriverRunning)
                    {
                        labdriverRunning = true;
                        progressId = Progress.Start("Searching for ML App Sim", options: Progress.Options.Indefinite);
                        Task.Run(LabdriverGetBackendPathAsync);
                    }
                }
            }
        }

        private static async void LabdriverGetBackendPathAsync()
        {
            var startInfo = new ProcessStartInfo
            {
#if UNITY_EDITOR_WIN
                FileName = "CMD.exe",
                Arguments = string.Format(@"/C {0}\labdriver.cmd -raw com.magicleap.zi:get-backend-path", mlsdkPath.Replace("/", "\\")),
#elif UNITY_EDITOR_OSX
                RedirectStandardInput = true,
                FileName = "/bin/bash",
                Arguments = string.Format("{0}/labdriver -raw com.magicleap.zi:get-backend-path", mlsdkPath),
#endif
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            var process = new Process
            {
                EnableRaisingEvents = true,
                StartInfo = startInfo
            };

            await Task.Run(() =>
            {
                process.Start();
                StreamReader outputStream = process.StandardOutput;
                string output = outputStream.ReadToEnd();
                process.WaitForExit();
                labdriverResultPath = output.Trim().Replace("\\", "/");
                usingLabdriverFoundPath = true;
                labdriverRunning = false;
            });
        }

        private static void SetZIHelpBoxStatusLocateFailed()
        {
            if (enableOverrideZiPath)
            {
                if (string.IsNullOrEmpty(ziInputField.value))
                {
                    ziPathHelp.text = "Without a ML App Sim runtime specified, you will not be able to connect to a simulator from Unity.";
                    ziPathHelp.messageType = HelpBoxMessageType.Error;
                }
                else
                {
                    ziPathHelp.text = "The specified path does not contain a valid ML App Sim runtime installation. You will not be able to connect to a simulator from Unity.";
                    ziPathHelp.messageType = HelpBoxMessageType.Error;
                }
            }
            else
            {
                ziPathHelp.text = "Unable to locate an installation of the ML App Sim runtime. Be sure to install it using Magic Leap Hub and then set your Magic Leap SDK path above.";
                ziPathHelp.messageType = HelpBoxMessageType.Warning;
            }
        }

        private static void SetZIHelpBoxStatusLocateSuccess()
        {
            if (enableOverrideZiPath)
            {
                ziPathHelp.text = "ML App Sim Runtime detected.";
                ziPathHelp.messageType = HelpBoxMessageType.Info;
            }
            else
            {
                if (mlsdkPath.EndsWith("LAYOUT") && ziRuntimePath.EndsWith("VirtualDevice"))
                {
                    ziPathHelp.text = "Using development App Sim runtime from built LAYOUT!";
                    ziPathHelp.messageType = HelpBoxMessageType.Warning;
                }
                else
                {
                    ziPathHelp.text = "App Simualtor Runtime detected.";
                    ziPathHelp.messageType = HelpBoxMessageType.Info;
                }
            }
        }
    }
}
