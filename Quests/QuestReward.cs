using ConsoleGodmist.Items;

namespace ConsoleGodmist.Quests;

public class QuestReward
{
    public int Gold { get; set; }
    public int Experience { get; set; }
    public int Honor { get; set; }
    public Dictionary<IItem, int> Items { get; set; }
    
    public QuestReward() {}
}