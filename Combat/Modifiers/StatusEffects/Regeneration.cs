using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class Regeneration(double strength, string source, int duration, string regenType) : 
    StatusEffect(StatusEffectType.Regeneration, source, duration)
{
    public double Strength { get; private set; } = strength;
    public string RegenType { get; private set; } = regenType;
    public void Handle(Character target)
    {
        base.Handle(target);
    }
}