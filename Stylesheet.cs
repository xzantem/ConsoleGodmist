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
            { "default-cursive", new Style(Color.White, Color.Black, Decoration.Italic) },
            { "default-bold", new Style(Color.White, Color.Black, Decoration.Italic) },
            { "gold", new Style(Color.Gold3_1) },
            { "damage-true", new Style(Color.White) },
            { "damage-physical", new Style(Color.Gold1) },
            { "damage-magic", new Style(Color.Magenta2) },
            { "damage-bleed", new Style(Color.Red3) },
            { "damage-poison", new Style(Color.GreenYellow) },
            { "damage-burn", new Style(Color.DarkOrange3) },
            { "success", new Style(Color.Green1) },
            { "failure", new Style(Color.Red3) },
            { "value-lost", new Style(Color.Red3_1) },
            { "value-gained", new Style(Color.DarkSlateGray2) },
            { "highlight-good", new Style(Color.LightGoldenrod2_2, Color.Black, Decoration.Bold) },
            { "highlight-bad", new Style(Color.Red3_1, Color.Black, Decoration.Bold) },
            { "level-up", new Style(Color.Aqua, Color.Black, Decoration.Bold) },
            { "dungeon-default", new Style(Color.White) },
            { "dungeon-collect-plant", new Style(Color.Green3_1) },
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
            { "honor", new Style(Color.Teal)},
            { "honor-exile", new Style(Color.DeepPink4_2, Color.Black, Decoration.Italic)},
            { "honor-useless", new Style(Color.DeepPink3, Color.Black, Decoration.Italic)},
            { "honor-shameful", new Style(Color.DarkGoldenrod, Color.Black, Decoration.Italic)},
            { "honor-uncertain", new Style(Color.DarkKhaki, Color.Black, Decoration.Italic)},
            { "honor-citizen", new Style(Color.Grey69, Color.Black, Decoration.Italic)},
            { "honor-mercenary", new Style(Color.Chartreuse4, Color.Black, Decoration.Italic)},
            { "honor-fighter", new Style(Color.Chartreuse3, Color.Black, Decoration.Italic)},
            { "honor-knight", new Style(Color.CornflowerBlue, Color.Black, Decoration.Italic)},
            { "honor-leader", new Style(Color.MediumPurple1, Color.Black, Decoration.Italic)},
        };
    }
}