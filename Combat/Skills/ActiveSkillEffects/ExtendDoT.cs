using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class ExtendDoT : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public StatusEffectType Type { get; set; }
    public int Duration { get; set; }

    public ExtendDoT(SkillTarget target, StatusEffectType type, int duration)
    {
        Target = target;
        Type = type;
        Duration = duration;
    }
    
    public ExtendDoT() {} // JSON constructor

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                foreach (var effect in caster.StatusEffects.Where(e => e.Type == Type))
                {
                    effect.Extend(Duration);
                    return;
                }
                break;
            case SkillTarget.Enemy:
                foreach (var effect in enemy.StatusEffects.Where(e => e.Type == Type))
                {
                    effect.Extend(Duration);
                    return;
                }
                break;
        }
    }
}