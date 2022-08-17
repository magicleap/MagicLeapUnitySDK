// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// <copyright file="MLRaycast.cs" company="Magic Leap, Inc">
//
// Copyright (c) 2018 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
// </copyright>
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

namespace UnityEngine.XR.MagicLeap
{
#if UNITY_MAGICLEAP || UNITY_ANDROID
    using UnityEngine.XR.MagicLeap.Native;
#endif

    /// <summary>
    /// Sends requests to create Rays intersecting world geometry and returns results through callbacks.
    /// </summary>
    public partial class MLRaycast : MLAutoAPISingleton<MLRaycast>
    {
        public static Request RequestRaycast(Request.Params requestParams) => new Request(requestParams);

        /// <summary>
        /// Enumeration of ray cast result states.
        /// </summary>
        public enum ResultState
        {
            /// <summary>
            /// The ray cast request failed.
            /// </summary>
            RequestFailed = -1,

            /// <summary>
            /// The ray passed beyond maximum ray cast distance and it doesn't hit any surface.
            /// </summary>
            NoCollision,

            /// <summary>
            /// The ray hit unobserved area. This will on occur when collide_with_unobserved is set to true.
            /// </summary>
            HitUnobserved,

            /// <summary>
            /// The ray hit only observed area.
            /// </summary>
            HitObserved,
        }

        public partial class Request : MLRequest<Request.Params, Request.Result>
        {
            internal Request(Params parameters) => Start(parameters);

            public override MLResult Start(Params parameters)
            {
                this.Dispose(true);
                this.parameters = parameters;
#if UNITY_MAGICLEAP || UNITY_ANDROID
                return MLResult.Create(MLRaycast.Instance.RequestInternal(this.parameters, ref this.handle));
#else
                return default;
#endif
            }

            public override MLResult TryGetResult(out Result result)
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                if (!MagicLeapNativeBindings.MLHandleIsValid(this.handle))
                {
                    MLResult startResult = this.Start(this.parameters);
                    if (!startResult.IsOk)
                    {
                        result = default;
                        return startResult;
                    }
                }

                MLResult.Code resultCode = MLRaycast.Instance.GetResult(MLRaycast.Instance.trackerHandle, handle, out NativeBindings.MLRaycastResultNative nativeResult);
                MLResult mlResult = MLResult.Create(resultCode);

                result = new Result(nativeResult);

                if (mlResult.IsOk)
                    this.Dispose(true);

                return mlResult;
#else
                result = default;
                return default;
#endif
            }

            protected override void Dispose(bool disposing)
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                this.handle = MagicLeapNativeBindings.InvalidHandle;
#endif
            }
        }

        #region Request
        public partial class Request
        {
            public struct Params
            {
                /// <summary>
                /// Gets or sets where the ray is cast from.
                /// </summary>
                public Vector3 Position;

                /// <summary>
                /// Gets or sets the direction of the ray to fire.
                /// </summary>
                public Vector3 Direction;

                /// <summary>
                /// Gets or sets the up vector of the ray to fire.  Use (0, 0, 0) to use the up vector of the rig frame.
                /// </summary>
                public Vector3 UpVector;

                /// <summary>
                /// Gets or sets the number of horizontal rays. For single point ray cast, set this to 1.
                /// </summary>
                public uint Width;

                /// <summary>
                /// Gets or sets the number of vertical rays. For single point ray cast, set this to 1.
                /// </summary>
                public uint Height;

                /// <summary>
                /// Gets or sets the horizontal field of view, in degrees.
                /// </summary>
                public float HorizontalFovDegrees;

                /// <summary>
                /// Gets or sets a value indicating whether a ray will terminate when encountering an unobserved area and return
                /// a surface or the ray will continue until it ends or hits a observed surface.
                /// </summary>
                public bool CollideWithUnobserved;
            }
        }

        public partial class Request
        {
            public struct Result
            {
#if UNITY_MAGICLEAP || UNITY_ANDROID
                internal Result(NativeBindings.MLRaycastResultNative nativeResult)
                {
                    this.State = nativeResult.State;
                    this.Confidence = nativeResult.Confidence;

                    bool didHit = nativeResult.State != ResultState.RequestFailed && nativeResult.State != ResultState.NoCollision;
                    this.Point = didHit ? MLConvert.ToUnity(nativeResult.Hitpoint) : Vector3.zero;
                    this.Normal = didHit ? MLConvert.ToUnity(nativeResult.Normal, true, false) : Vector3.zero;
                }
#endif

                /// <summary>
                /// Gets or sets the result state of the performed raycast.
                /// </summary>
                public MLRaycast.ResultState State;

                /// <summary>
                /// Gets or sets the hit point of the performed raycast.
                /// </summary>
                public Vector3 Point;

                /// <summary>
                /// Gets or sets the normal of the performed raycast.
                /// </summary>
                public Vector3 Normal;

                /// <summary>
                /// Gets or sets the confidence factor of the performed raycast.
                /// </summary>
                public float Confidence;
            }
        }
        #endregion Request
    }
}
