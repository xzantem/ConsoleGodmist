using System.Runtime.InteropServices;
using System.Linq;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Dungeons
{
    // Dungeon Characteristics
    // Dungeons consist of m x n sized rooms and 3 x p or p x 3 sized corridors - implemented
    // They are procedurally generated and once you clear the first room and corridor, another one generates and "adjoins" to the duneon
    // One room may branch out to multiple corridors - implemented
    // Entering a dungeon lands you in the first room and the goal is to reach the corridor. Once in the corridor, you may return back to the room you were in, or proceed to the next room, up to infinity
    // To reach the corridor, sometimes all you need to do is walk up, but sometimes there is a lever you have to move to open the Gate, or a key you have to pick up in order to open the Door - both may require solving a puzzle
    // Dungeons are shrouded in fog of war initially, walking around the dungeon "explores" an area in a 3x3 radius around. You may also find a map which automatically reveals the whole floor (sometimes also hidden rooms!)
    // Random places in rooms may contain an enemy! When you approach an enemy in their 3x3 radius, a battle starts (kinda pokemonesque)
    // Corridors also may contain enemies, in truth they do contain them more commonly than rooms
    // You may also find rewards in rooms, such as stashes(coffins, trunk stashes, chests, etc.) or regional specialties and other alchemical ingredients
    // Exiting a dungeon is simple - simply walk back or use a town portal scroll from your inventory at any given time other than a battle

    // Different dungeon types have different enemies, alchemical ingredients, stash types, and special generation characteristics. These include:
    // Catacombs - rooms are mostly square and corridors very short (up to 3 x 3) - implemented
    // Forest - rooms are very long and thin (up to m x 3), visually indistinguishable from corridors
    // Elvish Ruins - enemies respawn every 20 moves through portals
    // Cove - more stashes exist, but also more levers/keys are required to pass
    // Desert - Corridors are only 1 tile long
    // Temple - traps exist more commonly, rooms are mostly square
    // Mountains - corridors are very long and rooms small (up to 8 x 8)
    // Swamp - same as forest, player speed is reduced by 30% while in the dungeon
    public class Dungeon {
        public List<List<DungeonRoom>> Rooms {get; private set;}
        public List<List<DungeonCorridor>> Corridors {get; private set;}
        public DungeonType DungeonType {get; private set;}
        public int CurrentFloor {get; private set;}
        public DungeonRoom CurrentRoom { get; private set; }
        public DungeonField CurrentField { get; private set; }
        public int[] GlobalPlayerCoordinates { get; private set; }
        public int[] LocalPlayerCoordinates { get; private set; }
        public Canvas Canvas { get; private set; }
        public Dungeon(DungeonType dungeonType) {
            CurrentFloor = 0;
            DungeonType = dungeonType;
            Rooms = new List<List<DungeonRoom>>()
            {
                new()
            };
            Corridors = new();
            (int, int) size = RandomRoomSize();
            Rooms[0].Add(new DungeonRoom(0, 0, CurrentFloor, size.Item1, size.Item2, Direction.South));
            Canvas = new Canvas(21, 21);
        }
        public (int, int) RandomRoomSize() {
            int sizeX = new();
            int sizeY = new();
            Random random = new();
            switch (DungeonType) {
                case DungeonType.Catacombs:
                case DungeonType.Temple:
                    sizeX = random.Next(6, 13);
                    sizeY = EngineMethods.Clamp(sizeX + random.Next(-1, 2), 6, 12);
                    break;
                case DungeonType.Forest:
                case DungeonType.Swamp:
                    if (random.Next(0, 1) == 0) {
                        sizeX = 3;
                        sizeY = random.Next(6, 13);
                    }
                    else {
                        sizeY = 3;
                        sizeX = random.Next(6, 13);
                    }
                    break;
                case DungeonType.ElvishRuins:
                case DungeonType.Cove:
                case DungeonType.Desert:
                    sizeX = random.Next(6, 13);
                    sizeY = random.Next(6, 13);
                    break;
                case DungeonType.Mountains:
                    sizeX = random.Next(4, 9);
                    sizeY = random.Next(4, 9);
                    break;
                    
            }
            return (sizeX, sizeY);
        }
        public int RandomCorridorLength() {
            int length = new();
            Random random = new();
            switch (DungeonType) {
                case DungeonType.Catacombs:
                    length = random.Next(1, 4);
                    break;
                case DungeonType.Forest:
                case DungeonType.Swamp:
                    length = random.Next(6, 13);
                    break;
                case DungeonType.ElvishRuins:
                case DungeonType.Cove:
                case DungeonType.Temple:
                    length = random.Next(4, 9);
                    break;    
                case DungeonType.Desert:
                    length = 1;
                    break;    
                case DungeonType.Mountains:
                    length = random.Next(8, 17);
                    break;                           
            }
            return length;
        }
        private void DrawRectangle(DungeonRoom room, int offsetX, int offsetY)
        {
            for (int x = 0; x < room.SizeX; x++)
            {
                for (int y = 0; y < room.SizeY; y++)
                {
                    Color color = room.Contents[x, y].FieldType switch
                    {
                        DungeonFieldType.Door => Color.DarkSlateGray1,
                        DungeonFieldType.Empty => Color.RosyBrown,
                        _ => Color.White
                    };
                    // Calculate the world coordinates for each point in the rectangle
                    int worldX = room.CornerX + x;
                    int worldY = room.CornerY - y;

                    // Translate world coordinates to the canvas coordinates
                    int drawX = worldX - offsetX;
                    int drawY = worldY - offsetY;  // Maintain original Y position without flipping

                    // Check if the translated coordinates are within the canvas bounds
                    if (drawX >= 0 && drawX < 21 && drawY >= 0 && drawY < 21)
                    {
                        Canvas.SetPixel(drawX, 20 - drawY, color);
                    }
                }
            }
        }
        private void Redraw()
        {
            Canvas = new Canvas(21, 21);
            int offsetX = GlobalPlayerCoordinates[0] - 10;
            int offsetY = GlobalPlayerCoordinates[1] - 10;
            foreach (var room in Rooms.SelectMany(floor => floor))
            {
                DrawRectangle(room, offsetX, offsetY);
            }
            
            foreach (var corridor in Corridors.SelectMany(floor => floor))
            {
                DrawRectangle(corridor, offsetX, offsetY);
            }
            AnsiConsole.WriteLine($"[{GlobalPlayerCoordinates[0]}, {GlobalPlayerCoordinates[1]}]");
            AnsiConsole.WriteLine($"[{LocalPlayerCoordinates[0]}, {LocalPlayerCoordinates[1]}]");
            AnsiConsole.WriteLine($"{CurrentField.FieldType}");
            Canvas.SetPixel(10, 10, Color.Yellow);
            AnsiConsole.Write(Canvas);
        }
        public void GenerateRooms(int floorIndex, int roomIndex) {
            DungeonRoom pivotRoom = Rooms[floorIndex][roomIndex];
            CurrentFloor++;
            Random random = new();
            int adjacentRooms = random.Next(1, 4);
            Rooms.Add(new());
            Corridors.Add(new());
            List<Direction> usedDirections = new() {
                pivotRoom.EntranceDirection
            };
            for (int i = 0; i < adjacentRooms; i++) {
                int iterator = 0;
                DungeonRoom room;
                DungeonCorridor corridor;
                Direction direction;
                do
                {
                    if (usedDirections.Count == 3)
                        direction = Enum.GetValues(typeof(Direction))
                        .Cast<Direction>()
                        .ToList()
                        .Except(usedDirections)
                        .ToList()[0];
                    else
                        do
                        {
                            direction = random.Next(0, 3) switch
                            {
                                0 => Direction.North,
                                1 => Direction.South,
                                2 => Direction.West,
                                3 => Direction.East
                            };
                        } while (usedDirections.Contains(direction));
                    (int, int) roomSize = RandomRoomSize();
                    int minX = pivotRoom.CornerX + 3 - roomSize.Item1;
                    int maxX = pivotRoom.CornerX - 3 + pivotRoom.SizeX;
                    int minY = pivotRoom.CornerY + 3 - pivotRoom.SizeY;
                    int maxY = pivotRoom.CornerY - 3 + roomSize.Item2;
                    int distance = RandomCorridorLength();
                    room = direction switch
                    {
                        Direction.North => new DungeonRoom(random.Next(minX, maxX + 1), pivotRoom.CornerY + roomSize.Item2 + distance, CurrentFloor, roomSize.Item1, roomSize.Item2, Direction.South),
                        Direction.South => new DungeonRoom(random.Next(minX, maxX + 1), pivotRoom.CornerY - pivotRoom.SizeY - distance, CurrentFloor, roomSize.Item1, roomSize.Item2, Direction.North),
                        Direction.West => new DungeonRoom(pivotRoom.CornerX - roomSize.Item1 - distance, random.Next(minY, maxY + 1), CurrentFloor, roomSize.Item1, roomSize.Item2, Direction.East),
                        _ => new DungeonRoom(pivotRoom.CornerX + pivotRoom.SizeX + distance, random.Next(minY, maxY + 1), CurrentFloor, roomSize.Item1, roomSize.Item2, Direction.West),
                    };
                    switch (direction)
                    {
                        case Direction.North:
                            corridor = new DungeonCorridor(
                                random.Next(Math.Max(pivotRoom.CornerX, room.CornerX),
                                    Math.Min(pivotRoom.CornerX + pivotRoom.SizeX, room.CornerX + room.SizeX) - 2),
                                room.CornerY - room.SizeY,
                                CurrentFloor - 1, distance, Direction.South);
                            pivotRoom.Contents[Math.Abs(corridor.CornerX - pivotRoom.CornerX) + 1,
                                pivotRoom.SizeY - 1] = new DungeonField(DungeonFieldType.Door);
                            pivotRoom.Exits.Add(new int[Math.Abs(corridor.CornerX - pivotRoom.CornerX) + 1, pivotRoom.SizeY - 1],
                                [CurrentFloor - 1, i]);
                            break;
                        case Direction.South:
                            corridor = new DungeonCorridor(
                                random.Next(Math.Max(pivotRoom.CornerX, room.CornerX),
                                    Math.Min(pivotRoom.CornerX + pivotRoom.SizeX, room.CornerX + room.SizeX) - 2),
                                pivotRoom.CornerY - pivotRoom.SizeY,
                                CurrentFloor - 1, distance, Direction.North);
                            pivotRoom.Contents[Math.Abs(corridor.CornerX - pivotRoom.CornerX) + 1,
                                0] = new DungeonField(DungeonFieldType.Door);
                            pivotRoom.Exits.Add(new int[Math.Abs(corridor.CornerX - pivotRoom.CornerX) + 1, 0], 
                                [CurrentFloor - 1, i]);
                            break;
                        case Direction.West:
                            corridor = new DungeonCorridor(
                                room.CornerX + room.SizeX,
                                random.Next(
                                    Math.Max(pivotRoom.CornerY - pivotRoom.SizeY, room.CornerY - room.SizeY) + 3,
                                    Math.Min(pivotRoom.CornerY, room.CornerY) + 1),
                                CurrentFloor - 1, distance, Direction.East);
                            pivotRoom.Contents[0,
                                Math.Abs(corridor.CornerY - pivotRoom.CornerY) + 1] = new DungeonField(DungeonFieldType.Door);
                            pivotRoom.Exits.Add(new int[0, Math.Abs(corridor.CornerY - pivotRoom.CornerY) + 1],
                                [CurrentFloor - 1, i]);
                            break;
                        default:
                            corridor = new DungeonCorridor(
                                pivotRoom.CornerX + pivotRoom.SizeX,
                                random.Next(
                                    Math.Max(pivotRoom.CornerY - pivotRoom.SizeY, room.CornerY - room.SizeY) + 3,
                                    Math.Min(pivotRoom.CornerY, room.CornerY) + 1),
                                CurrentFloor - 1, distance, Direction.West);
                            pivotRoom.Contents[pivotRoom.SizeX - 1,
                                Math.Abs(corridor.CornerY - pivotRoom.CornerY) + 1] = new DungeonField(DungeonFieldType.Door);
                            pivotRoom.Exits.Add(new int[pivotRoom.SizeX - 1,
                                Math.Abs(corridor.CornerY - pivotRoom.CornerY) + 1], [CurrentFloor - 1, i]);
                            break;
                    }
                    iterator++;
                } while (DetectCollision(room) && iterator < 100);
                if (!DetectCollision(room))
                {
                    Rooms[CurrentFloor].Add(room);
                    Corridors[CurrentFloor - 1].Add(corridor);
                    usedDirections.Add(direction);
                }
            }
        }
        public bool DetectCollision(DungeonRoom roomToCheck)
        {
            foreach (List<DungeonRoom> floor in Rooms)
            {
                foreach (DungeonRoom room in floor)
                {
                    if (room.CornerX > (roomToCheck.CornerX + roomToCheck.SizeX) || roomToCheck.CornerX > (room.CornerX + room.SizeX))
                    {
                        continue;
                    }
                    if ((room.CornerY - room.SizeY) > roomToCheck.CornerY || (roomToCheck.CornerY - roomToCheck.SizeY) > room.CornerY)
                    {
                        continue;
                    }
                    return true;
                }
            }
            foreach (List<DungeonCorridor> floor in Corridors)
            {
                foreach (DungeonCorridor corridor in floor)
                {
                    if (corridor.CornerX > (roomToCheck.CornerX + roomToCheck.SizeX) || roomToCheck.CornerX > (corridor.CornerX + corridor.SizeX))
                    {
                        continue;
                    }
                    if ((corridor.CornerY - corridor.SizeY) > roomToCheck.CornerY || (roomToCheck.CornerY - roomToCheck.SizeY) > corridor.CornerY)
                    {
                        continue;
                    }
                    return true;
                }
            }
            return false;
        }
        public void TraverseDungeon() {
            CurrentRoom = Rooms[0][0];
            GlobalPlayerCoordinates = new int[2] {CurrentRoom.CornerX + (CurrentRoom.SizeX / 2), CurrentRoom.CornerY};
            LocalPlayerCoordinates = new int[2] { CurrentRoom.SizeX / 2, CurrentRoom.CornerY + CurrentRoom.SizeY - 1};
            CurrentField = CurrentRoom.Contents[LocalPlayerCoordinates[0], LocalPlayerCoordinates[1]];
            GenerateRooms(0, 0);
            while (true) {
                Console.Clear();
                AnsiConsole.Write(new FigletText("Dungeon").Centered().Color(Color.Gold1));
                Redraw();
                switch (Console.ReadKey().Key) {
                    case ConsoleKey.UpArrow:
                        if (GlobalPlayerCoordinates[1] < CurrentRoom.CornerY)
                        {
                            GlobalPlayerCoordinates[1]++;
                            LocalPlayerCoordinates[1]++;
                            CurrentField = CurrentRoom.Contents[LocalPlayerCoordinates[0], LocalPlayerCoordinates[1]];
                        }
                        break;
                    case ConsoleKey.DownArrow:
                        if (GlobalPlayerCoordinates[1] > CurrentRoom.CornerY - CurrentRoom.SizeY + 1)
                        {
                            GlobalPlayerCoordinates[1]--;
                            LocalPlayerCoordinates[1]--;
                            CurrentField = CurrentRoom.Contents[LocalPlayerCoordinates[0], LocalPlayerCoordinates[1]];
                        }
                        break;
                    case ConsoleKey.LeftArrow:
                        if (GlobalPlayerCoordinates[0] != CurrentRoom.CornerX)
                        {
                            GlobalPlayerCoordinates[0]--;
                            LocalPlayerCoordinates[0]--;
                            CurrentField = CurrentRoom.Contents[LocalPlayerCoordinates[0], LocalPlayerCoordinates[1]];
                        }
                        break;
                    case ConsoleKey.RightArrow:
                        if (GlobalPlayerCoordinates[0] != CurrentRoom.CornerX + CurrentRoom.SizeX - 1)
                        {
                            GlobalPlayerCoordinates[0]++;
                            LocalPlayerCoordinates[0]++;
                            CurrentField = CurrentRoom.Contents[LocalPlayerCoordinates[0], LocalPlayerCoordinates[1]];
                        }
                        break;
                    case ConsoleKey.Enter:
                        switch (CurrentField.FieldType)
                        {
                            case DungeonFieldType.Door:
                                var TargetFloorIndex = (CurrentRoom.Exits[new int[LocalPlayerCoordinates[0],LocalPlayerCoordinates[1]]])[0];
                                var TargetRoomIndex = (CurrentRoom.Exits[new int[LocalPlayerCoordinates[0],LocalPlayerCoordinates[1]]])[1];
                                DungeonRoom movedTo = null;
                                if (Rooms[CurrentFloor].Contains(CurrentRoom))
                                {
                                    movedTo = Corridors[TargetFloorIndex][TargetRoomIndex];
                                }
                                else if (Corridors[CurrentFloor].Contains(CurrentRoom))
                                {
                                    movedTo = Rooms[TargetFloorIndex][TargetRoomIndex];
                                }

                                CurrentRoom = movedTo;
                                CurrentFloor = TargetFloorIndex;
                                switch (movedTo.EntranceDirection)
                                {
                                    case Direction.North:
                                        LocalPlayerCoordinates[1] = movedTo.SizeY - 1;
                                        GlobalPlayerCoordinates[1]--;
                                        break;
                                    case Direction.South:
                                        LocalPlayerCoordinates[1] = 0;
                                        GlobalPlayerCoordinates[1]++;
                                        break;
                                    case Direction.East:
                                        LocalPlayerCoordinates[0] = 0;
                                        GlobalPlayerCoordinates[0]++;
                                        break;
                                    case Direction.West:
                                        LocalPlayerCoordinates[0] = movedTo.SizeX - 1;
                                        GlobalPlayerCoordinates[0]--;
                                        break;
                                }
                                break;
                        }
                        break;
                }
            }
        }
    }
}
