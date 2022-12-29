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
    using UnityEngine.XR.MagicLeap.Native;

    /// <summary>
    /// MLMediaTTML Summary placeholder.
    /// </summary>
    public partial class MLMediaTTML
    {
        /// <summary>
        ///  An unset position, width, or size.
        /// </summary>
        public const float MLTTMLDimen_Unset = -3.402823466e+38F;

        /// <summary>
        /// The cue default text size type;
        public enum MLTTMLTextSizeType
        {
            /// <summary>
            ///  An unset text size type value.
            /// </summary>
            Unset = 0x7FFFFFFF,

            /// <summary>
            ///  Text size is measured as a fraction of the viewport size minus the view padding.
            /// </summary>
            Fractional = 0,

            /// <summary>
            ///  Text size is measured as a fraction of the viewport size, ignoring the view padding
            /// </summary>
            FractionalIgnorePadding = 1,

            /// <summary>
            ///  Text size is measured in number of pixels.
            /// </summary>
            Absolute = 2,
        };

        /// <summary>
        /// The cue box anchor in the direction of position.
        /// </summary>
        public enum MLTTMLAnchorType
        {
            /// <summary>
            ///  An unset anchor type value.
            /// </summary>
            Unset = 0x7FFFFFFF,

            /// <summary>
            ///  Anchors the left (for horizontal positions) or top (for vertical positions) edge of the cue box.
            /// </summary>
            Start = 0,

            /// <summary>
            ///  Anchors the middle of the cue box.
            /// </summary>
            Middle = 1,

            /// <summary>
            ///  Anchors the right (for horizontal positions) or bottom (for vertical positions) edge of the cue box.
            /// </summary>
            End = 2
        };

        /// <summary>
        /// The alignment of the cue text within the cue box.
        /// </summary>
        public enum MLTTMLLayoutAlignment
        {
            /// <summary>
            ///  An unspecified alignment value.
            /// </summary>
            Unspecified = 0x7FFFFFFF,

            /// <summary>
            ///  Normal alignment relative to text direction.
            /// </summary>
            Normal = 0,

            /// <summary>
            ///  Centered alignment.
            /// </summary>
            Center = 1,

            /// <summary>
            ///  Opposite alignment relative to text direction.
            /// </summary>
            Opposite = 2,
        };

        /// <summary>
        /// The cue line type.
        /// </summary>
        public enum MLTTMLLineType
        {
            /// <summary>
            ///  An unset line type value.
            /// </summary>
            Unset = 0x7FFFFFFF,

            /// <summary>
            ///  Line position is fractional position within the viewport.
            /// </summary>
            Fraction = 0,

            /// <summary>
            ///  Line position is number of lines, interpretation also depends on the line anchor type.
            /// </summary>
            Number = 1,
        };

        /// <summary>
        /// TTML data structure.
        /// </summary>
        public struct MLTTMLData
        {
            /// <summary>
            /// Track cue start time in milliseconds.
            /// </summary>
            public long StartTimeMs { get; internal set; }

            /// <summary>
            /// Track cue end time in milliseconds.
            /// </summary>
            public long EndTimeMs { get; internal set; }

            /// <summary>
            /// The cue text encoded as UTF-8.  Or null if this is an image cue.
            /// </summary>
            public string Text { get; internal set; }

            /// <summary>
            /// The alignment of the cue text within the cue box.
            /// </summary>
            public MLTTMLLayoutAlignment TextAlignment { get; internal set; }

            /// <summary>
            /// The cue image if this is an image cue, an empty image otherwise.
            /// </summary>
            public MLTTMLImage Bitmap { get; internal set; }

            /// <summary>
            /// The position of the #line_anchor of the cue box within the viewport in the direction orthogonal to the writing direction,
            /// or <seealso cref="MLMediaTTML.MLTTMLDimen_Unset"/>.  When set, the interpretation of the value depends on the value of #line_type.
            /// For horizontal text and <seealso cref="LineType"/> equal to <seealso cref="MLTTMLLineType.Fraction"/>,
            /// this is the fractional vertical position relative to the top of the viewport.
            /// </summary>
            public float Line { get; internal set; }

            /// <summary>
            /// The cue line type. <seealso cref="MLTTMLLineType.Fraction"/> indicates that #line is a fractional position within the viewport.
            /// <seealso cref="MLTTMLLineType.Number"/> indicates that #line is a line number, where the size of each line is taken to be the size
            /// of the first line of the cue.  When #line is greater than or equal to 0 lines count from the start of the viewport, with 0
            /// indicating zero offset from the start edge.  When #line is negative lines count from the end of the viewport, with -1 indicating
            /// zero offset from the end edge.  For horizontal text the line spacing is the height of the first line of the cue, and the
            /// start and end of the viewport are the top and bottom respectively.  Note that it's particularly important to consider
            /// the effect of <seealso cref="LineAnchor"/> when using <seealso cref="MLTTMLLineType.Number"/>.
            /// </summary>
            public MLTTMLLineType LineType { get; internal set; }

            /// <summary>
            /// The cue box anchor in the direction of line.
            /// </summary>
            public MLTTMLAnchorType LineAnchor { get; internal set; }

            /// <summary>
            /// The fractional position of the #position_anchor of the cue box within the viewport in the direction orthogonal to #line,
            /// or <seealso cref="MLMediaTTML.MLTTMLDimen_Unset"/>.
            /// For horizontal text, this is the horizontal position relative to the left of the viewport.
            /// Note that positioning is relative to the left of the viewport even in the case of right-to-left text.
            /// </summary>
            public float Position { get; internal set; }

            /// <summary>
            /// The cue box anchor in the direction of position.
            /// </summary>
            public MLTTMLAnchorType PositionAnchor { get; internal set; }

            /// <summary>
            /// The cue box size in the writing direction, as a fraction of the viewport size or <seealso cref="MLMediaTTML.MLTTMLDimen_Unset"/>.
            /// </summary>
            public float Size { get; internal set; }

            /// <summary>
            /// The cue bitmap height as a fraction of the viewport size or <seealso cref="MLMediaTTML.MLTTMLDimen_Unset"/>
            /// if the bitmap should be displayed at its natural height given the bitmap dimensions and the specified #size.
            /// </summary>
            public float BitmapHeight { get; internal set; }

            /// <summary>
            /// Specifies whether or not the <seealso cref="WindowColor"/> property is set.
            /// </summary>
            public bool WindowColorSet { get; internal set; }

            /// <summary>
            /// The cue window fill color in ARGB format.
            /// </summary>
            public uint WindowColor { get; internal set; }

            /// <summary>
            /// The cue default text size type, or or <seealso cref="Unset"/> if this cue has no default text size.
            /// </summary>
            public MLTTMLTextSizeType TextSizeType { get; internal set; }

            /// <summary>
            /// The cue default text size, or <seealso cref="MLMediaTTML.MLTTMLDimen_Unset"/> if this cue has no default.
            /// </summary>
            public float TextSize { get; internal set; }

            /// <summary>
            /// Create and return an initialized version of this struct.
            /// </summary>
            /// <returns>A new instance of this struct.</returns>
            public static MLTTMLData Create()
            {
                return new MLTTMLData()
                {
                };
            }
        };

        /// <summary>
        /// Byte data for TTML image cues.
        /// </summary>
        public struct MLTTMLImage
        {
            /// <summary>
            /// Image size in bytes.
            /// </summary>
            public uint Size { get; private set; }

            /// <summary>
            /// Byte data of the image.
            /// </summary>
            public byte[] Data { get; private set; }

            /// <summary>
            /// Create and return an initialized version of this struct.
            /// </summary>
            public static MLTTMLImage Create()
            {
                return new MLTTMLImage()
                {
                    Size = 0,
                    Data = Array.Empty<byte>(),
                };
            }

            public static MLTTMLImage Create(byte[] outBuffer, uint imageSize)
            {
                return new MLTTMLImage()
                {
                    Size = imageSize,
                    Data = outBuffer,
                };
            }
        };
    }
}
