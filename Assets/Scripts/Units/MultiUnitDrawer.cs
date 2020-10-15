using System;
using UnityEditor;
using UnityEngine;

public abstract class MultiUnitDrawer<T> : PropertyDrawer
{
    private MultiUnitObject<T> _UnitObject;
    private const float kUnitWidth = 40F;

    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        Initialise(property);

        var unitsWidth = kUnitWidth * _UnitObject.GetUnits().Length;

        var propertyRect = new Rect(position.x, position.y, position.width - unitsWidth, position.height);
        var valueProperty = property.FindPropertyRelative("value");
        var symbolsProperty = property.FindPropertyRelative("symbols");

        EditorGUI.PropertyField(propertyRect, valueProperty, label, true);

        var unitRect = new Rect(position.x + propertyRect.width, position.y, kUnitWidth, position.height);

        foreach (var unit in _UnitObject.GetUnits())
        {
            var unitString = unit.ToString();
            if (GUI.Button(unitRect, new GUIContent(unitString)))
            {
                GenericMenu menu = new GenericMenu();

                foreach (var subUnit in UnitHelper.GetUnitsOf(unit.UnitType))
                {
                    AddMenuItem(menu, subUnit, unit);
                    menu.ShowAsContext();
                }
            }
            unitRect.x += kUnitWidth;
        }

        valueProperty.serializedObject.ApplyModifiedProperties();
        symbolsProperty.serializedObject.ApplyModifiedProperties();
    }

    //A method to simplify adding menu items
    private void AddMenuItem(GenericMenu menu, Unit newUnit, Unit currentUnit)
    {
        menu.AddItem(
            new GUIContent(newUnit.ToString()),                  
            newUnit == currentUnit,                     //Is this the selected menuItem
            u => OnUnitSelected((Unit)u, currentUnit),  //Callback when clicked
            newUnit);                                   //object passed to callback
    }

    private void OnUnitSelected(Unit unit, Unit currentUnit)
    {
        var symbol = unit.UnitSymbol;
        if (!_UnitObject.symbols.Contains(symbol))
        {
            var newUnit = unit ^ currentUnit.Power;
            _UnitObject.symbols.Remove(currentUnit.UnitSymbol);

            _UnitObject.ChangeTo(newUnit);
            _UnitObject.symbols.Add(symbol);
        }
    }

    private void Initialise(SerializedProperty property)
    {
        if (_UnitObject == null)
        {
            var target = property.serializedObject.targetObject;
            _UnitObject = fieldInfo.GetValue(target) as MultiUnitObject<T>;
        }
    }
}
