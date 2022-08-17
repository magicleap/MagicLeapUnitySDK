#if URP_14_0_0_OR_NEWER
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace UnityEngine.XR.MagicLeap
{
    public static class URPExtensions
    {
        /// <summary>
        /// Retrieve the first ScriptableRendererFeature of type T found among all of the URP asset's Renderers
        /// </summary>
        public static ScriptableRendererFeature GetRendererFeature<T>(this UniversalRenderPipelineAsset asset) where T : ScriptableRendererFeature
        {
            var universalRenderData = asset.GetRendererList();

            if (universalRenderData != null && universalRenderData.Count > 0)
            {
                foreach (var renderData in universalRenderData)
                {
                    foreach (var rendererFeature in renderData.rendererFeatures)
                    {
                        if (rendererFeature is T)
                        {
                            return rendererFeature;
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Retrieve the ScriptableRendererFeature of type T from the URP Renderer asset
        /// </summary>
        public static ScriptableRendererFeature GetRendererFeature<T>(this UniversalRendererData asset) where T : ScriptableRendererFeature
        {
            foreach (var rendererFeature in asset.rendererFeatures)
            {
                if (rendererFeature is T)
                {
                    return rendererFeature;
                }
            }

            return null;
        }

        /// <summary>
        /// Quickly retrieve the first (and probably only) item in the URP asset's Renderer List
        /// </summary>
        public static UniversalRendererData GetFirstRenderer(this UniversalRenderPipelineAsset asset)
        {
            var rendererList = asset.GetRendererList();

            if(rendererList != null)
            {
                if(rendererList.Count > 0)
                {
                    return rendererList[0];
                }
            }

            return null;
        }

        /// <summary>
        /// Uses reflection to retrieve the list of UniversalRendererData assets assigned to the URP asset's Renderer List
        /// </summary>
        public static List<UniversalRendererData> GetRendererList(this UniversalRenderPipelineAsset asset)
        {
            var type = asset.GetType();
            var propertyInfo = type.GetField("m_RendererDataList", BindingFlags.Instance | BindingFlags.NonPublic);

            if (propertyInfo == null)
            {
                return null;
            }

            var scriptableRenderDataArray = (ScriptableRendererData[])propertyInfo.GetValue(asset);

            var urpDataList = new List<UniversalRendererData>();
            
            if(scriptableRenderDataArray != null)
            {
                foreach(var dataAsset in scriptableRenderDataArray)
                {
                    if(dataAsset is UniversalRendererData)
                    {
                        urpDataList.Add(dataAsset as UniversalRendererData);
                    }
                }
            }

            return urpDataList;
        }
    }
}
#endif
