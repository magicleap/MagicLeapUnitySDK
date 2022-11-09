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
using UnityEditor;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.InteractionSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    public class MagicLeapXrProvider
    {
        public const string InputSubsystemId = "MagicLeapXr-Input";
        public const string MeshingSubsystemId = "MagicLeapXr-Meshing";
        public const string GestureSubsystemId = "MagicLeapXr-Gesture";
        public const string SessionSubsystemId = "MagicLeapXr-Session";
        public const string PlanesSubsystemId = "MagicLeapXr-Planes";
        public const string AnchorSubsystemId = "MagicLeapXr-Anchors";
        public const string ImageTrackingSubsystemId = "MagicLeapXr-ImageTracking";
        public const string RaycastSubsystemId = "MagicLeapXr-Raycast";

        public static bool IsZIRunning { get; private set; } = false;

        private static string LibHostDir
        {
            get
            {
#if UNITY_EDITOR_WIN
                return "WindowsEditor";
#elif UNITY_EDITOR_OSX
                return "MacEditor";
#else
                throw new NotSupportedException("Not supported on this platform!");
#endif
            }
        }

        private static string LibHostDirForXRPackage
        {
            get
            {
#if UNITY_EDITOR_WIN
                return "Windows";
#elif UNITY_EDITOR_OSX
                bool isAppleSiliconEditor = SystemInfo.processorType.Contains("Apple") && !EditorUtility.IsRunningUnderCPUEmulation();
                return isAppleSiliconEditor ? "macOS_arm64" : "macOS_x64";
#else
                throw new NotSupportedException("Not supported on this platform!");
#endif
            }
        }

        private static string LibHostDirForXRPackageUntil700Exp4
        {
            get
            {
#if UNITY_EDITOR_WIN
                return "Windows";
#elif UNITY_EDITOR_OSX
                return "macOS";
#else
                throw new NotSupportedException("Not supported on this platform!");
#endif
            }
        }

        public static void AddLibrarySearchPaths(List<string> librarySearchPaths)
        {
#if UNITY_EDITOR
            bool isZIRunning = false;

            try
            {
                MagicLeapXrProviderNativeBindings.MLSdkLoaderUnloadAllLibraries();

                // always do this so that xr package always uses the sdk loader lib.
                string packagePath = Path.GetFullPath("Packages/com.magicleap.unitysdk");
                string loaderLibPath = Path.Combine(packagePath, "Plugins", LibHostDir);
                if (!Directory.Exists(loaderLibPath))
                {
                    Debug.LogError($"Could not find host library path {loaderLibPath}. Some ZI features, like graphics, may not work correctly.");
                    return;
                }

                MagicLeapXrProviderNativeBindings.UnityMagicLeap_SetLibraryPath(loaderLibPath);

                if (librarySearchPaths.Count > 0)
                {
                    foreach (string librarySearchPath in librarySearchPaths)
                    {
                        MagicLeapXrProviderNativeBindings.MagicLeapXrProviderAddLibraryPath(librarySearchPath);
                    }

                    packagePath = Path.GetFullPath("Packages/com.unity.xr.magicleap");
                    loaderLibPath = Path.Combine(packagePath, "Runtime", LibHostDirForXRPackage);
                    if (!Directory.Exists(loaderLibPath))
                    {
                        // We need to do keep separate paths for versions until 7.0.0-exp.4 and later in this manner
                        // instead of checking for xr package veresion because xr package mainline is still marked as
                        // 7.0.0-exp.3 even though exp.4 is out and there is no guarantee what the next version would
                        // be called. Best to check if the path exists and set the loader path accordingly.
                        loaderLibPath = Path.Combine(packagePath, "Runtime", LibHostDirForXRPackageUntil700Exp4);
                        if (!Directory.Exists(loaderLibPath))
                        {
                            Debug.LogError($"Could not find host library path {loaderLibPath}. Some App Sim features, like graphics, may not work correctly.");
                            return;
                        }
                    }

                    MagicLeapXrProviderNativeBindings.MagicLeapXrProviderAddLibraryPath(loaderLibPath);

                    var resultCode = MagicLeapXrProviderNativeBindings.MLZIIsServerConfigured(out isZIRunning);
                    if (!MLResult.DidNativeCallSucceed(resultCode, nameof(MagicLeapXrProviderNativeBindings.MLZIIsServerConfigured)) || !isZIRunning)
                    {
                        Debug.LogError("Failed to detect running Magic Leap App Simulator session.");
                        MagicLeapXrProviderNativeBindings.MLSdkLoaderResetLibraryPaths();
                    }

                    IsZIRunning = isZIRunning;
                }
            }
            catch (Exception e)
            {
                Debug.LogError($"Adding library search paths failed with error {e.Message}");
            }
#if !(UNITY_MAGICLEAP || UNITY_ANDROID)
            if (isZIRunning)
            {
                Debug.LogError("When using ML App Sim you should be in Android build target instead of current build target.");
                EditorApplication.isPlaying = false;
            }
#endif
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void SetupLoaderLibPathInXRPackage()
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            // AddLibrarySearchPaths() is only invoked by AppSimShimLibSupport.cs and contains more complex logic to setting the various
            // lib search paths which isnt really required when running on device. On device, we need to simply set a dummy path in the
            // xr package (because on android we don't need to dlopen() on full lib paths, all valid paths are already in LD_LIBRARY_PATH
            // env var) so that it loads the sdk loader lib enabling us to intercept ml_graphics funcs.
            Debug.Log("Setting loading lib path in xr package");
            MagicLeapXrProviderNativeBindings.UnityMagicLeap_SetLibraryPath("some/dummy/path");
#endif
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        private static void AddSubsystemOverrides()
        {
            // IMPORTANT!! Override any and all perception related subsystems that are instantiated by
            // the XR package's MagicLeapLoader class, even if we dont actually provide an impl of those
            // subsystems ourselves (stubbed or not). Otherwise the XR package will instantiate its own impl
            // of that subsytem, which will try to instantiate the perception system and try to acquire
            // snapshots and query transforms from them etc etc. And in case of any failures, it will shutdown
            // the perception system which will affect the functioning of perception apis in our own subsystems.

            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRInputSubsystem>(InputSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRMeshSubsystem>(MeshingSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRGestureSubsystem>(GestureSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRSessionSubsystem>(SessionSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRPlaneSubsystem>(PlanesSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRAnchorSubsystem>(AnchorSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRImageTrackingSubsystem>(ImageTrackingSubsystemId);
            // Override the RaycastSubsystem with something other than what the XR package provides even if we don't
            // provide an implementation of it ourselves, otherwise the XR package will instantiate its own impl of this
            // and that kicks of new calls to MLPerceptionStartup() (via PerceptionHandle.Acquire()) and when it fails to
            // initialize the raycast api (because its not supported on the OS), it destroys the subsystem which ends up
            // calling MLPercpetionShutdown() (via PerceptionHandle.Release()) and all hell breaks loose because the
            // Percpetion system was shutdown without the knowledge of MagicLeapXrProvider and subsequent api calls from
            // our own subsystems fail.
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRRaycastSubsystem>(RaycastSubsystemId);
        }
    }
}
