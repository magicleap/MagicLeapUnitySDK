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
    /// MLMedia APIs.
    /// </summary>
    public partial class MLMedia
    {
        /// <summary>
        /// APIs for MediaMuxer to facilitate muxing of elementary streams.
        /// </summary>
        public sealed partial class Muxer
        {
            /// <summary>
            /// Output formats for MediaMuxer.<br/> 
            /// Use <see cref="Muxer.GetSupportedOutputFormats(out OutputFormat[])"/> to query for all supported output formats for the Muxer.
            /// </summary>
            public enum OutputFormat
            {
                /// <summary>
                /// MP4 format
                /// </summary>
                MPEG4 = 0,

                /// <summary>
                /// VP8/VORBIS data in a WEBM container
                /// </summary>
                WEBM = 1,

                /// <summary>
                /// 3gpp format
                /// </summary>
                ThreeGPP = 2,

                /// <summary>
                /// HEIC data in a HEIF container
                /// </summary>
                HEIF = 3,

                /// <summary>
                /// Opus audio data in an OGG container
                /// </summary>
                OGG = 4
            }

            /// <summary>
            /// Indicate the type of data when queuing the input buffer of muxer sample data
            /// </summary>
            [Flags]
            public enum CodecBufferFlags
            {
                /// <summary>
                /// The (encoded) buffer contains the data for a key frame.
                /// </summary>
                KeyFrame = 1,

                /// <summary>
                /// Signals the end of stream, i.e. no buffers will be available after this.
                /// </summary>
                EOS = 4,

                /// <summary>
                /// Buffer contains muxer data - Supported only for Exif data block.
                /// </summary>
                MuxerData = 16
            }

            private ulong handle = MagicLeapNativeBindings.InvalidHandle;

            public Muxer()
            {
                var result = NativeBindings.MLMediaMuxerCreate(out handle);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLMediaMuxerCreate));
            }

            ~Muxer()
            {
                var result = NativeBindings.MLMediaMuxerRelease(handle);
                MLResult.DidNativeCallSucceed(result, nameof(NativeBindings.MLMediaMuxerRelease));
            }

            /// <summary>
            /// Configure the <see cref="Muxer"/> for a given output format with output path.
            /// </summary>
            /// <param name="format">The format of the muxed output media file. Value should be one returned from <see cref="GetSupportedOutputFormats"/></param>
            /// <param name="filePath"></param>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult Configure(OutputFormat format, string filePath)
            {
                var resultCode = NativeBindings.MLMediaMuxerConfigure(handle, (int)format, filePath);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaMuxerConfigure));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Start muxing. Make sure all the tracks have been added (<see cref="AddAudioTrack"/> or <see cref="AddVideoTrack"/>) before calling this.<br/>
            /// If the Muxer has already been <c>Stopped</c>, it cannot be re-<c>Started</c>.
            /// </summary>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult Start()
            {
                var resultCode = NativeBindings.MLMediaMuxerStart(handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaMuxerStart));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Stop muxing. Once the muxer <c>Stops</c>, it can not be re-<c>Started</c>.
            /// </summary>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult Stop()
            {
                var resultCode = NativeBindings.MLMediaMuxerStop(handle);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaMuxerStop));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Get a list of all the supported Media Muxer Output Formats.
            /// </summary>
            /// <param name="results">Array of <see cref="OutputFormat"/> that will contain the supported formats.</param>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult GetSupportedOutputFormats(out OutputFormat[] results) => MLResult.Create(InternalGetSupportedOutputFormats(out results));

            /// <summary>
            /// Get a list of all the supported mime-types for a given Media Muxer Output Format.
            /// </summary>
            /// <param name="format"><see cref="OutputFormat"/> for which to retrieve supported MIME types.</param>
            /// <param name="results">Array that will contain the supported MIME type names.</param>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult GetSupportedMimes(OutputFormat format, out string[] results) => MLResult.Create(InternalGetSupportedMimeTypes(format, out results));

            /// <summary>
            /// Add a new audio track. This should be called before <see cref="Start"/> and after <see cref="Configure"/>.<br/><br/>
            /// <see cref="GetSupportedMimes"/> can be used to query for all the supported MIME types for a given audio output format.
            /// </summary>
            /// <param name="mimeType">The audio content's MIME type.</param>
            /// <param name="sampleRate">The audio's sample rate.</param>
            /// <param name="channels">The number of audio channels.</param>
            /// <param name="addedTrackIndex">Index of the added audio track.</param>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult AddAudioTrack(string mimeType, int sampleRate, int channels, out int addedTrackIndex)
            {
                MLMediaFormat mediaFormat = MLMediaFormat.CreateAudio(mimeType, sampleRate, channels);
                addedTrackIndex = -1;
                return (mediaFormat != null) ? MLResult.Create(InternalAddTrack(mediaFormat.Handle, out addedTrackIndex)) : MLResult.Create(MLResult.Code.UnspecifiedFailure);
            }

            /// <summary>
            /// Add a new video track. This should be called before <see cref="Start"/> and after <see cref="Configure"/>.<br/><br/>
            /// <see cref="GetSupportedMimes"/> can be used to query for all the supported MIME types for a given video output format.
            /// </summary>
            /// <param name="mimeType">The video content's MIME type.</param>
            /// <param name="width">The video content's width in pixels.</param>
            /// <param name="height">The video content's height in pixels.</param>
            /// <param name="addedTrackIndex">Index of the added video track.</param>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult AddVideoTrack(string mimeType, int width, int height, out int addedTrackIndex)
            {
                MLMediaFormat mediaFormat = MLMediaFormat.CreateVideo(mimeType, width, height);
                addedTrackIndex = -1;
                return (mediaFormat != null) ? MLResult.Create(InternalAddTrack(mediaFormat.Handle, out addedTrackIndex)) : MLResult.Create(MLResult.Code.UnspecifiedFailure);
            }

            /// <summary>
            /// Set the orientation hint for output video playback.<br/><br/>
            /// This should be called before <see cref="Start"/> and after <see cref="Configure"/><br/><br/>
            /// Calling this method will not rotate the video frame when muxer is generating the file,
            ///	but add a composition matrix containing the rotation angle in the output video
            /// if the output format is <see cref="OutputFormat.MPEG4"/> so that a video player can choose
            /// the proper orientation for playback.<br/><br/>
            /// Note that some video players may choose to ignore the composition matrix
            /// in a video during playback. By default, the rotation degree is 0.
            /// </summary>
            /// <param name="degrees">The rotation angle. Must be 0, 90, 180 or 270.</param>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult SetOrientationHint(int degrees)
            {
                var resultCode = NativeBindings.MLMediaMuxerSetOrientationHint(handle, degrees);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaMuxerSetOrientationHint));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Set and store the geodata (latitude and longitude) in the output file.<br/><br/>
            /// This should be called before <see cref="Start"/> and after <see cref="Configure"/>.<br/>
            /// The geodata is stored in udta box if the output format is <see cref="OutputFormat.MPEG4"/>,<br/>
            /// and is ignored for other output formats.<br/><br/>
            /// The geodata is stored according to ISO-6709 standard.
            /// </summary>
            /// <param name="latitude">The latitude in degrees. Its value must be in the range [-90, 90].</param>
            /// <param name="longitude">The longitude in degrees. Its value must be in the range [-180, 180].</param>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult SetLocation(float latitude, float longitude)
            {
                var resultCode = NativeBindings.MLMediaMuxerSetLocation(handle, latitude, longitude);
                MLResult.DidNativeCallSucceed(resultCode, nameof(NativeBindings.MLMediaMuxerSetLocation));
                return MLResult.Create(resultCode);
            }

            /// <summary>
            /// Writes an encoded sample into the muxer.<br/><br/>
            /// The application needs to make sure that the samples are written into the right tracks.<br/>
            /// Also, it needs to make sure the samples for each track are written in chronological order<br/>
            /// (e.g. in the order they are provided by the encoder.)<br/>
            /// </summary>
            /// <param name="trackIndex">The track index number to write the sample data into. This should be one of the number returned by <see cref="AddAudioTrack"/> and <see cref="AddVideoTrack"/></param>
            /// <param name="data">Buffer of data to write to the muxer</param>
            /// <param name="time">Timestamp in microseconds</param>
            /// <param name="flags"><c>MLMediaCodecBufferFlag</c>. Supported values: <c>KeyFrame</c>, <c>EOS</c>, and <c>MuxerData</c></param>
            /// <returns><see cref="MLResult"/> with Code "Ok" if the operation completed successfully without errors. Check <see cref="MLResult.IsOk"/> for status.</returns>
            public MLResult WriteSampleData(int trackIndex, byte[] data, long time, CodecBufferFlags flags) => MLResult.Create(InternalWriteSampleData(trackIndex, data, time, flags));
        }
    }
}
