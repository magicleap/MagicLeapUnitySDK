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
using UnityEngine;
using static UnityEditor.XR.MagicLeap.Permission;

namespace UnityEditor.XR.MagicLeap
{
    [Serializable]
    internal struct PermissionGroup
    {
        [SerializeField]
        private string name;

        [SerializeField]
        internal Permission[] permissions;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Permission[] Permissions
        {
            get { return permissions; }
            set { permissions = value; }
        }
    }
}
