
// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLPersistentCoordinateFrames.cs" company="Magic Leap, Inc">
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
    using System.Linq;
    using System.Collections.Generic;

    /// <summary>
    /// MLPersistentCoordinateFrames class contains the API to interface with the
    /// Persistent Coordinate Frame C API.
    /// </summary>
    public sealed partial class MLPersistentCoordinateFrames
    {
        [Obsolete("There is no longer a forced delay for querying all pcfs multiple times.")]
        public static float DelayBetweenAllPCFsQueryInSeconds = 1.0f;

        [Obsolete("There is no longer a time out state.")]
        public static float StartTimeOutInSeconds = 3.0f;

        [Obsolete("Please use MLPersistentCoordinateFrames.IsLocalized instead.")]
        public static bool IsReady { get; private set; }

        [Obsolete("Please use MLPersistentCoordinateFrames.OnLocalized event instead.")]
        public static event System.Action<MLResult> OnInitialized = delegate { };

        [Obsolete("Please directly call Update() using a reference to an MLPersistentCoordinateFrames.PCF object instead.", true)]
        public static MLResult GetPCFPosition(MLPCF pcf, Action<MLResult, MLPCF> callback)
        {
            return MLResult.Create(MLResult.Code.UnspecifiedFailure, "Please use MLPersistentCoordinateFrames.Update instead.");
        }

        [Obsolete("Please use the most recent QueueForUpdates that uses the MLPersistentCoordinateFrames.PCF class instead.")]
        public static void QueueForUpdates(MLPCF pcf)
        {
            QueueForUpdates((PCF)pcf);
        }

        [Obsolete("Please use the most recent FindClosestPCF that uses the MLPersistentCoordinateFrames.PCF class instead.")]
        public static MLResult FindClosestPCF(Vector3 position, Action<MLResult, MLPCF> callback)
        {
            return MLResult.Create(MLResult.Code.UnspecifiedFailure, "No longer used.");

        }

        [Obsolete("Please directly call Update() using a reference to an MLPersistentCoordinateFrames.PCF object instead.")]
        public static MLResult GetPCFPose(MLPCF pcf, Action<MLResult, MLPCF> callback)
        {
            if (pcf == null || callback == null)
            {
                MLResult result = MLResult.Create(MLResult.Code.InvalidParam, string.Format("invalid parameters. pcf: {0}, callback: {1}", pcf, callback));
                MLPluginLog.ErrorFormat("MLPersistentCoordinateFrames.GetPCFPose failed. Reason: {0}", result);
                return result;
            }

            else
            {
                MLResult result = pcf.Update();
                callback(result, pcf);
                return result;
            }

        }

        [Obsolete("Please use MLPersistentCoordinateFrames.FindAllPCFs instead.")]
        public static MLResult GetAllPCFs(out List<MLPCF> pcfList, int maxResults = int.MaxValue)
        {
            pcfList = new List<MLPCF>();
            return MLResult.Create(MLResult.Code.UnspecifiedFailure, "No longer used.");
        }

        [Obsolete("Please use MLResult.CodeToString(MLResult.Code) instead.")]
        public static string GetResultString(MLResultCode result)
        {
            return "";
        }
    }
}
#endif
