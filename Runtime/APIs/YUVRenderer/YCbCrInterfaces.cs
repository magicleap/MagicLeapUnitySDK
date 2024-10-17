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
using MagicLeap.Android.NDK.NativeWindow;

namespace UnityEngine.XR.MagicLeap
{
    public partial class YCbCrRenderer
    {
        public interface IFrameAvailabilityProvider
        {
            internal bool IsNewFrameAvailable();
        }
        public interface IFrameTransformMatrixProvider
        {
            internal bool GetFrameTransformMatrix(ref Matrix4x4 frameTransformMatColMajor);
        }
        
        public interface IHardwareBufferProvider
        {
            public bool AcquireNextAvailableHwBuffer(out AHardwareBuffer hwBuffer);

            public void ReleaseHwBuffer(AHardwareBuffer hwBuffer);
        }
        
        public interface IYCbCrConversionSamplerProvider
        {
            /// <summary>
            /// Invoked by the YcbcrRenderer plugin on Unity's render thread.
            /// Apps wanting to override the VkSamplerYcbcrConversionCreateInfo
            /// for the hardware buffer rendering should implement this interface.
            /// </summary>
            /// <param name="hwBufferFormatProperties">VkAndroidHardwareBufferFormatPropertiesANDROID of the currently acquired AHardwareBuffer</param>
            /// <param name="sampler">VkSamplerYcbcrConversionCreateInfo of the previous frame</param>
            /// <returns>true if the input sampler values were changed</returns>
            public bool OverrideYCbCrConversionSampler(ref VkAndroidHardwareBufferFormatPropertiesAndroid hwBufferFormatProperties, ref VkSamplerYCbCrConversionCreateInfo sampler);
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct VkComponentMapping
        {
            /// <summary>
            /// VkComponentSwizzle
            /// </summary>
            public uint r;

            /// <summary>
            /// VkComponentSwizzle
            /// </summary>
            public uint g;

            /// <summary>
            /// VkComponentSwizzle
            /// </summary>
            public uint b;

            /// <summary>
            /// VkComponentSwizzle
            /// </summary>
            public uint a;

            public override bool Equals(object obj)
            {
                if (obj is VkComponentMapping componentMapping)
                {
                    return componentMapping.r == r && componentMapping.g == g && componentMapping.b == b && componentMapping.a == a;
                }

                return false;
            }
            
            public override int GetHashCode()
            {
                return HashCode.Combine(r, g, b, a);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VkAndroidHardwareBufferFormatPropertiesAndroid
        {
            /// <summary>
            /// VkStructureType
            /// </summary>
            public uint sType;

            /// <summary>
            /// void*
            /// </summary>
            public IntPtr pNext;

            /// <summary>
            /// VkFormat
            /// </summary>
            public uint format;

            /// <summary>
            /// uint64
            /// </summary>
            public ulong externalFormat;

            /// <summary>
            /// VkFormatFeatureFlags
            /// </summary>
            public uint formatFeatures;

            /// <summary>
            /// VkComponentMapping
            /// </summary>
            public VkComponentMapping samplerYCbCrConversionComponents;

            /// <summary>
            /// VkSamplerYCbCrModelConversion
            /// </summary>
            public uint suggestedYCbCrModel;

            /// <summary>
            /// VkSamplerYCbCrRange
            /// </summary>
            public uint suggestedYCbCrRange;

            /// <summary>
            /// VkChromaLocation
            /// </summary>
            public uint suggestedXChromaOffset;

            /// <summary>
            /// VkChromaLocation
            /// </summary>
            public uint suggestedYChromaOffset;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VkSamplerYCbCrConversionCreateInfo
        {
            /// <summary>
            /// VkStructureType
            /// </summary>
            public uint sType;

            /// <summary>
            /// void*
            /// </summary>
            public IntPtr pNext;

            /// <summary>
            /// VkFormat
            /// </summary>
            public uint format;

            /// <summary>
            /// VkSamplerYCbCrModelConversion
            /// </summary>
            public uint yCbCrModel;

            /// <summary>
            /// VkSamplerYCbCrRange
            /// </summary>
            public uint yCbCrRange;

            /// <summary>
            /// VkComponentMapping
            /// </summary>
            public VkComponentMapping components;

            /// <summary>
            /// VkChromaLocation
            /// </summary>
            public uint xChromaOffset;

            /// <summary>
            /// VkChromaLocation
            /// </summary>
            public uint yChromaOffset;

            /// <summary>
            /// VkFilter
            /// </summary>
            public uint chromaFilter;

            /// <summary>
            /// VkBool32
            /// </summary>
            public uint forceExplicitReconstruction;
        }
    }
}
