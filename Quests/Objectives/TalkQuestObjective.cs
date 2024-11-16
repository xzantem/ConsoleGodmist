using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Quests;

public class TalkQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }

    public List<string> Dialogue { get; set; }
    public NPC NPCToTalkTo { get; set; }
    public string Description => 
        $"{locale.TalkTo} {NPCToTalkTo.Name}";
    public void Progress(QuestObjectiveContext context)
    {
        if (context.TalkTarget != NPCToTalkTo) return;
        IsComplete = true;
        foreach (var str in Dialogue)
            NPCToTalkTo.Say(str);
    }
}