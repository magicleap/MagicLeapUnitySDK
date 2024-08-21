// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap.Native
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.XR.MagicLeap;
    using Unity.Jobs;
    using Unity.Collections;

    /// <summary>
    /// Class containing utility functions to convert Unity Texture to <c>Magic Leap</c> OS compatible data structures.
    /// </summary>
    public static class MLTextureUtils
    {
        /// <summary>
        /// Convert Unity Texture2D to a byte array.
        /// Texture2D should be in one of the following formats: DXT1, RGBA32, RGB24.
        /// For DXT1 and RGBA32, resulting byte array is in RGBA format.
        /// For RGB24, resulting byte array is in RGB format.
        /// The origin of the byte array will be at the top left corner.
        /// Returns null on unsupported formats.
        /// </summary>
        /// <param name="texture">Texture to extract byte array from</param>
        /// <param name="numChannels">Out parameter to determine how many bytes per pixel</param>
        /// <returns>Returns an array of bytes that holds the converted texture</returns>
        public static byte[] ConvertToByteArray(Texture2D texture, out int numChannels)
        {
            byte[] encodedImage = null;
            numChannels = 0;

            // [1] Convert the unity texture to RGBA pixel format (if needed).
            // For a pixel at (x,y) the channel c data of that pixels is at the position
            // given by [(width * y + x)*channels + c].
            if (TextureFormat.DXT1 == texture.format)
            {
                Color32[] colorArray = texture.GetPixels32();
                numChannels = Marshal.SizeOf(typeof(Color32));
                int totalBytes = numChannels * colorArray.Length;
                encodedImage = new byte[totalBytes];
                GCHandle handle = default(GCHandle);
                try
                {
                    handle = GCHandle.Alloc(colorArray, GCHandleType.Pinned);
                    IntPtr ptr = handle.AddrOfPinnedObject();
                    Marshal.Copy(ptr, encodedImage, 0, totalBytes);
                }
                finally
                {
                    if (handle != default(GCHandle))
                    {
                        handle.Free();
                    }
                }
            }
            else if (TextureFormat.RGBA32 == texture.format)
            {
                numChannels = 4;
                encodedImage = texture.GetRawTextureData();
            }
            else if (TextureFormat.RGB24 == texture.format)
            {
                numChannels = 3;
                encodedImage = texture.GetRawTextureData();
            }
            else
            {
                MLPluginLog.Error("MLTextureUtils.ConvertToByteArray failed to convert to byte array. Reason: unsupported format. Use RGBA32, RGB24 or DXT1 format when importing textures.");
                return encodedImage;
            }

            // [2] Convert to coorindate frame used by Magic Leap OS
            // Unity texture origin in bottom left, but Magic Leap OS is expecting top left to be the origin.
            int rowLength = texture.width * numChannels;
            var rowTemp = new byte[rowLength];
            for (var i = 0; i < texture.height / 2; i++)
            {
                Buffer.BlockCopy(encodedImage, i * rowLength, rowTemp, 0, rowLength);
                Buffer.BlockCopy(encodedImage, (texture.height - i - 1) * rowLength, encodedImage, i * rowLength, rowLength);
                Buffer.BlockCopy(rowTemp, 0, encodedImage, (texture.height - i - 1) * rowLength, rowLength);
            }

            return encodedImage;
        }

        /// <summary>
        /// Performs a vertical flip on a byte array of image pixel data. Each pixel is assumed to be 4 bytes.
        /// </summary>
        /// <param name="originalPixels">The pixels array of the image to be flipped. The array length is assumed to be 4 * width * height.</param>
        /// <param name="width">The number of the pixels wide of the image from the data.</param>
        /// <param name="height">The number of the pixels high of the image from the data.</param>
        /// <returns>A new byte array arranged vertically flipped from the input array of pixels.</returns> 
        /// </summary>
        public static byte[] FlipPixelsVertically(byte[] originalPixels, int width, int height)
        {
            NativeArray<byte> newPixelsArray = new NativeArray<byte>(originalPixels.Length, Allocator.Temp);
            NativeArray<byte> originalArrayCopy = new NativeArray<byte>(originalPixels.Length, Allocator.Temp);
            CopyFromRawBytes<byte>(originalArrayCopy, originalPixels);

            new FlipPixelsVerticallyJob
            {
                InputArray = originalArrayCopy,
                Width = width,
                Height = height,
                OutputArray = newPixelsArray
            }.Schedule().Complete();

            return ToRawBytes<byte>(newPixelsArray);
        }

        private struct FlipPixelsVerticallyJob : IJob
        {
            [ReadOnly] public NativeArray<byte> InputArray;
            [ReadOnly] public int Width;
            [ReadOnly] public int Height;
            [WriteOnly] public NativeArray<byte> OutputArray;

            public void Execute()
            {
                for (var x = 0; x < Width; x++)
                {
                    for (var y = 0; y < Height; y++)
                    {
                        OutputArray[(x + y * Width) * 4] = InputArray[(x + (Height - y - 1) * Width) * 4];
                        OutputArray[(x + y * Width) * 4 + 1] = InputArray[(x + (Height - y - 1) * Width) * 4 + 1];
                        OutputArray[(x + y * Width) * 4 + 2] = InputArray[(x + (Height - y - 1) * Width) * 4 + 2];
                        OutputArray[(x + y * Width) * 4 + 3] = InputArray[(x + (Height - y - 1) * Width) * 4 + 3];
                    }
                }
            }
        }

        private static byte[] ToRawBytes<T>(this NativeArray<T> arr) where T : struct
            => arr.Reinterpret<byte>().ToArray();

        private static void CopyFromRawBytes<T>(this NativeArray<T> arr, byte[] bytes) where T : struct
            => arr.Reinterpret<byte>().CopyFrom(bytes);

        internal struct Color2x2
        {
            public Color topLeft;
            public Color topRight;
            public Color bottomLeft;
            public Color bottomRight;
        }

        internal static Color BilinearFilter(in Color2x2 inSample, float x, float y)
        {
            var top = Color.Lerp(inSample.topLeft, inSample.topRight, x);
            var bottom = Color.Lerp(inSample.bottomLeft, inSample.bottomRight, x);
            return Color.Lerp(top, bottom, y);
        }

        private static Color GetColorUnchecked(this NativeArray<Color> self, int stride, int x, int y)
            => self[stride * y + x];

        private static void SetColorUnchecked(this NativeArray<Color> self, int stride, int x, int y, Color newColor)
            => self[stride * y + x] = newColor;

        internal static Color2x2 ExtractColor2x2(this NativeArray<Color> self, int width, int height, int x, int y)
        {
            bool IsInRange(int min, int max, int t)
                => t >= min && t <= max;

            if (width < 2)
                throw new ArgumentOutOfRangeException($"{nameof(width)} must be at least 2");
            if (height < 2)
                throw new ArgumentOutOfRangeException($"{nameof(height)} must be at least 2");
            if (!IsInRange(0, width - 2, x))
                throw new ArgumentOutOfRangeException($"{nameof(x)} must be between 0 and ${nameof(width)} - 1, exclusive");
            if (!IsInRange(0, height - 2, y))
                throw new ArgumentOutOfRangeException($"{nameof(y)} must be between 0 and ${nameof(height)} - 1, exclusive");

            return self.ExtractColor2x2Unchecked(width, x, y);
        }

        internal static Color2x2 ExtractColor2x2Unchecked(this NativeArray<Color> self, int width, int x, int y)
            => new Color2x2{
                topLeft = self.GetColorUnchecked(width, x, y),
                topRight = self.GetColorUnchecked(width, x + 1, y),
                bottomLeft = self.GetColorUnchecked(width, x, y + 1),
                bottomRight = self.GetColorUnchecked(width, x + 1, y + 1),
            };

        /// <summary>
        /// NOTE: This job requires the source stride to be 2x the destination
        /// stride; likewise, the source array must be 4x the size of the destination
        /// array.
        /// </summary>
        internal struct BilinearFilteringDownSamplingJob : IJobParallelFor
        {
            private struct CoordinateMap
            {
                public Vector2Int Destination;
                public Vector2Int Source;

                public CoordinateMap(int index, int destStride)
                {
                    var destY = index / destStride;
                    var destX = index - destY;
                    var srcY = destY * 2;
                    var srcX = destX * 2;
                    Destination = new Vector2Int(destX, destY);
                    Source = new Vector2Int(srcX, srcY);
                }
            }

            [NativeDisableParallelForRestriction]
            [ReadOnly]
            public NativeArray<Color> Source;

            [WriteOnly]
            public NativeArray<Color> Destination;

            public int SourceStride;
            public int DestinationStride;

            public void Execute(int index)
            {
                var map = new CoordinateMap(index, DestinationStride);
                var block = Source.ExtractColor2x2Unchecked(SourceStride, map.Source.x, map.Source.y);
                var c = BilinearFilter(block, 0.5f, 0.5f);
                Destination.SetColorUnchecked(DestinationStride, map.Destination.x, map.Destination.y, c);
            }
        }
    }
}
