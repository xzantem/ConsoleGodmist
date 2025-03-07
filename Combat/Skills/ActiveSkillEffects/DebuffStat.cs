using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using Spectre.Console;

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
        var target = Target switch
        {
            SkillTarget.Self => caster,
            SkillTarget.Enemy => enemy
        };
        switch (Target)
        {
            case SkillTarget.Self:
                if (!(Random.Shared.NextDouble() < DebuffChance)) return;
                target.AddModifier(StatToDebuff,
                    new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                break;
            case SkillTarget.Enemy:
                var chance =
                    UtilityMethods.CalculateModValue(DebuffChance, caster.PassiveEffects.GetModifiers("DebuffChanceMod"));
                if (!(Random.Shared.NextDouble() <
                      UtilityMethods.EffectChance(
                          target.Resistances[StatusEffectType.Debuff].Value(enemy, "DebuffResistance"), chance))) return;
                enemy.AddModifier(StatToDebuff,
                    new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                break;
        }
        var txt1 = StatToDebuff switch
        {
            StatType.MaximalHealth => $"{locale.HealthC}",
            StatType.MinimalAttack => $"{locale.MinimalAttack}",
            StatType.MaximalAttack => $"{locale.MaximalAttack}",
            StatType.Dodge => $"{locale.Dodge}",
            StatType.PhysicalDefense => $"{locale.PhysicalDefense}",
            StatType.MagicDefense => $"{locale.MagicDefense}",
            StatType.CritChance => $"{locale.CritChance}",
            StatType.Speed => $"{locale.Speed}",
            StatType.Accuracy => $"{locale.Accuracy}",
            StatType.MaximalResource => $"{locale.MaximalResource}",
        };
        var txt2 = ModifierType switch
        {
            ModifierType.Relative or ModifierType.Multiplicative => $"{DebuffStrength:P}",
            ModifierType.Absolute or ModifierType.Additive => $"{DebuffStrength:#}",
        };
        BattleManager.CurrentBattle?.Interface.AddBattleLogLines(
            new Text($"{target.Name}: {txt1} {locale.IsDebuffed} {txt2} {locale.ForTheNext} {DebuffDuration} {locale.Turns}", Stylesheet.Styles["default"]));
    }
}