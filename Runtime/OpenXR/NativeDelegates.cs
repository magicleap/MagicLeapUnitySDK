using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.NativeDelegates
{
    internal delegate XrResult XrGetInstanceProcAddr(ulong instance, [MarshalAs(UnmanagedType.LPStr)] string name, ref IntPtr pointer);
        
    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal delegate XrResult XrPollEvent(ulong instance, IntPtr eventBuffer);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal unsafe delegate XrResult XrBeginFrame(ulong session, XrFrameBeginInfo* beginInfo);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal unsafe delegate XrResult XrWaitFrame(ulong session, XrFrameWaitInfo* frameWaitInfo, XrFrameState* frameState);

    [UnmanagedFunctionPointer(CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
    internal unsafe delegate XrResult XrEndFrame(ulong session, XrFrameEndInfo* frameEndInfo);
}
