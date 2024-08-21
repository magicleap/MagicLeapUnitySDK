// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) (2024) Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%
using UnityEditor;

public class SampleImportPostProcessor : AssetPostprocessor
{
    private const string samplesPath = "Assets/Samples/Magic Leap SDK";
    private const string inputsPath = "Packages/com.magicleap.unitysdk/Samples~/MLInputs.unitypackage";
    private string[] searchFolders = { "Assets" };

    private void OnPreprocessAsset()
    {
        ///This script should only apply to ML samples, so only do any logic here
        ///if it is an ML sample
        if (!assetPath.Contains(samplesPath))
            return;

        if (AssetDatabase.FindAssets("MagicLeapInput", searchFolders).Length != 0)
            return;

        AssetDatabase.importPackageFailed += OnImportFailed;
        AssetDatabase.ImportPackage(inputsPath, false);
    }
   
    private static void OnImportFailed(string packageName, string errorMessage)
    {
        UnityEngine.Debug.LogError("Failed to import MLInputs: " + errorMessage);
    }
}
