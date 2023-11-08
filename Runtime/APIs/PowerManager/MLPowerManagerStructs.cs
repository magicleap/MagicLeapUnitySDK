// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using static UnityEngine.XR.MagicLeap.MLPowerManager;
using static UnityEngine.XR.MagicLeap.MLPowerManager.NativeBindings;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLPowerManager
    {
        [StructLayout(LayoutKind.Explicit)]
        public struct ComponentPropertyData
        {
            /// <summary>
            /// Extra info about battery.
            /// </summary>
            [FieldOffset(0)]
            public BatteryInfo BatteryInfo;

            /// <summary>
            /// Battery level. Range is between 0 and 100.
            /// </summary>
            [FieldOffset(0)]
            public byte BatteryLevel;

            /// <summary>
            /// Charging state.
            /// </summary>
            [FieldOffset(0)]
            public ChargingState ChargingState;

            /// <summary>
            /// Connection state. 
            /// </summary>
            [FieldOffset(0)]
            public ConnectionState ConnectionState;
        }

        /// <summary>
        /// A structure to encapsulate the data for each #MLPowerManagerPropertyType.
        /// </summary>
        public struct ComponentProperty
        {
            /// <summary>
            /// The type of each property.
            /// </summary>
            public PropertyType Type;

            public ComponentPropertyData Data;
        }

        /// <summary>
        /// A structure to encapsulate output data when getting the
        /// current properties.
        /// </summary>
        public struct PropertyData
        {
            /// <summary>
            /// Array of #MLPowerManagerComponentProperty elements.
            /// </summary>
            public ComponentProperty[] Properties;
        }

        /// <summary>
        /// A structure to encapsulate output data when getting a component's available property types.
        /// </summary>
        public struct PropertyTypeData
        {
            /// <summary>
            /// Array of #PropertyType elements.
            /// </summary>
            public PropertyType[] PropertyTypes;
        }

        /// <summary>
        /// A structure to encapsulate settings used by the Power Manager
        /// when requesting the power state to be changed.
        /// </summary>
        public struct Settings
        {
            /// <summary>
            /// New power state to request.
            /// </summary>
            public PowerState State;
        }

        /// <summary>
        /// A structure to encapsulate output data when either getting
        /// available power states, or the current power state.
        /// </summary>
        public struct PowerStateData
        {
            /// <summary>
            /// Array of PowerState elements.
            /// </summary>
            public PowerState[] PowerStates;
        }
    }
}