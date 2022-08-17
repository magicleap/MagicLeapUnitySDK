// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;

namespace UnityEditor.XR.MagicLeap
{
    [Serializable]
    public class PermissionsListJson
    {
        public PermissionJson[] Settings;
    }

    [Serializable]
    public struct PermissionJson
    {
        public string name;
        public string description;
        public string level;
        public string min_api_level;
    }
}
