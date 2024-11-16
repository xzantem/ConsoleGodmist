using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.Town;
using ConsoleGodmist.Town.NPCs;
using Newtonsoft.Json;

namespace ConsoleGodmist.Quests;

public static class QuestManager
{
    public static List<Quest> MainQuests { get; set; }
    public static List<Quest> RandomizedSideQuests { get; set; }
    public static List<Quest> BossSideQuests { get; set; }

    public static List<Quest> Quests
    {
        get
        {
            var quests = MainQuests.ToList();
            quests.AddRange(RandomizedSideQuests);
            quests.AddRange(BossSideQuests);

            return quests;
        }
    }
    public static Dictionary<DungeonType, int> BossProgress { get; private set; }
    
    private const int QuestCount = 4;
    private const int ProgressTarget = 3;

    public static void InitMainQuests()
    {
        var path = "json/quests.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            MainQuests = JsonConvert.DeserializeObject<List<Quest>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
    
    public static void RerollSideQuests()
    {
        var level = Math.Max(Math.Min(2 + RandomizedSideQuests
                .Count(x => x.QuestState is QuestState.Completed or QuestState.HandedIn),
            BossSideQuests.Max(x => x.RecommendedLevel)), 2);
        while (RandomizedSideQuests.Count(x => x.QuestState is QuestState.Accepted or QuestState.Available) <= QuestCount)
        {
            var dungeonType = Enum.GetValues<DungeonType>()[Random.Shared.Next(0,8)];
            var randomQuest = Random.Shared.Next(0, 2) switch
            {
                0 => new Quest(dungeonType.ToString(), level, [
                        new QuestStage(dungeonType.ToString(),
                            [new KillInDungeonQuestObjective(dungeonType, Random.Shared.Next(6, 13))])
                    ], new QuestReward((int)(150 * Math.Pow(4, level / 10.0)),
                        (int)(Math.Pow(level, 1.5) + 13), 4, []), 
                    UtilityMethods.RandomChoice(TownsHandler.Arungard.NPCs.Select(x => x.Alias).ToList()),
                    "", ""),
                1 => new Quest(dungeonType.ToString(), level, [
                        new QuestStage(dungeonType.ToString(),
                            [new DescendQuestObjective(dungeonType, Random.Shared.Next(5, 11))])
                    ], new QuestReward((int)(150 * Math.Pow(4, level / 10.0)),
                        (int)(Math.Pow(level, 1.5) + 13), 4, []), 
                    UtilityMethods.RandomChoice(TownsHandler.Arungard.NPCs.Select(x => x.Alias).ToList()),
                    "", ""),
            };
            RandomizedSideQuests.Add(randomQuest);
        }
    }
    

    public static void CheckForProgress(QuestObjectiveContext context)
    {
        foreach (var quest in Quests.Where(q => q.QuestState == QuestState.Accepted))
        {
            quest.TryProgress(context);
            if (quest.QuestState != QuestState.Completed) continue;
            InitSideQuests();
        }
    }

    public static void InitSideQuests()
    {
        RandomizedSideQuests ??= [];
        BossSideQuests ??= [];
        RerollSideQuests();
        UpdateBossQuests();
    }

    public static void UpdateBossQuests()
    {
        foreach (var progress in BossProgress)
        {
            if (progress.Value < ProgressTarget) continue;
            var target = GetBossQuestTarget(progress.Key, progress.Value);
            if (!BossSideQuests.Any(x => x.Stages.Any(s => s.Objectives
                    .Any(o => o.GetType() == typeof(KillQuestObjective) &&
                              ((KillQuestObjective)o).Target == target)))) continue;
            {
                var quest = new Quest(target, GetLevel(BossSideQuests.Count),
                    [new QuestStage("", [new KillQuestObjective(target, 1)])],
                    new QuestReward(0, 0, 0, []), 
                    UtilityMethods.RandomChoice(TownsHandler.Arungard.NPCs.Select(x => x.Alias).ToList()),
                    "", "");
                BossSideQuests.Add(quest);
            }
        }
        return;
        int GetLevel(int n)
        {
            if (n < 1) throw new ArgumentException("n must be greater than or equal to 1.");
            var value = 6;
            for (var i = 2; i <= n; i++)
            {
                if (i == 9) value += 6;
                else if (i % 2 == 0) value += 2;
                else value += 3;
            }
            return value;
        }
    }
    
    public static string GetBossQuestTarget(DungeonType dungeon, int stage)
    {
        return (dungeon, stage) switch
        {
            (DungeonType.Catacombs, 1) => "SkeletonAssassin",
            (DungeonType.Catacombs, 2) => "DeadKingGhost",
            (DungeonType.Forest, 1) => "InfectedEnt",
            (DungeonType.Forest, 2) => "BanditLeader",
            (DungeonType.ElvishRuins, 1) => "DemonicJarl",
            (DungeonType.ElvishRuins, 2) => "Cerberus",
            (DungeonType.Cove, 1) => "AlphaShark",
            (DungeonType.Cove, 2) => "PirateCaptain",
            (DungeonType.Desert, 1) => "AvazarLeader",
            (DungeonType.Desert, 2) => "AlphaScorpionRider",
            (DungeonType.Temple, 1) => "Hierophant",
            (DungeonType.Temple, 2) => "Java",
            (DungeonType.Mountains, 1) => "CrushedSkullBerserker",
            (DungeonType.Mountains, 2) => "Wyvern",
            (DungeonType.Swamp, 1) => "SwampEnt",
            (DungeonType.Swamp, 2) => "ManeaterPlant",
        };
    }
}