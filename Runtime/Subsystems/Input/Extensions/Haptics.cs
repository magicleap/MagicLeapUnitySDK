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
using System.Runtime.InteropServices;
using MagicLeap.OpenXR.LegacySupport;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;

namespace UnityEngine.XR.MagicLeap
{
    public static partial class InputSubsystem
    {
        public static partial class Extensions
        {
            public static class Haptics
            {
                /// <summary>
                /// Custom haptics pattern types.
                /// </summary>
                public enum Type
                {
                    /// <summary>
                    /// Pattern is a buzz command.
                    /// </summary>
                    Buzz = 1,

                    /// <summary>
                    /// Pattern is a pre-defined pattern.
                    /// </summary>
                    PreDefined
                }

                /// <summary>
                /// Starts a buzz pattern.
                /// </summary>
                public static MLResult StartBuzz(ushort startHz, ushort endHz, uint durationMs, byte amplitude)
                {
                    if(MLDevice.IsOpenXRLoaderActive())
                    {
                        return MLInput.StartBuzz(startHz, endHz, durationMs, amplitude);
                    }
                    return Buzz.Create(startHz, endHz, durationMs, amplitude).StartHaptics();
                }

                /// <summary>
                /// Starts a predefined pattern.
                /// </summary>
                public static MLResult StartPreDefined(PreDefined.Type type)
                {
                    if(MLDevice.IsOpenXRLoaderActive())
                    {
                        return MLInput.StartPredefined((MLInput.PreDefinedPatternType)type);
                    }
                    return PreDefined.Create(type).StartHaptics();
                }

                /// <summary>
                /// Stops playing the current haptic pattern.
                /// </summary>
                public static MLResult Stop()
                {
                    if(MLDevice.IsOpenXRLoaderActive())
                    {
                        return MLInput.Stop();
                    }
                    var resultCode = MagicLeapXrProviderNativeBindings.StopHaptics();
                    return MLResult.Create(resultCode);
                }

                /// <summary>
                /// Shared haptics interface.
                /// </summary>
                public interface IHapticsCommand
                {
                    public MLResult StartHaptics();
                }

                /// <summary>
                /// A structure containing information about the Buzz command to be played.
                /// </summary>
                public readonly struct Buzz : IHapticsCommand
                {
                    public static Buzz Create(ushort startHz, ushort endHz, uint durationMs, byte amplitude) => new Buzz(startHz, endHz, durationMs, amplitude);

                    public MLResult StartHaptics()
                    {
                        var nativeCommandStruct = new NativeBindings.MLInputBuzzCommand(this);
                        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(nativeCommandStruct));
                        Marshal.StructureToPtr(nativeCommandStruct, ptr, false);

                        var resultCode = MagicLeapXrProviderNativeBindings.StartHapticsPattern((uint)(int)nativeCommandStruct.typeStatic, ptr);

                        Marshal.FreeHGlobal(ptr);
                        return MLResult.Create(resultCode);
                    }

                    private Buzz(ushort startHz, ushort endHz, uint durationMs, byte amplitude)
                    {
                        StartHz = startHz;
                        EndHz = endHz;
                        DurationMs = durationMs;
                        Amplitude = amplitude;
                    }

                    /// <summary>
                    /// Start frequency of the buzz command (0 - 1250).
                    /// </summary>
                    public readonly ushort StartHz;

                    /// <summary>
                    /// End frequency of the buzz command (0 - 1250).
                    /// </summary>
                    public readonly ushort EndHz;

                    /// <summary>
                    /// Duration of the buzz command in milliseconds (ms).
                    /// </summary>
                    public readonly uint DurationMs;

                    /// <summary>
                    /// amplitude of the buzz command, as a percentage (0 - 100).
                    /// </summary>
                    public readonly byte Amplitude;
                }

                /// <summary>
                /// A structure containing information about the Pre-Defined pattern to be played.
                /// </summary>
                public readonly struct PreDefined : IHapticsCommand
                {
                    /// <summary>
                    /// Predefined haptics pattern types.
                    /// </summary>
                    public enum Type
                    {
                        /// <summary>
                        /// Pre-Defined pattern used for VKB hover.
                        /// </summary>
                        A,

                        /// <summary>
                        /// Pre-Defined pattern used for VKB select.
                        /// </summary>
                        B,

                        /// <summary>
                        /// Pre-Defined pattern used for homemenu hover.
                        /// </summary>
                        C
                    }

                    public static PreDefined Create(Type patternType) => new PreDefined(patternType);

                    public MLResult StartHaptics()
                    {
                        var nativeCommandStruct = new NativeBindings.MLInputPreDefinedPattern(this);
                        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(nativeCommandStruct));
                        Marshal.StructureToPtr(nativeCommandStruct, ptr, false);

                        var resultCode = MagicLeapXrProviderNativeBindings.StartHapticsPattern((uint)(int)nativeCommandStruct.typeStatic, ptr);

                        Marshal.FreeHGlobal(ptr);
                        return MLResult.Create(resultCode);
                    }

                    private PreDefined(Type patternType) => PatternType = patternType;

                    /// <summary>
                    /// Pre-defined pattern to be played.
                    /// </summary>
                    public readonly Type PatternType;
                }

                /// <summary>
                /// A custom haptics pattern can be played by combining Buzz haptic command and/or a pre-defined pattern,
                /// this object will hold a list of the haptics to be played with their specified durations.
                /// </summary>
                public class CustomPattern : IHapticsCommand
                {
                    private List<NativeBindings.MLInputCustomHaptics> customHaptics = new List<NativeBindings.MLInputCustomHaptics>();

                    public MLResult StartHaptics()
                    {
                        if(MLDevice.IsOpenXRLoaderActive())
                        {
                            return MLInput.StartCustomPattern(customHaptics);
                        }

                        var nativeCommandStruct = new NativeBindings.MLInputCustomHapticsInfo(customHaptics);
                        IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(nativeCommandStruct));
                        Marshal.StructureToPtr(nativeCommandStruct, ptr, false);

                        var resultCode = MagicLeapXrProviderNativeBindings.StartHapticsPattern((uint)(int)nativeCommandStruct.typeStatic, ptr);

                        nativeCommandStruct.Dispose();
                        Marshal.FreeHGlobal(ptr);
                        return MLResult.Create(resultCode);
                    }

                    public void Add(in Buzz buzz) => customHaptics.Add(new NativeBindings.MLInputCustomHaptics(in buzz));
                    public void Add(in PreDefined preDefined, uint durationMs) => customHaptics.Add(new NativeBindings.MLInputCustomHaptics(in preDefined, durationMs));

                    public bool Remove(in Buzz buzz) => customHaptics.Remove(new NativeBindings.MLInputCustomHaptics(in buzz));
                    public bool Remove(in PreDefined preDefined, uint durationMs) => customHaptics.Remove(new NativeBindings.MLInputCustomHaptics(in preDefined, durationMs));

                }

                internal static class NativeBindings
                {
                    /// <summary>
                    /// A structure containing information about the Buzz command to be played.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public readonly struct MLInputBuzzCommand : IInputDeviceCommandInfo
                    {
                        private static FourCC commandType { get { return new FourCC('M', 'L', 'B', 'Z'); } }

                        public FourCC typeStatic => commandType;

                        public MLInputBuzzCommand(in Buzz buzzCommand)
                        {
                            Version = 1;
                            StartHz = buzzCommand.StartHz;
                            EndHz = buzzCommand.EndHz;
                            DurationMs = buzzCommand.DurationMs;
                            Amplitude = buzzCommand.Amplitude;
                        }

                        /// <summary>
                        /// Version of this structure.
                        /// </summary>
                        public readonly uint Version;

                        /// <summary>
                        /// Start frequency of the buzz command (0 - 1250).
                        /// </summary>
                        public readonly ushort StartHz;

                        /// <summary>
                        /// End frequency of the buzz command (0 - 1250).
                        /// </summary>
                        public readonly ushort EndHz;

                        /// <summary>
                        /// Duration of the buzz command in milliseconds (ms).
                        /// </summary>
                        public readonly uint DurationMs;

                        /// <summary>
                        /// amplitude of the buzz command, as a percentage (0 - 100).
                        /// </summary>
                        public readonly byte Amplitude;

                    }

                    /// <summary>
                    /// A structure containing information about the Pre-Defined pattern to be played.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public readonly struct MLInputPreDefinedPattern : IInputDeviceCommandInfo
                    {
                        private static FourCC commandType { get { return new FourCC('M', 'L', 'P', 'D'); } }

                        public FourCC typeStatic => commandType;

                        public MLInputPreDefinedPattern(in PreDefined preDefinedPattern)
                        {
                            Version = 1;
                            Pattern = (uint)preDefinedPattern.PatternType;
                        }

                        /// <summary>
                        /// Version of this structure.
                        /// </summary>
                        public readonly uint Version;

                        /// <summary>
                        /// Pre-defined pattern to be played.
                        /// </summary>
                        public readonly uint Pattern;
                    }

                    /// <summary>
                    /// A structure containing information about the Buzz command and/or pre-defined pattern to be played.
                    /// A custom haptics pattern can be played by combining Buzz haptic command and/or a pre-defined pattern,
                    /// this structure will hold info of a pattern to be added to the custom haptics array.
                    /// </summary>
                    [StructLayout(LayoutKind.Explicit)]
                    public readonly struct MLInputCustomHaptics
                    {
                        public MLInputCustomHaptics(in Buzz buzzHaptics)
                        {
                            Type = Type.Buzz;
                            PreDefined = default;
                            Buzz = new MLInputBuzzCommand(in buzzHaptics);
                            DurationMs = buzzHaptics.DurationMs;
                        }

                        public MLInputCustomHaptics(in PreDefined preDefinedPatternHaptics, uint durationMs)
                        {
                            Type = Type.PreDefined;
                            Buzz = default;
                            PreDefined = new MLInputPreDefinedPattern(in preDefinedPatternHaptics);
                            DurationMs = durationMs;
                        }

                        /// <summary>
                        /// Type of the pattern to be set.
                        /// </summary>
                        [FieldOffset(0)]
                        public readonly Type Type;

                        /// <summary>
                        /// Potential buzz pattern to use.
                        /// </summary>
                        [FieldOffset(4)]
                        public readonly MLInputBuzzCommand Buzz;

                        /// <summary>
                        /// Potential predefined pattern to use.
                        /// </summary>
                        [FieldOffset(4)]
                        public readonly MLInputPreDefinedPattern PreDefined;

                        /// <summary>
                        /// Duration of the pattern in millisecond (ms)
                        /// This field will set the time for which a pattern is played:
                        ///     -If the inherited duration of a pre-defined pattern is greater, then it will be cut short
                        /// and the next pattern will start playing.
                        ///     -If the inherited duration of a pre-defined pattern is smaller, then it will not repeat itself
                        /// and will wait for the #duration to complete before starting to play the next pattern.
                        ///     -Buzz command's duration will supersede the #duration. Above caveat is true in case buzz command'
                        /// </summary>
                        [FieldOffset(20)]
                        public readonly uint DurationMs;
                    }

                    /// <summary>
                    ///  A structure containing a list of Buzz command and/or pre-defined patterns to be played.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public readonly struct MLInputCustomHapticsInfo : IInputDeviceCommandInfo, IDisposable
                    {
                        private static FourCC commandType { get { return new FourCC('M', 'L', 'C', 'I'); } }

                        public FourCC typeStatic => commandType;

                        public MLInputCustomHapticsInfo(List<NativeBindings.MLInputCustomHaptics> customHaptics)
                        {
                            Version = 1;

                            CustomHapticsArrayPtr = Marshal.AllocHGlobal(Marshal.SizeOf<MLInputCustomHaptics>() * customHaptics.Count);

                            IntPtr walkPtr = CustomHapticsArrayPtr;
                            for (int i = 0; i < customHaptics.Count; ++i)
                            {
                                var custom = customHaptics[i];
                                Marshal.StructureToPtr(custom, walkPtr, true);
                                walkPtr = new IntPtr(walkPtr.ToInt64()) + Marshal.SizeOf<MLInputCustomHaptics>();
                            }

                            Size = (uint)customHaptics.Count;
                        }


                        public void Dispose() => Marshal.FreeHGlobal(CustomHapticsArrayPtr);

                        /// <summary>
                        /// Version of this structure.
                        /// </summary>
                        public readonly uint Version;

                        /// <summary>
                        /// An array containing the list of buzz command and/or pre-defined patterns.
                        /// </summary>
                        public readonly IntPtr CustomHapticsArrayPtr;

                        /// <summary>
                        /// Size of the array.
                        /// </summary>
                        public readonly uint Size;
                    }
                }
            }

        }
    }
}
