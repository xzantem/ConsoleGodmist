using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
using Spectre.Console;

namespace ConsoleGodmist.Quests;

public static class QuestLog
{
    private static readonly List<QuestState> Filters = [QuestState.Accepted, QuestState.Completed, QuestState.HandedIn];
    public static void InspectQuest(Quest quest)
    {
        AnsiConsole.Write(new Text($"{quest.Name} - {NameAliasHelper.GetName(quest.QuestGiver)} " +
                                   $"({locale.Level} {quest.RecommendedLevel})\n{quest.Description}\n", Stylesheet.Styles["gold"]));
        foreach (var stage in quest.Stages)
        {
            AnsiConsole.Write(new Text($"- {stage.Name}\n", 
                stage.Objectives.All(x => x.IsComplete) ? 
                    Stylesheet.Styles["quest-completed"] : Stylesheet.Styles["default"]));
            foreach (var objective in stage.Objectives)
                AnsiConsole.Write(new Text($"\t- {objective.Description}\n", 
                    objective.IsComplete ? Stylesheet.Styles["quest-completed"] : Stylesheet.Styles["default"]));
            if (stage.Objectives.All(x => x.IsComplete)) continue;
            AnsiConsole.Write("\n");return;
        }
        AnsiConsole.Write(new Text($"{locale.TalkTo} {quest.QuestEnder}\n\n", 
            quest.QuestState == QuestState.HandedIn ? Stylesheet.Styles["quest-completed"]: Stylesheet.Styles["default"]));
    }

    public static void OpenLog()
    {
        var quests = QuestManager.Quests.Where(x => x.QuestState != QuestState.Available)
            .OrderBy(x => x.QuestState).ToList();
        const int scrollAmount = 10;
        const int pageSize = 10;
        var index = 0;
        while (true)
        {
            
            var rows = quests.Where(x => Filters.Contains(x.QuestState)).Select(x => 
                new Text($"{NameAliasHelper.GetName(x.QuestState.ToString())} - " +
                         $"{x.Name} ({x.RecommendedLevel})")).ToList();
            if (rows.Count == 0) { AnsiConsole.Write(new Text(locale.QuestListEmpty, Stylesheet.Styles["default"])); }
            AnsiConsole.Write(new Rows(rows.GetRange(index, Math.Min(pageSize, rows.Count - index))));
            AnsiConsole.Write("\n\n");
            Dictionary<string, int> choices = [];
            if (index < rows.Count - scrollAmount)
                choices.Add(locale.GoDown, 0);
            if (index >= scrollAmount)
                choices.Add(locale.GoUp, 1);
            choices.Add(locale.InspectQuest, 2);
            choices.Add(locale.ChangeFilter, 3);
            choices.Add(locale.Return, 4);
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices.Keys)
                .HighlightStyle(new Style(Color.Gold3_1)).WrapAround());
            switch (choices[choice])
            {
                case 0: index += scrollAmount; break;
                case 1: index -= scrollAmount; break;
                case 2:
                    try { InspectQuest(quests[ChooseQuest(quests)]); }
                    catch { AnsiConsole.Write(new Text("", Stylesheet.Styles["error"])); }
                    break;
                case 3: ChangeFilters(); break;
                case 4: return;
            }
        }
    }

    private static int ChooseQuest(List<Quest> selection)
    {
        if (selection.Count <= 1) return 0;
        var choices = selection.Select(x => x.Name).ToList();
        choices.Add(locale.Return);
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices).WrapAround());
        return choices.IndexOf(choice);
    }

    private static void ChangeFilters()
    {
        Filters.Clear();
        var str = AnsiConsole.Prompt(new MultiSelectionPrompt<string>()
            .AddChoices([locale.Accepted, locale.Completed, locale.HandedIn]));
        if (str.Contains(locale.Accepted)) Filters.Add(QuestState.Accepted);
        if (str.Contains(locale.Completed)) Filters.Add(QuestState.Completed);
        if (str.Contains(locale.HandedIn)) Filters.Add(QuestState.HandedIn);
    }
}