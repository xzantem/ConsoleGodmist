using Spectre.Console;

namespace ConsoleGodmist;

public static class Stylesheet
{
    public static Dictionary<string, Style> Styles { get; private set; }

    public static void InitStyles()
    {
        Styles = new Dictionary<string, Style>()
        {
            { "default", new Style(Color.White) },
            { "gold", new Style(Color.Gold1) },
            { "value-lost", new Style(Color.Red3_1) },
            { "value-gained", new Style(Color.DarkSlateGray2) },
            { "level-up", new Style(Color.Aqua, Color.Black, Decoration.Bold) },
            { "dungeon-default", new Style(Color.White) },
            { "dungeon-icon-battle", new Style(Color.Red) },
            { "dungeon-icon-plant", new Style(Color.Green) },
            { "dungeon-icon-stash", new Style(Color.Tan) },
            { "dungeon-icon-trap", new Style(Color.Purple) },
            { "dungeon-icon-unrevealed", new Style(Color.Grey) },
            { "dungeon-icon-campfire", new Style(Color.DarkRed) },
            { "dungeon-icon-exit", new Style(Color.Aqua) },
            { "error", new Style(Color.DarkRed) },
            { "rarity-destroyed", new Style(Color.Maroon)},
            { "rarity-damaged", new Style(Color.DarkRed_1)},
            { "rarity-junk", new Style(Color.Orange4)},
            { "rarity-common", new Style(Color.Silver)},
            { "rarity-uncommon", new Style(Color.Green3)},
            { "rarity-rare", new Style(Color.DodgerBlue1)},
            { "rarity-ancient", new Style(Color.LightGoldenrod2_2)},
            { "rarity-legendary", new Style(Color.Purple_2)},
            { "rarity-mythical", new Style(Color.Cyan1, Color.Black, Decoration.Italic)},
            { "rarity-godly", new Style(Color.Fuchsia, Color.Black, Decoration.Bold)},
        };
    }
}