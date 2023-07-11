using System;
using System.Runtime.InteropServices;

namespace UnitySDKEditorTests
{
    public static class NativeLoader
    {
#if UNITY_EDITOR_OSX
        public static bool FreeLibrary(IntPtr hModule) => dlclose(hModule) == 0;
 
        [DllImport("libdl", ExactSpelling = true, EntryPoint = "dlopen")]
        public static extern IntPtr LoadLibrary(string lpFileName, int flags = 0);
 
        [DllImport("libdl", ExactSpelling = true, EntryPoint = "dlsym")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);   
        
        [DllImport("libdl", ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern int dlclose(IntPtr hModule);     
#elif UNITY_EDITOR_WIN
        [DllImport("kernel32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool FreeLibrary(IntPtr hModule);
 
        [DllImport("kernel32", SetLastError = true, CharSet = CharSet.Unicode)]
        public static extern IntPtr LoadLibrary(string lpFileName);
 
        [DllImport("kernel32")]
        public static extern IntPtr GetProcAddress(IntPtr hModule, string procedureName);
#endif
    }
}
