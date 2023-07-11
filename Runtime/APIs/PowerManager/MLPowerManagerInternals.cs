// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Linq;
using UnityEngine.XR.MagicLeap.Native;
using static UnityEngine.XR.MagicLeap.MLPowerManager.NativeBindings;

namespace UnityEngine.XR.MagicLeap
{
    public partial class MLPowerManager
    {
        private MLResult.Code InternalCreateManager()
        {
            Component controller = Component.Controller;
            MLResult.Code resultCode = NativeBindings.MLPowerManagerCreate(controller, out Handle);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerCreate)))
            {
                InternalSetCallbacks();
            }

            return resultCode;
        }

        private MLResult.Code InternalSetCallbacks()
        {
            MLPowerManagerCallbacks callbacks = MLPowerManagerCallbacks.Init();
            MLResult.Code resultCode = NativeBindings.MLPowerManagerSetCallbacks(Handle, callbacks, System.IntPtr.Zero);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerCreate));

            return resultCode;
        }

        private MLResult.Code InternalDestroyManager()
        {
            MLResult.Code resultCode = NativeBindings.MLPowerManagerDestroy(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerDestroy));

            return resultCode;
        }

        private MLResult.Code InternalSetPowerState(Settings settings)
        {
            MLPowerManagerPowerStateSettings stateSettings = MLPowerManagerPowerStateSettings.Init();
            stateSettings.State = settings.State;
            MLResult.Code resultCode = NativeBindings.MLPowerManagerSetPowerState(Handle, stateSettings);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerSetPowerState));

            return resultCode;
        }

        private MLResult.Code InternalGetComponentProperties(out PropertyData data)
        {
            MLPowerManagerPropertyInfo in_info = MLPowerManagerPropertyInfo.Init();
            MLPowerManagerPropertyData out_data = new();
            data = new();
            MLResult.Code resultCode = NativeBindings.MLPowerManagerGetComponentProperties(Handle, in_info, out out_data);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerGetComponentProperties)))
            {
                data.Properties = MLConvert.MarshalUnmanagedArray<ComponentProperty>(out_data.Properties, (int)out_data.Size);
                InternalReleasePropertyData(out_data);
            }

            return resultCode;
        }

        private MLResult.Code InternalReleasePropertyData(in MLPowerManagerPropertyData data)
        {
            MLResult.Code resultCode = NativeBindings.MLPowerManagerReleasePropertyData(Handle, in data);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerReleasePropertyData));

            return resultCode;
        }

        private MLResult.Code InternalReleasePowerStateData(in MLPowerManagerPowerStateData data)
        {
            MLResult.Code resultCode = NativeBindings.MLPowerManagerReleasePowerStateData(Handle, data);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerReleasePowerStateData));

            return resultCode;
        }

        private MLResult.Code InternalReleasePropertyTypeData(in MLPowerManagerPropertyTypeData data)
        {
            MLResult.Code resultCode = NativeBindings.MLPowerManagerReleasePropertyTypeData(Handle, data);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerReleasePropertyTypeData));

            return resultCode;
        }

        private MLResult.Code InternalGetAvailablePowerStates(out PowerStateData out_data)
        {
            MLPowerManagerPowerStateInfo in_info = MLPowerManagerPowerStateInfo.Init();
            MLPowerManagerPowerStateData out_states;
            out_data = new();

            MLResult.Code resultCode = NativeBindings.MLPowerManagerGetAvailablePowerStates(Handle, in_info, out out_states);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerGetAvailablePowerStates)))
            {
                var array = MLConvert.MarshalUnmanagedArray<int>(out_states.PowerState, (int)out_states.Size);
                out_data.PowerStates = array.Cast<PowerState>().ToArray();
                InternalReleasePowerStateData(out_states);
            }

            return resultCode;
        }

        private MLResult.Code InternalGetPowerState(out PowerStateData data)
        {
            MLPowerManagerPowerStateInfo info = MLPowerManagerPowerStateInfo.Init();
            MLPowerManagerPowerStateData out_state;
            data = new();

            MLResult.Code resultCode = NativeBindings.MLPowerManagerGetPowerState(Handle, info, out  out_state);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerGetPowerState)))
            {
                var array = MLConvert.MarshalUnmanagedArray<int>(out_state.PowerState, (int)out_state.Size);
                data.PowerStates = array.Cast<PowerState>().ToArray();
                InternalReleasePowerStateData(out_state);
            }

            return resultCode;
        }

        private MLResult.Code InternalGetAvailableProperties(out PropertyTypeData data)
        {
            MLPowerManagerPropertyTypeInfo in_info = MLPowerManagerPropertyTypeInfo.Init();
            MLPowerManagerPropertyTypeData out_properties;
            data = new();

            MLResult.Code resultCode = NativeBindings.MLPowerManagerGetAvailableProperties(Handle, in_info, out  out_properties);
            if (MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLPowerManagerGetAvailableProperties)))
            {
                var array = MLConvert.MarshalUnmanagedArray<int>(out_properties.PropertyTypes, (int)out_properties.Size);
                data.PropertyTypes = array.Cast<PropertyType>().ToArray();
                InternalReleasePropertyTypeData(out_properties);
            }

            return resultCode;
        }
    }
}