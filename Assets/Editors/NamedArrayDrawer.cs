using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(NamedArrayAttribute))]
public class NamedArrayDrawer : PropertyDrawer
{
    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return EditorGUI.GetPropertyHeight(property, label, property.isExpanded);
    }

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        try
        {
            var config = attribute as NamedArrayAttribute;
            var names = config.names;

            int pos = int.Parse(property.propertyPath.Split('[', ']')[1]);
            var labelText = names.GetValue(pos) as string;
            // Make names nicer to read.
            labelText = labelText.Replace('_', ' ');
            label = new GUIContent(labelText);          
        }
        catch
        {
            // keep default label         
        }

        EditorGUI.PropertyField(position, property, label, property.isExpanded);
    }
}
