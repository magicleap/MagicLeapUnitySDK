using System;
using System.Collections.Generic;
using UnityEngine;

namespace MagicLeap.OpenXR.Features.PixelSensors
{
    /// <summary>
    /// Represents the progress of an async Pixel sensor operation
    /// <para>Can be yielded inside a coroutine or has events that can be subscribed to</para>
    /// </summary>
    public class PixelSensorAsyncOperationResult : CustomYieldInstruction
    {
        private static PixelSensorAsyncOperationResult FailedOperation;
        public bool DidOperationFinish { get; private set; }
        public bool DidOperationFail { get; private set; }
        public bool DidOperationSucceed => DidOperationFinish && !DidOperationFail;

        public override bool keepWaiting => !DidOperationFinish;

        internal static PixelSensorAsyncOperationResult FailedOperationResult
        {
            get
            {
                FailedOperation ??= new PixelSensorAsyncOperationResult();
                FailedOperation.DidOperationFail = true;
                FailedOperation.DidOperationFinish = true;
                return FailedOperation;
            }
        }

        public event Action<PixelSensorId, PixelSensorStatus, IReadOnlyList<uint>> OnOperationFinishedSuccessfully;
        public event Action<PixelSensorId, PixelSensorStatus, IReadOnlyList<uint>> OnOperationStarted;
        public event Action<PixelSensorId, PixelSensorStatus, IReadOnlyList<uint>> OnOperationFailed;

        internal void StartOperation(PixelSensorId sensorType, PixelSensorStatus status, IReadOnlyList<uint> streams)
        {
            DidOperationFinish = false;
            DidOperationFail = false;
            OnOperationStarted?.Invoke(sensorType, status, streams);
        }

        internal void OperationFailed(PixelSensorId sensorType, PixelSensorStatus status, IReadOnlyList<uint> streams)
        {
            DidOperationFinish = true;
            DidOperationFail = true;
            OnOperationFailed?.Invoke(sensorType, status, streams);
        }

        internal void FinishOperation(PixelSensorId sensorType, PixelSensorStatus outputStatus, IReadOnlyList<uint> streams)
        {
            DidOperationFinish = true;
            DidOperationFail = false;
            OnOperationFinishedSuccessfully?.Invoke(sensorType, outputStatus, streams);
        }
    }
}
