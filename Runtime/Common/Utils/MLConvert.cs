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
    using System.Globalization;
    using System.IO;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using UnityEngine;
    using UnityEngine.XR.MagicLeap;

    /// <summary>
    /// Utility class used for converting vectors and matrices between native and unity format.
    /// </summary>
    public static class MLConvert
    {
        public static readonly float[] IdentityMatrixColMajor = { 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1, 0, 0, 0, 0, 1 };

        /// <summary>
        /// Converts a Vector3 to Unity coordinate space and scale.
        /// </summary>
        /// <param name="vec">Vector to convert</param>
        /// <param name="transformToRUF">If coordinate space should change.</param>
        /// <param name="applyScale">If world scale should be applied.</param>
        /// <returns>Converted Vector</returns>
        public static Vector3 ToUnity(Vector3 vec, bool transformToRUF = true) => new Vector3(vec.x, vec.y, (transformToRUF) ? -vec.z : vec.z);

        /// <summary>
        /// Creates a Unity 3D vector from a native vector.
        /// </summary>
        /// <param name="vec">A native vector.</param>
        /// <param name="transformToRUF">(Optional) If false, prevents conversion to Unity's coordinate system.</param>
        /// <param name="applyScale">(Optional) If false, prevents scaling to Unity's unit per meter scale.</param>
        /// <returns>A Unity vector.</returns>
        public static Vector3 ToUnity(MagicLeapNativeBindings.MLVec3f vec, bool transformToRUF = true) => ToUnity(vec.X, vec.Y, vec.Z, transformToRUF);


        /// <summary>
        /// Creates a Unity 2D vector from a native vector.
        /// </summary>
        /// <param name="vec">A native vector.</param>
        /// <param name="transformToRUF">(Optional) If false, prevents conversion to Unity's coordinate system.</param>
        /// <param name="applyScale">(Optional) If false, prevents scaling to Unity's unit per meter scale.</param>
        /// <returns>A Unity vector.</returns>
        public static Vector2 ToUnity(MagicLeapNativeBindings.MLVec2f vec, bool transformToRUF = true) => ToUnity(vec.X, vec.Y, 0, transformToRUF);
        /// <summary>
        /// Creates a Unity 3D vector from a x, y and z parameters.
        /// </summary>
        /// <param name="x">X component</param>
        /// <param name="y">Y component</param>
        /// <param name="z">Z component</param>
        /// <param name="transformToRUF">(Optional) If false, prevents conversion to Unity's coordinate system.</param>
        /// <param name="applyScale">(Optional) If false, prevents scaling to Unity's unit per meter scale.</param>
        /// <returns>A Unity vector.</returns>
        public static Vector3 ToUnity(float x, float y, float z, bool transformToRUF = true) => new Vector3(x, y, transformToRUF ? -z : z);

        /// <summary>
        /// Creates a Unity quaternion from a native quaternion.
        /// </summary>
        /// <param name="quat">A native quaternion.</param>
        /// <param name="transformToRUF">(Optional) If false, prevents conversion to Unity's coordinate system.</param>
        /// <returns>A Unity quaternion.</returns>
        public static Quaternion ToUnity(MagicLeapNativeBindings.MLQuaternionf quat, bool transformToRUF = true) => new Quaternion(quat.X, quat.Y, transformToRUF ? -quat.Z : quat.Z, transformToRUF ? -quat.W : quat.W);


        /// <summary>
        /// Converts a Quaternion to unity space.
        /// </summary>
        /// <param name="quat">Quaternion to convert.</param>
        /// <param name="transformToRUF">(Optional) If false, prevents conversion to Unity's coordinate system.</param>
        /// <returns>A Unity quaternion.</returns>
        public static Quaternion ToUnity(Quaternion quat, bool transformToRUF = true)
        {
            if (transformToRUF)
            {
                quat.z = -quat.z;
                quat.w = -quat.w;
            }
            return quat;
        }

        /// <summary>
        /// Creates Unity 4x4 matrix from native matrix.
        /// </summary>
        /// <param name="mat">A native matrix.</param>
        /// <returns>A Unity matrix.</returns>
        public static Matrix4x4 ToUnity(MagicLeapNativeBindings.MLMat4f mat)
        {
            return FloatsToMat(mat.MatrixColmajor);
        }

        /// <summary>
        /// Creates Unity 4x4 matrix from native transform.
        /// </summary>
        /// <param name="transform">A native transform.</param>
        /// <param name="transformToRUF">(Optional) If false, prevents conversion to Unity's coordinate system.</param>
        /// <param name="applyScale">(Optional) If false, prevents scaling to Unity's unit per meter scale.</param>
        /// <returns>A Unity matrix.</returns>
        public static Matrix4x4 ToUnity(MagicLeapNativeBindings.MLTransform transform, bool transformToRUF = true)
        {
            Vector3 position = ToUnity(transform.Position, transformToRUF);
            Quaternion rotation = ToUnity(transform.Rotation, transformToRUF);

            return Matrix4x4.TRS(position, rotation, Vector3.one);
        }

        /// <summary>
        /// Creates a System.Guid from an MLUUID
        /// </summary>
        /// <param name="uuid">A native UUID</param>
        /// <returns>A System.Guid</returns>
        public static Guid ToUnity(MagicLeapNativeBindings.MLUUID uuid)
        {
            return new Guid(uuid.TimeLow, uuid.TimeMid, uuid.TimeHiAndVersion, uuid.ClockSeqHiAndReserved, uuid.ClockSeqLow, uuid.Node0, uuid.Node1, uuid.Node2, uuid.Node3, uuid.Node4, uuid.Node5);
        }

        /// <summary>
        /// Creates native transform from a Unity matrix.
        /// </summary>
        /// <param name="mat">A Unity matrix.</param>
        /// <param name="transformFromRUF">(Optional) If false, prevents conversion to the native SDK coordinate system.</param>
        /// <param name="applyScale">(Optional) If false, prevents scaling to the native SDK's unit per meter scale.</param>
        /// <returns>A native transform.</returns>
        public static MagicLeapNativeBindings.MLTransform FromUnity(Matrix4x4 mat, bool transformFromRUF = true)
        {
            MagicLeapNativeBindings.MLTransform transform = new MagicLeapNativeBindings.MLTransform();

            transform.Position = FromUnity(GetPositionFromTransformMatrix(mat), transformFromRUF);
            transform.Rotation = FromUnity(GetRotationFromTransformMatrix(mat), transformFromRUF);

            return transform;
        }

        /// <summary>
        /// Fills out array with values from 4x4 Unity matrix.
        /// </summary>
        /// <param name="mat">An input native matrix.</param>
        /// <param name="matrixColMajor">An array to populate in Unity format.</param>
        public static void FromUnity(Matrix4x4 mat, ref float[] matrixColMajor)
        {
            for (int i = 0; i < 16; ++i)
            {
                matrixColMajor[i] = mat[i];
            }
        }

        /// <summary>
        /// Creates native 3d vector from a Unity vector.
        /// </summary>
        /// <param name="vec">A Unity vector.</param>
        /// <param name="transformFromRUF">(Optional) If false, prevents conversion to the native SDK coordinate system.</param>
        /// <param name="applyScale">(Optional) If false, prevents scaling to the native SDK's unit per meter scale.</param>
        /// <returns>A native vector.</returns>
        public static MagicLeapNativeBindings.MLVec3f FromUnity(Vector3 vec, bool transformFromRUF = true)
        {
            if (transformFromRUF)
            {
                vec.z = -vec.z;
            }


            MagicLeapNativeBindings.MLVec3f outVec = new MagicLeapNativeBindings.MLVec3f();
            outVec.X = vec.x;
            outVec.Y = vec.y;
            outVec.Z = vec.z;

            return outVec;
        }

        /// <summary>
        /// Creates native quaternion from a Unity quaternion.
        /// </summary>
        /// <param name="quat">A Unity quaternion.</param>
        /// <param name="transformFromRUF">(Optional) If false, prevents conversion to the native SDK coordinate system.</param>
        /// <returns>A native quaternion.</returns>
        public static MagicLeapNativeBindings.MLQuaternionf FromUnity(Quaternion quat, bool transformFromRUF = true)
        {
            MagicLeapNativeBindings.MLQuaternionf outQuat = new MagicLeapNativeBindings.MLQuaternionf();

            outQuat.X = quat.x;
            outQuat.Y = quat.y;

            if (transformFromRUF)
            {
                outQuat.Z = -quat.z;
                outQuat.W = -quat.w;
            }
            else
            {
                outQuat.Z = quat.z;
                outQuat.W = quat.w;
            }

            return outQuat;
        }

        /// <summary>
        /// Creates an MLUUID from a System.Guid
        /// </summary>
        /// <param name="guid">A System.Guid</param>
        /// <returns>A native MLUUID</returns>
        public static MagicLeapNativeBindings.MLUUID FromUnity(Guid guid)
        {
            MagicLeapNativeBindings.MLUUID result = new MagicLeapNativeBindings.MLUUID();
            string guidString = guid.ToString("N");

            result.TimeLow = uint.Parse(guidString.Substring(0, 8), NumberStyles.HexNumber);
            result.TimeMid = ushort.Parse(guidString.Substring(8, 4), NumberStyles.HexNumber);
            result.TimeHiAndVersion = ushort.Parse(guidString.Substring(12, 4), NumberStyles.HexNumber);
            result.ClockSeqHiAndReserved = byte.Parse(guidString.Substring(16, 2), NumberStyles.HexNumber);
            result.ClockSeqLow = byte.Parse(guidString.Substring(18, 2), NumberStyles.HexNumber);
            result.Node0 = byte.Parse(guidString.Substring(20, 2), NumberStyles.HexNumber);
            result.Node1 = byte.Parse(guidString.Substring(22, 2), NumberStyles.HexNumber);
            result.Node2 = byte.Parse(guidString.Substring(24, 2), NumberStyles.HexNumber);
            result.Node3 = byte.Parse(guidString.Substring(26, 2), NumberStyles.HexNumber);
            result.Node4 = byte.Parse(guidString.Substring(28, 2), NumberStyles.HexNumber);
            result.Node5 = byte.Parse(guidString.Substring(30, 2), NumberStyles.HexNumber);

            return result;
        }

        /// <summary>
        /// Gets the position vector stored in a transform matrix.
        /// </summary>
        /// <param name="transformMatrix">A Unity matrix treated as a transform matrix.</param>
        /// <returns>A Unity vector representing a position.</returns>
        public static Vector3 GetPositionFromTransformMatrix(Matrix4x4 transformMatrix)
        {
            return transformMatrix.GetColumn(3);
        }

        /// <summary>
        /// Gets the rotation quaternion stored in a transform matrix.
        /// </summary>
        /// <param name="transformMatrix">A Unity matrix treated as a transform matrix.</param>
        /// <returns>A Unity quaternion.</returns>
        public static Quaternion GetRotationFromTransformMatrix(Matrix4x4 transformMatrix)
        {
            return Quaternion.LookRotation(transformMatrix.GetColumn(2), transformMatrix.GetColumn(1));
        }

        /// <summary>
        /// Take a string, snips it to a desired length and converts it to UTF8.
        /// </summary>
        /// <param name="inString">String to snip and convert</param>
        /// <param name="snipLength">length to snip to</param>
        /// <returns>UTF8 string byte array</returns>
        public static byte[] ToUTF8Snipped(string inString, int snipLength)
        {
            int snipSize = Math.Min(inString.Length, snipLength);
            int size = Encoding.UTF8.GetByteCount(inString.Substring(0, snipSize));

            while (snipSize >= 0 && size > snipLength)
            {
                size -= Encoding.UTF8.GetByteCount(inString.Substring(snipSize - 1, 1));
                --snipSize;
            }

            return Encoding.UTF8.GetBytes(inString.Substring(0, snipSize));
        }

        /// <summary>
        /// Decodes a buffer of bytes into an ASCII string.
        /// </summary>
        /// <param name="buffer">bytes to convert to a string</param>
        /// <returns>A managed string</returns>
        public static string DecodeAscii(byte[] buffer)
        {
            int count = Array.IndexOf<byte>(buffer, 0, 0);

            if (count < 0)
            {
                count = buffer.Length;
            }

            return Encoding.ASCII.GetString(buffer, 0, count);
        }

        /// <summary>
        /// Decodes a buffer of bytes into a UTF8 string.
        /// </summary>
        /// <param name="buffer">bytes to convert to a UTF8 string</param>
        /// <returns>A managed string</returns>
        public static string DecodeUTF8(byte[] buffer)
        {
            int count = Array.IndexOf<byte>(buffer, 0, 0);

            if (count < 0)
            {
                count = buffer.Length;
            }

            return Encoding.UTF8.GetString(buffer, 0, count);
        }

        /// <summary>
        /// Converts a managed string into an unmanaged null terminated UTF-8 string.
        /// </summary>
        /// <param name="s">The managed string to convert</param>
        /// <returns>A pointer to the unmanaged string</returns>
        public static IntPtr EncodeToUnmanagedUTF8(string s)
        {
            int length = Encoding.UTF8.GetByteCount(s);
            byte[] buffer = new byte[length + 1];

            Encoding.UTF8.GetBytes(s, 0, s.Length, buffer, 0);

            IntPtr nativeUtf8 = Marshal.AllocHGlobal(buffer.Length);

            Marshal.Copy(buffer, 0, nativeUtf8, buffer.Length);

            return nativeUtf8;
        }

        /// <summary>
        /// This encodes the string into a UTF-8 byte array.
        /// </summary>
        /// <param name="decodedString">string to encode</param>
        /// <returns>UTF8 string byte array</returns>
        public static byte[] EncodeUTF8(string decodedString)
        {
            return Encoding.UTF8.GetBytes(decodedString);
        }

        /// <summary>
        /// Converts an unmanaged null terminated UTF-8 string into a managed string.
        /// </summary>
        /// <param name="nativeString">The unmanaged string to convert</param>
        /// <param name="maximumSize">maximum number of characters to convert</param>
        /// <returns>A managed string</returns>
        public static string DecodeUTF8(IntPtr nativeString, int maximumSize = -1)
        {
            if (nativeString == IntPtr.Zero)
            {
                return string.Empty;
            }

            int byteLength = 0;

            if (maximumSize > 0)
            {
                while (Marshal.ReadByte(nativeString, byteLength) != 0)
                {
                    ++byteLength;
                    if (byteLength == maximumSize)
                    {
                        break;
                    }
                }
            }
            else
            {
                while (Marshal.ReadByte(nativeString, byteLength) != 0)
                {
                    ++byteLength;
                }
            }

            if (byteLength == 0)
            {
                return string.Empty;
            }

            byte[] buffer = new byte[byteLength];
            Marshal.Copy(nativeString, buffer, 0, buffer.Length);

            return Encoding.UTF8.GetString(buffer);
        }

        /// <summary>
        /// Converts an unmanaged UTF-16 string into a managed string.
        /// </summary>
        /// <param name="nativeArray">Native byte array to convert</param>
        /// <returns>A managed string</returns>
        public static string DecodeUTF16BE(byte[] nativeArray)
        {
            return Encoding.BigEndianUnicode.GetString(nativeArray);
        }

        /// <summary>
        /// Converts an unmanaged UTF-16 string into a managed string.
        /// </summary>
        /// <param name="nativeArray">Native byte array to convert</param>
        /// <returns>A managed string</returns>
        public static string DecodeUTF16LE(byte[] nativeArray)
        {
            return Encoding.Unicode.GetString(nativeArray);
        }

        /// <summary>
        /// Convert an object to a byte array. Uses C# Binary formatter to serialize
        /// </summary>
        /// <typeparam name="T">Data type of object</typeparam>
        /// <param name="obj">Object to convert</param>
        /// <returns>Returns a binary array representation of the object</returns>
        public static byte[] ObjectToByteArray<T>(T obj)
        {
            BinaryFormatter bf = new BinaryFormatter();

            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// <summary>
        /// Convert a byte array to an Object
        /// </summary>
        /// <param name="byteArray">Byte array to convert</param>
        /// <returns>Returns the newly converted object</returns>
        public static object ByteArrayToObject(byte[] byteArray)
        {
            using (var memStream = new MemoryStream())
            {
                var binForm = new BinaryFormatter();
                memStream.Write(byteArray, 0, byteArray.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);
                return obj;
            }
        }

        public static void FlipTransformMatrixVertically(float[] frameTransformMatColMajor)
        {
            /*
            Using the matrix provided by MLNativeSurfaceGetFrameTransformationMatrix() directly renders
            the video upside down. Multiply it with the following matrix to flip it and render with the
            right orientation in Vulkan -

            | 1  0   0  0 |
            | 0  -1  0  1 |
            | 0  0   1  0 |
            | 0  0   0  1 |

            The multiplication result in this -

            | m00  -m01  m02  m03 + m01 |
            | m10  -m11  m12  m13 + m11 |
            | m20  -m21  m22  m23 + m21 |
            | m30  -m31  m32  m33 + m32 |
            */

            for (int i = 12; i <= 15; ++i)
            {
                frameTransformMatColMajor[i] += frameTransformMatColMajor[i - 8];
            }

            for (int i = 4; i <= 7; ++i)
            {
                frameTransformMatColMajor[i] = -1 * frameTransformMatColMajor[i];
            }
        }

        public static void FlipTransformMatrixHorizontally(float[] frameTransformMatColMajor)
        {
            /*
            Multiply with the following matrix to flip horizontally -

            | -1 0  0  1 |
            | 0  1  0  0 |
            | 0  0  1  0 |
            | 0  0  0  1 |

            The multiplication result in this -

            | -m00  m01  m02  m03 + m00 |
            | -m10  m11  m12  m13 + m10 |
            | -m20  m21  m22  m23 + m20 |
            | -m30  m31  m32  m33 + m30 |
            */

            for (int i = 12; i <= 15; ++i)
            {
                frameTransformMatColMajor[i] += frameTransformMatColMajor[i - 12];
            }

            for (int i = 0; i <= 4; ++i)
            {
                frameTransformMatColMajor[i] = -1 * frameTransformMatColMajor[i];
            }
        }


        /// <summary>
        /// Creates Unity 4x4 matrix from an array of 16 floats.
        /// </summary>
        /// <param name="vals">An array of 16 floats.</param>
        /// <returns>A Unity matrix.</returns>
        private static Matrix4x4 FloatsToMat(float[] vals)
        {
            Matrix4x4 mat = new Matrix4x4();

            for (int i = 0; i < 16; ++i)
            {
                mat[i] = vals[i];
            }

            return mat;
        }

        /// <summary>
        /// Converts an unmanged array to a managed array of type T.
        /// </summary>
        public static T[] MarshalUnmanagedArray<T>(IntPtr arrayPtr, int count)
        {
            T[] convertedArray = new T[count];
            int tSize = Marshal.SizeOf<T>();
            for (int i = 0; i < count; ++i)
            {
                convertedArray[i] = Marshal.PtrToStructure<T>((arrayPtr + (tSize * i)));
            }

            return convertedArray;
        }
    }
}
