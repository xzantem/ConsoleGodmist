using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class GainShield : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public string Source { get; set; }
    public DamageBase ShieldBase { get; set; }
    public double ShieldStrength { get; set; }
    public double ShieldChance { get; set; }
    public int ShieldDuration { get; set; }

    public GainShield(SkillTarget target, string source, DamageBase shieldBase,
        double shieldStrength, double shieldChance, int shieldDuration)
    {
        Target = target;
        Source = source;
        ShieldBase = shieldBase;
        ShieldStrength = shieldStrength;
        ShieldChance = shieldChance;
        ShieldDuration = shieldDuration;
    }

    public GainShield() {}

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                if (Random.Shared.NextDouble() >= ShieldChance) return;
                var shield = CalculateShield(caster, enemy);
                StatusEffectHandler.AddStatusEffect(shield, caster);
                CharacterEventTextService.DisplayStatusEffectText(caster, shield);
                break;
            case SkillTarget.Enemy:
                if (Random.Shared.NextDouble() >=
                    UtilityMethods.EffectChance(enemy.Resistances[StatusEffectType.Shield].Value(enemy, "ShieldResistance"), ShieldChance)) return;
                var enemyShield = CalculateShield(enemy, caster);
                StatusEffectHandler.AddStatusEffect(enemyShield, enemy);
                CharacterEventTextService.DisplayStatusEffectText(enemy, enemyShield);
                break;
        }
    }

    public Shield CalculateShield(Character caster, Character enemy)
    {
        return ShieldBase switch
        {
            DamageBase.Flat => new Shield(ShieldStrength, Source, ShieldDuration),
            DamageBase.Minimal => new Shield(ShieldStrength * caster.MinimalAttack, Source, ShieldDuration), 
            DamageBase.Random => new Shield(ShieldStrength * Random.Shared.Next((int)caster.MinimalAttack, (int)caster.MaximalAttack + 1), Source, ShieldDuration),
            DamageBase.Maximal => new Shield(ShieldStrength * caster.MaximalAttack, Source, ShieldDuration), 
            DamageBase.CasterMaxHealth => new Shield(ShieldStrength * caster.MaximalHealth, Source, ShieldDuration), 
            DamageBase.TargetMaxHealth => new Shield(ShieldStrength * enemy.MaximalHealth, Source, ShieldDuration), 
            DamageBase.CasterCurrentHealth => new Shield(ShieldStrength * caster.CurrentHealth, Source, ShieldDuration), 
            DamageBase.TargetCurrentHealth => new Shield(ShieldStrength * enemy.CurrentHealth, Source, ShieldDuration), 
            DamageBase.CasterMissingHealth => new Shield(ShieldStrength * (caster.MaximalAttack - caster.CurrentHealth), Source, ShieldDuration), 
            DamageBase.TargetMissingHealth => new Shield(ShieldStrength * (enemy.MaximalAttack - enemy.CurrentHealth), Source, ShieldDuration), 
        };
    }
}