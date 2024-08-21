// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLNativeSurface
    {
        internal sealed class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceCreate(ref MLNativeSurfaceConfig configValues, out ulong handle);

            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceRelease(ulong handle);

            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceAcquireNextAvailableFrame(ulong handle, out ulong nativeBuffer);

            // Note : hardwareBuffer is a double ptr so will need manual marshalling.
            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceAcquireHardwareBufferFromNativeBuffer(ulong handle, ulong native_buffer, out IntPtr hardwareBuffer, out uint width, out uint height);

            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceReleaseFrame(ulong handle, ulong native_buffer);

            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceGetFrameTransformationMatrix(ulong handle, [MarshalAs(UnmanagedType.LPArray, SizeConst = 16)] float[] OutMtx);

            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceGetFrameTimestamp(ulong handle, out long timestampNs);

            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceGetFrameQueueBufferTimestamp(ulong handle, out long timestampNs);

            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceGetFrameNumber(ulong handle, out ulong number);

            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceGetFrameCropRect(ulong handle, out MLRecti cropRect);

            [DllImport(MLNativeSurfaceDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLNativeSurfaceSetOnFrameAvailableCallback(ulong handle, ref MLNativeSurfaceOnFrameAvailableCallback callback, IntPtr userData);

            public delegate void OnFrameAvailable(ulong handle, ref MLNativeSurfaceFrameAvailableInfo info);

            [AOT.MonoPInvokeCallback(typeof(OnFrameAvailable))]
            private static void OnFrameAvailableCallback(ulong handle, ref MLNativeSurfaceFrameAvailableInfo info)
            {
                GCHandle gCHandle = GCHandle.FromIntPtr(info.UserData);
                MLNativeSurface nativeSurface = gCHandle.Target as MLNativeSurface;
                if (nativeSurface == null)
                {
                    return;
                }

                nativeSurface.OnFrameAvailable_CallbackThread();
            }

            public enum MLNativeSurfaceAcquiredBufferCount : uint
            {
                Min = 1,
                Max = 16
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLNativeSurfaceFrameAvailableInfo
            {
                public readonly IntPtr UserData;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLNativeSurfaceConfig
            {
                private readonly uint Version;

                private readonly PixelFormat PixelFormat;

                private readonly ushort BufferCount;

                private readonly uint Width;

                private readonly uint Height;

                public MLNativeSurfaceConfig(PixelFormat pixelFormat, ushort bufferCount, uint width, uint height)
                {
                    Version = 1;
                    PixelFormat = pixelFormat;
                    BufferCount = bufferCount;
                    Width = width;
                    Height = height;
                }
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLNativeSurfaceOnFrameAvailableCallback
            {
                private uint Version;
                private OnFrameAvailable OnFrameAvailableDelegate;

                public static MLNativeSurfaceOnFrameAvailableCallback Create()
                {
                    return new MLNativeSurfaceOnFrameAvailableCallback()
                    {
                        Version = 1,
                        OnFrameAvailableDelegate = OnFrameAvailableCallback
                    };
                }
            }
        }
    }
}
