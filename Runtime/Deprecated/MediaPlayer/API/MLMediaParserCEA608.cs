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
    using System.Text;

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
                MLResult.Code resultCode = NativeBindings.MLMediaCCParserCreate(out ulong parserHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCCParserCreate));
                this.Handle = parserHandle;
                this.gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
                IntPtr gcHandlePtr = GCHandle.ToIntPtr(this.gcHandle);
                resultCode = NativeBindings.MLMediaCCParserSetDisplayChangedCallback(this.Handle, NativeBindings.OnDisplayChanged, gcHandlePtr);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCCParserSetDisplayChangedCallback));
            }

            ~ParserCEA608()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaCCParserSetDisplayChangedCallback(this.Handle, null, IntPtr.Zero);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCCParserSetDisplayChangedCallback));
                resultCode = NativeBindings.MLMediaCCParserDestroy(this.Handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCCParserDestroy));
                this.gcHandle.Free();
            }

            public void ParseAsync(IntPtr data, uint dataSize)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaCCParserParse(this.Handle, data, dataSize);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCCParserParse));
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
                public NativeBindings.CaptionStyleColor[] midRowStyles;

                public NativeBindings.CaptionPAC[] pacStyles;
            }
        }
    }
}
