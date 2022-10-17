// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Runtime.InteropServices;
using System;

namespace UnityEngine.XR.MagicLeap
{
    public partial class YcbcrRenderer
    {
        private sealed class NativeBindings
        {
            private const string MLYcbcrRendererDll = "ml_ycbcr_renderer";

            /// <summary>
            /// Create an instance of the YcbcrRenderer.
            /// </summary>
            /// <param name="createInfo">Info used to create the instance</param>
            /// <param name="handle">Handle to the instance</param>
            /// <returns>MLResult.Code.Ok if instance was created successfully.</returns>
            /// <returns>MLResult.Code.InvalidParam if one of params was null.</returns>
            /// <returns>MLResult.Code.MediaGenericNoInit if func was called before Unity graphics was initialized.</returns>
            [DllImport(MLYcbcrRendererDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLYcbcrRendererCreate([In] ref CreateInfo createInfo, out ulong handle);

            /// <summary>
            /// Get the event id to be used in CommandBuffer.IssuePluginEvent() for a given rendering plugin event.
            /// </summary>
            /// <param name="pluginEvent">Rendering plugin event to get the id for</param>
            /// <returns>Event Id</returns>
            [DllImport(MLYcbcrRendererDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern int MLYcbcrRendererGetEventIdForPluginEvent(PluginEvent pluginEvent);

            /// <summary>
            /// Get the callback function pointer to be used in CommandBuffer.IssuePluginEvent() for a given rendering plugin event.
            /// </summary>
            /// <param name="pluginEvent">Rendering plugin event to get the callback function pointer for</param>
            /// <returns>Callback function pointer</returns>
            [DllImport(MLYcbcrRendererDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern IntPtr MLYcbcrRendererGetCallbackForPluginEvent(PluginEvent pluginEvent);

            /// <summary>
            /// Delegate signature for the callback invoked by the native rendering plugin, requesting a new
            /// native buffer handle.
            /// </summary>
            /// <param name="success">Whether a new native buffer handle was acquired</param>
            /// <param name="nativeBufferHandle">Acquired native buffer handle</param>
            /// <param name="context">User context passed during instance creation</param>
            public delegate void AcquireNextAvailableBufferDelegate([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref ulong nativeBufferHandle, IntPtr context);

            /// <summary>
            /// Delegate signature for the callback invoked by the native rendering plugin, requesting a new
            /// AHardwareBuffer.
            /// </summary>
            /// <param name="success">Whether a new native buffer handle was acquired</param>
            /// <param name="hwBuffer">Acquired native buffer handle</param>
            /// <param name="context">User context passed during instance creation</param>
            public delegate void AcquireNextAvailableHwBufferDelegate([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref IntPtr hwBuffer, IntPtr context);

            /// <summary>
            /// Delegate signature for the callback invoked by the native rendering plugin, requesting the
            /// given native buffer to be released.
            /// </summary>
            /// <param name="nativeBufferHandle">Native buffer handle to be released</param>
            /// <param name="context">User context passed during instance creation</param>
            public delegate void ReleaseBufferDelegate(ulong nativeBufferHandle, IntPtr context);

            /// <summary>
            /// Delegate signature for the callback invoked by the native rendering plugin, requesting the
            /// given AHardwareBuffer to be released.
            /// </summary>
            /// <param name="nativeBufferHandle">Native buffer handle to be released</param>
            /// <param name="context">User context passed during instance creation</param>
            public delegate void ReleaseHwBufferDelegate(IntPtr hwBuffer, IntPtr context);

#if UNITY_MAGICLEAP || UNITY_ANDROID
            /// <summary>
            /// Delegate signature for the callback invoked by the native rendering plugin, requesting the
            /// frame transform matrix for the last acquired native buffer handle.
            /// </summary>
            /// <param name="success">Whether a valid frame transform matrix was provided</param>
            /// <param name="frameMat">Frame transform matrix</param>
            /// <param name="context">User context passed during instance creation</param>
            public delegate void GetFrameTransformMatrixDelegate([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref Native.MagicLeapNativeBindings.MLMat4f frameMat, IntPtr context);
#endif

            public delegate void IsNewFrameAvailableDelegate([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, IntPtr context);

            public delegate void OnCleanupCompleteDelegate(IntPtr context);

            public delegate void OnFirstFrameRenderedDelegate(IntPtr context);

            public delegate void OverrideYcbcrConversionSamplerDelegate([In] ref VkAndroidHardwareBufferFormatPropertiesANDROID hwBufferFormatProperties, [MarshalAs(UnmanagedType.I1)][In][Out] ref bool samplerChanged, [In][Out] ref VkSamplerYcbcrConversionCreateInfo sampler, IntPtr context);

            [AOT.MonoPInvokeCallback(typeof(AcquireNextAvailableBufferDelegate))]
            private static void AcquireNextAvailableBuffer([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref ulong nativeBufferHandle, IntPtr context)
            {
                GCHandle gCHandle = GCHandle.FromIntPtr(context);
                INativeBufferProvider provider = gCHandle.Target as INativeBufferProvider;
                if (provider == null)
                {
                    return;
                }

                success = provider.AcquireNextAvailableBuffer(out nativeBufferHandle);
            }

            [AOT.MonoPInvokeCallback(typeof(AcquireNextAvailableHwBufferDelegate))]
            private static void AcquireNextAvailableHwBuffer([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref IntPtr hwBuffer, IntPtr context)
            {
                GCHandle gCHandle = GCHandle.FromIntPtr(context);
                IHardwareBufferProvider provider = gCHandle.Target as IHardwareBufferProvider;
                if (provider == null)
                {
                    return;
                }

                success = provider.AcquireNextAvailableHwBuffer(out hwBuffer);
            }

            [AOT.MonoPInvokeCallback(typeof(ReleaseBufferDelegate))]
            private static void ReleaseBuffer(ulong nativeBufferHandle, IntPtr context)
            {
                GCHandle gCHandle = GCHandle.FromIntPtr(context);
                INativeBufferProvider provider = gCHandle.Target as INativeBufferProvider;
                if (provider == null)
                {
                    return;
                }

                provider.ReleaseBuffer(nativeBufferHandle);
            }

            [AOT.MonoPInvokeCallback(typeof(ReleaseHwBufferDelegate))]
            private static void ReleaseHwBuffer(IntPtr hwBuffer, IntPtr context)
            {
                GCHandle gCHandle = GCHandle.FromIntPtr(context);
                IHardwareBufferProvider provider = gCHandle.Target as IHardwareBufferProvider;
                if (provider == null)
                {
                    return;
                }

                provider.ReleaseHwBuffer(hwBuffer);
            }

#if UNITY_MAGICLEAP || UNITY_ANDROID
            [AOT.MonoPInvokeCallback(typeof(GetFrameTransformMatrixDelegate))]
            private static void GetFrameTransformMatrix([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, [In][Out] ref Native.MagicLeapNativeBindings.MLMat4f frameMat, IntPtr context)
            {
                GCHandle gCHandle = GCHandle.FromIntPtr(context);
                IFrameTransformMatrixProvider provider = gCHandle.Target as IFrameTransformMatrixProvider;
                if (provider == null)
                {
                    return;
                }

                success = provider.GetFrameTransformMatrix(frameMat.MatrixColmajor);
            }
#endif

            [AOT.MonoPInvokeCallback(typeof(IsNewFrameAvailableDelegate))]
            private static void IsNewFrameAvailable([MarshalAs(UnmanagedType.I1)][In][Out] ref bool success, IntPtr context)
            {
                GCHandle gCHandle = GCHandle.FromIntPtr(context);
                IFrameAvailabilityProvider provider = gCHandle.Target as IFrameAvailabilityProvider;
                if (provider == null)
                {
                    return;
                }

                success = provider.IsNewFrameAvailable();
            }

            [AOT.MonoPInvokeCallback(typeof(OnCleanupCompleteDelegate))]
            private static void OnCleanupComplete(IntPtr context)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(context);
                YcbcrRenderer ycbcrRenderer = (gcHandle.Target as YcbcrRenderer);
                if (ycbcrRenderer == null)
                {
                    return;
                }

                ycbcrRenderer.InvokeOnCleanupComplete_CallbackThread();
            }

            [AOT.MonoPInvokeCallback(typeof(OnFirstFrameRenderedDelegate))]
            private static void OnFirstFrameRendered(IntPtr context)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(context);
                YcbcrRenderer ycbcrRenderer = (gcHandle.Target as YcbcrRenderer);
                if (ycbcrRenderer == null)
                {
                    return;
                }

                ycbcrRenderer.InvokeOnFirstFrameRendered();
            }

            [AOT.MonoPInvokeCallback(typeof(OverrideYcbcrConversionSamplerDelegate))]
            private static void OverrideYcbcrConversionSampler([In] ref VkAndroidHardwareBufferFormatPropertiesANDROID hwBufferFormatProperties, [MarshalAs(UnmanagedType.I1)][In][Out] ref bool samplerChanged, [In][Out] ref VkSamplerYcbcrConversionCreateInfo sampler, IntPtr context)
            {
                GCHandle gcHandle = GCHandle.FromIntPtr(context);
                IYcbcrConversionSamplerProvider ycbcrRenderer = (gcHandle.Target as IYcbcrConversionSamplerProvider);
                if (ycbcrRenderer == null)
                {
                    return;
                }

                samplerChanged = ycbcrRenderer.OverrideYcbcrConversionSampler(ref hwBufferFormatProperties, ref sampler);
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
                /// Handle of the YcbcrRenderer instance received from a MLYcbcrRendererCreate() call.
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
                    this.RendererHandle = rendererHandle;
                    this.TextureHandle = renderBuffer.colorBuffer.GetNativeRenderBufferPtr();
                    this.Width = renderBuffer.width;
                    this.Height = renderBuffer.height;
                    // As per https://docs.unity3d.com/ScriptReference/RenderTextureReadWrite.html,
                    // when project color space is Linear, RenderTextures that are supposed to be
                    // used as color textures should should use srgb read-write (default for Linear
                    // color space projets). In such a case, "fragment shaders are considered to output
                    // linear color values", which is why we select Linear here. If srgb read-wrtite is
                    // disabled for this RenderTexture, we should direclty output gamma pixels.
                    this.ColorSpace = renderBuffer.sRGB ? ColorSpace.Linear : ColorSpace.Gamma;
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

                /// <summary>
                /// Callback invoked by the native plugin to acquire a native buffer.
                /// </summary>
                public AcquireNextAvailableBufferDelegate AcquireNextAvailableBufferCallback;

                public AcquireNextAvailableHwBufferDelegate AcquireNextAvailableHwBufferCallback;

                /// <summary>
                /// Callback invoked by the native plugin to release a native buffer.
                /// </summary>
                public ReleaseBufferDelegate ReleaseBufferCallback;

                public ReleaseHwBufferDelegate ReleaseHwBufferCallback;

#if UNITY_MAGICLEAP || UNITY_ANDROID
                /// <summary>
                /// Callback invoked by the native plugin to get the frame transform matrix.
                /// </summary>
                public GetFrameTransformMatrixDelegate GetFrameTransformMatrixCallback;
#endif

                public IsNewFrameAvailableDelegate IsNewFrameAvailableCallback;

                public OnCleanupCompleteDelegate OnCleanupCompleteCallback;

                public OnFirstFrameRenderedDelegate OnFirstFrameRenderedCallback;

                public OverrideYcbcrConversionSamplerDelegate OverrideYcbcrConversionSamplerCallback;

                [MarshalAs(UnmanagedType.I1)]
                public bool ShouldWaitForQueueIdleOnSubmit;

                /// <summary>
                /// Construct the info for the native plugin instance
                /// </summary>
                /// <param name="context">GCHandle passed back to the callbacks as the user context</param>
                /// <param name="isReleaseBufferAvailable">If the api supports releasing the native buffer. Pass false to avoid unnecesarry calls from unmanaged to managed layer.</param>
                /// <param name="isFrameTransformMatrixAvailable">If the api supports a frame transform matrix. Pass false to avoid unnecesarry calls & data copies from unmanaged to managed layer & back.</param>
                public CreateInfo(GCHandle context, YcbcrRenderer renderer, bool waitForQueueIdleOnSubmit)
                {
                    this.Context = GCHandle.ToIntPtr(context);

                    this.AcquireNextAvailableBufferCallback = null;
                    this.ReleaseBufferCallback = null;
                    if (renderer is INativeBufferProvider)
                    {
                        this.AcquireNextAvailableBufferCallback = AcquireNextAvailableBuffer;
                        this.ReleaseBufferCallback = ReleaseBuffer;
                    }


                    this.AcquireNextAvailableHwBufferCallback = null;
                    this.ReleaseHwBufferCallback = null;
                    if (renderer is IHardwareBufferProvider)
                    {
                        this.AcquireNextAvailableHwBufferCallback = AcquireNextAvailableHwBuffer;
                        this.ReleaseHwBufferCallback = ReleaseHwBuffer;
                    }

#if UNITY_MAGICLEAP || UNITY_ANDROID
                    this.GetFrameTransformMatrixCallback = null;
                    if (renderer is IFrameTransformMatrixProvider)
                    {
                        this.GetFrameTransformMatrixCallback = GetFrameTransformMatrix;
                    }
#endif
                    this.IsNewFrameAvailableCallback = null;
                    if (renderer is IFrameAvailabilityProvider)
                    {
                        this.IsNewFrameAvailableCallback = IsNewFrameAvailable;
                    }

                    this.OnCleanupCompleteCallback = OnCleanupComplete;
                    this.OnFirstFrameRenderedCallback = OnFirstFrameRendered;

                    this.OverrideYcbcrConversionSamplerCallback = null;
                    if (renderer is IYcbcrConversionSamplerProvider)
                    {
                        this.OverrideYcbcrConversionSamplerCallback = OverrideYcbcrConversionSampler;
                    }

                    this.ShouldWaitForQueueIdleOnSubmit = waitForQueueIdleOnSubmit;
                }
            }
        }
    }
}
