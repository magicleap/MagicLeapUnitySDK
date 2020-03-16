// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPurchase.cs" company="Magic Leap, Inc">
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
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap.Internal
{
    /// <summary>
    /// MLPurchase class is the entry point for the Purchase API
    /// </summary>
    public sealed class MLPurchase : MLAPISingleton<MLPurchase>
    {
        /// <summary>
        /// An invalid purchase token to initialize tokens to.
        /// </summary>
        public const string InvalidPurchaseToken = "invalid token";

        /// <summary>
        /// Maximum number of items to return in a Purchase History query. NOTE: This is not found in the platform side.
        /// </summary>
        private const uint MaxPurchaseHistoryItems = 1;

        private class ItemDetailQueryInfo
        {
            public ulong Handle
            {
                get { return _handle; }
            }
            private ulong _handle = MagicLeapNativeBindings.InvalidHandle;
            public MLPurchaseItemDetail Details;
            public Action<MLPurchaseItemDetail> Callback
            {
                get; private set;
            }

            public ItemDetailQueryInfo(ulong newHandle, Action<MLPurchaseItemDetail> newCallback)
            {
                _handle = newHandle;
                Details = new MLPurchaseItemDetail();
                Callback = newCallback;
            }
        }

        private class PurchaseRequestInfo
        {
            public ulong Handle
            {
                get { return _handle; }
            }
            private ulong _handle = MagicLeapNativeBindings.InvalidHandle;
            public MLPurchaseRequest Details;
            public Action<MLPurchaseRequest> Callback
            {
                get; private set;
            }

            public PurchaseRequestInfo(ulong newHandle, Action<MLPurchaseRequest> newCallback)
            {
                _handle = newHandle;
                Details = new MLPurchaseRequest();
                Callback = newCallback;
            }
        }

        private class PurchaseHistoryQueryInfo
        {
            public ulong Handle
            {
                get { return _handle; }
            }
            private ulong _handle = MagicLeapNativeBindings.InvalidHandle;
            public MLPurchaseHistoryQuery Details;
            public Action<MLPurchaseHistoryQuery> Callback
            {
                get; private set;
            }
            public uint NumItemsLeft;
            public bool FetchAll
            {
                get; private set;
            }

            public PurchaseHistoryQueryInfo(ulong newHandle, Action<MLPurchaseHistoryQuery> newCallback)
            {
                _handle = newHandle;
                Details = new MLPurchaseHistoryQuery();
                Callback = newCallback;
                NumItemsLeft = UInt32.MaxValue; // this value is ignored when FetchAll is true
                FetchAll = true;
            }

            public PurchaseHistoryQueryInfo(ulong newHandle, Action<MLPurchaseHistoryQuery> newCallback, uint newNumItems)
            {
                _handle = newHandle;
                Details = new MLPurchaseHistoryQuery();
                Callback = newCallback;
                NumItemsLeft = newNumItems;
                FetchAll = false;
            }
        }

        private List<ItemDetailQueryInfo> _pendingItemDetailQueries;
        private List<PurchaseRequestInfo> _pendingPurchaseRequests;
        private List<PurchaseHistoryQueryInfo> _pendingPurchaseHistoryQueries;

        private static void CreateInstance()
        {
            if (!IsValidInstance())
            {
                _instance = new MLPurchase();
            }
        }

        /// <summary>
        /// Start the Purchase API.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be MLResult.Code.Ok if successful.
        ///
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if failed due to internal error (no device, dll not found, no API symbols).
        /// </returns>
        public static MLResult Start()
        {
            CreateInstance();
            return BaseStart();
        }

        /// <summary>
        /// Initialize private containers
        /// </summary>
        private MLPurchase()
        {
            _pendingItemDetailQueries = new List<ItemDetailQueryInfo>();
            _pendingPurchaseRequests = new List<PurchaseRequestInfo>();
            _pendingPurchaseHistoryQueries = new List<PurchaseHistoryQueryInfo>();
        }

        #if !DOXYGEN_SHOULD_SKIP_THIS
        /// <summary>
        /// Called by MLAPISingleton to start the API
        /// </summary>
        protected override MLResult StartAPI()
        {
            return MLResult.Create(MLResult.Code.Ok);
        }
        #endif // DOXYGEN_SHOULD_SKIP_THIS

        /// <summary>
        /// Called by MLAPISingleton on destruction
        /// </summary>
        /// <param name="isSafeToAccessManagedObjects"></param>
        protected override void CleanupAPI(bool isSafeToAccessManagedObjects)
        {
            foreach (ItemDetailQueryInfo query in _pendingItemDetailQueries)
            {
                DestroyItemDetail(query.Handle);
            }
            _pendingItemDetailQueries.Clear();

            foreach (PurchaseRequestInfo request in _pendingPurchaseRequests)
            {
                DestroyPurchaseRequest(request.Handle);
            }
            _pendingPurchaseRequests.Clear();

            foreach (PurchaseHistoryQueryInfo query in _pendingPurchaseHistoryQueries)
            {
                DestroyPurchaseHistoryQuery(query.Handle);
            }
            _pendingPurchaseHistoryQueries.Clear();
        }

        /// <summary>
        /// Called every device frame
        /// </summary>
        protected override void Update()
        {
            UpdateItemDetails();
            UpdatePurchaseRequest();
            UpdatePurchaseHistoryQuery();
        }

        /// <summary>
        /// Gets a readable version of the result code as an ASCII string.
        /// </summary>
        /// <param name="resultCode">The MLResult that should be converted.</param>
        /// <returns>ASCII string containing a readable version of the result code.</returns>
        public static string GetResultString(MLResult.Code resultCode)
        {
            return Marshal.PtrToStringAnsi(MLPurchaseNativeBindings.MLPurchaseGetResultString(resultCode));
        }

        /// <summary>
        /// Request for the details for the given IAP ids
        /// </summary>
        /// <param name="itemIds">Array of IAP ids</param>
        /// <param name="callback">Method to be called when the query is completed</param>
        /// <returns>
        /// MLResult.Result will be MLResult.Code.Ok if successful.
        ///
        /// MLResult.Result will be MLResult.Code.AllocFailed if the handle cannot be allocated.
        ///
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if there is an unexpected failure.
        ///
        /// MLResult.Result will be MLResult.Code.InvalidParam if any input parameters are invalid.
        ///
        /// MLResult.Result will be MLResult.Code.PrivilegeDenied if there is an privilege error with the purchase details system call.
        ///
        /// MLResult.Result will be MLResult.Code.CloudSystemError if there is an issue with the cloud service, e.g. service is not available for any reason.
        /// </returns>
        public static MLResult GetItemDetails(string[] itemIds, Action<MLPurchaseItemDetail> callback)
        {
            if (!IsStarted)
            {
                MLResult result = MLResult.Create(MLResult.Code.UnspecifiedFailure, "Please call MLPurchase.Start() first.");
                MLPluginLog.ErrorFormat("MLPurchase.GetItemDetails failed to get item details. Reason: {0}", result);
                return result;
            }

            if (itemIds == null || itemIds.Length == 0 || callback == null)
            {
                MLResult result = MLResult.Create(MLResult.Code.InvalidParam, "Invalid GetItemDetails params.");
                MLPluginLog.Error(result);
                return result;
            }

            return Instance.GetItemDetailsInternal(itemIds, callback);
        }

        private MLResult GetItemDetailsInternal(string[] itemIds, Action<MLPurchaseItemDetail> callback)
        {
            // Create handle
            ulong handle = MagicLeapNativeBindings.InvalidHandle;
            MLResult.Code detailsResult = MLPurchaseNativeBindings.MLPurchaseItemDetailsCreate(ref handle);
            MLResult result = MLResult.Create(detailsResult);
            if (!result.IsOk || !MagicLeapNativeBindings.MLHandleIsValid(handle))
            {
                MLPluginLog.ErrorFormat("MLPurchase.GetItemDetailsInternal failed, unable to create item details query. Reason: {0}", result);
                return result;
            }

            result = InitiateItemDetailsQuery(handle, itemIds);

            if (!result.IsOk)
            {
                MLPluginLog.ErrorFormat("MLPurchase.GetItemDetailsInternal failed to submit item details request. Reason: {0}", result);
                DestroyItemDetail(handle);
            }
            else
            {
                _pendingItemDetailQueries.Add(new ItemDetailQueryInfo(handle, callback));
            }
            return result;
        }

        private MLResult InitiateItemDetailsQuery(ulong handle, string[] itemIds)
        {
            // Allocate memory for query
            int size = Marshal.SizeOf(typeof(IntPtr));
            IntPtr itemIdsPtr = Marshal.AllocHGlobal(size * itemIds.Length);
            for (int i = 0; i < itemIds.Length; i++)
            {
                IntPtr idPtr = Marshal.StringToHGlobalAuto(itemIds[i]);
                Marshal.WriteIntPtr(new IntPtr(itemIdsPtr.ToInt64() + (Marshal.SizeOf(typeof(IntPtr)) * i)), idPtr);
            }

            // Build the query
            MLPurchaseItemDetailsQuery query = new MLPurchaseItemDetailsQuery()
            {
                ids = itemIdsPtr,
                idCount = Convert.ToUInt32(itemIds.Length)
            };

            // Perform the query
            MLResult.Code detailsResult = MLPurchaseNativeBindings.MLPurchaseItemDetailsGet(handle, ref query);
            MLResult result = MLResult.Create(detailsResult);
            // Free up memory before checking results of query
            for (int i = 0; i < itemIds.Length; i++)
            {
                IntPtr idPtr = Marshal.ReadIntPtr(new IntPtr(itemIdsPtr.ToInt64() + (Marshal.SizeOf(typeof(IntPtr)) * i)));
                Marshal.FreeHGlobal(idPtr);
            }
            Marshal.FreeHGlobal(itemIdsPtr);

            return result;
        }

        /// <summary>
        /// Properly dispose the Item Detail Query handle
        /// </summary>
        /// <param name="handle">Item Detail Query handle to be destroyed</param>
        /// <returns>Retrun true on success, otherwise false</returns>
        private bool DestroyItemDetail(ulong handle)
        {
            if (MagicLeapNativeBindings.MLHandleIsValid(handle))
            {
                MLResult.Code result = MLPurchaseNativeBindings.MLPurchaseItemDetailsDestroy(handle);
                if (result == MLResult.Code.Ok)
                {
                    return true;
                }
                MLPluginLog.ErrorFormat("MLPurchase.DestroyItemDetail failed, unable to free item details handle. Reason: {0}", GetResultString(result));
                return false;
            }
            MLPluginLog.ErrorFormat("MLPurchase.DestroyItemDetail failed, handle is invalid.");
            return false;
        }

        private void UpdateItemDetails()
        {
            List<ItemDetailQueryInfo> completedQueries = new List<ItemDetailQueryInfo>();

            for (int i = 0; i < _pendingItemDetailQueries.Count; ++i)
            {
                ItemDetailQueryInfo info = _pendingItemDetailQueries[i];
                MLPurchaseItemDetailsResults results = MLPurchaseItemDetailsResults.Create();
                MLCloudStatus status = MLCloudStatus.NotDone;
                info.Details.Result = MLPurchaseNativeBindings.MLPurchaseItemDetailsGetResult(info.Handle, ref results, ref status);

                if (info.Details.Result == MLResult.Code.Ok)
                {
                    if (status == MLCloudStatus.Done)
                    {
                        info.Details.ItemResults = new MLPurchaseItemDetailsResult[results.count];
                        for (int j = 0; j < results.count; j++)
                        {
                            IntPtr offsetPtr = new IntPtr(results.itemDetails.ToInt64() + (Marshal.SizeOf(typeof(MLPurchaseItemDetailsResult)) * j));
                            info.Details.ItemResults[j] = (MLPurchaseItemDetailsResult)Marshal.PtrToStructure(offsetPtr, typeof(MLPurchaseItemDetailsResult));
                        }
                        completedQueries.Add(info);
                    }
                }
                else
                {
                    completedQueries.Add(info);
                }
            }

            PublishItemDetails(completedQueries);
        }

        private void PublishItemDetails(List<ItemDetailQueryInfo> completedQueries)
        {
            foreach (ItemDetailQueryInfo info in completedQueries)
            {
                _pendingItemDetailQueries.Remove(info);
                if (info.Callback != null)
                {
                    info.Callback(info.Details);
                }
                DestroyItemDetail(info.Handle);
            }
        }

        /// <summary>
        /// Initiates a purchase
        /// </summary>
        /// <param name="token">Token returned by MLPurchaseItemDetailsResult</param>
        /// <param name="callback">Method to be called when the purchase is completed</param>
        /// <returns>
        /// MLResult.Result will be MLResult.Code.Ok if successful.
        ///
        /// MLResult.Result will be MLResult.Code.AllocFailed if the handle cannot be allocated.
        ///
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if there is an unexpected failure.
        ///
        /// MLResult.Result will be MLResult.Code.InvalidParam if any input parameters are invalid.
        ///
        /// MLResult.Result will be MLResult.Code.PrivilegeDenied if there is an privilege error with the purchase details system call.
        ///
        /// MLResult.Result will be MLResult.Code.CloudSystemError if there is an issue with the cloud service, e.g. service is not available for any reason.
        /// </returns>
        public static MLResult MakePurchase(string token, Action<MLPurchaseRequest> callback)
        {
            if (!IsStarted)
            {
                MLResult result = MLResult.Create(MLResult.Code.UnspecifiedFailure, "Please call MLPurchase.Start() first");
                MLPluginLog.ErrorFormat("MLPurchase.MakePurchase failed to make a purchase. Reason: {0}", result);
                return result;
            }

            if (callback == null)
            {
                MLResult result = MLResult.Create(MLResult.Code.InvalidParam, "Invalid callback parameter passed.");
                MLPluginLog.ErrorFormat("MLPurchase.MakePurchase failed to make a purchase. Reason: {0}", result);
                return result;
            }

            return Instance.MakePurchaseInternal(token, callback);
        }

        private MLResult MakePurchaseInternal(string token, Action<MLPurchaseRequest> callback)
        {
            ulong handle = MagicLeapNativeBindings.InvalidHandle;
            MLResult.Code purchaseResult = MLPurchaseNativeBindings.MLPurchaseCreate(ref handle);
            MLResult result = MLResult.Create(purchaseResult);
            if (!result.IsOk || !MagicLeapNativeBindings.MLHandleIsValid(handle))
            {
                MLPluginLog.ErrorFormat("MLPurchase.MakePurchaseInternal failed to create purchase request. Reason: {0}", result);
                return result;
            }

            MLPluginLog.Debug("Purchasing: " + token);
            purchaseResult = MLPurchaseNativeBindings.MLPurchaseSubmit(handle, token);
            result = MLResult.Create(purchaseResult);
            if (!result.IsOk)
            {
                MLPluginLog.ErrorFormat("MLPurchase.MakePurchaseInternal failed to submit purchase request. Reason: {0}", result);
                DestroyPurchaseRequest(handle);
            }
            else
            {
                _pendingPurchaseRequests.Add(new PurchaseRequestInfo(handle, callback));
            }
            return result;
        }

        private bool DestroyPurchaseRequest(ulong handle)
        {
            if (MagicLeapNativeBindings.MLHandleIsValid(handle))
            {
                MLResult.Code result = MLPurchaseNativeBindings.MLPurchaseDestroy(handle);
                if (result == MLResult.Code.Ok)
                {
                    return true;
                }
                MLPluginLog.ErrorFormat("MLPurchase.DestroyPurchaseRequest failed to free purchase request handle. Reason: {0}", GetResultString(result));
                return false;
            }
            MLPluginLog.ErrorFormat("MLPurchase.DestroyPurchaseRequest failed, handle is invalid.");
            return false;
        }

        private void UpdatePurchaseRequest()
        {
            List<PurchaseRequestInfo> completedRequests = new List<PurchaseRequestInfo>();

            for (int i = 0; i < _pendingPurchaseRequests.Count; ++i)
            {
                PurchaseRequestInfo info = _pendingPurchaseRequests[i];
                MLCloudStatus status = MLCloudStatus.NotDone;
                info.Details.Result = MLPurchaseNativeBindings.MLPurchaseGetResult(info.Handle, ref info.Details.PurchaseConfirmation, ref status);

                if (info.Details.Result == MLResult.Code.Ok)
                {
                    if (status == MLCloudStatus.Done)
                    {
                        completedRequests.Add(info);
                    }
                }
                else
                {
                    completedRequests.Add(info);
                }
            }

            PublishPurchaseRequests(completedRequests);
        }

        private void PublishPurchaseRequests(List<PurchaseRequestInfo> completedRequests)
        {
            foreach (PurchaseRequestInfo info in completedRequests)
            {
                _pendingPurchaseRequests.Remove(info);
                if (info.Callback != null)
                {
                    info.Callback(info.Details);
                }
                DestroyPurchaseRequest(info.Handle);
            }
        }

        /// <summary>
        /// Begins a purchase history query
        /// </summary>
        /// <param name="numItems">Number of most recent items to get</param>
        /// <param name="callback">Method to be called when the query is completed</param>
        /// <returns>
        /// MLResult.Result will be MLResult.Code.Ok if successful.
        ///
        /// MLResult.Result will be MLResult.Code.AllocFailed if the handle cannot be allocated.
        ///
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if there is an unexpected failure.
        ///
        /// MLResult.Result will be MLResult.Code.InvalidParam if any input parameters are invalid.
        ///
        /// MLResult.Result will be MLResult.Code.PrivilegeDenied if there is an privilege error with the purchase details system call.
        ///
        /// MLResult.Result will be MLResult.Code.CloudSystemError if there is an issue with the cloud service, e.g. service is not available for any reason.
        /// </returns>
        public static MLResult GetRecentPurchaseHistory(uint numItems, Action<MLPurchaseHistoryQuery> callback)
        {
            if (!IsStarted)
            {
                MLResult result = MLResult.Create(MLResult.Code.UnspecifiedFailure, "Please call MLPurchase.Start() first");
                MLPluginLog.ErrorFormat("MLPurchase.GetRecentPurchaseHistory failed to get recent purchase history. Reason: {0}", result);
                return result;
            }

            if (numItems < 1 || callback == null)
            {
                MLResult result = MLResult.Create(MLResult.Code.InvalidParam, "Invalid MakePurchaseHistoryQuery params");
                MLPluginLog.ErrorFormat("MLPurchase.GetRecentPurchaseHistory failed to get recent purchase history. Reason: {0}", result);
                return result;
            }

            return Instance.GetRecentPurchaseHistoryInternal(numItems, callback);
        }

        private MLResult GetRecentPurchaseHistoryInternal(uint numItems, Action<MLPurchaseHistoryQuery> callback)
        {
            ulong handle = MagicLeapNativeBindings.InvalidHandle;
            MLResult.Code historyResult = MLPurchaseNativeBindings.MLPurchaseHistoryQueryCreate(ref handle);
            MLResult result = MLResult.Create(historyResult);
            if (!result.IsOk || !MagicLeapNativeBindings.MLHandleIsValid(handle))
            {
                MLPluginLog.ErrorFormat("MLPurchase.GetRecentPurchaseHistoryInternal failed to create purchase history query. Reason: {0}", result);
                return result;
            }

            historyResult = MLPurchaseNativeBindings.MLPurchaseHistoryQueryGetPage(handle, Math.Min(numItems, MaxPurchaseHistoryItems));
            result = MLResult.Create(historyResult);
            if (!result.IsOk)
            {
                MLPluginLog.ErrorFormat("MLPurchase.GetRecentPurchaseHistoryInternal failed to submit purchase history query. Reason: {0}", result);
                DestroyPurchaseHistoryQuery(handle);
            }
            else
            {
                _pendingPurchaseHistoryQueries.Add(new PurchaseHistoryQueryInfo(handle, callback, numItems));
            }
            return result;
        }

        private bool DestroyPurchaseHistoryQuery(ulong handle)
        {
            if (MagicLeapNativeBindings.MLHandleIsValid(handle))
            {
                MLResult.Code result = MLPurchaseNativeBindings.MLPurchaseHistoryQueryDestroy(handle);
                if (result == MLResult.Code.Ok)
                {
                    return true;
                }
                MLPluginLog.ErrorFormat("MLPurchase.DestroyPurchaseHistoryQuery failed to free purchase history query handle. Reason: {0}", GetResultString(result));
                return false;
            }
            MLPluginLog.Error("MLPurchase.DestroyPurchaseHistoryQuery failed, handle is invalid.");
            return false;
        }

        private void UpdatePurchaseHistoryQuery()
        {
            List<PurchaseHistoryQueryInfo> completedQueries = new List<PurchaseHistoryQueryInfo>();

            for (int i = 0; i < _pendingPurchaseHistoryQueries.Count; ++i)
            {
                PurchaseHistoryQueryInfo info = _pendingPurchaseHistoryQueries[i];
                MLPurchaseHistoryResult historyResult = MLPurchaseHistoryResult.Create();
                info.Details.Result = MLPurchaseNativeBindings.MLPurchaseHistoryQueryGetPageResult(info.Handle, ref historyResult);

                if (info.Details.Result == MLResult.Code.Ok)
                {
                    if (historyResult.status == MLCloudStatus.Done)
                    {
                        uint numPurchaseConfirmationsToAdd = Math.Min(historyResult.count, info.NumItemsLeft);
                        for (int j = 0; j < numPurchaseConfirmationsToAdd; j++)
                        {
                            IntPtr offsetPtr = new IntPtr(historyResult.confirmations.ToInt64() + (Marshal.SizeOf(typeof(MLPurchaseConfirmation)) * j));
                            info.Details.PurchaseConfirmations.Add ( (MLPurchaseConfirmation)Marshal.PtrToStructure(offsetPtr, typeof(MLPurchaseConfirmation)) );
                        }
                        info.NumItemsLeft -= numPurchaseConfirmationsToAdd;

                        MLPluginLog.DebugFormat("purchase history query: hasNextPage {0}, fetchAll {1}, NumItemsLeft {2}",
                            historyResult.hasNextPage ? "true" : "false",
                            info.FetchAll ? "true" : "false",
                            info.NumItemsLeft.ToString()); // TESTING

                        if (historyResult.hasNextPage && (info.FetchAll || info.NumItemsLeft > 0))
                        {
                            info.Details.Result = MLPurchaseNativeBindings.MLPurchaseHistoryQueryGetPage(info.Handle, Math.Min(info.NumItemsLeft, MaxPurchaseHistoryItems));
                            if (info.Details.Result != MLResult.Code.Ok)
                            {
                                MLPluginLog.ErrorFormat("MLPurchase.UpdatePurchaseHistoryQuery failed to query for succeeding purchase history pages. Reason: {0}", GetResultString(info.Details.Result));
                                completedQueries.Add(info);
                            }
                        }
                        else
                        {
                            completedQueries.Add(info);
                        }
                    }
                }
                else
                {
                    MLPluginLog.Debug("purchase history query get page result, result: " + info.Details.Result.ToString());
                    completedQueries.Add(info);
                }
            }

            PublishPurchaseHistories(completedQueries);
        }

        private void PublishPurchaseHistories(List<PurchaseHistoryQueryInfo> completedQueries)
        {
            foreach (PurchaseHistoryQueryInfo info in completedQueries)
            {
                _pendingPurchaseHistoryQueries.Remove(info);
                if (info.Callback != null)
                {
                    info.Callback(info.Details);
                }
                DestroyPurchaseHistoryQuery(info.Handle);
            }
        }

        /// <summary>
        /// Begin query for the whole purchase history
        /// </summary>
        /// <param name="callback">Method to be called when the query is completed</param>
        /// <returns>
        /// MLResult.Result will be MLResult.Code.Ok if successful.
        ///
        /// MLResult.Result will be MLResult.Code.AllocFailed if the handle cannot be allocated.
        ///
        /// MLResult.Result will be MLResult.Code.UnspecifiedFailure if there is an unexpected failure.
        ///
        /// MLResult.Result will be MLResult.Code.InvalidParam if any input parameters are invalid.
        ///
        /// MLResult.Result will be MLResult.Code.PrivilegeDenied if there is an privilege error with the purchase details system call.
        ///
        /// MLResult.Result will be MLResult.Code.CloudSystemError if there is an issue with the cloud service, e.g. service is not available for any reason.
        /// </returns>
        public static MLResult GetAllPurchaseHistory(Action<MLPurchaseHistoryQuery> callback)
        {
            if (!IsStarted)
            {
                MLResult result = MLResult.Create(MLResult.Code.UnspecifiedFailure, "Please call MLPurchase.Start() first");
                MLPluginLog.ErrorFormat("MLPurchase.GetAllPurchaseHistory failed to get entire purchase history. Reason: {0}", result);
                return result;
            }

            if (callback == null)
            {
                MLResult result = MLResult.Create(MLResult.Code.InvalidParam, "Invalid MakePurchaseHistoryQuery params");
                MLPluginLog.ErrorFormat("MLPurchase.GetAllPurchaseHistory failed to get entire purchase history. Reason: {0}", result);
                return result;
            }

            return Instance.GetAllPurchaseHistoryInternal(callback);
        }

        private MLResult GetAllPurchaseHistoryInternal(Action<MLPurchaseHistoryQuery> callback)
        {
            ulong handle = MagicLeapNativeBindings.InvalidHandle;
            MLResult.Code historyResult = MLPurchaseNativeBindings.MLPurchaseHistoryQueryCreate(ref handle);
            MLResult result = MLResult.Create(historyResult);
            if (!result.IsOk || !MagicLeapNativeBindings.MLHandleIsValid(handle))
            {
                MLPluginLog.ErrorFormat("MLPurchase.GetAllPurchaseHistoryInternal failed to create purchase history query. Reason: {0}", result);
                return result;
            }

            historyResult = MLPurchaseNativeBindings.MLPurchaseHistoryQueryGetPage(handle, MaxPurchaseHistoryItems);
            result = MLResult.Create(historyResult);
            if (!result.IsOk)
            {
                MLPluginLog.ErrorFormat("MLPurchase.GetAllPurchaseHistoryInternal failed to submit purchase history query. Reason: {0}", result);
                DestroyPurchaseHistoryQuery(handle);
            }
            else
            {
                _pendingPurchaseHistoryQueries.Add(new PurchaseHistoryQueryInfo(handle, callback));
            }
            return result;
        }
    }
}

#endif
