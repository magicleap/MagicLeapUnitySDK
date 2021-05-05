using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    [Serializable]
    class SDKManifest
    {
        public string schemaVersion = null;
        public string label = null;
        public string version = null;
        public string mldb = null;
    }

    public class ZIImportFailureException : Exception
    {
        const string kHelpMessage = @"Cannot import {0} as the file already exists and is currently locked by Unity.";

        public string HelpMessage
        {
            get
            {
                return string.Format(kHelpMessage, Path, System.IO.Path.GetDirectoryName(Path));
            }
        }

        public string Path { get; }

        public ZIImportFailureException(string path, Exception inner) : base(string.Format(kHelpMessage, path), inner)
        {
            Path = path;
        }
    }

    public static class ZeroIterationImportSupport
    {
        const string kDestinationProjectFolder = "Assets/Plugins/Lumin/Editor/x64";
        const string kManifestPath = ".metadata/sdk.manifest";
        internal const string kShimDiscoveryPath = ".metadata/sdk_shim_discovery.txt";
        const string kShimPathTemplate = "ZI_SHIM_PATH_{0}";
        const string kFallbackShimData = @"
# some comments
STUB_PATH=$(MLSDK)/lib/$(HOST)
# in older builds, ML Remote lives inside MLSDK
MLREMOTE_BASE_0.16.0=$(MLSDK)/VirtualDevice
MLREMOTE_BASE_0.17.0=$(MLSDK)/VirtualDevice
MLREMOTE_BASE_0.18.0=$(MLSDK)/VirtualDevice
# ML Remote is installed in parallel, using the same version as MLSDK
MLREMOTE_BASE_0.19.0=$(MLSDK)/../../MLRemote/v$(MLSDK_VERSION)
# select the appropriate base for the running version
MLREMOTE_BASE=$(MLREMOTE_BASE_$(MLSDK_VERSION))
ZI_SHIM_PATH_win64=$(MLREMOTE_BASE)/lib;$(MLREMOTE_BASE)/bin;$(STUB_PATH)
ZI_SHIM_PATH_osx=$(MLREMOTE_BASE)/lib;$(STUB_PATH)
ZI_SHIM_PATH_linux64=$(MLREMOTE_BASE)/lib/linux64;$(STUB_PATH)
";
        const string kMacOSDependencyPath = "{0}/VirtualDevice/bin";

        private static bool finishedCopyingFiles = false;
        private static bool userCanceledImport = false;
        private static bool importThrewException = false;

        private static string fileBeingCopied;
        private static float progressPercent;
        private static int progressId;

        private static List<string> existingLibsNotDeleted = new List<string>();
        private static List<string> existingLibsReplaced = new List<string>();

        private static Dictionary<string, string> s_PluginLookupCache = new Dictionary<string, string>();

        static ZeroIterationImportSupport()
        {
            EditorApplication.update += new EditorApplication.CallbackFunction(PostImportLibrariesOperation);
        }

        internal static IEnumerable<string> discoveryFileOrFallbackData
        {
            get
            {
                return (hasDiscoveryFile)
                    ? File.ReadAllLines(Path.Combine(sdkPath, kShimDiscoveryPath))
                    : kFallbackShimData.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            }
        }
        public static string host
        {
            get
            {
#if UNITY_EDITOR_WIN
                return "win64";
#elif UNITY_EDITOR_OSX
                return "osx";
#elif UNITY_EDITOR_LINUX
                return "linux64";
#else
                throw new NotSupportedException("Not supported on this platform!");
#endif
            }
        }

        internal static string hostExtension
        {
            get
            {
#if UNITY_EDITOR_WIN
                return ".dll";
#elif UNITY_EDITOR_OSX
                return ".bundle"; // extension checking is handled later for OSX.
#elif UNITY_EDITOR_LINUX
                return "*.so";
#else
                throw new NotSupportedException("Not supported on this platform!");
#endif
            }
        }

        internal static string hostExtensionGlob
        {
            get
            {
#if UNITY_EDITOR_WIN
                return "*.dll";
#elif UNITY_EDITOR_OSX
                return "*.*"; // extension checking is handled later for OSX.
#elif UNITY_EDITOR_LINUX
                return "*.so";
#else
                throw new NotSupportedException("Not supported on this platform!");
#endif
            }
        }

        public static bool TryResolveMLPluginPath(string name, out string path)
        {
            if (!s_PluginLookupCache.TryGetValue(name, out path))
            {
                foreach (var importer in PluginImporter.GetAllImporters().Where(ap => LooksLikeRemoteLibrary(ap.assetPath)))
                {
                    var filename = Path.GetFileNameWithoutExtension(importer.assetPath);
                    if (filename.EndsWith(name))
                    {
                        path = Path.GetFullPath(importer.assetPath);
                        s_PluginLookupCache[name] = path;

                        return true;
                    }
                }
                path = null;
                return false;
            }

            return true;
        }

        static bool LooksLikeRemoteLibrary(string path)
        {
            var fileName = Path.GetFileName(path);
            return fileName.StartsWith("ml_") && fileName.EndsWith(hostExtension);
        }

        internal static bool hasDiscoveryFile
        {
            get
            {
                if (!sdkAvailable) return false;
                return File.Exists(Path.Combine(sdkPath, kShimDiscoveryPath));
            }
        }

        internal static string macOSDependencyPath
        {
            get
            {
                return string.Format(kMacOSDependencyPath, sdkPath);
            }
        }

        internal static string newMacOSDependencyPath;

        internal static bool sdkAvailable
        {
            get
            {
                if (string.IsNullOrEmpty(sdkPath)) return false;
                return File.Exists(Path.Combine(sdkPath, kManifestPath));
            }
        }

        public static string sdkPath
        {
            get
            {
                return Lumin.UserBuildSettings.SDKPath;
            }
        }

        internal static Version sdkVersion
        {
            get => new Version(JsonUtility.FromJson<SDKManifest>(File.ReadAllText(Path.Combine(sdkPath, kManifestPath))).version);
        }

        internal static IEnumerable<string> shimSearchPaths
        {
            get
            {
                return ParseDiscoveryData(discoveryFileOrFallbackData);
            }
        }

        public static string version
        {
            get
            {
                var manifest = Path.Combine(sdkPath, kManifestPath);
                return JsonUtility.FromJson<SDKManifest>(File.ReadAllText(manifest)).version;
            }
        }

        [Obsolete("sdk_shim_discovery.txt should no longer be used. Use LocateZeroIterationLibraries to locate ZI libraries with labdriver.", error: true)]
        public static IEnumerable<string> LocateMLRemoteLibrariesUsingShimPaths()
        {
            return LocateZeroIterationLibraries(shimSearchPaths);
        }

        public static IEnumerable<string> LocateZeroIterationLibraries(IEnumerable<string> libSearchPaths)
        {
            var paths = new HashSet<string>();
            foreach (var dir in libSearchPaths)
            {
                string path = dir;
                if(path.EndsWith("LAYOUT"))
                {
                    path = path + "/VirtualDevice";
                }
                var di = new DirectoryInfo(path);
                if (!di.Exists) 
                    continue;

                var files = di.GetFiles(hostExtensionGlob);
                if(files.Length >0)
                {
                    Debug.Log("Libraries found in " + path);
                }
                foreach (var fi in files)
                {
#if UNITY_EDITOR_OSX
                    var base_name = Path.GetFileNameWithoutExtension(fi.Name);
                    if (!base_name.StartsWith("lib"))
                        base_name = "lib" + base_name;
                    if (paths.Add(base_name))
#else
                    if (paths.Add(fi.Name))
#endif
                        yield return fi.FullName;
                }
            }
        }

        internal static IEnumerable<string> ParseDiscoveryData(IEnumerable<string> lines)
        {
            Dictionary<string, string> vars = new Dictionary<string, string>();
            vars.Add("MLSDK", sdkPath);
            vars.Add("MLSDK_VERSION", version);
            vars.Add("HOST", host);

            foreach (var raw in lines)
            {
                var line = raw.Trim();
                // discard comments
                if (line.StartsWith("#"))
                    continue;

                bool expansionPerformed = false;
                // replace all '$(VAR)' expansions with actual values, if possible.
                do
                {
                    expansionPerformed = false;
                    foreach (var kv in vars)
                    {
                        var v = string.Format("$({0})", kv.Key);
                        var temp = line.Replace(v, kv.Value);
                        if (temp != line)
                        {
                            expansionPerformed = true;
                            line = temp;
                        }
                    }

                } while (expansionPerformed);

                // if we see an equals sign, update the variable list with the new value.
                if (line.IndexOf("=") != -1)
                {
                    var parts = line.Split('=');
                    vars[parts[0].Trim()] = parts[1].Trim();
                }
            }

            var key = string.Format(kShimPathTemplate, host);
            if (!vars.ContainsKey(key))
                throw new Exception(string.Format("'{0}' key not found during shim lookup!", key));
            return vars[key].Split(';');
        }

        internal static string GetOSXTargetPath(string destFolder, string srcPath)
        {
            // we only want to rename the ML libs we load directly, not their dependencies.
            // let's assume for now that all front-facing libs begin with "libml_"
            if (Path.GetFileName(srcPath).StartsWith("libml_"))
            {
                var new_name = Path.GetFileNameWithoutExtension(srcPath) + ".bundle";
                new_name = new_name.TrimStart(new char[] {'l','i','b'});
                return Path.Combine(destFolder, new_name);
            }
            return null;
        }

        internal static bool TryFindDependencyOnSearchPath(string file_name, out string dep_path)
        {
            foreach (var path in shimSearchPaths)
            {
                dep_path = Path.Combine(path, file_name);
                if (File.Exists(dep_path) || Directory.Exists(dep_path)) // need to check directories too, because osx has bundle-type things...
                    return true;
            }
            dep_path = null;
            return false;
        }

        public static void ImportSupportLibraries(string destFolder, IEnumerable<string> ZILibraries)
        {
            existingLibsNotDeleted.Clear();
            isRunning = true;
            Directory.CreateDirectory(destFolder);
            Progress.Report(progressId, .25f);
            foreach (var lib in LocateZeroIterationLibraries(ZILibraries))
            {
                if (userCanceledImport)
                    break;

                fileBeingCopied = lib;
                try
                {
                    //Debug.Log("Copying " + lib);
                    if (Application.platform == RuntimePlatform.OSXEditor)
                    {
                        var target = GetOSXTargetPath(destFolder, lib);
                        if (target == null) continue;
                        CheckIfFileCanBeCopiedAndThrow(lib, target);
                        MacOSDependencyChecker.Migrate(lib, target);
                        var dm = MacOSDependencyChecker.GetDependencies(target);
                        var missing = new List<string>();

                        foreach (var dep in dm.dependencies)
                        {
                            Progress.Report(progressId, .5f);
                            if (userCanceledImport)
                                break;
                            string depRelPath = Path.Combine(Path.GetDirectoryName(target), dep);
                            if (!File.Exists(depRelPath))
                                missing.Add(dep);
                        }

                        foreach (var item in missing)
                        {
                            if (userCanceledImport)
                                break;
                            string dep = item;
                            if (!item.StartsWith("Assets/"))
                            {
                                dep = Path.Combine(Path.GetDirectoryName(target), dep);
                            }
                            var dep_path = Path.GetFullPath(dep);
                            if (!File.Exists(dep_path))
                            {
                                Progress.Report(progressId, .75f);
                                Directory.CreateDirectory(Path.GetDirectoryName(dep_path));
                                var src = Path.Combine(newMacOSDependencyPath, Path.GetFileName(dep));
                                fileBeingCopied = src;
                                if (File.Exists(src))
                                    File.Copy(src, dep_path);
                            }
                        }
                    }
                    else
                    {
                        Progress.Report(progressId, .75f);
                        var filename = Path.GetFileName(lib);
                        var target = Path.Combine(destFolder, filename);
                        if (target == null) continue; // null indicates the file shouldn't be copied.
                        CheckIfFileCanBeCopiedAndThrow(lib, target);
                        File.Copy(lib, target);
                    }
                }
                catch(Exception e)
                {
                    importThrewException = true;
                    Debug.LogError(e.Message);
                }
            }

            // AssetDatabse.Refresh must be called on the main thread
            finishedCopyingFiles = true;
        }

        private static void CheckIfFileCanBeCopiedAndThrow(string src, string dest)
        {
            if (!File.Exists(src))
                throw new Exception(string.Format("Cannot import {0}: file not found", src));
            if (File.Exists(dest))
            {
                try
                {
                    File.Delete(dest);
                    existingLibsReplaced.Add(dest);
                    Debug.LogWarningFormat("Replacing existing {0}", dest);
                }
                catch (Exception e)
                {
                    existingLibsNotDeleted.Add(dest);
                    throw new ZIImportFailureException(dest, e);
                }
            }
        }

        internal static void DiscoveryReturned(IEnumerable<string> libraryLocations)
        {
            progressId = Progress.Start("Import Support Libraries");

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                newMacOSDependencyPath = libraryLocations.First().Replace("/lib", "/bin");
            }

            bool error = false;
            try
            {
                ImportSupportLibraries(kDestinationProjectFolder, libraryLocations);
            }
            catch(Exception e)
            {
                error = true;
                Debug.LogException(e);
            }

            if (error)
            {
                Debug.LogError("Encountered an error importing support libraries.");
            }
            else
            {
                Debug.Log("Finished copying libraries to " + kDestinationProjectFolder);
            }
        }

        private static bool isRunning = false;

        [MenuItem("Magic Leap/Zero Iteration/Import Support Libraries")]
        static void DoImport()
        {
            if (!isRunning)
            {
                Debug.Log("Launching labdriver process....");

                EditorApplication.update += DisplayProgressBar;
                userCanceledImport = false;

                bool isLayout = sdkPath.EndsWith("LAYOUT");

                if (Application.platform == RuntimePlatform.OSXEditor)
                {
                    LabDriverControl.LaunchProcess("/bin/bash", 
                                                $"\"{sdkPath}/labdriver\" -pretty com.magicleap.zi:get-shim-search-paths -release \"{sdkPath}\"", 
                                                importCommand: true,
                                                useVirtualDevice: isLayout);
                }
                else if (Application.platform == RuntimePlatform.WindowsEditor)
                {
                    LabDriverControl.LaunchProcess("cmd /C", 
                                                $"\"{sdkPath}\\labdriver.cmd\" -pretty com.magicleap.zi:get-shim-search-paths -release \"{sdkPath}\"", 
                                                importCommand: true,
                                                useVirtualDevice: isLayout);
                }
            }
            else
            {
                Debug.LogError("Previous labdriver process is still running. Please wait until it completes.");
            }
        }

        [MenuItem("Magic Leap/Zero Iteration/Import Support Libraries", true)]
        static bool CanImport()
        {
            if ((EditorUserBuildSettings.activeBuildTarget == BuildTarget.Lumin) &&
                !string.IsNullOrWhiteSpace(sdkPath) &&
                File.Exists(Path.Combine(sdkPath, "labdriver")))
            {
                return true;
            }

            return false;
        }

        private static void PostImportLibrariesOperation()
        {
            if (finishedCopyingFiles)
            {
                Progress.Report(progressId, 1f);
                isRunning = false;
                finishedCopyingFiles = false;
#if UNITY_EDITOR_LINUX || UNITY_EDITOR_OSX
                if(existingLibsReplaced.Count() > 0)
                {
                    EditorUtility.DisplayDialog("Please Restart Unity", "It is highly recommended that you close and relaunch Unity for the newly imported libraries to be loaded correctly.", "Ok");
                    Debug.LogFormat("The following library files were successfully replaced:\n\t{0}", string.Join("\n\t", existingLibsReplaced));
                }
#endif
                if (userCanceledImport)
                {
                    Debug.LogWarning("The library import process was cancelled.");
                }
                else if(importThrewException)
                {
                    Debug.LogWarning("The library import process encountered one or more file I/O exceptions. Please check error messages.");
                    if(existingLibsNotDeleted.Count() > 0)
                    {
                        Debug.LogErrorFormat("The following library files could NOT be replaced and must be deleted after closing Unity:\n\t{0}", string.Join("\n\t", existingLibsNotDeleted));
                    }
                }
                else
                {
                    AssetDatabase.Refresh();
                    Debug.Log("Finished importing support libraries from Lumin SDK to Assets/Plugins/Lumin/Editor");
                    var pluginFolder = AssetDatabase.LoadAssetAtPath("Assets/Plugins/Lumin/Editor", typeof(UnityEngine.Object));
                    Selection.activeObject = pluginFolder;
                    EditorGUIUtility.PingObject(pluginFolder);
                }
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= DisplayProgressBar;
                Progress.Remove(progressId);
            }
        }

        private static void DisplayProgressBar()
        {
            if (isRunning)
            {
                if (EditorUtility.DisplayCancelableProgressBar("Zero Iteration - Import Support Libraries", "Copying libary file " + fileBeingCopied, Progress.GetProgress(progressId)))
                {
                    userCanceledImport = true;
                }
            }
            else if(userCanceledImport)
            {
                EditorUtility.ClearProgressBar();
                EditorApplication.update -= DisplayProgressBar;
                Progress.Remove(progressId);
            }
        }
    }

    class ZeroIterationLibraryImporter : AssetPostprocessor
    {
        private static bool debugImport = false;

        void OnPreprocessAsset()
        {
            if(debugImport && assetImporter.assetPath.Contains("Plugins/Lumin/Editor") && !string.IsNullOrEmpty(Path.GetExtension(assetImporter.assetPath)))
            {
                Debug.Log("Importing library file " + assetImporter.assetPath);
            }
        }
    }
}
