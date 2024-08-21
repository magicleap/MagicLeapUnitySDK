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
    /// <summary>
    /// Implements a renderer for android native & hardware buffers (Vulkan-only).
    /// </summary>
    public abstract partial class YCbCrRenderer : IDisposable
    {
        /// <summary>
        /// GCHandle for the callback user context
        /// </summary>
        private GCHandle gcHandle;

        /// <summary>
        /// MLYCbCrRenderer handle
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
        private readonly Dictionary<NativeBindings.PluginEvent, CommandBuffer> commandBuffers = new();

        /// <summary>
        /// Event fired on the callback thread to indicate that resource cleanup is complete in the native plugin
        /// and is now safe to clean up associated managed resources like the RenderTexture.
        /// </summary>
        public event Action OnCleanupCompleteCallbackThread;

        /// <summary>
        /// Event fired on Unity's main thread to indicate that resource cleanup is complete in the native plugin
        /// and  is now safe to clean up associated managed resources like the RenderTexture.
        /// </summary>
        public event Action OnCleanupComplete;

        /// <summary>
        /// Event fired to indicate a frame has been rendered on the current RenderTexture for the first time.
        /// Apps can use this event to disable UI elements like loading indicators since the RenderTexture will
        /// have a valid frame to display.
        /// </summary>
        public event Action OnFirstFrameRendered;


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

        ~YCbCrRenderer()
        {
            Dispose(false);
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
        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (commandBuffers.TryGetValue(NativeBindings.PluginEvent.Cleanup, out var cmdBuffer))
                {
                    Graphics.ExecuteCommandBuffer(cmdBuffer);
                }
            }
            ReleaseUnmanagedMemory();
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
            if (commandBuffers.ContainsKey(pluginEvent))
            {
                return;
            }

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

        private void CleanupCompleteCallbackThread()
        {
            OnCleanupCompleteCallbackThread?.Invoke();
            MLThreadDispatch.ScheduleMain(CleanUpCompletedMainThread);
        }

        private void CleanUpCompletedMainThread()
        {
            didExecuteSetTextureCmdBuffer = false;
            handle = MagicLeapNativeBindings.InvalidHandle;
            commandBuffers.Clear();
            gcHandle.Free();
            ReleaseUnmanagedMemory();
            OnCleanupComplete?.Invoke();
        }

        private void InvokeOnFirstFrameRendered()
        {
            MLThreadDispatch.Call(OnFirstFrameRendered);
        }
    }
}
