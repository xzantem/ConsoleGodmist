using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;

namespace ConsoleGodmist.Quests;

[JsonConverter(typeof(QuestObjectiveConverter))]
public class KillInDungeonQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }

    public string Description =>
        $"{locale.Kill} {locale.EnemiesIn} {NameAliasHelper.GetDungeonType(Target, "Locative")} " +
        $"({QuestProgress}/{AmountToKill})";
    public DungeonType Target { get; set; }
    public int AmountToKill { get; set; }
    public int QuestProgress { get; private set; }

    [JsonConstructor]
    public KillInDungeonQuestObjective()
    {
    }

    public KillInDungeonQuestObjective(DungeonType dungeon, int amountToKill)
    {
        Target = dungeon;
        AmountToKill = amountToKill;
        QuestProgress = 0;
        IsComplete = false;
    }

    public void Progress(QuestObjectiveContext context)
    {
        if (context.KillInDungeonTarget == null || context.KillInDungeonTarget != Target) return;
        QuestProgress++;
        if (QuestProgress >= AmountToKill)
            IsComplete = true;
    }
}