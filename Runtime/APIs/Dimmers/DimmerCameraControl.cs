// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2019-2023) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

#if UNITY_OPENXR_1_7_0_OR_NEWER
namespace UnityEngine.XR.OpenXR.Features.MagicLeapSupport
{
    [RequireComponent(typeof(Camera))]
    [ExecuteInEditMode]
    public class DimmerCameraControl : MonoBehaviour
    {
        private Camera _camera;
        private Camera mainCamera;

        [SerializeField] 
        private Shader shader;

        // Start is called before the first frame update
        private void OnEnable()
        {
            _camera = GetComponent<Camera>();
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError($"Could not find Main Camera!");
                return;
            }

            _camera.clearFlags = CameraClearFlags.Depth;
            _camera.depth = mainCamera.depth - 1;

            if (shader != null)
            {
                if (Application.isPlaying && !Application.isEditor)
                {
                    _camera.SetReplacementShader(shader, "RenderType");
                }
            }
        }

        private void OnValidate()
        {
            if (mainCamera != null)
            {
                if (_camera.depth >= mainCamera.depth)
                {
                    Debug.LogWarning($"Camera for Dimmer must have a depth lower than the Main Camera.");
                    _camera.depth = mainCamera.depth - 1;
                }

                if (_camera.clearFlags != CameraClearFlags.Depth)
                {
                    Debug.LogWarning($"Camera for Dimmer must have clearFlags set to Depth only.");
                    _camera.clearFlags = CameraClearFlags.Depth;
                }
            }
        }
    }
}
#endif
