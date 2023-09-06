using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap;

namespace UnitySDKPlayTests
{
    public class MLSpaceTests
    {
        // If you don't have any spaces on the device these tests will fail
        MLResult result;
        
        [Test]
        public void MLSpace_GetSpaceList()
        { 
            result = MLResult.Create(MLSpace.GetSpaceList(out _));
            Assert.IsTrue(result.IsOk, string.Format("GetSpaceList failed: {0}", result.ToString()));
        }

        [Test]
        public void MLSpace_ExportSpace()
        {
            MLSpace.Space space = GetSpace();
            result = MLResult.Create(MLSpace.ExportSpace(new MLSpace.SpaceInfo {SpaceId = space.SpaceId} , out MLSpace.SpaceData outData));
            Assert.IsTrue(result.IsOk, string.Format("ExportSpace failed: {0}", result.ToString()));
        }

        [Test]
        public void MLSpace_ImportSpace()
        {
            MLSpace.Space space = GetSpace();   
            result = MLResult.Create(MLSpace.ExportSpace(new MLSpace.SpaceInfo{ SpaceId = space.SpaceId }, out MLSpace.SpaceData spaceData));
            Assert.IsTrue(result.IsOk, string.Format("ExportSpace failed: {0}", result.ToString()));

            MLSpace.SpaceInfo spaceInfo;
            
            // Can't import an existing space, expect SpaceAlreadyExists 
            result = MLResult.Create(MLSpace.ImportSpace(in spaceData, out spaceInfo));
            if(!result.IsOk)
            {
                switch (result.Result)
                {
                    case MLResult.Code.SpaceAlreadyExists:
                        Assert.Pass("Cannot import an existing space (expected result)");
                        break;
                    default:
                        Assert.Fail(string.Format("ImportSpace failed: {0}", result.ToString()));
                        break;
                }
            }
        }

        [Test]
        public void MLSpace_RequestLocalization()
        {
            MLSpace.Space space = GetSpace();
            MLSpace.SpaceInfo spaceInfo = new MLSpace.SpaceInfo { SpaceId = space.SpaceId };
            result = MLResult.Create(MLSpace.RequestLocalization(ref spaceInfo));
            Assert.IsTrue(result.IsOk, string.Format("RequestLocalization failed: {0}", result.ToString()));
        }

        [Test]
        public void MLSpace_GetLocalizationResult() 
        {
            MLSpace.Space space = GetSpace();
            MLSpace.SpaceInfo spaceInfo = new MLSpace.SpaceInfo { SpaceId = space.SpaceId };
            result = MLResult.Create(MLSpace.RequestLocalization(ref spaceInfo));
            Assert.IsTrue(result.IsOk, string.Format("RequestLocalization failed: {0}", result.ToString()));
            result = MLResult.Create(MLSpace.GetLocalizationResult(out MLSpace.LocalizationResult res));
            Assert.IsTrue(result.IsOk, string.Format("GetLocalizationResult failed: {0}", result.ToString()));
        }

        // Used for methods that need a space to query
        private MLSpace.Space GetSpace()
        {
            MLSpace.GetSpaceList(out MLSpace.Space[] spaces);
            Assert.IsTrue(spaces.Length > 0, "GetSpace: no spaces retrieved from device; please create a space or import from AR Cloud");
            return spaces[0];
        }

    }
}

