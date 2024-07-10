// Disabling deprecated warning for the internal project
#pragma warning disable 618

using NUnit.Framework;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public partial class MLAnchorsTests
    {
        MLAnchors.Anchor anchor;
        MLAnchors.Request query;
        // Order enforced to ensure anchor and query are valid when they need to be 
        [Test, Order(1)]
        public void MLAnchors_CreateAnchor()
        {
            Assert.IsTrue(CheckAnchorsPermissions());

            MLAnchors.Anchor.Create(new Pose(new Vector3(0, 0, 0), Quaternion.identity), 300, out anchor);
            Assert.IsFalse(string.IsNullOrEmpty(anchor.Id));
            Assert.IsTrue(anchor.Pose.position == Vector3.zero);

            anchor.Publish();
        }

        [Test, Order(2)]
        public void MLAnchors_RequestAnchor()
        {
            Assert.IsTrue(CheckAnchorsPermissions());

            query = new MLAnchors.Request();
            query.Start(new MLAnchors.Request.Params(Vector3.zero, 0, 0, true));
            query.TryGetResult(out MLAnchors.Request.Result result);

            Assert.IsTrue(result.anchors.Length == 1);

            anchor = result.anchors[0];

            Assert.IsFalse(string.IsNullOrEmpty(anchor.Id));
            Assert.IsTrue(anchor.Pose.position == Vector3.zero);

        }

        [Test, Order(3)]
        public void MLAnchors_DeleteAnchor()
        {
            Assert.IsTrue(CheckAnchorsPermissions());

            anchor.Delete();

            query.Start(new MLAnchors.Request.Params(Vector3.zero, 0, 0, true));
            query.TryGetResult(out MLAnchors.Request.Result result);

            Assert.IsTrue(result.anchors.Length == 0);
        }


        [Test, Order(4)]
        public void MLAnchors_IsStarted()
        {
            Assert.IsTrue(CheckAnchorsPermissions());

            Assert.IsTrue(MLAnchors.IsStarted);

        }
        private bool CheckAnchorsPermissions()
        {
            MLPermissions.RequestPermission(MLPermission.SpatialAnchors, new MLPermissions.Callbacks());

            // SPATIAL_ANCHOR is a normal permission; we don't request it at runtime - must be included in AndroidManifest.xml
            return MLPermissions.CheckPermission(MLPermission.SpatialAnchors).IsOk;
        }
    }
}
