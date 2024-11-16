using ConsoleGodmist.Enums;
using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Quests;

public class DescendQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }

    public DungeonType Target { get; set; }
    public int FloorToReach { get; set; }
    public string Description => 
        $"{locale.Descend} {FloorToReach} {locale.FloorIn} {NameAliasHelper.GetDungeonType(Target, "Locative")}";
    public DescendQuestObjective()
    {
        IsComplete = false;
    }

    public DescendQuestObjective(DungeonType dungeon, int floorToReach)
    {
        Target = dungeon;
        FloorToReach = floorToReach;
        IsComplete = false;
    }
    public void Progress(QuestObjectiveContext context)
    {
        if (context.DescendTarget == Target && FloorToReach == context.DescendFloor)
            IsComplete = true;
    }
}