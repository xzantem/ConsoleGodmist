using ConsoleGodmist.Enums;
using ConsoleGodmist.Town.NPCs;
using Newtonsoft.Json;

namespace ConsoleGodmist.Quests;

public static class QuestManager
{
    public static List<Quest> Quests { get; private set; }

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

    public static List<Quest> GetAvailableQuests(NPC questGiver)
    {
        var quests = new List<Quest>();
        foreach (var quest in Quests)
        {
            if (quest.QuestGiver != questGiver || quest.QuestState != QuestState.Available) continue;
            foreach (var prerequisite in Quests)
            {
                if (!quest.Prerequisites.Contains(prerequisite.Alias))
                    continue;
                if (prerequisite.QuestState != QuestState.Completed) break; 
                quests.Add(quest);
            }
        }
        return quests;
    }
    public static List<Quest> GetReturnableQuests(NPC questEnder)
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