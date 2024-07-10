// Disabling deprecated warning for the internal project
#pragma warning disable 618

using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;
using static UnityEngine.XR.MagicLeap.MeshingSubsystem.Extensions.MLMeshing;

namespace UnitySDKPlayTests
{
    public class MLMeshingTests
    {
        [Test]
        public void Meshing_RequestPermission()
        {
            Assert.IsTrue(CheckPermission());
        }

        [Test]
        public void Meshing_FromDensityToLevelOfDetail()
        {
            CheckPermission();
            var result = MeshingSubsystemComponent.FromDensityToLevelOfDetail(0);
            Assert.IsNotNull(result);
        }

        [Test]
        public void Meshing_FromLevelOfDetailToDensity_Maximum()
        {
            CheckPermission();
            var result = MeshingSubsystemComponent.FromLevelOfDetailToDensity(MeshingSubsystem.Extensions.MLMeshing.LevelOfDetail.Maximum);
            Assert.IsNotNull(result);
        }

        [Test]
        public void Meshing_FromLevelOfDetailToDensity_Medium()
        {
            CheckPermission();
            var result = MeshingSubsystemComponent.FromLevelOfDetailToDensity(MeshingSubsystem.Extensions.MLMeshing.LevelOfDetail.Medium);
            Assert.IsNotNull(result);
        }

        [Test]
        public void Meshing_FromLevelOfDetailToDensity_Minimum()
        {
            CheckPermission();
            var result = MeshingSubsystemComponent.FromLevelOfDetailToDensity(MeshingSubsystem.Extensions.MLMeshing.LevelOfDetail.Minimum);
            Assert.IsNotNull(result);
        }

        private bool CheckPermission()
        {
            MLPermissions.RequestPermission(MLPermission.SpatialMapping, new MLPermissions.Callbacks());
            return MLPermissions.CheckPermission(MLPermission.SpatialMapping).IsOk;

        }
    }
}
