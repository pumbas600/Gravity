using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Debug = UnityEngine.Debug;

[Serializable]
public abstract class MultiUnitObject<T>
{
    #region Config
    public static bool CheckForSameUnitWhenAdding = true;
    #endregion

    #region Variables

    private readonly Dictionary<UnitType, Unit> units = new Dictionary<UnitType, Unit>();
    public Dictionary<UnitType, Unit> Units { 
        get {
            if (!overrideCheckUnits) CheckUnits();
            return units;
        }
    }
    //Type names is here as the units dictionary cannot be updated and saved in the drawer, so this is instead.
    //Also using a list rather than a hashset, as hashsets cannot be serialized (Updated in drawer).
    public List<string> symbols = new List<string>();
    public T value;
    private readonly bool overrideCheckUnits;

    #endregion

    public MultiUnitObject(T value, params Unit[] units)
    {
        UpdateUnits(units);
        this.value = value;
    }

    public MultiUnitObject(T value, bool overrideCheckUnits = false, params Unit[] units) : this(value, units)
    {
        this.overrideCheckUnits = overrideCheckUnits;
    }

    //Making a deep copy of the units dictionary, by making deep copies of each unit in it.
    public MultiUnitObject(MultiUnitObject<T> other)
    {
        overrideCheckUnits = true;
        value = other.value;
        foreach (var pair in other.Units)
        {
            Units.Add(pair.Key, pair.Value.DeepCopy());
        }
    }

    public Unit[] GetUnits()
    {
        return Units.Values.ToArray();
    }

    public virtual T GetAs(params Unit[] units)
    {
        T result = value;
        foreach (var unit in units)
        {
            if (IsValidUnit(unit))
            {
                result = ConvertValue(result, unit);
            }
        }
        return result;
    }

    public virtual MultiUnitObject<T> GetAsNew(params Unit[] units)
    {
        var newUnitType = DeepCopy();
        newUnitType.ChangeTo(units);
        return newUnitType;
    }

    public virtual void ChangeTo(params Unit[] units)
    {
        foreach (var unit in units)
        {
            if (!IsValidUnit(unit)) continue;

            value = ConvertValue(value, unit);

            var newUnit = unit.DeepCopy();
            newUnit.Power = Units[unit.UnitType].Power;
            Units[unit.UnitType] = newUnit;
        }
    }
    
    /// <summary>
    /// Updates the units. For example, velocity (ms-1) when multiplied with a time (s) 
    /// simply becomes distance (m). 
    /// NOTE: This will not work correctly if trying to update units with their subunits (e.g km * m)
    /// so make sure to have converted their units to be the same when multiplying and updating.
    /// </summary>
    /// <param name="units">The units to update</param>
    public virtual Unit[] UpdateUnits(params Unit[] units)
    {
        foreach (var unit in units)
        {
            if (Units.ContainsKey(unit.UnitType))
            {
                //TODO: Factor in if they're not of the same subunit
                //E.g: some wierd situation like km/m

                unit.Power += Units[unit.UnitType].Power;
                //If the units cancel
                if (unit.Power == 0)
                {
                    Units.Remove(unit.UnitType);
                }
                else
                {
                    Units[unit.UnitType] = unit;
                }

            }
            else if (unit.Power != 0)
            {
                Units.Add(unit.UnitType, unit);
            }
        }
        return GetUnits();
    }

    //Multiplies the power of each unit by -1.
    public virtual Unit[] InvertUnits()
    {
        foreach (var pair in Units)
        {
            Units[pair.Key].Power *= -1;
        }
        return GetUnits();
    }

    public virtual bool IsValidUnit(Unit unit)
    {
        return Units.ContainsKey(unit.UnitType)
            && Units[unit.UnitType].UnitSymbol != unit.UnitSymbol;
    }

    //Over 500 calls, averages a time of 1-1.4ms.
    //This was much faster than sorting the units based on their power.
    public virtual string GetUnitString()
    {
        StringBuilder numeratorUnits = new StringBuilder();
        StringBuilder denominatorUnits = new StringBuilder();
        var units = GetUnits();

        StringBuilder builder;
        for (int i = 0; i < units.Length; i++) 
        {
            builder = (units[i].Power > 0) ? numeratorUnits : denominatorUnits;

            builder.Append(units[i].ToString());
            if (units[i].Power != 1) builder.Append(" ");
        }
        numeratorUnits.Append(denominatorUnits);
        return numeratorUnits.ToString();
    }

    public override string ToString()
    {
        return value + " " + GetUnitString();
    }

    private void CheckUnits()
    {
        //0ms for 500 -> This is because most of the time there will be no change and thus
        //no need to actually create a new unit object, which is an expensive operation.
        foreach (var symbol in symbols)
        {
            bool isContained = false;
            foreach (var unit in units.Values)
            {
                if (unit.UnitSymbol == symbol)
                {
                    isContained = true;
                    break;
                }
            }
            if (!isContained)
            {
                var unit = UnitHelper.GetUnits().First(u => u.UnitSymbol == symbol).DeepCopy();
                unit.Power = units[unit.UnitType].Power;
                units[unit.UnitType] = unit;
            }
        }        
    }

    protected abstract T ConvertValue(T value, Unit unit);
    protected abstract MultiUnitObject<T> DeepCopy();

    #region Operators

    public static MultiUnitObject<T> AddUnitObjs(MultiUnitObject<T> left, MultiUnitObject<T> right,
        Func<MultiUnitObject<T>, MultiUnitObject<T>, T> adder)
    {
        var leftCopy = left.DeepCopy();
        var rightCopy = right.GetAsNew(leftCopy.GetUnits());

        if (CheckForSameUnitWhenAdding && left.GetUnitString() != rightCopy.GetUnitString())
        {
            Debug.LogWarning($"Cannot add {left} and {right} as their units aren't the same.");
            return left;
        }

        leftCopy.value = adder(leftCopy, rightCopy);
        return leftCopy;
    }

    public static MultiUnitObject<T> Add<K>(MultiUnitObject<T> left, K right,
        Func<MultiUnitObject<T>, K, T> adder)
    {
        var unitCopy = left.DeepCopy();
        unitCopy.value = adder(unitCopy, right);
        return unitCopy;
    }

    public static MultiUnitObject<T> SubUnitObjs(MultiUnitObject<T> left, MultiUnitObject<T> right,
        Func<MultiUnitObject<T>, MultiUnitObject<T>, T> subtracter)
    {
        var leftCopy = left.DeepCopy();
        var rightCopy = right.GetAsNew(leftCopy.GetUnits());

        if (CheckForSameUnitWhenAdding && left.GetUnitString() != rightCopy.GetUnitString())
        {
            Debug.LogWarning($"Cannot subtract {right} from {left} as their units aren't the same.");
            return left;
        }

        leftCopy.value = subtracter(leftCopy, rightCopy);
        return leftCopy;
    }

    public static MultiUnitObject<T> Sub<K>(MultiUnitObject<T> left, K right,
        Func<MultiUnitObject<T>, K, T> subtracter)
    {
        var unitCopy = left.DeepCopy();
        unitCopy.value = subtracter(unitCopy, right);
        return unitCopy;
    }

    public static MultiUnitObject<T> Sub<K>(K left, MultiUnitObject<T> right,
        Func<K, MultiUnitObject<T>, T> subtracter)
    {
        var unitCopy = right.DeepCopy();
        unitCopy.value = subtracter(left, unitCopy);
        return unitCopy;
    }

    public static MultiUnitObject<T> MultUnitObjs<K>(MultiUnitObject<T> left, MultiUnitObject<K> right,
        Func<MultiUnitObject<T>, MultiUnitObject<K>, T> multiplier)
    {
        var leftCopy = left.DeepCopy();
        var rightCopy = right.GetAsNew(leftCopy.GetUnits());
        leftCopy.value = multiplier(leftCopy, rightCopy);
        leftCopy.UpdateUnits(rightCopy.GetUnits());
        return leftCopy;
    }

    public static MultiUnitObject<T> Mult<K>(MultiUnitObject<T> left, K right,
        Func<MultiUnitObject<T>, K, T> multiplier)
    {
        var unitCopy = left.DeepCopy();
        unitCopy.value = multiplier(unitCopy, right);
        return unitCopy;
    }

    public static MultiUnitObject<T> DivUnitObjs<K>(MultiUnitObject<T> left, MultiUnitObject<K> right,
        Func<MultiUnitObject<T>, MultiUnitObject<K>, T> divider)
    {
        var leftCopy = left.DeepCopy();
        var rightCopy = right.GetAsNew(leftCopy.GetUnits());
        leftCopy.value = divider(leftCopy, rightCopy);
        leftCopy.UpdateUnits(rightCopy.InvertUnits());
        return leftCopy;
    }

    public static MultiUnitObject<T> Div<K>(MultiUnitObject<T> left, K right,
        Func<MultiUnitObject<T>, K, T> divider)
    {
        var unitCopy = left.DeepCopy();
        unitCopy.value = divider(unitCopy, right);
        return unitCopy;
    }

    public static MultiUnitObject<T> Div<K>(K left, MultiUnitObject<T> right,
        Func<K, MultiUnitObject<T>, T> divider)
    {
        var unitCopy = right.DeepCopy();
        unitCopy.value = divider(left, unitCopy);
        unitCopy.InvertUnits();
        return unitCopy;
    }

    public static bool ComparisonUnitObjs(MultiUnitObject<T> left, MultiUnitObject<T> right,
        Func<T, T, bool> compariter)
    {
        return compariter(left.value, right.GetAs(left.GetUnits()));
    }

    public static bool Comparison<K>(MultiUnitObject<T> left, K right,
        Func<T, K, bool> compariter)
    {
        return compariter(left.value, right);
    }
    public static bool Comparison<K>(K left, MultiUnitObject<T> right,
        Func<K, T, bool> compariter)
    {
        return compariter(left, right.value);
    }

    #endregion
}
