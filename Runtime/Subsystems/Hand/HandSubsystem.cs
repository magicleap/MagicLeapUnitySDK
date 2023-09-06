
#if XR_HANDS
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.XR.Hands;
using UnityEngine.XR.Hands.ProviderImplementation;
using UnityEngine.XR.Management;
using Unity.Collections.LowLevel.Unsafe;
using NativeBindings = UnityEngine.XR.OpenXR.Features.MagicLeapSupport.MagicLeapFeature.NativeBindings;

namespace UnityEngine.XR.MagicLeap
{
    using HandTracking = InputSubsystem.Extensions.MLHandTracking;

    public class HandSubsystem : XRHandSubsystem
    {
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
            XRHandSubsystemDescriptor.Register(new XRHandSubsystemDescriptor.Cinfo
            {
                id = MagicLeapXrProvider.HandSubsystemId,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(MagicLeapProvider),
                subsystemTypeOverride = typeof(HandSubsystem),
#else
                subsystemImplementationType = typeof(HandsSubsystem),
#endif

            });
        }
    }

    public class MagicLeapProvider : XRHandSubsystemProvider
    {
        private XRHandProviderUtility.SubsystemUpdater subsystemUpdater;
        private List<int> unsupportedIndices = new List<int>() { XRHandJointID.Palm.ToIndex() };
#if !UNITY_OPENXR_1_7_0_OR_NEWER
        private bool isHandTrackingStarted;
        private HandDevice leftHandDevice;
        private HandDevice rightHandDevice;     
#else
        private XRHandSubsystem subsystem;
        private List<XRHandJointID> jointOrder;
#endif
        public override void GetHandLayout(NativeArray<bool> handJointsInLayout)
        {
#if UNITY_OPENXR_1_7_0_OR_NEWER
            unsupportedIndices.Clear();
#endif
            for (int i = 0; i < handJointsInLayout.Length; ++i)
            {
                if (unsupportedIndices.Contains(i))
                    handJointsInLayout[i] = false;
                else
                    handJointsInLayout[i] = true;
            }
        }


        public override void Start()
        {
#if !UNITY_OPENXR_1_7_0_OR_NEWER
            leftHandDevice = new HandDevice(InputDeviceCharacteristics.Left);
            rightHandDevice = new HandDevice(InputDeviceCharacteristics.Right);      
#endif
            subsystem = XRGeneralSettings.Instance?.Manager?.activeLoader?.GetLoadedSubsystem<HandSubsystem>();
            subsystemUpdater = new XRHandProviderUtility.SubsystemUpdater(subsystem);
            subsystemUpdater.Start();

            jointOrder = new List<XRHandJointID> 
            { 
                XRHandJointID.Palm, XRHandJointID.Wrist, XRHandJointID.ThumbMetacarpal, XRHandJointID.ThumbProximal, XRHandJointID.ThumbDistal, XRHandJointID.ThumbTip,
                XRHandJointID.IndexMetacarpal, XRHandJointID.IndexProximal, XRHandJointID.IndexIntermediate, XRHandJointID.IndexDistal, XRHandJointID.IndexTip,
                XRHandJointID.MiddleMetacarpal, XRHandJointID.MiddleProximal, XRHandJointID.MiddleIntermediate, XRHandJointID.MiddleDistal, XRHandJointID.MiddleTip,
                XRHandJointID.RingMetacarpal, XRHandJointID.RingProximal, XRHandJointID.RingIntermediate, XRHandJointID.RingDistal, XRHandJointID.RingTip,
                XRHandJointID.LittleMetacarpal, XRHandJointID.LittleProximal, XRHandJointID.LittleIntermediate, XRHandJointID.LittleDistal, XRHandJointID.LittleTip
            };

        }
        public override XRHandSubsystem.UpdateSuccessFlags TryUpdateHands(XRHandSubsystem.UpdateType updateType, ref Pose leftHandRootPose, NativeArray<XRHandJoint> leftHandJoints, ref Pose rightHandRootPose, NativeArray<XRHandJoint> rightHandJoints)
        {
            var successFlags = XRHandSubsystem.UpdateSuccessFlags.None;
#if !UNITY_OPENXR_1_7_0_OR_NEWER
            if (!isHandTrackingStarted)
            {
                // Do not try to start permissions api if MLDevice has not yet been created.
                if (GameObject.FindObjectOfType<MLDevice>() == null)
                    return successFlags;

                // HANDTRACKING is a normal permission, so we don't request it at runtime. It is auto-granted if included in the app's manifest.
                // If it's missing from the manifest, the permission is not available.
                if (!MLPermissions.CheckPermission(MLPermission.HandTracking).IsOk)
                {
                    Debug.LogError($"You must include the {MLPermission.HandTracking} permission in the AndroidManifest.xml to run this example.");
                    return successFlags;
                }

                InputSubsystem.Extensions.MLHandTracking.StartTracking();
                InputSubsystem.Extensions.MLHandTracking.SetPreRenderHandUpdate(true);
                isHandTrackingStarted = true;
            }

            if (!leftHandDevice.Device.isValid)
            {
                leftHandDevice.SetDevice();
                return successFlags;
            }

            leftHandDevice.Device.TryGetFeatureValue(CommonUsages.devicePosition, out leftHandRootPose.position);
            leftHandDevice.Device.TryGetFeatureValue(CommonUsages.deviceRotation, out leftHandRootPose.rotation);
            successFlags = XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose;

            if (leftHandJoints.Length > 0)
            {
                leftHandDevice.FillJoints(ref leftHandJoints);
                successFlags |= XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints;
            }

            if (!rightHandDevice.Device.isValid)
            {
                rightHandDevice.SetDevice();
                return successFlags;
            }

            rightHandDevice.Device.TryGetFeatureValue(CommonUsages.devicePosition, out rightHandRootPose.position);
            rightHandDevice.Device.TryGetFeatureValue(CommonUsages.deviceRotation, out rightHandRootPose.rotation);
            successFlags |= XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose;

            if (rightHandJoints.Length > 0)
            {
                rightHandDevice.FillJoints(ref rightHandJoints);
                successFlags |= XRHandSubsystem.UpdateSuccessFlags.RightHandJoints;
            }
#else
            using var leftJoints = new NativeArray<NativeBindings.XrHandJointLocationEXT>(26 /* Hand Joints max*/, Allocator.Temp);
            using var rightJoints = new NativeArray<NativeBindings.XrHandJointLocationEXT>(26 /* Hand Joints max*/, Allocator.Temp);
            unsafe
            {
                MLResult.Code result = NativeBindings.MLOpenXRGetXRHandTrackingJoints(NativeBindings.XrHandEXT.Left, (NativeBindings.XrHandJointLocationEXT*)leftJoints.GetUnsafePtr());

                if (result == MLResult.Code.Ok)
                {
                    successFlags = XRHandSubsystem.UpdateSuccessFlags.LeftHandRootPose;
                    successFlags |= XRHandSubsystem.UpdateSuccessFlags.LeftHandJoints;


                    for (int jointIndex = 0; jointIndex < leftJoints.Length; jointIndex++)
                    {
                        Pose currentPose = new Pose();
                        currentPose.position = new Vector3(leftJoints[jointIndex].pose.position.x, leftJoints[jointIndex].pose.position.y, -leftJoints[jointIndex].pose.position.z);
                        currentPose.rotation = new Quaternion(-leftJoints[jointIndex].pose.orientation.x, -leftJoints[jointIndex].pose.orientation.y, leftJoints[jointIndex].pose.orientation.z, leftJoints[jointIndex].pose.orientation.w);

                        XRHandJointTrackingState currentState = XRHandJointTrackingState.Pose;
                        currentState |= XRHandJointTrackingState.Radius;

                        leftHandJoints[jointOrder[jointIndex].ToIndex()] = XRHandProviderUtility.CreateJoint(Handedness.Left, currentState, XRHandJointIDUtility.FromIndex(jointOrder[jointIndex].ToIndex()), currentPose, leftJoints[jointIndex].radius);

                        if (jointOrder[jointIndex] == XRHandJointID.Wrist)
                        {
                            leftHandRootPose = currentPose;
                        }
                    }
                }

                leftJoints.Dispose();

                result = NativeBindings.MLOpenXRGetXRHandTrackingJoints(NativeBindings.XrHandEXT.Right, (NativeBindings.XrHandJointLocationEXT*)rightJoints.GetUnsafePtr());

                if (result == MLResult.Code.Ok)
                {
                    successFlags |= XRHandSubsystem.UpdateSuccessFlags.RightHandRootPose;
                    successFlags |= XRHandSubsystem.UpdateSuccessFlags.RightHandJoints;

                    for (int jointIndex = 0; jointIndex < rightJoints.Length; jointIndex++)
                    {
                        Pose currentPose = new Pose();
                        currentPose.position = new Vector3(rightJoints[jointIndex].pose.position.x, rightJoints[jointIndex].pose.position.y, -rightJoints[jointIndex].pose.position.z);
                        currentPose.rotation = new Quaternion(-rightJoints[jointIndex].pose.orientation.x, -rightJoints[jointIndex].pose.orientation.y, rightJoints[jointIndex].pose.orientation.z, rightJoints[jointIndex].pose.orientation.w);

                        XRHandJointTrackingState currentState = XRHandJointTrackingState.Pose;
                        currentState |= XRHandJointTrackingState.Radius;

                        rightHandJoints[jointOrder[jointIndex].ToIndex()] = XRHandProviderUtility.CreateJoint(Handedness.Right, currentState, XRHandJointIDUtility.FromIndex(jointOrder[jointIndex].ToIndex()), currentPose, rightJoints[jointIndex].radius);

                        if (jointOrder[jointIndex] == XRHandJointID.Wrist)
                        {
                            rightHandRootPose = currentPose;
                        }
                    }
                }

                rightJoints.Dispose();
            }
#endif
            return successFlags;
        }

        public override void Stop()
        {
#if !UNITY_OPENXR_1_7_0_OR_NEWER
            InputSubsystem.Extensions.MLHandTracking.StopTracking();
#endif
            subsystemUpdater.Stop();
        }

        public override void Destroy() => subsystemUpdater.Destroy();
#if !UNITY_OPENXR_1_7_0_OR_NEWER
        private class HandDevice
        {
            public HandDevice(InputDeviceCharacteristics characteristics)
            {
                this.characteristics = characteristics;
                SetDevice();
            }

            public void SetDevice() => device = InputSubsystem.Utils.FindMagicLeapDevice(InputDeviceCharacteristics.HandTracking | characteristics);
            public InputDevice Device => device;

            private InputDeviceCharacteristics characteristics;
            private InputDevice device;
            private List<Bone> thumbBones = new List<Bone>();
            private List<Bone> indexFingerBones = new List<Bone>();
            private List<Bone> middleFingerBones = new List<Bone>();
            private List<Bone> ringFingerBones = new List<Bone>();
            private List<Bone> pinkyFingerBones = new List<Bone>();

            public void FillJoints(ref NativeArray<XRHandJoint> joints)
            {
                if (device.TryGetFeatureValue(CommonUsages.handData, out UnityEngine.XR.Hand hand))
                {
                    hand.TryGetFingerBones(HandFinger.Thumb, thumbBones);
                    hand.TryGetFingerBones(HandFinger.Index, indexFingerBones);
                    hand.TryGetFingerBones(HandFinger.Middle, middleFingerBones);
                    hand.TryGetFingerBones(HandFinger.Ring, ringFingerBones);
                    hand.TryGetFingerBones(HandFinger.Pinky, pinkyFingerBones);

                    device.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Hand.WristCenter, out Bone wristBone);
                    wristBone.TryGetPosition(out Vector3 wristPosition);
                    wristBone.TryGetRotation(out Quaternion wristRotation);
                    var wristPose = new Pose(wristPosition, wristRotation);

                    joints[XRHandJointID.Wrist.ToIndex()] = XRHandProviderUtility.CreateJoint(XRHandJointTrackingState.Pose, XRHandJointID.Wrist, wristPose);

                    // ML does not support the Palm joint, so make it's pose the same as the wrist.
                    joints[XRHandJointID.Palm.ToIndex()] = XRHandProviderUtility.CreateJoint(XRHandJointTrackingState.Pose, XRHandJointID.Palm, wristPose);

                    int jointIndex = XRHandJointID.Palm.ToIndex() + 1;
                    ConvertBoneArray(ref jointIndex, ref joints, thumbBones);
                    ConvertBoneArray(ref jointIndex, ref joints, indexFingerBones);
                    ConvertBoneArray(ref jointIndex, ref joints, middleFingerBones);
                    ConvertBoneArray(ref jointIndex, ref joints, ringFingerBones);
                    ConvertBoneArray(ref jointIndex, ref joints, pinkyFingerBones);
                }
            }

            private void ConvertBoneArray(ref int jointIndex, ref NativeArray<XRHandJoint> joints, List<Bone> bones)
            {
                // ML has it's bone arrays filled in the opposite direction that Unity is asking for so walk through the bones array backwards.
                for (int boneIndex = bones.Count - 1; boneIndex >= 0; --boneIndex)
                {
                    joints[jointIndex] = BoneToJoint(XRHandJointIDUtility.FromIndex(jointIndex), bones[boneIndex]);
                    jointIndex++;
                }
            }

            private XRHandJoint BoneToJoint(XRHandJointID jointId, Bone bone)
            {
                bone.TryGetPosition(out Vector3 pos);
                bone.TryGetRotation(out Quaternion rot);
                return XRHandProviderUtility.CreateJoint(XRHandJointTrackingState.Pose, jointId, new Pose(pos, rot));
            }
        }
#endif
    }

}
#endif
