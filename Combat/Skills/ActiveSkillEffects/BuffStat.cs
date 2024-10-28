using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class BuffStat : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public StatType StatToBuff { get; set; }
    public ModifierType ModifierType { get; set; }
    public double BuffStrength { get; set; }
    public double BuffChance { get; set; }
    public int BuffDuration { get; set; }

    public BuffStat(SkillTarget target, StatType statToBuff, ModifierType modifierType, double buffStrength,
        double buffChance, int buffDuration)
    {
        Target = target;
        StatToBuff = statToBuff;
        ModifierType = modifierType;
        BuffStrength = buffStrength;
        BuffChance = buffChance;
        BuffDuration = buffDuration;
    }
    public BuffStat() {}

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                if (Random.Shared.NextDouble() < BuffChance)
                {
                    caster.AddModifier(StatToBuff,
                        new StatModifier(ModifierType, BuffStrength, source, BuffDuration));
                }
                break;
            case SkillTarget.Enemy:
                if (Random.Shared.NextDouble() <
                    EngineMethods.EffectChance(enemy.Resistances[StatusEffectType.Buff].Value(), BuffChance))
                {
                    enemy.AddModifier(StatToBuff,
                        new StatModifier(ModifierType, BuffStrength, source, BuffDuration));
                }
                break;
        }
    }
}