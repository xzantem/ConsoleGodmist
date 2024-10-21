using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class DealDamage : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }

    public DamageType DamageType { get; set; }
    public DamageBase DamageBase { get; set; }
    public double DamageMultiplier { get; set; }
    public int DamageInstances { get; set; }
    public bool CanCrit { get; set; }
    public bool AlwaysCrits { get; set; }
    
    public DealDamage() {}

    public DealDamage(DamageType damageType, DamageBase damageBase, double damageMultiplier, bool canCrit,
        bool alwaysCrits, int damageInstances = 1)
    {
        Target = SkillTarget.Enemy;
        DamageType = damageType;
        DamageBase = damageBase;
        DamageMultiplier = damageMultiplier;
        CanCrit = canCrit;
        AlwaysCrits = alwaysCrits;
        DamageInstances = damageInstances;
    }

    private double CalculateDamage(Character caster, Character enemy)
    {
        var damage = DamageBase switch
        {
            DamageBase.Minimal => caster.MinimalAttack,
            DamageBase.Random => EngineMethods.RandomDouble(caster.MinimalAttack, caster.MaximalAttack + 1),
            DamageBase.Maximal => caster.MaximalAttack
        };
        damage *= DamageMultiplier;
        if ((CanCrit && Random.Shared.NextDouble() < caster.CritChance) || AlwaysCrits)
            damage *= caster.CritMod;
        return damage;
    }

    public void Execute(Character caster, Character enemy, string source)
    {    
        for (var i = 0; i < DamageInstances; i++)
            switch (Target)
            {
                case SkillTarget.Self:
                    caster.TakeDamage(DamageType, CalculateDamage(caster, enemy));
                    break;
                case SkillTarget.Enemy:
                    enemy.TakeDamage(DamageType, CalculateDamage(caster, enemy));
                    break;
            }
    }
}