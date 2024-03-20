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
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapSpatialAnchorsStorageFeature
    {
        internal class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern XrResult MLOpenXRCreateSpatialAnchors(NativeInterop.XrUUID* anchorUuids, in int size);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRCreateSpatialAnchorStorage();

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRDestroySpatialAnchorStorage();

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern void MLOpenXRGetSpatialAnchorStorageCompletionCount(int* count);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern void MLOpenXRCheckSpatialAnchorStorageCompletion(MLXrAnchorSubsystem.AnchorCompletionStatus* completed, int* completedArrayLength);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern XrResult MLOpenXRQuerySpatialAnchorsStorage(in Vector3 center, in float radius);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern void MLOpenXRGetSpatialAnchorQueryCount(int* count);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern XrResult MLOpenXRCheckQuerySpatialAnchorCompletion(NativeInterop.XrUUID* anchorUuids, int* size);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern XrResult MLOpenXRPublishSpatialAnchors(ulong* anchorIds, in int size, ulong expiration);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern void MLOpenXRGetSpatialAnchorPublishCount(int* count);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern XrResult MLOpenXRCheckPublishSpatialAnchorsCompletion(NativeInterop.XrUUID* anchorUuids, ulong* anchorIds, int* size);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern XrResult MLOpenXRDeleteSpatialAnchors(NativeInterop.XrUUID* anchorUuids, in int size);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern void MLOpenXRGetSpatialAnchorDeleteCount(int* count);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern XrResult MLOpenXRCheckDeleteSpatialAnchorsCompletion(NativeInterop.XrUUID* anchorUuids, int* size);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern XrResult MLOpenXRUpdateSpatialAnchorsExpiration(NativeInterop.XrUUID* anchorUuids, in int size, ulong expiration);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern void MLOpenXRGetSpatialAnchorUpdateExpirationCount(int* count);

            [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
            public static unsafe extern XrResult MLOpenXRCheckUpdateSpatialAnchorsExpirationCompletion(NativeInterop.XrUUID* anchorUuids, int* size);
        }
    }
}
#endif
