using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class DebuffStat : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public StatType StatToDebuff { get; set; }
    public ModifierType ModifierType { get; set; }
    public double DebuffStrength { get; set; }
    public double DebuffChance { get; set; }
    public int DebuffDuration { get; set; }

    public DebuffStat(SkillTarget target, StatType statToDebuff, ModifierType modifierType, double debuffStrength,
        double debuffChance, int debuffDuration)
    {
        Target = target;
        StatToDebuff = statToDebuff;
        ModifierType = modifierType;
        DebuffStrength = debuffStrength;
        DebuffChance = debuffChance;
        DebuffDuration = debuffDuration;
    }
    public DebuffStat() {}

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.AddModifier(StatToDebuff,
                    new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                break;
            case SkillTarget.Enemy:
                var chance =
                    UtilityMethods.CalculateModValue(DebuffChance, caster.PassiveEffects.GetModifiers("DebuffChanceMod"));
                if (Random.Shared.NextDouble() <
                    UtilityMethods.EffectChance(enemy.Resistances[StatusEffectType.Debuff].Value(enemy, "DebuffResistance"), chance))
                {
                    enemy.AddModifier(StatToDebuff,
                        new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                }
                break;
        }
    }
}