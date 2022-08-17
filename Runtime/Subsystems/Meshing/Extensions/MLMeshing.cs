// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MeshingSubsystemConfig.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2021 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
    using System;
    using System.Runtime.InteropServices;
    using UnityEngine.XR.ARSubsystems;

    public partial class MeshingSubsystem
    {
        public static partial class Extensions
        {
            public static class MLMeshing
            {
                public static class Config
                {
                    private const string LuminXrProviderDll = "LuminXrProvider";

                    public static IntPtr AcquireConfidence(MeshId meshId, out int count) => MeshingAcquireConfidence(meshId, out count);
                    public static void ReleaseConfidence(MeshId meshId) => MeshingReleaseConfidence(meshId);
                    public static void SetBounds(Transform transform, Vector3 extents) => SetBounds(transform.localPosition, transform.localRotation, extents);
                    public static void SetBounds(Vector3 position, Quaternion rotation, Vector3 extents) => MeshingSetBounds(position, rotation, extents);

                    public static int batchSize
                    {
                        set { MeshingSetBatchSize(value); }
                    }

                    public static float density
                    {
                        set { MeshingSetDensity(value); }
                    }

                    public static Settings meshingSettings
                    {
                        set
                        {
                            SubsystemFeatures.SetCurrentFeatureEnabled(Feature.Meshing | Feature.PointCloud, false);
                            if (value.flags.HasFlag(Flags.PointCloud))
                            {
                                SubsystemFeatures.SetCurrentFeatureEnabled(Feature.PointCloud, true);
                            }
                            else
                            {
                                SubsystemFeatures.SetCurrentFeatureEnabled(Feature.Meshing, true);
                            }
                            MeshingUpdateSettings(ref value);
                        }
                    }

#if UNITY_MAGICLEAP || UNITY_ANDROID
                    [DllImport(LuminXrProviderDll)]
                    internal static extern void MeshingUpdateSettings(ref Settings newSettings);

                    [DllImport(LuminXrProviderDll)]
                    internal static extern void MeshingSetDensity(float density);

                    [DllImport(LuminXrProviderDll)]
                    internal static extern void MeshingSetBounds(Vector3 center, Quaternion rotation, Vector3 extents);

                    [DllImport(LuminXrProviderDll)]
                    internal static extern void MeshingSetBatchSize(int batchSize);

                    [DllImport(LuminXrProviderDll)]
                    internal static extern IntPtr MeshingAcquireConfidence(MeshId meshId, out int count);

                    [DllImport(LuminXrProviderDll)]
                    internal static extern void MeshingReleaseConfidence(MeshId meshId);
#else
                    internal static void MeshingUpdateSettings(ref Settings newSettings) { }

                    internal static void MeshingSetDensity(float density) { }

                    internal static void MeshingSetBounds(Vector3 center, Quaternion rotation, Vector3 extents) { }

                    internal static void MeshingSetBatchSize(int batchSize) { }

                    internal static IntPtr MeshingAcquireConfidence(MeshId meshId, out int count) { count = 0; return IntPtr.Zero; }

                    internal static void MeshingReleaseConfidence(MeshId meshId) { }
#endif

                    [Flags]
                    public enum Flags
                    {
                        None = 0,
                        PointCloud = 1 << 0,
                        ComputeNormals = 1 << 1,
                        ComputeConfidence = 1 << 2,
                        Planarize = 1 << 3,
                        RemoveMeshSkirt = 1 << 4,
                        IndexOrderCW = 1 << 5
                    }

                    [StructLayout(LayoutKind.Sequential)]
                    public struct Settings
                    {
                        public Flags flags;
                        public float fillHoleLength;
                        public float disconnectedComponentArea;
                    }
                }

            }
        }
    }
}
