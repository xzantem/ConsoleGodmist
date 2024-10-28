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
        switch (Target)
        {
            case SkillTarget.Self:
                caster.StatusEffects.RemoveAll(x => x.Type == StatusEffectType);
                break;
            case SkillTarget.Enemy:
                enemy.StatusEffects.RemoveAll(x => x.Type == StatusEffectType);
                break;
        }
    }
}