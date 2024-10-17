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
using System.Text;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR
{
    internal unsafe struct XrBaseOutStructure<T> where T: unmanaged
    {
        internal T Type;
        internal XrBaseOutStructure<T>* Next;
    }

    internal struct XrTime
    {
        private long value;
        public static implicit operator long(XrTime time) => time.value;
        public static implicit operator XrTime(long time) =>
            new()
            {
                value = time
            };
    }
    
    internal struct XrBool32
    {
        private uint value;
        public static implicit operator bool(XrBool32 input) => input.value > 0;

        public static implicit operator XrBool32(bool value) =>
            new()
            {
                value = value ? 1U : 0
            };
    }

    internal struct XrSpace
    {
        private ulong handle;
        public static implicit operator ulong(XrSpace xrSpace) => xrSpace.handle;
        public static implicit operator XrSpace(ulong handle) =>
            new()
            {
                handle = handle
            };
    }

    internal struct XrInstance
    {
        private ulong handle;
        public static implicit operator ulong(XrInstance xrInstance) => xrInstance.handle;
        public static implicit operator XrInstance(ulong handle) =>
            new()
            {
                handle = handle
            };
    }

    internal struct XrPath
    {
        private ulong path;
        public static implicit operator ulong(XrPath xrPath) => xrPath.path;

        public static implicit operator XrPath(ulong path) =>
            new()
            {
                path = path
            };
    }
    
    internal struct XrSystemId
    {
        private ulong handle;
        public static implicit operator ulong(XrSystemId xrInstance) => xrInstance.handle;
        public static implicit operator XrSystemId(ulong handle) =>
            new()
            {
                handle = handle
            };
    }
    
    internal struct XrSession
    {
        private ulong handle;
        public static implicit operator ulong(XrSession xrSession) => xrSession.handle;
        public static implicit operator XrSession(ulong handle) =>
            new()
            {
                handle = handle
            };
    }

    internal struct XrRect2Di
    {
        internal Vector2Int Offset;
        internal Vector2Int Extent;
    }

    internal struct XrFov
    {
        internal float AngleLeft;
        internal float AngleRight;
        internal float AngleUp;
        internal float AngleDown;
    }

    internal struct XrEventDataBuffer
    {
        internal ulong Type;
        internal IntPtr Next;
        internal IntPtr Varying;
    }

    internal struct XrPose
    {
        internal Quaternion Rotation;
        internal Vector3 Position;

        internal static XrPose GetFromPose(Pose pose, bool shouldConvert = true)
        {
            var result = new XrPose
            {
                Position = shouldConvert ? pose.position.InvertZ() : pose.position,
                Rotation = shouldConvert ? pose.rotation.InvertXY() : pose.rotation
            };
            return result;
        }

        internal static Pose GetUnityPose(in XrPose pose, bool shouldConvert = true)
        {
            var result = new Pose
            {
                position = shouldConvert ? pose.Position.InvertZ() : pose.Position,
                rotation = shouldConvert ? pose.Rotation.InvertXY() : pose.Rotation
            };
            return result;
        }

        internal static XrPose IdentityPose => GetFromPose(Pose.identity);
    }
    
    internal struct Result
    {
#pragma warning disable 0414
        private long value;
#pragma warning restore 0414

        public bool ActuallySucceeded => value == 0;
        public bool Failed => value < 0;
        public bool Succeeded => value >= 0;
    }

    internal unsafe struct XrUUID
    {
        private static readonly int[] HyphenIndices = { 8, 13, 18, 23 };

        private fixed byte data[16];

        public override string ToString()
        {
            StringBuilder idString = new StringBuilder(20);

            for (int i = 0; i < 16; i++)
            {
                idString.AppendFormat("{0:x2}", this.data[i]);
            }

            foreach (int i in HyphenIndices)
                idString.Insert(i, "-");

            return idString.ToString();
        }

        internal XrUUID(string id)
        {
            id = id.Replace("-", string.Empty);
            fixed (byte* b = this.data)
            {
               StringToByteArray(id, b);
            }
        }

        private static void StringToByteArray(string hex, byte* bytes)
        {
            int numChars = hex.Length;
            for (int i = 0; i < numChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
        }
    }
    
    internal struct XrFrameWaitInfo
    {
        internal XrStructureType Type;
        internal IntPtr Next;
    }

    internal struct XrFrameBeginInfo
    {
        internal XrStructureType Type;
        internal IntPtr Next;
    }

    internal struct XrFrameState
    {
        internal XrStructureType Type;
        internal IntPtr Next;
        internal long PredictedDisplayTime;
        internal ulong PredictedDisplayPeriod;
        internal XrBool32 ShouldRender;
    }

    internal struct XrCompositionLayerBaseHeader
    {
        internal XrStructureType Type;
        internal IntPtr Next;
        internal XrCompositionLayerFlags LayerFlags;
        internal XrSpace Space;

        internal unsafe void AppendToLayer(IntPtr nativePtr, int index)
        {
            fixed (XrCompositionLayerBaseHeader* layer = &this)
            {
                var currentIndex = 0;
                var currentChain = (XrBaseOutStructure<uint>*)layer;
                while (currentIndex < index && currentChain->Next != null)
                {
                    currentChain = currentChain->Next;
                    currentIndex++;
                }

                var targetStruct = (XrBaseOutStructure<uint>*)nativePtr;
                var targetNext = targetStruct;
                while (targetNext->Next != null)
                {
                    targetNext = targetNext->Next;
                }
                targetNext->Next = currentChain->Next;
                currentChain->Next = targetStruct;
            }
        }
    }

    internal struct XrFrameEndInfo
    {
        internal XrStructureType Type;
        internal IntPtr Next;
        internal long DisplayTime;
        internal XrEnvironmentBlendMode EnvironmentBlendMode;
        internal uint LayerCount;
        internal IntPtr Layers;

        internal unsafe void AppendToProjectionLayer(IntPtr nativePtr, int index = 0, bool includeSecondaryViews = false)
        {
            var layers = (XrCompositionLayerBaseHeader**)Layers;
            if (layers != null)
            {
                for (var i = 0; i < LayerCount; i++)
                {
                    var layer = layers[i];
                    if (layer->Type != XrStructureType.XrTypeCompositionLayerProjection)
                    {
                        continue;
                    }
                    layer->AppendToLayer(nativePtr, index);
                }
            }

            if (!includeSecondaryViews)
            {
                return;
            }

            if (Next == IntPtr.Zero)
            {
                return;
            }

            var secondaryView = (XrSecondaryViewConfigurationFrameEndInfoMSFT*)Next;
            if (secondaryView == null || secondaryView->ViewConfigurationLayersInfo == null)
            {
                return;
            }
            for (var i = 0; i < secondaryView->ViewConfigurationCount; i++)
            {
                var secondaryLayerInfo = secondaryView->ViewConfigurationLayersInfo[i];
                var secondaryLayers = (XrCompositionLayerBaseHeader**)secondaryLayerInfo.Layers;
                if (secondaryLayers == null)
                {
                    continue;
                }

                for (var j = 0; j < secondaryLayerInfo.LayerCount; j++)
                {
                    var layer = secondaryLayers[j];
                    if (layer == null || layer->Type != XrStructureType.XrTypeCompositionLayerProjection)
                    {
                        continue;
                    }
                    layer->AppendToLayer(nativePtr, index);
                }
            }
        }
    }

    internal struct FeatureLifecycleNativeListenerInternal
    {
        internal IntPtr InstanceCreatedIntPtr;
        internal IntPtr InstanceDestroyedIntPtr;
        internal IntPtr SessionCreateIntPtr;
        internal IntPtr SessionDestroyIntPtr;
        internal IntPtr AppSpaceChangedIntPtr;
        internal IntPtr PredictedDisplayTimeChangedIntPtr;
    }

    internal unsafe struct FeatureLifecycleNativeListener
    {
        private delegate* unmanaged [Cdecl] <XrInstance, IntPtr, void> instanceCreated;
        private delegate* unmanaged [Cdecl] <XrInstance, void> instanceDestroyed;
        private delegate* unmanaged [Cdecl] <XrSession, void> sessionCreate;
        private delegate* unmanaged [Cdecl] <XrSession, void> sessionDestroy;
        private delegate* unmanaged [Cdecl] <XrSpace, void> appSpaceChanged;
        private delegate* unmanaged [Cdecl] <long, void> predictedDisplayTimeChanged;

        private void Initialize(FeatureLifecycleNativeListenerInternal featureLifecycleNativeListenerInternal)
        {
            instanceCreated = (delegate* unmanaged[Cdecl]<XrInstance, IntPtr, void>)featureLifecycleNativeListenerInternal.InstanceCreatedIntPtr;
            instanceDestroyed = (delegate* unmanaged[Cdecl]<XrInstance, void>)featureLifecycleNativeListenerInternal.InstanceDestroyedIntPtr;
            sessionCreate = (delegate* unmanaged[Cdecl]<XrSession, void>)featureLifecycleNativeListenerInternal.SessionCreateIntPtr;
            sessionDestroy = (delegate* unmanaged[Cdecl]<XrSession, void>)featureLifecycleNativeListenerInternal.SessionDestroyIntPtr;
            appSpaceChanged = (delegate* unmanaged[Cdecl]<XrSpace, void>)featureLifecycleNativeListenerInternal.AppSpaceChangedIntPtr;
            predictedDisplayTimeChanged = (delegate* unmanaged[Cdecl]<long, void>)featureLifecycleNativeListenerInternal.PredictedDisplayTimeChangedIntPtr;
        }

        public static implicit operator FeatureLifecycleNativeListener(FeatureLifecycleNativeListenerInternal listenerInternal)
        {
            var result = new FeatureLifecycleNativeListener();
            result.Initialize(listenerInternal);
            return result;
        }
        
        public void InstanceCreated(XrInstance instance, IntPtr hookAddr)
        {
            if (instanceCreated != null)
            {
                instanceCreated(instance, hookAddr);
            }
        }

        public void InstanceDestroyed(XrInstance instance)
        {
            if (instanceDestroyed != null)
            {
                instanceDestroyed(instance);
            }
        }

        public void SessionCreated(XrSession session)
        {
            if (sessionCreate != null)
            {
                sessionCreate(session);
            }
        }

        public void SessionDestroyed(XrSession session)
        {
            if (sessionDestroy != null)
            {
                sessionDestroy(session);
            }
        }

        public void AppSpaceChanged(XrSpace space)
        {
            if (appSpaceChanged != null)
            {
                appSpaceChanged(space);
            }
        }

        public void PredictedDisplayTimeChanged(long time)
        {
            if (predictedDisplayTimeChanged != null)
            {
                predictedDisplayTimeChanged(time);
            }
        }
    }

    internal struct XrSwapChainSubImage
    {
        internal ulong SwapChain;
        internal XrRect2Di ImageRect;
        internal uint ImageArrayIndex;
    }

    internal struct XrCompositionLayerProjectionView
    {
        internal XrStructureType Type;
        internal IntPtr Next;
        internal XrPose Pose;
        internal XrFov FOV;
    }

    internal unsafe struct XrCompositionLayerProjection
    {
        internal XrStructureType Type;
        internal IntPtr Next;
        internal XrCompositionLayerFlags LayerFlags;
        internal ulong Space;
        internal uint ViewCount;
        internal XrCompositionLayerProjectionView* Views;
    }

    internal enum XrStructureType
    {
        XrTypeFrameEndInfo = 12,
        XrTypeCompositionLayerProjectionView = 48,
        XrTypeCompositionLayerProjection = 35,
    }

    internal enum XrCompositionLayerFlags : ulong
    {
        CorrectChomaticAberrationBit = 0x00001,
        BlendTextureSourceAlpha = 2,
        UnPreMultipliedAlpha = 4,
    }

    internal struct XrSecondaryViewConfigurationLayerInfoMSFT
    {
        internal uint Type;
        internal IntPtr Next;
        internal XrViewConfigurationType ViewConfigurationType;
        internal XrEnvironmentBlendMode EnvironmentBlendMode;
        internal uint LayerCount;
        internal IntPtr Layers;
    }

    internal unsafe struct XrSecondaryViewConfigurationFrameEndInfoMSFT
    {
        internal uint Type;
        internal IntPtr Next;
        internal uint ViewConfigurationCount;
        internal XrSecondaryViewConfigurationLayerInfoMSFT* ViewConfigurationLayersInfo;
    }
}
