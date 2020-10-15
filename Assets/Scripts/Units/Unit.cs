[UnitContainer]
public class Unit
{
    public static readonly Unit Centimetre  = new Unit("cm", 1e-2F, UnitType.LengthUnit);
    public static readonly Unit Metre       = new Unit("m", 1F, UnitType.LengthUnit);
    public static readonly Unit Kilometre   = new Unit("km", 1e3F, UnitType.LengthUnit);
    public static readonly Unit Megametre   = new Unit("Mm", 1e6F, UnitType.LengthUnit);
    public static readonly Unit Gigametre   = new Unit("Gm", 1e9F, UnitType.LengthUnit);

    public static readonly Unit Gram        = new Unit("g", 1F, UnitType.MassUnit);
    public static readonly Unit Kilogram    = new Unit("kg", 1e3F, UnitType.MassUnit);
    public static readonly Unit Tonne       = new Unit("t", 1e6F, UnitType.MassUnit);

    public static readonly Unit Second      = new Unit("s", 1F, UnitType.DurationUnit);
    public static readonly Unit Hour        = new Unit("hr", 3600F, UnitType.DurationUnit);

    public virtual float Multiplier { get; }
    public virtual string UnitSymbol { get; }
    public virtual UnitType UnitType { get; }
    public float Power { get; set; }

    public Unit(float power = 1F)
    {
        Power = power;
    }

    public Unit(string unitSymbol, float multiplier, UnitType unitType, float power = 1)
    {
        UnitSymbol = unitSymbol;
        Multiplier = multiplier;
        UnitType = unitType;
        Power = power;
    }

    public override string ToString()
    {
        return UnitSymbol + ((Power == 1) ? "" : Power.ToString());
    }

    public virtual Unit DeepCopy()
    {
        return new Unit(UnitSymbol, Multiplier, UnitType, Power);
    }

    public static Unit operator ^(Unit unit, float power) => new Unit(unit.UnitSymbol, unit.Multiplier, unit.UnitType, power);
}

public enum UnitType
{
    LengthUnit, MassUnit, DurationUnit
}