using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.TextService;
using Spectre.Console;

namespace ConsoleGodmist.Combat.Battles;

public class Battle
{
    public Dictionary<BattleUser, int> Users { get; private set; }
    public int TurnCount { get; private set; }
    public bool Escaped { get; private set; }
    private int EscapeAttempts { get; set; }
    
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
        //BattleTextService.DisplayTurnOrder(Users);
        while (Users.Keys.Any(x => !x.MovedThisTurn) && !Escaped)
        {
            foreach (var user in Users.Keys.Where(user => user.TryMove()))
            {
                BattleTextService.DisplayMovementText(user.User);
                //BattleTextService.DisplayTurnOrder(Users);
                if (user.User.StatusEffects
                    .Any(x => x.Type is StatusEffectType.Stun or
                        StatusEffectType.Freeze or StatusEffectType.Sleep))
                {
                    BattleTextService.DisplayCannotMoveText(user.User);
                    HandleEffects(user.User);
                    continue;
                }
                HandleEffects(user.User);
                switch (Users[user])
                {
                    case 0:
                        var hasMoved = false;
                        while (!hasMoved) 
                            hasMoved = PlayerMove(user, Users
                                     .FirstOrDefault(x => x.Key != user).Key);
                        break;
                    case 1:
                        Thread.Sleep(1000);
                        AIMove(user, EngineMethods
                            .RandomChoice(Users
                                .Where(x => x.Key != user)
                                .ToDictionary(x => x.Key, x => 1)));
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
        user.RegenResource((int)user.ResourceRegen);
    }

    public bool ChooseSkill(PlayerCharacter player, Character target)
    {
        var skills = player.ActiveSkills
            .Select(x => x.Name + $" ({x.ResourceCost} {BattleTextService.ResourceShortText(player)})").ToArray();
        var choices = skills.Append(locale.Return).ToArray();
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        return choice != locale.Return && player.ActiveSkills[Array.IndexOf(choices, choice)].Use(player, target);
        ;
    }
    public bool PlayerMove(BattleUser player, BattleUser target)
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
                return ChooseSkill(player.User as PlayerCharacter, target.User);
            case 1:
                PotionManager.ChoosePotion((player.User as PlayerCharacter).Inventory.Items
                    .Where(x => x.Key.ItemType == ItemType.Potion)
                    .Select(x => x.Key).Cast<Potion>().ToList()).Use();
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
                BattleTextService.DisplayStatusEffectText(Users
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
                (player.User as PlayerCharacter).LoseHonor((int)Users.Where(x => x.Value == 1)
                    .Average(x => x.Key.User.Level) / 3 + 4);
                Escaped = true;
                return true;
        }
        return false;
    }
    public void AIMove(BattleUser enemy, BattleUser target)
    {
        BattleTextService.DisplayStatusText(target, enemy);
        var possibleSkills = enemy.User.ActiveSkills
            .Where(x => x.ResourceCost <= enemy.User.CurrentResource || 
                        Math.Abs(enemy.User.MaximalResource - enemy.User.CurrentResource) < 0.01)
            .ToDictionary(x => x, x => 1);
        var usedSkill = EngineMethods.RandomChoice(possibleSkills);
        usedSkill.Use(enemy.User, target.User);
        Thread.Sleep(1000);
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