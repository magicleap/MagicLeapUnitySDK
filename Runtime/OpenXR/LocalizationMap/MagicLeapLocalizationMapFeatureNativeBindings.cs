// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_OPENXR_1_9_0_OR_NEWER
using System;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.NativeInterop;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapLocalizationMapFeature
    {
        internal class NativeBindings : MagicLeapNativeBindings
        {
            [AOT.MonoPInvokeCallback(typeof(OnEventDataLocalizationChangedCallback))]
            internal static void HandleOnEventDataLocalizationChanged(ref XrEventDataLocalizationChangedML eventData)
            {
                LocalizationEventData data = new(eventData);
                MLThreadDispatch.ScheduleMain(() =>
                {
                    OnLocalizationChanged?.Invoke(data);
                });
            }

            #region NativeStructs
            internal struct XrEventDataLocalizationChangedML
            {
                internal int State;

                internal XrLocalizationMapML Map;

                internal int Confidence;

                internal ulong ErrorFlags;
            }

            internal unsafe struct XrLocalizationMapML
            {
                internal byte* Name;

                internal byte* MapUUid;

                internal LocalizationMapType MapType;

                internal static XrLocalizationMapML Create()
                {
                    var result = new XrLocalizationMapML();
                    result.Name = (byte*)new NativeArray<byte>((int)LocalizationMapNameSize, Allocator.Temp).GetUnsafePtr();
                    result.MapUUid = (byte*)new NativeArray<byte>(16, Allocator.Temp).GetUnsafePtr();
                    return result;
                }

                internal static string GetName(XrLocalizationMapML map)
                {
                    var name = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(map.Name, (int)LocalizationMapNameSize, Allocator.Temp).ToArray();
                    return Encoding.UTF8.GetString(name).TrimEnd('\0');
                }

                internal static string GetMapUuid(XrLocalizationMapML map)
                {
                    var bytes = NativeArrayUnsafeUtility.ConvertExistingDataToNativeArray<byte>(map.MapUUid, 16, Allocator.Temp);
                    var uuid = new XrUUID();
                    
                    unsafe
                    {
                        UnsafeUtility.MemCpy(uuid.Data, map.MapUUid, sizeof(byte)*16);
                    }
                    return uuid.ToString();
                }
            }

            internal unsafe struct XrLocalizationMapImportInfoML
            {
                internal int Type;

                internal IntPtr Next;

                internal uint Size;

                internal byte* Data;

                internal XrLocalizationMapImportInfoML(byte[] data)
                {
                    Type = 0;
                    Next = IntPtr.Zero;
                    Data = (byte*)new NativeArray<byte>(data, Allocator.Temp).GetUnsafePtr();
                    Size = (uint)data.Length;
                }
            }

            internal struct XrLocalizationEnableEventsInfoML
            {
                internal int Type;

                internal IntPtr Next;

                internal uint Size;

                internal bool Enabled;
            }
            #endregion

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXREnableLocalizationEvents(bool enableEvents);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern XrResult MLOpenXRQueryLocalizationMaps(uint maxMapSize, out uint mapOutputSize, XrLocalizationMapML* mapsPtr);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRRequestMapLocalization(NativeInterop.XrUUID xrUUID);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRCreateExportedLocalizationMap(in NativeInterop.XrUUID xrUUID, out ulong mapHandle);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRGetExportedLocalizationMapData(ulong mapHandle, uint maxMapSize, out uint mapOutputDataSize, byte[] mapData);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRDestroyExportedLocalizationMap(ulong mapHandle);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRImportLocalizationMap(ref XrLocalizationMapImportInfoML requestInfo, out NativeInterop.XrUUID xrUUID);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRLocalizationMapRegisterCallback(OnEventDataLocalizationChangedCallback callback);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRLocalizationMapClearCallback();

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public unsafe static extern bool MLOpenXRGetLocalizationMapData(out XrEventDataLocalizationChangedML data);
        }
    }
}
#endif // UNITY_OPENXR_1_9_0_OR_NEWER
