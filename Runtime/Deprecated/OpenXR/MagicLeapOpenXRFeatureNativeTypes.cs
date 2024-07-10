using System;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapOpenXRFeatureNativeTypes
    {
        internal struct XrBool32
        {
            private uint value;
            public static implicit operator bool(XrBool32 input) => input.value > 0;

            public static implicit operator XrBool32(bool value) =>
                new()
                {
                    value = value ? 1U : 0
                };
        }
        
        internal struct XrRect2Di
        {
            internal Vector2Int Offset;
            internal Vector2Int Extent;
        }

        internal struct XrFOV
        {
            internal float AngleLeft;
            internal float AngleRight;
            internal float AngleUp;
            internal float AngleDown;
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

        internal unsafe struct XrBaseOutStructure
        {
            internal ulong Type;
            internal XrBaseOutStructure* Next;
        }
    }
}
