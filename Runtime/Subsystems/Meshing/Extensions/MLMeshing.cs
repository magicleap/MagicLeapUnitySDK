// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
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
            public static partial class MLMeshing
            {
                public static partial class Config
                {
                    public static IntPtr AcquireConfidence(MeshId meshId, out int count) => NativeBindings.MeshingAcquireConfidence(meshId, out count);
                    public static void ReleaseConfidence(MeshId meshId) => NativeBindings.MeshingReleaseConfidence(meshId);
                    public static void SetBounds(Transform transform, Vector3 extents) => SetBounds(transform.localPosition, transform.localRotation, extents);
                    public static void SetBounds(Vector3 position, Quaternion rotation, Vector3 extents) => NativeBindings.MeshingSetBounds(position, rotation, extents);

                    public static int batchSize
                    {
                        set { NativeBindings.MeshingSetBatchSize(value); }
                    }

                    public static float density
                    {
                        set { NativeBindings.MeshingSetDensity(value); }
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
                            NativeBindings.MeshingUpdateSettings(ref value);
                        }
                    }

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
