// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;

public class PlanePrefabSample : MonoBehaviour
{
    private const int GRAY_PLANE_QUEUE = 3001;
    private const int DEFAULT_PLANE_QUEUE = 3000;

    public static int Count { get; private set; } = 0;

    private void Start()
    {
        ColorClassify();
        Count++;
    }

    private void OnDestroy()
    {
        Count--;
    }

    private void ColorClassify()
    {
        var plane = GetComponent<ARPlane>();
        var color = plane.classification switch
        {
            PlaneClassification.Floor => Color.green,
            PlaneClassification.Ceiling => Color.blue,
            PlaneClassification.Wall => Color.red,
            PlaneClassification.Table => Color.yellow,
            _ => Color.gray
        };

        var mat = GetComponent<MeshRenderer>().material;
        mat.color = color;
        mat.renderQueue = color == Color.gray ? GRAY_PLANE_QUEUE : DEFAULT_PLANE_QUEUE;
    }
}
