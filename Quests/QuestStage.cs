using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;

namespace ConsoleGodmist.Quests;

public class QuestStage
{
    public List<IQuestObjective> Objectives { get; set; }
    
    public string Alias { get; set; }
    [JsonIgnore]
    public string Name => NameAliasHelper.GetName(Alias);
    [JsonIgnore]
    public string Description => NameAliasHelper.GetName(Alias + "Description");
    
    public QuestStage() {}

    public QuestStage(string alias, List<IQuestObjective> objectives)
    {
        Alias = alias;
        Objectives = objectives;
    }
    
}