#if UNITY_OPENXR_1_9_0_OR_NEWER
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
    {
        internal class PixelSensorStartOperation : PixelSensorAsyncOperation
        {
            public readonly Dictionary<uint, HashSet<PixelSensorMetaDataType>> MetadataTypesForStream = new();
            protected override PixelSensorStatus OperationStartStatus => PixelSensorStatus.Starting;
            protected override PixelSensorStatus OperationFinishStatus => PixelSensorStatus.Started;

            protected override HashSet<PixelSensorStatus> IncomingValidStatus =>
                new()
                {
                    PixelSensorStatus.Configured,
                    PixelSensorStatus.Stopped
                };

            protected override bool StartOperation()
            {
                unsafe
                {
                    var streamArray = new NativeArray<uint>(ProcessedStreams.ToArray(), Allocator.Temp);
                    var startInfo = new XrPixelSensorStartInfo
                    {
                        Type = XrPixelSensorStructTypes.XrTypePixelSensorStartInfoML,
                        StreamCount = (uint)streamArray.Length,
                        Streams = (uint*)streamArray.GetUnsafePtr()
                    };
                    var xrResult = NativeFunctions.XrStartPixelSensorAsync(Sensor.Handle, ref startInfo, out Future);
                    return Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrStartPixelSensorAsync));
                }
            }

            protected override bool OperationCompleted()
            {
                unsafe
                {
                    var completionInfo = new XrPixelSensorStartCompletion
                    {
                        Type = XrPixelSensorStructTypes.XrTypePixelSensorStartCompletionML
                    };
                    NativeFunctions.XrStartPixelSensorComplete(Sensor.Handle, Future, out completionInfo);
                    var didXrCallSucceed = Utils.DidXrCallSucceed(completionInfo.FutureResult, nameof(PixelSensorNativeFunctions.XrStartPixelSensorComplete));
                    return didXrCallSucceed;
                }
            }

            protected override void OnOperationSucceeded()
            {
                base.OnOperationSucceeded();
                foreach (var stream in ProcessedStreams)
                {
                    var streamBuffer = Sensor.StreamBuffers[stream];
                    streamBuffer.RequestedMetaDataTypes.Clear();
                    if (MetadataTypesForStream.TryGetValue(stream, out var metaDataTypes))
                    {
                        streamBuffer.RequestedMetaDataTypes.AddRange(metaDataTypes);
                    }
                    streamBuffer.AllocateBuffer();
                }
            }
        }
    }
}
#endif
