﻿using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class InflictDoTStatusEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public int Duration { get; set; }
    public double Strength { get; set; }
    public string Source { get; set; }
    public StatusEffectType DoTType { get; set; }
    public double Chance { get; set; }
    
    public InflictDoTStatusEffect() {} // For JSON serialization

    public InflictDoTStatusEffect(SkillTarget target, int duration, double strength, string source,
        StatusEffectType doTType, double chance)
    {
        if (DoTType != StatusEffectType.Bleed && DoTType != StatusEffectType.Poison && DoTType != StatusEffectType.Burn)
        {
            throw new ArgumentException("Invalid DoT type. Must be Bleed, Poison, or Burn.");
        }
        Target = target;
        Duration = duration;
        Strength = strength;
        Source = source;
        DoTType = doTType;
        Chance = chance;
    }


    public void Execute(Character caster, Character enemy, string source)
    {
        var strength = Strength * Random.Shared.Next((int)caster.MinimalAttack, (int)caster.MinimalAttack + 1);
        switch (Target)
        {
            case SkillTarget.Self:
                if (Random.Shared.NextDouble() < Chance)
                    StatusEffectHandler.AddStatusEffect(new DoTStatusEffect(strength, DoTType, Source, Duration), caster);
                break;
            case SkillTarget.Enemy:
                if (Random.Shared.NextDouble() <
                    EngineMethods.EffectChance(enemy.Resistances[DoTType].Value(), Chance))
                    StatusEffectHandler.AddStatusEffect(new DoTStatusEffect(strength, DoTType, Source, Duration), enemy);
                break;
        }
    }
}