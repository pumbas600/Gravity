public abstract class DurationUnit : Unit
{
    public DurationUnit(float power = 1) : base(power)
    {
    }

    public override UnitType UnitType => UnitType.DurationUnit;
}

public class Millisecond : DurationUnit
{
    public Millisecond(float power = 1) : base(power)
    {
    }

    public override float Multiplier => 1e-3F;

    public override string UnitSymbol => "ms";

    public override Unit DeepCopy()
    {
        return new Millisecond(Power);
    }
}

public class Second : DurationUnit
{
    public Second(float power = 1) : base(power)
    {
    }

    public override float Multiplier => 1F;

    public override string UnitSymbol => "s";

    public override Unit DeepCopy()
    {
        return new Second(Power);
    }
}

public class Hour : DurationUnit
{
    public Hour(float power = 1) : base(power)
    {
    }

    public override float Multiplier => 3600F;

    public override string UnitSymbol => "hr";

    public override Unit DeepCopy()
    {
        return new Hour(Power);
    }
}
