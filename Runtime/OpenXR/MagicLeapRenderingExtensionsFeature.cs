#if UNITY_OPENXR_1_7_0_OR_NEWER
using System;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Unsafe;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif
namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    using Native = MagicLeapFeature.NativeBindings;
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Rendering Extenstions Support",
        Desc="Support for controlling rendering features specific to Magic Leap 2.",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new []{ BuildTargetGroup.Android, BuildTargetGroup.Standalone },
        FeatureId = featureId,
        OpenxrExtensionStrings = "XR_ML_frame_end_info " +
                                 "XR_ML_global_dimmer "
    )]
#endif
    public unsafe class MagicLeapRenderingExtensionsFeature : MagicLeapOpenXRFeatureBase
    {
        public enum BlendMode
        {
            Additive,
            AlphaBlend,
        }
        
        public const string featureId = "com.magicleap.openxr.feature.rendering_extensions";

        public float focusDistance = 0.0f;
        public bool useProtectedSurface = false;
        public bool useVignetteMode = false;

        public bool globalDimmerEnabled = false;
        public float globalDimmerValue = 0.0f;

        public BlendMode blendMode;

        [NonSerialized] 
        private FrameEndInfo* m_FrameEndInfo;

        [NonSerialized]
        private GlobalDimmerFrameEndInfo* m_GlobalDimmerFrameInfo;

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
            if (m_FrameEndInfo != null)
                UnsafeUtility.FreeTracked(m_FrameEndInfo, Allocator.Persistent);
            
            if (m_GlobalDimmerFrameInfo != null)
                UnsafeUtility.FreeTracked(m_GlobalDimmerFrameInfo, Allocator.Persistent);

            Application.onBeforeRender -= SynchronizeRenderState;
        }

        private void InitializeOpenXRState()
        {
            m_FrameEndInfo = UnsafeUtilityEx.MallocTracked<FrameEndInfo>(Allocator.Persistent, 1);
            *m_FrameEndInfo = FrameEndInfo.Init();
            
            m_GlobalDimmerFrameInfo =
                UnsafeUtilityEx.MallocTracked<GlobalDimmerFrameEndInfo>(Allocator.Persistent, 1);
            *m_GlobalDimmerFrameInfo = GlobalDimmerFrameEndInfo.Init();

            m_FrameEndInfo->next = m_GlobalDimmerFrameInfo;
            
            MLOpenXRInitializeEndFrameState(m_FrameEndInfo);
        }

        private NativeInterop.BlendMode ToNativeBlendMode(BlendMode blendMode)
        {
            switch (blendMode)
            {
                case BlendMode.Additive:
                    return NativeInterop.BlendMode.Additive;
                case BlendMode.AlphaBlend:
                    return NativeInterop.BlendMode.AlphaBlend;
                default:
                    throw new ArgumentException(nameof(blendMode));
            }
            
        }

        private void SynchronizeRenderState()
        {
            m_FrameEndInfo->focusDistance = focusDistance;
            ref var flags = ref m_FrameEndInfo->flags;
            flags = FrameInfoFlags.None;
            if (useProtectedSurface)
                flags &= FrameInfoFlags.Protected;
            if (useVignetteMode)
                flags &= FrameInfoFlags.Vignette;

            m_GlobalDimmerFrameInfo->dimmerValue = globalDimmerValue;
            m_GlobalDimmerFrameInfo->flags =
                (globalDimmerEnabled) ? GlobalDimmerFlags.Enabled : GlobalDimmerFlags.Disabled;

            MLOpenXRSetEnvironmentBlendMode(ToNativeBlendMode(blendMode));
        }
        
        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern MLResult.Code MLOpenXRSetEnvironmentBlendMode(NativeInterop.BlendMode blendMode);

        [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
        private static extern void MLOpenXRInitializeEndFrameState(NativeInterop.FrameEndInfo* frameEndInfo);
    }
}
#endif
