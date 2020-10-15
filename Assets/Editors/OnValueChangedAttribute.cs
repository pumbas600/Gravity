using System;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Field)]
public class OnValueChangedAttribute : Attribute
{
    public string key;
    public string propertyName;

    public OnValueChangedAttribute(string key)
    {
        this.key = key;
    }
}