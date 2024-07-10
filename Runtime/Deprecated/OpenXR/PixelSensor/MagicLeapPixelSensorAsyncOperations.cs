using System.Collections.Generic;
using System.Linq;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    public partial class MagicLeapPixelSensorFeature
    {
        internal partial class PixelSensor
        {
            internal PixelSensorAsyncOperationResult StartSensor(IEnumerable<uint> streams, Dictionary<uint, PixelSensorMetaDataType[]> metaDataTypes)
            {
                //If we are trying to start streams that have not been configured
                var streamArr = streams as uint[] ?? streams.ToArray();
                var startOperation = GetOperation<PixelSensorStartOperation>();
                if (metaDataTypes != null)
                {
                    foreach (var (stream, requestedMetadatas) in metaDataTypes)
                    {
                        if (requestedMetadatas == null)
                        {
                            continue;
                        }
                        startOperation.MetadataTypesForStream.Remove(stream);
                        startOperation.MetadataTypesForStream.Add(stream, new HashSet<PixelSensorMetaDataType>(requestedMetadatas));
                    }
                }

                startOperation.Start(streamArr);
                return startOperation.OperationResult;
            }

            internal PixelSensorAsyncOperationResult ConfigureSensor(IEnumerable<uint> streams)
            {
                var configOperation = GetOperation<PixelSensorConfigureOperation>();
                configOperation.Start(streams);
                return configOperation.OperationResult;
            }

            internal PixelSensorAsyncOperationResult StopSensor(IEnumerable<uint> streams)
            {
                var stopOperation = GetOperation<PixelSensorStopOperation>();
                stopOperation.Start(streams);
                return stopOperation.OperationResult;
            }
        }
    }
}
