using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class RegenResource : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public double RegenAmount { get; set; }
    public DamageBase RegenBase { get; set; }
    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.RegenResource((int)CalculateRegen(caster, enemy));
                break;
            case SkillTarget.Enemy:
                enemy.RegenResource((int)CalculateRegen(caster, enemy));
                break;
        }
    }

    public RegenResource(SkillTarget target, double regenAmount, DamageBase regenBase)
    {
        Target = target;
        RegenAmount = regenAmount;
        RegenBase = regenBase;
    }
    public RegenResource() { }

    public double CalculateRegen(Character caster, Character enemy)
    {
        return RegenBase switch
        {
            DamageBase.Flat => RegenAmount,
            DamageBase.Minimal => RegenAmount * caster.MinimalAttack,
            DamageBase.Random => RegenAmount * Random.Shared.Next((int)caster.MinimalAttack, (int)caster.MaximalAttack + 1),
            DamageBase.Maximal => RegenAmount * caster.MinimalAttack,
            DamageBase.CasterMaxHealth => RegenAmount * caster.MaximalHealth,
            DamageBase.TargetMaxHealth => RegenAmount * enemy.MaximalHealth,
            DamageBase.CasterCurrentHealth => RegenAmount * caster.CurrentHealth,
            DamageBase.TargetCurrentHealth => RegenAmount * enemy.CurrentHealth,
            DamageBase.CasterMissingHealth => RegenAmount * (caster.MaximalHealth - caster.CurrentHealth),
            DamageBase.TargetMissingHealth => RegenAmount * (enemy.MaximalHealth - enemy.CurrentHealth)
        };
    }
}