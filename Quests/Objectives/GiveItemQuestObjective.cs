using ConsoleGodmist.Town.NPCs;

namespace ConsoleGodmist.Quests;

public class GiveItemQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }
    public void Progress(QuestObjectiveContext context)
    {
        throw new NotImplementedException();
    }

    public string ItemToGive { get; set; }
    public NPC NPCToGive { get; set; }
    public int QuantityToGive { get; set; }
}