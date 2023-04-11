using System;
using System.IO;
using UnityEditor.XR.MagicLeap;
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
        
            var fullPath = Path.GetFullPath(Path.Combine(MagicLeapSDKUtil.SdkPath, @"lib/win/", lib + ".dll"));
            if (!File.Exists(fullPath))
            {
                Debug.LogError("File doesn't exists.");
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