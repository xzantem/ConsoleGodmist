using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Quests;

public static class QuestNPCHandler
{
    public static List<Quest> GetAvailableQuests(string questGiver)
    {
        var quests = new List<Quest>();
        foreach (var quest in QuestManager.Quests
                     .Where(quest => quest.QuestGiver == questGiver && quest.QuestState == QuestState.Available))
        {
            if (quest.Prerequisites.Count == 0 && quest.QuestState == QuestState.Available)
            {
                quests.Add(quest);
                continue;
            }
            quests.AddRange(QuestManager.Quests.Where(prerequisite => quest.Prerequisites
                    .Contains(prerequisite.Alias))
                .TakeWhile(prerequisite => prerequisite.QuestState is QuestState.Completed or QuestState.HandedIn)
                .Select(prerequisite => quest));
        }
        return quests;
    }
    public static List<Quest> GetReturnableQuests(string questEnder)
    {
        return QuestManager.Quests
            .Where(quest => quest.QuestEnder == questEnder && quest.QuestState == QuestState.Completed).ToList();
    }

    public static void SelectQuestToAccept(string questGiver)
    {
        while (true)
        {
            var availableQuests = GetAvailableQuests(questGiver);
            if (availableQuests.Count == 0) return;
            var choices = availableQuests.Select(x => x.Name).ToList();
            choices.Add(locale.Return);
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices));
            if (choices.IndexOf(choice) == choices.Count - 1) return;
            var quest = availableQuests[choices.IndexOf(choice)];
            AnsiConsole.Write(new Text($"{quest.Name} ({locale.Level} {quest.RecommendedLevel})\n{quest.Description}\n"));
            var accepted = UtilityMethods.Confirmation(locale.WantAcceptQuestThird, true);
            if (accepted) quest.AcceptQuest();
        }
    }
    public static void SelectQuestToReturn(string questEnder)
    {
        while (true)
        {
            var returnableQuests = GetReturnableQuests(questEnder);
            if (returnableQuests.Count == 0) return;
            var choices = returnableQuests.Select(x => x.Name).ToList();
            choices.Add(locale.Return);
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices));
            if (choices.IndexOf(choice) == choices.Count - 1) return;
            var quest = returnableQuests[choices.IndexOf(choice)];
            var accepted = UtilityMethods.Confirmation(locale.WantHandInQuestThird, true);
            if (accepted) quest.HandInQuest();
        }
    }
}