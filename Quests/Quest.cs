using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Town;
using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;

namespace ConsoleGodmist.Quests;

public class Quest
{
    [JsonIgnore]
    public string Name => NameAliasHelper.GetName(Alias);
    [JsonIgnore]
    public string Description => NameAliasHelper.GetName(Alias + "Description");
    public string Alias { get; set; }
    public int RecommendedLevel { get; set; }
    public List<QuestStage> Stages { get; set; }
    public QuestReward QuestReward { get; set; }
    public QuestState QuestState { get; set; }
    public List<string> Prerequisites { get; set; }
    public string QuestGiver { get; set; }
    public string QuestEnder { get; set; }
    public List<string> AcceptDialogue { get; set; }
    public List<string> HandInDialogue { get; set; }
    
    public Quest() {}

    public Quest(string alias, int level, List<QuestStage> stages, QuestReward questReward, string questGiver,
        string acceptDialogue, string handInDialogue)
    {
        Alias = alias;
        RecommendedLevel = level;
        Stages = stages;
        QuestReward = questReward;
        QuestState = QuestState.Available;
        Prerequisites = new List<string>();
        QuestGiver = questGiver;
        QuestEnder = questGiver;
        AcceptDialogue = new List<string> { acceptDialogue };
        HandInDialogue = new List<string> { handInDialogue };
    }

    public void TryProgress(QuestObjectiveContext context)
    {
        if (context.ContextLevel < RecommendedLevel || QuestState != QuestState.Accepted) return;
        foreach (var objective in GetCurrentStage().Objectives.Where(objective => !objective.IsComplete))
        {
            objective.Progress(context);
        }
        if (GetCurrentStage().Objectives.All(o => o.IsComplete))
            TryCompleteQuest();
    }

    public QuestStage GetCurrentStage()
    {
        return Stages.FirstOrDefault(stage => stage.Objectives.Any(objective => !objective.IsComplete))!;
    }

    public void AcceptQuest()
    {
        QuestState = QuestState.Accepted;
        foreach (var str in AcceptDialogue)
            TownsHandler.FindNPC(QuestGiver).Say(str);
    }

    public void TryCompleteQuest()
    {
        if (QuestState != QuestState.Accepted || Stages.Count <= 0 || 
            !Stages.All(s => s.Objectives.All(o => o.IsComplete))) return;
        QuestState = QuestState.Completed;
    }

    public void HandInQuest()
    {
        GetRewards();
        QuestState = QuestState.HandedIn;
        foreach (var str in HandInDialogue)
            TownsHandler.FindNPC(QuestEnder).Say(str);
    }

    private void GetRewards()
    {
        var player = PlayerHandler.player;
        if (QuestReward.Gold != 0) player.GainGold(QuestReward.Gold);
        if (QuestReward.Experience != 0) player.GainExperience(QuestReward.Experience);
        if (QuestReward.Honor !=  0) player.GainHonor(QuestReward.Honor);
        if (QuestReward.Items.Count == 0) return;
        foreach (var item in QuestReward.Items)
            player.Inventory.AddItem(item.Key, item.Value);
    }
}