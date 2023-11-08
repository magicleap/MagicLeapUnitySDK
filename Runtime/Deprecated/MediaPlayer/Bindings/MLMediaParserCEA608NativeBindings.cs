// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.InteropServices;
    using UnityEngine;
    using UnityEngine.Rendering;
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    public partial class MLMedia
    {
        /// <summary>
        /// Media player script that allows playback of a streaming video (either from file or web URL)
        /// This script will update the main texture parameter of the Renderer attached as a sibling
        /// with the video frame from playback. Audio is also handled through this class and will
        /// playback audio from the file.
        /// </summary>
        public partial class ParserCEA608
        {
            public class NativeBindings : MagicLeapNativeBindings
            {
                private const int MLCea608_TS = 0x00A0;

                public delegate void OnDisplayChangedDelegate(ulong parserHande, ref CaptionSegmentEx segment, IntPtr data);

                [StructLayout(LayoutKind.Sequential)]
                public struct CaptionStyleColor
                {
                    public uint style;
                    public uint color;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct CaptionPAC
                {
                    public CaptionStyleColor styleColor;
                    public uint row;
                    public uint col;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct CaptionLineEx
                {
                    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2 * (int)CaptionDimension.MaxColsPlus2)]
                    public ushort[] displayChars;

                    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CaptionDimension.MaxColsPlus2)]
                    public IntPtr[] midRowStyles;

                    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CaptionDimension.MaxColsPlus2)]
                    public IntPtr[] pacStyles;
                }

                [StructLayout(LayoutKind.Sequential)]
                public struct CaptionSegmentEx
                {
                    public uint version;

                    [MarshalAs(UnmanagedType.ByValArray, SizeConst = (int)CaptionDimension.MaxRowsPlus2)]
                    public IntPtr[] lines;
                }

                [DllImport(MLMediaCEA608DLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaCCParserCreate(out ulong parserHandle);

                [DllImport(MLMediaCEA608DLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaCCParserGetDisplayableEx(ulong parserHandle, IntPtr subtitleData, int subtitleDataSize, ref IntPtr captionSegment);

                [DllImport(MLMediaCEA608DLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaCCParserDestroy(ulong parserHandle);

                [DllImport(MLMediaCEA608DLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaCCParserSetDisplayChangedCallback(ulong parserHandle, OnDisplayChangedDelegate calback, IntPtr data);

                [DllImport(MLMediaCEA608DLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaCCParserParse(ulong parserHandle, IntPtr subtitleData, uint subtitleDataSize);

                /// <summary>
                /// Callback for the OnDisplayChanged event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnDisplayChangedDelegate))]
                public static void OnDisplayChanged(ulong parserHande, ref CaptionSegmentEx segment, IntPtr data)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(data);
                    ParserCEA608 parser = gcHandle.Target as ParserCEA608;
                    if (parser == null)
                    {
                        return;
                    }

                    parser.lines.Clear();
                    parser.stringBuilder.Clear();

                    foreach (IntPtr lineptr in segment.lines)
                    {
                        if (lineptr == IntPtr.Zero)
                            continue;

                        CaptionLineEx lineNative = Marshal.PtrToStructure<CaptionLineEx>(lineptr);
                        CaptionLine line = new CaptionLine();

                        line.text = new string(Array.ConvertAll(lineNative.displayChars, Convert.ToChar));
                        parser.stringBuilder.Append(line.text);

                        line.midRowStyles = new CaptionStyleColor[lineNative.midRowStyles.Length];
                        for (int i = 0; i < line.midRowStyles.Length; ++i)
                        {
                            IntPtr midRowStylePtr = lineNative.midRowStyles[i];
                            line.midRowStyles[i] = Marshal.PtrToStructure<CaptionStyleColor>(midRowStylePtr);
                        }

                        line.pacStyles = new CaptionPAC[lineNative.pacStyles.Length];
                        for (int i = 0; i < line.pacStyles.Length; ++i)
                        {
                            IntPtr pacStylePtr = lineNative.pacStyles[i];
                            line.pacStyles[i] = Marshal.PtrToStructure<CaptionPAC>(pacStylePtr);
                        }

                        parser.lines.Add(line);
                    }

                    string final = parser.stringBuilder.ToString();
                    MLThreadDispatch.Call(final, parser.OnText);
                    MLThreadDispatch.Call(parser.lines.ToArray(), parser.OnCaptionLines);
                }
            }
        }
    }
}
