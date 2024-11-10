using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Quests;

public class QuestStage
{
    public List<IQuestObjective> Objectives { get; set; }
    
    public string Alias { get; set; }
    public string Name => NameAliasHelper.GetName(Alias);
    public string Description => NameAliasHelper.GetName(Alias + "Description");
    
    public QuestStage() {}
}