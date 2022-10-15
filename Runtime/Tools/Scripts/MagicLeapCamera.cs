using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine.Jobs;
using UnityEngine.XR.MagicLeap.Rendering;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// MonoBehaviour class representing the Magic Leap camera
    /// </summary>
    [ExecuteInEditMode]
    [AddComponentMenu("AR/Magic Leap/Magic Leap Camera")]
    [DefaultExecutionOrder(ScriptOrder)]
    [RequireComponent(typeof(Camera))]
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
        private bool enforceNearClip = true;
        [SerializeField]
        private bool enforceFarClip = false;
        [SerializeField]
        private Transform stereoConvergencePoint;
        [SerializeField]
        private bool protectedSurface;

        /// <summary>
        /// Correct any issues with the camera on startup
        /// </summary>
        [SerializeField]
        private bool fixProblemsOnStartup = true;

        /// <summary>
        /// The minimum recommended near clip value
        /// </summary>
        private static readonly float MINIMUM_NEAR_CLIP_METERS =
#if UNITY_MAGICLEAP || UNITY_ANDROID
            MLDevice.MinimumNearClipDistance;
#else
            0.0f;
#endif

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

        private void Awake()
        {
            camera = GetComponent<Camera>();
            FixupCamera(fixProblemsOnStartup);
        }

        private void Reset()
        {
            FixupCamera(fixProblemsOnStartup);
        }

        private void LateUpdate()
        {
#if UNITY_ANDROID
            if (!(Application.isPlaying && MLDevice.IsMagicLeapLoaderActive()))
            {
                return;
            }
#endif

            RenderingSettings.cameraScale = RenderingUtility.GetParentScale(transform);
            ValidateFarClip();
            ValidateNearClip();

            camera.stereoConvergence = CalculateFocusDistance();
            RenderingSettings.focusDistance = camera.stereoConvergence;
            RenderingSettings.farClipDistance = camera.farClipPlane;
            RenderingSettings.nearClipDistance = camera.nearClipPlane;
        }

        /// <summary>
        /// Validate the Far Clip plane
        /// </summary>
        public void ValidateFarClip()
        {
            var farClip = camera.farClipPlane;
            var max = RenderingSettings.maxFarClipDistance;
            if (farClip > max)
            {
                if (enforceFarClip)
                {
                    camera.farClipPlane = max;
                }
            }
        }

        /// <summary>
        /// Validate the Near Clip plane
        /// </summary>
        public void ValidateNearClip()
        {
            var nearClip = camera.nearClipPlane;
            var min = RenderingSettings.minNearClipDistance;
            if (nearClip < min)
            {
                if (enforceNearClip)
                {
                    camera.nearClipPlane = min;
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
            return Mathf.Clamp(value, RenderingSettings.minNearClipDistance, RenderingSettings.maxFarClipDistance);
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
    }
}
