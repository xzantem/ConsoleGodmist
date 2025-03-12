using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using Spectre.Console;

namespace ConsoleGodmist.Combat.Modifiers;

public static class StatusEffectHandler
{

    public static double TakeShieldsDamage(List<TimedPassiveEffect> shields, Character target, double damage)
    {
        foreach (var shield in shields.ToList().TakeWhile(x => !(damage <= 0)))
        {
            if (shield.Effects[1] >= damage)
            {
                shield.Effects[1] -= damage;
                damage = 0;
                if (shield.Effects[1] == 0)
                {
                    target.PassiveEffects.Remove(shield);
                }
                break;
            }

            damage -= shield.Effects[1];
            target.PassiveEffects.Remove(shield);
        }
        return damage;
    }
}