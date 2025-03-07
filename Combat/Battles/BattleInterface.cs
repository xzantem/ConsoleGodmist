using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleGodmist.Combat.Battles;

public class BattleInterface(BattleUser displayedUser)
{
    public string InfoDisplayMode { get; private set; } = "Resistances";
    public BattleUser DisplayedUser { get; private set; } = displayedUser;
    public List<IRenderable> BattleLog { get; private set; } = [];
    private int _battleLogCurrentIndex;

    private const int SHOWN_BATTLE_LOG_LINES = 10;

    public int ToClear { get; private set; }
    
    public void ChangeDisplayMode()
    {
        string[] choices = [ locale.Resistances, locale.Skills, locale.PassiveEffects ];
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices));
        InfoDisplayMode = Array.IndexOf(choices.ToArray(), choice) switch
        { 0 => "Resistances", 1 => "Skills", 2 => "PassiveEffects" };
    }

    public void AddBattleLogLines(params IRenderable[] lines)
    {
        BattleLog.AddRange(lines);
        _battleLogCurrentIndex = Math.Max(0, BattleLog.Count - SHOWN_BATTLE_LOG_LINES);
    }

    public void ScrollBattleLog(bool up)
    {
        _battleLogCurrentIndex = up ? Math.Max(0, _battleLogCurrentIndex - SHOWN_BATTLE_LOG_LINES) : 
            Math.Max(0, Math.Min(_battleLogCurrentIndex + SHOWN_BATTLE_LOG_LINES, BattleLog.Count - SHOWN_BATTLE_LOG_LINES));
    }

    public bool CanScroll(bool up)
    {
        if (up)
            return _battleLogCurrentIndex > Math.Max(0, _battleLogCurrentIndex - SHOWN_BATTLE_LOG_LINES);
        return 
            _battleLogCurrentIndex < Math.Max(0, Math.Min(_battleLogCurrentIndex + SHOWN_BATTLE_LOG_LINES, 
                BattleLog.Count - SHOWN_BATTLE_LOG_LINES));
    }

    public void ChangeDisplayedUser(List<BattleUser> users)
    {
        var choices = users.Select(x => x.User.Name).ToArray();
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices));
        DisplayedUser = users.ElementAt(Array.IndexOf(choices, choice));
    }

    public void DisplayInterface(BattleUser movingUser, List<BattleUser> users, bool clear = true)
    {
        var table = new Table { ShowHeaders = false };
        table.AddColumn("PlayerUserInfo");
        table.AddColumn("BattleLog");
        table.AddColumn("SelectedUserInfo");
        table.AddColumn("TurnOrder");
        
        table.AddRow(new Text($"{movingUser.User.Name}, {locale.Level} {movingUser.User.Level}", Stylesheet.Styles["rarity-uncommon"]), 
            new Text(locale.BattleLog, Stylesheet.Styles["highlight-good"]), 
            new Text($"{DisplayedUser.User.Name}, {locale.Level} {DisplayedUser.User.Level}", Stylesheet.Styles["highlight-bad"]), 
            new Text(locale.TurnOrder, Stylesheet.Styles["highlight-good"]));
        var movingUserTable = GetUserTable(movingUser);
        var battleLog = new Rows(BattleLog.Slice(_battleLogCurrentIndex, 
            Math.Min(SHOWN_BATTLE_LOG_LINES,  BattleLog.Count - _battleLogCurrentIndex)));
        var displayedUserTable = GetUserTable(DisplayedUser);

        var turnOrder = new Rows(GetTurnOrder(users).Select(x => 
            new Text($"{x.Item1.User.Name} ({x.Item2})")));
        
        table.AddRow(movingUserTable, battleLog, displayedUserTable, turnOrder);
        if (clear)
            UtilityMethods.ClearConsole(ToClear);
        var before = Console.CursorTop;
        AnsiConsole.Write(table);
        ToClear = Console.CursorTop - before;
    }

    private List<(BattleUser, int)> GetTurnOrder(List<BattleUser> unorganized)
    {
        var copy = unorganized.Select(user => new BattleUser(user)).ToList();
        var organized = new List<(BattleUser, int)>();
        var index = 0;
        while (organized.Count < SHOWN_BATTLE_LOG_LINES)
        {
            index++;
            foreach (var user in copy.Where(user => user.TryMove()))
            {
                organized.Add((user, index));
                break;
            }
        }
        return organized.ToList();
    }

    private Table GetUserTable(BattleUser user)
    {
        var table = new Table();
        table.AddColumn(locale.Stats);
        var resourceType = user.User.ResourceType switch
        {
            ResourceType.Fury => "RP",
            ResourceType.Mana => "MP",
            ResourceType.Momentum => "SP"
        };
        var shields = user.User.PassiveEffects.TimedEffects.Any(x => x.Type == "Shield")
            ? $"(+{(int)user.User.PassiveEffects.TimedEffects.Where(x => x.Type == "Shield").ToList()
                .Sum(s => (double)s.Effects[1])})"
            : "";
        var stats = new Rows([
            new Text($"{locale.HealthC}: {user.User.CurrentHealth:F0}{shields}/{user.User.MaximalHealth:F0}"),
            new Text($"{resourceType}: {user.User.CurrentResource:F0}/{user.User.MaximalResource:F0}"),
            new Text($"{locale.Attack}: {user.User.MinimalAttack:F0}-{user.User.MaximalAttack:F0}"),
            new Text($"{locale.Crit}: {user.User.CritChance:P2} [{user.User.CritMod:F2}x]"),
            new Text($"{locale.Accuracy}: {(int)user.User.Accuracy:F0}"),
            new Text($"{locale.Speed}: {user.User.Speed:F0}"),
            new Text($"{locale.Defense}: {user.User.PhysicalDefense:F0}:{user.User.MagicDefense:F0}"),
            new Text($"{locale.Dodge}: {user.User.Dodge:F0}"),
        ]);
        switch (InfoDisplayMode)
        {
            case "Resistances":
                table.AddColumn(locale.Resistances);
                table.AddRow(stats, 
                    new Rows([
                        new Text($"{locale.Debuff}: {user.User.Resistances[StatusEffectType.Debuff]
                            .Value(user.User, "DebuffResistance"):P0}"),
                        new Text($"{locale.Stun}: {user.User.Resistances[StatusEffectType.Stun]
                            .Value(user.User, "StunResistance"):P0}"),
                        new Text($"{locale.Freeze}: {user.User.Resistances[StatusEffectType.Freeze]
                            .Value(user.User, "FreezeResistance"):P0}"),
                        new Text($"{locale.Bleed}: {user.User.Resistances[StatusEffectType.Bleed]
                            .Value(user.User, "BleedResistance"):P0}"),
                        new Text($"{locale.Poison}: {user.User.Resistances[StatusEffectType.Poison]
                            .Value(user.User, "PoisonResistance"):P0}"),
                        new Text($"{locale.Burn}: {user.User.Resistances[StatusEffectType.Burn]
                            .Value(user.User, "BurnResistance"):P0}"),
                        new Text($"{locale.Frostbite}: {user.User.Resistances[StatusEffectType.Frostbite]
                            .Value(user.User, "FrostbiteResistance"):P0}"),
                        new Text($"{locale.Sleep}: {user.User.Resistances[StatusEffectType.Sleep]
                            .Value(user.User, "SleepResistance"):P0}"),
                        new Text($"{locale.Paralysis}: {user.User.Resistances[StatusEffectType.Paralysis]
                            .Value(user.User, "ParalysisResistance"):P0}"),
                        new Text($"{locale.Provocation}: {user.User.Resistances[StatusEffectType.Provocation]
                            .Value(user.User, "ProvocationResistance"):P0}")
                    ]));
                break;
            case "Skills":
                table.AddColumn(locale.Skills);
                table.AddRow(stats, new Rows(user.User.ActiveSkills.Select(x => new Text(x.Name))));
                break;
            case "PassiveEffects":
                table.AddColumn(locale.PassiveEffects);
                table.AddRow(stats, new Rows(user.User.PassiveEffects.TimedEffects.
                    Select(x => new Text($"{x.Type} ({x.Source}) - [{x.Duration}]"))
                    .Concat(user.User.PassiveEffects.ListenerEffects.Select(x => new Text(x.Source))
                        .Concat(user.User.GetModifiers().Select(x => 
                            PassiveEffectTextService.ModifierText(x.Key, x.Value))))));
                break;
        }

        return table;
    }
}