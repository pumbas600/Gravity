using UnityEngine;
using Debug = UnityEngine.Debug;

[System.Serializable]
public class UnitFloat : MultiUnitObject<float>
{
    public UnitFloat(MultiUnitObject<float> other) : base(other)
    {
    }

    public UnitFloat(float value, bool overrideCheckUnits = false, params Unit[] units) : base(value, overrideCheckUnits, units)
    {
    }

    public UnitFloat(float value, params Unit[] units) : base(value, units)
    {
    }

    protected override MultiUnitObject<float> DeepCopy()
    {
        return new UnitFloat(this);
    }

    protected override float ConvertValue(float value, Unit unit)
    {
        return UnitHelper.ConvertFloat(value, Units[unit.UnitType], unit);
    }

    public new UnitFloat GetAsNew(params Unit[] units)
    {
        return (UnitFloat)base.GetAsNew(units);
    }

    public string ToString(string format)
    {
        return value.ToString(format) + " " + GetUnitString();
    }

    public override bool Equals(object obj)
    {
        return obj is UnitFloat unitFloat && unitFloat == this;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    #region Operators

    public static UnitFloat operator *(UnitFloat left, UnitFloat right) => (UnitFloat)MultUnitObjs(left, right, (l, r) => l.value * r.value);
    public static UnitFloat operator *(UnitFloat left, float right) => (UnitFloat)Mult(left, right, (l, r) => l.value * r);
    public static UnitFloat operator *(float left, UnitFloat right) => (UnitFloat)Mult(right, left, (l, r) => l.value * r);
    public static UnitVector3 operator *(UnitFloat left, Vector3 right) => new UnitVector3(left.value * right, left.GetUnits());
    public static UnitVector3 operator *(Vector3 left, UnitFloat right) => new UnitVector3(left * right.value, right.GetUnits());
    public static UnitFloat operator /(UnitFloat left, UnitFloat right) => (UnitFloat)DivUnitObjs(left, right, (l, r) => l.value / r.value);
    public static UnitFloat operator /(UnitFloat left, float right) => (UnitFloat)Div(left, right, (l, r) => l.value / r);
    public static UnitFloat operator /(float left, UnitFloat right) => (UnitFloat)Div(left, right, (l, r) => l / r.value);
    public static UnitFloat operator +(UnitFloat left, UnitFloat right) => (UnitFloat)AddUnitObjs(left, right, (l, r) => l.value + r.value);
    public static UnitFloat operator +(UnitFloat left, float right) => (UnitFloat)Add(left, right, (l, r) => l.value + r);
    public static UnitFloat operator +(float left, UnitFloat right) => (UnitFloat)Add(right, left, (l, r) => l.value + r);
    public static UnitFloat operator -(UnitFloat left, UnitFloat right) => (UnitFloat)SubUnitObjs(left, right, (l, r) => l.value - r.value);
    public static UnitFloat operator -(UnitFloat left, float right) => (UnitFloat)Sub(left, right, (l, r) => l.value - r);
    public static UnitFloat operator -(float left, UnitFloat right) => (UnitFloat)Sub(left, right, (l, r) => l - r.value);
    
    //Comparison operators

    public static bool operator >(UnitFloat left, UnitFloat right) => ComparisonUnitObjs(left, right, (l, r) => l > r);
    public static bool operator >(UnitFloat left, float right) => Comparison(left, right, (l, r) => l > r);
    public static bool operator >(float left, UnitFloat right) => Comparison(left, right, (l, r) => l > r);
    public static bool operator <(UnitFloat left, UnitFloat right) => ComparisonUnitObjs(left, right, (l, r) => l < r);
    public static bool operator <(UnitFloat left, float right) => Comparison(left, right, (l, r) => l < r);
    public static bool operator <(float left, UnitFloat right) => Comparison(left, right, (l, r) => l < r);
    public static bool operator <=(UnitFloat left, UnitFloat right) => ComparisonUnitObjs(left, right, (l, r) => l <= r);
    public static bool operator <=(UnitFloat left, float right) => Comparison(left, right, (l, r) => l <= r);
    public static bool operator <=(float left, UnitFloat right) => Comparison(left, right, (l, r) => l <= r);
    public static bool operator >=(UnitFloat left, UnitFloat right) => ComparisonUnitObjs(left, right, (l, r) => l >= r);
    public static bool operator >=(UnitFloat left, float right) => Comparison(left, right, (l, r) => l >= r);
    public static bool operator >=(float left, UnitFloat right) => Comparison(left, right, (l, r) => l >= r);
    public static bool operator ==(UnitFloat left, UnitFloat right) => ComparisonUnitObjs(left, right, (l, r) => l == r);
    public static bool operator ==(UnitFloat left, float right) => Comparison(left, right, (l, r) => l == r);
    public static bool operator ==(float left, UnitFloat right) => Comparison(left, right, (l, r) => l == r);
    public static bool operator !=(UnitFloat left, UnitFloat right) => ComparisonUnitObjs(left, right, (l, r) => l != r);
    public static bool operator !=(UnitFloat left, float right) => Comparison(left, right, (l, r) => l != r);
    public static bool operator !=(float left, UnitFloat right) => Comparison(left, right, (l, r) => l != r);


    #endregion
}

[UnityEditor.CustomPropertyDrawer(typeof(UnitFloat))]
public class MultiUnitFloatDrawer : MultiUnitDrawer<float> { }