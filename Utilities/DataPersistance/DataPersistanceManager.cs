using System.IO;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Quests;
using ConsoleGodmist.Town;
using ConsoleGodmist.Town.NPCs;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Utilities;

public static class DataPersistanceManager
{
    private static readonly string dir = 
        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Godmist/saves/";
    public static void SaveGame(SaveData data)
    {
        var prompt = AnsiConsole.Prompt(new TextPrompt<string>($"{locale.ChooseNameForSave}:").Validate(n => n.Length switch
        {
            > 20 => ValidationResult.Error(locale.NameTooLong),
            <= 20 => ValidationResult.Success(),
        }));
        var json = JsonConvert.SerializeObject(data, Formatting.Indented);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(dir + $"{prompt}.json", json);
    }

    public static bool LoadGame()
    {
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var saves = Directory.GetFiles(dir).Select(save => save[dir.Length..^5]).ToArray();
        if (saves.Length == 0)
        {
            AnsiConsole.WriteLine(locale.NoSavesFound);
            return false;
        }
        var choices = saves.Append(locale.Return).ToArray();
        AnsiConsole.Write("\n");
        var prompt = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices).HighlightStyle(new Style(Color.Gold3_1)).WrapAround());
        if (prompt == locale.Return) return false;
        var json = File.ReadAllText(dir + "/" + prompt + ".json");
        var data = JsonConvert.DeserializeObject<SaveData>(json, new QuestObjectiveConverter(), new NPCConverter());
        PlayerHandler.player = data.Player;
        GameSettings.Difficulty = data.Difficulty;
        QuestManager.MainQuests = data.Quests[0];
        QuestManager.RandomizedSideQuests = data.Quests[1];
        QuestManager.BossSideQuests = data.Quests[2];
        QuestManager.BossProgress = data.BossQuestProgress;
        TownsHandler.Arungard = data.Town;
        return true;
    }
    
    
    public static void DeleteSaveFile()
    {
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        var saves = Directory.GetFiles(dir).Select(save => save[dir.Length..^5]).ToArray();
        if (saves.Length == 0)
        {
            AnsiConsole.WriteLine(locale.NoSavesFound);
            return;
        }
        var choices = saves.Append(locale.Return).ToArray();
        AnsiConsole.Write("\n");
        var prompt = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices).HighlightStyle(new Style(Color.Gold3_1)).WrapAround());
        if (prompt == locale.Return) return;
        File.Delete(dir + "/" + prompt + ".json");
    }
}