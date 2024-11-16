using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Town;

namespace ConsoleGodmist.Quests;

public static class SideQuestManager
{
    public static Quest[] CurrentSideQuests { get; private set; }
    public static Dictionary<DungeonType, int> BossProgress { get; private set; }
    public static List<Quest> BossQuests { get; private set; }
    public static void RerollQuests(int level)
    {
        CurrentSideQuests = new Quest[6];
        for (var i = 0; i < 6; i++)
        {
            var dungeonType = Enum.GetValues<DungeonType>()[Random.Shared.Next(0,8)];
            var randomQuest = Random.Shared.Next(0, 2) switch
            {
                0 => new Quest(dungeonType.ToString(), level, [
                        new QuestStage(dungeonType.ToString(),
                            [new KillInDungeonQuestObjective(dungeonType, Random.Shared.Next(6, 13))])
                    ], new QuestReward((int)(150 * Math.Pow(4, level / 10.0)),
                        (int)(Math.Pow(level, 1.5) + 13), 4, []), TownsHandler.Arungard.NPCs[i].Alias,
                    "", ""),
                1 => new Quest(dungeonType.ToString(), level, [
                        new QuestStage(dungeonType.ToString(),
                            [new DescendQuestObjective(dungeonType, Random.Shared.Next(5, 11))])
                    ], new QuestReward((int)(150 * Math.Pow(4, level / 10.0)),
                        (int)(Math.Pow(level, 1.5) + 13), 4, []), TownsHandler.Arungard.NPCs[i].Alias,
                    "", ""),
            };
            CurrentSideQuests[i] = randomQuest;
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