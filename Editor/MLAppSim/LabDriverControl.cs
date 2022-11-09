using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

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
        internal static Action<bool, string> s_LabdriverOnComplete = null;

        private static int progressId;

        private static Process s_Process;

        internal class JSONObject
        {
            public List<JSONResult> results = null;
            public bool success;
        }

        [Serializable]
        internal class JSONResult
        {
            public List<string> output = null;
            public List<string> error = null;
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
            GUILayout.Label("Magic Leap App Simulator Requirements", EditorStyles.boldLabel);

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
                EditorUtility.DisplayProgressBar("Magic Leap Hub", "Issuing command to Magic Leap Hub...", 1f);
            }
            else
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= ShowProgressDialog;
            }
        }

        internal static void LaunchLabDriver(List<string> args, Action<bool, string> onComplete, bool importCommand = false, bool useVirtualDevice = false)
        {
            EditorApplication.update += ShowProgressDialog;

            s_LabdriverIsRunning = true;
            s_LabdriverExit = false;
            s_LabdriverOutputDone = false;
            s_LabdriverLog.Clear();

            var startInfo = new ProcessStartInfo
            {
                UseShellExecute = false,
#if UNITY_EDITOR_WIN
                FileName = "cmd.exe",
#else // OSX or Linux
                FileName = "/bin/sh",
#endif
                CreateNoWindow = true,
                RedirectStandardOutput = true,
                RedirectStandardError = true
            };

            // copy arguments into new-style ArgumentList, which handles spaces properly
#if UNITY_EDITOR_WIN
            // i.e. "cmd /C labdriver ..." runs this command rather than an interactive shell
            startInfo.ArgumentList.Add("/C");
#endif
            foreach (string arg in args)
            {
                startInfo.ArgumentList.Add(arg);
            }

            if (useVirtualDevice)
            {
                if (!startInfo.EnvironmentVariables.ContainsKey("ML_ZI_ROOT"))
                {
                    startInfo.EnvironmentVariables.Add("ML_ZI_ROOT", MagicLeapSDKUtil.SdkPath + "/VirtualDevice");
                }
            }

            s_Process = new System.Diagnostics.Process();
            s_Process.EnableRaisingEvents = true;
            s_Process.OutputDataReceived += OnOutputReceived;
            s_Process.Exited += OnProcessExit;
            s_Process.ErrorDataReceived += OnErrorReceived;
            s_Process.StartInfo = startInfo;

            s_LabdriverIsImport = importCommand;
            s_LabdriverOnComplete = onComplete;
            s_LabdriverLog.Clear();
            s_LabdriverErrorLog.Clear();

            progressId = Progress.Start("Running labdriver process");

            s_Process.Start();
            s_Process.BeginOutputReadLine();
            s_Process.BeginErrorReadLine();
        }

        private static void LaunchLabDriverCommand(List<string> commands, Action<bool, string> onComplete)
        {
            var sdkPath = MagicLeapSDKUtil.SdkPath;
            if (string.IsNullOrEmpty(sdkPath))
            {
                UnityEngine.Debug.LogError("Magic Leap SDK path not configured!");
                return;
            }
            else if (!File.Exists(Path.Combine(sdkPath, "labdriver")))
            {
                UnityEngine.Debug.LogErrorFormat("labdriver executable not found in configured SDK path \"{0}\"! Make sure the path is valid.", sdkPath);
                return;
            }
            if (!s_LabdriverIsRunning)
            {
                UnityEngine.Debug.Log("Launching labdriver with: " + string.Join(" ", commands));
#if UNITY_EDITOR_WIN
                commands.InsertRange(0, new List<string>{$"{sdkPath}/labdriver.cmd", "-pretty"});
#else // OSX or Linux
                commands.InsertRange(0, new List<string>{$"{sdkPath}/labdriver", "-pretty"});
#endif
                LaunchLabDriver(commands, onComplete);
            }
            else
            {
                UnityEngine.Debug.Log("Previous Magic Leap Hub command is still running. Please wait until it completes.");
            }
        }

        private static void HandleLabDriverResult(bool success, string json)
        {
            if (!success)
            {
                UnityEngine.Debug.LogError("Magic Leap Hub command failed:\n" + json);
            }
        }

        [MenuItem("Magic Leap/Launch Magic Leap Hub")]
        private static void LaunchHub()
        {
            LaunchLabDriverCommand(new List<string>{"start-gui"}, HandleLabDriverResult);
        }

        [MenuItem("Magic Leap/Save Diagnostic Logs...")]
        private static void SaveLogs()
        {
            bool result = EditorUtility.DisplayDialog("Privacy Notice",
                String.Concat(
                    "Create an error report file (.zip) to help us diagnose problems.\n\n",
                    "(Note: if you are using a Magic Leap device, please connect it now.)\n\n",
                    "Ask a question in the Magic Leap Developer Portal (https://developer.magicleap.cloud/support) " +
                    "and attach the error report .zip file.\n\n",
                    "Error reports are public, and the .zip file may contain identifying information, so you should inspect the .zip before sending.\n\n",
                    "See https://developer-docs.magicleap.cloud/docs/guides/developer-tools/ml-hub/error-reporting"
                ), "Ok", "Cancel");
            if (!result)
            {
                return;
            }

            string dateTime = DateTime.Now.ToString("yyyyMMddHHmmss");
            string tempFile = $"MLHubLogs-{dateTime}.zip";
            string tempFileDir = Path.GetTempPath();
            string tempFilePath = Path.Combine(tempFileDir, tempFile);

            void OpenLogFile(bool success, string json)
            {
                if (!success)
                {
                    HandleLabDriverResult(success, json);
                    return;
                }

                // reveal file in explorer/finder
                ProcessStartInfo startInfo = null;
#if UNITY_EDITOR_OSX
                startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = "/usr/bin/open",
                    ArgumentList = { "-R", tempFilePath },
                    CreateNoWindow = true
                };
#elif UNITY_EDITOR_WIN
                startInfo = new ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = "explorer.exe",
                    ArgumentList = { "/select,", tempFilePath },    // the embedded comma is needed
                    CreateNoWindow = true
                };
#endif
                if (startInfo != null) 
                {
                    var process = new System.Diagnostics.Process();
                    process.StartInfo = startInfo;
                    process.Start();
                }
            }

            LaunchLabDriverCommand(new List<string>{"save-logs", tempFilePath}, OpenLogFile);
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
                        IEnumerable<string> ZILibraries = fullLog.results[0].output;

                        // TODO : Lib discovery is no longer run via LabDriverControl.
                        // Refactor this class to remove unnecesarry code.
                        //ZeroIterationImportSupport.DiscoveryReturned(ZILibraries);
                    }

                    Progress.Remove(progressId);

                    if (s_LabdriverOnComplete != null)
                    {
                        // success
                        s_LabdriverOnComplete(true, finalResult);
                    }
                }
                else
                {
                    String finalError = s_LabdriverErrorLog.ToString();

                    Progress.Remove(progressId);

                    if (s_LabdriverOnComplete != null)
                    {
                        // failure
                        s_LabdriverOnComplete(false, finalError);
                    }
                }

                s_LabdriverIsImport = false;
                s_LabdriverOnComplete = null;
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
                s_LabdriverLog.Append(' ');
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
                s_LabdriverErrorLog.Append(' ');
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
                String currentAction = s_LabdriverIsImport ? "import support libraries" : "launch Magic Leap App Simulator";
                logToPrint = String.Format("Magic Leap Hub encountered an unknown error while attempting to {0}. " +
                    "Please confirm Magic Leap Hub in installed and up to date with the Magic Leap App Simulator Module and " +
                    "Magic Leap App Simulator Runtime package installed.\n", currentAction) + logToPrint;
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
