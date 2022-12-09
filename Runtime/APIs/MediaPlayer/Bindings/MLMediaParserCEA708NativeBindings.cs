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
    using System.Runtime.InteropServices;
    using UnityEngine;
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
        public partial class ParserCEA708
        {
            private class NativeBindings : MagicLeapNativeBindings
            {
                public delegate void OnEmitEventDelegate(ulong parserHandle, ref Cea708CaptionEvent captionEvent, IntPtr data);
                private const int MLCea708CaptionWindowsMax = 8;
                private const int MLCea708CaptionEmitCommandBufferMaxSize = 20;

                /// <summary>
                /// CEA708 Caption Color.
                /// <c>MLCea708CaptionColor</c> from <c>ml_media_cea708_caption.h</c>.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct Cea708CaptionColor
                {
                    /// <summary>
                    /// Opacity setting.
                    /// </summary>
                    public Cea708CaptionColorOpacity Opacity;

                    /// <summary>
                    /// Red component.
                    /// </summary>
                    public int Red;

                    /// <summary>
                    /// Green component.
                    /// </summary>
                    public int Green;

                    /// <summary>
                    /// Blue component.
                    /// </summary>
                    public int Blue;
                }

                /// <summary>
                /// CEA708 Caption Pen Attributes.
                /// <c>MLCea708CaptionPenAttr</c> from <c>ml_media_cea708_caption.h</c>.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct Cea708CaptionPenAttr
                {
                    /// <summary>
                    /// Pen size.
                    /// </summary>
                    public Cea708CaptionPenSize PenSize;

                    /// <summary>
                    /// Pen offset.
                    /// </summary>
                    public Cea708CaptionPenOffset PenOffset;

                    /// <summary>
                    /// Text tag.
                    /// </summary>
                    public int TextTag;

                    /// <summary>
                    /// Font tag.
                    /// </summary>
                    public int FontTag;

                    /// <summary>
                    /// Edge type.
                    /// </summary>
                    public int EdgeType;

                    /// <summary>
                    /// Underline setting.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool Underline;

                    /// <summary>
                    /// Italic setting.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool Italic;
                }

                /// <summary>
                /// CEA708 Caption Pen Color.
                /// <c>MLCea708CaptionPenColor</c> from <c>ml_media_cea708_caption.h</c>.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct Cea708CaptionPenColor
                {
                    /// <summary>
                    /// Foreground color.
                    /// </summary>
                    public Cea708CaptionColor ForegroundColor;

                    /// <summary>
                    /// Background color.
                    /// </summary>
                    public Cea708CaptionColor BackgroundColor;

                    /// <summary>
                    /// Edge color.
                    /// </summary>
                    public Cea708CaptionColor EdgeColor;
                }

                /// <summary>
                /// CEA708 Caption Pen Location.
                /// <c>MLCea708CaptionPenLocation</c> from <c>ml_media_cea708_caption.h</c>.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct Cea708CaptionPenLocation
                {
                    /// <summary>
                    /// Pen row.
                    /// </summary>
                    public int Row;

                    /// <summary>
                    /// Pen column.
                    /// </summary>
                    public int Column;
                }

                /// <summary>
                /// CEA708 Caption Window Attributes.
                /// <c>MLCea708CaptionWindowAttr</c> from <c>ml_media_cea708_caption.h</c>.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct Cea708CaptionWindowAttr
                {
                    /// <summary>
                    /// Window fill color.
                    /// </summary>
                    public Cea708CaptionColor FillColor;

                    /// <summary>
                    /// Window border color.
                    /// </summary>
                    public Cea708CaptionColor BorderColor;

                    /// <summary>
                    /// Window border type.
                    /// </summary>
                    public int BorderType;

                    /// <summary>
                    /// Window word wrap setting.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool WordWrap;

                    /// <summary>
                    /// Window print direction.
                    /// </summary>
                    public int PrintDirection;

                    /// <summary>
                    /// Window scroll direction.
                    /// </summary>
                    public int ScrollDirection;

                    /// <summary>
                    /// Window justification setting.
                    /// </summary>
                    public int Justify;

                    /// <summary>
                    /// Window effect direction.
                    /// </summary>
                    public int EffectDirection;

                    /// <summary>
                    /// Window effect speed.
                    /// </summary>
                    public int EffectSpeed;

                    /// <summary>
                    /// Window display effect.
                    /// </summary>
                    public int DisplayEffect;
                }

                /// <summary>
                /// CEA708 Caption Window.
                /// <c>MLCea708CaptionWindow</c> from <c>ml_media_cea708_caption.h</c>.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct Cea708CaptionWindow
                {
                    /// <summary>
                    /// Window ID.
                    /// </summary>
                    public int ID;

                    /// <summary>
                    /// Window visible setting.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool Visible;

                    /// <summary>
                    /// Window row lock setting.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool RowLock;

                    /// <summary>
                    /// Window column lock setting.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool ColumnLock;

                    /// <summary>
                    /// Window Priority.
                    /// </summary>
                    public int Priority;

                    /// <summary>
                    /// Window relative positioning setting.
                    /// </summary>
                    [MarshalAs(UnmanagedType.I1)]
                    public bool RelativePositioning;

                    /// <summary>
                    /// Window anchor vertical.
                    /// </summary>
                    public int AnchorVertical;

                    /// <summary>
                    /// Window anchor horizontal.
                    /// </summary>
                    public int AnchorHorizontal;

                    /// <summary>
                    /// Window anchor ID.
                    /// </summary>
                    public int AnchorID;

                    /// <summary>
                    /// Window row count.
                    /// </summary>
                    public int RowCount;

                    /// <summary>
                    /// Window column count.
                    /// </summary>
                    public int ColumnCount;

                    /// <summary>
                    /// Window pen style.
                    /// </summary>
                    public int PenStyle;

                    /// <summary>
                    /// Window style.
                    /// </summary>
                    public int WindowStyle;
                }

                /// <summary>
                /// Internal CEA708 Caption Event.
                /// <c>MLCea708CaptionEvent</c> from <c>ml_media_cea708_caption.h</c>.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct Cea708CaptionEventInternal
                {
                    /// <summary>
                    /// Emitted CEA708 caption event type.
                    /// </summary>
                    public Cea708CaptionEmitCommand Type;

                    /// <summary>
                    /// Emitted CEA708 caption event specific data, based on the event type.
                    /// If the type is <c>Cea708CaptionEmitCommand.Buffer</c>, Object will point to a null terminated string of maximum size <c>MLCea708CaptionEmitCommandBufferMaxSize</c> bytes.
                    /// If the type is <c>Cea708CaptionEmitCommand.Control</c>, Object will point to one byte character.
                    /// If the type is <c>Cea708CaptionEmitCommand.CWX</c>, Object will point to an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.CLW</c>, Object will point to an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.DSW</c>, Object will point to an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.HDW</c>, Object will point to an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.TGW</c>, Object will point to an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.DLW</c>, Object will point to an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.DLY</c>, Object will point to an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.DLC</c>, Object will be NULL.
                    /// If the type is <c>Cea708CaptionEmitCommand.RST</c>, Object will be NULL.
                    /// If the type is <c>Cea708CaptionEmitCommand.SPA</c>, Object will point to <c>Cea708CaptionPenAttr</c>.
                    /// If the type is <c>Cea708CaptionEmitCommand.SPC</c>, Object will point to <c>Cea708CaptionPenColor</c>.
                    /// If the type is <c>Cea708CaptionEmitCommand.SPL</c>, Object will point to <c>Cea708CaptionPenLocation</c>.
                    /// If the type is <c>Cea708CaptionEmitCommand.SWA</c>, Object will point to <c>Cea708CaptionWindowAttr</c>.
                    /// If the type is <c>Cea708CaptionEmitCommand.DFX</c>, Object will point to <c>Cea708CaptionWindow</c>.
                    /// </summary>
                    public IntPtr Object;
                }

                /// <summary>
                /// CEA708 Caption Event.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct Cea708CaptionEvent
                {
                    /// <summary>
                    /// Emitted CEA708 caption event type.
                    /// </summary>
                    public Cea708CaptionEmitCommand Type;

                    /// <summary>
                    /// Emitted CEA708 caption event specific data, based on the event type.
                    /// If the type is <c>Cea708CaptionEmitCommand.Buffer</c>, Object will be a string.
                    /// If the type is <c>Cea708CaptionEmitCommand.Control</c>, Object will be a byte.
                    /// If the type is <c>Cea708CaptionEmitCommand.CWX</c>, Object will be an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.CLW</c>, Object will be an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.DSW</c>, Object will be an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.HDW</c>, Object will be an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.TGW</c>, Object will be an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.DLW</c>, Object will be an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.DLY</c>, Object will be an integer.
                    /// If the type is <c>Cea708CaptionEmitCommand.DLC</c>, Object will be null.
                    /// If the type is <c>Cea708CaptionEmitCommand.RST</c>, Object will be null.
                    /// If the type is <c>Cea708CaptionEmitCommand.SPA</c>, Object will be a <c>Cea708CaptionPenAttr</c>.
                    /// If the type is <c>Cea708CaptionEmitCommand.SPC</c>, Object will be a <c>Cea708CaptionPenColor</c>.
                    /// If the type is <c>Cea708CaptionEmitCommand.SPL</c>, Object will be a <c>Cea708CaptionPenLocation</c>.
                    /// If the type is <c>Cea708CaptionEmitCommand.SWA</c>, Object will be a <c>Cea708CaptionWindowAttr</c>.
                    /// If the type is <c>Cea708CaptionEmitCommand.DFX</c>, Object will be a <c>Cea708CaptionWindow</c>.
                    /// </summary>
                    public IntPtr Data;
                }

                [DllImport(MLMediaCEA708DLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaCea708ParserCreate(out ulong parserHandle);

                [DllImport(MLMediaCEA708DLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaCea708ParserDestroy(ulong parserHandle);

                [DllImport(MLMediaCEA708DLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaCea708ParserSetEmitEventCallback(ulong parserHandle, OnEmitEventDelegate calback, IntPtr data);

                [DllImport(MLMediaCEA708DLL, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaCea708ParserParse(ulong parserHandle, IntPtr subtitleData, uint subtitleDataSize);

                /// <summary>
                /// Callback for the OnDisplayChanged event.
                /// </summary>
                [AOT.MonoPInvokeCallback(typeof(NativeBindings.OnEmitEventDelegate))]
                public static void OnEmitEvent(ulong parserHandle, ref Cea708CaptionEvent captionEvent, IntPtr data)
                {
                    GCHandle gcHandle = GCHandle.FromIntPtr(data);
                    ParserCEA708 parser708 = gcHandle.Target as ParserCEA708;
                    if (parser708 == null)
                    {
                        return;
                    }

                    object obj = null;

                    switch (captionEvent.Type)
                    {
                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.Buffer:
                            obj = MLConvert.DecodeUTF8(captionEvent.Data, MLCea708CaptionEmitCommandBufferMaxSize);
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.Control:
                            obj = Marshal.ReadByte(captionEvent.Data);
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.CWX:
                            obj = Marshal.ReadInt32(captionEvent.Data);
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.CLW:
                            obj = Marshal.ReadInt32(captionEvent.Data);
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DSW:
                            obj = Marshal.ReadInt32(captionEvent.Data);
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.HDW:
                            obj = Marshal.ReadInt32(captionEvent.Data);
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.TGW:
                            obj = Marshal.ReadInt32(captionEvent.Data);
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DLW:
                            obj = Marshal.ReadInt32(captionEvent.Data);
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DLY:
                            obj = Marshal.ReadInt32(captionEvent.Data);
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DLC:
                            obj = null;
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.RST:
                            obj = null;
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.SPA:
                            obj = Marshal.PtrToStructure(captionEvent.Data, typeof(Cea708CaptionPenAttr));
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.SPC:
                            obj = Marshal.PtrToStructure(captionEvent.Data, typeof(Cea708CaptionPenColor));
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.SPL:
                            obj = Marshal.PtrToStructure(captionEvent.Data, typeof(Cea708CaptionPenLocation));
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.SWA:
                            obj = Marshal.PtrToStructure(captionEvent.Data, typeof(Cea708CaptionWindowAttr));
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DFX:
                            obj = Marshal.PtrToStructure(captionEvent.Data, typeof(Cea708CaptionWindow));
                            break;
                    }
                    MLThreadDispatch.Call(captionEvent.Type, obj, parser708.OnEmitEvent);
                    int winBitmap;
                    switch (captionEvent.Type)
                    {
                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.Buffer:
                            if (parser708.cea708WindowID == -1)
                            {
                                parser708.cea708Strings[0] += (string)obj;
                                parser708.subtitleText = parser708.cea708Strings[0];
                                MLThreadDispatch.Call(parser708.subtitleText, parser708.OnText);
                            }
                            else
                            {
                                if (parser708.cea708WindowID == 0)
                                {
                                    parser708.cea708Strings[0] += (string)obj;
                                    parser708.subtitleText = parser708.cea708Strings[0];
                                    MLThreadDispatch.Call(parser708.subtitleText, parser708.OnText);
                                }
                                else if (parser708.cea708WindowID == 1)
                                {
                                    parser708.cea708Strings[1] += (string)obj;
                                    parser708.subtitleText = parser708.cea708Strings[1];
                                    MLThreadDispatch.Call(parser708.subtitleText, parser708.OnText);
                                }
                            }

                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.Control:
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.CWX:
                            parser708.cea708WindowID = (int)obj;
                            if (parser708.cea708WindowID == 0)
                            {
                                parser708.cea708Strings[0] = string.Empty;
                            }
                            else if (parser708.cea708WindowID == 1)
                            {
                                parser708.cea708Strings[1] = string.Empty;
                            }
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.CLW:
                            winBitmap = (int)obj;
                            if (parser708.cea708WindowID == -1)
                            {
                                parser708.cea708Strings[0] = string.Empty;
                            }
                            for (int i = 0; i < MLCea708CaptionWindowsMax; i++)
                            {
                                if ((winBitmap & (1 << i)) != 0)
                                {
                                    if (i == 0 || i == 1)
                                    {
                                        parser708.cea708Strings[i] = string.Empty;
                                    }
                                }
                            }
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DSW:
                            winBitmap = (int)obj;
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.HDW:
                            winBitmap = (int)obj;
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.TGW:
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DLW:
                            winBitmap = (int)obj;
                            for (int i = 0; i < MLCea708CaptionWindowsMax; i++)
                            {
                                if (((winBitmap & (1 << i)) == 0) && (i == 0 || i == 1))
                                {
                                    parser708.cea708Strings[i] = string.Empty;
                                }
                            }
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DLY:
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DLC:
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.RST:
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.SPA:
                            Cea708CaptionPenAttr pen = (Cea708CaptionPenAttr)obj;
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.SPC:
                            Cea708CaptionPenColor penColor = (Cea708CaptionPenColor)obj;
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.SPL:
                            Cea708CaptionPenLocation penLoc = (Cea708CaptionPenLocation)obj;
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.SWA:
                            Cea708CaptionWindowAttr winAttr = (Cea708CaptionWindowAttr)obj;
                            break;

                        case MLMedia.ParserCEA708.Cea708CaptionEmitCommand.DFX:
                            Cea708CaptionWindow win = (Cea708CaptionWindow)obj;
                            break;

                        default:
                            Debug.LogError("Error: ParserCEA708 unknown event type: " + captionEvent.Type);
                            break;
                    }
                }
            }
        }
    }
}
