// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
// Disabling deprecated warning for the internal project
#pragma warning disable 618

using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLPlanesTests
    {
        private const uint maxResults = 100;
        private const float minPlaneArea = 0.25f;

        private readonly MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();

        [Test]
        public void Planes_RequestPermission()
        {
            Assert.IsTrue(CheckPermission());
        }

        [Test]
        public void Planes_UpdateQuery()
        {
            SetUp();
            //yield return new WaitUntil(() => callbackReceived);
            //if (permissionGranted)
            //{
            PlanesSubsystem.Extensions.Query = new PlanesSubsystem.Extensions.PlanesQuery
            {
                Flags = PlanesSubsystem.Extensions.MLPlanesQueryFlags.Polygons | PlanesSubsystem.Extensions.MLPlanesQueryFlags.Semantic_All,
                BoundsCenter = Vector3.zero,
                BoundsRotation = Quaternion.identity,
                BoundsExtents = Vector3.one * 20f,
                MaxResults = maxResults,
                MinPlaneArea = minPlaneArea
            };
            //}
            //else
            //{
            //    Assert.Fail();
            //}
            TearDown();
        }

        private void SetUp()
        {
            CheckPermission();
            permissionCallbacks.OnPermissionGranted += OnPermissionGranted;
            permissionCallbacks.OnPermissionDenied += OnPermissionDenied;
            permissionCallbacks.OnPermissionDeniedAndDontAskAgain += OnPermissionDenied;
        }

        private void TearDown()
        {
            permissionCallbacks.OnPermissionGranted -= OnPermissionGranted;
            permissionCallbacks.OnPermissionDenied -= OnPermissionDenied;
            permissionCallbacks.OnPermissionDeniedAndDontAskAgain -= OnPermissionDenied;
        }

        private void OnPermissionGranted(string _)
        {
        }

        private void OnPermissionDenied(string _)
        {
        }

        private bool CheckPermission()
        {
            MLPermissions.RequestPermission(MLPermission.SpatialMapping, new MLPermissions.Callbacks());
            return MLPermissions.CheckPermission(MLPermission.SpatialMapping).IsOk;

        }
    }
}
