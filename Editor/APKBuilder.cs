// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;
using System;

namespace MagicLeap
{
    public class APKBuilder
    {
        private static readonly string Arg_Development = "--development";
        private static readonly string Arg_OutDir = "--outdir";
        private static readonly string Arg_OneApkPerScene = "--one-apk-per-scene";
        private static readonly string Arg_CheckProLicense = "--check-pro-license";
        private static readonly string Arg_AppVersionCodeValue = "--version-code";
        private static readonly string Arg_EnableFPSlogging = "--log-fps";

        private class BuildSettingsCache
        {
            private string appIdentifier;
            private string productName;
            private UnityEditor.BuildTargetGroup seletedBuildTargetGroup;

            public BuildSettingsCache()
            {
                appIdentifier = PlayerSettings.applicationIdentifier;
                productName = PlayerSettings.productName;
                seletedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
            }

            public void RestoreSettings()
            {
                PlayerSettings.applicationIdentifier = appIdentifier;
                PlayerSettings.productName = productName;
                EditorUserBuildSettings.selectedBuildTargetGroup = seletedBuildTargetGroup;
            }

            ~BuildSettingsCache()
            {
                RestoreSettings();
            }
        }

        public static void BuildAPK()
        {
            APKBuilder apkBuilder = new APKBuilder();
            apkBuilder.Build();
        }

        private void Build()
        {
#if UNITY_2022_2_OR_NEWER
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                throw new System.Exception("You must be on the Android Build Target to use APKBuilder.");
            }
#endif

            BuildSettingsCache buildSettingsCache = new BuildSettingsCache();

            try
            {
                if (IsArgSet(Arg_CheckProLicense) && !UnityEngine.Application.HasProLicense())
                {
                    throw new System.Exception($"{Arg_CheckProLicense} was passed but Unity Pro License not found!");
                }

                string versionCodeArgVal = "0";
                if (TryGetArgValue(Arg_AppVersionCodeValue, ref versionCodeArgVal))
                {
                    if (int.TryParse(versionCodeArgVal, out int versionCode))
                    {
                        PlayerSettings.Android.bundleVersionCode = versionCode;
                    }
                }

                var jsonPath = Path.GetFullPath("Packages/com.magicleap.unitysdk/package.json");
                var jsonText = File.ReadAllText(jsonPath);
                if (string.IsNullOrEmpty(jsonText))
                {
                    Debug.LogWarning($"failed to load SDK package manifest from path {jsonPath}");
                }
                else
                {
                    var packageManifest = JsonUtility.FromJson<SimplePackageManifest>(jsonText);
                    PlayerSettings.bundleVersion = packageManifest.version;
                }

                if (IsArgSet(Arg_OneApkPerScene))
                {
                    BuildSceneByScene();
                }
                else
                {
                    BuildAllScenes();
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            finally
            {
                buildSettingsCache.RestoreSettings();
            }
        }

        private void BuildAllScenes()
        {
            string[] scriptingDefineCache = null;
            List<string> activeScenes = new List<string>();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            int index = 0;
            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if (string.IsNullOrEmpty(scene.path) || scene == null)
                {
                    Debug.LogWarningFormat("[BuildAllScenes] A scene in BuildSettings seems to have been deleted. It should be removed from the BuildSettings list. It won't be included in this build.");
                    continue;
                }
                if (!scene.enabled)
                {
                    Debug.LogWarningFormat("[BuildAllScenes] Excluding scene {0} because it is disabled in BuildSettings", scene.path);
                    continue;
                }
                activeScenes.Add(scene.path);
                index++;
            }

            string apkName = PlayerSettings.applicationIdentifier;
            if (IsArgSet(Arg_Development))
            {
                apkName = $"{apkName}-dev";
            }
            if (IsArgSet(Arg_EnableFPSlogging))
            {
                PlayerSettings.GetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android, out scriptingDefineCache);
                var updatedDefines = new List<string>(scriptingDefineCache) { "ENABLE_FPS_LOGGING" };
                PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android, updatedDefines.ToArray());
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
            buildPlayerOptions.options = GetBuildOptions();
            buildPlayerOptions.scenes = activeScenes.ToArray();
            buildPlayerOptions.locationPathName = System.IO.Path.Combine(GetBuildFolder().FullName, $"{apkName}.apk");

            UnityEditor.Build.Reporting.BuildReport report = UnityEditor.BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (IsArgSet(Arg_EnableFPSlogging) && scriptingDefineCache != null)
            {
                PlayerSettings.SetScriptingDefineSymbols(UnityEditor.Build.NamedBuildTarget.Android, scriptingDefineCache);
            }
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
            {
                throw new System.Exception($"Building {PlayerSettings.applicationIdentifier} failed.");
            }
        }

        private void BuildSceneByScene()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
#if UNITY_2022_2_OR_NEWER
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
#else
            buildPlayerOptions.targetGroup = BuildTargetToGroup[EditorUserBuildSettings.activeBuildTarget];
#endif
            buildPlayerOptions.options = GetBuildOptions();

            System.IO.FileInfo buildFolder = GetBuildFolder();

            string cachedAppIdentifier = PlayerSettings.applicationIdentifier;

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if (string.IsNullOrEmpty(scene.path) || scene == null)
                {
                    Debug.LogWarningFormat("[BuildAllScenes] A scene in BuildSettings seems to have been deleted. It should be removed from the BuildSettings list. It won't be included in this build.");
                    continue;
                }
                if (!scene.enabled)
                {
                    Debug.LogWarningFormat("[BuildAllScenes] Excluding scene {0} because it is disabled in BuildSettings", scene.path);
                    continue;
                }

                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                PlayerSettings.applicationIdentifier = $"{cachedAppIdentifier}.{sceneName.ToLower()}";
                PlayerSettings.productName = sceneName;
                buildPlayerOptions.locationPathName = System.IO.Path.Combine(buildFolder.FullName, $"{PlayerSettings.applicationIdentifier}.apk");
                buildPlayerOptions.scenes = new string[] { scene.path };

                UnityEditor.Build.Reporting.BuildReport report = UnityEditor.BuildPipeline.BuildPlayer(buildPlayerOptions);
                if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
                {
                    throw new System.Exception($"Building {sceneName} failed.");
                }
            }
        }

        private UnityEditor.BuildOptions GetBuildOptions()
        {
            UnityEditor.BuildOptions buildOptions = BuildOptions.None;
            if (IsArgSet(Arg_Development))
            {
                buildOptions |= BuildOptions.Development;
            }

            return buildOptions;
        }

        private System.IO.FileInfo GetBuildFolder()
        {
            string outDir = "Build";
            TryGetArgValue(Arg_OutDir, ref outDir);

            System.IO.FileInfo buildFolder = new System.IO.FileInfo(outDir);
            buildFolder.Directory.Create();

            return buildFolder;
        }

        private bool IsArgSet(string arg)
        {
            return (System.Array.IndexOf(System.Environment.GetCommandLineArgs(), arg) != -1);
        }

        private bool TryGetArgValue(string arg, ref string value)
        {
            int argIndex = System.Array.IndexOf(System.Environment.GetCommandLineArgs(), arg);
            if (argIndex != -1 && argIndex < System.Environment.GetCommandLineArgs().Length - 1)
            {
                value = System.Environment.GetCommandLineArgs()[argIndex + 1];
                return true;
            }

            return false;
        }

        [Serializable]
        private class SimplePackageManifest
        {
            public string name;
            public string displayName;
            public string version;
            public string description;
        }
    }
}
