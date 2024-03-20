using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.MagicLeap
{
#if UNITY_XR_MAGICLEAP_PROVIDER
    using MLLog = UnityEngine.XR.MagicLeap.MagicLeapLogger;
#endif

    /// <summary>
    /// The Magic Leap implementation of the <c>XRAnchorSubsystem</c>. Do not create this directly.
    /// Use <c>XRAnchorSubsystemDescriptor.Create()</c> instead.
    /// </summary>
    [Preserve]
    public sealed partial class AnchorSubsystem : XRAnchorSubsystem
    {
#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MagicLeapProvider();
#endif

        class MagicLeapProvider : Provider
        {
            /// <summary>
            /// The squared amount by which a coordinate frame has to move for its anchor to be reported as "updated"
            /// </summary>
            const float CoordinateFramePositionEpsilonSquared = .0001f;

            const ulong AnchorTrackableIdSalt = 0xf52b75076e45ad88;

            static TrackableId GenerateTrackableId(string id)
            {
                var hash = id.GetHashCode();
                var trackableId = new TrackableId((ulong)hash, AnchorTrackableIdSalt);
                return trackableId;
            }

            static bool PosesAreApproximatelyEqual(Pose lhs, Pose rhs)
            {
                var positionDelta = lhs.position - rhs.position;
                var rotationDelta = lhs.rotation * Quaternion.Inverse(rhs.rotation);

                return positionDelta.sqrMagnitude <= CoordinateFramePositionEpsilonSquared;
            }

            private Camera mainCamera;
            private MLAnchors.LocalizationInfo localizationInfo;
            private MLAnchors.Request query = new MLAnchors.Request();
            private Dictionary<TrackableId, MLAnchors.Anchor> currentAnchors = new Dictionary<TrackableId, MLAnchors.Anchor>();

            public MagicLeapProvider()
            {
            }

            public override void Start() => mainCamera = Camera.main;

            public override void Stop() => mainCamera = null;

            public override void Destroy()
            {
            }

            public override unsafe TrackableChanges<XRAnchor> GetChanges(XRAnchor defaultAnchor, Allocator allocator)
            {
                if (mainCamera == null)
                    return default;

                MLAnchors.GetLocalizationInfo(out localizationInfo);
                if (localizationInfo.LocalizationStatus != MLAnchors.LocalizationStatus.Localized)
                    return default;

                query.Start(new MLAnchors.Request.Params(mainCamera.transform.position, 0, 0, false));
                var mlResult = query.TryGetResult(out MLAnchors.Request.Result result);
                if (!mlResult.IsOk)
                    return default;

                var added = new NativeFixedList<XRAnchor>((int)result.anchors.Length, Allocator.Temp);
                var updated = new NativeFixedList<XRAnchor>((int)result.anchors.Length, Allocator.Temp);
                var removed = new NativeFixedList<TrackableId>((int)currentAnchors.Count, Allocator.Temp);

                for (int i = 0; i < result.anchors.Length; ++i)
                {
                    var anchor = result.anchors[i];
                    var trackableId = GenerateTrackableId(anchor.Id);

                    // added
                    if (!currentAnchors.ContainsKey(trackableId))
                    {
                        var xrAnchor = new XRAnchor(trackableId, anchor.Pose, TrackingState.Tracking, IntPtr.Zero);
                        added.Add(xrAnchor);
                    }

                    // updated
                    else
                    {
                        var currentAnchor = currentAnchors[trackableId];
                        currentAnchors.Remove(trackableId);

                        if (!PosesAreApproximatelyEqual(currentAnchor.Pose, anchor.Pose))
                        {
                            var xrAnchor = new XRAnchor(trackableId, anchor.Pose, TrackingState.Tracking, IntPtr.Zero);
                            updated.Add(xrAnchor);
                        }
                    }
                }

                // removed
                foreach (var remainingAnchor in currentAnchors.Values)
                {
                    var trackableId = GenerateTrackableId(remainingAnchor.Id);
                    removed.Add(trackableId);
                }

                // reinitialize currentAnchors in place to avoid garbage collection.
                // cannot just add the updated anchors back in because some exiting anchors may not have qualified into the updated list.
                currentAnchors.Clear();
                foreach (var newAnchor in result.anchors)
                {
                    var trackableId = GenerateTrackableId(newAnchor.Id);
                    currentAnchors.Add(trackableId, newAnchor);
                }

                using (added)
                using (updated)
                using (removed)
                {
                    var changes = new TrackableChanges<XRAnchor>(
                        added.Length,
                        updated.Length,
                        removed.Length,
                        allocator);

                    added.CopyTo(changes.added);
                    updated.CopyTo(changes.updated);
                    removed.CopyTo(changes.removed);

                    Debug.Log("num added: " + added.Length + ", num updated: " + updated.Length + ", num removed: " + removed.Length);
                    return changes;
                }


            }

            public override unsafe bool TryAddAnchor(Pose pose, out XRAnchor xrAnchor)
            {
                if (localizationInfo.LocalizationStatus != MLAnchors.LocalizationStatus.Localized)
                {
                    xrAnchor = default;
                    return false;
                }

                var result = MLAnchors.Anchor.Create(pose, 100, out MLAnchors.Anchor anchor);
                if (!result.IsOk)
                {
                    xrAnchor = default;
                    return false;
                }

                result = anchor.Publish();
                if (!result.IsOk)
                {
                    xrAnchor = default;
                    return false;
                }

                xrAnchor = new XRAnchor(GenerateTrackableId(anchor.Id), anchor.Pose, TrackingState.Tracking, IntPtr.Zero);
                return result.IsOk;
            }

            public override bool TryRemoveAnchor(TrackableId trackableId)
            {
                if (!currentAnchors.ContainsKey(trackableId))
                    return false;

                var anchor = currentAnchors[trackableId];

                var result = anchor.Delete();
                return result.IsOk;
            }
        }

#if UNITY_XR_MAGICLEAP_PROVIDER
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        static void RegisterDescriptor()
        {
            XRAnchorSubsystemDescriptor.Create(new XRAnchorSubsystemDescriptor.Cinfo
            {
                id = MagicLeapXrProvider.AnchorSubsystemId,
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(AnchorSubsystem.MagicLeapProvider),
                subsystemTypeOverride = typeof(AnchorSubsystem),
#else
                subsystemImplementationType = typeof(AnchorSubsystem),
#endif
                supportsTrackableAttachments = false
            });
        }
    }
}
