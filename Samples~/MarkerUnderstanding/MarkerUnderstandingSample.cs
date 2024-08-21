// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using MagicLeap.OpenXR.Features.MarkerUnderstanding;
using System.Linq;
using UnityEngine.UI;

public class MarkerUnderstandingSample : MonoBehaviour
{
    [SerializeField]
    private GameObject markerVisualPrefab;
    private HashSet<GameObject> detectorVisuals = new();
    private MagicLeapMarkerUnderstandingFeature markerFeature;
    private MarkerDetectorSettings markerDetectorSettings;
    private MarkerDetector currentDetector;
    private StringBuilder builder = new();

    private void Start()
    {
        markerFeature = OpenXRSettings.Instance.GetFeature<MagicLeapMarkerUnderstandingFeature>();
        detectorVisuals = new HashSet<GameObject>();
        CreateDetector();
    }

    private void CreateDetector()
    {
        //Create a detector based on a subset of settings. For fully customizable detector
        //creation, see the Marker Tracking scene in our Examples project. 
        markerDetectorSettings.MarkerDetectorProfile = MarkerDetectorProfile.Default;
        markerDetectorSettings.MarkerType = MarkerType.QR;
        markerDetectorSettings.QRSettings.QRLength = .115f;
        markerDetectorSettings.QRSettings.EstimateQRLength = true;
        markerFeature.CreateMarkerDetector(markerDetectorSettings);
    }

    private void ClearVisuals()
    {
        foreach (var visual in detectorVisuals)
            Destroy(visual.gameObject);
        detectorVisuals.Clear();
    }

    private void Update()
    {
        builder.Clear();
        markerFeature.UpdateMarkerDetectors();
        currentDetector = markerFeature.MarkerDetectors[0];
        int expectedVisualCount = currentDetector.Data.Where(d => d.MarkerPose != null).Count();

        if (detectorVisuals.Count > expectedVisualCount)
            ClearVisuals();

        for (int i = 0; i < currentDetector.Data.Count; i++)
        {
            if (currentDetector.Data[i].MarkerPose != null)
            {
                var markerVisual = Instantiate(markerVisualPrefab);
                detectorVisuals.Add(markerVisual);
                markerVisual.transform.position = currentDetector.Data[i].MarkerPose.Value.position;
                markerVisual.transform.rotation = currentDetector.Data[i].MarkerPose.Value.rotation;

                builder.AppendLine("Marker:" + currentDetector.Data[i].MarkerString);
                builder.AppendLine("Position: " + currentDetector.Data[i].MarkerPose?.position);
                builder.AppendLine("Rotation: " + currentDetector.Data[i].MarkerPose?.rotation);
                markerVisual.GetComponentInChildren<Text>().text = builder.ToString();
            }
        }
    }

    private void OnDestroy()
    {
        ClearVisuals();
        markerFeature.DestroyAllMarkerDetectors();
    }
}
