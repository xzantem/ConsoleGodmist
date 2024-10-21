using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class HealTarget : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public double HealAmount { get; set; }
    public DamageBase HealBase { get; set; }
    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.Heal(CalculateHeal(caster, enemy));
                break;
            case SkillTarget.Enemy:
                enemy.Heal(CalculateHeal(caster, enemy));
                break;
        }
    }

    public double CalculateHeal(Character caster, Character enemy)
    {
        return HealBase switch
        {
            DamageBase.Minimal => HealAmount,
            DamageBase.Random => HealAmount,
            DamageBase.Maximal => HealAmount,
            DamageBase.CasterMaxHealth => HealAmount * caster.MaximalHealth,
            DamageBase.TargetMaxHealth => HealAmount * enemy.MaximalHealth,
            DamageBase.CasterCurrentHealth => HealAmount * caster.CurrentHealth,
            DamageBase.TargetCurrentHealth => HealAmount * enemy.CurrentHealth,
            DamageBase.CasterMissingHealth => HealAmount * (caster.MaximalHealth - caster.CurrentHealth),
            DamageBase.TargetMissingHealth => HealAmount * (enemy.MaximalHealth - enemy.CurrentHealth)
        };
    }
}