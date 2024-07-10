using System.Collections;
using System.Collections.Generic;
using MagicLeap.OpenXR.Features.Meshing;
using UnityEngine;
using UnityEngine.XR.Management;
#if UNITY_OPENXR_1_9_0_OR_NEWER
#endif

namespace UnityEngine.XR.MagicLeap
{
    public static class MeshingSubsystemLifecycle
    {
        public static XRMeshSubsystem MeshSubsystem { get; private set; }

        private static int subsysRefCount = 0;

#if UNITY_XR_MAGICLEAP_PROVIDER
        private static MagicLeapLoader mlLoader;
#endif

#if UNITY_OPENXR_1_9_0_OR_NEWER
        private static MagicLeapMeshingFeature mlOpenXrFeature;
#endif

        public static IEnumerator WaitUntilInited()
        {
            while (XRGeneralSettings.Instance == null)
            {
                yield return null;
            }
            while (XRGeneralSettings.Instance.Manager == null)
            {
                yield return null;
            }

            var meshSubsystems = new List<XRMeshSubsystem>();
            SubsystemManager.GetInstances(meshSubsystems);
            if (meshSubsystems.Count == 1)
            {
                MeshSubsystem = meshSubsystems[0];
            }

#if UNITY_XR_MAGICLEAP_PROVIDER
            mlLoader = XRGeneralSettings.Instance.Manager.ActiveLoaderAs<MagicLeapLoader>();
#endif
#if UNITY_OPENXR_1_9_0_OR_NEWER
            mlOpenXrFeature = OpenXR.OpenXRSettings.Instance.GetFeature<MagicLeapMeshingFeature>();
#endif
        }

        public static void StartSubsystem()
        {
            subsysRefCount++;
            if (subsysRefCount == 1)
            {
#if UNITY_XR_MAGICLEAP_PROVIDER
                if (mlLoader != null)
                {
                    mlLoader.StartMeshSubsystem();
                }
#elif UNITY_OPENXR_1_9_0_OR_NEWER
                if (mlOpenXrFeature != null)
                {
                    mlOpenXrFeature.StartSubsystemForMLMeshing();
                }
#endif
            }
        }

        public static void StopSubsystem()
        {
            if (subsysRefCount == 0)
            {
                return;
            }

            subsysRefCount--;

            if (subsysRefCount == 0)
            {
#if UNITY_XR_MAGICLEAP_PROVIDER
                if (mlLoader != null)
                {
                    mlLoader.StopMeshSubsystem();
                }
#elif UNITY_OPENXR_1_9_0_OR_NEWER
                if (mlOpenXrFeature != null)
                {
                    mlOpenXrFeature.StopSubsystemForMLMeshing();
                }
#endif
            }
        }
    }
}
