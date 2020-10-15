using UnityEngine;
using UnityEditor;
using System.Reflection;
using System.Collections.Generic;
using System;
using System.Linq;

[CanEditMultipleObjects]
[CustomEditor(typeof(ScriptableObject), true)]
public class ScriptableObjectInspectorButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        
        if (Application.isEditor)
        {
            var type = target.GetType();

            foreach (var method in type.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                var attribute = method.GetCustomAttribute(typeof(InspectorButtonAttribute));

                if (attribute != null)
                {
                    if (GUILayout.Button("Run: " + method.Name))
                    {
                        //If the user clicks the button, call the method
                        method.Invoke(target, new object[] { });
                    }
                }
            }
        }
    }
}

[CanEditMultipleObjects]
[CustomEditor(typeof(MonoBehaviour), true)]
public class MonobehaviourInspectorButtonEditor : Editor
{
    public override void OnInspectorGUI()
    {
        Dictionary<OnValueChangedAttribute, string> properties = new Dictionary<OnValueChangedAttribute, string>();
        if (Application.isEditor)
        {
            var type = target.GetType();

            foreach (var property in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                OnValueChangedAttribute attribute = (OnValueChangedAttribute)property.GetCustomAttribute(typeof(OnValueChangedAttribute));

                if (attribute != null)
                {
                    if (!properties.ContainsKey(attribute))
                    {
                        properties.Add(attribute, property.Name);
                    }
                }
            }

            DrawPropertiesExcluding(serializedObject, properties.ToList().ConvertAll(p => p.Value).ToArray());

            foreach (var pair in properties.ToList())
            {
                SerializedProperty serializedProperty = serializedObject.FindProperty(pair.Value);
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(serializedProperty);
                if (EditorGUI.EndChangeCheck())
                {
                    serializedObject.ApplyModifiedProperties();
                    CallMethodWithKey(type, pair.Key.key, target);
                }
            }

            foreach (var method in type.GetMethods(
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static))
            {
                InspectorButtonAttribute attribute = (InspectorButtonAttribute)method.GetCustomAttribute(typeof(InspectorButtonAttribute));

                if (attribute != null)
                {
                    if (GUILayout.Button("Run: " + method.Name))
                    {
                        //If the user clicks the button, call the method
                        method.Invoke(target, new object[] { });
                    }
                }
            }
            serializedObject.ApplyModifiedProperties();
        }
    }

    private void CallMethodWithKey(Type type, string key, object obj)
    {
        foreach (var method in type.GetMethods(
            BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Static))
        {
            OnValueChangedAttribute attribute = (OnValueChangedAttribute)method.GetCustomAttribute(typeof(OnValueChangedAttribute));
            if (attribute != null && attribute.key == key)
            {
                method.Invoke(obj, new object[] { });
            }
        }
    }
}
