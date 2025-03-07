using System.Resources;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class BattleTextService
{


    public static string ResourceShortText(Character character)
    {
        return character.ResourceType switch
        {
            ResourceType.Fury => locale.FuryShort,
            ResourceType.Mana => locale.ManaShort,
            ResourceType.Momentum => locale.MomentumShort
        };
    }
    public static string UnselectableSkillMarkup(ActiveSkill skill, BattleUser player)
    {
        var resourceCostInfo = (skill.ResourceCost <= player.User.CurrentResource ||
                                Math.Abs(player.User.MaximalResource - player.User.CurrentResource) < 0.01)
            ? $"{(int)UtilityMethods.CalculateModValue(skill.ResourceCost, player.User.PassiveEffects.GetModifiers("ResourceCost"))} " +
              $"{ResourceShortText(player.User as PlayerCharacter)}"
            : $"[red]{(int)UtilityMethods.CalculateModValue(skill.ResourceCost, player.User.PassiveEffects.GetModifiers("ResourceCost"))} " +
              $"{ResourceShortText(player.User as PlayerCharacter)}[/]";
        var actionCostInfo = (skill.ActionCost * player.MaxActionPoints.Value(player.User, "MaxActionPoints")
                              <= player.CurrentActionPoints)
            ? $"{(int)(skill.ActionCost * player.MaxActionPoints.BaseValue)} {locale.ActionPointsShort}"
            : $"[red]{(int)(skill.ActionCost * player.MaxActionPoints.BaseValue)} {locale.ActionPointsShort}[/]";
        return "\n- " + skill.Name + $" ({resourceCostInfo}, {actionCostInfo})";
    }

    public static Text SkillDescription(ActiveSkill skill)
    {
        return new Text("");
    }
}