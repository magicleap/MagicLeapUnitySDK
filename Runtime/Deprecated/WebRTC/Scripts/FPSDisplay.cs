// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2018-2022) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using UnityEngine;
using UnityEngine.UI;

public class FPSDisplay : MonoBehaviour
{
    public Text textMesh;
    private float accum = 0; // FPS accumulated over the interval
    private int frames = 0; // Frames drawn over the interval

    void Update()
    {
        accum += Time.timeScale / Time.deltaTime;
        ++frames;
        textMesh.text = (accum / frames).ToString();
    }
}
