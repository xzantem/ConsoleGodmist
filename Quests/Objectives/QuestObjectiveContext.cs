using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Town.NPCs;

namespace ConsoleGodmist.Quests;

public class QuestObjectiveContext
{
    public string KillTarget { get; private set; }
    public NPC? TalkTarget { get; private set; }
    public DungeonType DescendTarget { get; private set; }
    public int DescendFloor { get; private set; }
    public int ContextLevel { get; private set; }

    public QuestObjectiveContext(string killTarget, int contextLevel)
    {
        KillTarget = killTarget;
        ContextLevel = contextLevel;
        TalkTarget = null;
        DescendTarget = default;
        DescendFloor = 0;
    }
    
    public QuestObjectiveContext(NPC talkTarget)
    {
        KillTarget = "";
        ContextLevel = int.MaxValue;
        TalkTarget = talkTarget;
        DescendTarget = default;
        DescendFloor = 0;
    }
    
    public QuestObjectiveContext(DungeonType descendTarget, int descendFloor, int contextLevel)
    {
        KillTarget = "";
        ContextLevel = contextLevel;
        TalkTarget = null;
        DescendTarget = descendTarget;
        DescendFloor = descendFloor;
    }
}