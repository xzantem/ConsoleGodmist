
using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;

namespace ConsoleGodmist.Quests;

[JsonConverter(typeof(QuestObjectiveConverter))]
public class DescendQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }

    public DungeonType Target { get; set; }
    public int FloorToReach { get; set; }
    public string Description => 
        $"{locale.Descend} {FloorToReach} {locale.In} {NameAliasHelper.GetDungeonType(Target, "Locative")}";
    
    [JsonConstructor]
    public DescendQuestObjective()
    {
    }

    public DescendQuestObjective(DungeonType dungeon, int floorToReach)
    {
        Target = dungeon;
        FloorToReach = floorToReach;
        IsComplete = false;
    }
    public void Progress(QuestObjectiveContext context)
    {
        if (context.DescendTarget != null && context.DescendTarget == Target && FloorToReach == context.DescendFloor)
            
            IsComplete = true;
    }
}