using System;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapOpenXRFeatureNativeTypes;
using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapSpaceInfoNativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    namespace MagicLeapLocateSpacesNativeTypes
    {
        
        internal enum XrLocateSpaceStructTypes : ulong
        {
            XRTypeSpacesLocateInfo = 1000471000U,
            XRTypeSpaceLocations = 1000471001U,
            XRTypeSpaceVelocities = 1000471002U
        }

        internal enum XrSpaceVelocityFlags : ulong
        {
            LinearValid =  0x00000001,
            AngularValid =  0x00000002
        }

        internal unsafe struct XrSpaceLocateInfo
        {
            internal XrLocateSpaceStructTypes Type;
            internal IntPtr Next;
            internal ulong BaseSpace;
            internal long Time;
            internal uint SpaceCount;
            internal ulong* Spaces;
        }

        internal struct XrSpaceLocationData
        {
            internal XrSpaceLocationFlagsML LocationFlags;
            internal XrPose Pose;

            internal XrPose GetValidPose()
            {
                var result = XrPose.IdentityPose;
                if (LocationFlags.HasFlag(XrSpaceLocationFlagsML.PositionValid))
                {
                    result.Position = Pose.Position;
                }

                if (LocationFlags.HasFlag(XrSpaceLocationFlagsML.OrientationValid))
                {
                    result.Rotation = Pose.Rotation;
                }
                return result;
            }
        }

        internal unsafe struct XrSpaceLocations
        {
            internal XrLocateSpaceStructTypes Type;
            internal IntPtr Next;
            internal uint LocationCount;
            internal XrSpaceLocationData* Locations;
        }

        internal struct XrSpaceVelocityData
        {
            internal XrSpaceVelocityFlags VelocityFlags;
            internal Vector3 LinearVelocity;
            internal Vector3 AngularVelocity;
        }
        
        internal unsafe struct XrSpaceVelocities
        {
            internal XrLocateSpaceStructTypes Type;
            internal IntPtr Next;
            internal uint VelocityCount;
            internal XrSpaceVelocityData* Velocities;
        }
    }
}
