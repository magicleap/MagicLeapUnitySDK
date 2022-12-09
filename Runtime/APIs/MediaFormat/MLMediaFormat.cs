// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// APIs for creating and retrieving media format information.
    /// </summary>
    public partial class MLMediaFormat
    {
        /// <summary>
        /// Audio data format for track type.
        /// </summary>
        public enum AudioEncoding
        {
            None = 0,

            /// <summary>
            ///  Audio data format: PCM 16 bits per sample.
            /// </summary>
            PCM16Bits = 2,

            /// <summary>
            ///  Audio data format: PCM 8 bits per sample.
            /// </summary>
            PCM8Bits = 3,

            /// <summary>
            ///  Audio data format: single-precision floating-point per sample.
            /// </summary>
            PCMFloat = 4,

            /// <summary>
            ///  Audio data format: PCM 32 bits per sample.
            /// </summary>
            PCM32Bits = 201,
        }

        /// <summary>
        /// Handle for the underlying unmanaged object.
        /// </summary>
        public ulong Handle { get; private set; }

        /// <summary>
        /// Create a video format object.
        /// </summary>
        /// <param name="mimeType">Mime type of the content</param>
        /// <param name="width">Width of the content in pixels</param>
        /// <param name="height">Height of the content in pixels</param>
        /// <returns>An MLMediaFormat object if successful, null otherwise</returns>
        public static MLMediaFormat CreateVideo(string mimeType, int width, int height)
        {
            if (MLResult.DidNativeCallSucceed(NativeBindings.MLMediaFormatCreateVideo(mimeType, width, height, out ulong handle), nameof(NativeBindings.MLMediaFormatCreateVideo)))
            {
                return new MLMediaFormat(handle);
            }
            return null;
        }

        /// <summary>
        /// Create a audio format object.
        /// </summary>
        /// <param name="mimeType">Mime type of the content</param>
        /// <param name="sampleRate">Sample rate of the content</param>
        /// <param name="channelCount">Number of audio channels</param>
        /// <returns>An MLMediaFormat object if successful, null otherwise</returns>
        public static MLMediaFormat CreateAudio(string mimeType, int sampleRate, int channelCount)
        {
            if (MLResult.DidNativeCallSucceed(NativeBindings.MLMediaFormatCreateAudio(mimeType, sampleRate, channelCount, out ulong handle), nameof(NativeBindings.MLMediaFormatCreateAudio)))
            {
                return new MLMediaFormat(handle);
            }
            return null;
        }

        /// <summary>
        /// Create a subtitle format object.
        /// </summary>
        /// <param name="mimeType">Mime type of the content</param>
        /// <param name="language">Language of the content, using either
        /// ISO 639-1 or 639-2/T codes. Specify null or "und" if language
        /// information is only included in the content.</param>
        /// <returns>An MLMediaFormat object if successful, null otherwise</returns>
        public static MLMediaFormat CreateSubtitle(string mimeType, string language)
        {
            if (MLResult.DidNativeCallSucceed(NativeBindings.MLMediaFormatCreateSubtitle(mimeType, language, out ulong handle), nameof(NativeBindings.MLMediaFormatCreateSubtitle)))
            {
                return new MLMediaFormat(handle);
            }
            return null;
        }

        /// <summary>
        /// Create a copy of the format handle provided.
        ///
        /// This can be particularly useful to persist the media format handle/object that
        /// was received through the callbacks(as life cycle of those media format objects
        /// are with in the callback context only).
        /// The API call to make a copy ensures that the copied object exists until released
        /// by the app.
        /// </summary>
        /// <param name="format">MLMediaFormat object to copy</param>
        /// <returns>An MLMediaFormat object if successful, null otherwise</returns>
        // TODO : can we replace with a "copy-constructor" or Clone() method?
        public static MLMediaFormat CreateCopy(MLMediaFormat format)
        {
            if (MLResult.DidNativeCallSucceed(NativeBindings.MLMediaFormatCreateCopy(format.Handle, out ulong handle), nameof(NativeBindings.MLMediaFormatCreateCopy)))
            {
                return new MLMediaFormat(handle);
            }
            return null;
        }

        /// <summary>
        /// Create an empty format object.
        /// </summary>
        /// <returns>An MLMediaFormat object if successful, null otherwise</returns>
        // TODO : replace this with a regular constructor, but then we cant return null value if MLMediaFormatCreate() fails.
        public static MLMediaFormat CreateEmpty()
        {
            if (MLResult.DidNativeCallSucceed(NativeBindings.MLMediaFormatCreate(out ulong handle), nameof(NativeBindings.MLMediaFormatCreate)))
            {
                return new MLMediaFormat(handle);
            }
            return null;
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="handle">Unmanaged handle</param>
        internal MLMediaFormat(ulong handle)
        {
            this.Handle = handle;
        }

        /// <summary>
        /// Finalizer, destroys unmanaged object.
        /// </summary>
        ~MLMediaFormat()
        {
            MLResult.Code resultCode = NativeBindings.MLMediaFormatDestroy(Handle);
            MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaFormatDestroy));
        }

        /// <summary>
        /// Human readable representation of the format.
        /// </summary>
        /// <returns>Human readable representation of the format.</returns>
        public override string ToString()
        {
            string objToStr = string.Empty;

            IntPtr stringPtr = Marshal.AllocHGlobal(NativeBindings.MAX_FORMAT_STRING_SIZE);

            if (MLResult.DidNativeCallSucceed(NativeBindings.MLMediaFormatObjectToString(Handle, stringPtr), nameof(NativeBindings.MLMediaFormatObjectToString)))
            {
                objToStr = Marshal.PtrToStringAnsi(stringPtr);
            }
            Marshal.FreeHGlobal(stringPtr);

            return objToStr;
        }

        /// <summary>
        /// Obtain the value of an integer key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to get the value for</param>
        /// <param name="value">Out param to get the value in</param>
        /// <returns>MLResult.Result.Ok if value was obtained successfully</returns>
        public MLResult GetValue(string keyName, out int value)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatGetKeyValueInt32(Handle, keyName, out value));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatGetKeyValueInt32));
            return result;
        }

        /// <summary>
        /// Obtain the value of a long key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to get the value for</param>
        /// <param name="value">Out param to get the value in</param>
        /// <returns>MLResult.Result.Ok if value was obtained successfully</returns>
        public MLResult GetValue(string keyName, out long value)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatGetKeyValueInt64(Handle, keyName, out value));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatGetKeyValueInt64));
            return result;
        }

        /// <summary>
        /// Obtain the value of a float key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to get the value for</param>
        /// <param name="value">Out param to get the value in</param>
        /// <returns>MLResult.Result.Ok if value was obtained successfully</returns>
        public MLResult GetValue(string keyName, out float value)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatGetKeyValueFloat(Handle, keyName, out value));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatGetKeyValueFloat));
            return result;
        }

        /// <summary>
        /// Obtain the value of a string key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to get the value for</param>
        /// <param name="value">Out param to get the value in</param>
        /// <returns>MLResult.Result.Ok if value was obtained successfully</returns>
        public MLResult GetValue(string keyName, out string value)
        {
            value = string.Empty;

            IntPtr stringPtr = Marshal.AllocHGlobal(NativeBindings.MAX_KEY_STRING_SIZE);
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatGetKeyString(Handle, keyName, stringPtr));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatGetKeyString));
            if (result.IsOk)
            {
                value = Marshal.PtrToStringAnsi(stringPtr);
            }
            Marshal.FreeHGlobal(stringPtr);
            return result;
        }

        /// <summary>
        /// Obtain the value of a byte buffer key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to get the value for</param>
        /// <param name="value">Out param to get the value in</param>
        /// <returns>MLResult.Result.Ok if value was obtained successfully</returns>
        public MLResult GetValue(string keyName, out byte[] value)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatGetKeyByteBuffer(Handle, keyName, out NativeBindings.MLMediaFormatByteArray nativeByteBuffer));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatGetKeyByteBuffer));
            if (result.IsOk)
            {
                value = new byte[nativeByteBuffer.Length];
                Marshal.Copy(nativeByteBuffer.Ptr, value, 0, (int)nativeByteBuffer.Length);

                MLResult.Code resultCode = NativeBindings.MLMediaFormatKeyByteBufferRelease(Handle, ref nativeByteBuffer);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaFormatKeyByteBufferRelease));
            }
            else
            {
                value = null;
            }

            return result;
        }

        /// <summary>
        /// Obtain the value of an unsigned long key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to get the value for</param>
        /// <param name="value">Out param to get the value in</param>
        /// <returns>MLResult.Result.Ok if value was obtained successfully</returns>
        public MLResult GetValue(string keyName, out ulong size)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatGetKeySize(Handle, keyName, out size));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatGetKeySize));
            return result;
        }

        /// <summary>
        /// Set the value of an integer key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to set the value for</param>
        /// <param name="value">Value to set</param>
        /// <returns>MLResult.Result.Ok if value was set successfully</returns>
        public MLResult SetValue(string keyName, int value)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatSetKeyInt32(Handle, keyName, value));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatSetKeyInt32));
            return result;
        }

        /// <summary>
        /// Set the value of a long key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to set the value for</param>
        /// <param name="value">Value to set</param>
        /// <returns>MLResult.Result.Ok if value was set successfully</returns>
        public MLResult SetValue(string keyName, long value)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatSetKeyInt64(Handle, keyName, value));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatSetKeyInt64));
            return result;
        }

        /// <summary>
        /// Set the value of a float key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to set the value for</param>
        /// <param name="value">Value to set</param>
        /// <returns>MLResult.Result.Ok if value was set successfully</returns>
        public MLResult SetValue(string keyName, float value)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatSetKeyFloat(Handle, keyName, value));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatSetKeyFloat));
            return result;
        }

        /// <summary>
        /// Set the value of a string key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to set the value for</param>
        /// <param name="value">Value to set</param>
        /// <returns>MLResult.Result.Ok if value was set successfully</returns>
        public MLResult SetValue(string keyName, string value)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatSetKeyString(Handle, keyName, value));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatSetKeyString));
            return result;
        }

        /// <summary>
        /// Set the value of a byte buffer key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to set the value for</param>
        /// <param name="value">Value to set</param>
        /// <returns>MLResult.Result.Ok if value was set successfully</returns>
        public MLResult SetValue(string keyName, byte[] value)
        {
            IntPtr bufferPtr = Marshal.AllocHGlobal(value.Length);
            Marshal.Copy(value, 0, bufferPtr, value.Length);
            NativeBindings.MLMediaFormatByteArray nativeBuffer = new NativeBindings.MLMediaFormatByteArray(bufferPtr, (uint)value.Length);

            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatSetKeyByteBuffer(Handle, keyName, ref nativeBuffer));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatSetKeyByteBuffer));
            Marshal.FreeHGlobal(bufferPtr);

            return result;
        }

        /// <summary>
        /// Set the value of an unsigned long key.
        /// </summary>
        /// <param name="keyName">MLMediaFormatKey name to set the value for</param>
        /// <param name="value">Value to set</param>
        /// <returns>MLResult.Result.Ok if value was set successfully</returns>
        public MLResult SetValue(string keyName, ulong size)
        {
            MLResult result = MLResult.Create(NativeBindings.MLMediaFormatSetKeySize(Handle, keyName, size));
            MLResult.DidNativeCallSucceed(result.Result, nameof(NativeBindings.MLMediaFormatSetKeySize));
            return result;
        }

    }
}
