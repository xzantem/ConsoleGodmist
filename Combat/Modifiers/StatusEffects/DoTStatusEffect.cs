using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Combat.Modifiers;

public class DoTStatusEffect(double strength, StatusEffectType statusEffectType, string source, int duration)
    : StatusEffect(statusEffectType, source, duration)
{
    public double Strength { get; private set; } = strength;
}