using ConsoleGodmist.Characters;
using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Quests;

public class KillQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }
    public string Target { get; set; }
    public int AmountToKill { get; set; }
    public int QuestProgress { get; private set; }
    public string Description => 
        $"{locale.Kill} {EnemyFactory.EnemiesList.Find(x => x.Alias == Target).Name} " +
        $"({QuestProgress/AmountToKill})";

    public void Progress(QuestObjectiveContext context)
    {
        if (context.KillTarget != Target) return;
        QuestProgress++;
        if (QuestProgress >= AmountToKill)
            IsComplete = true;
    }
}