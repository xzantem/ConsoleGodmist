﻿using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class InflictTimedPassiveEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public string Type { get; set; }
    public int Duration { get; set; }
    public dynamic[]? Effects { get; set; }

    public InflictTimedPassiveEffect(SkillTarget target, string type, int duration, dynamic[]? effects = null)
    {
        Target = target;
        Type = type;
        Duration = duration;
        Effects = effects;
    }
    
    public InflictTimedPassiveEffect() {} // JSON constructor

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.PassiveEffects.Add(new TimedPassiveEffect(caster, source, Type, Duration, Effects));
                break;
            case SkillTarget.Enemy:
                enemy.PassiveEffects.Add(new TimedPassiveEffect(caster, source, Type, Duration, Effects));
                break;
        }
    }
}