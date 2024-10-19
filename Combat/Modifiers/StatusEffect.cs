using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class StatusEffect(double strength, StatusEffectType type, string source, int duration)
{
    public double Strength { get; private set; } = strength;
    public StatusEffectType Type { get; private set; } = type;
    public int Duration { get; private set; } = duration;
    public string Source { get; private set; } = source;
    public int RemainingDuration = duration;
}