using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Unity.Collections;
using UnityEngine.Lumin;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Native;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// The Magic Leap implementation of the <c>XRRaycastSubsystem</c>. Do not create this directly.
    /// Use <c>XRRaycastSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    [UsesLuminPrivilege("SpatialMapping")]
    public sealed class RaycastSubsystem : XRRaycastSubsystem
    {
        /// <summary>
        /// Asynchronously casts a ray. Use the returned <see cref="AsyncRaycastResult"/> to check for completion and
        /// retrieve the raycast hit results.
        /// </summary>
        /// <param name="query">The input query for the raycast job.</param>
        /// <returns>An <see cref="AsyncRaycastResult"/> which can be used to check for completion and retrieve the raycast result.</returns>
        public AsyncRaycastResult AsyncRaycast(RaycastQuery query)
        {
            return magicLeapProvider.AsyncRaycast(query);
        }

#if UNITY_2020_2_OR_NEWER
        MagicLeapProvider magicLeapProvider => (MagicLeapProvider)provider;
#else
        MagicLeapProvider magicLeapProvider;

        protected override Provider CreateProvider()
        {
            magicLeapProvider = new MagicLeapProvider();
            return magicLeapProvider;
        }
#endif

        class MagicLeapProvider : Provider
        {
            private Dictionary<TrackableId, MLRaycast.Request> requestMap = new Dictionary<TrackableId, MLRaycast.Request>();

            ulong m_TrackerHandle = Native.InvalidHandle;

            List<XRRaycast> added = new List<XRRaycast>();
            List<XRRaycast> updated = new List<XRRaycast>();
            List<TrackableId> removed = new List<TrackableId>();

            public override bool TryAddRaycast(Ray ray, float estimatedDistance, out XRRaycast raycast)
            {
                var requestParams = new MLRaycast.Request.Params()
                {
                    Position = ray.origin,
                    Direction = ray.direction,
                    UpVector = Vector3.up,
                    Width = 1,
                    Height = 1,
                    HorizontalFovDegrees = 0,
                    CollideWithUnobserved = false
                };

                var request = new MLRaycast.Request(requestParams);

                // TODO: make this into uid
                ulong id = 486187739;
                var requestId = new TrackableId(id, id);
                requestMap.Add(requestId, request);
                raycast = new XRRaycast(requestId, default, TrackingState.None, IntPtr.Zero, estimatedDistance, default);
                return true;
            }

            public override void RemoveRaycast(TrackableId trackableId)
            {
                if (requestMap.ContainsKey(trackableId))
                {
                    requestMap.Remove(trackableId);
                    removed.Add(trackableId);
                }
            }

            // list of active known about requests
            HashSet<TrackableId> currentTrackables = new HashSet<TrackableId>();

            public override TrackableChanges<XRRaycast> GetChanges(XRRaycast defaultRaycast, Allocator allocator)
            {
                foreach (var pair in requestMap)
                {
                    var request = pair.Value;
                    var requestId = pair.Key;
                    var mlResult = request.TryGetResult(out MLRaycast.Request.Result raycastResult);
                    if (mlResult.IsOk)
                    {
                        var raycast = new XRRaycast(requestId, new Pose(raycastResult.Point, Quaternion.identity), TrackingState.Tracking, IntPtr.Zero, Vector3.Distance(request.Parameters.Position, raycastResult.Point), TrackableId.invalidId);

                        Debug.Log($"Adding raycast with pose: {raycast.pose.position}");
                        if (!currentTrackables.Contains(requestId))
                        {
                            added.Add(raycast);
                            currentTrackables.Add(requestId);
                        }
                        else
                        {
                            updated.Add(raycast);
                        }
                    }
                    else if (currentTrackables.Contains(requestId))
                    {
                        currentTrackables.Remove(requestId);
                        removed.Add(requestId);
                    }
                }

                var nativeAdded = new NativeArray<XRRaycast>(added.ToArray(), allocator);
                var nativeUpdated = new NativeArray<XRRaycast>(updated.ToArray(), allocator);
                var nativeRemoved = new NativeArray<TrackableId>(removed.ToArray(), allocator);

                added.Clear();
                updated.Clear();
                removed.Clear();

                return TrackableChanges<XRRaycast>.CopyFrom(nativeAdded, nativeUpdated, nativeRemoved, allocator);
            }

            public AsyncRaycastResult AsyncRaycast(RaycastQuery query)
            {
                return new AsyncRaycastResult(m_TrackerHandle, query);
            }

            public MagicLeapProvider()
            {
            }

            public override void Start()
            {
                MagicLeapFeatures.SetFeatureRequested(Feature.Raycast, true);
            }

            public override void Stop()
            {
                MagicLeapFeatures.SetFeatureRequested(Feature.Raycast, false);
            }

            public override void Destroy()
            {
            }
        }

        internal class Native : MagicLeapNativeBindings
        {
            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLRaycastCreate(out ulong handle);

            [DllImport(MLPerceptionClientDll, CallingConvention = CallingConvention.Cdecl)]
            public static extern MLResult.Code MLRaycastDestroy(ulong handle);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            XRRaycastSubsystemDescriptor.RegisterDescriptor(new XRRaycastSubsystemDescriptor.Cinfo
            {
                id = LuminXrProvider.RaycastSubsystemId,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(RaycastSubsystem.MagicLeapProvider),
                subsystemTypeOverride = typeof(RaycastSubsystem),
#else
                subsystemImplementationType = typeof(RaycastSubsystem),
#endif
                supportsViewportBasedRaycast = false,
                supportsWorldBasedRaycast = false,
                supportedTrackableTypes = TrackableType.None,
            });
#endif 
        }
    }
}
