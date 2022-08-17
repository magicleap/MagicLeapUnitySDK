// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="ZeroIterationShimLibSupport.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.Management;
using UnityEditor.XR.Management;

namespace UnityEditor.XR.MagicLeap
{
    public static class ZeroIterationShimLibSupport
    {
        private class JSONObject
        {
            public List<JSONResult> results = null;
            public bool success;
        }

        [Serializable]
        private class JSONResult
        {
            public List<string> output = null;
            public List<string> error = null;
        }

        private static string LaunchProcess
        {
            get
            {
#if UNITY_EDITOR_WIN
                return "cmd.exe";
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                return "/bin/bash";
#else
                throw new NotSupportedException("Not supported on this platform!");
#endif
            }
        }

        private static readonly string SessionStateKey = "ZI_SEARCH_PATHS_LABDRIVER_RESULT";
        private static List<string> libSearchPaths = new List<string>();

        /// <summary>
        /// Gets called before the XR Loader starts initializing all the subsystems.
        /// Runs the labdriver command to get the list of folders where the ZI shim
        /// libs reside and sends them to the SDK loader lib.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void CheckForLibrarySearchPaths()
        {
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
            if (settings == null || settings.Manager == null || settings.Manager.activeLoaders.Where(l => l is MagicLeapLoader).Count() == 0)
            {
                return;
            }

            string cachedSearchPaths = SessionState.GetString(SessionStateKey, string.Empty);
            if (string.IsNullOrEmpty(cachedSearchPaths))
            {
                var path = MagicLeapSDKUtil.SdkPath;
                if (string.IsNullOrEmpty(path))
                {
                    Debug.LogError("Magic Leap SDK path is not set. You must specify a Magic Leap SDK to detect a Zero Iteration instance.");
                    SettingsService.OpenUserPreferences("Preferences/External Tools/Magic Leap");
                    return;
                }

                bool isLayout = path.EndsWith("LAYOUT");

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = LaunchProcess,
#if UNITY_EDITOR_WIN
                    Arguments = $"/C \"\"{path}\\labdriver.cmd\" -pretty com.magicleap.zi:get-shim-search-paths -release -sdk=\"{path}\"\"",
#elif UNITY_EDITOR_OSX || UNITY_EDITOR_LINUX
                    Arguments = $"\"{path}/labdriver\" -pretty com.magicleap.zi:get-shim-search-paths -release -sdk=\"{path}\"",
#endif
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                if (isLayout && !startInfo.EnvironmentVariables.ContainsKey("ML_ZI_ROOT"))
                {
                    startInfo.EnvironmentVariables.Add("ML_ZI_ROOT", path + "/VirtualDevice");
                }

                System.Diagnostics.Process labdriverProcess = new System.Diagnostics.Process();
                labdriverProcess.EnableRaisingEvents = true;
                labdriverProcess.StartInfo = startInfo;

                labdriverProcess.Start();

                StreamReader outputStream = labdriverProcess.StandardOutput;

                // This forces a sync and exits once the process has finished
                // and the entire output is available. This is both good and bad.
                // Good because we need the output to by synchronous and be available
                // before proceeding for the play-in-editor session.
                // Bad because labdriver is slowwwwww.
                string output = outputStream.ReadToEnd();

                if (labdriverProcess.ExitCode != 0)
                {
                    StreamReader errorStream = labdriverProcess.StandardError;
                    var error = errorStream.ReadToEnd();
                    Debug.LogError($"labdriver exited with return code {labdriverProcess.ExitCode}. Please make sure Zero Iteration is installed in The Lab 2.0.\nOutput:\n{output}\nError:\n{error}");
                    return;
                }

                JSONObject fullLog = JsonUtility.FromJson<JSONObject>(output);

                if (fullLog.success)
                {
                    libSearchPaths = fullLog.results[0].output;
                    SessionState.SetString(SessionStateKey, string.Join(Path.PathSeparator, libSearchPaths));
                }
                else
                {
                    Debug.LogError($"Json parsing for labdriver result failed. Labdriver output was {output}");
                }

                labdriverProcess.WaitForExit();
            }
            else
            {
                libSearchPaths = new List<string>(cachedSearchPaths.Split(Path.PathSeparator));
            }

            LuminXrProvider.AddLibrarySearchPaths(libSearchPaths);
        }
    }
}
