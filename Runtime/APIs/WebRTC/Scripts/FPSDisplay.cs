// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by your Early Access Terms and Conditions.
// This software is an Early Access Product.
//
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
