using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Components;

public static class DataPersistanceManager
{
    private static readonly string dir = 
        $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/Godmist/saves/";
    public static void SaveGame()
    {
        var prompt = AnsiConsole.Prompt(new TextPrompt<string>($"{locale.ChooseNameForSave}:").Validate(n => n.Length switch
        {
            > 20 => ValidationResult.Error(locale.NameTooLong),
            <= 20 => ValidationResult.Success(),
        }));
        var json = JsonConvert.SerializeObject(PlayerHandler.player, Formatting.Indented);
        if (!Directory.Exists(dir)) Directory.CreateDirectory(dir);
        File.WriteAllText(dir + $"{prompt}.json", json);
    }

    public static void LoadGame()
    {
        var saves = Directory.GetFiles(dir).Select(save => save[dir.Length..^5]);
        AnsiConsole.Write("\n");
        var prompt = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(saves).HighlightStyle(new Style(Color.Gold3_1)));
        var json = File.ReadAllText(dir + "/" + prompt + ".json");
        PlayerHandler.player  = JsonConvert.DeserializeObject<PlayerCharacter>(json);
    }
    
    public static void DeleteSaveFile()
    {
        var saves = Directory.GetFiles(dir).Select(save => save[dir.Length..^5]);
        AnsiConsole.Write("\n");
        var prompt = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(saves).HighlightStyle(new Style(Color.Gold3_1)));
        File.Delete(dir + "/" + prompt + ".json");
    }
}