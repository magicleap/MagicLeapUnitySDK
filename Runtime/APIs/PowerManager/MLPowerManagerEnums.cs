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
    public partial class MLPowerManager
    {
        /// <summary>
        /// Represents the different components which can be accessed/controlled using the Power Manager.
        /// </summary>
        public enum Component
        {
            /// <summary>
            /// Invalid or no component.
            /// </summary>
            None,

            /// <summary>
            /// Controller.
            /// </summary>
            Controller
        }

        /// <summary>
        /// Power Manager error codes.
        /// </summary>
        public enum Error
        {
            /// <summary>
            /// Controller component could not be connected due to Invalid SKU.
            /// </summary>
            InvalidSKU
        }

        /// <summary>
        /// Power states.
        /// </summary>
        public enum PowerState
        {
            /// <summary>
            /// Invalid or no power state.
            /// </summary>
            None,

            /// <summary>
            /// Normal mode, this is the default or active state of the component.
            /// </summary>
            Normal,

            /// <summary>
            /// Charging only mode. When charging component it cannot be used.
            /// </summary>
            DisabledWhileCharging,

            /// <summary>
            /// Standby mode.
            /// </summary>
            Standby,

            /// <summary>
            /// Sleep mode.
            /// </summary>
            Sleep
        }

        /// <summary>
        /// Power Manager charging states.
        /// </summary>
        public enum ChargingState
        {
            /// <summary>
            /// Not charging.
            /// </summary>
            NotCharging,

            /// <summary>
            /// Charging normally.
            /// </summary>
            ChargingNormally
        }

        /// <summary>
        /// Connection state of the component.
        /// </summary>
        public enum ConnectionState
        {
            /// <summary>
            /// Component is connected.
            /// </summary>
            Connected,

            /// <summary>
            /// Component is disconnected.
            /// </summary>
            Disconnected
        }

        /// <summary>
        /// Battery info/warning codes.
        /// </summary>
        public enum BatteryInfo
        {
            /// <summary>
            /// No issues reported.
            /// </summary>
            OK,

            /// <summary>
            /// Charge the component soon.
            /// </summary>
            BatteryLow,

            /// <summary>
            /// Charge the component immediately.
            /// </summary>
            BatteryCritical
        }

        /// <summary>
        /// Power Manager property types.
        /// </summary>
        public enum PropertyType
        {
            /// <summary>
            /// Extra info about battery, as represented by MLPowerManagerBatteryInfo.
            /// </summary>
            BatteryInfo,

            /// <summary>
            /// Battery level. Range is between 0 and 100.
            /// </summary>
            BatteryLevel,

            /// <summary>
            /// Charging state.
            /// </summary>
            ChargingState,

            /// <summary>
            /// Connection state.
            /// </summary>
            ConnectionState
        }
    }
}