using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class StatModifier(ModifierType modifierType, double value, int duration = -1) // optional parameter, duration -1 represents infinite modifier
{
    public ModifierType Type { get; private set; }
    public double Mod { get; private set; }
    public int Duration { get; private set; }
    public int remainingDuration;
}