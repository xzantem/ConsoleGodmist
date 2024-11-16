using ConsoleGodmist.Enums;
using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Quests;

public class KillInDungeonQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }

    public string Description =>
        $"{locale.Kill} {locale.EnemiesIn} {NameAliasHelper.GetDungeonType(Target, "Locative")} " +
        $"({QuestProgress}/{AmountToKill})";
    public DungeonType Target { get; set; }
    public int AmountToKill { get; set; }
    public int QuestProgress { get; private set; }

    public KillInDungeonQuestObjective()
    {
        QuestProgress = 0;
        IsComplete = false;
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
        if (context.KillInDungeonTarget != Target) return;
        QuestProgress++;
        if (QuestProgress >= AmountToKill)
            IsComplete = true;
    }
}