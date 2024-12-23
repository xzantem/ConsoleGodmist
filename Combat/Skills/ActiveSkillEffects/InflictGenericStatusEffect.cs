using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class InflictGenericStatusEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public int Duration { get; set; }
    public string Source { get; set; }
    public string Effect { get; set; }
    public StatusEffectType EffectType { get; set; }
    public double Chance { get; set; }
    
    public InflictGenericStatusEffect() {} // For JSON serialization

    public InflictGenericStatusEffect(StatusEffectType effectType, int duration, double chance, string source, string effect,
        SkillTarget target = SkillTarget.Enemy)
    {
        Target = target;
        EffectType = effectType;
        Duration = duration;
        Source = source;
        Chance = chance;
        Effect = effect;
    }
    

    public void Execute(Character caster, Character enemy, string source)
    {
        var effect = new StatusEffect(EffectType, Source, Duration, Effect);
        switch (Target)
        {
            case SkillTarget.Self:
                if (Random.Shared.NextDouble() >= Chance) return;
                StatusEffectHandler.AddStatusEffect(effect, caster);
                CharacterEventTextService.DisplayStatusEffectText(caster, effect);
                break;
            case SkillTarget.Enemy:
                if (Random.Shared.NextDouble() >=
                    UtilityMethods.EffectChance(enemy.Resistances[effect.Type].Value(), Chance)) return;
                StatusEffectHandler.AddStatusEffect(effect, enemy);
                break;
        }
    }
}