using System;
using System.Runtime.InteropServices;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapOpenXRFeatureNativeTypes
    {
        internal delegate XrResult GetInstanceProcAddr(ulong instance, [MarshalAs(UnmanagedType.LPStr)] string name, ref IntPtr pointer);

        internal struct XrBool32
        {
            private uint Value;
            public static implicit operator bool(XrBool32 input) => input.Value > 0;

            public static implicit operator XrBool32(bool value) =>
                new ()
                {
                    Value = value ? 1U : 0
                };
        }
        
        internal struct XrEventBuffer
        {
            internal ulong Type;
            internal IntPtr Next;
            internal IntPtr Varying;
        }
        
        internal struct XrPose
        {
            internal Quaternion Rotation;
            internal Vector3 Position;

            internal static XrPose GetFromPose(Pose pose, bool shouldConvert = true)
            {
                var result = new XrPose
                {
                    Position = shouldConvert ? pose.position.ConvertBetweenUnityOpenXr() : pose.position,
                    Rotation = shouldConvert ? pose.rotation.ConvertBetweenUnityOpenXr() : pose.rotation
                };
                return result;
            }

            internal static Pose GetUnityPose(in XrPose pose, bool shouldConvert = true)
            {
                var result = new Pose
                {
                    position = shouldConvert ? pose.Position.ConvertBetweenUnityOpenXr() : pose.Position,
                    rotation = shouldConvert ? pose.Rotation.ConvertBetweenUnityOpenXr() : pose.Rotation
                };
                return result;
            }

            internal static XrPose IdentityPose => GetFromPose(Pose.identity);
        }
    }
}
