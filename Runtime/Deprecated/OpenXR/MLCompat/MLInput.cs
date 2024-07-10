using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.MLInput")]
    internal class MLInput
    {
        public enum PreDefinedPatternType { A, B, C }

        public enum CustomHapticsType { None, Buzz, Predefined }

        private static Dictionary<NativeBindings.MLInputCustomHapticsInfo, uint> createdPatternIds = new Dictionary<NativeBindings.MLInputCustomHapticsInfo, uint>();

        private static ulong handle = MagicLeapNativeBindings.InvalidHandle;

        private static void CreateHandle()
        {
            if(handle == MagicLeapNativeBindings.InvalidHandle)
                MLResult.DidNativeCallSucceed(NativeBindings.MLInputCreate(out handle));
        }

        public static MLResult StartBuzz(ushort startHz, ushort endHz, uint durationMs, byte amplitude)
        {
            CreateHandle();
            var result = NativeBindings.MLInputStopControllerFeedback(handle, 0);
            if(MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLInputStopControllerFeedback)))
            {
                var command = NativeBindings.MLInputBuzzCommand.Init();
                command.StartHz = startHz;
                command.EndHz = endHz;
                command.DurationMs = durationMs;
                command.Amplitude = amplitude;
                result = NativeBindings.MLInputStartControllerFeedbackBuzzCommand(handle, 0, in command);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLInputStartControllerFeedbackBuzzCommand));
            }
            return MLResult.Create(result);
        }

        public static MLResult StartPredefined(PreDefinedPatternType patternType)
        {
            CreateHandle();
            var result = NativeBindings.MLInputStopControllerFeedback(handle, 0);
            if (MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLInputStopControllerFeedback)))
            {
                var pattern = NativeBindings.MLInputPreDefinedPattern.Init();
                pattern.Type = patternType;
                result = NativeBindings.MLInputStartControllerFeedbackPreDefinedPattern(handle, 0, ref pattern);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLInputStartControllerFeedbackPreDefinedPattern));
            }
            return MLResult.Create(result);
        }

        public static MLResult StartCustomPattern(List<InputSubsystem.Extensions.Haptics.NativeBindings.MLInputCustomHaptics> hapticsList)
        {
            CreateHandle();
            var customHaptics = new List<NativeBindings.MLInputCustomHaptics>();
            foreach(var h in hapticsList)
            {
                NativeBindings.MLInputCustomHaptics hap = new()
                {
                    Type = (CustomHapticsType)h.Type
                };
                if (h.Type == InputSubsystem.Extensions.Haptics.Type.Buzz)
                {
                    hap.Buzz = NativeBindings.MLInputBuzzCommand.Init();
                    hap.Buzz.StartHz = h.Buzz.StartHz;
                    hap.Buzz.EndHz = h.Buzz.EndHz;
                    hap.Buzz.DurationMs = h.Buzz.DurationMs;
                    hap.Buzz.Amplitude = h.Buzz.Amplitude;
                    hap.DurationMs = h.Buzz.DurationMs;
                }
                else
                {
                    hap.PreDefined = NativeBindings.MLInputPreDefinedPattern.Init();
                    hap.PreDefined.Type = (PreDefinedPatternType)h.PreDefined.Pattern;
                    hap.DurationMs = h.DurationMs;
                }
                customHaptics.Add(hap);
            }

            var info = new NativeBindings.MLInputCustomHapticsInfo((uint)customHaptics.Count);
            IntPtr itr = info.CustomHaptics;
            Debug.Log($"\n\n [bas] filling haptics info array with {info.Size} commands\n");
            for(int i = 0; i < customHaptics.Count; ++i)
            {
                var haptic = customHaptics[i];
                Marshal.StructureToPtr(haptic, itr, true);
                itr = new IntPtr(itr.ToInt64()) + Marshal.SizeOf<NativeBindings.MLInputCustomHaptics>();
            }

            uint id = 0;
            if(!createdPatternIds.ContainsKey(info))
            {
                if(NativeBindings.MLInputCreateCustomHapticsPattern(handle, ref info, ref id) == MLResult.Code.Ok)
                    createdPatternIds.Add(info, id);
            }
            else
            {
                id = createdPatternIds[info];
            }

            var result = NativeBindings.MLInputStartControllerFeedbackCustomHapticsPattern(handle, 0, id);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLInputStartControllerFeedbackCustomHapticsPattern));
            return MLResult.Create(result);
        }

        public static MLResult Stop()
        {
            var result = NativeBindings.MLInputStopControllerFeedback(handle, 0);
            MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLInputStopControllerFeedback));
            return MLResult.Create(result);
        }

        [System.Obsolete("Type has been relocated to new namespace. Update reference to MagicLeap.OpenXR.Features.NativeBindings")]

        internal class NativeBindings : MagicLeapNativeBindings
        {
            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLInputCreate(out ulong handle);

            #region Haptics
            public struct MLInputPreDefinedPattern // size 8
            {
                public uint Version;
                public PreDefinedPatternType Type;
                public static MLInputPreDefinedPattern Init() => new()
                {
                    Version = 1,
                    Type = PreDefinedPatternType.C
                };
            }

            [StructLayout(LayoutKind.Sequential, Size =  13)]
            public struct MLInputBuzzCommand // size 13
            {
                public uint Version; // 4
                public ushort StartHz; //2
                public ushort EndHz; // 2
                public uint DurationMs; //4
                public byte Amplitude; // 1
                public static MLInputBuzzCommand Init() => new()
                {
                    Version = 1,
                    StartHz = 200,
                    EndHz = 800,
                    DurationMs = 1000,
                    Amplitude = 20
                };
            }

            [StructLayout(LayoutKind.Explicit, Size = 21)]
            public struct MLInputCustomHaptics
            {
                [FieldOffset(0)]
                public CustomHapticsType Type; // 4
                [FieldOffset(4)]
                public MLInputBuzzCommand Buzz; // 13
                [FieldOffset(4)]
                public MLInputPreDefinedPattern PreDefined; // 8
                [FieldOffset(17)]
                public uint DurationMs; // 4
            }

            public struct MLInputCustomHapticsInfo : IDisposable
            {
                public uint Version;
                public IntPtr CustomHaptics;
                public uint Size;

                public MLInputCustomHapticsInfo(uint size)
                {
                    Version = 1;
                    CustomHaptics = Marshal.AllocHGlobal(Marshal.SizeOf<MLInputCustomHaptics>() * (int)size);
                    Size = size;
                }

                public void Dispose() => Marshal.FreeHGlobal(CustomHaptics);
            }

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLInputStartControllerFeedbackPreDefinedPattern(ulong handle, byte controllerId, ref MLInputPreDefinedPattern pattern);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLInputStartControllerFeedbackBuzzCommand(ulong handle, byte controllerId, in MLInputBuzzCommand command);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLInputCreateCustomHapticsPattern(ulong handle, ref MLInputCustomHapticsInfo info, ref uint patternId);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLInputStartControllerFeedbackCustomHapticsPattern(ulong handle, byte controllerId, uint patternId);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLInputDeleteCustomHapticsPattern(ulong handle, uint patternId);

            [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLInputStopControllerFeedback(ulong handle, byte controllerId);
            #endregion
        }
    }
}
