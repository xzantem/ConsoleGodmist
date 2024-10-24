using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Dungeons;

public static class DungeonMovementManager
{
    public static Dungeon CurrentDungeon { get; private set; }
    private static DungeonRoom CurrentLocation { get; set; } = null!;
    private static int LocationIndex { get; set; }
    private static int LastMovement { get; set; }
    private static bool Exited { get; set; }
    private static List<Text> Log { get; set; }

    public static void EnterDungeon(Dungeon dungeon)
    {
        if (CurrentDungeon != null)
            throw new Exception("Attempted to enter new dungeon, but player is already in a dungeon!");
        Exited = false;
        CurrentDungeon = dungeon;
        Log = [];
        CurrentLocation = CurrentDungeon.CurrentFloor.StarterRoom;
        LocationIndex = 0;
        LastMovement = 0;
        CurrentLocation.Reveal();
        OnMove();
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
        var color = CurrentDungeon.DungeonType switch
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
        AnsiConsole.Write(new FigletText(locationText).Centered().Color(color));
        foreach (var log in Log)
            AnsiConsole.Write(log);
        Log.Clear();
        string floor = CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0
            ? (CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor)).ToString()
            : ("-" + CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor));
        string visited = (CurrentDungeon.Floors.Count - 1) == 0
            ? (CurrentDungeon.Floors.Count - 1).ToString()
            : ("-" + (CurrentDungeon.Floors.Count - 1));
        locationText = locale.Level + " " + CurrentDungeon.DungeonLevel + ", " + 
                       locale.Floor + " " + floor + " [" +
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
        foreach (var corridor in CurrentDungeon.CurrentFloor.Corridor)
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
        var pos = " ";
        if (LocationIndex > 0)
            pos += new String(' ', LocationIndex * 4 + 1);
        if (CurrentLocation == CurrentDungeon.CurrentFloor.EndRoom) pos += " ";
        AnsiConsole.Write(pos + "^\n");
    }

    public static void TraverseDungeon()
    {
        while(!Exited)
        {
            DisplayCurrentFloorMap();
            switch (ChooseAction())
            {
                case 0:
                    MoveForward();
                    break;
                case 1:
                    MoveBackwards();
                    break;
                case 2:
                    InventoryMenuHandler.OpenInventoryMenu();
                    break;
                case 3:
                    //Open Quest Log
                    break;
                case 4:
                    //Show Character Info
                    break;
                case 5:
                    //Rest at Campfire
                    break;
                case 6:
                    var plant = PlantDropManager.DropDatabase[CurrentDungeon.DungeonType]
                        .GetDrop(CurrentDungeon.DungeonLevel);
                    PlayerHandler.player.Inventory.AddItem(plant.Key, plant.Value);
                    Log.Add(new Text($"{locale.PlantCollected}: {plant.Key.Name}\n"
                        , Stylesheet.Styles["dungeon-collect-plant"]));
                    CurrentLocation.Clear();
                    break;
                case 7:
                    //Open Stash
                    break;
                case 8:
                    var trap = CurrentDungeon.CurrentFloor.Traps
                        .FirstOrDefault(x => x.Location == CurrentDungeon.CurrentFloor.Corridor[LocationIndex]);
                    if (trap.Activate())
                    {
                        trap.Disarm();
                        CurrentDungeon.CurrentFloor.Traps.Remove(trap);
                        Log.Add(new Text($"{locale.TrapDisarmed}!\n", Stylesheet.Styles["success"]));
                    }
                    else
                        Log.Add(new Text($"{locale.TrapDisarmedFail}!\n", Stylesheet.Styles["failure"]));
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
        if (LastMovement == 0)
        {
            if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count + 1)
                choices.Add(locale.GoForward, 0);
            else if (LocationIndex == CurrentDungeon.CurrentFloor.Corridor.Count + 1)
                choices.Add(locale.GoDown, 0);
            
            if (LocationIndex == 0)
                choices.Add(CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0 ? 
                    locale.ExitDungeon : locale.GoUp, 1);
            else if (LocationIndex > 0)
                choices.Add(locale.GoBack, 1);
        }
        else
        {
            if (LocationIndex == 0)
                choices.Add(CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0 ? 
                    locale.ExitDungeon : locale.GoUp, 1);
            else if (LocationIndex > 0)
                choices.Add(locale.GoBack, 1);
            
            if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count + 1)
                choices.Add(locale.GoForward, 0);
            else if (LocationIndex == CurrentDungeon.CurrentFloor.Corridor.Count + 1)
                choices.Add(locale.GoDown, 0);
        }
        switch (CurrentLocation.FieldType)
        {
            case DungeonFieldType.Campfire:
                choices.Add(locale.RestAtCampfire, 5);
                break;
            case DungeonFieldType.Plant:
                choices.Add(locale.CollectPlant, 6);
                break;
            case DungeonFieldType.Stash:
                choices.Add(locale.OpenStash, 7);
                break;
        }
        if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count)
        {
            if (CurrentDungeon.CurrentFloor.Corridor[LocationIndex].FieldType == DungeonFieldType.Trap && CurrentDungeon.CurrentFloor.Corridor[LocationIndex].Revealed)
            {
                choices.Add(locale.DisarmTrap, 8);
            }
        }
        choices.Add(locale.OpenInventory, 2);
        choices.Add(locale.QuestLog, 3);
        choices.Add(locale.ShowCharacter, 4);
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .AddChoices(choices.Keys)
            .HighlightStyle(new Style(Color.MediumPurple3)));
        if (choices[choice] >= 0 && choices[choice] <= 3)
            LastMovement = choices[choice];
        return choices[choice];
    }

    public static void ExitDungeon()
    {
        Exited = true;
        CurrentDungeon = null;
    }

    private static void MoveForward()
    {
        if (LocationIndex < CurrentDungeon.CurrentFloor.Corridor.Count + 1)
        { // Go Forward if not on last block
            LocationIndex++;
            CurrentLocation = LocationIndex == CurrentDungeon.CurrentFloor.Corridor.Count + 1 ? 
                CurrentDungeon.CurrentFloor.EndRoom : CurrentDungeon.CurrentFloor.Corridor[LocationIndex - 1];
            if (!CurrentLocation.Revealed)
            {
                CurrentLocation.Reveal();
                if (Random.Shared.NextDouble() < 0.125)
                    CurrentDungeon.ScoutFloor(CurrentDungeon.CurrentFloor);
            }
        }
        else
        { // Go Down if on last block
            CurrentDungeon.Descend();
            CurrentLocation = CurrentDungeon.CurrentFloor.StarterRoom;
            LocationIndex = 0;
            CurrentLocation.Reveal();
        }
        OnMove();
    }
    private static void MoveBackwards()
    {
        if (LocationIndex == 0)
        { // Move Up if on first block
            if (CurrentDungeon.Floors.IndexOf(CurrentDungeon.CurrentFloor) == 0)
            {
                ExitDungeon();
                return;
            }
            CurrentDungeon.Ascend();
            CurrentLocation = CurrentDungeon.CurrentFloor.EndRoom;
            LocationIndex = CurrentDungeon.CurrentFloor.Corridor.Count + 1;
        }
        else
        { // Move Back if not on first block
            LocationIndex--;
            CurrentLocation = LocationIndex == 0 ? 
                CurrentDungeon.CurrentFloor.StarterRoom : CurrentDungeon.CurrentFloor.Corridor[LocationIndex - 1];
        }
        OnMove();
    }

    private static void OnMove()
    {
        switch (CurrentLocation.FieldType)
        {
            case DungeonFieldType.Battle:
                BattleManager.StartNewBattle(new Dictionary<BattleUser, int>
                {
                    {new BattleUser(PlayerHandler.player), 0}, 
                    {new BattleUser(EnemyFactory.CreateEnemy(CurrentDungeon.DungeonType, CurrentDungeon.DungeonLevel)), 1}
                });
                if (BattleManager.CurrentBattle.Escaped)
                    MoveBackwards();
                else
                    CurrentLocation.Clear();
                break;
            case DungeonFieldType.Trap:
                var trap = CurrentDungeon.CurrentFloor.Traps.FirstOrDefault(x => x.Location == CurrentLocation);
                AnsiConsole.Write(new Text($"{locale.TrapActivated}! {locale.TryDisarmTrap}\n"
                    , Stylesheet.Styles["dungeon-icon-trap"]));
                trap.Disarm();
                CurrentDungeon.CurrentFloor.Traps.Remove(trap);
                if (!trap.Activate())
                    trap.Trigger();
                else
                    Log.Add(new Text($"{locale.TrapDisarmed}!\n", Stylesheet.Styles["success"]));
                break;
                    
        }
    }
}