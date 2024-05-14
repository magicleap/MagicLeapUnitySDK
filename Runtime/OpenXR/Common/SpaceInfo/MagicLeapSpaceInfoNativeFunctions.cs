using UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapSpaceInfoNativeTypes;
using UnityEngine.XR.OpenXR.NativeTypes;

namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    internal unsafe class MagicLeapSpaceInfoNativeFunctions : MagicLeapNativeFunctionsBase
    {
        internal delegate* unmanaged [Cdecl] <ulong, ulong, long, out XrSpaceLocation, XrResult> XrLocateSpace;

        protected override void LocateNativeFunctions()
        {
            XrLocateSpace = (delegate* unmanaged[Cdecl]<ulong, ulong, long, out XrSpaceLocation, XrResult>)LocateNativeFunction("xrLocateSpace");
        }

        protected override void Validate()
        {
            base.Validate();
            if (XrLocateSpace == null)
            {
                Debug.LogError($"{nameof(MagicLeapSpaceInfoNativeFunctions)} Validation Error: {nameof(XrLocateSpace)} was null");
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
                result.rotation = spaceLocation.Pose.Rotation.ConvertBetweenUnityOpenXr();
            }

            if (spaceLocation.SpaceLocationFlags.HasFlag(XrSpaceLocationFlagsML.PositionValid))
            {
                result.position = spaceLocation.Pose.Position.ConvertBetweenUnityOpenXr();
            }
            return result;
        }
    }
}
