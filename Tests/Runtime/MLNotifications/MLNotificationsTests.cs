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
    public class MLNotificationsTests
    {

        MLResult result;
        bool hasPermission = false;
        static MLPermissions.Callbacks permissionCallbacks = new MLPermissions.Callbacks();

        [Test]
        public void MLNotifications_SetNotifications_True()
        {
            CheckPermissions();
            if (!hasPermission)
            {
                Assert.Fail("Could not get System notification permission");
            }
            result = MLNotifications.SetNotifications(true);
            if (!result.IsOk)
            {
                switch (result.Result)
                {
                    case MLResult.Code.IncompatibleSKU:
                        Assert.Ignore("Feature not supported on current device; requires medical device");
                        break;
                    default:
                        Assert.Fail(string.Format("SetNotifications_True failed: {0}", result.ToString()));
                        break;
                }
            }
        }

        [Test]
        public void MLNotifications_SetNotifications_False()
        {
            CheckPermissions();
            if (!hasPermission)
            {
                Assert.Fail("Could not get System notification permission");
            }
            result = MLNotifications.SetNotifications(false);
            if (!result.IsOk)
            {
                switch (result.Result)
                {
                    case MLResult.Code.IncompatibleSKU:
                        Assert.Ignore("Feature not supported on current device; requires medical device");
                        break;
                    default:
                        Assert.Fail(string.Format("SetNotifications_False failed: {0}", result.ToString()));
                        break;
                }
            }
        }

        private void CheckPermissions()
        {
            //Need system notification permissions for this 
            MLPermissions.RequestPermission(MLPermission.SystemNotification, permissionCallbacks);
            hasPermission = MLPermissions.CheckPermission(MLPermission.SystemNotification).IsOk;
        }

    }
}
