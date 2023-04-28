using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public partial class MLAnchorsTests
    {
        [Test]
        public void MLAnchors_IsStarted()
        {
            Assert.IsTrue(CheckAnchorsPermissions());

            Assert.IsTrue(MLAnchors.IsStarted);
        }

        [Test]
        public void MLAnchors_Create()
        {
            Assert.IsTrue(CheckAnchorsPermissions());

            MLAnchors.Anchor.Create(new Pose(Vector3.zero, Quaternion.identity), 300,
                out MLAnchors.Anchor anchor);

            Assert.IsFalse(string.IsNullOrEmpty(anchor.Id));
            Assert.IsTrue(anchor.Pose.position == Vector3.zero);
            // Assert.IsTrue(anchor.Pose.rotation == Quaternion.identity);

            anchor.Delete();
        }

        [Test]
        public void MLAnchors_Create_Request_Delete_Anchor()
        {
            Assert.IsTrue(CheckAnchorsPermissions());

            MLAnchors.Anchor.Create(new Pose(Vector3.zero, Quaternion.identity), 300,
                out MLAnchors.Anchor anchor);

            Assert.IsFalse(string.IsNullOrEmpty(anchor.Id));
            Assert.IsTrue(anchor.Pose.position == Vector3.zero);
            // Assert.IsTrue(anchor.Pose.rotation == Quaternion.identity);

            anchor.Publish();

            MLAnchors.GetLocalizationInfo(out MLAnchors.LocalizationInfo info);

            MLAnchors.Request query = new MLAnchors.Request();
            query.Start(new MLAnchors.Request.Params(Vector3.zero, 0, 0, true));
            query.TryGetResult(out MLAnchors.Request.Result result);

            Assert.IsTrue(result.anchors.Length == 1);

            anchor = result.anchors[0];

            Assert.IsFalse(string.IsNullOrEmpty(anchor.Id));
            Assert.IsTrue(anchor.Pose.position == Vector3.zero);
            // Assert.IsTrue(anchor.Pose.rotation == Quaternion.identity);

            anchor.Delete();

            query = new MLAnchors.Request();
            query.Start(new MLAnchors.Request.Params(Vector3.zero, 0, 0, true));
            query.TryGetResult(out result);

            Assert.IsTrue(result.anchors.Length == 0);
        }

        private bool CheckAnchorsPermissions()
        {
            MLPermissions.RequestPermission(MLPermission.SpatialAnchors, new MLPermissions.Callbacks());

            // SPATIAL_ANCHOR is a normal permission; we don't request it at runtime - must be included in AndroidManifest.xml
            return MLPermissions.CheckPermission(MLPermission.SpatialAnchors).IsOk;
        }
    }
}
