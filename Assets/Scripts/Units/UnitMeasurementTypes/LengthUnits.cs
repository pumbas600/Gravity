public abstract class LengthUnit : Unit
{
    public LengthUnit(float power = 1F) : base(power)
    {
    }

    public override UnitType UnitType => UnitType.LengthUnit;
}

public class Millimetre : LengthUnit
{
    public Millimetre(float power = 1F) : base(power)
    {
    }

    public override float Multiplier => 1e-3F;
    public override string UnitSymbol => "mm";

    public override Unit DeepCopy()
    {
        return new Millimetre(Power);
    }
}

public class Centimetre : LengthUnit
{
    public Centimetre(float power = 1F) : base(power)
    {
    }

    public override float Multiplier => 1e-2F;
    public override string UnitSymbol => "cm";

    public override Unit DeepCopy()
    {
        return new Centimetre(Power);
    }
}

public class Metre : LengthUnit
{
    public Metre(float power = 1F) : base(power)
    {
    }

    public override float Multiplier => 1F;
    public override string UnitSymbol => "m";

    public override Unit DeepCopy()
    {
        return new Metre(Power);
    }
}

public class Kilometre : LengthUnit
{
    public Kilometre(float power = 1F) : base(power)
    {
    }

    public override float Multiplier => 1e3F;
    public override string UnitSymbol => "km";

    public override Unit DeepCopy()
    {
        return new Kilometre(Power);
    }
}

public class Megametre : LengthUnit
{
    public Megametre(float power = 1F) : base(power)
    {
    }

    public override float Multiplier => 1e6F;
    public override string UnitSymbol => "Mm";

    public override Unit DeepCopy()
    {
        return new Megametre(Power);
    }
}

public class Gigametre : LengthUnit
{
    public Gigametre(float power = 1F) : base(power)
    {
    }

    public override float Multiplier => 1e9F;
    public override string UnitSymbol => "Gm";

    public override Unit DeepCopy()
    {
        return new Gigametre(Power);
    }
}


