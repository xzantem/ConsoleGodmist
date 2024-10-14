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
            { "dungeon-default", new Style(Color.White) },
            { "dungeon-icon-battle", new Style(Color.Red) },
            { "dungeon-icon-plant", new Style(Color.Green) },
            { "dungeon-icon-stash", new Style(Color.Tan) },
            { "dungeon-icon-trap", new Style(Color.Purple) },
            { "dungeon-icon-unrevealed", new Style(Color.Grey) },
            { "dungeon-icon-campfire", new Style(Color.DarkRed) },
            { "dungeon-icon-exit", new Style(Color.Aqua) }
        };
    }
}