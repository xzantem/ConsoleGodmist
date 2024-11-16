using ConsoleGodmist.Town.NPCs;

namespace ConsoleGodmist.Quests;

public interface IQuestObjective
{
    public bool IsComplete { get; set; }
    public string Description { get; }
    public void Progress(QuestObjectiveContext context);
}