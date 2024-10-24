using ConsoleGodmist.Characters;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.TextService;

namespace ConsoleGodmist.Combat.Battles;

public static class BattleManager
{
    public static Battle CurrentBattle { get; private set; }
    
    public static void StartNewBattle(Dictionary<BattleUser, int> usersTeams)
    {
        var initial = usersTeams
            .ToDictionary(c => c.Key, c => c.Value);
        CurrentBattle = new Battle(usersTeams);
        BattleTextService.DisplayBattleStartText(usersTeams.ElementAt(^1).Key.User as EnemyCharacter);
        while(CurrentBattle.CheckForResult() == -1)
            CurrentBattle.NewTurn();
        var battleResult = CurrentBattle.CheckForResult() switch
        {
            0 => true,
            1 => false,
            _ => false
        };
        if (CurrentBattle.CheckForResult() == 2)
            return;
        BattleTextService.DisplayEndBattleText(battleResult);
        Thread.Sleep(2000);
        if (!battleResult)
            Environment.Exit(0);
        GenerateReward(initial);
    }

    private static void GenerateReward(Dictionary<BattleUser, int> usersTeams)
    {
        var player = usersTeams.ElementAt(0).Key.User as PlayerCharacter;
        var dungeon = DungeonMovementManager.CurrentDungeon;
        var moneyReward = 0;
        var honorReward = 0;
        var experienceReward = 0;
        foreach (var enemy in usersTeams.Where(x => x.Value == 1))
        {
            moneyReward += (int)(Math.Pow(4, (dungeon.DungeonLevel - 1) * 0.1) * 
                Random.Shared.Next(15, 31) * 
                (10 * dungeon.Floors.IndexOf(dungeon.CurrentFloor) + 7)) / 
                           (dungeon.Floors.IndexOf(dungeon.CurrentFloor) + 16);
            if (enemy.Key.User.Level >= usersTeams.Keys.Average(x => x.User.Level))
                honorReward++;
            experienceReward += (int)Math.Max(0.1, (1 - 0.15 * Math.Abs(player.Level - dungeon.DungeonLevel)) 
                                                   * (Math.Pow(dungeon.DungeonLevel, 1.1) + 3));
        }
        player.GainGold(moneyReward);
        player.GainHonor(honorReward);
        player.GainExperience(experienceReward);
    }
}
