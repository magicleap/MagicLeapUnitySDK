// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPurchaseNativeBindings.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

#if PLATFORM_LUMIN
using UnityEngine.XR.MagicLeap.Native;
#endif

// Disable warnings about missing documentation for native interop.
#pragma warning disable 1591

namespace UnityEngine.XR.MagicLeap.Internal
{
    // TODO: currently used for 0.14.0, would eventually be moved to MLResult.Code.Pending in 0.15.0
    // from ml_cloud.h
    public enum MLCloudStatus
    {
        NotDone = 0,
        Done
    }

    public enum MLPurchaseItemDetails
    {
        MaximumIdsPerRequest = 20
    };

    public enum MLPurchaseType
    {
        Consumable,
        Nonconsumable
    }

    #if PLATFORM_LUMIN
    [StructLayout(LayoutKind.Sequential)]
    public struct MLPurchaseItemDetailsQuery
    {
        public IntPtr ids; // string[]
        public uint idCount;

        /// <summary>
        /// Create and return an initialized version of this struct.
        /// </summary>
        public static MLPurchaseItemDetailsQuery Create()
        {
            return new MLPurchaseItemDetailsQuery()
            {
                ids = IntPtr.Zero,
                idCount = 0u
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MLPurchaseItemDetailsResult
    {
        public string iapId;
        public string price;
        public string name;
        public MLPurchaseType type;
        public string token;
        [MarshalAs(UnmanagedType.I1)]
        public bool isFree;

        /// <summary>
        /// Create and return an initialized version of this struct.
        /// </summary>
        public static MLPurchaseItemDetailsResult Create()
        {
            return new MLPurchaseItemDetailsResult()
            {
                iapId = string.Empty,
                price = string.Empty,
                name = string.Empty,
                type = MLPurchaseType.Consumable,
                token = string.Empty,
                isFree = false
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MLPurchaseItemDetailsResults
    {
        public uint count;
        public IntPtr itemDetails; // MLPurchaseItemDetailsResult[]

        /// <summary>
        /// Create and return an initialized version of this struct.
        /// </summary>
        public static MLPurchaseItemDetailsResults Create()
        {
            return new MLPurchaseItemDetailsResults()
            {
                count = 0u,
                itemDetails = IntPtr.Zero
            };
        }
    }

    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
    public struct MLPurchaseConfirmation
    {
        public string orderId;
        public string packageName; // com.company.application
        public ulong purchaseTime; // number of seconds since Unix Epoch
        public string signature;

        public string iapId;
        public MLPurchaseType type;

        /// <summary>
        /// Create and return an initialized version of this struct.
        /// </summary>
        public static MLPurchaseConfirmation Create()
        {
            return new MLPurchaseConfirmation()
            {
                orderId = string.Empty,
                packageName = string.Empty,
                purchaseTime = 0,
                signature = string.Empty,
                iapId = string.Empty,
                type = MLPurchaseType.Consumable
            };
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MLPurchaseHistoryResult
    {
        public uint count;
        public IntPtr confirmations; // MLPurchaseConfirmation[]
        [MarshalAs(UnmanagedType.I1)]
        public bool hasNextPage;
        public MLCloudStatus status;

        /// <summary>
        /// Create and return an initialized version of this struct.
        /// </summary>
        public static MLPurchaseHistoryResult Create()
        {
            return new MLPurchaseHistoryResult()
            {
                count = 0u,
                confirmations = IntPtr.Zero,
                hasNextPage = false,
                status = MLCloudStatus.NotDone
            };
        }
    }
    #endif
}

#if PLATFORM_LUMIN
namespace UnityEngine.XR.MagicLeap.Internal
{
    /// <summary>
    /// See ml_purchase.h for additional comments
    /// </summary>
    public class MLPurchaseNativeBindings : MagicLeapNativeBindings
    {
        public const string MLPurchaseDll = "ml_purchase";

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseItemDetailsCreate(ref ulong outHandle);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseItemDetailsGet(ulong itemDetailsHandle, ref MLPurchaseItemDetailsQuery request);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseItemDetailsGetResult(ulong itemDetailsHandle, ref MLPurchaseItemDetailsResults outItemDetailsResult, ref MLCloudStatus outStatus);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseItemDetailsDestroy(ulong itemDetailsHandle);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseCreate(ref ulong outHandle);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseSubmit(ulong purchaseHandle, string token);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseGetResult(ulong purchaseHandle, ref MLPurchaseConfirmation outConfirmation, ref MLCloudStatus outStatus);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseDestroy(ulong purchaseHandle);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseHistoryQueryCreate(ref ulong outHandle);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseHistoryQueryGetPage(ulong historyHandle, uint limit);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseHistoryQueryGetPageResult(ulong historyHandle, ref MLPurchaseHistoryResult outPageResult);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPurchaseHistoryQueryDestroy(ulong historyHandle);

        [DllImport(MLPurchaseDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr MLPurchaseGetResultString(MLResult.Code resultCode);
    }
}

#endif
