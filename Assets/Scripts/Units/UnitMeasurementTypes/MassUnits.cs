public abstract class MassUnit : Unit
{
    public MassUnit(float power = 1) : base(power)
    {
    }

    public override UnitType UnitType => UnitType.MassUnit;
}

public class Milligram : MassUnit
{
    public Milligram(float power = 1) : base(power)
    {
    }

    public override float Multiplier => 1e-3F;

    public override string UnitSymbol => "mg";

    public override Unit DeepCopy()
    {
        return new Milligram(Power);
    }
}

public class Gram : MassUnit
{
    public Gram(float power = 1) : base(power)
    {
    }

    public override float Multiplier => 1F;

    public override string UnitSymbol => "g";

    public override Unit DeepCopy()
    {
        return new Gram(Power);
    }
}

public class Kilogram : MassUnit
{
    public Kilogram(float power = 1) : base(power)
    {
    }

    public override float Multiplier => 1e3F;

    public override string UnitSymbol => "kg";

    public override Unit DeepCopy()
    {
        return new Kilogram(Power);
    }
}

public class Tonne : MassUnit
{
    public Tonne(float power = 1) : base(power)
    {
    }

    public override float Multiplier => 1e6F;

    public override string UnitSymbol => "t";

    public override Unit DeepCopy()
    {
        return new Tonne(Power);
    }
}
