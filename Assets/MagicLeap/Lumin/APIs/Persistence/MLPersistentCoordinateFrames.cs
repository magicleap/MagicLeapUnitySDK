// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLPersistentCoordinateFrames.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018-present, Magic Leap, Inc. All Rights Reserved.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if PLATFORM_LUMIN

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;
    using Unity.Jobs;

    /// <summary>
    /// MLPersistentCoordinateFrames class contains the API to interface with the
    /// Persistent Coordinate Frames C API.
    /// </summary>
    public sealed partial class MLPersistentCoordinateFrames : MLAPISingleton<MLPersistentCoordinateFrames>
    {
        /// <summary>
        /// Handle to the PCF tracker.
        /// </summary>
        private ulong nativeTracker = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// The map of CFUIds to PCFs found in the current map.
        /// </summary>
        private Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, PCF> mapAllPCFs = new Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, PCF>();

        /// <summary>
        /// Map of CFUIDs to PCFs that we're currently tracking. A PCF is only tracked when it's added via QueueForUpdates().
        /// Tracked PCFs are updated every frame.
        /// </summary>
        private Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, PCF> mapTrackedPCFs = new Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, PCF>();

        /// <summary>
        /// Map of CFUIDs to PCFs that have been updated this frame, used to prevent a PCF from being updated multiple times in one frame.
        /// </summary>
        private Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, PCF> mapUpdatedPCFsThisFrame = new Dictionary<MagicLeapNativeBindings.MLCoordinateFrameUID, PCF>();

        /// <summary>
        /// Delegate used for when a user wants to request multiple PCFs from the MLPersistentCoordinateFrames API.
        /// </summary>
        /// <param name="resultCode">The result code of the query call for multiple PCFs.</param>
        /// <param name="pcfList">The list of PCFs returned by the query.</param>
        public delegate void OnFoundMultiPCFDelegate(MLResult.Code resultCode, List<PCF> pcfList);

        /// <summary>
        /// Delegate used for when a user wants to request a single PCF from the MLPersistentCoordinateFrames API.
        /// </summary>
        /// <param name="resultCode">The result code of the query call for a single PCF.</param>
        /// <param name="pcf">The PCF returned by the query.</param>
        public delegate void OnFoundSinglePCFDelegate(MLResult.Code resultCode, PCF pcf);

        /// <summary>
        /// Delegate for the OnLocalizedDelegate event used for notifying when the user has localized to a map or has lost localization.
        /// </summary>
        /// <param name="localized">True if localized to a map and False if localization was lost.</param>
        public delegate void OnLocalizedDelegate(bool localized);

        /// <summary>
        /// Event for notifying when the user has localized to a map or has lost localization.
        /// </summary>
        public static event OnLocalizedDelegate OnLocalized;

        /// <summary>
        /// Gets a value indicating whether the user is currently localized to a map.
        /// </summary>
        public static bool IsLocalized
        {
            get;
            private set;
        }

        /// <summary>
        /// Starts the PersistentCoordinateFrame API.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if starting the API is successful.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if the tracker to initialize was null.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
        /// </returns>
        public static MLResult Start()
        {
            CreateInstance();
            return MLPersistentCoordinateFrames.BaseStart();
        }

        /// <summary>
        /// Sets the offset position/rotation values from the current Perception root and can also set the PCF to be used as the new Perception root to which all MLSnapshotGetTransform will be relative to.
        /// </summary>
        /// <param name="offsetPosition">The offset to set from the root's position.</param>
        /// <param name="offsetRotation">The offset to set from the root's rotation.</param>
        /// <param name="pcf">The PCF to set the root to.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if operation completed successfully.
        /// MLResult.Result will be <c>MLResult.Code.MLPerceptionNotStarted</c> if unable to retrieve the Perception System.
        /// MLResult.Result will be <c>MLResult.Code.MLPerceptionInvalidPCFRoot</c> if the provided CFUID is not associated with a valid PCF.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if the CFUID is not valid.
        /// </returns>
        public static MLResult SetPerceptionRoot(Vector3 offsetPosition, Quaternion offsetRotation, PCF pcf = null)
        {
            if (MLPersistentCoordinateFrames.IsValidInstance())
            {
                MLResult.Code resultCode = NativeBindings.SetPerceptionRoot(offsetPosition, offsetRotation, pcf);

                if (!MLResult.IsOK(resultCode))
                {
                    MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.SetPerceptionRoot failed. Reason: {0}", MLResult.CodeToString(resultCode));
                    return MLResult.Create(MLResult.Code.UnspecifiedFailure, string.Format("MLPersistentCoordinateFrames.SetPerceptionRoot failed. Reason: {0}", MLResult.CodeToString(resultCode)));
                }

                return MLResult.Create(resultCode);
            }
            else
            {
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.SetPerceptionRoot failed. Reason: No Instance for MLPersistentCoordinateFrames.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.SetPerceptionRoot failed. Reason: No Instance for MLPersistentCoordinateFrames.");
            }
        }

        /// <summary>
        /// Gets the offset position/rotation values and the PCF of the current Perception root.
        /// </summary>
        /// <param name="offsetPosition">Stores the value of the current offset from the root's position.</param>
        /// <param name="offsetRotation">Stores the value of the current offset from the root's rotation.</param>
        /// <param name="pcf">Stores the reference to the current root PCF.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if operation completed successfully.
        /// MLResult.Result will be <c>MLResult.Code.MLPerceptionNotStarted</c> if unable to retrieve the Perception System.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
        /// </returns>
        public static MLResult GetPerceptionRoot(out Vector3 offsetPosition, out Quaternion offsetRotation, out PCF pcf)
        {
            offsetPosition = Vector3.zero;
            offsetRotation = Quaternion.identity;
            pcf = new PCF(new MagicLeapNativeBindings.MLCoordinateFrameUID());

            if (MLPersistentCoordinateFrames.IsValidInstance())
            {
                try
                {
                    MLResult.Code resultCode = NativeBindings.MLPerceptionGetRootCoordinateFrame(out MagicLeapNativeBindings.MLCoordinateFrameUID cfuid, out MagicLeapNativeBindings.MLTransform mlTransform);
                    if (MLResult.IsOK(resultCode))
                    {
                        offsetPosition = MLConvert.ToUnity(mlTransform.Position);
                        offsetRotation = MLConvert.ToUnity(mlTransform.Rotation);
                        return MLResult.Create(MLResult.Code.Ok);
                    }
                    else
                    {
                        MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.GetPerceptionRoot failed. Reason: {0}", MLResult.CodeToString(resultCode));
                        return MLResult.Create(resultCode);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    MLPluginLog.Error("MLPersistentCoordinateFrames.GetPerceptionRoot failed. Reason: API symbols not found.");
                    return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.GetPerceptionRoot failed. Reason: API symbols not found.");
                }
            }
            else
            {
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.GetPerceptionRoot failed. Reason: No Instance for MLPersistentCoordinateFrames.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.GetPerceptionRoot failed. Reason: No Instance for MLPersistentCoordinateFrames.");
            }
        }

        /// <summary>
        /// Adds the given PCF to the map of PCFs that are updated every frame as well as the map of all PCFs.
        /// </summary>
        /// <param name="pcf">PCF to be updated.</param>
        public static void QueueForUpdates(PCF pcf)
        {
            if (MLPersistentCoordinateFrames.IsValidInstance())
            {
                if (!_instance.mapAllPCFs.ContainsKey(pcf.CFUID))
                {
                    _instance.mapAllPCFs.Add(pcf.CFUID, pcf);
                }

                if (!_instance.mapTrackedPCFs.ContainsKey(pcf.CFUID))
                {
                    _instance.mapTrackedPCFs.Add(pcf.CFUID, pcf);
                }
            }
            else
            {
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.QueueForUpdates failed. Reason: No Instance for MLPersistentCoordinateFrames.");
            }
        }

        /// <summary>
        /// Retrieves the type of the PCF associated with the given CFUID.
        /// </summary>
        /// <param name="cfuid">The CFUID to look up the PCF type with.</param>
        /// <param name="type">Stores the type of PCF associated with the given CFUID.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid PCF was found.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid input parameter.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if necessary privilege is missing.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to other internal error.
        /// </returns>
        public static MLResult GetPCFTypeByCFUID(MagicLeapNativeBindings.MLCoordinateFrameUID cfuid, out PCF.Types type)
        {
            type = 0;

            if (MLPersistentCoordinateFrames.IsValidInstance())
            {
                try
                {
                    NativeBindings.FrameStateNative nativeState = NativeBindings.FrameStateNative.Create();
                    MLResult.Code resultCode = NativeBindings.MLPersistentCoordinateFramesGetFrameState(_instance.nativeTracker, in cfuid, ref nativeState);
                    if (!MLResult.IsOK(resultCode))
                    {
                        if (resultCode == MLResult.Code.PassableWorldLowMapQuality || resultCode == MLResult.Code.PassableWorldUnableToLocalize)
                        {
                            MLPluginLog.WarningFormat("Map quality not sufficient enough for MLPersistentCoordinateFrames.GetPCFTypeByCFUID. Reason: {0}", MLResult.CodeToString(resultCode));
                        }
                        else
                        {
                            MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.GetPCFTypeByCFUID failed. Reason: {0}", MLResult.CodeToString(resultCode));
                        }

                        return MLResult.Create(resultCode, string.Format("MLPersistentCoordinateFrames.GetPCFTypeByCFUID failed. Reason: {0}", MLResult.CodeToString(resultCode)));
                    }
                    else
                    {
                        type = nativeState.Type;
                        return MLResult.Create(resultCode);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    MLPluginLog.Error("MLPersistentCoordinateFrames.GetPCFTypeByCFUID failed. Reason: API symbols not found.");
                    return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.GetPCFTypeByCFUID failed. Reason: API symbols not found.");
                }
            }
            else
            {
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.GetPCFTypeByCFUID failed. Reason: No Instance for MLPersistentCoordinateFrames.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.GetPCFTypeByCFUID failed. Reason: No Instance for MLPersistentCoordinateFrames.");
            }
        }

        /// <summary>
        /// Retrieves the PCF associated with the given CFUID.
        /// </summary>
        /// <param name="cfuid">The CFUID to look up the PCF type with.</param>
        /// <param name="callback">Delegate used to return the resulting MLResult.Result and the closest found PCF.</param>
        /// <param name="update">Determines if the PCF should have it's pose and state updated.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid PCF was found.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to no API instance being found.
        /// </returns>
        public static MLResult FindPCFByCFUID(MagicLeapNativeBindings.MLCoordinateFrameUID cfuid, OnFoundSinglePCFDelegate callback, bool update = true)
        {
            if (MLPersistentCoordinateFrames.IsValidInstance())
            {
                if (_instance.mapAllPCFs.ContainsKey(cfuid))
                {
                    PCF pcf = _instance.mapAllPCFs[cfuid];
                    if (update)
                    {
                        MLResult updateResult = pcf.Update();
                        if (!updateResult.IsOk)
                        {
                            MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindPCFByCFUID failed to update the found PCF. Reason: {0}", updateResult);
                            callback(MLResult.Code.PassableWorldNotFound, pcf);
                            return updateResult;
                        }
                    }

                    callback(MLResult.Code.Ok, pcf);
                    return MLResult.Create(MLResult.Code.Ok);
                }
                else
                {
                    QueryFilter queryFilter = QueryFilter.Create();
                    queryFilter.TypesMask = PCF.Types.SingleUserSingleSession | PCF.Types.SingleUserMultiSession | PCF.Types.MultiUserMultiSession;
                    queryFilter.MaxResults = int.MaxValue;
                    queryFilter.Sorted = false;

                    FindPCFsByFilter(queryFilter, update: update, callback: (MLResult.Code resultCode, List<PCF> pcfList) =>
                    {
                        PCF pcfWithCFUID = pcfList.Find(pcf => pcf.CFUID == cfuid);
                        if(pcfWithCFUID != null)
                        {
                            if (update)
                            {
                                MLResult updateResult = pcfWithCFUID.Update();
                                if (!updateResult.IsOk)
                                {
                                    MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindPCFByCFUID failed to update the found PCF. Reason: {0}", updateResult);
                                    callback(MLResult.Code.PassableWorldNotFound, pcfWithCFUID);
                                }
                            }
                            callback(MLResult.Code.Ok, pcfWithCFUID);
                        }
                        else
                        {
                            callback(resultCode, null);
                        }
                    });

                    return MLResult.Create(MLResult.Code.Ok);
                }
            }
            else
            {
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindPCFByCFUID failed. Reason: No Instance for MLPersistentCoordinateFrames.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.FindPCFByCFUID failed. Reason: No Instance for MLPersistentCoordinateFrames.");
            }
        }

        /// <summary>
        /// Retrieves the closest known PCF of the types provided by the typesMask to the given position.
        /// </summary>
        /// <param name="position">The position of the object we want to anchor.</param>
        /// <param name="callback">Delegate used to return the resulting MLResult.Result and the closest found PCF.</param>
        /// <param name="typesMask">The bitmask of which PCF types to consider.</param>
        /// <param name="update">Determines if the PCF should have it's pose and state updated.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a valid PCF was found.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to no API instance being found.
        /// </returns>
        public static MLResult FindClosestPCF(Vector3 position, OnFoundSinglePCFDelegate callback, PCF.Types typesMask = PCF.Types.SingleUserSingleSession | PCF.Types.SingleUserMultiSession | PCF.Types.MultiUserMultiSession, bool update = true)
        {
            if (MLPersistentCoordinateFrames.IsValidInstance())
            {
                QueryFilter queryFilter = QueryFilter.Create();
                queryFilter.TargetPoint = position;
                queryFilter.TypesMask = typesMask;
                queryFilter.Sorted = true;

                FindPCFsByFilter(queryFilter, update: update, callback: (MLResult.Code resultCode, List<PCF> pcfList) =>
                {
                    callback?.Invoke(resultCode, pcfList[0]);
                });

                return MLResult.Create(MLResult.Code.Ok);
            }
            else
            {
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindClosestPCF failed. Reason: No Instance for MLPersistentCoordinateFrames.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.FindClosestPCF failed. Reason: No Instance for MLPersistentCoordinateFrames.");
            }
        }

        /// <summary>
        /// Returns a list of all the PCFs of the types provided by the typesMask inside the current map.
        /// </summary>
        /// <param name="callback">Delegate used to return the resulting MLResult.Result and list of found PCFs.</param>
        /// <param name="maxResults">The max number of PCFs to get.</param>
        /// <param name="typesMask">The bitmask of which PCF types to consider.</param>
        /// <param name="update">Determines if the PCFs should have their pose updated.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if all the PCFs from the current map have been found successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to no API instance being found.
        /// </returns>
        public static MLResult FindAllPCFs(OnFoundMultiPCFDelegate callback, uint maxResults = int.MaxValue, PCF.Types typesMask = PCF.Types.SingleUserSingleSession | PCF.Types.SingleUserMultiSession | PCF.Types.MultiUserMultiSession, bool update = true)
        {

            if (MLPersistentCoordinateFrames.IsValidInstance())
            {
                QueryFilter queryFilter = QueryFilter.Create();
                queryFilter.TypesMask = typesMask;
                queryFilter.MaxResults = maxResults;
                queryFilter.Sorted = false;

                FindPCFsByFilter(queryFilter, callback, update);
                return MLResult.Create(MLResult.Code.Ok);
            }
            else
            {
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindAllPCFs failed. Reason: No Instance for MLPersistentCoordinateFrames.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.FindAllPCFs failed. Reason: No Instance for MLPersistentCoordinateFrames.");
            }
        }

        /// <summary>
        /// Returns filtered list of PCFs based on the parameters of the given queryFilter.
        /// </summary>
        /// <param name="queryFilter">Parameters used to curate the returned values.</param>
        /// <param name="callback">Delegate used to return the resulting MLResult.Result and list of found PCFs.</param>
        /// <param name="update">Determines if the PCFs should have their pose updated.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if all the PCFs from the current map have been found successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to no API instance being found.
        /// </returns>
        public static MLResult FindPCFsByFilter(QueryFilter queryFilter, OnFoundMultiPCFDelegate callback, bool update = true)
        {
            List<PCF> pcfList = new List<PCF>();

            if (MLPersistentCoordinateFrames.IsValidInstance())
            {
                NativeBindings.QueryFilterNative queryFilterNative = NativeBindings.QueryFilterNative.Create();
                queryFilterNative.Data = queryFilter;

                MLThreadDispatch.ScheduleWork(() =>
                {
                    if (MLPersistentCoordinateFrames.IsValidInstance())
                    {
                        try
                        {
                            uint numPCFs = 0;
                            MLResult.Code resultCode = NativeBindings.MLPersistentCoordinateFrameGetCount(MLPersistentCoordinateFrames._instance.nativeTracker, ref numPCFs);
                            MagicLeapNativeBindings.MLCoordinateFrameUID[] cfuidArray = new MagicLeapNativeBindings.MLCoordinateFrameUID[numPCFs];

                            if (MLResult.IsOK(resultCode) && numPCFs > 0)
                            {
                                //// With these conditions the user is asking for all PCFs, no need to use the slower thised query call.
                                if (queryFilter.TypesMask == (PCF.Types.SingleUserSingleSession | PCF.Types.SingleUserMultiSession | PCF.Types.MultiUserMultiSession) &&
                                    queryFilter.Radius <= 0 && !queryFilter.Sorted)
                                {
                                    resultCode = NativeBindings.MLPersistentCoordinateFrameGetAllEx(_instance.nativeTracker, numPCFs, cfuidArray);
                                }
                                else
                                {
                                    resultCode = NativeBindings.MLPersistentCoordinateFrameQuery(_instance.nativeTracker, in queryFilterNative, cfuidArray, out uint cfuidCount);
                                }

                                if (!MLResult.IsOK(resultCode))
                                {
                                    if (resultCode == MLResult.Code.PassableWorldLowMapQuality || resultCode == MLResult.Code.PassableWorldUnableToLocalize)
                                    {
                                        MLPluginLog.WarningFormat("Map quality not sufficient enough for MLPersistentCoordinateFrames.FindPCFsByFilter. Reason: {0}", MLResult.CodeToString(resultCode));
                                    }
                                    else
                                    {
                                        MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindPCFsByFilter failed. Reason: {0}", MLResult.IsOK(resultCode) ? "No PCFs could be found." : MLResult.CodeToString(resultCode));
                                    }

                                    return false;
                                }
                            }
                            else
                            {
                                if (resultCode == MLResult.Code.PassableWorldLowMapQuality || resultCode == MLResult.Code.PassableWorldUnableToLocalize)
                                {
                                    MLPluginLog.WarningFormat("Map quality not sufficient enough for MLPersistentCoordinateFrames.FindPCFsByFilter. Reason: {0}", MLResult.CodeToString(resultCode));
                                }
                                else
                                {
                                    MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindPCFsByFilter failed. Reason: {0}", MLResult.IsOK(resultCode) ? "No PCFs could be found." : MLResult.CodeToString(resultCode));
                                }

                                return false;
                            }

                            MLThreadDispatch.ScheduleMain(() =>
                            {
                                if (MLPersistentCoordinateFrames.IsValidInstance())
                                {
                                    if (MLResult.IsOK(resultCode) && MLPersistentCoordinateFrames.IsLocalized)
                                    {
                                        for (int i = 0; i < cfuidArray.Length; ++i)
                                        {
                                            MagicLeapNativeBindings.MLCoordinateFrameUID pcfCFUID = cfuidArray[i];
                                            if (!pcfCFUID.Equals(MagicLeapNativeBindings.MLCoordinateFrameUID.EmptyFrame))
                                            {
                                                PCF pcf = null;
                                                if (_instance.mapAllPCFs.ContainsKey(pcfCFUID))
                                                {
                                                    pcf = _instance.mapAllPCFs[pcfCFUID];
                                                    if (update)
                                                    {
                                                        MLResult result = pcf.Update();
                                                        if (!result.IsOk)
                                                        {
                                                            MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindPCFsByFilter failed to update the found PCF with CFUID {0}, Reason: {1}", pcf.CFUID, result);
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    pcf = new PCF(pcfCFUID);
                                                    _instance.mapAllPCFs.Add(pcfCFUID, pcf);
                                                    pcf.Update();
                                                }

                                                pcfList.Add(pcf);
                                            }
                                        }
                                    }

                                    callback?.Invoke(resultCode, pcfList);
                                }
                                else
                                {
                                    MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindPCFsByFilter failed. Reason: No Instance for MLPersistentCoordinateFrames.");
                                }
                            });

                            return true;
                        }
                        catch (EntryPointNotFoundException)
                        {
                            MLPluginLog.Error("MLPersistentCoordinateFrames.FindPCFsByFilter failed. Reason: API symbols not found.");
                            callback?.Invoke(MLResult.Code.UnspecifiedFailure, pcfList);
                            return true;
                        }
                    }
                    else
                    {
                        MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindPCFsByFilter failed. Reason: No Instance for MLPersistentCoordinateFrames.");
                        callback?.Invoke(MLResult.Code.UnspecifiedFailure, pcfList);
                        return true;
                    }

                });
                return MLResult.Create(MLResult.Code.Ok);
            }
            else
            {
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.FindPCFsByFilter failed. Reason: No Instance for MLPersistentCoordinateFrames.");
                callback?.Invoke(MLResult.Code.UnspecifiedFailure, pcfList);
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.FindPCFsByFilter failed. Reason: No Instance for MLPersistentCoordinateFrames.");
            }
        }

        /// <summary>
        /// Gets the result string for a MLResult.Code.
        /// </summary>
        /// <param name="resultCode">The MLResult.Code to be requested.</param>
        /// <returns>The result string.</returns>
        internal static IntPtr GetResultString(MLResult.Code resultCode)
        {
            try
            {
                return NativeBindings.MLPersistentCoordinateFrameGetResultString(resultCode);
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error("MLPersistentCoordinateFrames.GetResultString failed. Reason: MLPersistentCoordinateFrames API is currently available only on device.");
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLPersistentCoordinateFrames.GetResultString failed. Reason: API symbols not found");
            }

            return IntPtr.Zero;
        }

        /// <summary>
        /// Gets the result string for a MLResult.Code. Use MLResult.CodeToString(MLResult.Code) to get the string value of any MLResult.Code.
        /// </summary>
        /// <param name="result">The MLResult.Code to be requested.</param>
        /// <returns>A pointer to the result string.</returns>
        internal static string GetPerceptionResultString(MLResult.Code result)
        {
            try
            {
                return Marshal.PtrToStringAnsi(NativeBindings.MLPerceptionGetResultString(result));
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error("MLPersistentCoordinateFrames.GetPerceptionResultString failed. Reason: MLPersistentCoordinateFrames API is currently available only on device.");
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLPersistentCoordinateFrames.GetPerceptionResultString failed. Reason: API symbols not found");
            }

            return string.Empty;
        }

        /// <summary>
        /// Starts the Persistent Coordinate Frames API.
        /// Initializes the PCF tracker.
        /// </summary>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the PCF tracker could be created successfully.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if the tracker to initialize was null.
        /// MLResult.Result will be <c>MLResult.Code.PrivilegeDenied</c> if there were any missing privileges.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to an internal error.
        /// </returns>
        protected override MLResult StartAPI()
        {
            try
            {
                MLResult.Code resultCode = NativeBindings.MLPersistentCoordinateFrameTrackerCreate(ref _instance.nativeTracker);
                if (!MLResult.IsOK(resultCode))
                {
                    MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.StartAPI failed to create PCF tracker. Reason: {0}", MLResult.CodeToString(resultCode));
                    return MLResult.Create(resultCode, "MLPersistentCoordinateFrames.StartAPI failed to create PCF tracker.");
                }

                return MLResult.Create(resultCode);
            }
            catch (EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLPersistentCoordinateFrames.StartAPI failed. Reason: API symbols not found.");
                return MLResult.Create(MLResult.Code.UnspecifiedFailure, "MLPersistentCoordinateFrames.StartAPI failed. Reason: API symbols not found.");
            }
        }

        /// <summary>
        /// Cleans up unmanaged memory.
        /// </summary>
        /// <param name="isSafeToAccessManagedObjects">Determines if the pendingPCFPoseQueries list should be cleared.</param>
        protected override void CleanupAPI(bool isSafeToAccessManagedObjects)
        {
            if (!MagicLeapNativeBindings.MLHandleIsValid(_instance.nativeTracker))
            {
                return;
            }

            try
            {
                MLResult.Code resultCode = NativeBindings.MLPersistentCoordinateFrameTrackerDestroy(_instance.nativeTracker);
                if (!MLResult.IsOK(resultCode))
                {
                    MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.CleanupAPI failed to destroy PersistentCoordinateFrame tracker. Reason: {0}", MLResult.CodeToString(resultCode));
                }

                _instance.nativeTracker = MagicLeapNativeBindings.InvalidHandle;
            }
            catch (EntryPointNotFoundException)
            {
                MLPluginLog.Error("MLPersistentCoordinateFrames.CleanupAPI failed. Reason: API symbols not found.");
            }
        }

        /// <summary>
        /// Update loop used to schedule a SetLocalizationStatus that determines when the device has localized to some map.
        /// </summary>
        protected override void Update()
        {
            MLPersistentCoordinateFrames._instance.SetLocalizationStatus();

            if (MLPersistentCoordinateFrames.IsLocalized)
            {
                if (this.mapTrackedPCFs.Count > 0)
                {
                    foreach (PCF pcf in this.mapTrackedPCFs.Values)
                    {
                        pcf.Update();
                    }
                }
                MLPersistentCoordinateFrames._instance.mapUpdatedPCFsThisFrame.Clear();
            }
        }

        /// <summary>
        /// Used to initialize an instance of the <see cref="MLPersistentCoordinateFrames" /> class.
        /// </summary>
        private static void CreateInstance()
        {
            if (!MLPersistentCoordinateFrames.IsValidInstance())
            {
                MLPersistentCoordinateFrames._instance = new MLPersistentCoordinateFrames();
            }
        }

        /// <summary>
        /// Sets the localization status based on if PCFs can currently be found
        /// </summary>
        private void SetLocalizationStatus()
        {
            if (MLPersistentCoordinateFrames.IsValidInstance())
            {
                try
                {
                    MLResult result = MLResult.Create(MLResult.Code.Ok);
                    uint numPCFs = 0;
                    MLResult.Code resultCode = NativeBindings.MLPersistentCoordinateFrameGetCount(MLPersistentCoordinateFrames._instance.nativeTracker, ref numPCFs);

                    if (!MLResult.IsOK(resultCode))
                    {
                        if (resultCode == MLResult.Code.PassableWorldLowMapQuality || resultCode == MLResult.Code.PassableWorldUnableToLocalize)
                        {
                            MLPluginLog.WarningFormat("Map quality not sufficient enough for MLPersistentCoordinateFrames.SetLocalizationStatus. Reason: {0}", MLResult.CodeToString(resultCode));
                        }
                        else
                        {
                            MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.SetLocalizationStatus failed. Reason: {0}", MLResult.IsOK(resultCode) ? "No PCFs could be found." : MLResult.CodeToString(resultCode));
                        }

                        result = MLResult.Create(resultCode, string.Format("MLPersistentCoordinateFrames.SetLocalizationStatus failed. Reason: {0}", MLResult.IsOK(resultCode) ? "No PCFs could be found." : MLResult.CodeToString(resultCode)));
                    }

                    bool foundPCFs = result.IsOk && numPCFs > 0;
                    if (!MLPersistentCoordinateFrames.IsLocalized && foundPCFs)
                    {
                        MLPersistentCoordinateFrames.IsLocalized = true;
                        MLPersistentCoordinateFrames.OnLocalized?.Invoke(IsLocalized);
                    }
                    else if (MLPersistentCoordinateFrames.IsLocalized && !foundPCFs)
                    {
                        List<PCF> allPCFs = new List<PCF>(MLPersistentCoordinateFrames._instance.mapAllPCFs.Values);

                        MLPersistentCoordinateFrames._instance.mapTrackedPCFs.Clear();
                        MLPersistentCoordinateFrames._instance.mapAllPCFs.Clear();

                        MLPersistentCoordinateFrames.IsLocalized = false;

                        foreach (PCF pcf in allPCFs)
                        {
                            pcf.Update();
                        }

                        MLPersistentCoordinateFrames._instance.mapUpdatedPCFsThisFrame.Clear();
                        MLPersistentCoordinateFrames.OnLocalized?.Invoke(MLPersistentCoordinateFrames.IsLocalized);
                    }
                }
                catch (EntryPointNotFoundException)
                {
                    MLPluginLog.Error("MLPersistentCoordinateFrames.SetLocalizationStatus failed. Reason: API symbols not found.");
                }
            }
            else
            {
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.SetLocalizationStatus failed. Reason: No Instance for MLPersistentCoordinateFrames.");
            }

        }

        /// <summary>
        /// This represents a collection of filters and modifiers used by
        /// MLPersistentCoordinateFrameQuery to curate the returned values.
        /// </summary>
        public struct QueryFilter
        {
            /// <summary>
            /// [X,Y,Z] center query point from where the nearest neighbors will be calculated.
            /// </summary>
            public Vector3 TargetPoint;

            /// <summary>
            /// Expected types of the results.
            /// This is a bitmask field to specify all expected types.
            /// For example, use:
            /// TypesMask = PCF.Types.SingleUserMultiSession | PCF.Types.MultiUserMultiSession;
            /// to get PCFs of PCF.Types.SingleUserMultiSession and PCF.Types.MultiUserMultiSession types
            /// </summary>
            public PCF.Types TypesMask;

            /// <summary>
            /// Upper bound number of expected results.
            /// The implementation may return less entries than requested when total number of
            /// available elements is less than max_results or if internal memory limits are reached.
            /// </summary>
            public uint MaxResults;

            /// <summary>
            /// Return only entries within radius of the sphere from target_point.
            /// Radius is provided in meters. Set to zero for unbounded results.
            /// Filtering by distance will incur a performance penalty.
            /// </summary>
            public float Radius;

            /// <summary>
            /// Indicate if the result set should be sorted by distance from target_point.
            /// Sorting the PCFs by distance will incur a performance penalty.
            /// </summary>
            public bool Sorted;

            /// <summary>
            /// Initializes default values for PersistentCoordinateFramesQueryFilter.
            /// </summary>
            /// <returns> An initialized version of this struct.</returns>
            public static QueryFilter Create()
            {
                return new QueryFilter()
                {
                    TargetPoint = new Vector3(),
                    TypesMask = PCF.Types.MultiUserMultiSession,
                    MaxResults = 1,
                    Radius = 0.0f,
                    Sorted = true
                };
            }
        }
    }
}
#endif
