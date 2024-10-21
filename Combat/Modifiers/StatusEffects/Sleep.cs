using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class Sleep(double strength, string source, int duration, bool canInterrupt) : StatusEffect(StatusEffectType.Sleep, source, duration)
{
    public double Strength { get; private set; } = strength;
    public bool CanInterrupt { get; private set; } = canInterrupt;
    public void Handle(Character target)
    {
        base.Handle(target);
    }
}