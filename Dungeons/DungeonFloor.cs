using ConsoleGodmist.Components;

namespace ConsoleGodmist.Dungeons;
using Enums;

public class DungeonFloor
{
    public DungeonRoom StarterRoom { get; private set; }
    public DungeonRoom EndRoom { get; private set; }
    public List<DungeonCorridor> Corridor { get; private set; }
    
    public List<Trap> Traps { get; private set; }
    public DungeonFloor(int length, int difficulty, DungeonType dungeonType)
    {
        Traps = [];
        StarterRoom = new DungeonRoom(EngineMethods.RandomChoice<DungeonFieldType>(new 
            Dictionary<DungeonFieldType, double> { { DungeonFieldType.Empty, 0.5 }, { DungeonFieldType.Battle, 0.5 } }));
        EndRoom = new DungeonRoom(DungeonFieldType.Empty);
        Corridor = new List<DungeonCorridor>();
        Dictionary<DungeonFieldType, int> weights = new()
        {
            {DungeonFieldType.Empty, 9},
            {DungeonFieldType.Plant, 4},
            {DungeonFieldType.Battle, difficulty},
            {DungeonFieldType.Bonfire, 4},
            {DungeonFieldType.Trap, 1 + difficulty},
        };
        for (var i = 0; i < length; i++)
        {
            var fieldType = EngineMethods.RandomChoice(weights);
            Corridor.Add(new DungeonCorridor(fieldType));
            if (fieldType == DungeonFieldType.Trap)
                Traps.Add(new Trap(GameSettings.Difficulty, Corridor[^1], dungeonType));
        }
    }
}