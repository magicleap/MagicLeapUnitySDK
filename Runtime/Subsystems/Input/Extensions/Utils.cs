// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Collections.Generic;

namespace UnityEngine.XR.MagicLeap
{
    public static partial class InputSubsystem
    {
        public static class Utils
        {
            /// <summary>
            /// Returns valid Magic Leap input device for provided <seealso cref="InputDeviceCharacteristics"/>.
            /// If there is none matching device the returned <seealso cref="InputDevice.isValid"/> gonna be set as false.  
            /// </summary>
            public static InputDevice FindMagicLeapDevice(InputDeviceCharacteristics inputDeviceCharacteristics)
            {
                List<InputDevice> devices = new List<InputDevice>();
                InputDevices.GetDevicesWithCharacteristics(inputDeviceCharacteristics, devices);
                foreach (InputDevice device in devices)
                {
                    if (device.name.Contains("MagicLeap") && device.isValid)
                    {
                        return device;
                    }
                }

                return new InputDevice();
            }

            /// <summary>
            /// Instructs the input subsystem to use MLPerceptionGetPerdictedSnapshot by default.
            /// </summary>
            /// <param name="timestamp">
            /// Time representing the time in ms for which to predict poses (the max value is 100 ms).
            /// This timestamp typically comes from out_frame_info.predicted_display_time.
            /// </param>
            /// <param name="predictSnapshots">Boolean representing whether or not to predict the snapshot.</param>
            public static void PredictSnapshot(long timestamp, bool predictSnapshots) => MagicLeapXrProviderNativeBindings.PredictSnapshot(timestamp, predictSnapshots);
            
            /// <summary>
            /// Reset the snapshot state of the subsystem to before prediction. Use this to cleanup after PredictSnapshot
            /// </summary>
            public static void ResetSnapshotPrediction() => MagicLeapXrProviderNativeBindings.ResetSnapshotPrediction();
        }
    }
}
