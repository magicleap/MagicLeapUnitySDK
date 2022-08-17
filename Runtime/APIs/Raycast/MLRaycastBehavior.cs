// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Core
{
    /// <summary>
    /// Base raycast class containing the some common variables and functionality.
    /// </summary>
    [AddComponentMenu("XR/MagicLeap/MLRaycastBehavior")]
    public class MLRaycastBehavior : MonoBehaviour
    {
        public enum RaycastDirection
        {
            Forward,
            EyesFixationPoint
        }

        public enum Mode
        {
            World,
            Virtual,
            Combination
        }

        // Certain properties to set _raycastParams with before raycasting into the world
        [Serializable]
        public class WorldRayProperties
        {
            [Tooltip("The number of horizontal rays to cast. Single raycasts set to 1.")]
            public uint width = 1;

            [Tooltip("The number of vertical rays to cast. Single raycasts set to 1.")]
            public uint height = 1;

            [Tooltip("The horizontal field of view in degrees to determine density of Width/Height raycasts.")]
            public float horizontalFovDegrees;

            [Tooltip("If true the ray will terminate when encountering an unobserved area. Useful for determining unmapped areas.")]
            public bool collideWithUnobserved = false;

            // Note: Generated mesh may include noise (bumps). This bias is meant to cover
            // the possible deltas between that and the perception stack results.
            public const float bias = 0.04f;
        }

        // Certain properties we would like the virtual raycast to have
        [Serializable]
        public class VirtualRayProperties
        {
            public float distance = 9f;

            [Tooltip("The layer(s) that will be used for hit detection.")]
            public LayerMask hitLayerMask = new LayerMask();
        }

        public delegate void OnRaycastResultDelegate(MLRaycast.ResultState state, Mode mode, Ray ray, RaycastHit hit, float confidence);
        public event OnRaycastResultDelegate OnRaycastResult;

        [Tooltip("Direction to determine if raycast come from this object's forward vector or use MLEyes fixation point")]
        public RaycastDirection direction;

        [Tooltip("Mode to determine if raycast should be world, virtual, or combinaton.")]
        public Mode mode;

        [Tooltip("Properties to use for a world raycast.")]
        public WorldRayProperties worldRayProperties;

        [Tooltip("Properties to use for a virutal raycast.")]
        public VirtualRayProperties virtualRayProperties;

        // The parameters used for querying raycasts
        protected MLRaycast.Request.Params raycastRequestParams = new MLRaycast.Request.Params();

        protected MLRaycast.Request raycastRequest;

        // Used to send to the OnRaycastResult callback
        protected Ray ray = new Ray();

        // Used to send to the OnRaycastResult callback
        protected RaycastHit raycastResult;

        // Used inside HandleOnReceiveRaycast to keep track of the mode we were in when raycasting
        protected Mode modeOnWorldRaycast;

        // Used to wait on the world raycast callback to finish before attempting another world raycast
        protected bool isRequestComplete = true;

        // Used to determine if a callback should be fired or not.
        private bool isLastResultHit = false;

        // Used to get ml inputs.
        private MagicLeapInputs mlInputs;

        // Used to get eyes action data.
        private static MagicLeapInputs.EyesActions eyesActions;

        /// <summary>
        /// Returns raycast position.
        /// </summary>
        protected Vector3 Position
        {
            get
            {
                return transform.position;
            }
        }

        /// <summary>
        // Gets the point that the user is looking at
        /// </summary>
        protected static Vector3 FixationPoint
        {
            get
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                return eyesActions.Data.ReadValue<Eyes>().fixationPoint;
#else
                return Vector3.zero;
#endif
            }
        }

        /// <summary>
        /// Returns raycast direction.
        /// </summary>
        protected Vector3 Direction
        {
            get
            {
                switch (direction)
                {
                    case RaycastDirection.EyesFixationPoint:
                        return (FixationPoint - Position).normalized;

                    case RaycastDirection.Forward:
                    default:
                        return transform.forward;
                }
            }
        }

        /// <summary>
        /// Returns raycast up.
        /// </summary>
        protected Vector3 Up
        {
            get
            {
                return transform.up;
            }
        }

        private void UpdateRequestParams()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            raycastRequestParams.Position = Position;
            raycastRequestParams.Direction = Direction;
            raycastRequestParams.UpVector = Up;
            raycastRequestParams.Width = worldRayProperties.width;
            raycastRequestParams.Height = worldRayProperties.height;
            raycastRequestParams.HorizontalFovDegrees = worldRayProperties.horizontalFovDegrees;
            raycastRequestParams.CollideWithUnobserved = worldRayProperties.collideWithUnobserved;

            ray.origin = raycastRequestParams.Position;
            ray.direction = raycastRequestParams.Direction;

            modeOnWorldRaycast = mode;
#endif
        }

        private void Start()
        {
            mlInputs = new MagicLeapInputs();
            mlInputs.Enable();
            eyesActions = new MagicLeapInputs.EyesActions(mlInputs);
            UpdateRequestParams();
            raycastRequest = MLRaycast.RequestRaycast(raycastRequestParams);
        }

        private void OnDestroy()
        {
            mlInputs.Disable();
            mlInputs.Dispose();
        }

        /// <summary>
        /// Deals with what to do when the application is paused
        /// </summary>
        protected void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                isRequestComplete = true;
            }
        }

        /// <summary>
        /// Continuously casts rays
        /// </summary>
        protected void Update()
        {
            if (mode == Mode.Virtual)
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                raycastRequestParams.Position = Position;
                raycastRequestParams.Direction = Direction;
#endif

                CastVirtualRay(virtualRayProperties.distance);
            }
            else
            {
                CastWorldRay();
            }
        }

        /// <summary>
        /// Casts a world ray using _raycastParams
        /// </summary>
        protected void CastWorldRay()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (raycastRequest.TryGetResult(out MLRaycast.Request.Result result).IsOk)
            {
                HandleOnReceiveRaycast(result.State, result.Point, result.Normal, result.Confidence);
                UpdateRequestParams();
                raycastRequest.Start(raycastRequestParams);
            }
#endif
        }

        /// <summary>
        /// Casts a virtual ray using _raycastParams for origin and direction
        /// </summary>
        protected void CastVirtualRay(float distance)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (Physics.Raycast(raycastRequestParams.Position, raycastRequestParams.Direction, out raycastResult, distance, virtualRayProperties.hitLayerMask))
            {
                ray.origin = raycastRequestParams.Position;
                ray.direction = raycastRequestParams.Direction;

                OnRaycastResult?.Invoke(MLRaycast.ResultState.HitObserved, Mode.Virtual, ray, raycastResult, 1.0f);
            }
#endif
        }

        /// <summary>
        /// Sets _raycastResult based on results from callback function HandleOnReceiveRaycast.
        /// </summary>
        /// <param name="state"> The state of the raycast result.</param>
        /// <param name="point"> Position of the hit.</param>
        /// <param name="normal"> Normal of the surface hit.</param>
        /// <returns></returns>
        protected void SetWorldRaycastResult(MLRaycast.ResultState state, Vector3 point, Vector3 normal)
        {
            raycastResult = new RaycastHit();

            if (state != MLRaycast.ResultState.RequestFailed && state != MLRaycast.ResultState.NoCollision)
            {
                raycastResult.point = point;
                raycastResult.normal = normal;

#if UNITY_MAGICLEAP || UNITY_ANDROID
                raycastResult.distance = Vector3.Distance(raycastRequestParams.Position, point);
#endif
            }
        }

        /// <summary>
        /// Callback handler called when raycast call has a result.
        /// </summary>
        /// <param name="state"> The state of the raycast result.</param>
        /// <param name="point"> Position of the hit.</param>
        /// <param name="normal"> Normal of the surface hit.</param>
        /// <param name="confidence"> Confidence value on hit.</param>
        protected void HandleOnReceiveRaycast(MLRaycast.ResultState state, Vector3 point, Vector3 normal, float confidence)
        {
            if ((state == MLRaycast.ResultState.HitObserved || state == MLRaycast.ResultState.HitUnobserved) || (state == MLRaycast.ResultState.NoCollision && isLastResultHit))
            {
                if (modeOnWorldRaycast == Mode.World)
                {
                    SetWorldRaycastResult(state, point, normal);
                    OnRaycastResult?.Invoke(state, Mode.World, ray, raycastResult, confidence);
                }

                if (modeOnWorldRaycast == Mode.Combination)
                {
                    // If there was a hit on world raycast, change max distance to the hitpoint distance
                    float maxDist = (raycastResult.distance > 0.0f) ? (raycastResult.distance + WorldRayProperties.bias) : virtualRayProperties.distance;
                    CastVirtualRay(maxDist);
                }
            }

            isLastResultHit = (state == MLRaycast.ResultState.HitObserved || state == MLRaycast.ResultState.HitUnobserved);
        }
    }
}
