// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2023-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using MagicLeap;
using System;
using System.IO;
using UnityEngine;

namespace UnitySDKEditorTests
{
    public class MlSdkDllLoader
    {
        static IntPtr nativeLibraryPtr;
    
        public void Load(string lib)
        {
            if (nativeLibraryPtr != IntPtr.Zero) 
                return;

            if (!MagicLeapSDKUtil.SdkAvailable)
            {
                Debug.LogError("No SDK path set.");
                return;
            }

            var libFileName =
#if UNITY_EDITOR_OSX
                // ex: lib/osx/libcamera.magicleap.dylib
                Path.Combine(@"lib/osx", "lib" + lib + ".dylib");
#elif UNITY_EDITOR_WIN
                // ex: lib\win\camera.magicleap.dll
                Path.Combine(@"lib\\win", lib + ".dll");
#endif
            var fullPath = Path.GetFullPath(Path.Combine(MagicLeapSDKUtil.SdkPath, libFileName));
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Library {libFileName} doesn't exist in MLSDK.");
                return;
            }

            nativeLibraryPtr = NativeLoader.LoadLibrary(fullPath);
            if (nativeLibraryPtr != IntPtr.Zero)
            {
                Debug.Log($"Native library {lib} successfully loaded.");
            }
            else
            {
                Debug.LogError($"Failed to load {lib} native library");
            }
        }

        public void Free()
        {
            if (nativeLibraryPtr == IntPtr.Zero)
                return;

            if (NativeLoader.FreeLibrary(nativeLibraryPtr))
            {
                Debug.Log("Native library successfully unloaded.");
            }
            else
            {
                Debug.LogError("Native library could not be unloaded.");
            }

            nativeLibraryPtr = IntPtr.Zero;
        }
    }
}
