using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.MagicLeap.Native;

namespace UnitySDKEditorTests
{
    public partial class MLConvert
    {
        public static object[] Vector3_ToUnity_Test_Cases =
        {
            new object[] { new Vector3(0, 0, 0), true, new Vector3(0, 0, 0) },
            new object[] { new Vector3(0, 0, 1), true, new Vector3(0, 0, -1) },
            new object[] { new Vector3(0, 0, 1), false, new Vector3(0, 0, 1) },
        };

        [Test]
        [TestCaseSource(nameof(Vector3_ToUnity_Test_Cases))]
        public void Vector3_ToUnity_Test(Vector3 @case, bool transformFromRUF, Vector3 result)
        {
            var converted = UnityEngine.XR.MagicLeap.Native.MLConvert.ToUnity(@case, transformFromRUF);
            Assert.IsTrue(converted == result);
        }

        public static object[] MLVec3f_ToUnity_Test_Cases =
        {
            new object[] { new MagicLeapNativeBindings.MLVec3f { X = 0, Y = 0, Z = 0 }, true, new Vector3(0, 0, 0) },
            new object[] { new MagicLeapNativeBindings.MLVec3f { X = 0, Y = 0, Z = 1 }, true, new Vector3(0, 0, -1) },
            new object[] { new MagicLeapNativeBindings.MLVec3f { X = 0, Y = 0, Z = 1 }, false, new Vector3(0, 0, 1) },
        };

        [Test]
        [TestCaseSource(nameof(MLVec3f_ToUnity_Test_Cases))]
        public void MLVec3f_ToUnity_Test(MagicLeapNativeBindings.MLVec3f @case, bool transformFromRUF, Vector3 result)
        {
            var converted = UnityEngine.XR.MagicLeap.Native.MLConvert.ToUnity(@case, transformFromRUF);
            Assert.IsTrue(converted == result);
        }

        public static object[] MLVec2f_ToUnity_Test_Cases =
        {
            new object[] { new MagicLeapNativeBindings.MLVec2f { X = 0, Y = 0 }, true, new Vector2(0, 0) },
            new object[] { new MagicLeapNativeBindings.MLVec2f { X = 0, Y = 1 }, true, new Vector2(0, 1) },
            new object[] { new MagicLeapNativeBindings.MLVec2f { X = 0, Y = 1 }, false, new Vector2(0, 1) },
        };

        [Test]
        [TestCaseSource(nameof(MLVec2f_ToUnity_Test_Cases))]
        public void MLVec2f_ToUnity_Test(MagicLeapNativeBindings.MLVec2f @case, bool transformFromRUF, Vector2 result)
        {
            var converted = UnityEngine.XR.MagicLeap.Native.MLConvert.ToUnity(@case, transformFromRUF);
            Assert.IsTrue(converted == result);
        }

        public static object[] Floats_Vector3_ToUnity_Test_Cases =
        {
            new object[] { 0f, 0f, 0f, true, new Vector3(0, 0, 0) },
            new object[] { 0f, 0f, 1f, true, new Vector3(0, 0, -1) },
            new object[] { 0f, 0f, 1f, false, new Vector3(0, 0, 1) },
        };

        [Test]
        [TestCaseSource(nameof(Floats_Vector3_ToUnity_Test_Cases))]
        public void Floats_Vector3_ToUnity_Test(float x, float y, float z, bool transformFromRUF, Vector3 result)
        {
            var converted = UnityEngine.XR.MagicLeap.Native.MLConvert.ToUnity(x, y, z, transformFromRUF);
            Assert.IsTrue(converted == result);
        }

        public static object[] MLQuaternionf_ToUnity_Test_Cases =
        {
            new object[] { new MagicLeapNativeBindings.MLQuaternionf() { X = 0, Y = 0, Z = 0, W = 1 }, true, new Quaternion(0, 0, 0, -1) },
            new object[] { MagicLeapNativeBindings.MLQuaternionf.Identity(), true, new Quaternion(0, 0, 0, -1) },
            new object[] { new MagicLeapNativeBindings.MLQuaternionf() { X = 1, Y = 1, Z = 1, W = 1 }, true, new Quaternion(0, 0, -1, -1) },
            new object[] { new MagicLeapNativeBindings.MLQuaternionf() { X = 1, Y = 1, Z = 1, W = 1 }, false, new Quaternion(0, 0, 1, 1) },
        };

        [Test]
        [TestCaseSource(nameof(MLQuaternionf_ToUnity_Test_Cases))]
        public void MLQuaternionf_ToUnity_Test(MagicLeapNativeBindings.MLQuaternionf @case, bool transformFromRUF, Quaternion result)
        {
            Quaternion converted = UnityEngine.XR.MagicLeap.Native.MLConvert.ToUnity(@case, transformFromRUF);
            Assert.IsTrue(Quaternion.Angle(converted, result) == 0f);
        }

        public static object[] Quaternion_ToUnity_Test_Cases =
        {
            new object[] { new Quaternion(0, 0, 0, 1), true, new Quaternion(0, 0, 0, -1) },
            new object[] { Quaternion.identity, true, new Quaternion(0, 0, 0, -1) },
            new object[] { new Quaternion(0, 0, 1, 1), true, new Quaternion(0, 0, -1, -1) },
            new object[] { new Quaternion(0, 0, 1, 1), false, new Quaternion(0, 0, 1, 1) },
        };

        [Test]
        [TestCaseSource(nameof(Quaternion_ToUnity_Test_Cases))]
        public void Quaternion_ToUnity_Test(Quaternion @case, bool transformFromRUF, Quaternion result)
        {
            Quaternion converted = UnityEngine.XR.MagicLeap.Native.MLConvert.ToUnity(Quaternion.identity, transformFromRUF);
            Assert.IsTrue(Quaternion.Angle(converted, result) == 0f);
        }

        public static object[] Native_Matrix4x4_ToUnity_Test_Cases =
        {
            new object[]
            {
                new MagicLeapNativeBindings.MLMat4f
                {
                    MatrixColmajor = new float[]
                    {
                        1, 0, 0, 0,
                        0, 1, 0, 0,
                        0, 0, 1, 0,
                        0, 0, 0, 1
                    }
                },
                Matrix4x4.identity
            },
        };

        [Test]
        [TestCaseSource(nameof(Native_Matrix4x4_ToUnity_Test_Cases))]
        public void Native_Matrix4x4_ToUnity_Test(MagicLeapNativeBindings.MLMat4f @case, Matrix4x4 result)
        {
            Matrix4x4 converted = UnityEngine.XR.MagicLeap.Native.MLConvert.ToUnity(@case);
            Assert.IsTrue(converted == result);

        }

        public static object[] Matrix4x4_ToUnity_Test_Cases =
        {
            new object[] { MagicLeapNativeBindings.MLTransform.Identity(), true, Matrix4x4.identity },
            new object[] { MagicLeapNativeBindings.MLTransform.Identity(), false, Matrix4x4.identity },
        };

        [Test]
        [TestCaseSource(nameof(Matrix4x4_ToUnity_Test_Cases))]
        public void Matrix4x4_ToUnity_Test(MagicLeapNativeBindings.MLTransform @case, bool transformFromRUF, Matrix4x4 result)
        {
            Matrix4x4 converted = UnityEngine.XR.MagicLeap.Native.MLConvert.ToUnity(@case, transformFromRUF);
            Assert.IsTrue(converted == result);
        }

        public static object[] MLUUID_To_GUID_ToUnity_Test_Cases =
        {
            new object[]
            {
                new MagicLeapNativeBindings.MLUUID()
                { TimeLow = 2084825365, TimeMid = 19383, TimeHiAndVersion = 19084, ClockSeqHiAndReserved = 135, ClockSeqLow = 202, Node0 = 185,
                    Node1 = 247, Node2 = 224, Node3 = 33, Node4 = 255, Node5 = 117, },
                Guid.Parse("7c43e915-4bb7-4a8c-87ca-b9f7e021ff75"),
            },
            new object[]
            {
                new MagicLeapNativeBindings.MLUUID()
                { TimeLow = 3673749188, TimeMid = 41144, TimeHiAndVersion = 19572, ClockSeqHiAndReserved = 152, ClockSeqLow = 73, Node0 = 94,
                    Node1 = 175, Node2 = 42, Node3 = 128, Node4 = 243, Node5 = 59, },
                Guid.Parse("daf8f6c4-a0b8-4c74-9849-5eaf2a80f33b"),
            },
            new object[]
            {
                new MagicLeapNativeBindings.MLUUID()
                { TimeLow = 3382984881, TimeMid = 30024, TimeHiAndVersion = 17420, ClockSeqHiAndReserved = 167, ClockSeqLow = 171, Node0 = 188,
                    Node1 = 169, Node2 = 33, Node3 = 36, Node4 = 174, Node5 = 188, },
                Guid.Parse("c9a440b1-7548-440c-a7ab-bca92124aebc"),
            },
        };

        [Test]
        [TestCaseSource(nameof(MLUUID_To_GUID_ToUnity_Test_Cases))]
        public void MLUUID_To_GUID_ToUnity_Test(MagicLeapNativeBindings.MLUUID @case, Guid result)
        {
            var converted = UnityEngine.XR.MagicLeap.Native.MLConvert.ToUnity(@case);
            Assert.IsTrue(converted == result);
        }
        
        public static object[] From_Matrix4x4_To_MLTransform_Test_Cases =
        {
            new object[] { Matrix4x4.identity, true, MagicLeapNativeBindings.MLTransform.Identity() },
            
            new object[] { Matrix4x4.TRS(Vector3.forward, Quaternion.identity, Vector3.one), true,
                new MagicLeapNativeBindings.MLTransform ()
                {
                    Position = new MagicLeapNativeBindings.MLVec3f() {Z = -1f},
                    Rotation = new MagicLeapNativeBindings.MLQuaternionf() {W = -1f}
                }},
            
            new object[] { Matrix4x4.TRS(new Vector3 (10f, 5, 1f), Quaternion.Euler(90f, 180f, 45f), Vector3.one), true,
                new MagicLeapNativeBindings.MLTransform ()
                {
                    Position = new MagicLeapNativeBindings.MLVec3f() {X = 10f, Y = 5f , Z = -1f},
                    Rotation = new MagicLeapNativeBindings.MLQuaternionf() { X = -0.270598024f, Y = -0.65328151f, Z = -0.65328151f, W = 0.270598054f }
                } },
                
            new object[] { Matrix4x4.TRS(Vector3.forward, Quaternion.identity, Vector3.one), false, 
                MagicLeapNativeBindings.MLTransform.Identity() },
        };

        [Test]
        [TestCaseSource(nameof(From_Matrix4x4_To_MLTransform_Test_Cases))]
        public void From_Matrix4x4_To_MLTransform_Test(Matrix4x4 @case, bool transformFromRUF , MagicLeapNativeBindings.MLTransform result)
        {
            MagicLeapNativeBindings.MLTransform converted = UnityEngine.XR.MagicLeap.Native.MLConvert.FromUnity(@case, transformFromRUF);
         
            Assert.IsTrue(result.Position.ToVector3() == converted.Position.ToVector3());
            Assert.IsTrue(converted.Rotation.X == result.Rotation.X);
            Assert.IsTrue(converted.Rotation.Y == result.Rotation.Y);
            Assert.IsTrue(converted.Rotation.Z == result.Rotation.Z);
            Assert.IsTrue(converted.Rotation.W == result.Rotation.W);
        }
        
        public static object[] From_Matrix4x4_To_Array_Test_Cases =
        {
            new object[] { Matrix4x4.zero, new float[16],
                new[] { 0f, 0.0f, 0.0f, 0.0f, 0.0f, 0f, 0.0f, 0.0f, 0.0f, 0.0f, 0f, 0.0f, 0.0f, 0.0f, 0.0f, 0f } },
            new object[] { Matrix4x4.identity, new float[16],
                new[] { 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f, 0.0f, 0.0f, 0.0f, 0.0f, 1f } },
        };
        
        [Test]
        [TestCaseSource(nameof(From_Matrix4x4_To_Array_Test_Cases))]
        public void From_Matrix4x4_To_Array_Test(Matrix4x4 @case, float[] converted, float[] result)
        {
            UnityEngine.XR.MagicLeap.Native.MLConvert.FromUnity(@case, ref converted);
            
            for (int i = 0; i < converted.Length; i++) 
                Assert.IsTrue(converted[i] == result[i]);
        }

        public static object[] From_Vector3_To_MLVec3f_Test_Cases =
        {
            new object[] { new Vector3(0, 0, 0), true, new Vector3(0, 0, 0) },
            new object[] { new Vector3(0, 0, 1), true, new Vector3(0, 0, -1) },
            new object[] { new Vector3(0, 0, 1), false, new Vector3(0, 0, 1) },
        };

        [Test]
        [TestCaseSource(nameof(From_Vector3_To_MLVec3f_Test_Cases))]
        public void From_Vector3_To_MLVec3f_Test(Vector3 @case, bool transformFromRUF, Vector3 result)
        {
            var converted = UnityEngine.XR.MagicLeap.Native.MLConvert.FromUnity(@case, transformFromRUF);
            Assert.IsTrue(converted.ToVector3() == result);
        }

        public static object[] From_MLQuaternionf_To_Quaternion_Test_Cases =
        {
            new object[] { Quaternion.identity, false, MagicLeapNativeBindings.MLQuaternionf.Identity() },
            new object[] { Quaternion.identity, true, new MagicLeapNativeBindings.MLQuaternionf { X = 0, Y = 0, Z = 0, W = -1 } },
            new object[] { new Quaternion(0, 0, 0, 0), true, new MagicLeapNativeBindings.MLQuaternionf { X = 0, Y = 0, Z = 0, W = 0 }, },
            new object[] { new Quaternion(0, 0, 1, 1), true, new MagicLeapNativeBindings.MLQuaternionf { X = 0, Y = 0, Z = -1, W = -1 }, },
            new object[] { new Quaternion(0, 0, 1, 1), false, new MagicLeapNativeBindings.MLQuaternionf { X = 0, Y = 0, Z = 1, W = 1 }, },
        };

        [Test]
        [TestCaseSource(nameof(From_MLQuaternionf_To_Quaternion_Test_Cases))]
        public void From_MLQuaternionf_To_Quaternion_Test(Quaternion @case, bool transformFromRUF, MagicLeapNativeBindings.MLQuaternionf result)
        {
            MagicLeapNativeBindings.MLQuaternionf converted = UnityEngine.XR.MagicLeap.Native.MLConvert.FromUnity(@case, transformFromRUF);
            Assert.IsTrue(converted.X == result.X);
            Assert.IsTrue(converted.Y == result.Y);
            Assert.IsTrue(converted.Z == result.Z);
            Assert.IsTrue(converted.W == result.W);
        }

        public static object[] From_GUID_To_MLUUID_Test_Cases =
        {
            new object[]
            {
                Guid.Parse("7c43e915-4bb7-4a8c-87ca-b9f7e021ff75"), new MagicLeapNativeBindings.MLUUID()
                {
                    TimeLow = 2084825365, TimeMid = 19383, TimeHiAndVersion = 19084, ClockSeqHiAndReserved = 135, ClockSeqLow = 202, Node0 = 185,
                    Node1 = 247, Node2 = 224, Node3 = 33, Node4 = 255, Node5 = 117,
                }
            },
            new object[]
            {
                Guid.Parse("daf8f6c4-a0b8-4c74-9849-5eaf2a80f33b"), new MagicLeapNativeBindings.MLUUID()
                {
                    TimeLow = 3673749188, TimeMid = 41144, TimeHiAndVersion = 19572, ClockSeqHiAndReserved = 152, ClockSeqLow = 73, Node0 = 94,
                    Node1 = 175, Node2 = 42, Node3 = 128, Node4 = 243, Node5 = 59,
                }
            },
            new object[]
            {
                Guid.Parse("c9a440b1-7548-440c-a7ab-bca92124aebc"), new MagicLeapNativeBindings.MLUUID()
                {
                    TimeLow = 3382984881, TimeMid = 30024, TimeHiAndVersion = 17420, ClockSeqHiAndReserved = 167, ClockSeqLow = 171, Node0 = 188,
                    Node1 = 169, Node2 = 33, Node3 = 36, Node4 = 174, Node5 = 188,
                }
            },
        };

        [Test]
        [TestCaseSource(nameof(From_GUID_To_MLUUID_Test_Cases))]
        public void From_GUID_To_MLUUID_Test(Guid @case, MagicLeapNativeBindings.MLUUID result)
        {
            MagicLeapNativeBindings.MLUUID converted = UnityEngine.XR.MagicLeap.Native.MLConvert.FromUnity(@case);

            Assert.IsTrue(result.TimeLow == converted.TimeLow);
            Assert.IsTrue(result.TimeMid == converted.TimeMid);
            Assert.IsTrue(result.TimeHiAndVersion == converted.TimeHiAndVersion);
            Assert.IsTrue(result.ClockSeqHiAndReserved == converted.ClockSeqHiAndReserved);
            Assert.IsTrue(result.ClockSeqLow == converted.ClockSeqLow);
            Assert.IsTrue(result.Node0 == converted.Node0);
            Assert.IsTrue(result.Node1 == converted.Node1);
            Assert.IsTrue(result.Node2 == converted.Node2);
            Assert.IsTrue(result.Node3 == converted.Node3);
            Assert.IsTrue(result.Node4 == converted.Node4);
            Assert.IsTrue(result.Node5 == converted.Node5);
        }
    }
}
