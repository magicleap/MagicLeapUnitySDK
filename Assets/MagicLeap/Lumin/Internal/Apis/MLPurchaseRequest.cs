// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPurchaseRequest.cs" company="Magic Leap, Inc">
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
    /// MLPurchaseRequest class is returned on callback after successfully submitting a purchase request.
    /// See MLPurchase.MakePurchase(...)
    /// </summary>
    public class MLPurchaseRequest
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
        /// MLResult.Result will be MLResult.Code.CloudSystemError if failed due to an issue with the cloud service.
        ///
        /// MLResult.Result will be MLResult.Code.CloudNotFound if failed due to the requested resource was not found on the server.
        ///
        /// MLResult.Result will be MLResult.Code.CloudServerError if failed due to an unexpected server error.
        ///
        /// MLResult.Result will be MLResult.Code.CloudNetworkError if failed due to a network connectivity error for the service call.
        ///
        /// MLResult.Result will be MLResult.Code.PurchaseUserCancelled if failed due to the user cancelling the purchase via the confirmation UI.
        ///
        /// MLResult.Result will be MLResult.Code.PurchaseItemAlreadyPurchased if failed because the non-consumable item has already been purchased.
        ///
        /// MLResult.Result will be MLResult.Code.PurchasePaymentSourceError if failed due to a failure with the payment source.
        ///
        /// MLResult.Result will be MLResult.Code.PurchaseInvalidPurchaseToken if failed due an invalid token representing the in-app purchase.
        ///
        /// MLResult.Result will be MLResult.Code.PurchaseItemUnavailable if failed due to the item being no longer available.
        ///
        /// MLResult.Result will be MLResult.Code.PurchaseNoDefaultPayment if failed due to the user failing to configured a default payment method.
        /// </returns>
        public MLResult.Code Result;

        /// <summary>
        /// PurchaseConfirmation represents the details of the order
        /// </summary>
        public MLPurchaseConfirmation PurchaseConfirmation;

        /// <summary>
        /// Initialize properties
        /// </summary>
        public MLPurchaseRequest()
        {
            Result = MLResult.Code.UnspecifiedFailure;
            PurchaseConfirmation = MLPurchaseConfirmation.Create();
        }
    }
}

#endif
