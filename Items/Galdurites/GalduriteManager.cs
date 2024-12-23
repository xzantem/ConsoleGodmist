using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public static class GalduriteManager
{
    public static List<GalduriteComponent> GalduriteComponents { get; private set; }
    
    public static void InitComponents()
    {
        var path = "json/galdurite-components.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            GalduriteComponents = JsonConvert.DeserializeObject<List<GalduriteComponent>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }

    public static GalduriteComponent GetGalduriteComponent(string tier, string color, byte type, HashSet<string> excludedColors)
    {
        if (color == "Random")
        {
            return UtilityMethods.RandomChoice(GalduriteComponents.Where(x =>
                x.EffectTier == tier && x.EquipmentType == type && 
                !excludedColors.Contains(x.PoolColor)).ToList());
        }
        return UtilityMethods.RandomChoice(GalduriteComponents.Where(x =>
            x.EffectTier == tier && x.PoolColor == color && x.EquipmentType == type && 
            !excludedColors.Contains(x.PoolColor)).ToList());
    }
    
    public static Galdurite? ChooseGaldurite(List<Galdurite> galdurites)
    {
        if (galdurites.Count == 0) return null;
        var gals = galdurites.Select(x => $"{x.Name} - {NameAliasHelper.GetName
            (x.ItemType == ItemType.WeaponGaldurite ? "Weapon" : "Armor")} ({x.RarityName()})").ToArray();
        var choices = gals.Append(locale.Return).ToArray();
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        return choice == locale.Return ? null : galdurites.ElementAt(Array.IndexOf(choices, choice));
    }
}