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

    /// <summary>
    /// Defines C# API interface to C-API layer.
    /// </summary>
    public partial class MagicLeapNativeBindings
    {
        /// <summary>
        /// The 64 bit id for an invalid native handle.
        /// </summary>
        public const ulong InvalidHandle = 0xFFFFFFFFFFFFFFFF;

        /// <summary>
        /// Initializes a new instance of the <see cref="MagicLeapNativeBindings" /> class.
        /// </summary>
        protected MagicLeapNativeBindings()
        {
        }

        /// <summary>
        /// The current state of a given tracker.
        /// </summary>
        public enum MLSensoryState
        {
            /// <summary>
            /// The tracker is not ready, don't use the data.
            /// </summary>
            Initializing,

            /// <summary>
            /// The tracker's data can be used.
            /// </summary>
            Ready
        }

        /// <summary>
        /// Checks if 64 bit handle is valid.
        /// </summary>
        /// <returns><c>true</c>, if handle is valid, <c>false</c> if invalid.</returns>
        /// <param name="handle">The handle to check.</param>
        public static bool MLHandleIsValid(ulong handle)
        {
            return handle != InvalidHandle;
        }

        /// <summary>
        /// Returns an ASCII string for MLResultGlobal codes.
        /// </summary>
        /// <param name="result">The input MLResult enum from ML API methods.</param>
        /// <returns>An ASCII string containing readable version of result code.</returns>
        public static string MLGetResultString(MLResult.Code result)
        {
            switch (result)
            {
                case MLResult.Code.Ok:
                    {
                        return "MLResult_Ok";
                    }

                case MLResult.Code.Pending:
                    {
                        return "MLResult_Pending";
                    }

                case MLResult.Code.Timeout:
                    {
                        return "MLResult_Timeout";
                    }

                case MLResult.Code.Locked:
                    {
                        return "MLResult_Locked";
                    }

                case MLResult.Code.UnspecifiedFailure:
                    {
                        return "MLResult_UnspecifiedFailure";
                    }

                case MLResult.Code.InvalidParam:
                    {
                        return "MLResult_InvalidParam";
                    }

                case MLResult.Code.AllocFailed:
                    {
                        return "MLResult_AllocFailed";
                    }

                case MLResult.Code.PermissionDenied:
                    {
                        return "MLResult_PermissionDenied";
                    }

                case MLResult.Code.NotImplemented:
                    {
                        return "MLResult_NotImplemented";
                    }

                case MLResult.Code.ClientLimitExceeded:
                    {
                        return "MLResult_ClientLimitExceeded";
                    }

                case MLResult.Code.PoseNotFound:
                    {
                        return "MLResult_PoseNotFound";
                    }

                case MLResult.Code.APIDLLNotFound:
                    {
                        return "MLResult_APIDLLNotFound";
                    }

                case MLResult.Code.APISymbolsNotFound:
                    {
                        return "MLResult_APISymbolsNotFound";
                    }

                case MLResult.Code.IncompatibleSKU:
                    {
                        return "MLResult_IncompatibleSKU";
                    }

                case MLResult.Code.PerceptionSystemNotStarted:
                    {
                        return "MLResult_PerceptionSystemNotStarted";
                    }

                case MLResult.Code.LicenseError:
                    {
                        return "MLResult_LicenseError";
                    }

                default:
                    {
                        return "MLResult_Unknown";
                    }
            }
        }

        /// <summary>
        /// Returns an ASCII string for MLSnapshotResult codes.
        /// </summary>
        /// <param name="resultCode">The input MLResult enum from ML API methods.</param>
        /// <returns>An ASCII string containing readable version of result code.</returns>
        internal static string MLGetSnapshotResultString(MLResult.Code resultCode)
        {
            try
            {
                return Marshal.PtrToStringAnsi(MagicLeapNativeBindings.MLSnapshotGetResultString(resultCode));
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error("MagicLeapNativeBindings.MLGetSnapshotResultString failed. Reason: MagicLeapNativeBindings is currently available only on device.");
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MagicLeapNativeBindings.MLGetSnapshotResultString failed. Reason: API symbols not found");
            }

            return string.Empty;
        }

        /// <summary>
        /// Returns an ASCII string for MLSnapshotResult codes.
        /// </summary>
        /// <param name="resultCode">The input MLResult enum from ML API methods.</param>
        /// <returns>An ASCII string containing readable version of result code.</returns>
        internal static string MLGetInputResultString(MLResult.Code resultCode)
        {
            try
            {
                return Marshal.PtrToStringAnsi(MagicLeapNativeBindings.MLInputGetResultString(resultCode));
            }
            catch (System.DllNotFoundException)
            {
                MLPluginLog.Error("MagicLeapNativeBindings.MLGetInputResultString failed. Reason: MagicLeapNativeBindings is currently available only on device.");
            }
            catch (System.EntryPointNotFoundException)
            {
                MLPluginLog.Error("MagicLeapNativeBindings.MLGetInputResultString failed. Reason: API symbols not found");
            }

            return string.Empty;
        }

        /// <summary>
        /// Pull in the latest state of all persistent transforms and all
        /// enabled trackers extrapolated to the next frame time.
        /// Returns an MLSnapshot with this latest state. This snap should be
        /// used for the duration of the frame being constructed and then
        /// released with a call to MLPerceptionReleaseSnapshot().
        /// </summary>
        /// <param name="snapshot">Pointer to a pointer containing an MLSnapshot on success.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if operation was successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// </returns>
        [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPerceptionGetSnapshot(ref IntPtr snapshot);

        /// <summary>
        /// Pulls in the state of all persistent transforms and all
        /// enabled trackers extrapolated to the provided timestamp.
        /// This timestamp typically comes from out_frame_info.predicted_display_time out parameter from
        /// the MLGraphicsBeginFrameEx function.
        /// Returns a MLSnapshot with this latest state. This snap should be
        /// used for the duration of the frame being constructed and then
        /// released with a call to MLPerceptionReleaseSnapshot().
        /// </summary>
        /// <param name="timestamp">Timestamp representing the time for which to predict poses.</param>
        /// <param name="out_snapshot">Pointer to a pointer containing an MLSnapshot on success.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if operation was successful.
        /// MLResult.Result will be <c>MLResult.Code.InvalidTimestamp</c> if Timestamp is either more than 100ms in the future or too old for cached state.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if Output parameter was not valid (null).
        /// MLResult.Result will be <c>MLResult.Code.PerceptionSystemNotStartede</c> if Perception System has not been started.
        /// </returns>
        [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPerceptionGetPredictedSnapshot(ulong timestamp, ref IntPtr out_snapshot);

        /// <summary>
        /// Pull in the latest state of all persistent transforms and all
        /// enabled trackers extrapolated to the next frame time.
        /// Return an MLSnapshot with this latest state. This snap should be
        /// used for the duration of the frame being constructed and then
        /// released with a call to MLPerceptionReleaseSnapshot().
        /// </summary>
        /// <param name="snap">Pointer to a pointer containing an MLSnapshot on success.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if a Snapshot was created successfully successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if a Snapshot was not created successfully.
        /// </returns>
        [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPerceptionReleaseSnapshot(IntPtr snap);

        /// <summary>
        /// Gets the transform between world origin and the coordinate frame `id'.
        /// </summary>
        /// <param name="snap">A snapshot of the tracker state. Can be obtained with MLPerceptionGetSnapshot().</param>
        /// <param name="id">Look up the transform between the current origin and this coordinate frame id.</param>
        /// <param name="outTransform">Valid pointer to an MLTransform. To be filled out with requested transform data.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if the transform was obtained successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed due to an invalid parameter.
        /// MLResult.Result will be <c>MLResult.Code.PoseNotFound</c> if the coordinate frame is valid, but not found in the current pose snapshot.
        /// </returns>
        [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLSnapshotGetTransform(IntPtr snap, ref MLCoordinateFrameUID id, ref MLTransform outTransform);

        /// <summary>
        /// Get the static data pertaining to the snapshot system. Requires API level 30.
        /// </summary>
        /// <param name="outStaticData">Valid pointer to an MLSnapshotStaticData. To be filled out with snapshot static data.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to obtain static data due to invalid parameter.
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if obtained static data successfully.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain static data due to internal error.
        /// </returns>
        [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLSnapshotGetStaticData(ref MLSnapshotStaticData outStaticData);

        /// <summary>
        /// Get transform between coordinate frame 'base_id' and the coordinate frame `id' as well as any derivatives
        /// that have been calculated.
        /// </summary>
        /// <param name="snap">A snapshot of tracker state. Can be obtained with MLPerceptionGetSnapshot().</param>
        /// <param name="base_id">The coordinate frame in which to locate 'id'.</param>
        /// <param name="id">The coordinate frame which needs to be located in the base_id coordinate frame.</param>
        /// <param name="outPose">Valid pointer to an MLPose. To be filled out with requested pose data.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to obtain transform due to invalid parameter.
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if obtained transform successfully.
        /// MLResult.Result will be <c>MLResult.Code.PoseNotFoundk</c> if coordinate Frame is valid, but not found in the current pose snapshot.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to obtain transform due to internal error.
        /// </returns>
        [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLSnapshotGetPoseInBase(IntPtr snap, ref MLCoordinateFrameUID base_id, ref MLCoordinateFrameUID id, ref MLPose outPose);

        /// <summary>
        /// Returns a pointer to an ASCII string representation for each result code.
        /// This call can return a pointer to the string for any of the MLSnapshot related MLResult codes.
        /// Developers should use MLResult.CodeToString(MLResult.Code).
        /// </summary>
        /// <param name="result">MLResult type to be converted to string.</param>
        /// <returns>Returns a pointer to an ASCII string containing readable version of the result code.</returns>
        [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr MLSnapshotGetResultString(MLResult.Code result);

        /// <summary>
        /// Returns a pointer to an ASCII string representation for each result code.
        /// This call can return a pointer to the string for any of the MLInput related MLResult codes.
        /// Developers should use MLResult.CodeToString(MLResult.Code).
        /// </summary>
        /// <param name="result">MLResult type to be converted to string.</param>
        /// <returns>Returns a pointer to an ASCII string containing readable version of the result code.</returns>
        [DllImport(MLInputDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern IntPtr MLInputGetResultString(MLResult.Code result);

        /// <summary>
        /// Query the OS for which Platform API Level is supported.
        /// </summary>
        /// <param name="level">Pointer to an integer that will store the API level.</param>
        /// <returns>
        /// MLResult.Result will be <c>MLResult.Code.Ok</c> if operation was successful.
        /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed due to internal error.
        /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if level was not valid (null).
        /// </returns>
        [DllImport(MLPlatformDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLPlatformGetAPILevel(ref uint level);

        /// <summary>
        /// Returns the minimum API level of the MLSDK used by Unity
        /// </summary>
        /// <param name="minApiLevel">Value containing the minimum API level.</param>
        /// <returns>
        /// <c>MLResult.Code.Ok</c>: Minimum API level was retrieved successfully.<br/>
        /// <c>MLResult.Code.NotImplemented</c>: The ml_sdk_loader plugin was not compiled with knowledge of the minimum API level.
        /// </returns>
        [DllImport(MLSdkLoaderDll, CallingConvention = CallingConvention.Cdecl)]
        public static extern MLResult.Code MLUnitySdkGetMinApiLevel(out uint minApiLevel);

        /// <summary>
        /// Tries to get the pose for the given coordinate frame id.
        /// </summary>
        /// <param name="id">The coordinate frame id to get the pose of.</param>
        /// <param name="pose">The object to initialize the found pose with.</param>
        /// <returns>True if a pose was successfully found.</returns>
        [DllImport(UnityMagicLeapDll, EntryPoint = "UnityMagicLeap_TryGetPose")]
        [return: MarshalAs(UnmanagedType.I1)]
        public static extern bool UnityMagicLeap_TryGetPose(MLCoordinateFrameUID id, out UnityEngine.Pose pose);

        /// <summary>
        /// 2D vector represented with X and Y floats.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MLVec2f
        {
            /// <summary>
            /// X coordinate.
            /// </summary>
            public float X;

            /// <summary>
            /// Y coordinate.
            /// </summary>
            public float Y;

            public Vector2 ToVector2() => new Vector3(X, Y);
        }

        /// <summary>
        /// 3D vector in native format.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct MLVec3f
        {
            /// <summary>
            /// X coordinate.
            /// </summary>
            public float X;

            /// <summary>
            /// Y coordinate.
            /// </summary>
            public float Y;

            /// <summary>
            /// Z coordinate.
            /// </summary>
            public float Z;

            public Vector3 ToVector3() => new Vector3(X, Y, Z);
        }

        /// <summary>
        /// Quaternion in native format.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct MLQuaternionf
        {
            /// <summary>
            /// X coordinate.
            /// </summary>
            public float X;

            /// <summary>
            /// Y coordinate.
            /// </summary>
            public float Y;

            /// <summary>
            /// Z coordinate.
            /// </summary>
            public float Z;

            /// <summary>
            /// W coordinate.
            /// </summary>
            public float W;

            /// <summary>
            /// Returns an initialized <c>MLQuaternionf</c> with default values.
            /// </summary>
            /// <returns>An initialized <c>MLQuaternionf</c>.</returns>
            public static MLQuaternionf Identity()
            {
                MLQuaternionf quat = new MLQuaternionf()
                {
                    X = 0,
                    Y = 0,
                    Z = 0,
                    W = 1
                };

                return quat;
            }
        }

        /// <summary>
        /// Information used to transform from one coordinate frame to another.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MLTransform
        {
            /// <summary>
            /// The rotation of the coordinate frame to apply after the translation.
            /// </summary>
            public MLQuaternionf Rotation;

            /// <summary>
            /// The translation to apply to get the coordinate frame in the proper location.
            /// </summary>
            public MLVec3f Position;

            /// <summary>
            /// Returns an initialized MLTransform with default values.
            /// </summary>
            /// <returns>An initialized MLTransform.</returns>
            public static MLTransform Identity()
            {
                MLTransform t = new MLTransform();
                t.Rotation = MLQuaternionf.Identity();
                return t;
            }

            public override string ToString()
            {
                return $"Position: ({Position.X}, {Position.Y}, {Position.Z}) Rotation: ({Rotation.X}, {Rotation.Y}, {Rotation.Z}, {Rotation.W})";
            }
        }

        /// <summary>
        /// 4x4 matrix in native format.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MLMat4f
        {
            /// <summary>
            /// The 16 matrix values.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public float[] MatrixColmajor;
        }

        /// <summary>
        /// 2D rectangle in native format.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MLRectf
        {
            /// <summary>
            /// The x coordinate.
            /// </summary>
            public float X;

            /// <summary>
            /// The y coordinate.
            /// </summary>
            public float Y;

            /// <summary>
            /// The width.
            /// </summary>
            public float W;

            /// <summary>
            /// The height.
            /// </summary>
            public float H;
        }

        /// <summary>
        /// 2D rectangle with integer values in native format.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MLRecti
        {
            /// <summary
            /// >The x coordinate.
            /// </summary>
            public int X;

            /// <summary>
            /// The y coordinate.
            /// </summary>
            public int Y;

            /// <summary>
            /// The width.
            /// </summary>
            public int W;

            /// <summary>
            /// The height.
            /// </summary>
            public int H;
        }

        /// <summary>
        /// Universally unique identifier
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MLUUID
        {
            /// <summary>
            /// The TimeLow field.
            /// </summary>
            public uint TimeLow;

            /// <summary>
            /// The TimeMid field.
            /// </summary>
            public ushort TimeMid;

            /// <summary>
            /// The TimeHiAndVersion field.
            /// </summary>
            public ushort TimeHiAndVersion;

            /// <summary>
            /// The <c>ClockSeqHiAndReserved</c> field.
            /// </summary>
            public byte ClockSeqHiAndReserved;

            /// <summary>
            /// The <c>ClockSeqLow</c> field.
            /// </summary>
            public byte ClockSeqLow;

            /// <summary>
            /// The Node0 field.
            /// </summary>
            public byte Node0;

            /// <summary>
            /// The Node1 field.
            /// </summary>
            public byte Node1;

            /// <summary>
            /// The Node2 field.
            /// </summary>
            public byte Node2;

            /// <summary>
            /// The Node3 field.
            /// </summary>
            public byte Node3;

            /// <summary>
            /// The Node4 field.
            /// </summary>
            public byte Node4;

            /// <summary>
            /// The Node5 field.
            /// </summary>
            public byte Node5;
        }

        /// <summary>
        /// Universally unique identifier, byte array.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MLUUIDBytes
        {
            private static readonly int[] hyphenIndices = new int[] { 8, 13, 18, 23 };

            /// <summary>
            /// The 16 byte data array.
            /// </summary>
            [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
            public byte[] Data;

            public static bool operator ==(MLUUIDBytes one, MLUUIDBytes two)
            {
                if (one.Data.Length != two.Data.Length)
                    return false;

                for (int i = 0; i < one.Data.Length; ++i)
                {
                    if (one.Data[i] != two.Data[i])
                        return false;
                }

                return true;
            }

            public static bool operator !=(MLUUIDBytes one, MLUUIDBytes two)
            {
                return !(one == two);
            }

            public override bool Equals(object obj)
            {
                if (obj is MLUUIDBytes)
                {
                    var rhs = (MLUUIDBytes)obj;
                    return this == rhs;
                }

                return false;
            }

            public override int GetHashCode() => this.Data[0].GetHashCode();

            public override string ToString()
            {
                string idString = string.Empty;

                foreach (byte b in this.Data)
                    idString += string.Format("{0:x2}", b);

                foreach (int i in hyphenIndices)
                    idString = idString.Insert(i, "-");

                return idString;
            }

            internal MLUUIDBytes(string id)
            {
                id = id.Replace("-", string.Empty);
                this.Data = StringToByteArray(id);
            }

            private static byte[] StringToByteArray(string hex)
            {
                int numChars = hex.Length;
                byte[] bytes = new byte[numChars / 2];
                for (int i = 0; i < numChars; i += 2)
                    bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
                return bytes;
            }
        }

        /// <summary>
        /// A unique identifier which represents a coordinate frame.
        /// The unique identifier is comprised of two values.
        /// </summary>
        [Serializable]
        [StructLayout(LayoutKind.Sequential)]
        public struct MLCoordinateFrameUID
        {
            /// <summary>
            /// The first data value.
            /// </summary>
            public ulong First;

            /// <summary>
            /// The second data value.
            /// </summary>
            public ulong Second;

            /// <summary>
            /// Gets an initialized MLCoordinateFrameUID.
            /// </summary>
            /// <returns>An initialized MLCoordinateFrameUID.</returns>
            public static MLCoordinateFrameUID EmptyFrame
            {
                get
                {
                    return new MLCoordinateFrameUID();
                }
            }

            /// <summary>
            /// Constructor getting two ulongs as argument.
            /// </summary>
            /// <param name="first">First data value.</param>
            /// <param name="second">Second data value.</param>
            public MLCoordinateFrameUID(ulong first, ulong second)
            {
                First = first;
                Second = second;
            }

            /// <summary>
            /// Constructor getting GUID as argument.
            /// </summary>
            /// <param name="guid">GUID from which both values will be calculated.</param>
            public MLCoordinateFrameUID(Guid guid)
            {
                First = 0;
                Second = 0;
                FromGuid(guid);
            }

            /// <summary>
            /// Constructor getting GUID as argument in string form.
            /// </summary>
            /// <param name="guidString">GUID from which both values will be calculated.</param>
            public MLCoordinateFrameUID(string guidString)
            {
                First = 0;
                Second = 0;
                Guid guid;

                if (Guid.TryParse(guidString, out guid))
                {
                    FromGuid(guid);
                }
            }

            /// <summary>
            /// The equality check to be used for comparing two MLCoordinateFrameUID structs.
            /// </summary>
            /// <param name="one">The first struct to compare with the second struct. </param>
            /// <param name="two">The second struct to compare with the first struct. </param>
            /// <returns>True if the two provided structs have the same two data values.</returns>
            public static bool operator ==(MLCoordinateFrameUID one, MLCoordinateFrameUID two)
            {
                return one.First == two.First && one.Second == two.Second;
            }

            /// <summary>
            /// The inequality check to be used for comparing two MLCoordinateFrameUID structs.
            /// </summary>
            /// <param name="one">The first struct to compare with the second struct. </param>
            /// <param name="two">The second struct to compare with the first struct. </param>
            /// <returns>True if the two provided structs do not have the same two data values.</returns>
            public static bool operator !=(MLCoordinateFrameUID one, MLCoordinateFrameUID two)
            {
                return !(one == two);
            }

            /// <summary>
            /// The equality check to be used for when being compared to an object.
            /// </summary>
            /// <param name="obj">The object to compare to this one with.</param>
            /// <returns>True if the the provided object is of the MLCoordinateFrameUID type and has the same two data values.</returns>
            public override bool Equals(object obj)
            {
                if (obj is MLCoordinateFrameUID)
                {
                    var rhs = (MLCoordinateFrameUID)obj;
                    return this == rhs;
                }

                return false;
            }

            /// <summary>
            /// Gets the hash code to use from the first data value.
            /// </summary>
            /// <returns>The hash code returned by the first data value of this object </returns>
            public override int GetHashCode()
            {
                return this.First.GetHashCode();
            }

            /// <summary>
            /// Returns the string value of the GUID of this MLCoordinateFrameUID.
            /// </summary>
            /// <returns>The string value of the GUID.</returns>
            public override string ToString()
            {
                return this.ToGuid().ToString();
            }

            /// <summary>
            /// Returns the GUID based on the values of this MLCoordinateFrameUID.
            /// </summary>
            /// <returns>The calculated GUID.</returns>
            public Guid ToGuid()
            {
                byte[] toConvert = BitConverter.GetBytes(this.First);
                byte[] newSecond = BitConverter.GetBytes(this.Second);
                FlipGuidComponents(toConvert);
                ulong newFirst = BitConverter.ToUInt64(toConvert, 0);

                return new Guid((int)(newFirst >> 32 & 0x00000000FFFFFFFF), (short)(newFirst >> 16 & 0x000000000000FFFF), (short)(newFirst & 0x000000000000FFFF), newSecond);
            }

            /// <summary>
            /// Sets First and Second data value based on given GUID.
            /// </summary>
            /// <param name="guid">GUID needed to calculate both data values.</param>
            public void FromGuid(Guid guid)
            {
                byte[] guidBytes = guid.ToByteArray();

                byte[] arrayInt = new byte[4];
                Array.Copy(guidBytes, arrayInt, 4);
                int firstPart = (int)BitConverter.ToUInt32(arrayInt, 0);

                byte[] arrayShort = new byte[2];
                Array.Copy(guidBytes, 4, arrayShort, 0, 2);
                short secondPart = (short)BitConverter.ToUInt16(arrayShort, 0);

                Array.Copy(guidBytes, 6, arrayShort, 0, 2);
                short thirdPart = (short)BitConverter.ToUInt16(arrayShort, 0);

                ulong first = ((ulong)firstPart << 32) + ((ulong)secondPart << 16) + (ulong)thirdPart;

                byte[] firstBytes = BitConverter.GetBytes(first);
                FlipGuidComponents(firstBytes);

                Second = BitConverter.ToUInt64(guidBytes, 8);
                First = BitConverter.ToUInt64(firstBytes, 0);
            }

            /// <summary>
            /// Sets First and Second data value based on given GUID in stirng form.
            /// </summary>
            /// <param name="guidString">GUID needed to calculate both data values</param>
            public void FromString(string guidString)
            {
                Guid guid;

                if (Guid.TryParse(guidString, out guid))
                {
                    FromGuid(guid);
                }
            }

            /// <summary>
            /// Sets First and Second value.
            /// </summary>
            /// <param name="first">First data value.</param>
            /// <param name="second">Second data value.</param>
            public void FromULongPair(ulong first, ulong second)
            {
                First = first;
                Second = second;
            }

            /// <summary>
            /// Flips a component of the GUID based on <c>endianness</c>.
            /// </summary>
            /// <param name="bytes">The array of bytes to reverse.</param>
            private static void FlipGuidComponents(byte[] bytes)
            {
                if (BitConverter.IsLittleEndian)
                {
                    Array.Reverse(bytes);
                }
            }
        }

        /// <summary>
        /// Geometric relationship between two coordinate frames.
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MLPose
        {
            /// <summary>
            /// 6-DoF transformation between the two coordinate frames that can be
            /// directly used to express source frame coordinates in destination frame
            /// coordinates.
            /// </summary>
            public MLTransform Transform;

            /// <summary>
            /// Indicate if this pose has derivative values.
            /// </summary>
            public bool HasDerivatives;

            /// <summary>
            /// The linear velocity in meters per second.
            /// </summary>
            public MLVec3f LinearVelocity;

            /// <summary>
            /// The linear acceleration in meters per second squared.
            /// </summary>
            public MLVec3f LinearAcceleration;

            /// <summary>
            /// Angular velocity in radians per second.
            /// </summary>
            public MLVec3f AngularVelocity;

            /// <summary>
            /// Angular accleration in radians per second squared.
            /// </summary>
            public MLVec3f AngularAcceleration;

            /// <summary>
            /// Time when this relationship was measured.
            /// </summary>
            public long OriginTimeNs;

            /// <summary>
            /// Time to which this relationship has been predicted.
            /// May be equal to origin_time_ns.
            /// </summary>
            public long PredictTimeNs;
        }

        /// <summary>
        /// Static information about the snapshot system.
        /// Initalize this structure with MLSnapshotStaticDataInit() and populate with MLSnapshotGetStaticData()
        /// </summary>
        [StructLayout(LayoutKind.Sequential)]
        public struct MLSnapshotStaticData
        {
            /// <summary>
            /// Version of this structure.
            /// </summary>
            UInt32 version;

            /// <summary>
            /// Coordinate frame ID.
            /// </summary>
            MLCoordinateFrameUID CoordWorldOrigin;

            public static MLSnapshotStaticData Init()
            {
                return new MLSnapshotStaticData()
                {
                    version = 1u,
                };
            }
        }

    }
}
