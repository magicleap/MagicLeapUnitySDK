// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.XR.MagicLeap;
using MagicLeap.Core;

namespace MagicLeap
{
    /// <summary>
    /// ContentObject is a component that defines the behaviour of an object
    /// that can be placed on a plane. The PlanePopulator requires a list of
    /// ContentObjects that can be used as props in the Room Features demo.
    /// </summary>
    public class ContentObject : MonoBehaviour
    {
        [MLBitMask(typeof(PlanePopulator.SurfaceType))]
        [Tooltip("What surfaces type this ContentObject will be placed on")]
        public PlanePopulator.SurfaceType Flags;

        /// <summary>
        /// The Bounds that encapsulates all the mesh objects who are
        /// children to this ContentObject. The bounds returned is the
        /// complete bounding box of the object in local space.
        /// </summary>
        public Bounds LocalBounds
        {
            get
            {
                if (!_cachedBounds)
                {
                    // Use MeshFilter objects here because their bounding boxes
                    // are calculated using local space.
                    // Also pass true as a parameter to GetComponentsInChildren<>
                    // in case this get function is being called from an object
                    // that has not yet been instantiated or activated.
                    MeshFilter[] filters = gameObject.GetComponentsInChildren<MeshFilter>(true);
                    Bounds newBounds = new Bounds();
                    foreach (MeshFilter filter in filters)
                    {
                        #if UNITY_EDITOR
                        if (filter.sharedMesh != null)
                        {
                            newBounds.Encapsulate(filter.sharedMesh.bounds);
                        }
                        #else
                        if(filter.mesh != null)
                        {
                            newBounds.Encapsulate(filter.mesh.bounds);
                        }
                        #endif
                    }

                    // Cache the bounds so it's not calculated more than once.
                    _localBounds = newBounds;
                    _cachedBounds = true;
                }
                return _localBounds;
            }
        }

        // The cached bounding box that encapsulates all meshes
        // and submeshes of this object.
        private Bounds _localBounds;

        // bool to determine whether or not the _localBounds field
        // has already been initialized. This is required as Bounds
        // is a struct and cannot be set to null.
        private bool _cachedBounds = false;

        #if UNITY_EDITOR
        private static readonly Color WallColor = new Color(1, 0, 0, 0.25f);
        private static readonly Color FloorColor = new Color(0, 1, 0, 0.25f);
        private static readonly Color CeilingColor = new Color(0, 0, 1, 0.25f);

        /// <summary>
        /// Editor only function that resets the status of the cached bounds,
        /// just in case the user changes the object's state and they need to
        /// be recalculated.
        /// </summary>
        private void Reset()
        {
            _cachedBounds = false;
        }

        /// <summary>
        /// Draws the localized bounding box as a wireframe cube, as well
        /// as draws the sides of the object that will be used as anchor
        /// points based on the SemanticFlags that have been set.
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            Bounds bounds = LocalBounds;

            Gizmos.color = Color.yellow;
            Gizmos.DrawWireCube(this.transform.position + bounds.center, bounds.size);

            // Wall
            if ((Flags & PlanePopulator.SurfaceType.Wall) == PlanePopulator.SurfaceType.Wall)
            {
                Gizmos.color = WallColor;
                Gizmos.DrawCube(transform.position + bounds.center + new Vector3(0, 0, -bounds.extents.z), 2 * new Vector3(bounds.extents.x, bounds.extents.y, 0));
            }

            // Floor
            if ((Flags & PlanePopulator.SurfaceType.Floor) == PlanePopulator.SurfaceType.Floor)
            {
                Gizmos.color = FloorColor;
                Gizmos.DrawCube(transform.position + bounds.center + new Vector3(0, -bounds.extents.y, 0), 2 * new Vector3(bounds.extents.x, 0, bounds.extents.z));
            }

            // Ceiling
            if ((Flags & PlanePopulator.SurfaceType.Ceiling) == PlanePopulator.SurfaceType.Ceiling)
            {
                Gizmos.color = CeilingColor;
                Gizmos.DrawCube(transform.position + bounds.center + new Vector3(0, +bounds.extents.y, 0), 2 * new Vector3(bounds.extents.x, 0, bounds.extents.z));
            }
        }
        #endif
    }
}
