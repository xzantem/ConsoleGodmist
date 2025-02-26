using System.ComponentModel;
using System.IO;
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

    public static GalduriteComponent GetGalduriteComponent(string tier, string color, bool type, HashSet<string> excludedColors)
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

    public static string GetColorMaterial(int tier, string color)
    {
        return (tier, color) switch
        {
            (1, "Red") => "RedPowder",
            (2, "Red") => "CondensedRedPowder",
            (3, "Red") => "MagicalRedPowder",
            (1, "Blue") => "BluePowder",
            (2, "Blue") => "CondensedBluePowder",
            (3, "Blue") => "MagicalBluePowder",
            (1, "Purple") => "PurplePowder",
            (2, "Purple") => "CondensedPurplePowder",
            (3, "Purple") => "MagicalPurplePowder",
            (1, "Yellow") => "YellowPowder",
            (2, "Yellow") => "CondensedYellowPowder",
            (3, "Yellow") => "MagicalYellowPowder",
            (1, "Green") => "GreenPowder",
            (2, "Green") => "CondensedGreenPowder",
            (3, "Green") => "MagicalGreenPowder",
            (1, "Black") => "BlackPowder",
            (2, "Black") => "CondensedBlackPowder",
            (3, "Black") => "MagicalBlackPowder",
            (1, "Pink") => "PinkPowder",
            (2, "Pink") => "CondensedPinkPowder",
            (3, "Pink") => "MagicalPinkPowder",
            (1, "White") => "WhitePowder",
            (2, "White") => "CondensedWhitePowder",
            (3, "White") => "MagicalWhitePowder",
            (1, "Golden") => "GoldenPowder",
            (2, "Golden") => "CondensedGoldenPowder",
            (3, "Golden") => "MagicalGoldenPowder",
            (1, "Orange") => "OrangePowder",
            (2, "Orange") => "CondensedOrangePowder",
            (3, "Orange") => "MagicalOrangePowder",
            _ => ""
        };
    }
    
    public static Galdurite? ChooseGaldurite(List<Galdurite> galdurites)
    {
        if (galdurites.Count == 0) return null;
        var gals = galdurites.Select(x => x.Revealed ? $"{x.Name} ({x.RarityName()}): \n{string
            .Join("", x.Components.Select(c => $"{c.EffectTier} - {c.EffectText}\n"))}" : $"{x.Name} ({x.RarityName()})").ToArray();
        var choices = gals.Append(locale.Return).ToArray();
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)).WrapAround());
        return choice == locale.Return ? null : galdurites.ElementAt(Array.IndexOf(choices, choice));
    }
}