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
using UnityEngine;

namespace MagicLeap.Core
{
    /// <summary>
    /// This class extends the inspector for the MLMeshingBehavior component, providing visual runtime information.
    /// </summary>
    [CustomEditor(typeof(MLMeshingBehavior))]
    [CanEditMultipleObjects]
    public class MLMeshingBehaviorEditor : Editor
    {
        SerializedProperty meshParent;
        SerializedProperty meshPrefab;
        SerializedProperty triangleMeshMaterial;
        SerializedProperty pointCloudMeshMaterial;
        SerializedProperty pollingRate;
        SerializedProperty meshQueueSize;
        SerializedProperty meshBatchSize;
        SerializedProperty levelOfDetail;
        SerializedProperty useDefaultSettings;
        SerializedProperty meshType;
        SerializedProperty computeNormals;
        SerializedProperty requestVertexConfidence;
        SerializedProperty planarize;
        SerializedProperty removeMeshSkirt;
        SerializedProperty fillHoleLength;
        SerializedProperty disconnectedComponentArea;

        class Tooltips
        {
            public static readonly GUIContent MeshParent = new GUIContent(
                "Mesh Parent",
                "GameObject to use as parent of generated mesh objects.");

            public static readonly GUIContent MeshPrefab = new GUIContent(
                "Mesh Prefab",
                "Prefab to instantiate for each mesh instances. May have a mesh renderer and an optional mesh collider for physics.");

            public static readonly GUIContent TriangleMeshMaterial = new GUIContent(
                "Triangle Mesh Material",
                "Material to use for Triangle mesh type. Only required if MeshType is set to Triangles.");

            public static readonly GUIContent PointCloudMeshMaterial = new GUIContent(
                "PointCloud Mesh Material",
                "Material to use for PointCloud mesh type. Only required if MeshType is set to PointCloud.");

            public static readonly GUIContent PollingRate = new GUIContent(
                "Polling Rate",
                "How often to check for updates, in seconds. More frequent updates will increase CPU usage.");

            public static readonly GUIContent MeshQueueSize = new GUIContent(
                 "Mesh Queue Size",
                 "Controls the number of mesh requests ongoing at a time. Larger numbers will lead to higher CPU usage.");

            public static readonly GUIContent MeshBatchSize = new GUIContent(
                 "Mesh Batch Size",
                 "Controls the maximum number of blocks to query in a same mesh request. Larger numbers will lead to higher CPU usage.");

            public static readonly GUIContent LevelOfDetail = new GUIContent(
                 "Level Of Detail",
                 "Level of detail meshes will be requested and generated with.");

            public static readonly GUIContent UseDefaultSettings = new GUIContent(
                "Use Default Settings",
                "When enabled, meshing client will use the default system settings instead of the custom ones.");

            public static readonly GUIContent MeshType = new GUIContent(
                "Mesh Type",
                "Specifies if mesh will be represented as triangles or pointcloud.");

            public static readonly GUIContent ComputeNormals = new GUIContent(
                "Compute Normals",
                "When enabled, the system will compute the normals for the triangle vertices.");

            public static readonly GUIContent RequestVertexConfidence = new GUIContent(
                "Request Vertex Confidence",
                "When enabled, the system will generate confidence values for each vertex, ranging from 0-1.");

            public static readonly GUIContent Planarize = new GUIContent(
                "Planarize",
                "When enabled, the system will planarize the returned mesh (planar regions will be smoothed out).");

            public static readonly GUIContent RemoveMeshSkirt = new GUIContent(
                "Remove Mesh Skirt",
                "When enabled, the mesh skirt (overlapping area between two mesh blocks) will be removed.");

            public static readonly GUIContent FillHoleLength = new GUIContent(
                "Fill Hole Length",
                "Boundary distance (in meters) of holes you wish to have filled.");

            public static readonly GUIContent DisconnectedComponentArea = new GUIContent(
                "Disconnected Component Area",
                "Any component that is disconnected from the main mesh and which has an area less than this size will be removed.");
        }

        protected void OnEnable()
        {
            CacheSerializedProperties();
        }

        public override void OnInspectorGUI()
        {
            this.serializedObject.Update();

            LayoutGUI();

            this.serializedObject.ApplyModifiedProperties();
        }

        void CacheSerializedProperties()
        {
            meshParent = this.serializedObject.FindProperty("MeshParent");
            meshPrefab = this.serializedObject.FindProperty("MeshPrefab");
            triangleMeshMaterial = this.serializedObject.FindProperty("TriangleMeshMaterial");
            pointCloudMeshMaterial = this.serializedObject.FindProperty("PointCloudMeshMaterial");
            pollingRate = this.serializedObject.FindProperty("PollingRate");
            meshQueueSize = this.serializedObject.FindProperty("MeshQueueSize");
            meshBatchSize = this.serializedObject.FindProperty("MeshBatchSize");
            levelOfDetail = this.serializedObject.FindProperty("levelOfDetail");
            useDefaultSettings = this.serializedObject.FindProperty("UseDefaultSettings");
            meshType = this.serializedObject.FindProperty("meshType");
            computeNormals = this.serializedObject.FindProperty("computeNormals");
            requestVertexConfidence = this.serializedObject.FindProperty("requestVertexConfidence");
            planarize = this.serializedObject.FindProperty("planarize");
            removeMeshSkirt = this.serializedObject.FindProperty("removeMeshSkirt");
            fillHoleLength = this.serializedObject.FindProperty("fillHoleLength");
            disconnectedComponentArea = this.serializedObject.FindProperty("disconnectedComponentArea");
        }

        void LayoutGUI()
        {
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(meshParent, Tooltips.MeshParent);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(meshPrefab, Tooltips.MeshPrefab);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(triangleMeshMaterial, Tooltips.TriangleMeshMaterial);
            EditorGUILayout.PropertyField(pointCloudMeshMaterial, Tooltips.PointCloudMeshMaterial);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(pollingRate, Tooltips.PollingRate);
            EditorGUILayout.PropertyField(meshQueueSize, Tooltips.MeshQueueSize);
            EditorGUILayout.PropertyField(meshBatchSize, Tooltips.MeshBatchSize);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(levelOfDetail, Tooltips.LevelOfDetail);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(useDefaultSettings, Tooltips.UseDefaultSettings);

            EditorGUILayout.Space();

            if (useDefaultSettings.boolValue == false)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                {
                    EditorGUILayout.PropertyField(meshType, Tooltips.MeshType);
                    EditorGUILayout.PropertyField(computeNormals, Tooltips.ComputeNormals);
                    EditorGUILayout.PropertyField(requestVertexConfidence, Tooltips.RequestVertexConfidence);
                    EditorGUILayout.PropertyField(planarize, Tooltips.Planarize);
                    EditorGUILayout.PropertyField(removeMeshSkirt, Tooltips.RemoveMeshSkirt);
                    EditorGUILayout.PropertyField(fillHoleLength, Tooltips.FillHoleLength);
                    EditorGUILayout.PropertyField(disconnectedComponentArea, Tooltips.DisconnectedComponentArea);
                }
                EditorGUILayout.EndVertical();
            }
        }
    }
}
