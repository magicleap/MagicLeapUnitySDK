using UnityEditor.AnimatedValues;
using UnityEngine;
using UnityEngine.XR.MagicLeap;
using UnityEngine.XR.MagicLeap.Rendering;
using MagicLeapCamera = UnityEngine.XR.MagicLeap.MagicLeapCamera;

namespace UnityEditor.XR.MagicLeap
{
    [CustomEditor(typeof(MagicLeapCamera))]
    class MagicLeapCameraEditor : Editor
    {
        private static GUIContent stereoConvergencePointText = new GUIContent("Stereo Convergence Point", "Transform you want to be the focus point of the camera");
        private static GUIContent protectedSurfaceText = new GUIContent("Protected Surface", "Content for this app is protected and should not be recorded or captured");
        private static GUIContent fixIssuesText = new GUIContent("Fix Problems On Startup", "Should the Camera settings be automatically adjusted to preferred Magic Leap values on scene startup?");
        private static GUIContent nearClipText = new GUIContent("Enforce Near Clip Distance", "Enforce Camera Near Clip Plane validation.");
        private static GUIContent farClipText = new GUIContent("Enforce Far Clip Distance", "Enforce Camera Far Clip Plane validation");

        private SerializedProperty stereoConvergencePointProp;
        private SerializedProperty protectedSurfaceProp;
        private SerializedProperty fixeIssuesProp;
        private SerializedProperty enforceNearClipProp;
        private SerializedProperty enforceFarClipProp;

        void OnEnable()
        {
            stereoConvergencePointProp = serializedObject.FindProperty("stereoConvergencePoint");
            protectedSurfaceProp = serializedObject.FindProperty("protectedSurface");
            enforceNearClipProp = serializedObject.FindProperty("enforceNearClip");
            enforceFarClipProp = serializedObject.FindProperty("enforceFarClip");
            fixeIssuesProp = serializedObject.FindProperty("fixProblemsOnStartup");
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

            serializedObject.Update();

            EditorGUILayout.PropertyField(fixeIssuesProp, fixIssuesText);
            EditorGUILayout.PropertyField(enforceNearClipProp, nearClipText);
            EditorGUILayout.PropertyField(enforceFarClipProp, farClipText);

            EditorGUILayout.ObjectField(stereoConvergencePointProp, typeof(Transform), stereoConvergencePointText);

            EditorGUILayout.PropertyField(protectedSurfaceProp, protectedSurfaceText);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
