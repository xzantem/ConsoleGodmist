using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using Newtonsoft.Json.Serialization;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class ToggleInnatePassiveEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public string Type { get; set; }
    public dynamic[]? Effects { get; set; }

    public ToggleInnatePassiveEffect(SkillTarget target, string type, dynamic[]? effects = null)
    {
        Target = target;
        Type = type;
        Effects = effects;
    }
    
    public ToggleInnatePassiveEffect() {} // JSON constructor

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                if (caster.PassiveEffects.InnateEffects.Any(x => x.Type == Type))
                    caster.PassiveEffects.InnateEffects.RemoveAll(x => x.Type == Type);
                else
                    caster.PassiveEffects.Add(new InnatePassiveEffect(caster, source, Type, Effects));
                break;
            case SkillTarget.Enemy:
                if (enemy.PassiveEffects.InnateEffects.Any(x => x.Type == Type))
                    enemy.PassiveEffects.InnateEffects.RemoveAll(x => x.Type == Type);
                else
                    enemy.PassiveEffects.Add(new InnatePassiveEffect(enemy, source, Type, Effects));
                break;
        }
    }
}