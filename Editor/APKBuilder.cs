using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace UnityEditor.XR.MagicLeap
{
    public class APKBuilder
    {
        private static readonly string Arg_Development = "--development";
        private static readonly string Arg_OutDir = "--outdir";
        private static readonly string Arg_OneApkPerScene = "--one-apk-per-scene";
        private static readonly string Arg_CheckProLicense = "--check-pro-license";
        private static readonly string Arg_ForceSDKPathFromEnvVar = "--force_sdk_path_from_env_var";

#if !UNITY_2022_2_OR_NEWER
        private static readonly Dictionary<BuildTarget, BuildTargetGroup> BuildTargetToGroup = new Dictionary<BuildTarget, BuildTargetGroup>()
        {
            { BuildTarget.Relish, BuildTargetGroup.Relish }
        };
#endif

        private bool didSetSDKPathFromEnvVar = false;

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

        ~APKBuilder()
        {
            if (this.didSetSDKPathFromEnvVar)
            {
                MagicLeapSDKUtil.DeleteSDKPathFromEditorPrefs(EditorUserBuildSettings.activeBuildTarget);
            }
        }

        private void Build()
        {
#if UNITY_2022_2_OR_NEWER
            if (EditorUserBuildSettings.activeBuildTarget != BuildTarget.Android)
            {
                throw new System.Exception("You must be on the Android Build Target to use APKBuilder.");
            }
#else
            if (!BuildTargetToGroup.ContainsKey(EditorUserBuildSettings.activeBuildTarget))
            {
                throw new System.Exception($"Unsupported build target {EditorUserBuildSettings.activeBuildTarget} for APKBuilder. Only Relish build target is supported.");
            }
#endif

            SetupSDKPaths(System.Array.IndexOf(System.Environment.GetCommandLineArgs(), Arg_ForceSDKPathFromEnvVar) != -1);

            BuildSettingsCache buildSettingsCache = new BuildSettingsCache();

            try
            {
                if (IsArgSet(Arg_CheckProLicense) && !UnityEngine.Application.HasProLicense())
                {
                    throw new System.Exception($"{Arg_CheckProLicense} was passed but Unity Pro License not found!");
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
            List<string> activeScenes = new List<string>();
            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if(string.IsNullOrEmpty(scene.path) || scene == null)
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
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.target = EditorUserBuildSettings.activeBuildTarget;
#if UNITY_2022_2_OR_NEWER
            buildPlayerOptions.targetGroup = BuildTargetGroup.Android;
#else
            buildPlayerOptions.targetGroup = BuildTargetToGroup[EditorUserBuildSettings.activeBuildTarget];
#endif
            buildPlayerOptions.options = GetBuildOptions();
            buildPlayerOptions.scenes = activeScenes.ToArray();
            buildPlayerOptions.locationPathName = System.IO.Path.Combine(GetBuildFolder().FullName, $"{PlayerSettings.applicationIdentifier}.apk");

            UnityEditor.Build.Reporting.BuildReport report = UnityEditor.BuildPipeline.BuildPlayer(buildPlayerOptions);
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

            System.IO.FileInfo buildFolder = new System.IO.FileInfo(System.IO.Path.Combine(outDir, EditorUserBuildSettings.activeBuildTarget.ToString()));
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

        /// <summary>
        /// Magic Leap SDK is required during the build process to include the
        /// correct permissions in AndroidManifest.xml.
        /// </summary>
        /// <param name="bForceSetFromEnvVar"></param>
        private void SetupSDKPaths(bool bForceSetFromEnvVar)
        {
            bool didSetFromEnvVar = false;
            if (bForceSetFromEnvVar || !MagicLeapSDKUtil.SdkAvailable)
            {
                string relishSDKPath = System.Environment.GetEnvironmentVariable("RELISHSDK_UNITY");
                if (string.IsNullOrEmpty(relishSDKPath))
                {
                    Debug.LogWarning("No SDK path found for Relish in editor preferences or RELISHSDK_UNITY environment variable. Build will probably fail OR Magic Leap permissions will not be included in AndroidManifest.xml.");
                }
                else
                {
                    if (bForceSetFromEnvVar)
                    {
                        Debug.LogFormat("{0} was passed as cmd line arg. Force setting Relish SDK Path to {1} from env vars.", Arg_ForceSDKPathFromEnvVar, relishSDKPath);
                    }
                    else
                    {
                        Debug.LogFormat("No SDK path set for Relish in editor preferences. Using {0} from env vars.", relishSDKPath);
                    }
                    MagicLeapSDKUtil.SdkPath = relishSDKPath;
                    didSetFromEnvVar = true;
                }
            }
            else
            {
                Debug.LogFormat("Relish SDK found in editor preferences at {0}", MagicLeapSDKUtil.SdkPath);
            }

            this.didSetSDKPathFromEnvVar = didSetFromEnvVar;
        }
    }
}
