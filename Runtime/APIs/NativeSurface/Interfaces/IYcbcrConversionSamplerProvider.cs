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

namespace UnityEngine.XR.MagicLeap
{
    public partial class YcbcrRenderer
    {
        public interface IYcbcrConversionSamplerProvider
        {
            /// <summary>
            /// Invoked by the YcbcrRenderer plugin on Unity's render thread.
            /// Apps wanting to override the VkSamplerYcbcrConversionCreateInfo
            /// for the hardware buffer rendering should implement this interface.
            /// </summary>
            /// <param name="hwBufferFormatProperties">VkAndroidHardwareBufferFormatPropertiesANDROID of the currently acquired AHardwareBuffer</param>
            /// <param name="sampler">VkSamplerYcbcrConversionCreateInfo of the previous frame</param>
            /// <returns>true if the input sampler values were changed</returns>
            public abstract bool OverrideYcbcrConversionSampler(ref VkAndroidHardwareBufferFormatPropertiesANDROID hwBufferFormatProperties, ref VkSamplerYcbcrConversionCreateInfo sampler);
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
                VkComponentMapping other = (VkComponentMapping)obj;
                return (r == other.r)
                    && (g == other.g)
                    && (b == other.b)
                    && (a == other.a);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct VkAndroidHardwareBufferFormatPropertiesANDROID
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
            public VkComponentMapping samplerYcbcrConversionComponents;

            /// <summary>
            /// VkSamplerYcbcrModelConversion
            /// </summary>
            public uint suggestedYcbcrModel;

            /// <summary>
            /// VkSamplerYcbcrRange
            /// </summary>
            public uint suggestedYcbcrRange;

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
        public struct VkSamplerYcbcrConversionCreateInfo
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
            /// VkSamplerYcbcrModelConversion
            /// </summary>
            public uint ycbcrModel;

            /// <summary>
            /// VkSamplerYcbcrRange
            /// </summary>
            public uint ycbcrRange;

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
