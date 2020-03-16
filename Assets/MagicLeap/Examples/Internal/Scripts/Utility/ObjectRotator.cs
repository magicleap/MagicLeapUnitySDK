// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
//
// Copyright (c) 2019-present, Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Developer Agreement, located
// here: https://auth.magicleap.com/terms/developer
//
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MagicLeap
{
    public class ObjectRotator : MonoBehaviour
    {
        [SerializeField]
        public float RotationSpeed;

        public void FixedUpdate()
        {
            this.transform.Rotate(0, RotationSpeed * Time.deltaTime, 0);
        }
    }
}