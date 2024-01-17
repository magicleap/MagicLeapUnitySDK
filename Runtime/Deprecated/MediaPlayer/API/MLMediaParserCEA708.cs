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
    using System.Text;

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    public partial class MLMedia
    {
        /// <summary>
        /// CEA708 Parser.
        /// </summary>
        public partial class ParserCEA708
        {
            public const string Mime = "text/cea-708";
            private string subtitleText = string.Empty;
            private string[] cea708Strings = new string[2];
            private int cea708WindowID = -1;

            public delegate void OnTextDelegate(string text);
            public event OnTextDelegate OnText = delegate { };

            public delegate void OnEmitEventDelegate(Cea708CaptionEmitCommand command, object obj);
            public event OnEmitEventDelegate OnEmitEvent = delegate { };

            private StringBuilder stringBuilder = new StringBuilder();

            public ParserCEA708()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaCea708ParserCreate(out ulong parserHandle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCea708ParserCreate));
                this.Handle = parserHandle;
                this.gcHandle = GCHandle.Alloc(this, GCHandleType.Weak);
                IntPtr gcHandlePtr = GCHandle.ToIntPtr(this.gcHandle);
                resultCode = NativeBindings.MLMediaCea708ParserSetEmitEventCallback(this.Handle, NativeBindings.OnEmitEvent, gcHandlePtr);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCea708ParserSetEmitEventCallback));
            }

            ~ParserCEA708()
            {
                MLResult.Code resultCode = NativeBindings.MLMediaCea708ParserSetEmitEventCallback(this.Handle, null, IntPtr.Zero);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCea708ParserSetEmitEventCallback));
                resultCode = NativeBindings.MLMediaCea708ParserDestroy(this.Handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCea708ParserDestroy));
                this.gcHandle.Free();
            }

            public void ParseAsync(IntPtr data, uint dataSize)
            {
                MLResult.Code resultCode = NativeBindings.MLMediaCea708ParserParse(this.Handle, data, dataSize);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaCea708ParserParse));
            }

            internal ulong Handle
            {
                get;
                private set;
            }

            private GCHandle gcHandle;

            /// <summary>
            /// CEA708 Caption Pen Size constants.
            /// <c>MLCea708CaptionPenSize</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionPenSize
            {
                /// <summary>
                /// Small pen size.
                /// </summary>
                Small = 0,

                /// <summary>
                /// Standard pen size.
                /// </summary>
                Standard = 1,

                /// <summary>
                /// Large pen size.
                /// </summary>
                Large = 2,
            }

            /// <summary>
            /// CEA708 Caption Pen Offset constants.
            /// <c>MLCea708CaptionPenOffset</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionPenOffset
            {
                /// <summary>
                /// Subscript offset.
                /// </summary>
                Subscript = 0,

                /// <summary>
                /// Normal offset.
                /// </summary>
                Normal = 1,

                /// <summary>
                /// Superscript offset.
                /// </summary>
                Superscript = 2,
            }


            /// <summary>
            /// CEA708 Caption Emit Commands constants.
            /// <c>MLCea708CaptionEmitCommand</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionEmitCommand : int
            {
                /// <summary>
                /// Buffer command.
                /// </summary>
                Buffer = 1,

                /// <summary>
                /// Control command.
                /// </summary>
                Control = 2,

                /// <summary>
                /// SetCurrentWindow tells the caption decoder which window the following commands describe:
                /// - SetWindowAttributes
                /// - SetPenAttributes
                /// - SetPenColor
                /// - SetPenLocation.
                /// If the window specified has not already been created with a DefineWindow command then,
                /// SetCurrentWindow and the window property commands can be safely ignored.
                /// </summary>
                CWX = 3,

                /// <summary>
                /// ClearWindows clears all the windows specified in the 8 bit window bitmap.
                /// </summary>
                CLW = 4,

                /// <summary>
                /// DisplayWindows displays all the windows specified in the 8 bit window bitmap.
                /// </summary>
                DSW = 5,

                /// <summary>
                /// HideWindows hides all the windows specified in the 8 bit window bitmap.
                /// </summary>
                HDW = 6,

                /// <summary>
                /// ToggleWindows hides all displayed windows, and displays all hidden windows specified in the 8 bit window bitmap.
                /// </summary>
                TGW = 7,

                /// <summary>
                /// DeleteWindows deletes all the windows specified in the 8 bit window bitmap.
                /// If the current window, as specified by the last SetCurrentWindow command,
                /// is deleted then the current window becomes undefined and the window attribute commands
                /// should have no effect until after the next SetCurrentWindow or DefineWindow command.
                /// </summary>
                DLW = 8,

                /// <summary>
                /// Delay suspends all processing of the current service, except for DelayCancel and Reset scanning.
                /// </summary>
                DLY = 9,

                /// <summary>
                /// DelayCancel terminates any active delay and resumes normal command processing. DelayCancel should be scanned for during a Delay.
                /// </summary>
                DLC = 10,

                /// <summary>
                /// Reset deletes all windows, cancels any active delay, and clears the buffer before the Reset command. Reset should be scanned for during a Delay.
                /// </summary>
                RST = 11,

                /// <summary>
                /// The SetPenAttributes command specifies how certain attributes of subsequent characters are to be rendered in the current window, until the next SetPenAttributes command.
                /// </summary>
                SPA = 12,

                /// <summary>
                /// SetPenColor sets the foreground, background, and edge color for the subsequent characters.
                /// </summary>
                SPC = 13,

                /// <summary> SetPenLocation sets the location of for the next bit of appended text in the current window. It has two parameters, row and column.
                /// </summary>
                SPL = 14,

                /// <summary>
                /// SetWindowAttributes Sets the window attributes of the current window.
                /// </summary>
                SWA = 15,

                /// <summary>
                /// DefineWindow0-7 creates one of the eight windows used by a caption decoder.
                /// </summary>
                DFX = 16
            }

            /// <summary>
            /// CEA708 Caption Emit Command Control constants.
            /// <c>MLCea708CaptionEmitCommandControl</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionEmitCommandControl : int
            {
                /// <summary>
                /// End of text.
                /// </summary>
                ETX = 0x03,

                /// <summary>
                /// Back space.
                /// </summary>
                BS = 0x08,

                /// <summary>
                /// This code is equivalent to CEA708 CLW command
                /// </summary>
                FF = 0x0c,

                /// <summary>
                /// Carriage return.
                /// </summary>
                HCR = 0x0e
            }

            /// <summary>
            /// CEA708 Caption Color Opacity constants.
            /// <c>MLCea708CaptionColorOpacity</c> from <c>ml_media_cea708_caption.h</c>.
            /// </summary>
            public enum Cea708CaptionColorOpacity : int
            {
                /// <summary>
                /// Solid opacity.
                /// </summary>
                Solid = 0,

                /// <summary>
                /// Flashing opacity.
                /// </summary>
                Flash = 1,

                /// <summary>
                /// Translucent opacity.
                /// </summary>
                Translucent = 2,

                /// <summary>
                /// Transparent opacity.
                /// </summary>
                Transparent = 3
            }
        }
    }
}
