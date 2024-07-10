// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using UnityEngine;

namespace MagicLeap.Settings.Android
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
