// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file = "MLLightingTracking.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using UnityEngine;

#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// Provides environment lighting information.
    /// Capturing images or video will stop the lighting information update.
    /// </summary>
    public sealed partial class MLLightingTracking : MLAutoAPISingleton<MLLightingTracking>
    {
#if UNITY_MAGICLEAP || UNITY_ANDROID
        /// <summary>
        /// Time when captured in nanoseconds since the Epoch.
        /// </summary>
        private static readonly DateTime EPOCHSTART = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);

        /// <summary>
        /// Handle to the native lighting tracking.
        /// </summary>
        private ulong nativeTracker = MagicLeapNativeBindings.InvalidHandle;

        /// <summary>
        /// Color temperature state.
        /// </summary>
        private NativeBindings.ColorTemperatureStateNative temperatureState;

        /// <summary>
        /// Stores the color value.
        /// </summary>
        private Color globalTemperatureColor;

        /// <summary>
        /// Indicates if MLLightTracker() fails to get color temperature state.
        /// </summary>
        private bool getColorTemperatureStateFailed = false;

        /// <summary>
        /// The CIE Tri-Stimulus values in Q16 format
        /// </summary>
        private Vector3 tristimulusValues;

        /// <summary>
        /// Initializes a new instance of the <see cref="MLLightingTracking" /> class from being created.
        /// Constructor used to initialize MLLightingTracking data members.
        /// </summary>
        public MLLightingTracking()
        {
            this.nativeTracker = MagicLeapNativeBindings.InvalidHandle;
            this.temperatureState = NativeBindings.ColorTemperatureStateNative.Create();
            this.globalTemperatureColor = new Color();
        }
#endif

#if UNITY_MAGICLEAP || UNITY_ANDROID
        /// <summary>
        /// Gets kelvin color temperature
        /// </summary>
        public static ushort GlobalTemperature => Instance.temperatureState.ColorTemperature;

        /// <summary>
        /// Gets time at which global temperature data was captured (represented as elapsed seconds
        /// since DateTime.MinValue).
        /// </summary>
        public static MLTime GlobalTemperatureAgeSeconds => Instance.temperatureState.Timestamp;

        /// <summary>
        /// Gets RGB representation of the kelvin color temperature.
        /// </summary>
        public static Color GlobalTemperatureColor => Instance.globalTemperatureColor;

        /// <summary>Gets the CIE Tri-Stimulus values in Q16 format (scaled 2^16.)</summary>
        public static Vector3 TristimulusValues => Instance.tristimulusValues;

#if !DOXYGEN_SHOULD_SKIP_THIS
        /// <summary>
        /// Turn on Lighting Tracking API.
        /// </summary>
        /// <returns>MLResult.Ok if successful.</returns>
        protected override MLResult.Code StartAPI()
        {
            startAPIPerfMarker.Begin();
            this.nativeTracker = MagicLeapNativeBindings.InvalidHandle;
            nativeMLLightingTrackingCreatePerfMarker.Begin();
            MLResult.Code resultCode = NativeBindings.MLLightingTrackingCreate(ref this.nativeTracker);
            nativeMLLightingTrackingCreatePerfMarker.End();

            MLResult.DidNativeCallSucceed(resultCode, "MLLightingTracking.StartAPI()");

            startAPIPerfMarker.End();
            return resultCode;
        }
#endif // DOXYGEN_SHOULD_SKIP_THIS

        /// <summary>
        /// Stops the Lighting Tracking API.
        /// </summary>
        /// <returns>MLResult.Ok if successful.</returns>
        protected override MLResult.Code StopAPI()
        {
            MLResult.Code result = MLResult.Code.Ok;
            cleanupAPIPerfMarker.Begin();

            if (MagicLeapNativeBindings.MLHandleIsValid(this.nativeTracker))
            {
                nativeMLLightingTrackingDestroyPerfMarker.Begin();
                result = NativeBindings.MLLightingTrackingDestroy(this.nativeTracker);
                nativeMLLightingTrackingDestroyPerfMarker.End();

                MLResult.DidNativeCallSucceed(result, "MLLightingTracking.StopAPI()");

                this.nativeTracker = MagicLeapNativeBindings.InvalidHandle;
            }
            cleanupAPIPerfMarker.End();

            return result;
        }

        /// <summary>
        /// Update is called every frame. <c>>Monobehaviour</c>. callback.
        /// </summary>
        protected override void Update()
        {
            updatePerfMarker.Begin();
            nativeMLLightingTrackingGetColorTemperatureStatePerfMarker.Begin();
            MLResult.Code result = NativeBindings.MLLightingTrackingGetColorTemperatureState(this.nativeTracker, ref this.temperatureState);
            nativeMLLightingTrackingGetColorTemperatureStatePerfMarker.End();

            if (result != MLResult.Code.Ok)
            {
                if (!this.getColorTemperatureStateFailed)
                {
                    MLPluginLog.WarningFormat("MLLightingTracking.Update failed getting color temperature state. Reason: {0}", MLResult.CodeToString(result));
                    this.getColorTemperatureStateFailed = true;
                }
            }
            else
            {
                this.getColorTemperatureStateFailed = false;

                // populate tristimulus values
                this.tristimulusValues.x = this.temperatureState.XCIE;
                this.tristimulusValues.y = this.temperatureState.YCIE;
                this.tristimulusValues.z = this.temperatureState.ZCIE;
            }

            this.CalculateGlobalTemperatureColor();
            updatePerfMarker.End();
        }

        /// <summary>
        /// Calculates the elapsed seconds since start of the API.
        /// </summary>
        /// <param name="mlTimestamp">Time when captured in nanoseconds since the Epoch.</param>
        /// <returns>Elapsed time in seconds</returns>
        private double ElapsedSeconds(MLTime mlTimestamp)
        {
            MLTime.ConvertMLTimeToSystemTime(mlTimestamp, out long nanoseconds);
            double timestampInSeconds = nanoseconds / Mathf.Pow(10, 9);
            DateTime timestampDateTime = EPOCHSTART.AddSeconds(timestampInSeconds);
            return timestampDateTime.Subtract(DateTime.MinValue).TotalSeconds;
        }

        /// <summary>
        /// Calculates the global color value using the lighting tracker data.
        /// </summary>
        private void CalculateGlobalTemperatureColor()
        {
            // Algorithm from: http://www.tannerhelland.com/4435/convert-temperature-rgb-algorithm-code/
            double scaledTemp = Instance.temperatureState.ColorTemperature / 100.0;
            double red = 0.0;
            double green = 0.0;
            double blue = 0.0;

            if (scaledTemp <= 66)
            {
                red = 255;

                green = scaledTemp;
                green = (99.4708025861 * Math.Log(green)) - 161.1195681661;

                if (scaledTemp <= 19)
                {
                    blue = 0;
                }
                else
                {
                    blue = scaledTemp - 10;
                    blue = (138.5177312231 * Math.Log(blue)) - 305.0447927307;
                }
            }
            else
            {
                red = scaledTemp - 60;
                red = 329.698727446 * Math.Pow(red, -0.1332047592);

                green = scaledTemp - 60;
                green = 288.1221695283 * Math.Pow(green, -0.0755148492);

                blue = 255;
            }

            this.globalTemperatureColor.r = (float)(Math.Min(Math.Max(red, 0.0), 255.0) / 255.0);
            this.globalTemperatureColor.g = (float)(Math.Min(Math.Max(green, 0.0), 255.0) / 255.0);
            this.globalTemperatureColor.b = (float)(Math.Min(Math.Max(blue, 0.0), 255.0) / 255.0);
            this.globalTemperatureColor.a = 1.0f;
        }
#endif
    }
}
