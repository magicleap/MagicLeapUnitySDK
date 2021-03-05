using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

namespace UnityEditor.XR.MagicLeap
{
    public class MPKBuilder
    {
        private static readonly string Arg_Development = "--development";
        private static readonly string Arg_Shipping = "--shipping";
        private static readonly string Arg_OutDir = "--outdir";
        private static readonly string Arg_OneMpkPerScene = "--one-mpk-per-scene";
        private static readonly string Arg_CheckProLicense = "--check-pro-license";

        private class BuildSettingsCache
        {
            private string appIdentifier;
            private string productName;
            private UnityEditor.BuildTargetGroup seletedBuildTargetGroup;
            private bool signPackage;
            private bool isPackageDebuggable;

            public BuildSettingsCache()
            {
                appIdentifier = PlayerSettings.applicationIdentifier;
                productName = PlayerSettings.productName;
                seletedBuildTargetGroup = EditorUserBuildSettings.selectedBuildTargetGroup;
                signPackage = PlayerSettings.Lumin.signPackage;
                isPackageDebuggable = UnityEditor.Lumin.UserBuildSettings.isPackageDebuggable;
            }

            public void RestoreSettings()
            {
                PlayerSettings.applicationIdentifier = appIdentifier;
                PlayerSettings.productName = productName;
                EditorUserBuildSettings.selectedBuildTargetGroup = seletedBuildTargetGroup;
                PlayerSettings.Lumin.signPackage = signPackage;
                UnityEditor.Lumin.UserBuildSettings.isPackageDebuggable = isPackageDebuggable;
            }

            ~BuildSettingsCache()
            {
                RestoreSettings();
            }
        }

        public static void BuildMPK()
        {
            MPKBuilder mpkBuilder = new MPKBuilder();
            mpkBuilder.Build();
        }

        private void Build()
        {
            BuildSettingsCache buildSettingsCache = new BuildSettingsCache();

            try
            {
                if (IsArgSet(Arg_CheckProLicense) && !UnityEngine.Application.HasProLicense())
                {
                    throw new System.Exception($"{Arg_CheckProLicense} was passed but Unity Pro License not found!");
                }

                EditorUserBuildSettings.selectedBuildTargetGroup = BuildTargetGroup.Lumin;
                PlayerSettings.Lumin.signPackage = true;
                UnityEditor.Lumin.UserBuildSettings.isPackageDebuggable = !IsArgSet(Arg_Shipping);

                if (IsArgSet(Arg_OneMpkPerScene))
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
                if (!scene.enabled)
                {
                    continue;
                }

                activeScenes.Add(scene.path);
            }

            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.target = BuildTarget.Lumin;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Lumin;
            buildPlayerOptions.options = GetBuildOptions();
            buildPlayerOptions.scenes = activeScenes.ToArray();
            buildPlayerOptions.locationPathName = System.IO.Path.Combine(GetBuildFolder().FullName, $"{PlayerSettings.applicationIdentifier}.mpk");

            UnityEditor.Build.Reporting.BuildReport report = UnityEditor.BuildPipeline.BuildPlayer(buildPlayerOptions);
            if (report.summary.result == UnityEditor.Build.Reporting.BuildResult.Failed)
            {
                throw new System.Exception($"Building {PlayerSettings.applicationIdentifier} failed.");
            }
        }

        private void BuildSceneByScene()
        {
            BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
            buildPlayerOptions.target = BuildTarget.Lumin;
            buildPlayerOptions.targetGroup = BuildTargetGroup.Lumin;
            buildPlayerOptions.options = GetBuildOptions();

            System.IO.FileInfo buildFolder = GetBuildFolder();

            string cachedAppIdentifier = PlayerSettings.applicationIdentifier;

            EditorBuildSettingsScene[] scenes = EditorBuildSettings.scenes;
            foreach (EditorBuildSettingsScene scene in scenes)
            {
                if (!scene.enabled)
                {
                    continue;
                }

                string sceneName = System.IO.Path.GetFileNameWithoutExtension(scene.path);
                PlayerSettings.applicationIdentifier = $"{cachedAppIdentifier}.{sceneName.ToLower()}";
                PlayerSettings.productName = sceneName;
                buildPlayerOptions.locationPathName = System.IO.Path.Combine(buildFolder.FullName, $"{PlayerSettings.applicationIdentifier}.mpk");
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

            System.IO.FileInfo buildFolder = new System.IO.FileInfo(System.IO.Path.Combine(outDir, BuildTarget.Lumin.ToString()));
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
    }
}
