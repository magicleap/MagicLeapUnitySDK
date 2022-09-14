// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;

    public partial class MeshingSubsystem
    {
        public static partial class Extensions
        {
            public static partial class MLMeshing
            {

                public static partial class Config
                {
                    private class NativeBindings
                    {
                        private const string MagicLeapXrProviderDll = "MagicLeapXrProvider";

#if UNITY_MAGICLEAP || UNITY_ANDROID
                        [DllImport(MagicLeapXrProviderDll)]
                        internal static extern void MeshingUpdateSettings(ref Config.Settings newSettings);

                        [DllImport(MagicLeapXrProviderDll)]
                        internal static extern void MeshingSetDensity(float density);

                        [DllImport(MagicLeapXrProviderDll)]
                        internal static extern void MeshingSetBounds(Vector3 center, Quaternion rotation, Vector3 extents);

                        [DllImport(MagicLeapXrProviderDll)]
                        internal static extern void MeshingSetBatchSize(int batchSize);

                        [DllImport(MagicLeapXrProviderDll)]
                        internal static extern IntPtr MeshingAcquireConfidence(MeshId meshId, out int count);

                        [DllImport(MagicLeapXrProviderDll)]
                        internal static extern void MeshingReleaseConfidence(MeshId meshId);
#else
                        internal static void MeshingUpdateSettings(ref Config.Settings newSettings) { }

                        internal static void MeshingSetDensity(float density) { }

                        internal static void MeshingSetBounds(Vector3 center, Quaternion rotation, Vector3 extents) { }

                        internal static void MeshingSetBatchSize(int batchSize) { }

                        internal static IntPtr MeshingAcquireConfidence(MeshId meshId, out int count) { count = 0; return IntPtr.Zero; }

                        internal static void MeshingReleaseConfidence(MeshId meshId) { }
#endif
                    }
                }
            }
        }
    }
}
