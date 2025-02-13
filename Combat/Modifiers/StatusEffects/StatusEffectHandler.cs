using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
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
        HandleRegens(effects
            .Where(effect => effect.Type == StatusEffectType.Regeneration).Cast<Regeneration>().ToList(), target);
        foreach (var effect in effects.ToList())
        {
            effect.Tick(target);
        }
    }

    private static void HandleDoTs(List<DoTStatusEffect> dots, Character target)
    {
        var dotsDict = new Dictionary<StatusEffectType, double>();
        foreach (var effect in dots)
        {
            if (dotsDict.ContainsKey(effect.Type))
                dotsDict[effect.Type] += effect.Strength;
            else
                dotsDict.Add(effect.Type, effect.Strength);
        }
        foreach (var dot in dotsDict)
        {
            var damageType = dot.Key switch
            {
                StatusEffectType.Bleed => DamageType.Bleed,
                StatusEffectType.Poison => DamageType.Poison,
                StatusEffectType.Burn => DamageType.Burn,
            };
            target.TakeDamage(damageType, dot.Value, dot);
        }
    }

    public static double TakeShieldsDamage(List<Shield> shields, Character target, double damage)
    {
        foreach (var shield in shields.Where(x => damage > 0).ToList())
        {
            damage = shield.TakeDamage(damage);
            if (damage > 0)
                target.StatusEffects.Remove(shield);
        }
        return damage;
    }

    private static void HandleSleep(List<Sleep> sleeps, Character target)
    {
        var regen = sleeps.Sum(x => x.Strength);
        if (regen > 0)
            target.Heal(regen);
    }

    private static void HandleRegens(List<Regeneration> regens, Character target)
    {
        var healthRegen = regens.Where(x => x.RegenType == "Health")
            .Sum(r => r.Strength);
        if (healthRegen > 0)
            target.Heal(healthRegen);
        var resourceRegen = regens.Where(x => x.RegenType == "Resource")
            .Sum(r => r.Strength);
        if ((int)resourceRegen > 0)
            target.RegenResource((int)resourceRegen);
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
                else target.StatusEffects.Add(statusEffect);
                break;
            default:
                target.StatusEffects.Add(statusEffect);
                break;
        }
    }
}