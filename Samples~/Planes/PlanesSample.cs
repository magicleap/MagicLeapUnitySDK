// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using MagicLeap.Android;
using System.Collections;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using MagicLeap.OpenXR.Features.Planes;
using MagicLeap.OpenXR.Subsystems;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit.Inputs;

public class PlanesSample : MonoBehaviour
{
    private ARPlaneManager planeManager;

    private MagicLeapPlanesFeature planeFeature;

    [SerializeField, Tooltip("Maximum number of planes to return each query")]
    private uint maxResults = 100;

    [SerializeField, Tooltip("Minimum plane area to treat as a valid plane")]
    private float minPlaneArea = 0.25f;

    [SerializeField]
    private Text status;

    private Camera mainCamera;
    private bool permissionGranted;

    private InputActionAsset inputActions;
    private InputActionMap inputMap;

    private IEnumerator Start()
    {
        mainCamera = Camera.main;
        yield return new WaitUntil(AreSubsystemsLoaded);
        planeManager = FindObjectOfType<ARPlaneManager>();
        if (planeManager == null)
        {
            Debug.LogError("Failed to find ARPlaneManager in scene. Disabling Script");
            enabled = false;
        }
        else
        {
            // disable planeManager until we have successfully requested required permissions
            planeManager.enabled = false;
        }

        //Assign bumper press to perform ToggleScanning
        inputActions = FindObjectOfType<InputActionManager>().actionAssets[0];
        if(inputActions == null)
            throw new System.NullReferenceException("Could not find an InputActionAsset. Make sure that the MagicLeapInput input actions is present (try reimporting this sample)");

        inputMap = inputActions.FindActionMap("Controller");
        inputMap.FindAction("Bumper").performed += ToggleScanning;

        permissionGranted = false;
        Permissions.RequestPermission(Permissions.SpatialMapping, OnPermissionGranted, OnPermissionDenied);
    }
    public bool AreSubsystemsLoaded()
    {
        if (XRGeneralSettings.Instance == null) return false;
        if (XRGeneralSettings.Instance.Manager == null) return false;
        var activeLoader = XRGeneralSettings.Instance.Manager.activeLoader;
        if (activeLoader == null) return false;
        return activeLoader.GetLoadedSubsystem<XRPlaneSubsystem>() != null;
    }

    private IEnumerator StopPlanesScanning()
    {
        if (planeFeature != null && planeFeature.enabled)
        {
            planeFeature.InvalidateCurrentPlanes();
        }
        // Skip a frame for the changes to take effect and prefabs get removed.
        yield return new WaitForEndOfFrame();
        planeManager.enabled = false;
    }

    private void Update()
    {
        UpdateQuery();
    }

    private void UpdateQuery()
    {
        if (planeManager != null && planeManager.enabled && permissionGranted)
        {
            var newQuery = new MLXrPlaneSubsystem.PlanesQuery
            {
                Flags = planeManager.requestedDetectionMode.ToMLXrQueryFlags() | MLXrPlaneSubsystem.MLPlanesQueryFlags.SemanticAll,
                BoundsCenter = mainCamera.transform.position,
                BoundsRotation = mainCamera.transform.rotation,
                BoundsExtents = Vector3.one * 20f,
                MaxResults = maxResults,
                MinPlaneArea = minPlaneArea
            };

            MLXrPlaneSubsystem.Query = newQuery;
            status.text = $"Detection Mode:\n<B>{planeManager.requestedDetectionMode}</B>\n\n" +
                          $"Query Flags:\n<B>{newQuery.Flags.ToString().Replace(" ", "\n")}</B>\n\n" +
                          $"Query MaxResults:\n<B>{newQuery.MaxResults}</B>\n\n" +
                          $"Query MinPlaneArea:\n<B>{newQuery.MinPlaneArea}</B>\n\n" +
                          $"Plane GameObjects:\n<B>{planeManager.trackables.count}</B>";
        }
    }

    private void OnPermissionGranted(string permission)
    {
        planeManager.enabled = true;
        permissionGranted = true;
        planeFeature = OpenXRSettings.Instance.GetFeature<MagicLeapPlanesFeature>();
    }

    private void OnPermissionDenied(string permission)
    {
        Debug.LogError($"Failed to create Planes Subsystem due to missing or denied {Permissions.SpatialMapping} permission. Please add to manifest. Disabling script.");
        enabled = false;
    }

    private void OnDisable()
    {
        inputMap.FindAction("Bumper").performed -= ToggleScanning;
    }

    public void ToggleScanning(InputAction.CallbackContext obj)
    {
        if (!permissionGranted)
            return;

        if (planeManager.enabled)
            StartCoroutine(StopPlanesScanning());
        else
            planeManager.enabled = true;
    }
}
