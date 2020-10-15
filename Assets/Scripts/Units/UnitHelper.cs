using System;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Reflections;
using System.Collections.Generic;

public static class UnitHelper
{
    public static Unit[] GetUnits()
    {
        List<Unit> units = new List<Unit>();

        var containers = Assembly.GetExecutingAssembly().GetTypes()
            .Where(t => t.HasAttribute<UnitContainerAttribute>());

        foreach (var container in containers)
        {
            //This is for the Getter / Setters
            units.AddRange(
                container.GetProperties(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                .Where(p => p.PropertyType.IsAssignableFrom(typeof(Unit)))
                .ToList()
                .ConvertAll(u => (Unit)u.GetValue(null)));

            //This is for the static variables.
            units.AddRange(
                container.GetFields(BindingFlags.Static | BindingFlags.Public | BindingFlags.FlattenHierarchy)
                .Where(f => f.FieldType.IsAssignableFrom(typeof(Unit)))
                .ToList()
                .ConvertAll(u => (Unit)u.GetValue(null)));
        }
        return units.ToArray();
    }

    public static Unit[] GetUnitsOf(UnitType unitType)
    {
        return GetUnits().Where(u => u.UnitType == unitType).ToArray();
    }

    public static float ConvertFloat(float value, Unit from, Unit to)
    {
        //NOTE: The two units powers should be the same (This should be confirmed in the
        //      IsValidUnit() method) so it doesn't matter which unit you get the power from.
        return value * Mathf.Pow(from.Multiplier / to.Multiplier, from.Power);
    }
}

public class UnitContainerAttribute : Attribute
{
}