using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class DungeonTextService
{
    public static void DisplayDungeonHeader(Dungeon dungeon)
    {
        var locationText = LocationText(dungeon);
        var color = dungeon.DungeonType switch
        {
            DungeonType.Catacombs => Color.Grey37,
            DungeonType.Forest => Color.DarkSeaGreen4_1,
            DungeonType.ElvishRuins => Color.Orange4_1,
            DungeonType.Cove => Color.DeepSkyBlue3,
            DungeonType.Desert => Color.Tan,
            DungeonType.Temple => Color.Gold1,
            DungeonType.Mountains => Color.Grey70,
            DungeonType.Swamp => Color.DarkGreen,
            _ => Color.White,
        };
        AnsiConsole.Write(new FigletText(locationText).Color(color));
    }

    public static void DisplayDungeonFloor(Dungeon dungeon)
    {
        var locationText = LocationText(dungeon);
        var floor = dungeon.Floors.IndexOf(dungeon.CurrentFloor) == 0
            ? dungeon.Floors.IndexOf(dungeon.CurrentFloor).ToString()
            : "-" + dungeon.Floors.IndexOf(dungeon.CurrentFloor);
        var visited = dungeon.Floors.Count - 1 == 0
            ? (dungeon.Floors.Count - 1).ToString()
            : "-" + (dungeon.Floors.Count - 1);
        locationText = locale.Level + " " + dungeon.DungeonLevel + ", " + locale.Floor + " " + floor + " [" +
                       visited + "]\n" + locale.Map + ":";
        AnsiConsole.WriteLine(locationText);
    }

    private static string LocationText(Dungeon dungeon)
    {
        var locationText = dungeon.DungeonType switch
        {
            DungeonType.Catacombs => locale.Catacombs,
            DungeonType.Forest => locale.Forest,
            DungeonType.ElvishRuins => locale.ElvishRuins,
            DungeonType.Cove => locale.Cove,
            DungeonType.Desert => locale.Desert,
            DungeonType.Temple => locale.Temple,
            DungeonType.Mountains => locale.Mountains,
            DungeonType.Swamp => locale.Swamp,
            _ => locale.Catacombs,
        };
        return locationText;
    }

    public static void DisplayDungeonMap(Dungeon dungeon, int locationIndex, DungeonRoom currentLocation)
    {
        if (dungeon.CurrentFloor.StarterRoom.Revealed)
        {
            switch (dungeon.CurrentFloor.StarterRoom.FieldType)
            {
                case DungeonFieldType.Battle:
                    AnsiConsole.Write(new Text("[W]", Stylesheet.Styles["dungeon-icon-battle"]));
                    break;
                case DungeonFieldType.Empty:
                    AnsiConsole.Write(new Text("[Z]", Stylesheet.Styles["dungeon-icon-exit"]));
                    break;
            }
        }
        else
        {
            AnsiConsole.Write(new Text("[?]", Stylesheet.Styles["dungeon-default"]));
        }
        Console.Write("--");
        foreach (var corridor in dungeon.CurrentFloor.Corridor)
        {
            if (corridor.Revealed)
            {
                switch (corridor.FieldType)
                {
                    case DungeonFieldType.Battle:
                        AnsiConsole.Write(new Text("[W]", Stylesheet.Styles["dungeon-icon-battle"]));
                        break;
                    case DungeonFieldType.Stash:
                        AnsiConsole.Write(new Text("[S]", Stylesheet.Styles["dungeon-icon-stash"]));
                        break;
                    case DungeonFieldType.Bonfire:
                        AnsiConsole.Write(new Text("[O]", Stylesheet.Styles["dungeon-icon-campfire"]));
                        break;
                    case DungeonFieldType.Trap:
                        AnsiConsole.Write(new Text("[P]", Stylesheet.Styles["dungeon-icon-trap"]));
                        break;
                    case DungeonFieldType.Empty:
                        AnsiConsole.Write(new Text("[X]", Stylesheet.Styles["dungeon-default"]));
                        break;
                    case DungeonFieldType.Plant:
                        AnsiConsole.Write(new Text("[R]", Stylesheet.Styles["dungeon-icon-plant"]));
                        break;
                }
            }
            else
            {
                AnsiConsole.Write(new Text("[?]", Stylesheet.Styles["dungeon-icon-unrevealed"]));
            }
            AnsiConsole.Write("-");
        }
        AnsiConsole.Write("-");
        AnsiConsole.Write(new Text("[Z]\n", Stylesheet.Styles["dungeon-icon-exit"]));
        var pos = " ";
        if (locationIndex > 0)
            pos += new string(' ', locationIndex * 4 + 1);
        if (currentLocation == dungeon.CurrentFloor.EndRoom) pos += " ";
        AnsiConsole.Write(pos + "^\n");
    }

    public static void DisplayCollectPlantText(IItem plant)
    {
        AnsiConsole.Write(new Text($"{locale.PlantCollected}: {plant.Name}\n"
            , Stylesheet.Styles["dungeon-collect-plant"]));
    }

    public static void DisplayTrapDisarmText(bool success)
    {
        AnsiConsole.Write(success
            ? new Text($"\n{locale.TrapDisarmed}!\n", Stylesheet.Styles["success"])
            : new Text($"\n{locale.TrapDisarmedFail}!\n", Stylesheet.Styles["failure"]));
    }

    public static void DisplayRestAtBonfire(bool ambushed)
    {
        AnsiConsole.Write(new Text($"{locale.RestedAtBonfire}\n", Stylesheet.Styles["dungeon-icon-campfire"]));
        if (ambushed)
            AnsiConsole.Write(new Text($"{locale.Ambushed}\n", Stylesheet.Styles["failure"]));
    }
}