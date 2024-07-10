using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.MagicLeap.Native;
using static UnityEngine.XR.MagicLeap.InputSubsystem.Extensions.MLHeadTracking.NativeBindings;

namespace UnityEngine.XR.MagicLeap
{
    public static partial class InputSubsystem
    {
        public static partial class Extensions
        {
            [Obsolete]
            public static bool TryGetHeadTrackingState(InputDevice headDevice, out MLHeadTracking.State headTrackingState) => MLHeadTracking.TryGetState(headDevice, out headTrackingState);

            public static bool TryGetHeadTrackingStateEx(InputDevice headDevice, out MLHeadTracking.StateEx headTrackingState) => MLHeadTracking.TryGetStateEx(headDevice, out headTrackingState);

            public static class MLHeadTracking
            {
                /// <summary>
                /// A set of possible error conditions that can cause Head Tracking to be less than ideal.
                /// </summary>
                [Obsolete]
                public enum TrackingError
                {
                    /// <summary>
                    /// No error, tracking is nominal.
                    /// </summary>
                    None,

                    /// <summary>
                    /// There are not enough features in the environment.
                    /// </summary>
                    NotEnoughFeatures,

                    /// <summary>
                    /// Lighting in the environment is not sufficient to track accurately.
                    /// </summary>
                    LowLight,

                    /// <summary>
                    /// Head tracking failed for an unknown reason.
                    /// </summary>
                    Unknown
                }

                /// <summary>
                /// A set of possible error conditions that can cause Head Tracking to be less than ideal.
                /// </summary>
                [Flags]
                public enum TrackingErrorFlag
                {
                    /// <summary>
                    /// No error, tracking is nominal.
                    /// </summary>
                    None = 0,

                    /// <summary>
                    /// Head tracking failed for an unknown reason.
                    /// </summary>
                    Unknown = 1 << 0,

                    /// <summary>
                    /// There are not enough features in the environment.
                    /// </summary>
                    NotEnoughFeatures = 1 << 1,

                    /// <summary>
                    /// Lighting in the environment is not sufficient to track accurately.
                    /// </summary>
                    LowLight = 1 << 2,

                    /// <summary>
                    /// Head tracking failed due to excessive motion.
                    /// </summary>
                    ExcessiveMotion = 1 << 3
                }

                /// <summary>
                /// A set of possible tracking modes the Head Tracking system can be in.
                /// </summary>
                [Obsolete]
                public enum TrackingMode
                {
                    /// <summary>
                    /// Full 6 degrees of freedom tracking (position and orientation).
                    /// </summary>
                    Mode6DOF,

                    /// <summary>
                    /// Head tracking is unavailable.
                    /// </summary>
                    ModeUnavailable
                }

                /// <summary>
                /// A set of possible tracking status for the Head Tracking system.
                /// </summary>
                public enum HeadTrackingStatus
                {
                    /// <summary>
                    /// Head tracking is unavailable.
                    /// </summary>
                    Invalid = 0,

                    /// <summary>
                    /// Head tracking is initializing.
                    /// </summary>
                    Initializing = 1,

                    /// <summary>
                    /// Head tracking is relocalizing.
                    /// </summary>
                    Relocalizing = 2,

                    /// <summary>
                    /// Valid head tracking data is available.
                    /// </summary>
                    Valid = 100
                }

                /// <summary>
                /// A set of all types of map events that can occur that a developer
                /// may have to handle.
                /// </summary>
                [Flags]
                public enum MapEvents : ulong
                {
                    /// <summary>
                    /// Map was lost. It could possibly recover.
                    /// </summary>
                    Lost = (1 << 0),

                    /// <summary>
                    /// Previous map was recovered.
                    /// </summary>
                    Recovered = (1 << 1),

                    /// <summary>
                    /// Failed to recover previous map.
                    /// </summary>
                    RecoveryFailed = (1 << 2),

                    /// <summary>
                    /// New map session created.
                    /// </summary>
                    NewSession = (1 << 3)
                }

                [Obsolete]
                public static bool TryGetState(InputDevice headDevice, out State headTrackingState) => NativeBindings.TryGetState(headDevice, out headTrackingState);

                public static bool TryGetStateEx(InputDevice headDevice, out StateEx headTrackingState)
                {
                    if(MLDevice.IsOpenXRLoaderActive())
                    {
                        return global::MagicLeap.OpenXR.LegacySupport.MLHeadTracking.TryGetStateEx(out headTrackingState);
                    }
                    else
                    {
                        return NativeBindings.TryGetStateEx(headDevice, out headTrackingState);
                    }
                }
                public static bool TryGetMapEvents(InputDevice headDevice, out MapEvents mapEvents) 
                {
                    if (MLDevice.IsOpenXRLoaderActive())
                    {
                        return global::MagicLeap.OpenXR.LegacySupport.MLHeadTracking.TryGetMapEvents(out mapEvents);
                    }
                    else
                    {
                        return NativeBindings.TryGetMapEvents(headDevice, out mapEvents);
                    }
                
                }

                private static bool gotData = false;
                private static NativeBindings.MLHeadTrackingStaticData data;
                public static void GetStaticData(out MagicLeapNativeBindings.MLCoordinateFrameUID outUID)
                {
                    outUID = MagicLeapNativeBindings.MLCoordinateFrameUID.EmptyFrame;
                    if (!gotData)
                    {
                        var headHandle = MagicLeapXrProviderNativeBindings.GetHeadTrackerHandle();
                        data = new();
                        MLResult result = MLResult.Create(MLHeadTrackingGetStaticData(headHandle, ref data));
                        if (!result.IsOk)
                        {
                            Debug.LogError($"MLHeadTracking::GetStaticData failed: {result}");
                            gotData = false;
                            return;
                        }
                    }

                    outUID = data.coord_frame_head;
                    return;
                }

                /// <summary>
                /// A structure containing information on the current state of the
                /// Head Tracking system.
                /// </summary>
                [Obsolete]
                public readonly struct State
                {
                    /// <summary>
                    /// What tracking mode the Head Tracking system is currently in.
                    /// </summary>
                    public readonly TrackingMode Mode;

                    /// <summary>
                    /// A confidence value (from 0..1) representing the confidence in the
                    /// current pose estimation.
                    /// </summary>
                    public readonly float Confidence;

                    /// <summary>
                    /// Represents what tracking error (if any) is present.
                    /// </summary>
                    public readonly TrackingError Error;

                    internal State(NativeBindings.State nativeState)
                    {
                        this.Mode = nativeState.Mode;
                        this.Confidence = nativeState.Confidence;
                        this.Error = nativeState.Error;
                    }

                    public override string ToString()
                    {
                        return $"Mode: {Mode}, Condidence: {Confidence}, Error: {Error}";
                    }
                }

                /// <summary>
                /// A structure containing information on the current state of the
                /// Head Tracking system.
                /// </summary>
                public readonly struct StateEx
                {
                    /// <summary>
                    /// Current status of the Head Tracking system.
                    /// </summary>
                    public readonly HeadTrackingStatus Status;

                    /// <summary>
                    /// A confidence value (from 0..1) representing the confidence in the
                    /// current pose estimation.
                    /// </summary>
                    public readonly float Confidence;

                    /// <summary>
                    /// Represents what tracking error (if any) is present.
                    /// </summary>
                    public readonly TrackingErrorFlag Error;

                    internal StateEx(NativeBindings.MLHeadTrackingStateEx nativeState)
                    {
                        this.Status = nativeState.Status;
                        this.Confidence = nativeState.Confidence;
                        this.Error = (TrackingErrorFlag)nativeState.Error;
                    }

                    public override string ToString()
                    {
                        return $"Status: {Status}, Condidence: {Confidence}, Error: {Error}";
                    }
                }

                internal static class NativeBindings
                {
                    [Obsolete]
                    private static byte[] allocatedHeadTrackingStateData = new byte[Marshal.SizeOf<NativeBindings.State>()];

                    private static byte[] allocatedHeadTrackingStateExData = new byte[Marshal.SizeOf<NativeBindings.MLHeadTrackingStateEx>()];
                    private static byte[] allocatedHeadTrackingMapEventsData = new byte[sizeof(MapEvents)];

                    [Obsolete]
                    public static bool TryGetState(InputDevice device, out MLHeadTracking.State state)
                    {
                        if (!device.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Head.TrackingState, allocatedHeadTrackingStateData))
                            goto Failure;

                        try
                        {
                            IntPtr ptr = Marshal.AllocHGlobal(allocatedHeadTrackingStateData.Length);
                            Marshal.Copy(allocatedHeadTrackingStateData, 0, ptr, allocatedHeadTrackingStateData.Length);
                            var nativeState = Marshal.PtrToStructure<NativeBindings.State>(ptr);
                            Marshal.FreeHGlobal(ptr);
                            state = new MLHeadTracking.State(nativeState);
                            return true;
                        }

                        catch (Exception e)
                        {
                            Debug.LogError("TryGetState failed with the exception: " + e);
                            goto Failure;
                        }

                    Failure:
                        state = default;
                        return false;
                    }

                    public static bool TryGetStateEx(InputDevice device, out MLHeadTracking.StateEx state)
                    {
                        if (!device.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Head.TrackingStateEx, allocatedHeadTrackingStateExData))
                            goto Failure;

                        try
                        {
                            IntPtr ptr = Marshal.AllocHGlobal(allocatedHeadTrackingStateExData.Length);
                            Marshal.Copy(allocatedHeadTrackingStateExData, 0, ptr, allocatedHeadTrackingStateExData.Length);
                            var nativeState = Marshal.PtrToStructure<NativeBindings.MLHeadTrackingStateEx>(ptr);
                            Marshal.FreeHGlobal(ptr);
                            state = new MLHeadTracking.StateEx(nativeState);
                            return true;
                        }

                        catch (Exception e)
                        {
                            Debug.LogError("TryGetStateEx failed with the exception: " + e);
                            goto Failure;
                        }

                    Failure:
                        state = default;
                        return false;
                    }

                    public static bool TryGetMapEvents(InputDevice device, out MLHeadTracking.MapEvents mapEvents)
                    {
                        if (!device.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Head.TrackingMapEvents, allocatedHeadTrackingMapEventsData))
                        {
                            mapEvents = default;
                            return false;
                        }

                        try
                        {
                            mapEvents = (MapEvents)BitConverter.ToUInt64(allocatedHeadTrackingMapEventsData);
                            return (mapEvents != 0);
                        }

                        catch (Exception e)
                        {
                            Debug.LogError("TryGetMapEvents failed with the exception: " + e);
                            mapEvents = default;
                            return false;
                        }
                    }

                    /// <summary>
                    /// A structure containing information on the current state of the
                    /// Head Tracking system.
                    /// </summary>
                    [Obsolete]
                    [StructLayout(LayoutKind.Sequential)]
                    public readonly struct State
                    {
                        /// <summary>
                        /// What tracking mode the Head Tracking system is currently in.
                        /// </summary>
                        public readonly TrackingMode Mode;

                        /// <summary>
                        /// A confidence value (from 0..1) representing the confidence in the
                        /// current pose estimation.
                        /// </summary>
                        public readonly float Confidence;

                        /// <summary>
                        /// Represents what tracking error (if any) is present.
                        /// </summary>
                        public readonly TrackingError Error;
                    }

                    /// <summary>
                    /// A structure containing information on the current state of the
                    /// Head Tracking system.
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public readonly struct MLHeadTrackingStateEx
                    {
                        /// <summary>
                        /// Version of this structure.
                        /// </summary>
                        public readonly uint Version;

                        /// <summary>
                        /// Current status of the Head Tracking system.
                        /// </summary>
                        public readonly HeadTrackingStatus Status;

                        /// <summary>
                        /// A confidence value (from 0..1) representing the confidence in the
                        /// current pose estimation.
                        /// </summary>
                        public readonly float Confidence;

                        /// <summary>
                        /// Represents what tracking error (if any) is present.
                        /// </summary>
                        public readonly uint Error;
                    }

                    /// <summary>
                    /// Returns static information about the Head Tracker
                    /// </summary>
                    /// <param name="handle"> A handle to the tracker retireved by GetHeadTrackerHandle()</param>
                    /// <param name="data"> Target to populate the data about that Head Tracker</param>
                    /// <returns>
                    /// MLResult.Result will be <c>MLResult.Code.InvalidParam</c> if failed to receive static data due to an invalid input parameter.
                    /// MLResult.Result will be <c>MLResult.Code.Ok</c> if successfully received static data.
                    /// MLResult.Result will be <c>MLResult.Code.UnspecifiedFailure</c> if failed to receive static data due to an unknown error.
                    /// </returns>
                    [DllImport(MagicLeapNativeBindings.MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
                    public static extern MLResult.Code MLHeadTrackingGetStaticData(ulong handle, ref MLHeadTrackingStaticData data);

                    /// <summary>
                    /// Static information about a Head Tracker. Populate this structure with MLHeadTrackingGetStaticData()
                    /// </summary>
                    [StructLayout(LayoutKind.Sequential)]
                    public struct MLHeadTrackingStaticData
                    {
                        /// <summary>
                        /// Coordinate frame ID of the head 
                        /// </summary>
                        public MagicLeapNativeBindings.MLCoordinateFrameUID coord_frame_head;
                    }
                }
            }
        }
    }

}
