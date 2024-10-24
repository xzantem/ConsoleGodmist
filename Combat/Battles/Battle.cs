using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using Spectre.Console;

namespace ConsoleGodmist.Combat.Battles;

public class Battle
{
    public Dictionary<BattleUser, int> Users { get; private set; }
    public int TurnCount { get; private set; }
    public bool Escaped { get; private set; }
    private int EscapeAttempts { get; set; }
    public List<Text> Log { get; set; }
    
    public Battle(Dictionary<BattleUser, int> usersTeams)
    {
        Users = usersTeams;
        TurnCount = 1;
    }

    public void NewTurn()
    {
        foreach (var user in Users.Keys)
        {
            user.StartNewTurn();
        }
        BattleTextService.DisplayTurnOrder(Users);
        while (Users.Keys.Any(x => !x.MovedThisTurn) && !Escaped)
        {
            foreach (var user in Users.Keys.Where(user => user.TryMove()))
            {
                BattleTextService.DisplayMovementText(user.User);
                BattleTextService.DisplayTurnOrder(Users);
                if (user.User.StatusEffects
                    .Any(x => x.Type is StatusEffectType.Stun or
                        StatusEffectType.Freeze or StatusEffectType.Sleep))
                    continue;
                HandleEffects(user.User);
                switch (Users[user])
                {
                    case 0:
                        var hasMoved = false;
                        while (!hasMoved) 
                            hasMoved = PlayerMove(user.User as PlayerCharacter, Users
                                     .FirstOrDefault(x => x.Key != user).Key.User as EnemyCharacter);
                        break;
                    case 1:
                        Thread.Sleep(2000);
                        AIMove(user.User as EnemyCharacter, EngineMethods
                            .RandomChoice(Users
                                .Where(x => x.Key != user)
                                .ToDictionary(x => x.Key, x => 1)).User);
                        break;
                }
                var dead = Users
                    .Where(x => x.Key.User.CurrentHealth == 0);
                foreach (var deadUser in dead)
                {
                    BattleTextService.DisplayDeathText(deadUser.Key.User);
                    Users.Remove(deadUser.Key);
                }
            }
        }
        TurnCount++;
    }
    public void HandleEffects(Character user)
    {
        StatusEffectHandler.HandleEffects(user.StatusEffects, user);
        user.HandleModifiers();
        //Handle Potion Effects
    }

    public bool ChooseSkill(PlayerCharacter player, Character target)
    {
        var choices = player.ActiveSkills.Select(x => x.Name).ToArray();
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        player.ActiveSkills[Array.IndexOf(choices, choice)].Use(player, target);
        return true;
    }
    public bool PlayerMove(PlayerCharacter player, EnemyCharacter target)
    {
        BattleTextService.DisplayStatusText(player, target);
        string[] choices =
        [
            locale.UseSkill, locale.UsePotion, locale.ShowStats, locale.ShowStatus, locale.Escape
        ];
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        switch (Array.IndexOf(choices, choice))
        {
            case 0:
                return ChooseSkill(player, target);
            case 1:
                // use potion
                return false;
            case 2:
                var charactersStats = new string[Users.Keys.Count];
                for (var i = 0; i < Users.Keys.Count; i++)
                {
                    charactersStats[i] = Users.Keys.ElementAt(i).User.Name;
                }
                var characterStatsChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .AddChoices(charactersStats)
                    .HighlightStyle(new Style(Color.Gold3_1)));
                BattleTextService.DisplayBattleStatsText(Users
                    .ElementAt(Array.IndexOf(charactersStats, characterStatsChoice)).Key.User);
                return false;
            case 3:
                var charactersStatus = new string[Users.Keys.Count];
                for (var i = 0; i < Users.Keys.Count; i++)
                {
                    charactersStatus[i] = Users.Keys.ElementAt(i).User.Name;
                }
                var characterStatusChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .AddChoices(charactersStatus)
                    .HighlightStyle(new Style(Color.Gold3_1)));
                BattleTextService.DisplayBattleStatusText(Users
                    .ElementAt(Array.IndexOf(charactersStatus, characterStatusChoice)).Key.User);
                return false;
            case 4:
                BattleTextService.DisplayTryEscapeText();
                if (!(Random.Shared.NextDouble() < 0.5 + EscapeAttempts * 0.1))
                {
                    BattleTextService.DisplayEscapeFailText();
                    EscapeAttempts++;
                    return true;
                }
                BattleTextService.DisplayEscapeSuccessText();
                player.LoseHonor((int)Users.Where(x => x.Value == 1)
                    .Average(x => x.Key.User.Level) / 3 + 4);
                Escaped = true;
                return true;
        }
        return false;
    }
    public void AIMove(EnemyCharacter enemy, Character target)
    {
        var possibleSkills = enemy.ActiveSkills
            .Where(x => x.ResourceCost >= enemy.CurrentResource || 
                        Math.Abs(enemy.MaximalResource - enemy.CurrentResource) < 0.01)
            .ToDictionary(x => x, x => 1);
        var usedSkill = EngineMethods.RandomChoice(possibleSkills);
        usedSkill.Use(enemy, target);
        Thread.Sleep(2000);
    }

    public int CheckForResult()
    {
        if (Users.All(x => x.Value == 0))
            return 0;
        if (Users.All(x => x.Value == 1))
            return 1;
        if (Escaped)
            return 2;
        return -1;
    }
}