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
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    [Obsolete("YcbcrRenderer has been deprecated. Use YCbCrRenderer instead")]
    /// <summary>
    /// Implements a renderer for android native & hardware buffers (Vulkan-only).
    /// </summary>
    public abstract partial class YcbcrRenderer
    {
        /// <summary>
        /// GCHandle for the callback user context
        /// </summary>
        protected GCHandle gcHandle;

        /// <summary>
        /// MLYcbcrRenderer handle
        /// </summary>
        private ulong handle = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Pointer to the unmanaged memory passed to the native rendering plugin for every event.
        /// </summary>
        private IntPtr eventDataPtr = IntPtr.Zero;

        /// <summary>
        /// Managed memory for the data passed to the native rendering plugin for every event.
        /// </summary>
        private NativeBindings.PluginEventData eventData;

        protected RenderTexture RenderTarget;
        private bool didExecuteSetTextureCmdBuffer;

        /// <summary>
        /// Command buffers for every native rendering plugin event.
        /// </summary>
        private readonly Dictionary<NativeBindings.PluginEvent, CommandBuffer> commandBuffers = new Dictionary<NativeBindings.PluginEvent, CommandBuffer>();

        public delegate void OnCleanupCompleteDelegate();
        public delegate void OnFirstFrameRendereredDelegate();

        /// <summary>
        /// Event fired on the callback thread to indicate that resource cleanup is complete in the native plugin
        /// and it is now safe to cleanup associated managed resources like the RenderTexture.
        /// </summary>
        public event OnCleanupCompleteDelegate OnCleanupComplete_CallbackThread = delegate { };

        /// <summary>
        /// Event fired on Unity's main thread to indicate that resource cleanup is complete in the native plugin
        /// and it is now safe to cleanup associated managed resources like the RenderTexture.
        /// </summary>
        public event OnCleanupCompleteDelegate OnCleanupComplete = delegate { };

        /// <summary>
        /// Event fired to indicate a frame has been rendered on the current RenderTexture for the first time.
        /// Apps can use this event to disable UI elements like loading indicators since the RenderTexture will
        /// have a valid frame to display.
        /// </summary>
        public event OnFirstFrameRendereredDelegate OnFirstFrameRendered = delegate { };

        private ulong nativeBufferHandle;
        
        /// <summary>
        /// Initialize the native api handle & the graphics command buffers.
        /// </summary>
        protected void Initialize(bool waitForQueueIdleOnSubmit = false)
        {
            eventDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<NativeBindings.PluginEventData>());
            gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);

            var createInfo = new NativeBindings.CreateInfo(gcHandle, this, waitForQueueIdleOnSubmit);
            var result = NativeBindings.MLYCbCrRendererCreate(ref createInfo, out handle);
            if (DidSucceed(result, nameof(NativeBindings.MLYCbCrRendererCreate)))
            {
                eventData.RendererHandle = handle;
                Marshal.StructureToPtr(eventData, eventDataPtr, false);
                CreateAndStoreCommandBufferForEvent(NativeBindings.PluginEvent.Draw);
                CreateAndStoreCommandBufferForEvent(NativeBindings.PluginEvent.Cleanup);
            }
            else
            {
                handle = MagicLeapNativeBindings.InvalidHandle;
            }
        }

        ~YcbcrRenderer()
        {
            ReleaseUnmanagedMemory();
        }

        /// <summary>
        /// Currently only 1 call after obj instantiation will work
        /// </summary>
        /// <param name="renderTexture"></param>
        public void SetRenderBuffer(RenderTexture renderTexture)
        {
            RenderTarget = renderTexture;
            // clear the texture
            var rt = RenderTexture.active;
            RenderTexture.active = RenderTarget;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;

            // Make sure the hardware resources are created before we start using them
            RenderTarget.Create();
            didExecuteSetTextureCmdBuffer = false;
            OnRenderTargetSet();
        }

        protected virtual void OnRenderTargetSet()
        {
            
        }
        
        /// <summary>
        /// Render the latest native buffer onto the provided Unity texture. Should preferably be called every frame.
        /// </summary>
        public void Render()
        {
            if (RenderTarget == null)
            {
                return;
            }

            if (!didExecuteSetTextureCmdBuffer)
            {
                if (RenderTarget != null && RenderTarget.IsCreated() && RenderTarget.colorBuffer.GetNativeRenderBufferPtr() != IntPtr.Zero)
                {
                    eventData = new NativeBindings.PluginEventData(handle, RenderTarget);
                }
                Marshal.StructureToPtr(eventData, eventDataPtr, false);
                CreateAndStoreCommandBufferForEvent(NativeBindings.PluginEvent.SetTexture);
                Graphics.ExecuteCommandBuffer(commandBuffers[NativeBindings.PluginEvent.SetTexture]);

                didExecuteSetTextureCmdBuffer = true;
            }

            if (RenderTarget != null && !RenderTarget.IsCreated())
            {
                return;
            }

            if (commandBuffers.TryGetValue(NativeBindings.PluginEvent.Draw, out var cmdBuffer))
            {
                Graphics.ExecuteCommandBuffer(cmdBuffer);
            }
        }

        /// <summary>
        /// Destroy all resources held by the native rendering plugin.
        /// </summary>
        public void Cleanup()
        {
            if (commandBuffers.TryGetValue(NativeBindings.PluginEvent.Cleanup, out var cmdBuffer))
            {
                Graphics.ExecuteCommandBuffer(cmdBuffer);
            }
        }

        private void ReleaseUnmanagedMemory()
        {
            if (eventDataPtr == IntPtr.Zero)
            {
                return;
            }

            Marshal.FreeHGlobal(eventDataPtr);
            eventDataPtr = IntPtr.Zero;
        }

        private void CreateAndStoreCommandBufferForEvent(NativeBindings.PluginEvent pluginEvent)
        {
            if (!commandBuffers.ContainsKey(pluginEvent))
            {
                var cmdBuffer = new CommandBuffer();
                if (pluginEvent == NativeBindings.PluginEvent.Draw)
                {
                    cmdBuffer.IssuePluginEventAndData(
                        NativeBindings.MLYCbCrRendererGetCallbackForPluginEvent(NativeBindings.PluginEvent.AccessTexture),
                        NativeBindings.MLYCbCrRendererGetEventIdForPluginEvent(NativeBindings.PluginEvent.AccessTexture),
                        eventDataPtr);
                }
                cmdBuffer.IssuePluginEventAndData(
                    NativeBindings.MLYCbCrRendererGetCallbackForPluginEvent(pluginEvent),
                    NativeBindings.MLYCbCrRendererGetEventIdForPluginEvent(pluginEvent),
                    eventDataPtr);

                commandBuffers.Add(pluginEvent, cmdBuffer);
            }
        }

        private void InvokeOnCleanupComplete_CallbackThread()
        {
            OnCleanupComplete_CallbackThread();
            MLThreadDispatch.ScheduleMain(InvokeOnCleanupCompleted_MainThread);
        }

        private void InvokeOnCleanupCompleted_MainThread()
        {
            if (RenderTarget != null)
            {
                RenderTarget.Release();
            }

            didExecuteSetTextureCmdBuffer = false;
            handle = MagicLeapNativeBindings.InvalidHandle;
            commandBuffers.Clear();
            gcHandle.Free();
            ReleaseUnmanagedMemory();

            OnCleanupComplete();
        }

        private void InvokeOnFirstFrameRendered()
        {
            MLThreadDispatch.Call(OnFirstFrameRendered);
        }
    }
}
