using System;
namespace MagicLeap.OpenXR.Features.PhysicalOcclusion
{
        internal enum XrPhysicalOcclusionStructTypes : uint
        {
            XRTypePhysicalWorldOcclusionSourceController = 1000478002U,
            XRTypePhysicalWorldOcclusionSourceHands = 1000478003U,
            XRTypePhysicalWorldOcclusionSourceDepthSensor = 1000478004U,
            XRTypePhysicalWorldOcclusionSourceEnvironment = 1000478005U,
            XRTypeViewConfigurationOcclusionControllerProperties = 1000478006U,
            XRTypeViewConfigurationOcclusionHandsProperties = 1000478007U,
            XRTypeViewConfigurationOcclusionDepthSensorProperties = 1000478008U,
            XRTypeViewConfigurationOcclusionEnvironmentProperties = 1000478009U,
            XRTypeCompositionLayerPhysicalWorldOcclusion = 1000478010U
        }

        internal struct XrPhysicalWorldOcclusionSource
        {
            internal XrPhysicalOcclusionStructTypes Type;
            internal IntPtr Next;
        }

        internal struct XrPhysicalWorldOcclusionSourceDepthSensor
        {
            internal XrPhysicalOcclusionStructTypes Type;
            internal IntPtr Next;
            internal float NearRange;
            internal float FarRange;
        }

        internal struct XrViewConfigurationOcclusionSourceProperties
        {
            internal XrPhysicalOcclusionStructTypes Type;
            internal IntPtr Next;
            internal XrBool32 SupportsOcclusion;
        }

        internal struct XrViewConfigurationOcclusionDepthSensorProperties
        {
            internal XrPhysicalOcclusionStructTypes Type;
            internal IntPtr Next;
            internal XrBool32 SupportsOcclusion;
            internal float MinNearRange;
            internal float MaxNearRange;
            internal float MinFarRange;
            internal float MaxFarRange;
        }

        internal unsafe struct XrCompositionLayerPhysicalWorldOcclusion
        {
            internal XrPhysicalOcclusionStructTypes Type;
            internal IntPtr Next;
            internal uint OcclusionSourceCount;
            internal IntPtr* OcclusionSources;
        }

        internal struct XrPhysicalOcclusionPropertiesHolder
        {
            internal XrViewConfigurationOcclusionSourceProperties EnvironmentProperties;
            internal XrViewConfigurationOcclusionSourceProperties HandsProperties;
            internal XrViewConfigurationOcclusionSourceProperties ControllerProperties;
            internal XrViewConfigurationOcclusionDepthSensorProperties DepthSensorProperties;

            private void Initialize()
            {
                EnvironmentProperties.Type = XrPhysicalOcclusionStructTypes.XRTypeViewConfigurationOcclusionEnvironmentProperties;
                HandsProperties.Type = XrPhysicalOcclusionStructTypes.XRTypeViewConfigurationOcclusionHandsProperties;
                ControllerProperties.Type = XrPhysicalOcclusionStructTypes.XRTypeViewConfigurationOcclusionControllerProperties;
                DepthSensorProperties.Type = XrPhysicalOcclusionStructTypes.XRTypeViewConfigurationOcclusionDepthSensorProperties;
            }

            internal static XrPhysicalOcclusionPropertiesHolder Create()
            {
                var result = new XrPhysicalOcclusionPropertiesHolder();
                result.Initialize();
                return result;
            }
        }

        internal struct XrPhysicalOcclusionSourcesHolder
        {
            internal XrPhysicalWorldOcclusionSource Environment;
            internal XrPhysicalWorldOcclusionSource Hands;
            internal XrPhysicalWorldOcclusionSource Controller;
            internal XrPhysicalWorldOcclusionSourceDepthSensor DepthSensor;

            internal void Initialize(in XrPhysicalOcclusionPropertiesHolder propertiesHolder)
            {
                Environment.Type = XrPhysicalOcclusionStructTypes.XRTypePhysicalWorldOcclusionSourceEnvironment;
                Hands.Type = XrPhysicalOcclusionStructTypes.XRTypePhysicalWorldOcclusionSourceHands;
                Controller.Type = XrPhysicalOcclusionStructTypes.XRTypePhysicalWorldOcclusionSourceController;
                DepthSensor.Type = XrPhysicalOcclusionStructTypes.XRTypePhysicalWorldOcclusionSourceDepthSensor;
                DepthSensor.NearRange = propertiesHolder.DepthSensorProperties.MinNearRange;
                DepthSensor.FarRange = propertiesHolder.DepthSensorProperties.MinFarRange;
            }
        }
}
