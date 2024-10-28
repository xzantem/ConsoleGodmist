﻿using System.Resources;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class BattleTextService
{
    public static void DisplayStatusText(PlayerCharacter player, EnemyCharacter enemy)
    {
        var playerShields = player.StatusEffects.Any(x => x.Type == StatusEffectType.Shield)
            ? $"(+{player.StatusEffects.Where(x => x.Type == StatusEffectType.Shield)
                .Cast<Shield>().Sum(s => s.Strength):F0})"
            : "";
        var enemyShields = enemy.StatusEffects.Any(x => x.Type == StatusEffectType.Shield)
            ? $"(+{player.StatusEffects.Where(x => x.Type == StatusEffectType.Shield)
                .Cast<Shield>().Sum(s => s.Strength):F0})"
            : "";
        AnsiConsole.Write(new Text($"\n{player.Name}, {locale.Level} {player.Level}", Stylesheet.Styles["success"]));
        AnsiConsole.Write(new Text($"\n{locale.HealthC}: {(int)player.CurrentHealth}{playerShields}/{(int)player.MaximalHealth}" +
                                   $", {ResourceShortText(player)}: {(int)player.CurrentResource}/{(int)player.MaximalResource}\n"
            , Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"{enemy.Name}, {locale.Level} {enemy.Level}", Stylesheet.Styles["failure"]));
        AnsiConsole.Write(new Text($"\n{locale.HealthC}: {(int)enemy.CurrentHealth}{enemyShields}/{(int)enemy.MaximalHealth}" +
                                   $", {ResourceShortText(enemy)}: {(int)enemy.CurrentResource}/{(int)enemy.MaximalResource}\n\n"
            , Stylesheet.Styles["default"]));
    }

    public static string ResourceShortText(Character character)
    {
        return character.ResourceType switch
        {
            ResourceType.Fury => locale.FuryShort,
            ResourceType.Mana => locale.ManaShort,
            ResourceType.Momentum => locale.MomentumShort
        };
    }

    public static void DisplayMovementText(Character moving)
    {
        AnsiConsole.Write(new Text($"{locale.Moving}: {moving.Name}\n", Stylesheet.Styles["dungeon-icon-exit"]));
    }

    public static void DisplayEndBattleText(bool won)
    {
        AnsiConsole.Write(won
            ? new Text($"{locale.Victory}!\n\n", Stylesheet.Styles["highlight-good"])
            : new Text($"{locale.Defeat}! {locale.GameOver}!\n\n", Stylesheet.Styles["highlight-bad"]));
    }
    public static void DisplayDeathText(Character dead)
    {
        AnsiConsole.Write(new Text($"{dead.Name} {locale.Dies}\n", Stylesheet.Styles["value-lost"]));
    }

    public static void DisplayTurnOrder(Dictionary<BattleUser, int> battleUsers)
    {
        battleUsers = battleUsers.ToList().OrderBy(x => x.Key.ActionValue)
            .ToDictionary(x => x.Key, x => x.Value);
        var segments = battleUsers
            .Select(battleUser => $"{battleUser.Key.User.Name} ({battleUser.Key.ActionValue})")
            .ToList();
        AnsiConsole.Write(new Text($"{locale.TurnOrder}: [{string.Join(" -> ", segments)}]\n", Stylesheet.Styles["dungeon-icon-exit"]));
    }

    public static void DisplayBattleStartText(EnemyCharacter enemy)
    {
        AnsiConsole.Write(new Text($"{locale.BattleStart}: {enemy.Name}\n\n", Stylesheet.Styles["dungeon-icon-battle"]));
    }
    
    public static void DisplayTryEscapeText()
    {
        AnsiConsole.Write(new Text($"{locale.TryEscape}...\n", Stylesheet.Styles["default"]));
        Thread.Sleep(2000);
    }
    public static void DisplayEscapeFailText()
    {
        AnsiConsole.Write(new Text($"{locale.EscapeFail}!\n\n", Stylesheet.Styles["failure"]));
    }
    public static void DisplayEscapeSuccessText()
    {
        AnsiConsole.Write(new Text($"{locale.EscapeSuccess}!\n\n", Stylesheet.Styles["success"]));
    }

    public static void DisplayBattleStatsText(Character target)
    {
        var resourceType = target.ResourceType switch
        {
            ResourceType.Fury => "RP",
            ResourceType.Mana => "MP",
            ResourceType.Momentum => "SP"
        };
        AnsiConsole.Write(new Text($"\n{target.Name}, {locale.Level} {target.Level} ", Stylesheet.Styles["highlight-good"]));
        AnsiConsole.Write(new Text($"\n{locale.HealthC}: {target.CurrentHealth:F0}/{target.MaximalHealth:F0}" +
                                  $", {resourceType}: {target.CurrentResource:F0}/{target.MaximalResource:F0}\n" +
                                  $"{locale.Attack}: {target.MinimalAttack:F0}-{target.MaximalAttack:F0}, {locale.Crit}: " +
                                  $"{target.CritChance:P2} [{target.CritMod:F2}x]\n" +
                                  $"{locale.Accuracy}: {(int)target.Accuracy:F0}, {locale.Speed}: {target.Speed:F0}\n" +
                                  $"{locale.Defense}: {target.PhysicalDefense:F0} | {target.MagicDefense:F0}, " +
                                  $"{locale.Dodge}: {target.Dodge:F0}\n\n", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"{locale.Resistances}\n", Stylesheet.Styles["default-bold"]));
        AnsiConsole.Write(new Text($"{locale.Debuff}: {target.Resistances[StatusEffectType.Debuff].Value():P0}, " +
                                   $"{locale.Stun}: {target.Resistances[StatusEffectType.Stun].Value():P0}, " +
                                   $"{locale.Freeze}: {target.Resistances[StatusEffectType.Freeze].Value():P0}\n" +
                                   $"{locale.Bleed}: {target.Resistances[StatusEffectType.Bleed].Value():P0}, " +
                                   $"{locale.Poison}: {target.Resistances[StatusEffectType.Poison].Value():P0}, " +
                                   $"{locale.Burn}: {target.Resistances[StatusEffectType.Burn].Value():P0}\n" +
                                   $"{locale.Frostbite}: {target.Resistances[StatusEffectType.Frostbite].Value():P0}, " +
                                   $"{locale.Paralysis}: {target.Resistances[StatusEffectType.Paralysis].Value():P0}, " +
                                   $"{locale.Provocation}: {target.Resistances[StatusEffectType.Provocation].Value():P0}\n\n", 
            Stylesheet.Styles["default"]));
    }

    public static void DisplayBattleStatusText(Character target)
    {
        AnsiConsole.Write(new Text($"\n{target.Name}, {locale.Level} {target.Level}\n", Stylesheet.Styles["highlight-good"]));
        if (target.StatusEffects.Count == 0)
        {
            AnsiConsole.Write(new Text($"{locale.NoStatusEffects}\n\n", Stylesheet.Styles["default"]));
            return;
        }
        var dotsList = target.StatusEffects.Where(s => s.Type is StatusEffectType.Bleed
                or StatusEffectType.Poison or StatusEffectType.Burn).Cast<DoTStatusEffect>().ToList();
        if (dotsList.Count > 0)
        {
            if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Bleed))
                AnsiConsole.Write(new Text($"{locale.Bleed} ({string.Join(", ",dotsList
                    .Where(x => x.Type == StatusEffectType.Bleed).Select(s => s.Source))}): {(int)dotsList
                    .Where(x => x.Type == StatusEffectType.Bleed).Sum(s => s.Strength)} [{dotsList
                    .Where(x => x.Type == StatusEffectType.Bleed).Max(s => s.RemainingDuration)}]\n", Stylesheet.Styles["damage-bleed"]));
            if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Poison))
                AnsiConsole.Write(new Text($"{locale.Poison} ({string.Join(", ",dotsList
                    .Where(x => x.Type == StatusEffectType.Poison).Select(s => s.Source))}): {(int)dotsList
                    .Where(x => x.Type == StatusEffectType.Poison).Sum(s => s.Strength)} [{dotsList
                    .Where(x => x.Type == StatusEffectType.Poison).Max(s => s.RemainingDuration)}]\n", Stylesheet.Styles["damage-poison"]));
            if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Sleep))
                AnsiConsole.Write(new Text($"{locale.Burn} ({string.Join(", ",dotsList
                    .Where(x => x.Type == StatusEffectType.Burn).Select(s => s.Source))}): {(int)dotsList
                    .Where(x => x.Type == StatusEffectType.Burn).Sum(s => s.Strength)} [{dotsList
                    .Where(x => x.Type == StatusEffectType.Burn).Max(s => s.RemainingDuration)}]\n", Stylesheet.Styles["damage-burn"]));
        }
        if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Sleep))
            AnsiConsole.Write(new Text($"{locale.Sleep} ({string.Join(", ", target.StatusEffects
                .Where(x => x.Type == StatusEffectType.Sleep).Select(s => s.Source))}): +" +
               $"{(int)target.StatusEffects.Where(x => x.Type == StatusEffectType.Sleep)
               .Cast<Sleep>().Sum(s => s.Strength)} [{target.StatusEffects
               .Where(x => x.Type == StatusEffectType.Sleep).Max(s => s.RemainingDuration)}]\n", Stylesheet.Styles["default"]));
        if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Shield))
            AnsiConsole.Write(new Text($"{locale.Sleep} ({string.Join(", ", target.StatusEffects
                .Where(x => x.Type == StatusEffectType.Shield).Select(s => s.Source))}): +" +
                                       $"{(int)target.StatusEffects.Where(x => x.Type == StatusEffectType.Shield)
                                           .Cast<Sleep>().Sum(s => s.Strength)} [{target.StatusEffects
                                           .Where(x => x.Type == StatusEffectType.Shield).Max(s => s.RemainingDuration)}]\n", Stylesheet.Styles["default"]));
        var other = target.StatusEffects.Where(x => x.Type != StatusEffectType.Bleed && 
                                    x.Type != StatusEffectType.Poison && x.Type != StatusEffectType.Burn &&
                                    x.Type != StatusEffectType.Sleep && x.Type != StatusEffectType.Shield).ToList();
        foreach (var status in other.OrderByDescending(x => x.RemainingDuration).OrderBy(x => x.Type))
        {
            AnsiConsole.Write(new Text($"{locale.ResourceManager.GetString(status.Type.ToString())} " +
                                       $"({status.Source}): {status.Effect} [{status.RemainingDuration}]\n"));
        }
    }
}