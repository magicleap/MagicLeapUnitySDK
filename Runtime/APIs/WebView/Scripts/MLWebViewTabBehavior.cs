using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Native;
using UnityEngine.UI;

namespace MagicLeap.Core
{
    [RequireComponent(typeof(Toggle))]
    public class MLWebViewTabBehavior : MonoBehaviour
    {
        /// <summary>
        /// MLWebView instance that this tab is associated with.
        /// </summary>
        public MLWebView WebView
        {
            get; private set;
        }

        private MLWebViewTabBarBehavior tabBar;
        private MLWebViewScreenBehavior webViewScreen;
        private InputField addressBar;

        private Toggle toggle;
        private Text text;

        private bool loadOnServiceConnected = false;

        public string tabUrl
        {
            get; private set;
        }

        void Awake()
        {
            toggle = GetComponent<Toggle>();
            toggle.onValueChanged.AddListener(OnValueChanged);

            text = GetComponentInChildren<Text>();
        }

        void OnDestroy()
        {
            DestroyTab();
        }

        public bool CreateTab(MLWebViewTabBarBehavior tabBar, MLWebViewScreenBehavior webViewScreen, InputField addressBar)
        {
            if (!MLPermissions.CheckPermission(MLPermission.WebView).IsOk)
            {
                Debug.LogError($"You must include {MLPermission.WebView} in AndroidManifest.xml to run this example");
                Destroy(this);
                return false;
            }

            this.tabBar = tabBar;
            this.webViewScreen = webViewScreen;
            this.addressBar = addressBar;
            webViewScreen.GetWebViewSize(out uint width, out uint height);
#if UNITY_MAGICLEAP || UNITY_ANDROID
            WebView = MLWebView.Create(width, height);
            if (WebView == null)
            {
                Debug.LogError("Failed to create new web view tab");
                Destroy(this);
                return false;
            }

            WebView.OnLoadEnded += OnLoadEnded;
            WebView.OnServiceConnected += OnServiceConnected;
            return true;
#else 
            return false;
#endif
        }

        public void SelectTab()
        {
            if (webViewScreen != null)
            {
                webViewScreen.WebView = WebView;
            }
            if (tabBar != null)
            {
                tabBar.SelectTab(this);
            }
            toggle.isOn = true;
        }

        public void UnselectTab()
        {
            toggle.isOn = false;
        }

        private void OnValueChanged(bool selected)
        {
            if (selected)
            {
                SelectTab();
            }
        }

        public void DestroyTab()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (webViewScreen != null)
            {
                if (webViewScreen.WebView == WebView)
                {
                    webViewScreen.WebView = null;
                }
            }

            if (WebView != null)
            {
                WebView.OnLoadEnded -= OnLoadEnded;
                WebView.OnServiceConnected -= OnServiceConnected;

                // manually call ServiceDisconnected because webview could be garbage collected
                // prior to the callback coming back from platform
                webViewScreen.ServiceDisconnected();

                if (!WebView.Destroy().IsOk)
                {
                    Debug.LogError("Failed to destroy webview tab " + WebView.WebViewHandle);
                }
                else
                {
                    WebView = null;
                    Destroy(gameObject);
                }
            }
#endif
        }

        public void GoToUrl(string url)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (WebView != null)
            {
                if (webViewScreen.IsConnected)
                {
                    if (!WebView.GoTo(url).IsOk)
                    {
                        Debug.LogError("Failed to navigate to url " + url);
                    }
                    else
                    {
                        tabUrl = url;
                    }
                }
                else
                {
                    tabUrl = url;
                    loadOnServiceConnected = true;
                }

            }
#endif
        }

        public void UpdateTabLabel()
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (text != null && WebView != null)
            {
                tabUrl = WebView.GetURL();
                System.Uri uri = new System.Uri(tabUrl);
                text.text = uri.Host;
            }
#endif
        }

        private void OnServiceConnected(MLWebView webView)
        {
#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (webViewScreen != null)
            {
                webViewScreen.ServiceConnected();
            }

            if (loadOnServiceConnected)
            {
                loadOnServiceConnected = false;
                if (!String.IsNullOrEmpty(tabUrl))
                {
                    if (!WebView.GoTo(tabUrl).IsOk)
                    {
                        Debug.LogError("Failed to navigate to url " + tabUrl);
                    }
                }
            }
#endif
        }

        private void OnLoadEnded(MLWebView webView, bool isMainFrame, int httpStatusCode)
        {
            UpdateTabLabel();

#if UNITY_MAGICLEAP || UNITY_ANDROID
            if (WebView != null && webViewScreen.WebView == WebView)
            {
                if (addressBar != null)
                {
                    addressBar.text = WebView.GetURL();
                }
            }
#endif
        }
    }
}