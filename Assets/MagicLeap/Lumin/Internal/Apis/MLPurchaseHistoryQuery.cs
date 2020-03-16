// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPurchaseHistoryQuery.cs" company="Magic Leap, Inc">
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
    /// MLPurchaseHistoryQuery class is returned on callback after successfully submitting a query for the purchase history.
    /// See MLPurchase.MakePurchaseHistoryQuery(...) or MLPurchase.GetRecentPurchaseHistory(...)
    /// </summary>
    public class MLPurchaseHistoryQuery
    {
        /// <summary>
        /// Use MLPurchase.GetResultString to get the string version of the result.
        ///</summary>
        /// <returns>
        /// MLResult.Result will be MLResult.Code.Ok if successful.
        ///
        /// MLResult.Result will be MLResult.Code.InvalidParam if failed due to internal invalid input parameter.
        ///
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if failed due to internal error.
        ///
        /// MLResult.Result will be MLResult.Code.CloudSystemError if failed due to an issue with the cloud service.
        ///
        /// MLResult.Result will be MLResult.Code.CloudServerError if there is an unexepected server error.
        ///
        /// MLResult.Result will be MLResult.Code.CloudNetworkError is returned if there is a network connectivity error for the service call.
        ///
        /// </returns>
        public MLResult.Code Result;

        /// <summary>
        /// PurchaseConfirmations represents a list of the details of the orders
        /// </summary>
        public List<MLPurchaseConfirmation> PurchaseConfirmations;

        /// <summary/>
        public MLPurchaseHistoryQuery()
        {
            Result = MLResult.Code.UnspecifiedFailure;
            PurchaseConfirmations = new List<MLPurchaseConfirmation>();
        }
    }
}

#endif
