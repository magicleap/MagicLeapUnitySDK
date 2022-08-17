// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLMediaParserCEA608.cs" company="Magic Leap, Inc">
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
    using System.Collections.Generic;
    using System.Text;
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    public partial class MLMedia
    {
        /// <summary>
        /// CEA608 Parser.
        /// </summary>
        public partial class ParserCEA608
        {
            public const string Mime = "text/cea-608";
            public delegate void OnCaptionLinesDelegate(CaptionLine[] lines);
            public delegate void OnTextDelegate(string text);

            public event OnCaptionLinesDelegate OnCaptionLines = delegate { };
            public event OnTextDelegate OnText = delegate { };

            private StringBuilder stringBuilder = new StringBuilder();
            private List<CaptionLine> lines = new List<CaptionLine>();

            public ParserCEA608()
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                MLResult.Code resultCode = NativeBindings.MLMediaCCParserCreate(out ulong parserHandle);
                MLResult.DidNativeCallSucceed(resultCode, "MLMediaCCParserCreate");
                this.Handle = parserHandle;
                this.gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
                IntPtr gcHandlePtr = GCHandle.ToIntPtr(this.gcHandle);
                resultCode = NativeBindings.MLMediaCCParserSetDisplayChangedCallback(this.Handle, NativeBindings.OnDisplayChanged, gcHandlePtr);
                MLResult.DidNativeCallSucceed(resultCode, "MLMediaCCParserSetDisplayChangedCallback");
#endif
            }

            ~ParserCEA608()
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                MLResult.Code resultCode = NativeBindings.MLMediaCCParserSetDisplayChangedCallback(this.Handle, null, IntPtr.Zero);
                MLResult.DidNativeCallSucceed(resultCode, "MLMediaCCParserSetDisplayChangedCallback");
                resultCode = NativeBindings.MLMediaCCParserDestroy(this.Handle);
                MLResult.DidNativeCallSucceed(resultCode, "MLMediaCCParserDestroy");
                this.gcHandle.Free();
#endif
            }

            public void ParseAsync(IntPtr data, uint dataSize)
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                MLResult.Code resultCode = NativeBindings.MLMediaCCParserParse(this.Handle, data, dataSize);
                MLResult.DidNativeCallSucceed(resultCode, "MLMediaCCParserParse");
#endif
            }

            internal ulong Handle
            {
                get;
                private set;
            }

            private GCHandle gcHandle;

            public enum CaptionDimension
            {
                MaxRows = 15,
                MaxCols = 32,
                MaxRowsPlus2 = 17,
                MaxColsPlus2 = 34
            }

            public enum CaptionColor
            {
                White = 0,
                Green,
                Blue,
                Cyan,
                Red,
                Yellow,
                Magenta,
                Invalide
            }

            public enum CaptionStyle
            {
                Italics = 0x00000001,
                Underline = 0x00000002
            }


            public struct CaptionLine
            {
                public string text;

                // TODO : native bindings classes should not be used in public stuff
#if UNITY_MAGICLEAP || UNITY_ANDROID
                public NativeBindings.CaptionStyleColor[] midRowStyles;

                public NativeBindings.CaptionPAC[] pacStyles;
#endif
            }
        }
    }
}
