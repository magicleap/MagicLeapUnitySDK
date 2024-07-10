using System;
using System.Collections.Generic;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;

namespace MagicLeap.OpenXR.Features.PixelSensors
{
    internal class PixelSensorConfigureOperation : PixelSensorAsyncOperation
    {
        private NativeArray<IntPtr> appliedConfigArray;

        private NativeArray<XrPixelSensorConfigData> configHolder;
        protected override PixelSensorStatus OperationStartStatus => PixelSensorStatus.Configuring;
        protected override PixelSensorStatus OperationFinishStatus => PixelSensorStatus.Configured;

        protected override PixelSensorStatus OperationFailedStatus => PixelSensorStatus.NotConfigured;

        protected override HashSet<PixelSensorStatus> IncomingValidStatus =>
            new()
            {
                PixelSensorStatus.NotConfigured,
                PixelSensorStatus.Stopped
            };

        protected override bool StartOperation()
        {
            unsafe
            {
                var appliedConfigurations = Sensor.AppliedConfigs.ToArray();
                //Has to persist as the operation expects these arrays to exist for the entire duration
                configHolder = new NativeArray<XrPixelSensorConfigData>(appliedConfigurations.Length, Allocator.Persistent);
                appliedConfigArray = new NativeArray<IntPtr>(appliedConfigurations.Length, Allocator.Persistent);
                for (var i = 0; i < configHolder.Length; i++)
                {
                    ref var currentConfig = ref appliedConfigurations[i];
                    var currentConfigHolder = new XrPixelSensorConfigData();
                    currentConfigHolder.AssignFromConfig(currentConfig);
                    appliedConfigArray[i] = currentConfigHolder.GetBaseHeader();
                    configHolder[i] = currentConfigHolder;
                }

                var streamArray = new NativeArray<uint>(ProcessedStreams.ToArray(), Allocator.Temp);
                var startInfo = new XrPixelSensorConfigInfo
                {
                    Type = XrPixelSensorStructTypes.XrTypePixelSensorConfigInfoML,
                    Next = default,
                    StreamCount = (uint)streamArray.Length,
                    Streams = (uint*)streamArray.GetUnsafePtr(),
                    ConfigurationCount = (uint)appliedConfigArray.Length,
                    Configurations = (IntPtr*)appliedConfigArray.GetUnsafePtr()
                };
                var xrResult = NativeFunctions.XrConfigurePixelSensorAsync(Sensor.Handle, ref startInfo, out Future);
                return Utils.DidXrCallSucceed(xrResult, nameof(PixelSensorNativeFunctions.XrConfigurePixelSensorAsync));
            }
        }

        protected override bool OperationCompleted()
        {
            unsafe
            {
                var completionInfo = new XrPixelSensorConfigureCompletion
                {
                    Type = XrPixelSensorStructTypes.XrTypePixelSensorConfigureCompletionML
                };
                var xrResult = NativeFunctions.XrConfigurePixelSensorComplete(Sensor.Handle, Future, out completionInfo);
                for (var i = 0; i < configHolder.Length; i++)
                {
                    configHolder[i].Dispose();
                }

                configHolder.Dispose();
                appliedConfigArray.Dispose();
                return Utils.DidXrCallSucceed(xrResult, $"{nameof(PixelSensorNativeFunctions.XrConfigurePixelSensorComplete)}BeforeFuture") && 
                       Utils.DidXrCallSucceed(completionInfo.FutureResult, nameof(PixelSensorNativeFunctions.XrConfigurePixelSensorComplete));
            }
        }
    }
}
