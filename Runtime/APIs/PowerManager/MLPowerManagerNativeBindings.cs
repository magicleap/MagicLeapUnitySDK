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
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.MagicLeap.Native;

    public partial class MLPowerManager
    {
        /// <summary>
        /// See ml_power_manager.h for additional comments.
        /// </summary>
        internal class NativeBindings : Native.MagicLeapNativeBindings
        {
            #region Delegates
            /// <summary>
            /// This callback will be invoked when #MLPowerManagerPowerState changes.
            /// </summary>
            /// <param name="state">#MLPowerManagerPowerState representing the new power state.</param>
            /// <param name="user_data">User data as passed to the #MLPowerManagerSetCallbacks.</param>
            private delegate void OnPowerStateChangedInternalDelegate(PowerState state, in IntPtr user_data);

            /// <summary>
            /// This callback will be invoked when #MLPowerManagerPropertyData of a component changes.
            /// Only the properties that have changed will be returned, the component may support additional
            /// properties which values were not returned.
            /// </summary>
            /// <param name="properties">#MLPowerManagerPropertyData struct encapsulating the properties changed.</param>
            /// <param name="user_data">User data as passed to the #MLPowerManagerSetCallbacks.</param>
            private delegate void OnPropertiesChangedInternalDelegate(in MLPowerManagerPropertyData properties, in IntPtr user_data);

            /// <summary>
            /// This callback will be invoked when an #MLPowerManagerError occurs on one of the components.
            /// </summary>
            /// <param name="error">The error which has occurred.</param>
            /// <param name="user_data">User data as passed to the #MLPowerManagerSetCallbacks.</param>
            private delegate void OnErrorOccurredInternalDelegate(Error error, in IntPtr user_data);
            #endregion

            #region CallbackMethods
            [AOT.MonoPInvokeCallback(typeof(OnPowerStateChangedInternalDelegate))]
            public static void HandleOnPowerChanged(PowerState state, in IntPtr user_data)
            {
                PowerState newState = state;
                MLThreadDispatch.ScheduleMain(() =>
                {
                    OnPowerStateChangedEvent?.Invoke(newState);
                });
            }

            [AOT.MonoPInvokeCallback(typeof(OnPropertiesChangedInternalDelegate))]
            public static void HandleOnPropertiesChanged(in MLPowerManagerPropertyData properties, in IntPtr user_data)
            {
                PropertyData newProperties;
                newProperties.Properties = MLConvert.MarshalUnmanagedArray<ComponentProperty>(properties.Properties, (int)properties.Size);

                MLThreadDispatch.ScheduleMain(() =>
                {
                    OnPropertiesChangedEvent?.Invoke(newProperties);
                });
            }

            [AOT.MonoPInvokeCallback(typeof(OnErrorOccurredInternalDelegate))]
            public static void HandleOnErrorOccurred(Error error, in IntPtr user_data)
            {
                var newError = error;
                MLThreadDispatch.ScheduleMain(() =>
                {
                    OnErrorOccurredEvent?.Invoke(newError);
                });
            }
            #endregion

            #region Structs
            [StructLayout(LayoutKind.Sequential)]
            public struct MLPowerManagerComponentProperty
            {
                /// <summary>
                /// The type of each property.
                /// </summary>
                public PropertyType Type;

                /// <summary>
                /// Union of ComponentPropertyData. This struct and union is used as a flexible way
                /// for each component to output an array containing distinct types of data.
                /// </summary>
                public ComponentPropertyData Data;
            }

            /// <summary>
            /// A structure to encapsulate output data when getting the
            /// current properties.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLPowerManagerPropertyData
            {
                /// <summary>
                /// Size of Properties array.
                /// </summary>
                public byte Size;

                /// <summary>
                /// Array of #MLPowerManagerComponentProperty elements.
                /// </summary>
                public IntPtr Properties;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct MLPowerManagerPropertyTypeData
            {
                /// <summary>
                /// Size of PropertyTypes array.
                /// </summary>
                public byte Size;

                /// <summary>
                /// Array of PropertyType elements. 
                /// </summary>
                public IntPtr PropertyTypes;
            }

            /// <summary>
            /// A structure to encapsulate information used by the Power Manager
            /// when getting the available property types.
            /// </summary
            [StructLayout(LayoutKind.Sequential)]
            public struct MLPowerManagerPropertyTypeInfo
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                public static MLPowerManagerPropertyTypeInfo Init(uint version = 1)
                {
                    return new MLPowerManagerPropertyTypeInfo
                    {
                        Version = version,
                    };
                }
            }

            /// <summary>
            /// A structure to encapsulate info data used by the Power Manager
            /// when getting the current properties.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLPowerManagerPropertyInfo
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                public static MLPowerManagerPropertyInfo Init(uint version = 1)
                {
                    return new MLPowerManagerPropertyInfo
                    {
                        Version = version,
                    };
                }
            }

            /// <summary>
            /// A structure to encapsulate settings used by the Power Manager
            /// when requesting the power state to be changed.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLPowerManagerPowerStateSettings
            {
                /// <summary>
                ///  Version of this structure.
                /// </summary>
                public uint Version;

                /// <summary>
                /// New power state to request.
                /// </summary>
                public PowerState State;

                public static MLPowerManagerPowerStateSettings Init(uint version = 1)
                {
                    return new MLPowerManagerPowerStateSettings
                    {
                        Version = version,
                        State = PowerState.None,
                    };
                }
            }

            /// <summary>
            /// A structure to encapsulate info data used by the Power Manager
            /// when getting the current power state.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLPowerManagerPowerStateInfo
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                public static MLPowerManagerPowerStateInfo Init(uint version = 1)
                {
                    return new MLPowerManagerPowerStateInfo
                    {
                        Version = version,
                    };
                }
            }

            /// <summary>
            /// A structure to encapsulate output data when either getting
            /// available power states, or the current power state.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLPowerManagerPowerStateData
            {
                /// <summary>
                /// Size of #power_states array.
                /// </summary>
                public byte Size;

                /// <summary>
                /// Array of #MLPowerManagerPowerState elements.
                /// </summary>
                public IntPtr PowerState;
            }

            /// <summary>
            /// A structure containing Power Manager callback events.
            /// Individual callbacks which are not required by the Power Manager can be NULL.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            public struct MLPowerManagerCallbacks
            {
                /// <summary>
                /// Version of this structure.
                /// </summary>
                public uint Version;

                private OnPowerStateChangedInternalDelegate OnPowerChanged;

                private OnPropertiesChangedInternalDelegate OnPropertiesChanged;

                private OnErrorOccurredInternalDelegate OnErrorOccurred;

                public static MLPowerManagerCallbacks Init(uint version = 1)
                {
                    return new MLPowerManagerCallbacks
                    {
                        Version = version,
                        OnPowerChanged = NativeBindings.HandleOnPowerChanged,
                        OnPropertiesChanged = NativeBindings.HandleOnPropertiesChanged,
                        OnErrorOccurred = NativeBindings.HandleOnErrorOccurred
                    };
                }
            }
            #endregion


            #region DLL
            /// <summary>
            /// Creates a Power Manager handle for a specified component.
            /// </summary>
            /// <param name="component">The component specific to the handle to be created.</param>
            /// <param name="handle">The handle to be created.</param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerCreate(Component component, out ulong handle);

            /// <summary>
            /// Destroys a Power Manager handle.
            /// </summary>
            /// <param name="handle">The Power Manager handle for a specific component to be destroyed.</param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerDestroy(ulong handle);

            /// <summary>
            /// Register Power Manager callbacks for a specific handle.
            /// </summary>
            /// <param name="handle">Power Manager handle for component to set #MLPowerManagerCallbacks for.</param>
            /// <param name="callbacks">Callbacks to receive Power Manager events. Set this to NULL to unregister callbacks.</param>
            /// <param name="user_data">The caller can pass in user context data that will be returned in the callback(can be NULL). </param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerSetCallbacks(ulong handle, in MLPowerManagerCallbacks callbacks , in IntPtr user_data);

            /// <summary>
            /// Sets the power state of a component.
            /// </summary>
            /// <param name="handle">Power Manager handle for component to set power state for.</param>
            /// <param name="settings">Settings used by the Power Manager updating a component's power state.</param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerSetPowerState(ulong handle, in MLPowerManagerPowerStateSettings settings);

            /// <summary>
            /// Gets the power manager properties of a component.
            /// </summary>
            /// <param name="handle">Power Manager handle for component to get properties of.</param>
            /// <param name="in_info">
            ///  #MLPowerManagerPropertyInfo struct filled with information about
            ///  the power manager properties of a component to request.
            /// </param>
            /// <param name="out_properties">
            ///  Information about the properties of a component. Must be
            ///  released using #MLPowerManagerReleasePropertyData after each successful call.
            /// </param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerGetComponentProperties(ulong handle, in MLPowerManagerPropertyInfo in_info, out MLPowerManagerPropertyData out_properties);

            /// <summary>
            /// Releases specified #MLPowerManagerPropertyData.
            /// </summary>
            /// <param name="handle">Power Manager handle for component relating to #MLPowerManagerPropertyData.</param>
            /// <param name="properties">
            ///  Pointer to a #MLPowerManagerPropertyData returned
            ///  from #MLPowerManagerGetComponentProperties.
            /// </param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerReleasePropertyData(ulong handle, in MLPowerManagerPropertyData properties);

            /// <summary>
            /// Query available power states for a component.
            /// </summary>
            /// <param name="handle">Power Manager handle for component to get list of all available power states for.</param>
            /// <param name="in_info">
            ///  #MLPowerManagerPowerStateInfo struct filled with data
            ///  to be used by the Power Manager when requesting/receiving all available power states.
            /// </param>
            /// <param name="out_states">#MLPowerManagerPowerStateData holding list of available power states.</param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerGetAvailablePowerStates(ulong handle, in MLPowerManagerPowerStateInfo in_info, out MLPowerManagerPowerStateData out_states);

            /// <summary>
            /// Gets the power state of a component.
            /// </summary>
            /// <param name="handle">Power Manager handle for component to get power state from.</param>
            /// <param name="in_info">
            ///  #MLPowerManagerPowerStateInfo struct filled with data
            ///  to be used by the Power Manager when requesting/receiving power state.
            /// </param>
            /// <param name="out_state">
            ///  #MLPowerManagerPowerStateData with the current power
            ///  state.Must be released using #MLPowerManagerReleasePowerStateData
            ///  after each successful call.
            /// </param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerGetPowerState(ulong handle, in MLPowerManagerPowerStateInfo in_info, out MLPowerManagerPowerStateData out_state);

            /// <summary>
            /// Releases specified #MLPowerManagerPowerStateData.
            /// </summary>
            /// <param name="handle">Power Manager handle for component relating to #MLPowerManagerPowerStateData.</param>
            /// <param name="power_states">Pointer to a #MLPowerManagerPowerStateData.</param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerReleasePowerStateData(ulong handle, in MLPowerManagerPowerStateData power_states);

            /// <summary>
            /// Request a list of the available #MLPowerManagerPropertyType.
            /// </summary>
            /// <param name="handle">Power Manager handle for component to get properties from.</param>
            /// <param name="in_info">
            ///  #MLPowerManagerPropertyTypeInfo struct filled with data
            ///  to be used by the Power Manager when requesting/receiving available property types.
            /// </param>
            /// <param name="out_properties">
            ///  Information about the properties of a component. Must be
            ///  released using #MLPowerManagerReleasePropertyTypeData after each successful call.
            /// </param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerGetAvailableProperties(ulong handle, in MLPowerManagerPropertyTypeInfo in_info, out MLPowerManagerPropertyTypeData out_properties);

            /// <summary>
            /// Releases specified #MLPowerManagerPropertyTypeData.
            /// </summary>
            /// <param name="handle">Power Manager handle for component relating to #MLPowerManagerPropertyTypeData.</param>
            /// <param name="properties">Pointer to a #MLPowerManagerPropertyTypeData.</param>
            [DllImport(MLPowerManagerDll, CallingConvention = CallingConvention.Cdecl)]
            public extern static MLResult.Code MLPowerManagerReleasePropertyTypeData(ulong handle, in MLPowerManagerPropertyTypeData properties);
            #endregion
        }
    }
}