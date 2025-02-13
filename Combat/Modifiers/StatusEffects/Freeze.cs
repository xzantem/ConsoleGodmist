using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class Freeze(string source, int duration) : StatusEffect(StatusEffectType.Freeze, source, duration)
{
    public void Handle(Character target)
    {
        base.Tick(target);
        target.AddModifier(StatType.Speed, new StatModifier(ModifierType.Additive, Duration, Source, Duration));
    }
}