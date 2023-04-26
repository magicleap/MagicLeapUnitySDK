using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.XR.MagicLeap;

namespace MagicLeap.Core
{
    [RequireComponent(typeof(ToggleGroup))]
    public class MLWebViewTabBarBehavior : MonoBehaviour
    {
        [SerializeField, Tooltip("Prefab to use for the web view tabs")]
        private MLWebViewTabBehavior tabPrefab;

        [SerializeField, Tooltip("Web view screen of the browser")]
        private MLWebViewScreenBehavior webViewScreen;

        [SerializeField, Tooltip("URL address bar of the browser")]
        private InputField addressBar;

        private ToggleGroup toggleGroup;

        public MLWebViewTabBehavior currentTab
        {
            get; private set;
        } = null;

        private Stack<MLWebViewTabBehavior> previousTabs = new Stack<MLWebViewTabBehavior>();

        public Action<MLWebViewTabBehavior, string> OnTabCreated;
        public Action<MLWebViewTabBehavior> OnTabDestroyed;

        void Awake()
        {
            toggleGroup = GetComponent<ToggleGroup>();
        }

        void Start()
        {
            if (tabPrefab == null)
            {
                Debug.LogError("tabPrefab is not set, disabling script");
                enabled = false;
            }
            if (webViewScreen == null)
            {
                Debug.LogError("webViewScreen is not set, disabling script");
                enabled = false;
            }
        }

        public void CreateTab()
        {
            MLWebViewTabBehavior newTab = Instantiate(tabPrefab, this.transform);
            newTab.GetComponent<Toggle>().group = toggleGroup;
            if (newTab.CreateTab(this, webViewScreen, addressBar))
            {
                SelectTab(newTab);

                // notify listeners that the tab was created
                OnTabCreated?.Invoke(newTab, null);
            }
        }

        public void CreatePopupTab(MLWebView popupWebView, ulong popupID, string url)
        {
            MLWebViewTabBehavior newTab = Instantiate(tabPrefab, this.transform);
            newTab.GetComponent<Toggle>().group = toggleGroup;
            if (newTab.CreateTab(this, webViewScreen, addressBar, (popupWebView != null), popupID))
            {
                SelectTab(newTab);

                // notify listeners that the tab was created
                OnTabCreated?.Invoke(newTab, url);
            }
        }

        public void SelectTab(MLWebViewTabBehavior tab)
        {
            if (currentTab != null)
            {
                if (currentTab == tab)
                {
                    // tab is already selected
                    return;
                }
                currentTab.UnselectTab();
                previousTabs.Push(currentTab);
            }
            currentTab = tab;
            currentTab.SelectTab();

            if (addressBar != null)
            {
                addressBar.text = currentTab.tabUrl;
            }
        }

        public MLWebViewTabBehavior[] GetAllTabs()
        {
            return GetComponentsInChildren<MLWebViewTabBehavior>();
        }

        public MLWebViewTabBehavior GetTab(MLWebView webView)
        {
            MLWebViewTabBehavior[] allTabs = GetComponentsInChildren<MLWebViewTabBehavior>();
            foreach (MLWebViewTabBehavior tab in allTabs)
            {
                if (tab.WebView == webView)
                {
                    return tab;
                }
            }
            return null;
        }

        public void DestroyCurrentTab()
        {
            if (currentTab != null)
            {
                // notify listeners that the tab is being destroyed
                OnTabDestroyed?.Invoke(currentTab);

                currentTab.DestroyTab();
                MLWebViewTabBehavior previousTab = null;
                while (previousTab == null && previousTabs.Count > 0)
                {
                    previousTab = previousTabs.Pop();
                }
                currentTab = previousTab;
                if (currentTab != null)
                {
                    currentTab.SelectTab();
                }
                else
                {
                    // look for any tab
                    currentTab = GetComponentInChildren<MLWebViewTabBehavior>();
                }
            }
        }
    }
}
