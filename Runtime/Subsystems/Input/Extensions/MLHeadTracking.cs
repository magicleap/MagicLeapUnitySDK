using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace UnityEngine.XR.MagicLeap
{
    public static partial class InputSubsystem
    {
        public static partial class Extensions
        {
            public static bool TryGetHeadTrackingState(InputDevice headDevice, out MLHeadTracking.State headTrackingState) => MLHeadTracking.TryGetState(headDevice, out headTrackingState);

            public static class MLHeadTracking
            {
                /// <summary>
                /// A set of possible error conditions that can cause Head Tracking to be less than ideal.
                /// </summary>
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
                /// A set of possible tracking modes the Head Tracking system can be in.
                /// </summary>
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

                public static bool TryGetState(InputDevice headDevice, out State headTrackingState) => NativeBindings.TryGetState(headDevice, out headTrackingState);
                public static bool TryGetMapEvents(InputDevice headDevice, out MapEvents mapEvents) => NativeBindings.TryGetMapEvents(headDevice, out mapEvents);

                /// <summary>
                /// A structure containing information on the current state of the
                /// Head Tracking system.
                /// </summary>
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

                internal static class NativeBindings
                {
                    private static byte[] allocatedHeadTrackingStateData = new byte[Marshal.SizeOf<NativeBindings.State>()];
                    private static byte[] allocatedHeadTrackingMapEventsData = new byte[sizeof(MapEvents)];

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

                    public static bool TryGetMapEvents(InputDevice device, out MLHeadTracking.MapEvents mapEvents)
                    {
                        if (!device.TryGetFeatureValue(InputSubsystem.Extensions.DeviceFeatureUsages.Head.TrackingMapEvents, allocatedHeadTrackingMapEventsData))
                            goto Failure;

                        try
                        {
                            mapEvents = (MapEvents)BitConverter.ToUInt64(allocatedHeadTrackingMapEventsData);
                            return (mapEvents != 0);
                        }

                        catch (Exception e)
                        {
                            Debug.LogError("TryGetMapEvents failed with the exception: " + e);
                            goto Failure;
                        }

                    Failure:
                        mapEvents = default;
                        return false;

                    }

                    /// <summary>
                    /// A structure containing information on the current state of the
                    /// Head Tracking system.
                    /// </summary>
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
                }
            }

        }
    }

}
