using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class ActiveSkillTextService
{
    public static void DisplayUseSkillText(Character caster, Character target, ActiveSkill skill, double hitChance)
    {
        AnsiConsole.Write(new Text(skill.Effects.Any(x => x.Target == SkillTarget.Enemy)?
                $"{caster.Name} {locale.Uses} {skill.Name} {locale.On} {target.Name}! (to hit: {hitChance:P0})\n" : 
                $"{caster.Name} {locale.Uses} {skill.Name}!\n", 
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
}