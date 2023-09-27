// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2021-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;

namespace MagicLeap.Core
{
    /// <summary>
    /// This class is used to Render Web View on the texture and propagate input to Web View.
    /// </summary>
    public class MLWebViewScreenBehavior : MonoBehaviour
    {
        [SerializeField, Tooltip("Web View rendered screen width in pixels.")]
        private uint webViewWidth;

        [SerializeField, Tooltip("Web View rendered screen height in pixels.")]
        private uint webViewHeight;

        [SerializeField, Tooltip("MeshRenderer which will display rendered WebView.")]
        private MeshRenderer webViewMeshRenderer = null;

        [SerializeField, Tooltip("Transform of pointer that is used to interact with WebView.")]
        private Transform pointRayTransform;

        private uint serviceConnectionCount = 0;

        public enum ScrollingMode
        {
            Touchpad,
            TriggerDrag
        }

        [Tooltip("Scrolling mode determines the form of interaction used to scroll WebView content.")]
        public ScrollingMode scrollingMode = ScrollingMode.Touchpad;

        public MLWebView WebView = null;

        /// <summary>
        /// Magic Leap input system.
        /// </summary>
        private MagicLeapInputs mlInputs;

        /// <summary>
        /// Magic Leap controller actions.
        /// </summary>
        private MagicLeapInputs.ControllerActions controllerActions;

        /// <summary>
        /// A reference of the Web View texture.
        /// </summary>
        private RenderTexture webViewTexture = null;

        /// <summary>
        /// Is WebView initialized.
        /// </summary>
        private bool isInitialized = false;

        /// <summary>
        /// Is WebView service connected.
        /// </summary>
        public bool IsConnected
        {
            get; private set;
        } = false;

        /// <summary>
        /// Is user currently scrolling.
        /// </summary>
        private bool isScrolling = false;

        /// <summary>
        /// Is the user currently pulling the trigger.
        /// </summary>
        private bool isTriggerDown = false;

        /// <summary>
        /// Is pointer over WebView window.
        /// </summary>
        private bool pointerOverWebView = false;

        /// <summary>
        /// Previous horizonal position of cursor in pixels.
        /// </summary>
        private uint previousCursorPositionX;

        /// <summary>
        /// Previous vertical position of cursor in pixels.
        /// </summary>
        private uint previousCursorPositionY;

        /// <summary>
        /// Currrent horizonal position of cursor in pixels.
        /// </summary>
        private uint currentCursorPositionX;

        /// <summary>
        /// Current vertical position of cursor in pixels.
        /// </summary>
        private uint currentCursorPositionY;

        /// <summary>
        /// Previous touchpad 1 position, used for touchpad scrolling mode.
        /// </summary> 
        private Vector2 previousTouchpadPos = new Vector2(0.0f, 0.0f);

        /// <summary>
        /// Current touchpad 1 position, used for touchpad scrolling mode.
        /// </summary> 
        private Vector2 currentTouchpadPos = new Vector2(0.0f, 0.0f);

        /// <summary> 
        /// Should the touchpad position tracking be reset.
        /// </summary>
        private bool resetTouchpadPos = false;

        /// <summary>
        /// Raycast hit used to calculate the WebView cursor position.
        /// </summary>
        private RaycastHit raycastHit;

        /// <summary>
        /// WebView Renderer.
        /// </summary>
        private MLWebView.Renderer webViewRenderer;


        public void Start()
        {
            if (pointRayTransform == null)
            {
                Debug.LogError("pointRayTransform is not set, disabling script.");
                enabled = false;
            }

            if (webViewMeshRenderer == null)
            {
                Debug.LogError("webViewMeshRenderer is not set, disabling script.");
                enabled = false;
            }
        }

        public void GetWebViewSize(out uint width, out uint height)
        {
            width = webViewWidth;
            height = webViewHeight;
        }

        /// <summary>
        /// Registers to Controller Callbacks.
        /// </summary>
        private void OnEnable()
        {
            mlInputs = new MagicLeapInputs();
            mlInputs.Enable();
            controllerActions = new MagicLeapInputs.ControllerActions(mlInputs);

            controllerActions.TouchpadPosition.performed += HandleOnTouchpadPosition;
            controllerActions.TouchpadForce.performed += HandleOnTouchpadForceDown;
            controllerActions.TouchpadForce.canceled += HandleOnTouchpadForceUp;
            controllerActions.Trigger.performed += HandleTriggerDown;
            controllerActions.Trigger.canceled += HandleTriggerUp;
        }

        /// <summary>
        /// Unregisters controller callbacks.
        /// </summary>
        private void OnDisable()
        {
            controllerActions.TouchpadPosition.performed -= HandleOnTouchpadPosition;
            controllerActions.TouchpadForce.performed -= HandleOnTouchpadForceDown;
            controllerActions.TouchpadForce.canceled -= HandleOnTouchpadForceUp;
            controllerActions.Trigger.performed -= HandleTriggerDown;
            controllerActions.Trigger.canceled -= HandleTriggerUp;
        }

        /// <summary>
        /// Destroys the WebView window.
        /// </summary>
        private void OnDestroy()
        {
            DestroyWebViewWindow();
        }

        /// <summary>
        /// Processes the input and renders WebView window.
        /// </summary>
        private void Update()
        {
            ProcessInput();

            if (isInitialized)
            {
                if (IsConnected)
                {
                    if (WebView != null)
                    {
                        if (MagicLeapNativeBindings.MLHandleIsValid(WebView.WebViewHandle))
                        {
                            if (webViewRenderer != null)
                            {
                                webViewRenderer.WebViewHandle = WebView.WebViewHandle;
                                webViewRenderer.Render();
                            }
                        }
                    }

                    if (webViewTexture != null)
                    {
                        if (!webViewTexture.IsCreated())
                        {
                            Debug.LogError("Failed to create WebViewTexture");
                        }
                    }

                }
            }
        }

        /// <summary>
        /// Handles the Touchpad position change events.
        /// </summary>
        private void HandleOnTouchpadPosition(InputAction.CallbackContext obj)
        {
            if (isInitialized)
            {
                currentTouchpadPos = obj.ReadValue<Vector2>();
                currentTouchpadPos.x *= webViewWidth;
                currentTouchpadPos.y *= webViewHeight;
            }
        }

        /// <summary>
        /// Handles the Touchpad force down change events.
        /// </summary>
        private void HandleOnTouchpadForceDown(InputAction.CallbackContext obj)
        {
            if (scrollingMode == ScrollingMode.Touchpad)
            {
                isScrolling = true;
            }
        }

        /// <summary>
        /// Handles the Touchpad force up change events.
        /// </summary>
        private void HandleOnTouchpadForceUp(InputAction.CallbackContext obj)
        {
            if (scrollingMode == ScrollingMode.Touchpad)
            {
                isScrolling = false;
                resetTouchpadPos = true;
            }
        }

        /// <summary>
        /// Handles the WebView Button Down events.
        /// </summary>
        private void HandleTriggerDown(InputAction.CallbackContext callbackContext)
        {
            if (pointerOverWebView && isInitialized && !isTriggerDown)
            {
                if (WebView != null)
                {
                    WebView.InjectMouseButtonDown(currentCursorPositionX, currentCursorPositionY, MLWebView.EventFlags.LeftMouseButton);
                    isTriggerDown = true;

                    if (scrollingMode == ScrollingMode.TriggerDrag)
                    {
                        isScrolling = true;
                    }
                }
            }
        }

        /// <summary>
        /// Handles the WebView Button Up events.
        /// </summary>
        private void HandleTriggerUp(InputAction.CallbackContext callbackContext)
        {
            if (pointerOverWebView && isInitialized && isTriggerDown)
            {
                WebView?.InjectMouseButtonUp(currentCursorPositionX, currentCursorPositionY, MLWebView.EventFlags.LeftMouseButton);
                isTriggerDown = false;
            }
            if (scrollingMode == ScrollingMode.TriggerDrag)
            {
                isScrolling = false;
            }
        }

        /// <summary>
        /// Calculates the WebView cursor position and scroll value.
        /// </summary>
        private void ProcessInput()
        {
            if (Physics.Raycast(pointRayTransform.position, pointRayTransform.forward, out raycastHit) && raycastHit.collider.gameObject == this.gameObject)
            {
                if (!EventSystem.current.IsPointerOverGameObject())
                {
                    pointerOverWebView = true;

                    if (isInitialized)
                    {
                        if (WebView != null)
                        {
                            var position = webViewMeshRenderer.transform.position;
                            var bounds = webViewMeshRenderer.bounds;
                            currentCursorPositionX = (uint)((raycastHit.point.x - position.x) / bounds.size.x * webViewWidth + webViewWidth / 2);
                            currentCursorPositionY = (uint)(-(raycastHit.point.y - position.y) / bounds.size.y * webViewHeight + webViewHeight / 2);

                            if (isTriggerDown)
                            {
                                WebView.InjectMouseMove(currentCursorPositionX, currentCursorPositionY, MLWebView.EventFlags.LeftMouseButton);
                            }
                            else
                            {
                                WebView.InjectMouseMove(currentCursorPositionX, currentCursorPositionY, MLWebView.EventFlags.None);
                            }

                            if (isScrolling && scrollingMode == ScrollingMode.TriggerDrag)
                            {
                                WebView.ScrollBy(currentCursorPositionX - previousCursorPositionX, currentCursorPositionY - previousCursorPositionY);
                            }

                            previousCursorPositionX = currentCursorPositionX;
                            previousCursorPositionY = currentCursorPositionY;
                        }
                    }
                }
                else
                {
                    pointerOverWebView = false;
                }
            }
            else
            {
                pointerOverWebView = false;
            }

            if (isInitialized)
            {
                if (isScrolling && scrollingMode == ScrollingMode.Touchpad)
                {
                    if (resetTouchpadPos)
                    {
                        previousTouchpadPos = currentTouchpadPos;
                        resetTouchpadPos = false;
                    }
                    else
                    {
                        WebView?.ScrollBy((uint)(currentTouchpadPos.x - previousTouchpadPos.x), (uint)(currentTouchpadPos.y - previousTouchpadPos.y));
                    }
                }

                previousTouchpadPos = currentTouchpadPos;
            }
        }

        public void ServiceConnected()
        {
            serviceConnectionCount++;
            IsConnected = true;
        }

        public void ServiceDisconnected()
        {
            if (--serviceConnectionCount == 0)
            {
                IsConnected = false;

                if (webViewTexture != null)
                {
                    // clear the texture
                    RenderTexture rt = RenderTexture.active;
                    RenderTexture.active = webViewTexture;
                    GL.Clear(true, true, Color.clear);
                    RenderTexture.active = rt;

                    // Set texture on quad
                    webViewMeshRenderer.material.mainTexture = this.webViewTexture;
                }
            }
        }

        /// <summary>
        /// Creates the WebView window.
        /// </summary>
        /// <returns>True if created WebView window with success.</returns>
        public bool CreateWebViewWindow()
        {
            if (!isInitialized)
            {
                CreateTexture((int)webViewWidth, (int)webViewHeight);
                isInitialized = true;
                return true;
            }
            return false;
        }

        /// <summary>
        /// Destroys the WebView window.
        /// </summary>
        public void DestroyWebViewWindow()
        {
            if (isInitialized)
            {
                isInitialized = false;
                webViewRenderer.Cleanup();
                webViewRenderer = null;
            }
        }


        /// <summary>
        /// Creates texture on which WebView will be rendered.
        /// </summary>
        /// <param name="width">Web View rendered screen width in pixels.</param>
        /// <param name="height">Web View rendered screen height in pixels.</param>
        private void CreateTexture(int width, int height)
        {
            width = Mathf.Max(width, 1);
            height = Mathf.Max(height, 1);

            if (webViewTexture != null && (webViewTexture.width != width || webViewTexture.height != height))
            {
                Destroy(webViewTexture);
                webViewTexture = null;
            }

            if (webViewTexture == null)
            {
                // Create texture with given dimensions
                webViewTexture = new RenderTexture(width, height, 0, RenderTextureFormat.ARGB32);

                // Set texture on quad
                webViewMeshRenderer.material.mainTexture = this.webViewTexture;
            }

            webViewRenderer = new MLWebView.Renderer();
            webViewRenderer.SetRenderBuffer(this.webViewTexture);
        }
    }
}
