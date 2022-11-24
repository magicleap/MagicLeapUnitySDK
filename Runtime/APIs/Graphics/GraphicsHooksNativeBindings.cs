// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;

namespace UnityEngine.XR.MagicLeap
{
    public static partial class MLGraphicsHooks
    {
        private class NativeBindings : Native.MagicLeapNativeBindings
        {
            public enum ProjectionType
            {
                SignedZ = 0,
                ReversedInfiniteZ = 1,
                UnsignedZ = 2
            }

            public enum EnvironmentBlendMode
            {
                Additive = 0,
                AlphaBlend = 1
            }

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern ulong MLUnityGraphicsGetHandle();

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern void MLUnityGraphicsRegisterCallbacks([In] ref MLUnityGraphicsCallbacks callbacks);

            public delegate void OnPreBeginRenderFrameNativeDelegate(IntPtr context, ref MLGraphicsFrameParamsEx frameParams);

            [StructLayout(LayoutKind.Sequential)]
            public struct MLGraphicsFrameParamsEx
            {
                public uint Version;
                public float NearClip;
                public float FarClip;
                public float FocusDustance;
                public float SurfaceScale;

                [MarshalAs(UnmanagedType.I1)]
                public bool Vignette;

                [MarshalAs(UnmanagedType.I1)]
                public bool ProtectedSurface;

                public ProjectionType ProjectionType;
                public EnvironmentBlendMode BlendMode;
            }

            [AOT.MonoPInvokeCallback(typeof(OnPreBeginRenderFrameNativeDelegate))]
            private static void OnPreBeginRenderFrameCallback(IntPtr context, [In][Out] ref MLGraphicsFrameParamsEx frameParams)
            {
                if (preferAlphaBlendMode)
                {
                    if (frameParams.Version < 4)
                    {
                        frameParams.Version = 4;
                    }
                    frameParams.BlendMode = EnvironmentBlendMode.AlphaBlend;
                }
                internalOnPreBeginRenderFrame();
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLUnityGraphicsCallbacks
            {
                public IntPtr Context;
                public OnPreBeginRenderFrameNativeDelegate OnPreBeginRenderFrame;

                public static MLUnityGraphicsCallbacks Create()
                {
                    return new MLUnityGraphicsCallbacks()
                    {
                        Context = IntPtr.Zero,
                        OnPreBeginRenderFrame = OnPreBeginRenderFrameCallback
                    };
                }
            }
        }
    }
}
