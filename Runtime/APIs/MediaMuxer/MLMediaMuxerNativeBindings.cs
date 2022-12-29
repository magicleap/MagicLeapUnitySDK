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
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>
    /// MLMedia APIs.
    /// </summary>
    public partial class MLMedia
    {
        /// <summary>
        /// MediaMuxer description goes here.
        /// </summary>
        public sealed partial class Muxer
        {
            /// <summary>
            /// See ml_media_muxer.h for additional comments.
            /// </summary>
            private class NativeBindings : Native.MagicLeapNativeBindings
            {
                /// <summary>
                /// Data type that encapsulates sample data to be written in to <see cref="Muxer"/>.
                /// </summary>
                [StructLayout(LayoutKind.Sequential)]
                public struct MLMediaMuxerSampleData
                {
                    /// <summary>
                    /// Version of this structure.
                    /// </summary>
                    public uint Version;

                    /// <summary>
                    /// The track index number, to which to write the sample data into.  This should be a value returned by <see cref="AddAudioTrack"/> or <see cref="AddVideoTrack"/>
                    /// </summary>
                    public uint TrackIndex;

                    /// <summary>
                    /// Pointer to the sample buffer/data.
                    /// </summary>
                    public byte[] Buffer;

                    /// <summary>
                    /// Size of the sample buuffer/data.
                    /// </summary>
                    public uint Size;

                    /// <summary>
                    /// The buffer's time stamp in microseconds.
                    /// </summary>
                    public ulong Time;

                    /// <summary>
                    /// The flag about the data. 
                    /// </summary>
                    public CodecBufferFlags Flags;

                    public static MLMediaMuxerSampleData Create()
                    {
                        return new MLMediaMuxerSampleData()
                        {
                            Version = 1,
                            TrackIndex = 0,
                            Buffer = null,
                            Size = 0,
                            Time = 0,
                            Flags = CodecBufferFlags.KeyFrame
                        };
                    }
                };

                /// <summary>
                /// Create a Muxer
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerCreate(out ulong muxerHandle);


                /// <summary>
                /// Writes an encoded sample into the muxer.  The application needs to make sure that the samples are written into the right
                /// tracks.  Also, it needs to make sure the samples for each track are written in chronological order (e.  g.  in the
                /// order they are provided by the encoder.  ) For MPEG4 media format, the duration of the last sample in a track can be set by
                /// passing an additional empty buffer) with #MLMediaCodecBufferFlag_EOS flag and a suitable presentation timestamp set in
                /// time_us (of MLMediaMuxerSampleData structure) as the last sample of that track.  This last sample's presentation
                /// timestamp shall be a sum of the presentation timestamp and the duration preferred for the original last sample.  If no explicit
                /// END_OF_STREAM sample was passed, then the duration of the last sample would be the same as that of the sample before
                /// that.  The buffer can be reused once this method returns.
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerWriteSampleData(ulong muxerHandle, in MLMediaMuxerSampleData Data);


                /// <summary>
                /// Set the orientation hint for output video playback.  This should be called before #MLMediaMuxerStart() and after
                /// #MLMediaMuxerConfigure.  Calling this method will not rotate the video frame when muxer is generating the file, but add a
                /// composition matrix containing the rotation angle in the output video if the output format is MLMediaMuxerOutputFormat_MPEG4
                /// so that a video player can choose the proper orientation for playback.  Note that some video players may choose to ignore
                /// the composition matrix in a video during playback.  By default, the rotation degree is 0.
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerSetOrientationHint(ulong muxerHandle, int Degrees);


                /// <summary>
                /// Stop muxing.  Once the muxer stops, it can not be restarted.
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerStop(ulong muxerHandle);


                /// <summary>
                /// Start muxing.  Make sure all the tracks have been added (#MLMediaMuxerAddTrack) before calling this.
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerStart(ulong muxerHandle);


                /// <summary>
                /// Release the Muxer
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerRelease(ulong muxerHandle);


                /// <summary>
                /// Get a list of all the supported Media Muxer Output Formats on the Platform.  User doesn't own the memory/buffer returned
                /// from this API and MUST NOT be freeing or releasing the out_format_list.
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerGetSupportedOutputFormats(ulong muxerHandle, [MarshalAs(UnmanagedType.LPArray)] out IntPtr OutFormatList, out int OutFormatListSize);


                /// <summary>
                /// Get a list of all the supported mime-types for a given Media Muxer Output Format on the Platform.  User doesn't own the
                /// memory/buffer returned from this API and MUST NOT be freeing or releasing the out_mime_list.
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerGetSupportedMimes(ulong muxerHandle, OutputFormat Format, out IntPtr OutMimeList, out int OutMimeListSize);


                /// <summary>
                /// Add a track with given format information.
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerAddTrack(ulong muxerHandle, ulong formatHandle, out UIntPtr OutTrackIndex);


                /// <summary>
                /// Configure the Muxer
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerConfigure(ulong muxerHandle, int Format, string Path);


                /// <summary>
                /// Set and store the geodata (latitude and longitude) in the output file.  This should be called before
                /// #MLMediaMuxerStart() and after #MLMediaMuxerConfigure.  The geodata is stored in udta box if the output format is
                /// MLMediaMuxerOutputFormat_MPEG4, and is ignored for other output formats.  The geodata is stored according to ISO-6709 standard.
                /// </summary>
                [DllImport(MediaMuxerDll, CallingConvention = CallingConvention.Cdecl)]
                public static extern MLResult.Code MLMediaMuxerSetLocation(ulong muxerHandle, float Latitude, float Longitude);
            }

            private MLResult.Code InternalGetSupportedOutputFormats(out OutputFormat[] formats)
            {
                const int sizeOfOutputFormatEnum = sizeof(OutputFormat);
                var resultCode = NativeBindings.MLMediaMuxerGetSupportedOutputFormats(handle, out IntPtr resultPtr, out int resultCount);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaMuxerGetSupportedOutputFormats));
                formats = new OutputFormat[resultCount];
                if (resultCode == MLResult.Code.Ok)
                {
                    for (int i = 0; i < resultCount; i++)
                    {
                        formats[i] = (OutputFormat)Marshal.ReadInt32(resultPtr);
                        resultPtr += sizeOfOutputFormatEnum;
                    }
                }
                Marshal.FreeHGlobal(resultPtr);
                return resultCode;
            }

            private MLResult.Code InternalGetSupportedMimeTypes(OutputFormat format, out string[] mimeResults)
            {
                var resultCode = NativeBindings.MLMediaMuxerGetSupportedMimes(handle, format, out IntPtr resultsPtr, out int resultsCount);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaMuxerGetSupportedMimes));
                mimeResults = new string[resultsCount];
                if (resultCode == MLResult.Code.Ok)
                {
                    IntPtr[] pIntPtrArray = new IntPtr[resultsCount];
                    Marshal.Copy(resultsPtr, pIntPtrArray, 0, resultsCount);
                    for (int i = 0; i < resultsCount; i++)
                    {
                        mimeResults[i] = Marshal.PtrToStringAnsi(pIntPtrArray[i]);
                    }
                }
                Marshal.FreeCoTaskMem(resultsPtr);
                return resultCode;
            }

            private MLResult.Code InternalAddTrack(ulong mediaFormatHandle, out int trackIndex)
            {
                var resultCode = NativeBindings.MLMediaMuxerAddTrack(handle, mediaFormatHandle, out UIntPtr indexPtr);
                trackIndex = (int)indexPtr;
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaMuxerAddTrack));
                return resultCode;
            }

            private MLResult.Code InternalWriteSampleData(int trackIndex, byte[] buffer, long time, CodecBufferFlags flags)
            {
                var sampleData = NativeBindings.MLMediaMuxerSampleData.Create();
                sampleData.TrackIndex = (uint)trackIndex;
                sampleData.Buffer = buffer;
                sampleData.Size = (buffer == null) ? 0 : (uint)buffer.Length;
                sampleData.Time = (ulong)time;
                sampleData.Flags = flags;

                var resultCode = NativeBindings.MLMediaMuxerWriteSampleData(handle, sampleData);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaMuxerWriteSampleData));
                return resultCode;
            }
        }
    }
}
