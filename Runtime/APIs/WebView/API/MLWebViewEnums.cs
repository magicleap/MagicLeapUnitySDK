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

    /// <summary>
    /// MLWebView class exposes static functions that allows an application to instantiate a hardware
    /// accelerated WebView and interact with it(via "mouse" and "keyboard" events).
    /// </summary>
    public partial class MLWebView
    {

        /// <summary>
        /// Enums for all possible ML Key Codes.
        /// </summary>
        public enum KeyCode
        {
            Unknown = 0,
            Home = 3,
            Back = 4,
            DpadLeft = 21,
            DpadRight = 22,
            Delete = 67,
            Focus = 80,
            Plus = 81,
            Menu = 82,
            Forward = 125,
            Trigger = 500,
            Capture = 501,
            Bumper = 502,
            Reality = 503,
            Battery = 504,
            MLBack = 508,
            HomeTap = MLBack, // for backward compatability.
            HomeMediumTap = 509,
            Popple = 510,
            MouseLeft = 511,
            MouseRight = 512,
            MouseWheel = 513,
            MouseExtra = 514,
            MouseSide = 515,
            VkbCancel = 1001,
            VkbSubmit = 1002,
            VkbPrevious = 1003,
            VkbNext = 1004,
            VkbClear = 1005,
            VkbClose = 1006,
            VkbCustom1 = 1007,
            VkbCustom2 = 1008,
            VkbCustom3 = 1009,
            VkbCustom4 = 1010,
            VkbHidePassword = 1012,
            VkbShowPassword = 1013
        }

        /// <summary>
        /// Flags to set special key states during input.
        /// </summary>
        [Flags]
        public enum EventFlags
        {
            /// <summary>
            /// No flag is applied.
            /// </summary>
            None = 0,

            /// <summary>
            /// Caps Lock is on.
            /// </summary>
            CapsLockOn = 1 << 0,

            /// <summary>
            /// Shift is pressed.
            /// </summary>
            ShiftDown = 1 << 1,

            /// <summary>
            /// Control is pressed.
            /// </summary>
            ControlDown = 1 << 2,

            /// <summary>
            /// Alt is pressed.
            /// </summary>
            AltDown = 1 << 3,

            /// <summary>
            /// Left mouse button is pressed.
            /// </summary>
            LeftMouseButton = 1 << 4,

            /// <summary>
            /// Middle mouse button is pressed.
            /// </summary>
            MiddleMouseButton = 1 << 5,

            /// <summary>
            /// Right mouse button is pressed.
            /// </summary>
            RightMouseButton = 1 << 6,

            /// <summary>
            /// Command key is pressed.
            /// </summary>
            CommandDown = 1 << 7,

            /// <summary>
            /// NumLock is on.
            /// </summary>
            NumLockOn = 1 << 8,

            /// <summary>
            /// Was key struck on KeyPad.
            /// </summary>
            IsKeyPad = 1 << 9,

            /// <summary>
            /// Is left key.
            /// </summary>
            IsLeft = 1 << 10,

            /// <summary>
            /// Is right key.
            /// </summary>
            IsRight = 1 << 11,

            /// <summary>
            /// AltGR is pressed.
            /// </summary>
            AltGRDown = 1 << 12,
        };

        /// <summary>
        /// Flags related to a text entry field passed when on_show_keyboard is called.
        /// </summary>
        [Flags]
        public enum TextInputFlags
        {
            /// <summary>
            /// Nonne of flasg are applied.
            /// </summary>
            None = 0,

            /// <summary>
            /// Autocompletion is on.
            /// </summary>
            AutocompleteOn = 1 << 0,

            /// <summary>
            /// Autocompletion is off.
            /// </summary>
            AutocompleteOff = 1 << 1,

            /// <summary>
            /// Autocorrection is on.
            /// </summary>
            AutocorrectOn = 1 << 2,

            /// <summary>
            /// Autocorrection is off.
            /// </summary>
            AutocorrectOff = 1 << 3,

            /// <summary>
            /// Spellcheck is on.
            /// </summary>
            SpellcheckOn = 1 << 4,

            /// <summary>
            /// Spellcheck is of.
            /// </summary>
            SpellcheckOff = 1 << 5,

            /// <summary>
            /// Autocapitalize is off.
            /// </summary>
            AutocapitalizeNone = 1 << 6,

            /// <summary>
            /// Autocapitalize characters.
            /// </summary>
            AutocapitalizeCharacters = 1 << 7,

            /// <summary>
            /// Autocapitalize words.
            /// </summary>
            AutocapitalizeWords = 1 << 8,

            /// <summary>
            /// Autocapitalize sentences.
            /// </summary>
            AutocapitalizeSentences = 1 << 9,

            /// <summary>
            /// Have next focusable element.
            /// </summary>
            HaveNextFocusableElement = 1 << 10,

            /// <summary>
            /// Have previous focusable element.
            /// </summary>
            HavePreviousFocusableElement = 1 << 11,

            /// <summary>
            /// has been a password field.
            /// </summary>
            HasBeenPasswordField = 1 << 12,
        };

        /// <summary>
        /// The type of text entry selected when onShowKeyboard is called.
        /// </summary>
        public enum TextInputType
        {
            /// <summary>
            /// None.
            /// </summary>
            None,

            /// <summary>
            /// Text.
            /// </summary>
            Text,

            /// <summary>
            /// Password.
            /// </summary>
            Password,

            /// <summary>
            /// Search.
            /// </summary>
            Search,

            /// <summary>
            /// Email.
            /// </summary>
            Email,

            /// <summary>
            /// Number
            /// </summary>
            Number,

            /// <summary>
            /// Telephone.
            /// </summary>
            Telephone,

            /// <summary>
            /// URL.
            /// </summary>
            URL,

            /// <summary>
            /// Date.
            /// </summary>
            Date,

            /// <summary>
            /// Date Time.
            /// </summary>
            DateTime,

            /// <summary>
            /// Date Time Local.
            /// </summary>
            DateTimeLocal,

            /// <summary>
            /// Month.
            /// </summary>
            Month,

            /// <summary>
            /// Time.
            /// </summary>
            Time,

            /// <summary>
            /// Week.
            /// </summary>
            Week,

            /// <summary>
            /// Text Area.
            /// </summary>
            TextArea,

            /// <summary>
            /// Content Editable.
            /// </summary>
            ContentEditable,

            /// <summary>
            /// Date Time Field.
            /// </summary>
            DateTimeField,
        };
    }
}

