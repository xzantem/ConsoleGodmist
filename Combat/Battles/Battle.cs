using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.Quests;
using ConsoleGodmist.TextService;
using Spectre.Console;

namespace ConsoleGodmist.Combat.Battles;

public class Battle
{
    public Dictionary<BattleUser, int> Users { get; private set; }
    public int TurnCount { get; private set; }
    public bool Escaped { get; set; }
    public bool CanEscape { get; private set; }
    private int EscapeAttempts { get; set; }
    public DungeonField Location { get; private set; }
    
    public Battle(Dictionary<BattleUser, int> usersTeams, DungeonField location, bool canEscape = true)
    {
        Users = usersTeams;
        TurnCount = 1;
        Location = location;
        CanEscape = canEscape;
    }

    public void NewTurn()
    {
        foreach (var user in Users.Keys)
        {
            user.StartNewTurn();
        }
        while (Users.Keys.Any(x => !x.MovedThisTurn) && !Escaped)
        {
            foreach (var user in Users.Keys.Where(user => user.TryMove()))
            {
                BattleTextService.DisplayMovementText(user.User);
                if (user.User.StatusEffects
                    .Any(x => x.Type is StatusEffectType.Stun or
                        StatusEffectType.Freeze or StatusEffectType.Sleep))
                {
                    BattleTextService.DisplayCannotMoveText(user.User);
                    HandleEffects(user);
                    if (CheckForResult() != -1)
                        return;
                    continue;
                }
                HandleEffects(user);
                if (CheckForResult() != -1)
                    return;
                switch (Users[user])
                {
                    case 0:
                        var hasMoved = false;
                        while (!hasMoved)
                        {
                            CheckForDead();
                            if (CheckForResult() != -1)
                                return;
                            BattleTextService.DisplayStatusText(user, Users
                                .FirstOrDefault(x => x.Key != user).Key);
                            hasMoved = PlayerMove(user, Users
                                .FirstOrDefault(x => x.Key != user).Key);
                        }
                        break;
                    case 1:
                        Thread.Sleep(1000);
                        AIMove(user, UtilityMethods
                            .RandomChoice(Users
                                .Where(x => x.Key != user)
                                .ToDictionary(x => x.Key, x => 1)));
                        if (CheckForResult() != -1)
                            return;
                        break;
                }
            }
        }
        TurnCount++;
    }
    public void HandleEffects(BattleUser user)
    {
        StatusEffectHandler.HandleEffects(user.User.StatusEffects, user.User);
        user.User.HandleModifiers();
        user.User.RegenResource((int)user.User.ResourceRegen);
        user.User.PassiveEffects.HandleBattleEvent(new BattleEventData("PerTurn", user));
        user.User.PassiveEffects.TickEffects();
        CheckForDead();
    }

    public void ChooseSkill(BattleUser player, BattleUser target)
    {
        var skills = (player.User as PlayerCharacter)?.ActiveSkills
            .Select(x => x.Name + $" ({(int)UtilityMethods.CalculateModValue(x.ResourceCost, player.User.PassiveEffects.GetModifiers("ResourceCost"))} {BattleTextService.ResourceShortText(player.User as PlayerCharacter)}, " +
                         $"{(int)(x.ActionCost * player.MaxActionPoints.BaseValue)} {locale.ActionPointsShort})").ToArray();
        var choices = skills.Append(locale.Return).ToArray();
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        if (choice == locale.Return) return;
        if (!((player.User as PlayerCharacter)?.ActiveSkills[Array.IndexOf(choices, choice)]
                .ActionCost * player.MaxActionPoints.BaseValue > player.CurrentActionPoints))
            (player.User as PlayerCharacter)!.ActiveSkills[Array.IndexOf(choices, choice)]
                .Use(player, target);
    }
    public bool PlayerMove(BattleUser player, BattleUser target)
    {
        List<string> choices =
        [
            locale.EndTurn, locale.UseSkill, locale.UsePotion + $" ({(int)(0.2 * player.MaxActionPoints.Value(player.User, "MaxActionPoints"))} {locale.ActionPointsShort})", 
            locale.ShowStats, locale.ShowStatus
        ];
        if (CanEscape)
            choices.Add(locale.Escape);
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)).Title($"{locale.ChooseNextAction} " +
                $"({player.CurrentActionPoints:F0}/{player.MaxActionPoints.Value(player.User, "MaxActionPoints"):F0} {locale.ActionPointsShort})"));
        switch (Array.IndexOf(choices.ToArray(), choice))
        {
            case 0:
                return true;
            case 1:
                ChooseSkill(player, target);
                CheckForDead();
                return false;
            case 2:
                var potion = PotionManager.ChoosePotion((player.User as PlayerCharacter).Inventory.Items
                    .Where(x => x.Key.ItemType == ItemType.Potion)
                    .Select(x => x.Key).Cast<Potion>().ToList(), false);
                if (potion == null) return false;
                player.UseActionPoints(0.2 * player.MaxActionPoints.Value(player.User, "MaxActionPoints"));
                potion.Use();
                return false;
            case 3:
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
            case 4:
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
            case 5:
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
        while (CheckForResult() == -1)
        {
            var activeSkills = enemy.User is BossEnemy boss
                ? boss.CurrentPhase == 1
                    ? [boss.ActiveSkills[0], boss.ActiveSkills[1], boss.ActiveSkills[2]]
                    : [boss.ActiveSkills[3], boss.ActiveSkills[4], boss.ActiveSkills[5]] 
                : enemy.User.ActiveSkills;
            var possibleSkills = activeSkills
                .Where(x => (x.ResourceCost <= enemy.User.CurrentResource ||
                             Math.Abs(enemy.User.MaximalResource - enemy.User.CurrentResource) < 0.01)
                            && x.ActionCost * enemy.MaxActionPoints.Value(enemy.User, "MaxActionPoints") <= enemy.CurrentActionPoints)
                .ToList();
            if (possibleSkills.Count < 1)
                break;
            var usedSkill = UtilityMethods.RandomChoice(possibleSkills);
            usedSkill.Use(enemy, target);
            CheckForDead();
            Thread.Sleep(1000);
        } 
    }

    public int CheckForResult()
    {
        if (Users.All(x => x.Value == 0))
            return 0; // Player win
        if (Users.All(x => x.Value == 1))
            return 1; // Enemy win
        if (Escaped)
            return 2; // Player escaped
        return -1; // No change
    }

    public void CheckForDead()
    {
        var dead = Users
            .Where(x => x.Key.User.CurrentHealth == 0);
        foreach (var deadUser in dead)
        {
            BattleTextService.DisplayDeathText(deadUser.Key.User);
            QuestManager.CheckForProgress(
                new QuestObjectiveContext((deadUser.Key.User as EnemyCharacter)?.Alias, deadUser.Key.User.Level));
            QuestManager.CheckForProgress(
                new QuestObjectiveContext(DungeonMovementManager.CurrentDungeon.DungeonType, 
                    DungeonMovementManager.CurrentDungeon.DungeonType, deadUser.Key.User.Level));
            Users.Remove(deadUser.Key);
        }
    }
}