using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Skills;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public class ActiveSkillTextService
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
}