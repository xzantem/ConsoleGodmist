using ConsoleGodmist.Enums;
using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Quests;

public class DescendQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }

    public DungeonType Target { get; set; }
    public int FloorToReach { get; set; }
    public void Progress(QuestObjectiveContext context)
    {
        if (context.DescendTarget == Target && FloorToReach == context.DescendFloor)
            IsComplete = true;
    }
}