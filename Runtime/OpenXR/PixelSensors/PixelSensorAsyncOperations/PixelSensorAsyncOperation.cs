using System;
using System.Collections.Generic;
using MagicLeap.OpenXR.Futures;
using UnityEngine;

namespace MagicLeap.OpenXR.Features.PixelSensors
{
    /// <summary>
    /// A class that represents any asynchronous operation that the pixel sensor performs
    /// </summary>
    internal abstract class PixelSensorAsyncOperation
    {
        internal readonly List<uint> ProcessedStreams = new();
        protected XrFuture Future;
        internal PixelSensorNativeFunctions NativeFunctions;
        internal MagicLeapPixelSensorFeature PixelSensorFeature;
        internal PixelSensor Sensor;
        public PixelSensorAsyncOperationResult OperationResult { get; } = new();

        protected abstract PixelSensorStatus OperationStartStatus { get; }

        protected abstract PixelSensorStatus OperationFinishStatus { get; }

        protected virtual PixelSensorStatus OperationFailedStatus => PixelSensorStatus.Undefined;

        protected abstract HashSet<PixelSensorStatus> IncomingValidStatus { get; }

        public bool DidOperationFinish => OperationResult.DidOperationFinish || OperationResult.DidOperationFail;

        public void Start(IEnumerable<uint> streams)
        {
            if (!IncomingValidStatus.Contains(Sensor.Status) && Sensor.Status != OperationFinishStatus)
            {
                throw new InvalidOperationException($"{Sensor.SensorType}'s streams does not have the right status (Expected Status: [{string.Join(',', IncomingValidStatus)}], Current Status: {Sensor.Status}");
            }

            ProcessedStreams.Clear();
            ProcessedStreams.AddRange(streams);

            if (StartOperation())
            {
                Sensor.Status = OperationStartStatus;
                OperationResult.StartOperation(Sensor.SensorType, OperationStartStatus, ProcessedStreams);
            }
            else
            {
                Sensor.Status = OperationFailedStatus;
                OperationResult.OperationFailed(Sensor.SensorType, OperationFailedStatus, ProcessedStreams);
            }
        }

        protected abstract bool StartOperation();

        protected abstract bool OperationCompleted();

        private void FinishOperation(bool success)
        {
            if (success)
            {
                OnOperationSucceeded();
                OperationResult.FinishOperation(Sensor.SensorType, OperationFinishStatus, ProcessedStreams);
            }
            else
            {
                OperationResult.OperationFailed(Sensor.SensorType, OperationFailedStatus, ProcessedStreams);
            }
        }

        public void PollOperation()
        {
            if (!NativeFunctions.PollFuture(in Future, out var futureState))
            {
                Debug.LogError("Unable to poll future!");
                FinishOperation(false);
                return;
            }

            if (futureState != XrFutureState.Ready)
            {
                return;
            }

            var operationSuccessful = OperationCompleted();
            var targetStatus = operationSuccessful ? OperationFinishStatus : OperationFailedStatus;
            Sensor.Status = targetStatus;
            Sensor.StreamStatus.SetStatusForStreams(targetStatus, ProcessedStreams);
            FinishOperation(operationSuccessful);
        }

        protected virtual void OnOperationSucceeded()
        {
        }
    }
}
