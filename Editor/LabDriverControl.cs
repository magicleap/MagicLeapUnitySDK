using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.Rendering;

namespace UnityEditor.XR.MagicLeap
{
    public class LabDriverControl : EditorWindow
    {
        private IMGUIContainer _remoteChecksUi;
        private VisualElement _mainVisualContainer;

        private string[] _availablePackages = new string[] { };

        internal static StringBuilder s_LabdriverLog = new StringBuilder();
        internal static StringBuilder s_LabdriverErrorLog = new StringBuilder();

        internal static bool s_LabdriverIsRunning = false;
        internal static bool s_LabdriverExit = false;
        internal static bool s_LabdriverOutputDone = false;
        internal static bool s_LabdriverIsImport = false;

        private static int progressId;

        private static Process s_Process;

        internal class JSONObject
        {
            public List<JSONResult> results;
            public string version;
            public bool success;
        }

        [Serializable]
        internal class JSONResult
        {
            public string token;
            public string state;
            public string startTime;
            public string endTime;
            public string commandName;
            public string commandArgs;
            public List<string> output;
            public List<string> error;
            public List<JSONSubCommand> subCommands;
        }

        [Serializable]
        internal class JSONSubCommand
        {
            public string token;
            public string state;
            public string startTime;
            public string endTime;
            public string commandName;
            public string commandArgs;
            public List<string> output;
            public List<string> error;
        }

        private void OnDisable()
        {
        }

        private void OnEnable()
        {
            _remoteChecksUi = new IMGUIContainer(OnRemoteChecksUI);
            _mainVisualContainer = new VisualElement()
            {
                name = "MainVisualContainer"
            };
            _mainVisualContainer.Add(_remoteChecksUi);
            var root = this.rootVisualElement;
            root.Add(_mainVisualContainer);

            _availablePackages = MagicLeapPackageLocator.GetUnityPackages().ToArray();
        }

        private void OnGUI()
        {
            
        }

        private void OnRemoteChecksUI()
        {
            GUILayout.Label("Zero Iteration Requirements", EditorStyles.boldLabel);

            using (new GUILayout.HorizontalScope())
            {
                GUILayout.Label("Rendering API:");
                GUILayout.Label(SystemInfo.graphicsDeviceType.ToString(), GUI.skin.textField);
                using (new EditorGUI.DisabledScope(!NeedToSwitchToGLCore))
                {
                    if (GUILayout.Button("Restart w/ OpenGL"))
                    {
                        if (EditorUtility.DisplayDialog("Editor Restart Required",
                            string.Format(
                                "To use Magic Leap Zero Iteration mode in the editor, the editor must restart using OpenGL."),
                            "Restart", "Do Not Restart"))
                        {
                            Restart("-force-glcore");
                        }
                    }
                }
            }

            using (new GUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Import MagicLeap unitypackage"))
                {
                    var rect = GUILayoutUtility.GetLastRect();
                    var versions = new GenericMenu();
                    foreach (var pkg in _availablePackages)
                    {
                        versions.AddItem(new GUIContent(pkg), false, InstallPackage, pkg);
                    }
                    // show options as a drop down.
                    versions.DropDown(rect);
                }

            }
        }

        //[MenuItem("Window/XR/MagicLeap Dashboard", false, 1)]
        private static void Display()
        {
            // Get existing open window or if none, make a new one:
            EditorWindow.GetWindow<LabDriverControl>(false, "ML Dashboard").Show();
        }

        private void InstallPackage(object p)
        {
            var path = p as string;
            UnityEngine.Debug.LogFormat("Importing: {0}", path);
            AssetDatabase.ImportPackage(path, true);
        }

        // EditorUtility.DisplayProgressbar must be called from the main thread
        // Typically you would call it from OnGUI, but this class may not have its own GUI calls
        // so instead we place it in the Editor's global update ticks.
        private static void ShowProgressDialog()
        {
            if (s_LabdriverIsRunning)
            {
                EditorUtility.DisplayProgressBar("Zero Iteration - Import Support Libraries", "labdriver is running...", 1f);
            }
            else
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= ShowProgressDialog;
            }
        }

        internal static void LaunchProcess(string filename, string args = "", bool importCommand = false, bool useVirtualDevice = false)
        {
            EditorApplication.update += ShowProgressDialog;

            s_LabdriverIsRunning = true;
            s_LabdriverExit = false;
            s_LabdriverOutputDone = false;
            s_LabdriverLog.Clear();

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
                FileName = filename,
                Arguments = args,
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            if(useVirtualDevice)
            {
                startInfo.EnvironmentVariables.Add("ML_VDS_DATA", ZeroIterationImportSupport.sdkPath + "/VirtualDevice/data");
            }

            s_Process = new System.Diagnostics.Process();
            s_Process.EnableRaisingEvents = true;
            s_Process.OutputDataReceived += OnOutputReceived;
            s_Process.Exited += OnProcessExit;
            s_Process.ErrorDataReceived += OnErrorReceived;
            s_Process.StartInfo = startInfo;

            s_LabdriverIsImport = importCommand;

            progressId = Progress.Start("Running labdriver process");

            s_Process.Start();
            s_Process.BeginOutputReadLine();
            s_Process.BeginErrorReadLine();
        }

        [MenuItem("Magic Leap/Zero Iteration/Launch Zero Iteration")]
        private static void LaunchRemote()
        {
            if (!s_LabdriverIsRunning)
            {
                UnityEngine.Debug.Log("Launching labdriver process.");
#if UNITY_EDITOR_OSX
                // MacOSX - ZI Launch
                LaunchProcess("/bin/bash", $"\"{ZeroIterationImportSupport.sdkPath}/labdriver\" -pretty go-zi-module");
#elif UNITY_EDITOR_WIN
                // Windows - ZI Launch
                LaunchProcess(Path.Combine(ZeroIterationImportSupport.sdkPath, "labdriver.cmd"), "-pretty go-zi-module");
#endif
            }
            else
            {
                UnityEngine.Debug.Log("Previous labdriver process is still running. Please wait until it completes.");
            }
        }

        [MenuItem("Magic Leap/Zero Iteration/Launch Zero Iteration", true)]
        private static bool CanLaunchRemote()
        {
            if ((EditorUserBuildSettings.activeBuildTarget == BuildTarget.Lumin) &&
                !string.IsNullOrWhiteSpace(MagicLeapRemoteImportSupport.sdkPath) &&
                File.Exists(Path.Combine(MagicLeapRemoteImportSupport.sdkPath, "labdriver")))
            {
                return true;
            }

            return false;
        }

        private static void Restart(params string[] args)
        {
            EditorApplication.OpenProject(ProjectPath, args);
        }

        private static string ProjectPath
        {
            get { return Path.GetDirectoryName(Application.dataPath); }
        }

        private static bool NeedToSwitchToGLCore
        {
            get { return SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore; }
        }

        private static void WaitForComplete()
        {
            // Confirm the process has exited and the output has completed before processing logs.
            if (s_LabdriverExit && s_LabdriverOutputDone)
            {
                s_Process.CancelErrorRead();
                s_Process.CancelOutputRead();
                String finalResult = s_LabdriverLog.ToString();

                s_LabdriverIsRunning = false;

                Progress.Report(progressId, 1.0f);

                if (!String.IsNullOrEmpty(finalResult))
                {
                    JSONObject fullLog = JsonUtility.FromJson<JSONObject>(finalResult);

                    if (!fullLog.success)
                    {
                        ProcessLogs(fullLog);
                    }
                    else if (s_LabdriverIsImport)
                    {
                        IEnumerable<string> ZILibraryLocations = fullLog.results[0].output;

                        ZeroIterationImportSupport.DiscoveryReturned(ZILibraryLocations);
                    }

                    Progress.Remove(progressId);
                }
                else
                {
                    String finalError = s_LabdriverErrorLog.ToString();

                    if (String.IsNullOrEmpty(finalError))
                    {
                        String currentAction = s_LabdriverIsImport ? "import support libraries" : "launch ZI";
                        finalError = String.Format("Magic Leap's The Lab encountered an unknown error while attempting to {0}. " +
                            "Please confirm The Lab in installed and up to date with the Zero Iteration Module and Zero Iteration " +
                            "Runtime package installed.", currentAction);
                    }

                    Progress.Remove(progressId);

                    UnityEngine.Debug.LogError(finalError);
                }

                s_LabdriverIsImport = false;
            }
        }

        private static void OnOutputReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                s_LabdriverLog.Append(e.Data);

                string statusCheck = s_LabdriverLog.ToString();

                if ((statusCheck.Substring(statusCheck.Length - 1, 1) == "}") && statusCheck.Contains("\"success\":"))
                {
                    s_LabdriverOutputDone = true;
                    WaitForComplete();
                }
            }
            else
            {
                if (!s_LabdriverOutputDone)
                {
                    s_LabdriverOutputDone = true;
                    WaitForComplete();
                }
            }
        }

        private static void OnErrorReceived(object sender, DataReceivedEventArgs e)
        {
            if (!String.IsNullOrEmpty(e.Data))
            {
                s_LabdriverErrorLog.Append(e.Data);
            }
        }

        private static void OnProcessExit(object sender, EventArgs e)
        {
            s_LabdriverExit = true;

            WaitForComplete();
        }

        private static void ProcessLogs(JSONObject currentLogs)
        {
            string logToPrint = "";

            foreach (JSONResult result in currentLogs.results)
            {
                foreach (string item in result.error)
                {
                    if (!String.IsNullOrEmpty(item))
                    {
                        logToPrint += "\n" + item;
                    }
                }
            }

            if (String.IsNullOrEmpty(logToPrint) || s_LabdriverIsImport)
            {
                String currentAction = s_LabdriverIsImport ? "import support libraries" : "launch ZI";
                logToPrint = String.Format("The Lab encountered an unknown error while attempting to {0}. " +
                    "Please confirm The Lab in installed and up to date with the Zero Iteration Module and Zero Iteration " +
                    "Runtime package installed.\n", currentAction) + logToPrint;
            }

            String finalLog = "labdriver completed with errors. \nErrors:" + logToPrint;

            UnityEngine.Debug.LogError(finalLog);
        }
    }


    internal static class MagicLeapPackageLocator
    {
        public static IEnumerable<string> GetUnityPackages()
        {
            var tools = Path.Combine(MagicLeapRoot, "tools");
            return new DirectoryInfo(tools).GetFiles("*.unitypackage", SearchOption.AllDirectories).Select(fi => fi.FullName);
        }

        private static string HomeFolder
        {
            get
            {
                var home = Environment.GetEnvironmentVariable("USERPROFILE");
                return (string.IsNullOrEmpty(home))
                    ? Environment.GetEnvironmentVariable("HOME")
                    : home;
            }
        }

        public static string MagicLeapRoot { get { return Path.Combine(HomeFolder, "MagicLeap"); } }
    }
}
