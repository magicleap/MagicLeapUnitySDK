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
using MagicLeap.OpenXR.NativeDelegates;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.MagicLeap.Unsafe;
using UnityEngine.XR.OpenXR.NativeTypes;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.OpenXR.Features;
#endif

namespace MagicLeap.OpenXR.Features
{
#if UNITY_EDITOR
    [OpenXRFeature(UiName = "Magic Leap 2 Rendering Extensions",
        Desc="Support for controlling rendering features specific to Magic Leap 2, such as ",
        Company = "Magic Leap",
        Version = "1.0.0",
        BuildTargetGroups = new []{ BuildTargetGroup.Android },
        FeatureId = FeatureId,
        OpenxrExtensionStrings = "XR_ML_frame_end_info " +
                                 "XR_ML_global_dimmer "
    )]
#endif
    public unsafe partial class MagicLeapRenderingExtensionsFeature : MagicLeapOpenXRFeatureWithInterception<MagicLeapRenderingExtensionsFeature>
    {   
        public const string FeatureId = "com.magicleap.openxr.feature.rendering_extensions";

        [SerializeField, FormerlySerializedAs("BlendMode")]
        private XrEnvironmentBlendMode defaultBlendModeSelection = XrEnvironmentBlendMode.Additive;
        public float FocusDistance;
        public bool UseProtectedSurface;
        public bool UseVignetteMode;
        public bool GlobalDimmerEnabled;
        public float GlobalDimmerValue;

        // This is the value which will actually be set in the onEndFrame interception
        private XrEnvironmentBlendMode requestedBlendMode = XrEnvironmentBlendMode.Additive;

        /// <summary>
        /// Controls alpha passthrough behavior and determines whether Segmented Dimming will be visible in the scene
        /// </summary>
        public XrEnvironmentBlendMode BlendMode
        {
            get => requestedBlendMode;
            set
            {
                if (value == XrEnvironmentBlendMode.Opaque)
                    Debug.LogError($"XrEnvironmentBlendMode.Opaque is not currently supported on Magic Leap.");
                else
                    requestedBlendMode = value;
            }
        }

        private XrFrameEndInfoML* frameEndInfoML;
        private XrGlobalDimmerFrameEndInfoML* globalDimmerFrameInfo;

        protected override void OnSessionBegin(ulong xrSession)
        {
            base.OnSessionBegin(xrSession);
            Cleanup();
            InitializeOpenXRState();

            Application.onBeforeRender += SynchronizeRenderState;
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            requestedBlendMode = defaultBlendModeSelection;
            SceneManager.sceneLoaded -= OnSceneLoaded;
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
            frameEndInfoML = UnsafeUtilityEx.CallocTracked<XrFrameEndInfoML>(Allocator.Persistent);
            frameEndInfoML->Type = XrRenderingStructTypes.XrTypeFrameEndInfoML;
            
            globalDimmerFrameInfo = UnsafeUtilityEx.CallocTracked<XrGlobalDimmerFrameEndInfoML>(Allocator.Persistent);
            globalDimmerFrameInfo->Type = XrRenderingStructTypes.XrTypeGlobalDimmerFrameEndInfo;
            frameEndInfoML->Next = new IntPtr(globalDimmerFrameInfo);
        }

        private void SynchronizeRenderState()
        {
            frameEndInfoML->FocusDistance = FocusDistance;
            frameEndInfoML->Flags = 
                (UseProtectedSurface ? XrFrameEndInfoFlagsML.Protected : 0) | 
                (UseVignetteMode ? XrFrameEndInfoFlagsML.Vignette : 0);
            globalDimmerFrameInfo->DimmerValue = GlobalDimmerValue;
            globalDimmerFrameInfo->Flags = (GlobalDimmerEnabled) ? XrGlobalDimmerFrameEndInfoFlags.Enabled : 0;
        }

        internal override XrResult OnEndFrame(ulong session, XrFrameEndInfo* origFrameEndInfo, XrEndFrame origEndFrame)
        {
            var frameEndInfo = *origFrameEndInfo;
            frameEndInfo.EnvironmentBlendMode = requestedBlendMode;
            
            var end = (XrBaseOutStructure<uint>*)&frameEndInfo;
            while (end->Next != null)
            {
                end = end->Next;
            }
            end->Next = (XrBaseOutStructure<uint>*)(frameEndInfoML);
            var result = origEndFrame(session, &frameEndInfo);
            return result;
        }

        protected override void MarkFunctionsToIntercept()
        {
            InterceptEndFrame = true;
        }
    }
}
