// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.MagicLeap.Unsafe;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapRenderingFeatureNativeTypes;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRNativeDelegates;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif
namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features")]
    public unsafe class MagicLeapRenderingExtensionsFeature : MagicLeapOpenXRFeatureWithInterception<MagicLeapRenderingExtensionsFeature>
    {   
        public const string FeatureId = "com.magicleap.openxr.feature.rendering_extensions";
        public float FocusDistance;
        public bool UseProtectedSurface;
        public bool UseVignetteMode;

        public bool GlobalDimmerEnabled;
        public float GlobalDimmerValue;

        public XrEnvironmentBlendMode BlendMode = XrEnvironmentBlendMode.AlphaBlend;


        private XrFrameEndInfoML* frameEndInfoML;
        private XrGlobalDimmerFrameEndInfoML* globalDimmerFrameInfo;

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
            if (frameEndInfoML != null)
                UnsafeUtility.FreeTracked(frameEndInfoML, Allocator.Persistent);
            
            if (globalDimmerFrameInfo != null)
                UnsafeUtility.FreeTracked(globalDimmerFrameInfo, Allocator.Persistent);

            Application.onBeforeRender -= SynchronizeRenderState;
        }

        private void InitializeOpenXRState()
        {
            frameEndInfoML = UnsafeUtilityEx.MallocTracked<XrFrameEndInfoML>(Allocator.Persistent);
            frameEndInfoML->Type = XrRenderingStructTypes.XrTypeFrameEndInfoML;
            
            globalDimmerFrameInfo = UnsafeUtilityEx.MallocTracked<XrGlobalDimmerFrameEndInfoML>(Allocator.Persistent);
            globalDimmerFrameInfo->Type = XrRenderingStructTypes.XrTypeGlobalDimmerFrameEndInfo;
            frameEndInfoML->Next = new IntPtr(globalDimmerFrameInfo);
        }
        
        private void ValidateBlendMode()
        {
            if (BlendMode == XrEnvironmentBlendMode.Opaque)
            {
                throw new NotSupportedException($"XrEnvironmentBlendMode.Opaque is not supported on Magic Leap 2");
            }
        }

        private void SynchronizeRenderState()
        {
            frameEndInfoML->FocusDistance = FocusDistance;
            frameEndInfoML->Flags = 
                (UseProtectedSurface ? XrFrameEndInfoFlagsML.Protected : 0) | 
                (UseVignetteMode ? XrFrameEndInfoFlagsML.Vignette : 0);
            globalDimmerFrameInfo->DimmerValue = GlobalDimmerValue;
            globalDimmerFrameInfo->Flags = (GlobalDimmerEnabled) ? XrGlobalDimmerFrameEndInfoFlags.Enabled : 0;
            ValidateBlendMode();
        }

        internal override XrResult OnEndFrame(ulong session, XrFrameEndInfo* origFrameEndInfo, XrEndFrame origEndFrame)
        {
            var frameEndInfo = new XrFrameEndInfo
            {
                Type = XrRenderingStructTypes.XrTypeFrameEndInfo,
                DisplayTime = origFrameEndInfo->DisplayTime,
                EnvironmentBlendMode = BlendMode,
                LayerCount =  origFrameEndInfo->LayerCount,
                Layers = origFrameEndInfo->Layers,
                Next = origFrameEndInfo->Next
            };
            
            var end = (XrBaseOutStructure*)&frameEndInfo;
            while (end->Next != null)
            {
                end = end->Next;
            }

            end->Next = (XrBaseOutStructure*)(frameEndInfoML);
            var result = origEndFrame(session, &frameEndInfo);
            return result;
        }

        protected override void MarkFunctionsToIntercept()
        {
            InterceptEndFrame = true;
        }
    }
}
