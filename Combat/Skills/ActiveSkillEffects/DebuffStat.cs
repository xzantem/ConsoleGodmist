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

    public void Execute(Character caster, Character enemy, string source)
    {
        switch (Target)
        {
            case SkillTarget.Self:
                caster.AddModifier(StatToDebuff,
                    new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                break;
            case SkillTarget.Enemy:
                if (Random.Shared.NextDouble() <
                    EngineMethods.EffectChance(enemy.Resistances[StatusEffectType.Debuff].Value(), DebuffChance))
                {
                    enemy.AddModifier(StatToDebuff,
                        new StatModifier(ModifierType, -DebuffStrength, source, DebuffDuration));
                }
                break;
        }
    }
}