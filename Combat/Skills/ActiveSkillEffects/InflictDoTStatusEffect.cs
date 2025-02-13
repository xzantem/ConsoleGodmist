using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class InflictDoTStatusEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public int Duration { get; set; }
    public double Strength { get; set; }
    public string Source { get; set; }
    public StatusEffectType DoTType { get; set; }
    public double Chance { get; set; }
    
    public InflictDoTStatusEffect() {} // For JSON serialization

    public InflictDoTStatusEffect(SkillTarget target, int duration, double strength, string source,
        StatusEffectType doTType, double chance)
    {
        if (doTType != StatusEffectType.Bleed && doTType != StatusEffectType.Poison && doTType != StatusEffectType.Burn)
        {
            throw new ArgumentException("Invalid DoT type. Must be Bleed, Poison, or Burn.");
        }
        Target = target;
        Duration = duration;
        Strength = strength;
        Source = source;
        DoTType = doTType;
        Chance = chance;
    }


    public void Execute(Character caster, Character enemy, string source)
    {
        var strength = Strength * Random.Shared.Next((int)caster.MinimalAttack, (int)caster.MinimalAttack + 1);
        var chance =
            UtilityMethods.CalculateModValue(Chance, caster.PassiveEffects.GetModifiers("DoTChanceMod"));
        var status = new DoTStatusEffect(strength, DoTType, Source, Duration);
        switch (Target)
        {
            case SkillTarget.Self:
                if (Random.Shared.NextDouble() >= chance) return;
                StatusEffectHandler.AddStatusEffect(status, caster);
                break;
            case SkillTarget.Enemy:
                if (Random.Shared.NextDouble() >=
                    UtilityMethods.EffectChance(enemy.Resistances[DoTType].Value(enemy, $"{DoTType.ToString()}Resistance"), chance)) return;
                StatusEffectHandler.AddStatusEffect(status, enemy);
                break;
        }
    }
}