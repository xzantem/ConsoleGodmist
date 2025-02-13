using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class Frostbite(string source, int duration) : StatusEffect(StatusEffectType.Frostbite, source, duration)
{
    public void Handle(Character target)
    {
        base.Tick(target);
        StatusEffectHandler.AddStatusEffect(new StatusEffect(StatusEffectType.Debuff, source, duration), target);
    }
}