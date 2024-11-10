using ConsoleGodmist.Town.NPCs;

namespace ConsoleGodmist.Quests;

public interface IQuestObjective
{
    public bool IsComplete { get; set; }
    public void Progress(QuestObjectiveContext context);
}