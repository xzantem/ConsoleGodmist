using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using Spectre.Console;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class InflictGenericStatusEffect : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public int Duration { get; set; }
    public string Effect { get; set; }
    public string EffectType { get; set; }
    public double Chance { get; set; }
    
    public InflictGenericStatusEffect() {} // For JSON serialization

    public InflictGenericStatusEffect(string effectType, int duration, double chance, string effect,
        SkillTarget target = SkillTarget.Enemy)
    {
        Target = target;
        EffectType = effectType;
        Duration = duration;
        Chance = chance;
        Effect = effect;
    }
    

    public void Execute(Character caster, Character enemy, string source)
    {
        var chance =
            UtilityMethods.CalculateModValue(Chance, caster.PassiveEffects.GetModifiers("SuppressionChanceMod"));
        var target = Target switch
        {
            SkillTarget.Self => caster,
            SkillTarget.Enemy => enemy
        };
        var effect = EffectType switch
        {
            "Stun" => StatusEffectFactory.CreateStunEffect(target, source, Duration, chance),
            "Freeze" => StatusEffectFactory.CreateFreezeEffect(target, source, Duration, chance)
        };
        if (!StatusEffectFactory.TryAddEffect(target, effect)) return;
        var text = EffectType switch
        {
            "Stun" => $"{target.Name} {locale.IsStunned} {locale.And1} {locale.CannotMove}" +
                      $" {locale.ForTheNext} {Duration} {locale.Turns}",
            "Freeze" => $"{target.Name} {locale.IsFrozen} {locale.And1} {locale.CannotMove}" +
                        $" {locale.ForTheNext} {Duration} {locale.Turns}"
        };
        BattleManager.CurrentBattle?.Interface.AddBattleLogLines(new Text(text, Stylesheet.Styles["default"]));
    }
}