using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class ClearStatusEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public StatusEffectType StatusEffectType { get; set; }
    
    public ClearStatusEffect(SkillTarget target, StatusEffectType statusEffectType)
    {
        Target = target;
        StatusEffectType = statusEffectType;
    }
    public ClearStatusEffect() {}
    public void Execute(Character caster, Character enemy, string source)
    {
        //TODO
    }
}