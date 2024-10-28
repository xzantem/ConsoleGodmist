using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class TradeHealthForResource : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public double HealthSacrificed { get; set; }
    public double ExchangeRatio { get; set; }
    public void Execute(Character caster, Character enemy, string source)
    {
        double damage;
        switch (Target)
        {
            case SkillTarget.Self:
                damage = caster.TakeDamage(DamageType.True, HealthSacrificed * caster.MaximalHealth);
                caster.RegenResource((int)(damage * ExchangeRatio));
                break;
            case SkillTarget.Enemy:
                damage = enemy.TakeDamage(DamageType.True, HealthSacrificed * enemy.MaximalHealth);
                enemy.RegenResource((int)(damage * ExchangeRatio));
                break;
        }
    }

    public TradeHealthForResource(SkillTarget target, double healthSacrificed, double exchangeRatio)
    {
        Target = target;
        HealthSacrificed = healthSacrificed;
        ExchangeRatio = exchangeRatio;
    }
    public TradeHealthForResource() {}
}