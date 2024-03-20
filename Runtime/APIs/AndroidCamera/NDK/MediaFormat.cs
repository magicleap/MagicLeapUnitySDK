using System;

namespace MagicLeap.Android.NDK.Media
{
    public enum MediaFormat
    {
        /// <summary>
        /// 32 bits RGBA format, 8 bits for each of the four channels.
        ///
        /// <p>
        /// Corresponding formats:
        /// <ul>
        /// <li>AHardwareBuffer: AHARDWAREBUFFER_FORMAT_R8G8B8A8_UNORM</li>
        /// <li>Vulkan: VK_FORMAT_R8G8B8A8_UNORM</li>
        /// <li>OpenGL ES: GL_RGBA8</li>
        /// </ul>
        /// </p>
        ///
        /// @see AImage
        /// @see AImageReader
        /// @see AHardwareBuffer
        /// </summary>
        Rgba8888         = 0x1,

        /// <summary>
        /// 32 bits RGBX format, 8 bits for each of the four channels.  The values
        /// of the alpha channel bits are ignored (image is assumed to be opaque).
        ///
        /// <p>
        /// Corresponding formats:
        /// <ul>
        /// <li>AHardwareBuffer: AHARDWAREBUFFER_FORMAT_R8G8B8X8_UNORM</li>
        /// <li>Vulkan: VK_FORMAT_R8G8B8A8_UNORM</li>
        /// <li>OpenGL ES: GL_RGB8</li>
        /// </ul>
        /// </p>
        ///
        /// @see AImage
        /// @see AImageReader
        /// @see AHardwareBuffer
        /// </summary>
        Rgbx8888         = 0x2,

        /// <summary>
        /// 24 bits RGB format, 8 bits for each of the three channels.
        ///
        /// <p>
        /// Corresponding formats:
        /// <ul>
        /// <li>AHardwareBuffer: AHARDWAREBUFFER_FORMAT_R8G8B8_UNORM</li>
        /// <li>Vulkan: VK_FORMAT_R8G8B8_UNORM</li>
        /// <li>OpenGL ES: GL_RGB8</li>
        /// </ul>
        /// </p>
        ///
        /// @see AImage
        /// @see AImageReader
        /// @see AHardwareBuffer
        /// </summary>
        Rgb888           = 0x3,

        /// <summary>
        /// 16 bits RGB format, 5 bits for Red channel, 6 bits for Green channel,
        /// and 5 bits for Blue channel.
        ///
        /// <p>
        /// Corresponding formats:
        /// <ul>
        /// <li>AHardwareBuffer: AHARDWAREBUFFER_FORMAT_R5G6B5_UNORM</li>
        /// <li>Vulkan: VK_FORMAT_R5G6B5_UNORM_PACK16</li>
        /// <li>OpenGL ES: GL_RGB565</li>
        /// </ul>
        /// </p>
        ///
        /// @see AImage
        /// @see AImageReader
        /// @see AHardwareBuffer
        /// </summary>
        Rgb565           = 0x4,

        /// <summary>
        /// 64 bits RGBA format, 16 bits for each of the four channels.
        ///
        /// <p>
        /// Corresponding formats:
        /// <ul>
        /// <li>AHardwareBuffer: AHARDWAREBUFFER_FORMAT_R16G16B16A16_FLOAT</li>
        /// <li>Vulkan: VK_FORMAT_R16G16B16A16_SFLOAT</li>
        /// <li>OpenGL ES: GL_RGBA16F</li>
        /// </ul>
        /// </p>
        ///
        /// @see AImage
        /// @see AImageReader
        /// @see AHardwareBuffer
        /// </summary>
        Rgba_Fp16         = 0x16,

        /// <summary>
        /// Multi-plane Android YUV 420 format.
        ///
        /// <p>This format is a generic YCbCr format, capable of describing any 4:2:0
        /// chroma-subsampled planar or semiplanar buffer (but not fully interleaved),
        /// with 8 bits per color sample.</p>
        ///
        /// <p>Images in this format are always represented by three separate buffers
        /// of data, one for each color plane. Additional information always
        /// accompanies the buffers, describing the row stride and the pixel stride
        /// for each plane.</p>
        ///
        /// <p>The order of planes is guaranteed such that plane #0 is always Y, plane #1 is always
        /// U (Cb), and plane #2 is always V (Cr).</p>
        ///
        /// <p>The Y-plane is guaranteed not to be interleaved with the U/V planes
        /// (in particular, pixel stride is always 1 in {@link AImage_getPlanePixelStride}).</p>
        ///
        /// <p>The U/V planes are guaranteed to have the same row stride and pixel stride, that is, the
        /// return value of {@link AImage_getPlaneRowStride} for the U/V plane are guaranteed to be the
        /// same, and the return value of {@link AImage_getPlanePixelStride} for the U/V plane are also
        /// guaranteed to be the same.</p>
        ///
        /// <p>For example, the {@link AImage} object can provide data
        /// in this format from a {@link ACameraDevice} through an {@link AImageReader} object.</p>
        ///
        /// <p>This format is always supported as an output format for the android Camera2 NDK API.</p>
        ///
        /// @see AImage
        /// @see AImageReader
        /// @see ACameraDevice
        /// </summary>
        Yuv_420_888       = 0x23,

        /// <summary>
        /// Compressed JPEG format.
        ///
        /// <p>This format is always supported as an output format for the android Camera2 NDK API.</p>
        /// </summary>
        Jpeg              = 0x100,

        /// <summary>
        /// 16 bits per pixel raw camera sensor image format, usually representing a single-channel
        /// Bayer-mosaic image.
        ///
        /// <p>The layout of the color mosaic, the maximum and minimum encoding
        /// values of the raw pixel data, the color space of the image, and all other
        /// needed information to interpret a raw sensor image must be queried from
        /// the {@link ACameraDevice} which produced the image.</p>
        /// </summary>
        Raw16             = 0x20,

        /// <summary>
        /// Private raw camera sensor image format, a single channel image with implementation depedent
        /// pixel layout.
        ///
        /// <p>AIMAGE_FORMAT_RAW_PRIVATE is a format for unprocessed raw image buffers coming from an
        /// image sensor. The actual structure of buffers of this format is implementation-dependent.</p>
        ///
        /// </summary>
        RawPrivate       = 0x24,

        /// <summary>
        /// Android 10-bit raw format.
        ///
        /// <p>
        /// This is a single-plane, 10-bit per pixel, densely packed (in each row),
        /// unprocessed format, usually representing raw Bayer-pattern images coming
        /// from an image sensor.
        /// </p>
        /// <p>
        /// In an image buffer with this format, starting from the first pixel of
        /// each row, each 4 consecutive pixels are packed into 5 bytes (40 bits).
        /// Each one of the first 4 bytes contains the top 8 bits of each pixel, The
        /// fifth byte contains the 2 least significant bits of the 4 pixels, the
        /// exact layout data for each 4 consecutive pixels is illustrated below
        /// (Pi[j] stands for the jth bit of the ith pixel):
        /// </p>
        /// <table>
        /// <tr>
        /// <th align="center"></th>
        /// <th align="center">bit 7</th>
        /// <th align="center">bit 6</th>
        /// <th align="center">bit 5</th>
        /// <th align="center">bit 4</th>
        /// <th align="center">bit 3</th>
        /// <th align="center">bit 2</th>
        /// <th align="center">bit 1</th>
        /// <th align="center">bit 0</th>
        /// </tr>
        /// <tr>
        /// <td align="center">Byte 0:</td>
        /// <td align="center">P0[9]</td>
        /// <td align="center">P0[8]</td>
        /// <td align="center">P0[7]</td>
        /// <td align="center">P0[6]</td>
        /// <td align="center">P0[5]</td>
        /// <td align="center">P0[4]</td>
        /// <td align="center">P0[3]</td>
        /// <td align="center">P0[2]</td>
        /// </tr>
        /// <tr>
        /// <td align="center">Byte 1:</td>
        /// <td align="center">P1[9]</td>
        /// <td align="center">P1[8]</td>
        /// <td align="center">P1[7]</td>
        /// <td align="center">P1[6]</td>
        /// <td align="center">P1[5]</td>
        /// <td align="center">P1[4]</td>
        /// <td align="center">P1[3]</td>
        /// <td align="center">P1[2]</td>
        /// </tr>
        /// <tr>
        /// <td align="center">Byte 2:</td>
        /// <td align="center">P2[9]</td>
        /// <td align="center">P2[8]</td>
        /// <td align="center">P2[7]</td>
        /// <td align="center">P2[6]</td>
        /// <td align="center">P2[5]</td>
        /// <td align="center">P2[4]</td>
        /// <td align="center">P2[3]</td>
        /// <td align="center">P2[2]</td>
        /// </tr>
        /// <tr>
        /// <td align="center">Byte 3:</td>
        /// <td align="center">P3[9]</td>
        /// <td align="center">P3[8]</td>
        /// <td align="center">P3[7]</td>
        /// <td align="center">P3[6]</td>
        /// <td align="center">P3[5]</td>
        /// <td align="center">P3[4]</td>
        /// <td align="center">P3[3]</td>
        /// <td align="center">P3[2]</td>
        /// </tr>
        /// <tr>
        /// <td align="center">Byte 4:</td>
        /// <td align="center">P3[1]</td>
        /// <td align="center">P3[0]</td>
        /// <td align="center">P2[1]</td>
        /// <td align="center">P2[0]</td>
        /// <td align="center">P1[1]</td>
        /// <td align="center">P1[0]</td>
        /// <td align="center">P0[1]</td>
        /// <td align="center">P0[0]</td>
        /// </tr>
        /// </table>
        /// <p>
        /// This format assumes
        /// <ul>
        /// <li>a width multiple of 4 pixels</li>
        /// <li>an even height</li>
        /// </ul>
        /// </p>
        ///
        /// <pre>size = row stride * height</pre> where the row stride is in <em>bytes</em>,
        /// not pixels.
        ///
        /// <p>
        /// Since this is a densely packed format, the pixel stride is always 0. The
        /// application must use the pixel data layout defined in above table to
        /// access each row data. When row stride is equal to (width * (10 / 8)), there
        /// will be no padding bytes at the end of each row, the entire image data is
        /// densely packed. When stride is larger than (width * (10 / 8)), padding
        /// bytes will be present at the end of each row.
        /// </p>
        /// <p>
        /// For example, the {@link AImage} object can provide data in this format from a
        /// {@link ACameraDevice} (if supported) through a {@link AImageReader} object.
        /// The number of planes returned by {@link AImage_getNumberOfPlanes} will always be 1.
        /// The pixel stride is undefined ({@link AImage_getPlanePixelStride} will return
        /// {@link AMEDIA_ERROR_UNSUPPORTED}), and the {@link AImage_getPlaneRowStride} described the
        /// vertical neighboring pixel distance (in bytes) between adjacent rows.
        /// </p>
        ///
        /// @see AImage
        /// @see AImageReader
        /// @see ACameraDevice
        /// </summary>
        Raw10             = 0x25,

        /// <summary>
        /// Android 12-bit raw format.
        ///
        /// <p>
        /// This is a single-plane, 12-bit per pixel, densely packed (in each row),
        /// unprocessed format, usually representing raw Bayer-pattern images coming
        /// from an image sensor.
        /// </p>
        /// <p>
        /// In an image buffer with this format, starting from the first pixel of each
        /// row, each two consecutive pixels are packed into 3 bytes (24 bits). The first
        /// and second byte contains the top 8 bits of first and second pixel. The third
        /// byte contains the 4 least significant bits of the two pixels, the exact layout
        /// data for each two consecutive pixels is illustrated below (Pi[j] stands for
        /// the jth bit of the ith pixel):
        /// </p>
        /// <table>
        /// <tr>
        /// <th align="center"></th>
        /// <th align="center">bit 7</th>
        /// <th align="center">bit 6</th>
        /// <th align="center">bit 5</th>
        /// <th align="center">bit 4</th>
        /// <th align="center">bit 3</th>
        /// <th align="center">bit 2</th>
        /// <th align="center">bit 1</th>
        /// <th align="center">bit 0</th>
        /// </tr>
        /// <tr>
        /// <td align="center">Byte 0:</td>
        /// <td align="center">P0[11]</td>
        /// <td align="center">P0[10]</td>
        /// <td align="center">P0[ 9]</td>
        /// <td align="center">P0[ 8]</td>
        /// <td align="center">P0[ 7]</td>
        /// <td align="center">P0[ 6]</td>
        /// <td align="center">P0[ 5]</td>
        /// <td align="center">P0[ 4]</td>
        /// </tr>
        /// <tr>
        /// <td align="center">Byte 1:</td>
        /// <td align="center">P1[11]</td>
        /// <td align="center">P1[10]</td>
        /// <td align="center">P1[ 9]</td>
        /// <td align="center">P1[ 8]</td>
        /// <td align="center">P1[ 7]</td>
        /// <td align="center">P1[ 6]</td>
        /// <td align="center">P1[ 5]</td>
        /// <td align="center">P1[ 4]</td>
        /// </tr>
        /// <tr>
        /// <td align="center">Byte 2:</td>
        /// <td align="center">P1[ 3]</td>
        /// <td align="center">P1[ 2]</td>
        /// <td align="center">P1[ 1]</td>
        /// <td align="center">P1[ 0]</td>
        /// <td align="center">P0[ 3]</td>
        /// <td align="center">P0[ 2]</td>
        /// <td align="center">P0[ 1]</td>
        /// <td align="center">P0[ 0]</td>
        /// </tr>
        /// </table>
        /// <p>
        /// This format assumes
        /// <ul>
        /// <li>a width multiple of 4 pixels</li>
        /// <li>an even height</li>
        /// </ul>
        /// </p>
        ///
        /// <pre>size = row stride * height</pre> where the row stride is in <em>bytes</em>,
        /// not pixels.
        ///
        /// <p>
        /// Since this is a densely packed format, the pixel stride is always 0. The
        /// application must use the pixel data layout defined in above table to
        /// access each row data. When row stride is equal to (width * (12 / 8)), there
        /// will be no padding bytes at the end of each row, the entire image data is
        /// densely packed. When stride is larger than (width * (12 / 8)), padding
        /// bytes will be present at the end of each row.
        /// </p>
        /// <p>
        /// For example, the {@link AImage} object can provide data in this format from a
        /// {@link ACameraDevice} (if supported) through a {@link AImageReader} object.
        /// The number of planes returned by {@link AImage_getNumberOfPlanes} will always be 1.
        /// The pixel stride is undefined ({@link AImage_getPlanePixelStride} will return
        /// {@link AMEDIA_ERROR_UNSUPPORTED}), and the {@link AImage_getPlaneRowStride} described the
        /// vertical neighboring pixel distance (in bytes) between adjacent rows.
        /// </p>
        ///
        /// @see AImage
        /// @see AImageReader
        /// @see ACameraDevice
        /// </summary>
        Raw12             = 0x26,

        /// <summary>
        /// Android dense depth image format.
        ///
        /// <p>Each pixel is 16 bits, representing a depth ranging measurement from a depth camera or
        /// similar sensor. The 16-bit sample consists of a confidence value and the actual ranging
        /// measurement.</p>
        ///
        /// <p>The confidence value is an estimate of correctness for this sample.  It is encoded in the
        /// 3 most significant bits of the sample, with a value of 0 representing 100% confidence, a
        /// value of 1 representing 0% confidence, a value of 2 representing 1/7, a value of 3
        /// representing 2/7, and so on.</p>
        ///
        /// <p>As an example, the following sample extracts the range and confidence from the first pixel
        /// of a DEPTH16-format {@link AImage}, and converts the confidence to a floating-point value
        /// between 0 and 1.f inclusive, with 1.f representing maximum confidence:
        ///
        /// <pre>
        ///    uint16_t* data;
        ///    int dataLength;
        ///    AImage_getPlaneData(image, 0, (uint8_t**)&data, &dataLength);
        ///    uint16_t depthSample = data[0];
        ///    uint16_t depthRange = (depthSample & 0x1FFF);
        ///    uint16_t depthConfidence = ((depthSample >> 13) & 0x7);
        ///    float depthPercentage = depthConfidence == 0 ? 1.f : (depthConfidence - 1) / 7.f;
        /// </pre>
        /// </p>
        ///
        /// <p>This format assumes
        /// <ul>
        /// <li>an even width</li>
        /// <li>an even height</li>
        /// <li>a horizontal stride multiple of 16 pixels</li>
        /// </ul>
        /// </p>
        ///
        /// <pre> y_size = stride * height </pre>
        ///
        /// When produced by a camera, the units for the range are millimeters.
        /// </summary>
        Depth16           = 0x44363159,

        /// <summary>
        /// Android sparse depth point cloud format.
        ///
        /// <p>A variable-length list of 3D points plus a confidence value, with each point represented
        /// by four floats; first the X, Y, Z position coordinates, and then the confidence value.</p>
        ///
        /// <p>The number of points is ((size of the buffer in bytes) / 16).
        ///
        /// <p>The coordinate system and units of the position values depend on the source of the point
        /// cloud data. The confidence value is between 0.f and 1.f, inclusive, with 0 representing 0%
        /// confidence and 1.f representing 100% confidence in the measured position values.</p>
        ///
        /// <p>As an example, the following code extracts the first depth point in a DEPTH_POINT_CLOUD
        /// format {@link AImage}:
        /// <pre>
        ///    float* data;
        ///    int dataLength;
        ///    AImage_getPlaneData(image, 0, (uint8_t**)&data, &dataLength);
        ///    float x = data[0];
        ///    float y = data[1];
        ///    float z = data[2];
        ///    float confidence = data[3];
        /// </pre>
        ///
        /// </summary>
        DepthPointCloud = 0x101,

        /// <summary>
        /// Android private opaque image format.
        ///
        /// <p>The choices of the actual format and pixel data layout are entirely up to the
        /// device-specific and framework internal implementations, and may vary depending on use cases
        /// even for the same device. Also note that the contents of these buffers are not directly
        /// accessible to the application.</p>
        ///
        /// <p>When an {@link AImage} of this format is obtained from an {@link AImageReader} or
        /// {@link AImage_getNumberOfPlanes()} method will return zero.</p>
        /// </summary>
        Private           = 0x22,

        /// <summary>
        /// Android Y8 format.
        ///
        /// <p>Y8 is a planar format comprised of a WxH Y plane only, with each pixel
        /// being represented by 8 bits.</p>
        ///
        /// <p>This format assumes
        /// <ul>
        /// <li>an even width</li>
        /// <li>an even height</li>
        /// <li>a horizontal stride multiple of 16 pixels</li>
        /// </ul>
        /// </p>
        ///
        /// <pre> size = stride * height </pre>
        ///
        /// <p>For example, the {@link AImage} object can provide data
        /// in this format from a {@link ACameraDevice} (if supported) through a
        /// {@link AImageReader} object. The number of planes returned by
        /// {@link AImage_getNumberOfPlanes} will always be 1. The pixel stride returned by
        /// {@link AImage_getPlanePixelStride} will always be 1, and the
        /// {@link AImage_getPlaneRowStride} described the vertical neighboring pixel distance
        /// (in bytes) between adjacent rows.</p>
        ///
        /// </summary>
        Y8 = 0x20203859,

        /// <summary>
        /// Compressed HEIC format.
        ///
        /// <p>This format defines the HEIC brand of High Efficiency Image File
        /// Format as described in ISO/IEC 23008-12.</p>
        /// </summary>
        Heic = 0x48454946,

        /// <summary>
        /// Depth augmented compressed JPEG format.
        ///
        /// <p>JPEG compressed main image along with XMP embedded depth metadata
        /// following ISO 16684-1:2011(E).</p>
        /// </summary>
        DepthJpeg = 0x69656963,
    }

    public static class MediaFormatExtensions
    {
        public static int BytesPerPixel(this MediaFormat format)
        {
            switch (format)
            {
                case MediaFormat.Rgba8888:
                    return 4;
                case MediaFormat.Rgbx8888:
                    return 4;
                case MediaFormat.Rgb888:
                    return 3;
                case MediaFormat.Rgb565:
                    return 2;
                case MediaFormat.Rgba_Fp16:
                    return 2;
                case MediaFormat.Yuv_420_888:
                    return 1;
                case MediaFormat.Jpeg:
                    return 1;
                case MediaFormat.Raw16:
                    return 2;
                case MediaFormat.RawPrivate:
                    return 1;
                case MediaFormat.Raw10:
                    return 2;
                case MediaFormat.Raw12:
                    return 2;
                case MediaFormat.Depth16:
                    return 2;
                case MediaFormat.DepthPointCloud:
                    return 16;
                case MediaFormat.Private:
                    return 1;
                case MediaFormat.Y8:
                    return 1;
                case MediaFormat.Heic:
                    return 1;
                case MediaFormat.DepthJpeg:
                    return 1;
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public static bool IsMultiPlanar(this MediaFormat format)
        {
            switch (format)
            {
                case MediaFormat.Y8:
                case MediaFormat.Yuv_420_888:
                    return true;
                default:
                    return false;
            }
        }

        public static string ToNameOrHexValue(this MediaFormat format)
        {
            var name = Enum.GetName(typeof(MediaFormat), format);
            return string.IsNullOrEmpty(name)
                ? $"0x{(int)format:X}"
                : name;
        }
    }
}
