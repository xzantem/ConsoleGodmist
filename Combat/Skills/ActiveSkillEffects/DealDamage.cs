using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using Spectre.Console;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class DealDamage : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public DamageType DamageType { get; set; }
    public DamageBase DamageBase { get; set; }
    public double DamageMultiplier { get; set; }
    public bool CanCrit { get; set; }
    public bool AlwaysCrits { get; set; }
    public double LifeSteal { get; set; }
    
    public DealDamage() {}

    public DealDamage(DamageType damageType, DamageBase damageBase, double damageMultiplier, bool canCrit,
        bool alwaysCrits, double lifeSteal)
    {
        Target = SkillTarget.Enemy;
        DamageType = damageType;
        DamageBase = damageBase;
        DamageMultiplier = damageMultiplier;
        CanCrit = canCrit;
        AlwaysCrits = alwaysCrits;
        LifeSteal = lifeSteal;
    }

    private double CalculateDamage(Character caster, Character enemy)
    {
        var damage = caster.DamageDealt * DamageBase switch
        {
            DamageBase.Minimal => caster.MinimalAttack,
            DamageBase.Random => UtilityMethods.RandomDouble(caster.MinimalAttack, caster.MaximalAttack + 1),
            DamageBase.Maximal => caster.MaximalAttack
        };
        damage *= DamageMultiplier;
        if ((!CanCrit || !(Random.Shared.NextDouble() < caster.CritChance)) && !AlwaysCrits) return damage;
        damage *= caster.CritMod;
        ActiveSkillTextService.DisplayCritText(caster);
        return damage;
    }

    public void Execute(Character caster, Character enemy, string source)
    {
        var damage = 0.0;
        damage = Target switch
        {
            SkillTarget.Self => caster.TakeDamage(DamageType, CalculateDamage(caster, enemy)),
            SkillTarget.Enemy => enemy.TakeDamage(DamageType, CalculateDamage(caster, enemy)),
            _ => damage
        };
        if (!(LifeSteal > 0) || !(damage > 0)) return;
        switch (Target)
        {
            case SkillTarget.Self:
                enemy.Heal(damage * LifeSteal);
                break;
            case SkillTarget.Enemy:
                caster.Heal(damage * LifeSteal);
                break;
        }
    }
}