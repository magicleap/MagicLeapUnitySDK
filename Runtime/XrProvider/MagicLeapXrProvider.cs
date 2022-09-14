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
        public const string ImageTrackingSubsystemId = "MagicLeapXr-ImageTracking";
        public const string RaycastSubsystemId = "MagicLeapXr-Raycast";
        public const string AnchorSubsystemId = "MagicLeapXr-Anchors";

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
                MagicLeapXrProviderNativeBindings.UnityMagicLeap_SetLibraryPath(loaderLibPath);

                if (librarySearchPaths.Count > 0)
                {
                    foreach (string librarySearchPath in librarySearchPaths)
                    {
                        MagicLeapXrProviderNativeBindings.MagicLeapXrProviderAddLibraryPath(librarySearchPath);
                    }

                    packagePath = Path.GetFullPath("Packages/com.unity.xr.magicleap");
                    loaderLibPath = Path.Combine(packagePath, "Runtime", LibHostDirForXRPackage);
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
        private static void AddSubsystemOverrides()
        {
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRInputSubsystem>(InputSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRMeshSubsystem>(MeshingSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRGestureSubsystem>(GestureSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRSessionSubsystem>(SessionSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRPlaneSubsystem>(PlanesSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRImageTrackingSubsystem>(ImageTrackingSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRRaycastSubsystem>(RaycastSubsystemId);
            MagicLeapSettings.Subsystems.RegisterSubsystemOverride<XRAnchorSubsystem>(AnchorSubsystemId);
        }
    }
}
