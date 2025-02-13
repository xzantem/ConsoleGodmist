using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class Shield(double strength, string source, int duration) : StatusEffect(StatusEffectType.Shield, source, duration)
{
    public double Strength { get; private set; } = strength;
    public void Handle(Character target)
    {
        base.Tick(target);
    }
    public double TakeDamage(double amount)
    {
        if (amount > Strength)
        {
            var remainingDamage = amount - Strength;
            Strength = 0;
            return remainingDamage;
        }
        Strength -= amount;
        return 0;
    }
}