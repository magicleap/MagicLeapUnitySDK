using System;
using MagicLeap.OpenXR.Features;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace MagicLeap.OpenXR.Spaces
{
    internal enum XrSpaceLocationFlagsML : ulong
    {
        OrientationValid = 0x00000001,
        PositionValid = 0x00000002,
        OrientationTracked = 0x00000004,
        PositionTracked = 0x00000008,
    }

    internal struct XrSpaceLocation
    {
        internal const ulong XrSpaceLocationStructType = 42;
        internal ulong Type;
        internal IntPtr Next;
        internal XrSpaceLocationFlagsML SpaceLocationFlags;
        internal XrPose Pose;
    }

    internal enum XrLocateSpaceStructTypes : ulong
    {
        XRTypeSpacesLocateInfo = 1000471000U,
        XRTypeSpaceLocations = 1000471001U,
        XRTypeSpaceVelocities = 1000471002U
    }

    internal enum XrSpaceVelocityFlags : ulong
    {
        LinearValid = 0x00000001,
        AngularValid = 0x00000002
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

    internal unsafe class SpacesNativeFunctions : NativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, ulong, long, out XrSpaceLocation, XrResult> XrLocateSpace;
        internal delegate* unmanaged [Cdecl]<ulong, XrResult> XrDestroySpace;
        internal delegate* unmanaged [Cdecl] <ulong, in XrSpaceLocateInfo, out XrSpaceLocations, XrResult> XrLocateSpaces;
        
        protected override void LocateNativeFunctions()
        {
            XrLocateSpace = (delegate* unmanaged[Cdecl]<ulong, ulong, long, out XrSpaceLocation, XrResult>)LocateNativeFunction("xrLocateSpace");
            XrDestroySpace = (delegate* unmanaged[Cdecl]<ulong, XrResult>)LocateNativeFunction("xrDestroySpace");
            XrLocateSpaces = (delegate* unmanaged[Cdecl]<ulong, in XrSpaceLocateInfo, out XrSpaceLocations, XrResult>)LocateNativeFunction("xrLocateSpacesKHR");
        }

        internal XrPose[] LocateSpaces(ulong baseSpace, long predictedDisplayTime, ulong session, ulong[] spaces)
        {
            var result = new XrPose[spaces.Length];
            Array.Fill(result, XrPose.IdentityPose);

            var spaceLocateInfo = new XrSpaceLocateInfo
            {
                Type = XrLocateSpaceStructTypes.XRTypeSpacesLocateInfo,
                BaseSpace = baseSpace,
                Time = predictedDisplayTime,
                SpaceCount = (uint)spaces.Length
            };
            var spacesArray = new NativeArray<ulong>(spaces, Allocator.Temp);
            spaceLocateInfo.Spaces = (ulong*)spacesArray.GetUnsafePtr();

            var spaceLocationsArray = new NativeArray<XrSpaceLocationData>(spaces.Length, Allocator.Temp);
            var spaceLocations = new XrSpaceLocations
            {
                Type = XrLocateSpaceStructTypes.XRTypeSpaceLocations,
                LocationCount = (uint)spaces.Length,
                Locations = (XrSpaceLocationData*)spaceLocationsArray.GetUnsafePtr()
            };

            var xrResult = XrLocateSpaces(session, in spaceLocateInfo, out spaceLocations);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(XrLocateSpaces)))
            {
                return result;
            }

            result = new XrPose[spaces.Length];
            for (var i = 0; i < spaces.Length; i++)
            {
                result[i] = spaceLocationsArray[i].GetValidPose();
            }

            return result;
        }

        protected override void Validate()
        {
            base.Validate();
            if (XrLocateSpace == null)
            {
                Debug.LogError($"{nameof(SpacesNativeFunctions)} Validation Error: {nameof(XrLocateSpace)} was null");
            }
        }

        public Pose GetUnityPose(ulong space, ulong baseSpace, long nextPredictedDisplayTime)
        {
            var result = Pose.identity;
            var spaceLocation = new XrSpaceLocation
            {
                Type = XrSpaceLocation.XrSpaceLocationStructType,
            };
            var xrResult = XrLocateSpace(space, baseSpace, nextPredictedDisplayTime, out spaceLocation);
            if (!Utils.DidXrCallSucceed(xrResult, nameof(XrLocateSpace)))
            {
                return result;
            }

            if (spaceLocation.SpaceLocationFlags.HasFlag(XrSpaceLocationFlagsML.OrientationValid))
            {
                result.rotation = spaceLocation.Pose.Rotation.InvertXY();
            }

            if (spaceLocation.SpaceLocationFlags.HasFlag(XrSpaceLocationFlagsML.PositionValid))
            {
                result.position = spaceLocation.Pose.Position.InvertZ();
            }

            return result;
        }
    }
}
