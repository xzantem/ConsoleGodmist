using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class ActiveSkillTextService
{
    public static void DisplayUseSkillText(Character caster, Character target, ActiveSkill skill)
    {
        AnsiConsole.Write(new Text($"{caster.Name} {locale.Uses} {skill.Name} {locale.On} {target.Name}!\n", 
            Stylesheet.Styles["default"]));
    }

    public static void DisplayCritText(Character caster)
    {
        AnsiConsole.Write(new Text($"{caster.Name} {locale.StrikesCritically}! ", 
            Stylesheet.Styles["default"]));
    }
    public static void DisplayMissText(Character caster)
    {
        AnsiConsole.Write(new Text($"{caster.Name} {locale.Misses}!\n", 
            Stylesheet.Styles["default"]));
    }
    public static void DisplayStatusEffectText(Character target, StatusEffect statusEffect)
    {
        switch (statusEffect.Type)
        {
            case StatusEffectType.Bleed:
                AnsiConsole.Write(new Text($"{target.Name} {locale.Bleeds} {locale.And} {locale.Takes}", Stylesheet.Styles["default"]));
                AnsiConsole.Write(new Text($" {(int)((DoTStatusEffect)statusEffect).Strength}", Stylesheet.Styles["damage-bleed"]));
                AnsiConsole.Write(new Text($" {locale.DamageGenitive} {locale.ForTheNext} " +
                                           $"{statusEffect.Duration} {locale.Turns}\n", Stylesheet.Styles["default"]));
                break;
            case StatusEffectType.Buff:
                break;
            case StatusEffectType.Debuff:
                break;
            case StatusEffectType.Poison:
                break;
            case StatusEffectType.Burn:
                break;
            case StatusEffectType.Stun:
                break;
            case StatusEffectType.Freeze:
                break;
            case StatusEffectType.Frostbite:
                break;
            case StatusEffectType.Sleep:
                break;
            case StatusEffectType.Invisible:
                break;
            case StatusEffectType.Paralysis:
                break;
            case StatusEffectType.Provocation:
                break;
            case StatusEffectType.Shield:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}