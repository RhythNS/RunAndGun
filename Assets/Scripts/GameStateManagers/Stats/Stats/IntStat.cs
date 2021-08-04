public abstract class IntStat : Stat
{
    public int Value { get; protected set; }

    public override string StringValue => Value.ToString();

    public void Add(int toAdd) => Value += toAdd;
}

public class KillStat : IntStat
{
    public override string Name { get => "Kills"; }
}

public class ShotsFiredStat : IntStat
{
    public override string Name => "Shots fired";
}

public class ShotsHitStat : IntStat
{
    public override string Name => "Shots hit";
}

public class TimesDied : IntStat
{
    public override string Name => "Times died";
}

public class OtherPlayerRevived : IntStat
{
    public override string Name => "Players revived";
}
