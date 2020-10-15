using UnityEngine;

[System.Serializable]
public class UnitVector3 : MultiUnitObject<Vector3>
{
    public UnitVector3(MultiUnitObject<Vector3> other) : base(other)
    {
    }

    public UnitVector3(Vector3 value, params Unit[] units) : base(value, units)
    {
    }

    public UnitVector3(Vector3 value, bool overrideCheckUnits = false, params Unit[] units) : base(value, overrideCheckUnits, units)
    {
    }

    protected override Vector3 ConvertValue(Vector3 value, Unit unit)
    {
        var currentUnit = Units[unit.UnitType];
        return new Vector3(
            UnitHelper.ConvertFloat(value.x, currentUnit, unit),
            UnitHelper.ConvertFloat(value.y, currentUnit, unit),
            UnitHelper.ConvertFloat(value.z, currentUnit, unit));
    }

    protected override MultiUnitObject<Vector3> DeepCopy()
    {
        return new UnitVector3(this);
    }

    public new UnitVector3 GetAsNew(params Unit[] units)
    {
        return (UnitVector3)base.GetAsNew(units);
    }

    public override bool Equals(object obj)
    {
        return obj is UnitVector3 unitVector3 && unitVector3 == this;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #region Operators

    public static UnitVector3 operator +(UnitVector3 left, UnitVector3 right) => (UnitVector3)AddUnitObjs(left, right, (l, r) => l.value + r.value);
    public static UnitVector3 operator +(UnitVector3 left, Vector3 right)     => (UnitVector3)Add(left, right, (l, r) => l.value + r);
    public static UnitVector3 operator +(Vector3 left, UnitVector3 right)     => (UnitVector3)Add(right, left, (l, r) => l.value + r);
    public static UnitVector3 operator -(UnitVector3 left, UnitVector3 right) => (UnitVector3)SubUnitObjs(left, right, (l, r) => l.value - r.value);
    public static UnitVector3 operator -(UnitVector3 left, Vector3 right)     => (UnitVector3)Sub(left, right, (l, r) => l.value - r);
    public static UnitVector3 operator -(Vector3 left, UnitVector3 right)     => (UnitVector3)Sub(left, right, (l, r) => l - right.value);
    public static UnitVector3 operator *(UnitVector3 left, UnitFloat right)   => (UnitVector3)MultUnitObjs(left, right, (l, r) => l.value * r.value);
    public static UnitVector3 operator *(UnitFloat left, UnitVector3 right)   => (UnitVector3)MultUnitObjs(right, left, (l, r) => l.value * r.value);
    public static UnitVector3 operator *(UnitVector3 left, float right)       => (UnitVector3)Mult(left, right, (l, r) => l.value * r);
    public static UnitVector3 operator *(float left, UnitVector3 right)       => (UnitVector3)Mult(right, left, (l, r) => l.value * r);
    public static UnitVector3 operator /(UnitVector3 left, UnitFloat right)   => (UnitVector3)DivUnitObjs(left, right, (l, r) => l.value / r.value);
    public static UnitVector3 operator /(UnitVector3 left, float right)       => (UnitVector3)Div(left, right, (l, r) => l.value / r);

    //Comparison operators

    public static bool operator ==(UnitVector3 left, UnitVector3 right) => ComparisonUnitObjs(left, right, (l, r) => l == r);
    public static bool operator ==(UnitVector3 left, Vector3 right) => Comparison(left, right, (l, r) => l == r);
    public static bool operator ==(Vector3 left, UnitVector3 right) => Comparison(left, right, (l, r) => l == r);
    public static bool operator !=(UnitVector3 left, UnitVector3 right) => ComparisonUnitObjs(left, right, (l, r) => l != r);
    public static bool operator !=(UnitVector3 left, Vector3 right) => Comparison(left, right, (l, r) => l != r);
    public static bool operator !=(Vector3 left, UnitVector3 right) => Comparison(left, right, (l, r) => l != r);

    #endregion
}

[UnityEditor.CustomPropertyDrawer(typeof(UnitVector3))]
public class UnitVector3Drawer : MultiUnitDrawer<Vector3> { }
