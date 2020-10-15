using UnityEditor;
using UnityEngine;

public class EditorOnlyAttribute : PropertyAttribute
{
}

[CustomPropertyDrawer(typeof(EditorOnlyAttribute))]
public class EditorOnlyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        bool previouslyEnabled = GUI.enabled;
        GUI.enabled = !Application.isPlaying;
        EditorGUI.PropertyField(position, property, label, true);
        GUI.enabled = previouslyEnabled;
    }
}

