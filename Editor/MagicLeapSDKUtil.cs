// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using System.IO;
using System;

namespace UnityEditor.XR.MagicLeap
{
    [InitializeOnLoad]
    public sealed class MagicLeapSDKUtil
    {
        private const string kManifestPath = ".metadata/sdk.manifest";
        private const string kMagicLeapSDKRoot = "MagicLeapSDKRoot";
#if UNITY_2022_2_OR_NEWER
        private const UnityEditor.BuildTarget kBuildTarget = BuildTarget.Android;
#else
        private const UnityEditor.BuildTarget kBuildTarget = BuildTarget.Relish;
#endif
        private static uint minApiLevel = 0;

        static MagicLeapSDKUtil()
        {
            try
            {
                var result = UnityEngine.XR.MagicLeap.Native.MagicLeapNativeBindings.MLUnitySdkGetMinApiLevel(out minApiLevel);
                UnityEngine.XR.MagicLeap.MLResult.DidNativeCallSucceed(result, nameof(UnityEngine.XR.MagicLeap.Native.MagicLeapNativeBindings.MLUnitySdkGetMinApiLevel));
            }
            catch(Exception e)
            {
                Debug.LogError($"Failed looking up minimum API level for Magic Leap SDK due to exception: {e}"); 
            }
        }

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
                if (string.IsNullOrEmpty(SdkPath))
                    return false;
                return File.Exists(Path.Combine(SdkPath, kManifestPath));
            }
        }

        public static uint MinimumApiLevel => minApiLevel;

        /// <summary>
        /// MLSDK path for the relish target.
        /// </summary>
        public static string SdkPath
        {
            get { return GetSDKPath(kBuildTarget); }
            set { SetSDKPath(kBuildTarget, value); }
        }

        public static string AppSimRuntimePath => MagicLeapEditorPreferences.ZeroIterationRuntimePath;
        public static bool SearchingForZI => MagicLeapEditorPreferences.RunningLabdriver;
        public static event Action<string> OnZeroIterationPathChanged
        {
            add { MagicLeapEditorPreferences.ZIRuntimePathChangeEvt += value; }
            remove { MagicLeapEditorPreferences.ZIRuntimePathChangeEvt -= value; }
        }

        /// <summary>
        /// MLSDK version
        /// </summary>
        public static Version SdkVersion => new Version(JsonUtility.FromJson<SDKManifest>(File.ReadAllText(Path.Combine(SdkPath, kManifestPath))).version);

        public static void DeleteSDKPathFromEditorPrefs(BuildTarget target)
        {
            if (target == BuildTarget.Android)
            {
                EditorPrefs.DeleteKey(kMagicLeapSDKRoot);
            }
            else
            {
                EditorPrefs.DeleteKey(target.ToString() + "SDKRoot");
            }
        }

        /// <summary>
        /// Get the MLSDK path for the given build target platform.
        /// </summary>
        /// <param name="target">Relish is the only valid target for now.</param>
        /// <returns></returns>
        private static string GetSDKPath(BuildTarget target)
        {
            if (target == BuildTarget.Android)
            {
                string path = GetMLSDKCmdLineArg();
                if (string.IsNullOrEmpty(path))
                {
                    path = EditorPrefs.GetString(kMagicLeapSDKRoot, null);
                }
                return path;                
            }
            string targetPath = EditorPrefs.GetString(target.ToString() + "SDKRoot");
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
                EditorPrefs.SetString(kMagicLeapSDKRoot, path);
            }
            else
            {
                EditorPrefs.SetString(target.ToString() + "SDKRoot", path);
            }
        }
    }
}
