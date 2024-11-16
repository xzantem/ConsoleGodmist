using ConsoleGodmist.Enums;
using ConsoleGodmist.Town;
using ConsoleGodmist.Town.NPCs;
using Newtonsoft.Json;

namespace ConsoleGodmist.Quests;

public static class MainQuestManager
{
    public static List<Quest> Quests { get; set; }

    public static void InitQuests()
    {
        var path = "json/quests.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            Quests = JsonConvert.DeserializeObject<List<Quest>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }

    public static List<Quest> GetAvailableQuests(string questGiver)
    {
        var quests = new List<Quest>();
        foreach (var quest in Quests
                     .Where(quest => quest.QuestGiver == questGiver && quest.QuestState == QuestState.Available))
        {
            if (quest.Prerequisites.Count == 0 && quest.QuestState == QuestState.Available)
            {
                quests.Add(quest);
                continue;
            }
            quests.AddRange(Quests.Where(prerequisite => quest.Prerequisites
                .Contains(prerequisite.Alias))
                .TakeWhile(prerequisite => prerequisite.QuestState is QuestState.Completed or QuestState.HandedIn)
                .Select(prerequisite => quest));
        }
        return quests;
    }
    public static List<Quest> GetReturnableQuests(string questEnder)
    {
        return Quests
            .Where(quest => quest.QuestEnder == questEnder && quest.QuestState == QuestState.Completed).ToList();
    }

    public static void CheckForProgress(QuestObjectiveContext context)
    {
        foreach (var quest in Quests.Where(q => q.QuestState == QuestState.Accepted))
            quest.TryProgress(context);
    }
}