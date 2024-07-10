using System;
using System.Collections;
using System.Collections.Generic;
using MagicLeap.OpenXR;
using MagicLeap.OpenXR.Features;
using UnityEngine.XR.OpenXR;
using Utils = MagicLeap.OpenXR.Utils;
#if UNITY_XR_MAGICLEAP_PROVIDER
using MagicLeapXRRenderSettings = UnityEngine.XR.MagicLeap.Rendering.RenderingSettings;
using MagicLeapXRRenderUtility = UnityEngine.XR.MagicLeap.Rendering.RenderingUtility;
#endif

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// MonoBehaviour class representing the Magic Leap camera
    /// </summary>
    [ExecuteInEditMode]
    [DefaultExecutionOrder(ScriptOrder)]
    [RequireComponent(typeof(Camera))]
    [Obsolete("The MagicLeapCamera component is not recommended for use with OpenXR. It has been deprecated and will be removed in a future release.")]
    public sealed class MagicLeapCamera : MonoBehaviour
    {
        /// <summary>
        /// Ensures this MonoBehavior runs its event methods very early before most other scripts.
        /// </summary>
        private const int ScriptOrder = -15000;

        private new Camera camera;

        private List<Transform> transforms = new List<Transform>();
        private Unity.Jobs.JobHandle jobHandle;
        
        [SerializeField]
        private bool enforceFarClip = false;
        [SerializeField]
        private Transform stereoConvergencePoint;
        [SerializeField]
        private bool protectedSurface;

        [SerializeField] 
        private bool vignette;

        /// <summary>
        /// Correct any issues with the camera on startup
        /// </summary>
        [SerializeField]
        private bool fixProblemsOnStartup = true;

        /// <summary>
        /// Recenter the XROrigin one frame after Start() is called.
        /// </summary>
        [SerializeField]
        private bool recenterXROriginAtStart = true;

        /// <summary>
        /// The minimum recommended near clip value
        /// </summary>
        private static readonly float MINIMUM_NEAR_CLIP_METERS = MLDevice.MinimumNearClipDistance;

        /// <summary>
        /// The recommended clear mode for the camera
        /// </summary>
        private static readonly CameraClearFlags DEFAULT_CLEAR_FLAGS = CameraClearFlags.SolidColor;

        /// <summary>
        /// The recommended clear color for the camera
        /// </summary>
        private static readonly Color DEFAULT_CLEAR_COLOR = new Color(0.0f, 0.0f, 0.0f, 0.0f);

        /// <summary>
        /// The recommended camera position
        /// </summary>
        private static readonly Vector3 DEFAULT_CAMERA_POSITION = Vector3.zero;

        /// <summary>
        /// The recommended camera rotation
        /// </summary>
        private static readonly Quaternion DEFAULT_CAMERA_ROTATION = Quaternion.identity;

        /// <summary>
        /// The recommended camera scale
        /// </summary>
        private static readonly Vector3 DEFAULT_CAMERA_SCALE = Vector3.one;

        private static float systemSettingClippingPlaneVal = MLDevice.DefaultNearClipDistance;

        /// <summary>
        /// Getter/Setter for the stereo convergence point
        /// </summary>
        public Transform StereoConvergencePoint
        {
            get => stereoConvergencePoint;
            set => stereoConvergencePoint = value;
        }

        /// <summary>
        /// Getter/Setter for setting the Protected surface attribute.
        /// </summary>
        public bool ProtectedSurface
        {
            get => protectedSurface;
            set => protectedSurface = value;
        }

        public bool EnforceFarClip
        {
            get => enforceFarClip;
            set => enforceFarClip = value;
        }

        public bool RecenterXROriginAtStart
        {
            get => recenterXROriginAtStart;
            set => recenterXROriginAtStart = value;
        }

        private void Awake()
        {
            camera = GetComponent<Camera>();
            FixupCamera(fixProblemsOnStartup);

            systemSettingClippingPlaneVal = GetNearClipPlaneFromSystem();

            if (!Application.isEditor)
            {
                Debug.Log($"Acquired clipping plane value from system: {systemSettingClippingPlaneVal}");

                if (MLDevice.IsOpenXRLoaderActive())
                {
                    var renderFeature = OpenXRSettings.Instance.GetFeature<MagicLeapRenderingExtensionsFeature>();
                    if (renderFeature == null)
                        return;
                    renderFeature.FocusDistance = camera.stereoConvergence;
                    renderFeature.UseProtectedSurface = protectedSurface;
                    renderFeature.UseVignetteMode = vignette;
                }
            }
        }

        private IEnumerator Start()
        {
            yield return new WaitForEndOfFrame();
            if (recenterXROriginAtStart)
            {
                var xro = FindObjectOfType<Unity.XR.CoreUtils.XROrigin>();
                xro?.MoveCameraToWorldLocation(Vector3.zero);
                xro?.MatchOriginUpCameraForward(Vector3.up, Vector3.forward);
            }
        }

        private void Reset()
        {
            FixupCamera(fixProblemsOnStartup);
        }

        private void LateUpdate()
        {
            if (!(Application.isPlaying && MLDevice.IsMagicLeapLoaderActive()))
            {
                return;
            }

            if (MLDevice.IsMagicLeapLoaderActive())
            {
#if UNITY_XR_MAGICLEAP_PROVIDER
                MagicLeapXRRenderSettings.cameraScale = MagicLeapXRRenderUtility.GetParentScale(transform);
#endif
            }
            ValidateNearClip();
            ValidateFarClip();

            camera.stereoConvergence = CalculateFocusDistance();
            if (MLDevice.IsOpenXRLoaderActive())
            {
                if (!Utils.TryGetOpenXRFeature<MagicLeapRenderingExtensionsFeature>(out var renderFeature))
                    return;
                renderFeature.FocusDistance = camera.stereoConvergence;
                renderFeature.UseProtectedSurface = protectedSurface;
                renderFeature.UseVignetteMode = vignette;
            }
            else if (MLDevice.IsMagicLeapLoaderActive())
            {
#if UNITY_XR_MAGICLEAP_PROVIDER
                MagicLeapXRRenderSettings.focusDistance = camera.stereoConvergence;
                MagicLeapXRRenderSettings.farClipDistance = camera.farClipPlane;
                MagicLeapXRRenderSettings.nearClipDistance = camera.nearClipPlane;
#endif
            }
        }

#if !UNITY_EDITOR
        private void OnApplicationPause(bool pause)
        {
            if(!pause)
            {
                CheckForClipPlaneSettingChange();
            }
        }
#endif

        private void ValidateNearClip()
        {
            if (camera.nearClipPlane < systemSettingClippingPlaneVal)
            {
                // reacquire from system just in case
                systemSettingClippingPlaneVal = GetNearClipPlaneFromSystem();
                if (camera.nearClipPlane < systemSettingClippingPlaneVal)
                {
                    // unity camera nearClipPlane cannot go below what is set in system settings
                    camera.nearClipPlane = Mathf.Max(camera.nearClipPlane, systemSettingClippingPlaneVal);
                }
            }
        }

        /// <summary>
        /// Validate the Far Clip plane
        /// </summary>
        public void ValidateFarClip()
        {
            if (MLDevice.IsMagicLeapLoaderActive())
            {
                var farClip = camera.farClipPlane;
                float max = 100f;
#if UNITY_XR_MAGICLEAP_PROVIDER
                max = MagicLeapXRRenderSettings.maxFarClipDistance;
#endif
                if (farClip > max)
                {
                    if (enforceFarClip)
                    {
                        camera.farClipPlane = max;
                    }
                }
            }
        }

        private float CalculateFocusDistance()
        {
            // Get Focus Distance and log warnings if not within the allowed value bounds.
            float focusDistance = camera.stereoConvergence;
            if (StereoConvergencePoint != null)
            {
                // From Unity documentation:
                // Note that camera space matches OpenGL convention: camera's forward is the negative Z axis.
                // This is different from Unity's convention, where forward is the positive Z axis.
                Vector3 worldForward = new Vector3(0.0f, 0.0f, -1.0f);
                Vector3 camForward = camera.cameraToWorldMatrix.MultiplyVector(worldForward);
                camForward = camForward.normalized;

                // We are only interested in the focus object's distance to the camera forward tangent plane.
                focusDistance = Vector3.Dot(StereoConvergencePoint.position - transform.position, camForward);
            }

            float nearClip = camera.nearClipPlane;
            if (focusDistance < nearClip)
            {
                focusDistance = nearClip;
            }

            return focusDistance;
        }

        /// <summary>
        /// Clamp a given value to the near and far clip planes
        /// </summary>
        /// <param name="value">Value to clamp</param>
        /// <returns>Clamped value</returns>
        public float ClampToClippingPlanes(float value)
        {
            return Mathf.Clamp(value, systemSettingClippingPlaneVal,
#if UNITY_XR_MAGICLEAP_PROVIDER
                MagicLeapXRRenderSettings.maxFarClipDistance
#else
                value
#endif
                );
        }

        private void FixupCamera(bool fixIssues)
        {
            Vector3 cameraScale = GetCameraScale();
            float scale;
            if (!(Mathf.Approximately(cameraScale.x, cameraScale.y) && Mathf.Approximately(cameraScale.x, cameraScale.z)))
            {
                // Non uniform scale
                scale = (cameraScale.x + cameraScale.y + cameraScale.z) / 3.0f;
            }
            else
            {
                scale = cameraScale.x;
            }

            if ((camera.nearClipPlane / scale) < MINIMUM_NEAR_CLIP_METERS)
            {
                Debug.LogWarning("[Magic Leap] The near clipping plane of the main camera is closer than " + MINIMUM_NEAR_CLIP_METERS + "m, which can cause artifacts.");

                if (fixIssues)
                {
                    camera.nearClipPlane = MINIMUM_NEAR_CLIP_METERS * scale;
                }
            }

            if ((camera.clearFlags != DEFAULT_CLEAR_FLAGS) ||
                (camera.backgroundColor != DEFAULT_CLEAR_COLOR))
            {
                Debug.LogWarning("[Magic Leap] The main camera background is not solid black with an alpha of zero, which is recommended for the Magic Leap platform.");

                if (fixIssues)
                {
                    camera.clearFlags = DEFAULT_CLEAR_FLAGS;
                    camera.backgroundColor = DEFAULT_CLEAR_COLOR;
                }
            }

            if ((camera.transform.localPosition != DEFAULT_CAMERA_POSITION) ||
                (camera.transform.localRotation != DEFAULT_CAMERA_ROTATION) ||
                (camera.transform.localScale != DEFAULT_CAMERA_SCALE))
            {
                Debug.LogWarning("[Magic Leap] The main camera isn't at the origin. This will cause head tracking problems.");

                if (fixIssues)
                {
                    camera.transform.localPosition = DEFAULT_CAMERA_POSITION;
                    camera.transform.localRotation = DEFAULT_CAMERA_ROTATION;
                    camera.transform.localScale = DEFAULT_CAMERA_SCALE;
                }
            }
        }

        /// <summary>
        /// Get the scale of the camera based on its parent transform
        /// </summary>
        /// <returns></returns>
        private Vector3 GetCameraScale()
        {
            Vector3 scale = Vector3.one;

            if (camera && camera == Camera.main)
            {
                if (transform.parent)
                {
                    scale = transform.parent.lossyScale;
                }
            }

            return scale;
        }

        private float GetNearClipPlaneFromSystem()
        {
            if (!Application.isEditor)
            {
                using (var actClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
                {
                    var context = actClass.GetStatic<AndroidJavaObject>("currentActivity");
                    AndroidJavaClass systemGlobal = new AndroidJavaClass("android.provider.Settings$Secure");

                    var clipPlaneDist = systemGlobal.CallStatic<float>("getFloat", context.Call<AndroidJavaObject>("getContentResolver"), "clipping_plane_distance", MLDevice.DefaultNearClipDistance);

                    // if the setting doesn't exist we'll get the default value from MLDevice in meters.
                    if (clipPlaneDist < 1f)
                        return clipPlaneDist;

                    // Otherwise the setting returns centimeters and we need to convert.
                    return clipPlaneDist / 100f;
                }
            }

            return MLDevice.DefaultNearClipDistance;
        }

        public void CheckForClipPlaneSettingChange()
        {
            systemSettingClippingPlaneVal = GetNearClipPlaneFromSystem();
            if (!Application.isEditor)
                Debug.Log($"Acquired clipping plane value from system: {systemSettingClippingPlaneVal}");
        }
    }
}
