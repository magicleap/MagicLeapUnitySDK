// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEditor;
using System.IO;
using System;

namespace MagicLeap
{
    [InitializeOnLoad]
    public sealed class MagicLeapSDKUtil
    {
        private const string kManifestPath = ".metadata/sdk.manifest";
        private const string kMagicLeapSDKRoot = "MagicLeapSDKRoot";
        private const UnityEditor.BuildTarget kBuildTarget = BuildTarget.Android;
        private static uint minApiLevel = 0;

        [Serializable]
        private class SDKManifest
        {
            public string schemaVersion = null;
            public string label = null;
            public string version = null;
            public string mldb = null;
        }

        /// <summary>
        /// If the MLSDK path is set in the editor preferences and points to a valid sdk.
        /// </summary>
        public static bool SdkAvailable
        {
            get
            {
                var path = SdkPath;
                if (string.IsNullOrEmpty(path))
                    return false;
                return Directory.Exists(path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar));
            }
        }

        public static uint MinimumApiLevel
        {
            get
            {
                if(minApiLevel == 0)
                {
                    try
                    {
                        var result = UnityEngine.XR.MagicLeap.Native.MagicLeapNativeBindings.MLUnitySdkGetMinApiLevel(out minApiLevel);
                        UnityEngine.XR.MagicLeap.MLResult.DidNativeCallSucceed(result, nameof(UnityEngine.XR.MagicLeap.Native.MagicLeapNativeBindings.MLUnitySdkGetMinApiLevel));
                    }
                    catch(DllNotFoundException)
                    {
                        Debug.LogWarning($"Native plugins have not been built for host ({Application.platform}). Minimum API level can't be queried.");
                    }
                }
                return minApiLevel;
            }
        }

        /// <summary>
        /// MLSDK path for the android target.
        /// </summary>
        public static string SdkPath
        {
            get { return GetSDKPath(kBuildTarget); }
            set { SetSDKPath(kBuildTarget, value); }
        }

        /// <summary>
        /// MLSDK version
        /// </summary>
        public static Version SdkVersion => new Version(JsonUtility.FromJson<SDKManifest>(File.ReadAllText(Path.Combine(SdkPath, kManifestPath))).version);

        /// <summary>
        /// Get the MLSDK path based on provided command-line argument or environment variable.
        /// </summary>
        private static string GetSDKPath(BuildTarget target)
        {
            if (target == BuildTarget.Android)
            {
                string path = GetMLSDKCmdLineArg();
                if (string.IsNullOrEmpty(path))
                {
                    path = SessionState.GetString(kMagicLeapSDKRoot, null);
                    if (string.IsNullOrEmpty(path))
                    {
                        // We now only ever need the MLSDK path when running some TestRunner tests and this is generally only
                        // done in CI by passing a "-mlsdk" argument to the Unity command line. Therefore the call to GetMLSDKCmdLineArg()
                        // above should be enough to take care of this. In the rare event someone is trying to run the tests locally in the editor,
                        // this makes the assumption it's a developer with the common "MLSDK" environment variable set on their dev environment
                        path = Environment.GetEnvironmentVariable("MLSDK");
                    }
                }
                return path;                
            }
            string targetPath = SessionState.GetString(target.ToString() + "SDKRoot", kMagicLeapSDKRoot);
            if (string.IsNullOrEmpty(targetPath))
            {
                Debug.LogWarningFormat("MagicLeapSDKUtil couldn't determine SDK path for BuildTarget {0}.", target.ToString());
            }
            return targetPath;
        }

        private static string GetMLSDKCmdLineArg()
        {
            string path = "";
            var args = new System.Collections.Generic.List<string>(Environment.GetCommandLineArgs());
            var mlsdkParam = args.IndexOf("-mlsdk");
            if (mlsdkParam >= 0 && mlsdkParam < args.Count - 1)
            {
                path = args[mlsdkParam + 1].Replace("\\", "/");
                Debug.Log("GetSDKPath got -mlsdk path from command line args: " + path);
            }
            return path;
        }

        private static void SetSDKPath(BuildTarget target, string path)
        {
            if (target == BuildTarget.Android)
            {
                SessionState.SetString(kMagicLeapSDKRoot, path);
            }
            else
            {
                SessionState.SetString(target.ToString() + "SDKRoot", path);
            }
        }
    }
}
