// %BANNER_BEGIN%
// ---------------------------------------------------------------------
// %COPYRIGHT_BEGIN%
// Copyright (c) 2022 Magic Leap, Inc. All Rights Reserved.
// Use of this file is governed by the Software License Agreement, located here: https://www.magicleap.com/software-license-agreement-ml2
// Terms and conditions applicable to third-party materials accompanying this distribution may also be found in the top-level NOTICE file appearing herein.
// %COPYRIGHT_END%
// ---------------------------------------------------------------------
// %BANNER_END%

using System.Linq;
using UnityEditor;

#if UNITY_EDITOR
namespace UnityEngine.XR.MagicLeap
{
    [CustomPropertyDrawer(typeof(OnChangedCallAttribute))]
    public class OnChangedCallPropertyDrawer : PropertyDrawer
    {
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            EditorGUI.BeginChangeCheck();
            EditorGUI.PropertyField(position, property, label);
            if (!EditorGUI.EndChangeCheck()) return;

            var targetObject = property.serializedObject.targetObject;

            var callAttribute = attribute as OnChangedCallAttribute;
            var methodName = callAttribute?.methodName;

            var classType = targetObject.GetType();
            var methodInfo = classType.GetMethods().FirstOrDefault(info => info.Name == methodName);

            // Update the serialized field
            property.serializedObject.ApplyModifiedProperties();

            // If we found a public function with the given name that takes no parameters, invoke it
            if (methodInfo != null && !methodInfo.GetParameters().Any())
            {
                methodInfo.Invoke(targetObject, null);
            }
            else
            {
                throw new OnChangedCallException($"OnChangedCall error : No public function taking no " +
                    $"argument named {methodName} in class {classType.Name}");
            }
        }
    }
}
#endif
