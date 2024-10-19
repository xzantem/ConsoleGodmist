namespace ConsoleGodmist.Combat.Modifiers;

public class Buff
{
    public class StatusEffect(double strength, string source, int duration)
    {
        public double Strength { get; private set; } = strength;
        public int Duration { get; private set; } = duration;
        public string Source { get; private set; } = source;
        public int RemainingDuration = duration;
    }
}