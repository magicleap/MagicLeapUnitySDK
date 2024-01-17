// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022-2023 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Native;
    public sealed partial class MLEyeCamera
    {
        private NativeBindings.MLEyeCameraSettings settings;

        protected override MLResult.Code StartAPI()
        {
            Instance.settings = new NativeBindings.MLEyeCameraSettings(1, MLEyeCameraIdentifier.All);

            var resultCode = NativeBindings.MLEyeCameraConnect(ref settings, ref Instance.Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLEyeCameraConnect));
            return resultCode;
        }

        protected override MLResult.Code StopAPI()
        {
            var resultCode = NativeBindings.MLEyeCameraDisconnect(Instance.Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLEyeCameraDisconnect));
            return resultCode;
        }

        private MLResult InternalMLEyeCameraGetLatestCameraData(out EyeCameraData outData, UInt64 timeoutMS)
        {
            NativeBindings.MLEyeCameraData eyeCameraData = new NativeBindings.MLEyeCameraData(1);

            var resultCode = NativeBindings.MLEyeCameraGetLatestCameraData(Instance.Handle, timeoutMS, ref eyeCameraData);

            outData = new EyeCameraData(eyeCameraData);

            if ((resultCode == MLResult.Code.Timeout && timeoutMS == 0) || !MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLEyeCameraGetLatestCameraData)))
                return MLResult.Create(resultCode);

            resultCode = NativeBindings.MLEyeCameraReleaseCameraData(Instance.Handle, ref eyeCameraData);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLEyeCameraReleaseCameraData));

            return MLResult.Create(resultCode);
        }

        private MLResult InternalMLEyeCameraUpdateSettings(MLEyeCameraIdentifier cameras)
        {
            Instance.settings = new NativeBindings.MLEyeCameraSettings(1, cameras);

            var resultCode = NativeBindings.MLEyeCameraUpdateSettings(Instance.Handle, ref Instance.settings);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLEyeCameraUpdateSettings));

            return MLResult.Create(resultCode);
        }
    }
}
