using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Combat.Modifiers;

public static class StatusEffectHandler
{
    public static void HandleEffects(List<StatusEffect> effects, Character target)
    {
        HandleDoTs(effects
            .Where(effect => effect.Type is StatusEffectType.Bleed or StatusEffectType.Poison or StatusEffectType.Burn)
            .Cast<DoTStatusEffect>().ToList(), target);
        HandleSleep(effects
            .Where(effect => effect.Type == StatusEffectType.Sleep).Cast<Sleep>().ToList(), target);
        foreach (var effect in effects.ToList())
        {
            effect.Handle(target);
        }
    }

    private static void HandleDoTs(List<DoTStatusEffect> dots, Character target)
    {
        var dotsDict = dots
            .ToDictionary(x => x.Type, x => dots
                .Where(s => s.Type == x.Type).Sum(s => s.Strength));
        foreach (var dot in dotsDict)
        {
            var damageType = dot.Key switch
            {
                StatusEffectType.Bleed => DamageType.Bleed,
                StatusEffectType.Poison => DamageType.Poison,
                StatusEffectType.Burn => DamageType.Burn,
            };
            target.TakeDamage(damageType, dot.Value);
        }
    }

    public static double TakeShieldsDamage(List<Shield> shields, Character target, double damage)
    {
        foreach (var shield in shields.Where(shield => damage > 0))
            damage = shield.TakeDamage(damage);
        return damage;
    }

    private static void HandleSleep(List<Sleep> sleeps, Character target)
    {
        var regen = sleeps.Sum(x => x.Strength);
        if (regen > 0)
            target.Heal(regen);
    }
    public static void AddStatusEffect(StatusEffect statusEffect, Character target)
    {
        CharacterEventTextService.DisplayStatusEffectText(target, statusEffect);
        switch (statusEffect.Type)
        {
            case StatusEffectType.Stun:
            case StatusEffectType.Freeze:
            case StatusEffectType.Frostbite:
            case StatusEffectType.Invisible:
            case StatusEffectType.Paralysis:
            case StatusEffectType.Provocation:
                if (target.StatusEffects.Any(x => x.Type == statusEffect.Type))
                    target.StatusEffects.FirstOrDefault(x => x.Type == statusEffect.Type).Extend(statusEffect.Duration);
                break;
            default:
                target.StatusEffects.Add(statusEffect);
                break;
        }
    }
}