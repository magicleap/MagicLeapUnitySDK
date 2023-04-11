// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices;

    public partial class InputSubsystem
    {
        public static partial class Extensions
        {
            public static class MLHandTracking
            {
                /// <summary>
                /// The max number of key points to track.
                /// </summary>
                public const int MaxKeyPoints = 28;

                /// <summary>
                /// Represents if a hand is the right or left hand.
                /// </summary>
                public enum HandType
                {
                    /// <summary>
                    /// Left hand.
                    /// </summary>
                    Left,

                    /// <summary>
                    /// Right hand.
                    /// </summary>
                    Right
                }

                public enum KeyPoints
                {
                    Thumb_Tip = 0,
                    Thumb_IP,
                    Thumb_MCP,
                    Thumb_CMC,
                    Index_Tip,
                    Index_DIP,
                    Index_PIP,
                    Index_MCP,
                    Middle_Tip,
                    Middle_DIP,
                    Middle_PIP,
                    Middle_MCP,
                    Ring_Tip,
                    Ring_DIP,
                    Ring_PIP,
                    Ring_MCP,
                    Pinky_Tip,
                    Pinky_DIP,
                    Pinky_PIP,
                    Pinky_MCP,
                    Wrist_Center,
                    Wrist_Ulnar,
                    Wrist_Radial,
                    Hand_Center,
                    Index_CMC,
                    Middle_CMC,
                    Ring_CMC,
                    Pinky_CMC
                }

                public enum KeyPointLocation
                {
                    Thumb = 0,
                    Index = 4,
                    Middle = 8,
                    Ring = 12,
                    Pinky = 16,
                    Wrist = 20,
                    Center = 23,
                    FifthBone = 24
                }

                public static void StartTracking() => MagicLeapXrProviderNativeBindings.StartHandTracking();

                public static void StopTracking() => MagicLeapXrProviderNativeBindings.StopHandTracking();

                /// <summary>
                /// By default the keypoints data is updated twice. To turn this off
                /// set enable to false to potentially improve performance. This is not
                /// recommended if keypoints are visual in the app as it will
                /// significantly decrease the smoothness of visuals.
                /// </summary>
                public static void SetPreRenderHandUpdate(bool enable = true)
                {
                    NativeBindings.SetPreRenderPoseUpdate(enable);
                }

                public static bool TryGetKeyPointsMask(InputDevice handDevice, out bool[] keyPointsMask) => NativeBindings.TryGetKeyPointsMask(handDevice, out keyPointsMask);

                public static string GetKeyPointName(KeyPointLocation location, int keyPointIndex) => NativeBindings.GetKeyPointName(location, keyPointIndex);

                public static bool GetKeyPointStatus(InputDevice handDevice, KeyPointLocation location, int keyPointIndex) => NativeBindings.GetKeyPointStatus(handDevice, location, keyPointIndex);

                internal static class NativeBindings
                {
                    private static byte[] allocatedKeyPointsMaskData = new byte[Marshal.SizeOf<NativeBindings.KeyPointsMask>()];

                    public static bool TryGetKeyPointsMask(InputDevice handDevice, out bool[] keyPointsMask)
                    {
                        if (!handDevice.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Hand.KeyPointsMask, allocatedKeyPointsMaskData))
                            goto Failure;

                        try
                        {
                            IntPtr ptr = Marshal.AllocHGlobal(allocatedKeyPointsMaskData.Length);
                            Marshal.Copy(allocatedKeyPointsMaskData, 0, ptr, allocatedKeyPointsMaskData.Length);
                            var nativeStruct = Marshal.PtrToStructure<NativeBindings.KeyPointsMask>(ptr);
                            Marshal.FreeHGlobal(ptr);
                            keyPointsMask = new bool[MaxKeyPoints];

                            for (int i = 0; i < MaxKeyPoints; i++)
                            {
                                keyPointsMask[i] = Convert.ToBoolean(nativeStruct.Mask[i]);
                            }

                            return true;
                        }

                        catch (Exception e)
                        {
                            Debug.LogError("TryGetKeyPointsMask failed with the exception: " + e);
                            goto Failure;
                        }

                    Failure:
                        keyPointsMask = new bool[MaxKeyPoints];
                        return false;
                    }

                    public static string GetKeyPointName(KeyPointLocation location, int keyPointIndex)
                    {
                        int keypointEnum = (int)location + keyPointIndex;

                        if ((int)location > 0 && keyPointIndex == 4)
                        {
                            keypointEnum = FifthBoneKeypointValue(location);
                            location = KeyPointLocation.FifthBone;
                        }

                        if (!CheckKeypointEnumValid(location, keypointEnum))
                        {
                            return "Invalid KeyPoint";
                        }

                        return Enum.GetName(typeof(KeyPoints), keypointEnum);
                    }

                    public static bool GetKeyPointStatus(InputDevice handDevice, KeyPointLocation location, int keyPointIndex)
                    {
                        if (TryGetKeyPointsMask(handDevice, out bool[] handKeyPoints))
                        {
                            int keypointEnum = (int)location + keyPointIndex;

                            if ((int)location > 0 && keyPointIndex == 4)
                            {
                                keypointEnum = FifthBoneKeypointValue(location);
                                location = KeyPointLocation.FifthBone;
                            }

                            if (!CheckKeypointEnumValid(location, keypointEnum))
                            {
                                return false;
                            }

                            return handKeyPoints[keypointEnum];
                        }

                        return false;
                    }

                    private static bool CheckKeypointEnumValid(KeyPointLocation location, int keypointEnum)
                    {
                        if (keypointEnum < 0 || keypointEnum >= MaxKeyPoints)
                        {
                            return false;
                        }

                        Array enumValues = Enum.GetValues(typeof(KeyPointLocation));
                        int locationValue = (int)location;
                        int nextValue = MaxKeyPoints;
                        for (int i = 0; i < enumValues.Length - 1; i++)
                        {
                            if ((int)enumValues.GetValue(i) == locationValue)
                            {
                                nextValue = (int)enumValues.GetValue(i + 1);
                                break;
                            }
                        }

                        if (keypointEnum >= nextValue)
                        {
                            return false;
                        }

                        return true;
                    }

                    // Should not be used for Thumb.
                    private static int FifthBoneKeypointValue(KeyPointLocation location)
                    {
                        switch (location)
                        {
                            case KeyPointLocation.Index:
                                return (int)KeyPointLocation.FifthBone;
                            case KeyPointLocation.Middle:
                                return ((int)KeyPointLocation.FifthBone + 1);
                            case KeyPointLocation.Ring:
                                return ((int)KeyPointLocation.FifthBone + 2);
                            case KeyPointLocation.Pinky:
                                return ((int)KeyPointLocation.FifthBone + 3);
                            default:
                                return (int)location;
                        }
                    }

                    /// <summary>
                    /// Native call for pre render Keypoints update.
                    /// </summary>
                    /// <param name="enable">bool to determine if pre render pose update should happen.</param>
                    [DllImport(MagicLeapXrProviderNativeBindings.MagicLeapXrProviderDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern void SetPreRenderPoseUpdate(bool enable);

                    [StructLayout(LayoutKind.Sequential)]
                    public readonly struct KeyPointsMask
                    {
                        [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.I1, SizeConst = (int)MaxKeyPoints)]
                        public readonly byte[] Mask;
                    }
                }
            }
        }
    }
}
