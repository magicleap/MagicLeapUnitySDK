using System.Linq;
using UnityEditor.XR.Management;
using UnityEngine;
using UnityEngine.XR.OpenXR;
using MagicLeapCamera = UnityEngine.XR.MagicLeap.MagicLeapCamera;

namespace UnityEditor.XR.MagicLeap
{
#pragma warning disable CS0618 // Type or member is obsolete
    [CustomEditor(typeof(MagicLeapCamera))]
#pragma warning restore CS0618 // Type or member is obsolete
    class MagicLeapCameraEditor : Editor
    {
        private static GUIContent stereoConvergencePointText = new GUIContent("Stereo Convergence Point", "Transform you want to be the focus point of the camera");
        private static GUIContent protectedSurfaceText = new GUIContent("Protected Surface", "Content for this app is protected and should not be recorded or captured");
        private static GUIContent fixIssuesText = new GUIContent("Fix Problems On Startup", "Should the Camera settings be automatically adjusted to preferred Magic Leap values on scene startup?");
        private static GUIContent nearClipText = new GUIContent("Enforce Near Clip Distance", "Enforce Camera Near Clip Plane validation.");
        private static GUIContent farClipText = new GUIContent("Enforce Far Clip Distance", "Enforce Camera Far Clip Plane validation");
        private static GUIContent recenterText = new GUIContent("Recenter XR Origin At Start", "Determine if the app should recenter the XR Origin object so that the Main Camera is at the scene's origin on start");

        private SerializedProperty stereoConvergencePointProp;
        private SerializedProperty protectedSurfaceProp;
        private SerializedProperty fixeIssuesProp;
        private SerializedProperty enforceNearClipProp;
        private SerializedProperty enforceFarClipProp;
        private SerializedProperty recenterXROriginProp;

        void OnEnable()
        {
            stereoConvergencePointProp = serializedObject.FindProperty("stereoConvergencePoint");
            protectedSurfaceProp = serializedObject.FindProperty("protectedSurface");
            enforceNearClipProp = serializedObject.FindProperty("enforceNearClip");
            enforceFarClipProp = serializedObject.FindProperty("enforceFarClip");
            fixeIssuesProp = serializedObject.FindProperty("fixProblemsOnStartup");
            recenterXROriginProp = serializedObject.FindProperty("recenterXROriginAtStart");
        }

        public override void OnInspectorGUI()
        {
            using (new EditorGUI.DisabledScope(true))
                EditorGUILayout.ObjectField("Script", MonoScript.FromMonoBehaviour((MonoBehaviour)target), GetType(), false);

            serializedObject.Update();

            EditorGUILayout.PropertyField(fixeIssuesProp, fixIssuesText);
            //EditorGUILayout.PropertyField(enforceNearClipProp, nearClipText);
            EditorGUILayout.PropertyField(enforceFarClipProp, farClipText);

            EditorGUILayout.ObjectField(stereoConvergencePointProp, typeof(Transform), stereoConvergencePointText);

            EditorGUILayout.PropertyField(protectedSurfaceProp, protectedSurfaceText);
            EditorGUILayout.PropertyField(recenterXROriginProp, recenterText);

            serializedObject.ApplyModifiedProperties();
        }
    }
}
