using System.Globalization;
using ConsoleGodmist.Enums;
using Microsoft.VisualBasic.FileIO;
using Spectre.Console;

namespace ConsoleGodmist.Dungeons;

public static class DungeonMovementManager
{
    private static Dungeon? CurrentDungeon { get; set; }
    private static DungeonRoom CurrentLocation { get; set; }

    private static int LocationIndex { get; set; }

    public static void EnterDungeon(Dungeon dungeon)
    {
        if (CurrentDungeon != null)
            throw new Exception("Attempted to enter new dungeon, but player is already in a dungeon!");
        CurrentDungeon = dungeon;
        CurrentLocation = CurrentDungeon.CurrentFloor.StarterRoom;
        LocationIndex = 0;
        CurrentLocation.Reveal();
    }
    
    private static void DisplayCurrentFloorMap()
    {
        var locationText = CurrentDungeon.DungeonType switch
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
        string floor = CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0
            ? (CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor)).ToString()
            : ("-" + CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor));
        string visited = (CurrentDungeon.Floors.Count - 1) == 0
            ? (CurrentDungeon.Floors.Count - 1).ToString()
            : ("-" + (CurrentDungeon.Floors.Count - 1));
        locationText += " " + locale.Level + " " + CurrentDungeon.DungeonLevel + ", " + locale.Floor + " " + floor + " [" +
                        visited + "]\n" + locale.Map + ":";
        AnsiConsole.WriteLine(locationText);
        if (CurrentDungeon.CurrentFloor.StarterRoom.Revealed)
        {
            switch (CurrentDungeon.CurrentFloor.StarterRoom.FieldType)
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
        for (int i = 0; i < CurrentDungeon.CurrentFloor.Corridor.Count; i++)
        {
            if (CurrentDungeon.CurrentFloor.Corridor[i].Revealed)
            {
                switch (CurrentDungeon.CurrentFloor.Corridor[i].FieldType)
                {
                    case DungeonFieldType.Battle:
                        AnsiConsole.Write(new Text("[W]", Stylesheet.Styles["dungeon-icon-battle"]));
                        break;
                    case DungeonFieldType.Stash:
                        AnsiConsole.Write(new Text("[S]", Stylesheet.Styles["dungeon-icon-stash"]));
                        break;
                    case DungeonFieldType.Campfire:
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
        string pos = " ";
        if (LocationIndex > 0)
            pos += new String(' ', LocationIndex * 4 + 1);
        if (CurrentLocation == CurrentDungeon.CurrentFloor.EndRoom) pos += " ";
        AnsiConsole.Write(pos + "^\n");
    }

    public static void TraverseDungeon()
    {
        while(true)
        {
            DisplayCurrentFloorMap();
            switch (ChooseAction())
            {
                case 0:
                    LocationIndex++;
                    CurrentLocation = LocationIndex == CurrentDungeon.CurrentFloor.Corridor.Count + 1 ? 
                        CurrentDungeon.CurrentFloor.EndRoom : CurrentDungeon.CurrentFloor.Corridor[LocationIndex - 1];
                    if (!CurrentLocation.Revealed)
                    {
                        CurrentLocation.Reveal();
                        if (Random.Shared.NextDouble() < 0.125)
                            CurrentDungeon.ScoutFloor(CurrentDungeon.CurrentFloor);
                    }
                    break;
                case 1:
                    LocationIndex--;
                    CurrentLocation = LocationIndex == 0 ? 
                        CurrentDungeon.CurrentFloor.StarterRoom : CurrentDungeon.CurrentFloor.Corridor[LocationIndex - 1];
                    break;
                case 2:
                    if (CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0)
                    {
                        CurrentDungeon = null;
                        return;
                    }
                    CurrentDungeon.Ascend();
                    CurrentLocation = CurrentDungeon.CurrentFloor.EndRoom;
                    LocationIndex = CurrentDungeon.CurrentFloor.Corridor.Count + 1;
                    break;
                case 3:
                    CurrentDungeon.Descend();
                    CurrentLocation = CurrentDungeon.CurrentFloor.StarterRoom;
                    LocationIndex = 0;
                    CurrentLocation.Reveal();
                    break;
                case 4:
                    //Open Inventory
                    break;
                case 5:
                    //Open Quest Log
                    break;
                case 6:
                    //Show Character Info
                    break;
                case 7:
                    //Rest at Campfire
                    break;
                case 8:
                    //Collect Plant
                    break;
                case 9:
                    //Open Stash
                    break;
                case 10:
                    //Disarm Trap
                    break;
                default:
                    AnsiConsole.Write(new Text("Invalid action.\n", Stylesheet.Styles["error"]));
                    break;
            }
        }
    }

    private static int ChooseAction()
    {
        Dictionary<string, int> choices = new();
        if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count + 1)
            choices.Add(locale.GoForward, 0);
        if (LocationIndex > 0)
            choices.Add(locale.GoBack, 1);
        if (LocationIndex == 0)
            choices.Add(CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0 ? 
                locale.ExitDungeon : locale.GoUp, 2);
        else if (LocationIndex == CurrentDungeon.CurrentFloor.Corridor.Count + 1)
            choices.Add(locale.GoDown, 3);
        else
        {
            switch (CurrentLocation.FieldType)
            {
                case DungeonFieldType.Campfire:
                    choices.Add(locale.RestAtCampfire, 7);
                    break;
                case DungeonFieldType.Plant:
                    choices.Add(locale.CollectPlant, 8);
                    break;
                case DungeonFieldType.Stash:
                    choices.Add(locale.OpenStash, 9);
                    break;
            }
        }
        if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count)
        {
            if (CurrentDungeon.CurrentFloor.Corridor[LocationIndex].FieldType == DungeonFieldType.Trap && CurrentDungeon.CurrentFloor.Corridor[LocationIndex].Revealed)
            {
                choices.Add(locale.DisarmTrap, 10);
            }
        }
        choices.Add(locale.OpenInventory, 4);
        choices.Add(locale.QuestLog, 5);
        choices.Add(locale.ShowCharacter, 6);
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .AddChoices(choices.Keys)
            .HighlightStyle(new Style(Color.MediumPurple3)));
        return choices[choice];
    }
}