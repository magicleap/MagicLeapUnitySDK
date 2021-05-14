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

using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
using UnityEngine;

namespace MagicLeap
{
    /// <summary>
    /// Warns the developers about the proper recommended color space setting
    /// at build time.
    /// </summary>
    public class ColorSpaceBuildPreprocessor : IPreprocessBuildWithReport
    {
        public int callbackOrder { get { return 0; } }

        public void OnPreprocessBuild(BuildReport report)
        {
            if (report.summary.platform == BuildTarget.Lumin && PlayerSettings.colorSpace != ColorSpace.Linear)
            {
                Debug.LogWarning("PlayerSettings.colorSpace is set to Gamma for Lumin which is not recommended for that platform." +
                    " Support for gamma color space will be deprecated and removed in the future." +
                    " Switch to the Linear color space for a better visual output on the device display & to keep this project future-proof.");
            }
        }
    }
}
