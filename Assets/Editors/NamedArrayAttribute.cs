using System;
using UnityEngine;

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public NamedArrayAttribute(string[] names)
    {
        this.names = names;
    }

    public NamedArrayAttribute(Type type)
    {
        names = Enum.GetNames(type);
    }
}
