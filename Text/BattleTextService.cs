using System.Resources;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class BattleTextService
{
    public static void DisplayStatusText(BattleUser player, BattleUser enemy)
    {
        var playerShields = player.User.StatusEffects.Any(x => x.Type == StatusEffectType.Shield)
            ? $"(+{(int)player.User.StatusEffects.Where(x => x.Type == StatusEffectType.Shield)
                .Cast<Shield>().Sum(s => s.Strength)})"
            : "";
        var enemyShields = enemy.User.StatusEffects.Any(x => x.Type == StatusEffectType.Shield)
            ? $"(+{(int)player.User.StatusEffects.Where(x => x.Type == StatusEffectType.Shield)
                .Cast<Shield>().Sum(s => s.Strength)})"
            : "";
        AnsiConsole.Write(new Text($"\n{player.User.Name}, {locale.Level} {player.User.Level} ({player.ActionValue} {locale.ToMove})", Stylesheet.Styles["success"]));
        AnsiConsole.Write(new Text($"\n{locale.HealthC}: {(int)player.User.CurrentHealth}{playerShields}/{(int)player.User.MaximalHealth}" +
                                   $", {ResourceShortText(player.User)}: {(int)player.User.CurrentResource}/{(int)player.User.MaximalResource}\n"
            , Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"\n{enemy.User.Name}, {locale.Level} {enemy.User.Level} ({enemy.ActionValue} {locale.ToMove})", Stylesheet.Styles["failure"]));
        AnsiConsole.Write(new Text($"\n{locale.HealthC}: {(int)enemy.User.CurrentHealth}{enemyShields}/{(int)enemy.User.MaximalHealth}" +
                                   $", {ResourceShortText(enemy.User)}: {(int)enemy.User.CurrentResource}/{(int)enemy.User.MaximalResource}\n\n"
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
    public static void DisplayCannotMoveText(Character target)
    {
        AnsiConsole.Write(new Text($"{target.Name} {locale.CannotMove}\n", Stylesheet.Styles["highlight-bad"]));
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
                                  $"{locale.Defense}: {target.PhysicalDefense:F0}:{target.MagicDefense:F0}, " +
                                  $"{locale.Dodge}: {target.Dodge:F0}\n\n", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"{locale.Resistances}\n", Stylesheet.Styles["default-bold"]));
        AnsiConsole.Write(new Text($"{locale.Debuff}: {target.Resistances[StatusEffectType.Debuff].Value(target, "DebuffResistance"):P0}, " +
                                   $"{locale.Stun}: {target.Resistances[StatusEffectType.Stun].Value(target, "StunResistance"):P0}, " +
                                   $"{locale.Freeze}: {target.Resistances[StatusEffectType.Freeze].Value(target, "FreezeResistance"):P0}\n" +
                                   $"{locale.Bleed}: {target.Resistances[StatusEffectType.Bleed].Value(target, "BleedResistance"):P0}, " +
                                   $"{locale.Poison}: {target.Resistances[StatusEffectType.Poison].Value(target, "PoisonResistance"):P0}, " +
                                   $"{locale.Burn}: {target.Resistances[StatusEffectType.Burn].Value(target, "BurnResistance"):P0}\n" +
                                   $"{locale.Frostbite}: {target.Resistances[StatusEffectType.Frostbite].Value(target, "FrostbiteResistance"):P0}, " +
                                   $"{locale.Sleep}: {target.Resistances[StatusEffectType.Sleep].Value(target, "SleepResistance"):P0}, " +
                                   $"{locale.Paralysis}: {target.Resistances[StatusEffectType.Paralysis].Value(target, "ParalysisResistance"):P0}, " +
                                   $"{locale.Provocation}: {target.Resistances[StatusEffectType.Provocation].Value(target, "ProvocationResistance"):P0}\n\n", 
            Stylesheet.Styles["default"]));
    }

    public static void DisplayStatusEffectText(Character target)
    {
        AnsiConsole.Write(new Text($"\n{target.Name}, {locale.Level} {target.Level}\n", Stylesheet.Styles["highlight-good"]));
        if (target.StatusEffects.Count == 0)
        {
            AnsiConsole.Write(new Text($"{locale.NoStatusEffects}\n\n", Stylesheet.Styles["default"]));
        }
        else
        {
            var dotsList = target.StatusEffects.Where(s => s.Type is StatusEffectType.Bleed
                or StatusEffectType.Poison or StatusEffectType.Burn).Cast<DoTStatusEffect>().ToList();
            if (dotsList.Count > 0)
            {
                if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Bleed))
                    AnsiConsole.Write(new Text($"{locale.Bleed} ({string.Join(", ",dotsList
                        .Where(x => x.Type == StatusEffectType.Bleed).Select(s => s.Source))}): {(int)dotsList
                        .Where(x => x.Type == StatusEffectType.Bleed).Sum(s => s.Strength)} [{dotsList
                        .Where(x => x.Type == StatusEffectType.Bleed).Max(s => s.Duration)}]\n", Stylesheet.Styles["damage-bleed"]));
                if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Poison))
                    AnsiConsole.Write(new Text($"{locale.Poison} ({string.Join(", ",dotsList
                        .Where(x => x.Type == StatusEffectType.Poison).Select(s => s.Source))}): {(int)dotsList
                        .Where(x => x.Type == StatusEffectType.Poison).Sum(s => s.Strength)} [{dotsList
                        .Where(x => x.Type == StatusEffectType.Poison).Max(s => s.Duration)}]\n", Stylesheet.Styles["damage-poison"]));
                if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Burn))
                    AnsiConsole.Write(new Text($"{locale.Burn} ({string.Join(", ",dotsList
                        .Where(x => x.Type == StatusEffectType.Burn).Select(s => s.Source))}): {(int)dotsList
                        .Where(x => x.Type == StatusEffectType.Burn).Sum(s => s.Strength)} [{dotsList
                        .Where(x => x.Type == StatusEffectType.Burn).Max(s => s.Duration)}]\n", Stylesheet.Styles["damage-burn"]));
            }

            if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Sleep))
            {
                var sleeps = target.StatusEffects
                    .Where(x => x.Type == StatusEffectType.Sleep).Cast<Sleep>().ToList();
                AnsiConsole.Write(new Text($"{locale.Sleep} ({string.Join(", ", sleeps
                    .Select(s => s.Source))}): +{(int)sleeps.Sum(s => s.Strength)} [{sleeps
                    .Max(s => s.Duration)}]\n", Stylesheet.Styles["default"]));
            }
            if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Shield))
            {
                var shields = target.StatusEffects
                    .Where(x => x.Type == StatusEffectType.Shield).Cast<Shield>().ToList();
                AnsiConsole.Write(new Text($"{locale.Sleep} ({string.Join(", ", shields
                    .Select(s => s.Source))}): +{(int)shields.Sum(s => s.Strength)} [{shields
                    .Max(s => s.Duration)}]\n", Stylesheet.Styles["default"]));
            }
            if (target.StatusEffects.Any(x => x.Type == StatusEffectType.Regeneration))
            {
                var regens = target.StatusEffects
                    .Where(x => x.Type == StatusEffectType.Regeneration).Cast<Regeneration>().ToList();
                var regensHealth = regens.Where(x => x.RegenType == "Health").ToList();
                var regensResource = regens.Where(x => x.RegenType == "Resource").ToList();
                if (regensHealth.Count > 0)
                    AnsiConsole.Write(new Text($"{locale.Regeneration} ({string.Join(", ", regensHealth
                        .Select(s => s.Source))}): +{(int)regensHealth
                        .Sum(s => s.Strength)} {locale.HealthShort} [{regensHealth
                        .Max(s => s.Duration)}]\n", Stylesheet.Styles["default"]));
                if (regensResource.Count > 0)
                    AnsiConsole.Write(new Text($"{locale.Regeneration} ({string.Join(", ", regensResource
                        .Select(s => s.Source))}): +{(int)regensResource
                        .Sum(s => s.Strength)} {ResourceShortText(target)} [{regensResource
                        .Max(s => s.Duration)}]\n", Stylesheet.Styles["default"]));
            }
            var other = target.StatusEffects.Where(x => x.Type != StatusEffectType.Bleed && 
                                        x.Type != StatusEffectType.Poison && x.Type != StatusEffectType.Burn &&
                                        x.Type != StatusEffectType.Sleep && x.Type != StatusEffectType.Shield && 
                                        x.Type != StatusEffectType.Regeneration).ToList();
            foreach (var status in other.OrderByDescending(x => x.Duration).OrderBy(x => x.Type))
            {
                AnsiConsole.Write(new Text($"{locale.ResourceManager.GetString(status.Type.ToString())} " +
                                           $"({status.Source}): {status.Effect} [{status.Duration}]\n"));
            }

        }
        foreach (var modifier in target.GetModifiers())
        {
            var mod = modifier.Key.Type switch
            {
                ModifierType.Multiplicative => modifier.Key.Mod.ToString("+#%;-#%;0%"),
                ModifierType.Relative => modifier.Key.Mod.ToString("+#%;-#%;0%"),
                ModifierType.Absolute => modifier.Key.Mod.ToString("+#;-#;0"),
                ModifierType.Additive => modifier.Key.Mod.ToString("+#;-#;0")
            };
            AnsiConsole.Write(new Text($"{locale.ResourceManager.GetString(modifier.Value.ToString())} " +
                                       $"({modifier.Key.Source}): {mod} [{modifier.Key.Duration}]\n"));
        }
    }
}