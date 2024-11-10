using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Quests;

public class CollectQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }
    public void Progress(QuestObjectiveContext context)
    {
        throw new NotImplementedException();
    }

    public List<string> ViableMonsters { get; set; }
    public string ItemToCollect { get; set; }
    public int AmountToCollect { get; set; }
}