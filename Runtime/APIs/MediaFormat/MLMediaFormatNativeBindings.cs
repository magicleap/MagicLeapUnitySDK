// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// APIs for creating and retrieving media format information.
    /// </summary>
    public partial class MLMediaFormat
    {
        /// <summary>
        /// See ml_media_format.h for additional comments.
        /// </summary>
        private class NativeBindings : MagicLeapNativeBindings
        {
            /// <summary>
            /// Internal max size of key string.
            /// </summary>
            public static int MAX_KEY_STRING_SIZE = 64;

            /// <summary>
            /// Internal max size of format string.
            /// </summary>
            public static int MAX_FORMAT_STRING_SIZE = 512;

            /// <summary>
            /// Data type containing byte array buffer and the size.
            /// </summary>
            [StructLayout(LayoutKind.Sequential)]
            internal struct MLMediaFormatByteArray
            {
                /// <summary>
                /// Buffer starting pointer.
                /// </summary>
                public readonly IntPtr Ptr;

                /// <summary>
                /// Buffer length.
                /// </summary>
                public readonly ulong Length;

                /// <summary>
                /// Constructor
                /// </summary>
                /// <param name="ptr">Buffer starting pointer</param>
                /// <param name="length">Buffer length</param>
                public MLMediaFormatByteArray(IntPtr ptr, ulong length)
                {
                    this.Ptr = ptr;
                    this.Length = length;
                }
            };

            /// <summary>
            /// Create a video format object.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatCreateVideo([MarshalAs(unmanagedType: UnmanagedType.LPStr)] string mimeType, int Width, int Height, out ulong outHandle);

            /// <summary>
            /// Create a audio format object.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatCreateAudio([MarshalAs(unmanagedType: UnmanagedType.LPStr)] string MimeType, int SampleRate, int ChannelCount, out ulong outHandle);

            /// <summary>
            /// Create a subtitle format object.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatCreateSubtitle([MarshalAs(unmanagedType: UnmanagedType.LPStr)] string MimeType, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string Language, out ulong outHandle);

            /// <summary>
            /// Create an empty format object.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatCreate(out ulong outHandle);

            /// <summary>
            /// Create a copy of the format handle provided.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatCreateCopy(ulong handle, out ulong outHandle);

            /// <summary>
            /// Destroy a format object.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatDestroy(ulong handle);

            /// <summary>
            /// Obtain the human readable representation of the format.  User shall allocate at least #MAX_FORMAT_STRING_SIZE bytes of
            /// memory for storing the output format string.  The format string contains a list of key-value pairs, which can be
            /// extracted individually via the suitable "GetKeyValue" APIs.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatObjectToString(ulong handle, IntPtr outString);

            /// <summary>
            /// Obtain the value of an integer key.  For example, use #MLMediaFormat_Key_Frame_Rate key to get the framerate of a video
            /// track.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatGetKeyValueInt32(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, out int outKey);

            /// <summary>
            /// Obtain the value of an long key.  For example, use MLMediaFormat_Key_Duration key to get duration of a track.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatGetKeyValueInt64(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, out long outKey);

            /// <summary>
            /// Obtain the value of an float key.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatGetKeyValueFloat(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, out float outKey);

            /// <summary>
            /// Obtain the size of a key.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatGetKeySize(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, out ulong outSize);

            /// <summary>
            /// Obtain the value of a string key.  User shall allocate at least #MAX_KEY_STRING_SIZE bytes of memory for storing the
            /// output key string.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatGetKeyString(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, IntPtr outString);

            /// <summary>
            /// Obtain the value of a ByteBuffer key.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatGetKeyByteBuffer(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, out MLMediaFormatByteArray OutBuffer);

            /// <summary>
            /// Release the ByteBuffer acquired by MLMediaFormatGetKeyByteBuffer.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatKeyByteBufferRelease(ulong handle, ref MLMediaFormatByteArray buffer);

            /// <summary>
            /// Set the value of an integer key.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatSetKeyInt32(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, int keyValue);

            /// <summary>
            /// Set the value of a long key.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatSetKeyInt64(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, long keyValue);

            /// <summary>
            /// Set the value of a float key.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatSetKeyFloat(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, float keyValue);

            /// <summary>
            /// Set the size of a key.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatSetKeySize(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, ulong keySize);

            /// <summary>
            /// Set the value of a string key.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatSetKeyString(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string KeyValue);

            /// <summary>
            /// Set the value of a ByteBuffer key.
            /// </summary>
            [DllImport(MLMediaFormatDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLMediaFormatSetKeyByteBuffer(ulong handle, [MarshalAs(unmanagedType: UnmanagedType.LPStr)] string name, ref MLMediaFormatByteArray Buffer);
        }
    }
}
