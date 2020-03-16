// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPurchaseItemDetail.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

using UnityEngine.XR.MagicLeap;

namespace UnityEngine.XR.MagicLeap.Internal
{
    /// <summary>
    /// MLPurchaseItemDetail class is returned on callback after a successful request for available IAPs.
    /// See MLPurchase.GetItemDetails(...)
    /// </summary>
    public struct MLPurchaseItemDetail
    {
        /// <summary>
        /// Use MLPurchase.GetResultString to get the string version of the result.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be MLResult.Code.Ok if successful.
        ///
        /// MLResult.Result will be MLResult.Code.InvalidParam if failed due to internal invalid input parameter.
        ///
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if failed due to internal error.
        ///
        /// MLResult.Result will be MLResult.Code.PrivilegeDenied if necessary privilege is missing.
        ///
        /// MLResult.Result will be MLResult.Code.CloudSystemError if failed due to an issue with the cloud service.
        ///
        /// MLResult.Result will be MLResult.Code.CloudNotFound if failed due to the requested resource was not found on the server.
        ///
        /// MLResult.Result will be MLResult.Code.CloudServerError if failed due to an unexpected server error.
        ///
        /// MLResult.Result will be MLResult.Code.CloudNetworkError if failed due to a network connectivity error for the service call.
        /// </returns>
        public MLResult.Code Result;

        /// <summary>
        /// ItemResults represents an array of the details of the items
        /// </summary>
        public MLPurchaseItemDetailsResult[] ItemResults;
    }
}

#endif
