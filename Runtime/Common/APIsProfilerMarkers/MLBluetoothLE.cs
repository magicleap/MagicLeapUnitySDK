// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLBluetoothLE.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// --------------------------------------------------------------------
// %BANNER_END%

using Unity.Profiling;

namespace UnityEngine.XR.MagicLeap
{
    public sealed partial class MLBluetoothLE
    {
        //Profiler markers for MLBluetoothLE Native Bindings calls

        private readonly static ProfilerMarker nativeGattConnectPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.GattConnect");
        private readonly static ProfilerMarker nativeStartupBluetoothPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.StartupBluetooth");
        private readonly static ProfilerMarker nativeGetServiceRecordPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.GetServiceRecord");
        private readonly static ProfilerMarker nativeReadCharacteristicPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.ReadCharacteristic");
        private readonly static ProfilerMarker nativeWriteCharacteristicPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.WriteCharacteristic");
        private readonly static ProfilerMarker nativeReadDescriptorPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.ReadDescriptor");
        private readonly static ProfilerMarker nativeWriteDescriptorPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.WriteDescriptor");
        private readonly static ProfilerMarker nativeEnableCharacteristicNotificationPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.EnableCharacteristicNotification");
        private readonly static ProfilerMarker nativeGetAdapterNamePerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.GetAdapterName");
        private readonly static ProfilerMarker nativeMLBluetoothLeStartScanPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.MLBluetoothLeStartScan");
        private readonly static ProfilerMarker nativeMLBluetoothLeStopScanPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.MLBluetoothLeStopScan");
        private readonly static ProfilerMarker nativeMLBluetoothAdapterGetStatePerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.MLBluetoothAdapterGetState");
        private readonly static ProfilerMarker nativeMLBluetoothAdapterCreateBondPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.MLBluetoothAdapterCreateBond");
        private readonly static ProfilerMarker nativeMLBluetoothGattRequestConnectionPriorityPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.MLBluetoothGattRequestConnectionPriority");
        private readonly static ProfilerMarker nativeMLBluetoothGattDisconnectPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.MLBluetoothGattDisconnect");
        private readonly static ProfilerMarker nativeMLBluetoothGattDiscoverServicesPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.MLBluetoothGattDiscoverServices");
        private readonly static ProfilerMarker nativeMLBluetoothGattReadRemoteRssiPerfMarker = new ProfilerMarker("MLBluetoothNativeBindings.MLBluetoothGattReadRemoteRssi");


        //Profiler markers for MLBluetoothLE calls.

        private readonly static ProfilerMarker startAPIPerfMarker = new ProfilerMarker("MLAudioOutput.StartAPI");

    }
}
