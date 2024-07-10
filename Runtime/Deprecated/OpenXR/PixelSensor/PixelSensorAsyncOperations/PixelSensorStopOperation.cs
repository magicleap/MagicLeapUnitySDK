#if UNITY_OPENXR_1_9_0_OR_NEWER
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
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
                    var completionInfo = new XrPixelSensorStopCompletion
                    {
                        Type = XrPixelSensorStructTypes.XrTypePixelSensorStopCompletionML
                    };
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
}
#endif
