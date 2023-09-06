// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
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
#if UNITY_OPENXR_1_7_0_OR_NEWER
using UnityEngine.XR.OpenXR;
#endif

namespace UnityEditor.XR.MagicLeap
{
    public static class AppSimShimLibSupport
    {
        private static string LaunchProcess => Path.Combine(MagicLeapSDKUtil.AppSimRuntimePath, "bin/ZIDiscovery");

        public static readonly string SessionStateKey_ZISearchPaths = "ZI_SEARCH_PATHS";
        private static List<string> libSearchPaths = new List<string>();

        /// <summary>
        /// Gets called before the XR Loader starts initializing all the subsystems.
        /// Runs ZIDiscovery.exe to get the list of folders where the ZI shim
        /// libs reside and sends them to the SDK loader lib.
        /// </summary>
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void CheckForLibrarySearchPaths()
        {
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(BuildTargetGroup.Standalone);
            if (settings == null || settings.Manager == null )
            {
                return;
            }

            bool foundSupportedLoader = false;
#if UNITY_XR_MAGICLEAP_PROVIDER
            foundSupportedLoader = settings.Manager.activeLoaders.Any(l => l is MagicLeapLoader);
#elif UNITY_OPENXR_1_7_0_OR_NEWER
            foundSupportedLoader = settings.Manager.activeLoaders.Any(l => l is OpenXRLoader);
#endif

            if (!foundSupportedLoader)
            {
                Debug.LogError("No supported XR loader found for AppSim");
                return;
            }

            string cachedSearchPaths = SessionState.GetString(SessionStateKey_ZISearchPaths, string.Empty);
            if (string.IsNullOrEmpty(cachedSearchPaths))
            {
                var ziRuntime = MagicLeapSDKUtil.AppSimRuntimePath;
                if (string.IsNullOrEmpty(ziRuntime))
                {
                    Debug.LogError("Zero Iteration Runtime path is not set.");
                    SettingsService.OpenUserPreferences("Preferences/External Tools/Magic Leap");
                    return;
                }

#if UNITY_EDITOR_WIN
                ziRuntime = ziRuntime.Replace("/", "\\");
#endif
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    UseShellExecute = false,
                    FileName = LaunchProcess,
                    Arguments = $"-m\"{MagicLeapSDKUtil.SdkPath}\" -p",
                    CreateNoWindow = true,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true
                };

                var discoveryProc = new System.Diagnostics.Process();
                discoveryProc.EnableRaisingEvents = true;
                discoveryProc.StartInfo = startInfo;

                discoveryProc.Start();

                StreamReader outputStream = discoveryProc.StandardOutput;

                string output = outputStream.ReadToEnd();

                discoveryProc.WaitForExit();

                if (discoveryProc.ExitCode != 0)
                {
                    StreamReader errorStream = discoveryProc.StandardError;
                    var error = errorStream.ReadToEnd();
                    Debug.LogError($"ZIDiscovery returned with code {discoveryProc.ExitCode}: {error}");
                    return;
                }

                libSearchPaths = new List<string>(output.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.RemoveEmptyEntries));
                SessionState.SetString(SessionStateKey_ZISearchPaths, string.Join(Path.PathSeparator, libSearchPaths));
            }
            else
            {
                libSearchPaths = new List<string>(cachedSearchPaths.Split(Path.PathSeparator));
            }

            MagicLeapXrProvider.AddLibrarySearchPaths(libSearchPaths, settings.Manager.activeLoaders);
        }
    }
}
