using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace MagicLeap.OpenXR.Features.PixelSensors
{
    internal class PixelSensorStopOperation : PixelSensorAsyncOperation
    {
        protected override PixelSensorStatus OperationStartStatus => PixelSensorStatus.Stopping;
        protected override PixelSensorStatus OperationFinishStatus => PixelSensorStatus.Stopped;

        protected override HashSet<PixelSensorStatus> IncomingValidStatus =>
            new()
            {
                PixelSensorStatus.Started
            };

        protected override bool StartOperation()
        {
            unsafe
            {
                var streamArray = new NativeArray<uint>(ProcessedStreams.ToArray(), Allocator.Temp);
                var startInfo = new XrPixelSensorStopInfo
                {
                    Type = XrPixelSensorStructTypes.XrTypePixelSensorStopInfoML,
                    StreamCount = (uint)streamArray.Length,
                    Streams = (uint*)streamArray.GetUnsafePtr()
                };
                var xrResult = NativeFunctions.XrStopPixelSensorAsync(Sensor.Handle, ref startInfo, out Future);
                return Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrStopPixelSensorAsync));
            }
        }

        protected override bool OperationCompleted()
        {
            unsafe
            {
                Utils.OpenXRStructHelpers<XrPixelSensorStopCompletion>.Create(XrPixelSensorStructTypes.XrTypePixelSensorStopCompletionML, out var completionInfo);
                NativeFunctions.XrStopPixelSensorComplete(Sensor.Handle, Future, out completionInfo);
                return Utils.DidXrCallSucceed(completionInfo.FutureResult, nameof(PixelSensorNativeFunctions.XrStopPixelSensorComplete));
            }
        }

        protected override void OnOperationSucceeded()
        {
            base.OnOperationSucceeded();
            foreach (var stream in ProcessedStreams)
            {
                Sensor.StreamBuffers[stream].Dispose();
            }
        }
    }
}
