using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class InflictGenericStatusEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public StatusEffect StatusEffect { get; set; }
    public double Chance { get; set; }
    
    public InflictGenericStatusEffect() {} // For JSON serialization

    public InflictGenericStatusEffect(StatusEffect statusEffect, double chance, SkillTarget target = SkillTarget.Enemy)
    {
        Target = target;
        StatusEffect = statusEffect;
        Chance = chance;
    }
    

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                if (Random.Shared.NextDouble() < Chance)
                    StatusEffectHandler.AddStatusEffect(StatusEffect, caster);
                break;
            case SkillTarget.Enemy:
                if (Random.Shared.NextDouble() <
                    EngineMethods.EffectChance(enemy.Resistances[StatusEffect.Type].Value(), Chance))
                    StatusEffectHandler.AddStatusEffect(StatusEffect, enemy);
                break;
        }
    }
}