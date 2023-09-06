// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLPowerManager : MLAutoAPISingleton<MLPowerManager>
    {

        /// <summary>
        /// Start the API.
        /// </summary>
        protected override MLResult.Code StartAPI() => Instance.InternalCreateManager();

        /// <summary>
        /// Stop the API.
        /// </summary>
        protected override MLResult.Code StopAPI() => Instance.InternalDestroyManager();

        /// <summary>
        /// Sets the power state of a component.
        /// <param name="settings">
        ///   Settings used by the Power Manager updating a component's power state.
        /// </param>
        /// </summary>
        public static MLResult.Code SetPowerState(Settings settings) => Instance.InternalSetPowerState(settings);

        /// <summary>
        /// Get controller component properties.
        /// <param name="out_properties"> Information about the properties of a component.</param>
        /// </summary>
        public static MLResult.Code GetComponentProperties(out PropertyData out_properties) => Instance.InternalGetComponentProperties(out out_properties);

        /// <summary>
        /// Get available power states for a component.
        /// <param name="data">
        ///   #PowerStateData holding list of available power states.
        /// </param>
        /// </summary>
        public static MLResult.Code GetAvailablePowerStates(out PowerStateData data) => Instance.InternalGetAvailablePowerStates(out data);

        /// <summary>
        /// Get the current power state for a component.
        /// <param name="data">
        ///   #PowerStateData with the current power state of the component.
        /// </param>
        /// </summary>
        public static MLResult.Code GetPowerState(out PowerStateData data) => Instance.InternalGetPowerState(out data);

        /// <summary>
        /// Request a list of the available #PropertyType.
        /// <param name="data">
        ///   Information about the properties of a component.
        /// </param>
        /// </summary>
        public static MLResult.Code GetAvailableProperties(out PropertyTypeData data) => Instance.InternalGetAvailableProperties(out data);



        #region Delegates

        private static event OnErrorOccurredDelegate OnErrorOccurredEvent = delegate { };

        /// <summary>
        /// This callback will be invoked when an #MLPowerManagerError occurs on one of the components.
        /// </summary>
        /// <param name="error">The error which has occurred.</param>
        public delegate void OnErrorOccurredDelegate(in Error error);

        private static event OnPowerStateChangedDelegate OnPowerStateChangedEvent = delegate { };

        /// <summary>
        /// This callback will be invoked when #MLPowerManagerPowerState changes.
        /// </summary>
        /// <param name="state">#MLPowerManagerPowerState representing the new power state.</param>
        public delegate void OnPowerStateChangedDelegate(in PowerState state);

        private static event OnPropertiesChangedDelegate OnPropertiesChangedEvent = delegate { };

        /// <summary>
        /// This callback will be invoked when #MLPowerManagerPropertyData of a component changes.
        /// Only the properties that have changed will be returned, the component may support additional
        /// properties which values were not returned.
        /// </summary>
        /// <param name="properties">#MLPowerManagerPropertyData struct encapsulating the properties changed.</param>
        public delegate void OnPropertiesChangedDelegate(in PropertyData properties);

        /// <summary>
        /// Event callback fired when an error occurs.
        /// </summary>
        public static event OnErrorOccurredDelegate OnErrorOccurred
        {
            add
            {
                OnErrorOccurredEvent += value;
            }
            remove
            {
                OnErrorOccurredEvent -= value;
            }
        }

        /// <summary>
        /// Event callback fired when a power state change occurs.
        /// </summary>
        public static event OnPowerStateChangedDelegate OnPowerStateChanged
        {
            add
            {
                OnPowerStateChangedEvent += value;
            }
            remove
            {
                OnPowerStateChangedEvent -= value;
            }
        }

        /// <summary>
        /// Event callback fired when a property change occurs.
        /// </summary>
        public static event OnPropertiesChangedDelegate OnPropertiesChanged
        {
            add
            {
                OnPropertiesChangedEvent += value;
            }
            remove
            {
                OnPropertiesChangedEvent -= value;
            }
        }
        #endregion

    }
}