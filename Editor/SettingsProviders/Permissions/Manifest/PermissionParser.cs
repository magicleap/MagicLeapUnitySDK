using System;
using System.Collections.Generic;

namespace UnityEditor.XR.MagicLeap
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
