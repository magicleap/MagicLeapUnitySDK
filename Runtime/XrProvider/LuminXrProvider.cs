// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="LuminXrProvider.cs" company="Magic Leap, Inc">
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
using UnityEditor;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.InteractionSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    public class LuminXrProvider
    {
        public const string InputSubsystemId = "LuminXr-Input";
        public const string MeshingSubsystemId = "LuminXr-Meshing";
        public const string GestureSubsystemId = "LuminXr-Gesture";
        public const string SessionSubsystemId = "LuminXr-Session";
        public const string PlanesSubsystemId = "LuminXr-Planes";
        public const string ImageTrackingSubsystemId = "LuminXr-ImageTracking";
        public const string RaycastSubsystemId = "LuminXr-Raycast";
        public const string AnchorSubsystemId = "LuminXr-Anchors";

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
                LuminXrProviderNativeBindings.MLSdkLoaderUnloadAllLibraries();

                // always do this so that xr package always uses the sdk loader lib.
                string packagePath = Path.GetFullPath("Packages/com.magicleap.unitysdk");
                string loaderLibPath = Path.Combine(packagePath, "Plugins", LibHostDir);
                LuminXrProviderNativeBindings.UnityMagicLeap_SetLibraryPath(loaderLibPath);

                if (librarySearchPaths.Count > 0)
                {
                    foreach (string librarySearchPath in librarySearchPaths)
                    {
                        LuminXrProviderNativeBindings.LuminXrProviderAddLibraryPath(librarySearchPath);
                    }

                    packagePath = Path.GetFullPath("Packages/com.unity.xr.magicleap");
                    loaderLibPath = Path.Combine(packagePath, "Runtime", LibHostDirForXRPackage);
                    LuminXrProviderNativeBindings.LuminXrProviderAddLibraryPath(loaderLibPath);

                    var resultCode = LuminXrProviderNativeBindings.MLZIIsServerConfigured(out isZIRunning);
                    if (!MLResult.DidNativeCallSucceed(resultCode, nameof(LuminXrProviderNativeBindings.MLZIIsServerConfigured)) || !isZIRunning)
                    {
                        Debug.LogError("Failed to detect running Zero Iteration session.");
                        LuminXrProviderNativeBindings.MLSdkLoaderResetLibraryPaths();
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
                Debug.LogError("When using ZI you should be in Android build target instead of current build target.");
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
