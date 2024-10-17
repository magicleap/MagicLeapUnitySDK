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
using AOT;
using MagicLeap.Android.NDK.NativeWindow;

namespace UnityEngine.XR.MagicLeap
{
    public partial class YCbCrRenderer
    {
        public enum YCbCrResult
        {
            Success = 0,
            InvalidParam = 1,
            RendererCreationFailed,
            UnityGraphicsInterfaceNotFound,
            AllocationFailed
        }

        internal static bool DidSucceed(YCbCrResult result, string functionName, bool logIfError = true)
        {
            var didSucceed = result == YCbCrResult.Success;
            if (!didSucceed && logIfError)
            {
                Debug.LogError($"{functionName} failed with error: {result}");
            }
            return didSucceed;
        }
        
        private static class NativeBindings
        {
            private const string YCbCrRendererDll = "ml_ycbcr_renderer";

            /// <summary>
            /// Create an instance of the YcbcrRenderer.
            /// </summary>
            /// <param name="createInfo">Info used to create the instance</param>
            /// <param name="handle">Handle to the instance</param>
            /// <returns>MLResult.Code.Ok if instance was created successfully.</returns>
            /// <returns>MLResult.Code.InvalidParam if one of params was null.</returns>
            /// <returns>MLResult.Code.MediaGenericNoInit if func was called before Unity graphics was initialized.</returns>
            [DllImport(YCbCrRendererDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern YCbCrResult MLYCbCrRendererCreate([In] ref CreateInfo createInfo, out ulong handle);

            /// <summary>
            /// Get the event id to be used in CommandBuffer.IssuePluginEvent() for a given rendering plugin event.
            /// </summary>
            /// <param name="pluginEvent">Rendering plugin event to get the id for</param>
            /// <returns>Event Id</returns>
            [DllImport(YCbCrRendererDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern int MLYCbCrRendererGetEventIdForPluginEvent(PluginEvent pluginEvent);

            /// <summary>
            /// Get the callback function pointer to be used in CommandBuffer.IssuePluginEvent() for a given rendering plugin event.
            /// </summary>
            /// <param name="pluginEvent">Rendering plugin event to get the callback function pointer for</param>
            /// <returns>Callback function pointer</returns>
            [DllImport(YCbCrRendererDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLYCbCrRendererGetCallbackForPluginEvent(PluginEvent pluginEvent);
            
            /// <summary>
            /// Delegate signature for the callback invoked by the native rendering plugin, requesting a new
            /// AHardwareBuffer.
            /// </summary>
            /// <param name="success">Whether a new native buffer handle was acquired</param>
            /// <param name="hwBuffer">Acquired native buffer handle</param>
            /// <param name="context">User context passed during instance creation</param>
            public delegate void AcquireNextAvailableHwBufferDelegate([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref AHardwareBuffer hwBuffer, IntPtr context);
            
            /// <summary>
            /// Delegate signature for the callback invoked by the native rendering plugin, requesting the
            /// given AHardwareBuffer to be released.
            /// </summary>
            /// <param name="context">User context passed during instance creation</param>
            public delegate void ReleaseHwBufferDelegate(AHardwareBuffer hwBuffer, IntPtr context);

            /// <summary>
            /// Delegate signature for the callback invoked by the native rendering plugin, requesting the
            /// frame transform matrix for the last acquired native buffer handle.
            /// </summary>
            /// <param name="success">Whether a valid frame transform matrix was provided</param>
            /// <param name="frameMat">Frame transform matrix</param>
            /// <param name="context">User context passed during instance creation</param>
            public delegate void GetFrameTransformMatrixDelegate([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref Matrix4x4 frameMat, IntPtr context);

            public delegate void IsNewFrameAvailableDelegate([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, IntPtr context);

            public delegate void CleanupCompleteDelegate(IntPtr context);

            public delegate void FirstFrameRenderedDelegate(IntPtr context);

            public delegate void OverrideYCbCrConversionSamplerDelegate([In] ref VkAndroidHardwareBufferFormatPropertiesAndroid hwBufferFormatProperties, [MarshalAs(UnmanagedType.I1)][In][Out] ref bool samplerChanged, [In][Out] ref VkSamplerYCbCrConversionCreateInfo sampler, IntPtr context);
            
            [MonoPInvokeCallback(typeof(AcquireNextAvailableHwBufferDelegate))]
            private static void AcquireNextAvailableHwBuffer([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref AHardwareBuffer hwBuffer, IntPtr context)
            {
                var gCHandle = GCHandle.FromIntPtr(context);
                if (gCHandle.Target is not IHardwareBufferProvider provider)
                {
                    return;
                }

                success = provider.AcquireNextAvailableHwBuffer(out hwBuffer);
            }
            
            [MonoPInvokeCallback(typeof(ReleaseHwBufferDelegate))]
            private static void ReleaseHwBuffer(AHardwareBuffer hwBuffer, IntPtr context)
            {
                var gCHandle = GCHandle.FromIntPtr(context);
                var provider = gCHandle.Target as IHardwareBufferProvider;
                provider?.ReleaseHwBuffer(hwBuffer);
            }

            [MonoPInvokeCallback(typeof(GetFrameTransformMatrixDelegate))]
            private static void GetFrameTransformMatrix([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref Matrix4x4 frameMat, IntPtr context)
            {
                var gCHandle = GCHandle.FromIntPtr(context);
                if (gCHandle.Target is not IFrameTransformMatrixProvider provider)
                {
                    return;
                }
                success = provider.GetFrameTransformMatrix(ref frameMat);
            }

            [MonoPInvokeCallback(typeof(IsNewFrameAvailableDelegate))]
            private static void IsNewFrameAvailable([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, IntPtr context)
            {
                var gCHandle = GCHandle.FromIntPtr(context);
                if (gCHandle.Target is not IFrameAvailabilityProvider provider)
                {
                    return;
                }
                success = provider.IsNewFrameAvailable();
            }

            [MonoPInvokeCallback(typeof(CleanupCompleteDelegate))]
            private static void OnCleanupComplete(IntPtr context)
            {
                var gcHandle = GCHandle.FromIntPtr(context);
                var yCbCrRenderer = (gcHandle.Target as YCbCrRenderer);
                yCbCrRenderer?.CleanupCompleteCallbackThread();
            }

            [MonoPInvokeCallback(typeof(FirstFrameRenderedDelegate))]
            private static void OnFirstFrameRendered(IntPtr context)
            {
                var gcHandle = GCHandle.FromIntPtr(context);
                var yCbCrRenderer = (gcHandle.Target as YCbCrRenderer);
                yCbCrRenderer?.InvokeOnFirstFrameRendered();
            }

            [MonoPInvokeCallback(typeof(OverrideYCbCrConversionSamplerDelegate))]
            private static void OverrideYCbCrConversionSampler([In] ref VkAndroidHardwareBufferFormatPropertiesAndroid hwBufferFormatProperties, [MarshalAs(UnmanagedType.I1)][In][Out] ref bool samplerChanged, [In][Out] ref VkSamplerYCbCrConversionCreateInfo sampler, IntPtr context)
            {
                var gcHandle = GCHandle.FromIntPtr(context);
                if (gcHandle.Target is not IYCbCrConversionSamplerProvider renderer)
                {
                    return;
                }
                samplerChanged = renderer.OverrideYCbCrConversionSampler(ref hwBufferFormatProperties, ref sampler);
            }

            /// <summary>
            /// Color spaces supported by the native rendering plugin
            /// </summary>
            public enum ColorSpace : uint
            {
                Linear,
                Gamma
            }

            /// <summary>
            /// Plugin events supported by the native rendering plugin
            /// </summary>
            public enum PluginEvent : uint
            {
                /// <summary>
                /// Pass a unity texture and its width, height to the native rendering plugin
                /// </summary>
                SetTexture,

                /// <summary>
                /// Draw the latest native buffer onto the unity texture
                /// </summary>
                Draw,

                /// <summary>
                /// Destroy all resources and the renderer instance
                /// </summary>
                Cleanup,

                AccessTexture
            }

            /// <summary>
            /// Data to be passed down to the native plugin
            /// for a rendering event.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct PluginEventData
            {
                /// <summary>
                /// Handle of the YCbCrRenderer instance received from a MLYCbCrRendererCreate() call.
                /// </summary>
                public ulong RendererHandle;
                /// <summary>
                /// Native pointer of the Unity texture to render the android native buffer on.
                /// </summary>
                public IntPtr TextureHandle;

                /// <summary>
                /// Width of the Unity texture
                /// </summary>
                public int Width;

                /// <summary>
                /// Height of the Unity texture
                /// </summary>
                public int Height;

                /// <summary>
                /// Color space to render the native buffer in.
                /// </summary>
                public ColorSpace ColorSpace;

                public PluginEventData(ulong rendererHandle, RenderTexture renderBuffer)
                {
                    RendererHandle = rendererHandle;
                    TextureHandle = renderBuffer.colorBuffer.GetNativeRenderBufferPtr();
                    Width = renderBuffer.width;
                    Height = renderBuffer.height;
                    ColorSpace = renderBuffer.sRGB ? ColorSpace.Linear : ColorSpace.Gamma;
                }
            }

            /// <summary>
            /// Info to create the native rendering plugin instance with
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct CreateInfo
            {
                /// <summary>
                /// User context data provided in the callbacks.
                /// </summary>
                public IntPtr Context;
                
                public AcquireNextAvailableHwBufferDelegate AcquireNextAvailableHwBufferCallback;
                public ReleaseHwBufferDelegate ReleaseHwBufferCallback;

                /// <summary>
                /// Callback invoked by the native plugin to get the frame transform matrix.
                /// </summary>
                public GetFrameTransformMatrixDelegate GetFrameTransformMatrixCallback;

                public IsNewFrameAvailableDelegate IsNewFrameAvailableCallback;

                public CleanupCompleteDelegate OnCleanupCompleteCallback;

                public FirstFrameRenderedDelegate OnFirstFrameRenderedCallback;

                public OverrideYCbCrConversionSamplerDelegate OverrideYCbCrConversionSamplerCallback;

                [MarshalAs(UnmanagedType.I1)]
                public bool ShouldWaitForQueueIdleOnSubmit;

                /// <summary>
                /// Construct the info for the native plugin instance
                /// </summary>
                /// <param name="context">GCHandle passed back to the callbacks as the user context</param>
                /// <param name="renderer">The YCbCr renderer</param>
                /// <param name="waitForQueueIdleOnSubmit">Whether to wait after submitting a frame</param>
                public CreateInfo(GCHandle context, YCbCrRenderer renderer, bool waitForQueueIdleOnSubmit)
                {
                    Context = GCHandle.ToIntPtr(context);
                    AcquireNextAvailableHwBufferCallback = null;
                    ReleaseHwBufferCallback = null;
                    if (renderer is IHardwareBufferProvider)
                    {
                        AcquireNextAvailableHwBufferCallback = AcquireNextAvailableHwBuffer;
                        ReleaseHwBufferCallback = ReleaseHwBuffer;
                    }

                    GetFrameTransformMatrixCallback = null;
                    if (renderer is IFrameTransformMatrixProvider)
                    {
                        GetFrameTransformMatrixCallback = GetFrameTransformMatrix;
                    }

                    IsNewFrameAvailableCallback = null;
                    if (renderer is IFrameAvailabilityProvider)
                    {
                        IsNewFrameAvailableCallback = IsNewFrameAvailable;
                    }

                    OnCleanupCompleteCallback = OnCleanupComplete;
                    OnFirstFrameRenderedCallback = OnFirstFrameRendered;

                    OverrideYCbCrConversionSamplerCallback = null;
                    if (renderer is IYCbCrConversionSamplerProvider)
                    {
                        OverrideYCbCrConversionSamplerCallback = OverrideYCbCrConversionSampler;
                    }

                    ShouldWaitForQueueIdleOnSubmit = waitForQueueIdleOnSubmit;
                }
            }
        }
    }
}
