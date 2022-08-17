// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "YcbcrRenderer.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine.Rendering;

namespace UnityEngine.XR.MagicLeap
{
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
        private ulong handle = Native.MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Pointer to the unmanaged memory passed to the native rendering plugin for every event.
        /// </summary>
        private IntPtr eventDataPtr = IntPtr.Zero;

        /// <summary>
        /// Managed memory for the data passed to the native rendering plugin for every event.
        /// </summary>
        private NativeBindings.PluginEventData eventData;

        private RenderTexture renderTarget = null;
        private bool didExecuteSetTextureCmdBuffer = false;

        /// <summary>
        /// Command buffers for every native rendering plugin event.
        /// </summary>
        private readonly Dictionary<NativeBindings.PluginEvent, CommandBuffer> commandBuffers = new Dictionary<NativeBindings.PluginEvent, CommandBuffer>();

        public delegate void OnCleanupCompleteDelegate();

        public event OnCleanupCompleteDelegate OnCleanupComplete_CallbackThread = delegate { };
        public event OnCleanupCompleteDelegate OnCleanupComplete = delegate { };

        /// <summary>
        /// Initialize the native api handle & the graphics command buffers.
        /// </summary>
        /// <param name="colorSpace">Color space to render in</param>
        protected void Initialize(UnityEngine.ColorSpace colorSpace, bool waitForQueueIdleOnSubmit = false)
        {
            eventDataPtr = Marshal.AllocHGlobal(Marshal.SizeOf<NativeBindings.PluginEventData>());
            this.gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);

            NativeBindings.ColorSpace nativeColorSpace = NativeBindings.ColorSpace.Linear;
            // TODO : can this be auto-interpretted via the Texture2D supplied to SetTexture()?
            if (colorSpace == ColorSpace.Gamma)
            {
                nativeColorSpace = NativeBindings.ColorSpace.Gamma;
            }

            NativeBindings.CreateInfo createInfo = new NativeBindings.CreateInfo(nativeColorSpace, gcHandle, this, waitForQueueIdleOnSubmit);
            MLResult.Code result = NativeBindings.MLYcbcrRendererCreate(ref createInfo, out handle);
            if (MLResult.IsOK(result))
            {
                eventData.RendererHandle = handle;
                Marshal.StructureToPtr(eventData, eventDataPtr, false);

                CreateAndStoreCommandBufferForEvent(NativeBindings.PluginEvent.Draw);
                // TODO : ideally cleanup command should also create a fence and we should wait on it before destroying the other stuff
                // but i dont think Graphics.CreateGraphicsFence() works with command buffers that issue plugin events. We may need to
                // synchronize directly in the native plugin and expose a func or sync primitive from in there.
                CreateAndStoreCommandBufferForEvent(NativeBindings.PluginEvent.Cleanup);
            }
            else
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                handle = Native.MagicLeapNativeBindings.InvalidHandle;
#endif
            }
        }

        ~YcbcrRenderer()
        {
            // TODO : See comment regarding GPU-CPU sync for cleanup in the Initialize() func.
            ReleaseUnmanagedMemory();
        }

        /// <summary>
        /// Currently only 1 call after obj instantiation will work
        /// </summary>
        /// <param name="renderTexture"></param>
        public void SetRenderBuffer(RenderTexture renderTexture)
        {
            renderTarget = renderTexture;

            // clear the texture
            RenderTexture rt = RenderTexture.active;
            RenderTexture.active = renderTarget;
            GL.Clear(true, true, Color.clear);
            RenderTexture.active = rt;

            // Make sure the hardware resources are created before we start using them
            renderTarget.Create();
            didExecuteSetTextureCmdBuffer = false;
        }

        /// <summary>
        /// Render the latest native buffer onto the provided Unity texture. Should preferably be called every frame.
        /// </summary>
        public void Render()
        {
            // Only execute SetTexture cmd buffer if render target hw resources have been created.
            if (renderTarget != null && renderTarget.IsCreated() && !didExecuteSetTextureCmdBuffer)
            {
                eventData = new NativeBindings.PluginEventData(handle, renderTarget);
                Marshal.StructureToPtr(eventData, eventDataPtr, false);
                CreateAndStoreCommandBufferForEvent(NativeBindings.PluginEvent.SetTexture);
                Graphics.ExecuteCommandBuffer(commandBuffers[NativeBindings.PluginEvent.SetTexture]);

                didExecuteSetTextureCmdBuffer = true;
            }

            if (renderTarget.IsCreated())
            {
                if (commandBuffers.TryGetValue(NativeBindings.PluginEvent.Draw, out CommandBuffer cmdBuffer))
                {
                    Graphics.ExecuteCommandBuffer(cmdBuffer);
                }
            }
        }

        /// <summary>
        /// Destroy all resources held by the native rendering plugin.
        /// </summary>
        public void Cleanup()
        {
            if (commandBuffers.TryGetValue(NativeBindings.PluginEvent.Cleanup, out CommandBuffer cmdBuffer))
            {
                Graphics.ExecuteCommandBuffer(cmdBuffer);
            }

            // TODO : See comment regarding GPU-CPU sync for cleanup in the Initialize() func.
        }

        private void ReleaseUnmanagedMemory()
        {
            if (eventDataPtr != IntPtr.Zero)
            {
                Marshal.FreeHGlobal(eventDataPtr);
                eventDataPtr = IntPtr.Zero;
            }
        }

        private void CreateAndStoreCommandBufferForEvent(NativeBindings.PluginEvent pluginEvent)
        {
            if (!commandBuffers.ContainsKey(pluginEvent))
            {
                CommandBuffer cmdBuffer = new CommandBuffer();
                if (pluginEvent == NativeBindings.PluginEvent.Draw)
                {
                    cmdBuffer.IssuePluginEventAndData(
                        NativeBindings.MLYcbcrRendererGetCallbackForPluginEvent(NativeBindings.PluginEvent.AccessTexture),
                        NativeBindings.MLYcbcrRendererGetEventIdForPluginEvent(NativeBindings.PluginEvent.AccessTexture),
                        eventDataPtr);
                }
                cmdBuffer.IssuePluginEventAndData(
                    NativeBindings.MLYcbcrRendererGetCallbackForPluginEvent(pluginEvent), 
                    NativeBindings.MLYcbcrRendererGetEventIdForPluginEvent(pluginEvent),
                    eventDataPtr);

                commandBuffers.Add(pluginEvent, cmdBuffer);
            }
        }

        private void InvokeOnCleanupComplete_CallbackThread()
        {
            OnCleanupComplete_CallbackThread();
#if UNITY_ANDROID
            Native.MLThreadDispatch.ScheduleMain(() => InvokeOnCleanupCompleted_MainThread());
#endif
        }

        private void InvokeOnCleanupCompleted_MainThread()
        {
            if (renderTarget != null)
            {
                renderTarget.Release();
            }

            didExecuteSetTextureCmdBuffer = false;
            handle = Native.MagicLeapNativeBindings.InvalidHandle;
            commandBuffers.Clear();
            gcHandle.Free();
            ReleaseUnmanagedMemory();

            OnCleanupComplete();
        }
    }
}
