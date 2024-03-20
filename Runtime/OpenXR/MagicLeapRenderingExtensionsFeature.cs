// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_OPENXR_1_9_0_OR_NEWER
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Unsafe;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop;
using UnityEngine.XR.OpenXR.NativeTypes;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif
namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Rendering Extensions",
        Desc="Support for controlling rendering features specific to Magic Leap 2.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new []{ BuildTargetGroup.Android },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_ML_frame_end_info " +
                                 "XR_ML_global_dimmer "
    )]
#endif
    public unsafe class MagicLeapRenderingExtensionsFeature : MagicLeapOpenXRFeatureBase
    {   
        public const string FeatureId = "com.magicleap.openxr.feature.rendering_extensions";

        public float FocusDistance = 0.0f;
        public bool UseProtectedSurface = false;
        public bool UseVignetteMode = false;

        public bool GlobalDimmerEnabled = false;
        public float GlobalDimmerValue = 0.0f;

        public XrEnvironmentBlendMode BlendMode = XrEnvironmentBlendMode.AlphaBlend;

        [NonSerialized] 
        private FrameEndInfo* frameEndInfo;

        [NonSerialized]
        private GlobalDimmerFrameEndInfo* globalDimmerFrameInfo;

        protected override void OnSessionBegin(ulong xrSession)
        {
            base.OnSessionBegin(xrSession);

            Cleanup();
            
            InitializeOpenXRState();

            Application.onBeforeRender += SynchronizeRenderState;
        }

        protected override void OnSessionEnd(ulong xrSession)
        {
            base.OnSessionEnd(xrSession);
            
            Cleanup();
        }

        private void Cleanup()
        {
            if (frameEndInfo != null)
                UnsafeUtility.FreeTracked(frameEndInfo, Allocator.Persistent);
            
            if (globalDimmerFrameInfo != null)
                UnsafeUtility.FreeTracked(globalDimmerFrameInfo, Allocator.Persistent);

            Application.onBeforeRender -= SynchronizeRenderState;
        }

        private void InitializeOpenXRState()
        {
            frameEndInfo = UnsafeUtilityEx.MallocTracked<FrameEndInfo>(Allocator.Persistent, 1);
            *frameEndInfo = FrameEndInfo.Init();
            
            globalDimmerFrameInfo =
                UnsafeUtilityEx.MallocTracked<GlobalDimmerFrameEndInfo>(Allocator.Persistent, 1);
            *globalDimmerFrameInfo = GlobalDimmerFrameEndInfo.Init();

            frameEndInfo->Next = globalDimmerFrameInfo;
            
            MLOpenXRInitializeEndFrameState(frameEndInfo);
        }

        private NativeInterop.BlendMode ToNativeBlendMode(XrEnvironmentBlendMode blendMode)
        {
            switch (blendMode)
            {
                case XrEnvironmentBlendMode.Opaque:
                    throw new NotSupportedException($"XrEnvironmentBlendMode.Opaque is not supported on Magic Leap 2");
                case XrEnvironmentBlendMode.Additive:
                    return NativeInterop.BlendMode.Additive;
                case XrEnvironmentBlendMode.AlphaBlend:
                    return NativeInterop.BlendMode.AlphaBlend;
                default:
                    throw new ArgumentException(nameof(blendMode));
            }
            
        }

        private void SynchronizeRenderState()
        {
            frameEndInfo->FocusDistance = FocusDistance;
            frameEndInfo->Flags = 
                (UseProtectedSurface ? FrameInfoFlags.Protected : 0) | 
                (UseVignetteMode ? FrameInfoFlags.Vignette : 0);
            globalDimmerFrameInfo->DimmerValue = GlobalDimmerValue;
            globalDimmerFrameInfo->Flags =
                (GlobalDimmerEnabled) ? GlobalDimmerFlags.Enabled : GlobalDimmerFlags.Disabled;

            MLOpenXRInitializeEndFrameState(frameEndInfo);
            MLOpenXRSetEnvironmentBlendMode(ToNativeBlendMode(BlendMode));
        }
        
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern MLResult.Code MLOpenXRSetEnvironmentBlendMode(NativeInterop.BlendMode blendMode);

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern void MLOpenXRInitializeEndFrameState(NativeInterop.FrameEndInfo* frameEndInfo);
    }
}
#endif
