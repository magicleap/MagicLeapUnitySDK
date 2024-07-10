// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2022-2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System;
using System.Collections.Generic;

namespace MagicLeap.Settings.Android
{
    internal static class PermissionParser
    {
        internal static List<Permission> ParsePermissionsFromArray(PermissionJson[] permissions)
        {
            var result = new List<Permission>();
            foreach(var permission in permissions)
            {
                result.Add(ParsePermission(permission));
            }
            return result;
        }

        internal static Permission.ProtectionLevel ParseProtectionLevel(string input)
        {
            Permission.ProtectionLevel level;
            if (Enum.TryParse(input, true, out level))
            {
                return level;
            }
            else
            {
                throw new ArgumentException($"Invalid input value for {nameof(Permission.ProtectionLevel)}: \"{input}\".");
            }
        }

        private static Permission ParsePermission(PermissionJson permission)
        {
            Permission result = new();
            result.MinimumApiLevel = (int.Parse(permission.min_api_level));
            result.Level = ParseProtectionLevel(permission.level);
            result.Description = permission.description;
            result.Name = permission.name;
            return result;
        }
    }
}
