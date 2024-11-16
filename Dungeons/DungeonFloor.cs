using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Dungeons;
using Enums;

public class DungeonFloor
{
    public DungeonRoom StarterRoom { get; private set; }
    public DungeonRoom EndRoom { get; private set; }
    public List<DungeonCorridor> Corridor { get; private set; }
    
    public List<Trap> Traps { get; private set; }
    public List<Battle> Battles { get; private set; }
    public DungeonFloor(int length, int difficulty, DungeonType dungeonType, int level)
    {
        Traps = [];
        Battles = [];
        var starterRoomField = UtilityMethods.RandomChoice(new
            Dictionary<DungeonFieldType, double> { { DungeonFieldType.Empty, 0.5 }, { DungeonFieldType.Battle, 0.5 } });
        StarterRoom = new DungeonRoom(starterRoomField);
        if (starterRoomField == DungeonFieldType.Battle)
            Battles.Add(new Battle(new Dictionary<BattleUser, int>
            {
                {new BattleUser(PlayerHandler.player), 0}, 
                {new BattleUser(EnemyFactory.CreateEnemy(dungeonType, level)), 1}
            }, StarterRoom));
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
            var fieldType = UtilityMethods.RandomChoice(weights);
            Corridor.Add(new DungeonCorridor(fieldType));
            switch (fieldType)
            {
                case DungeonFieldType.Trap:
                    Traps.Add(new Trap(GameSettings.Difficulty, Corridor[^1], dungeonType));
                    break;
                case DungeonFieldType.Battle:
                    Battles.Add(new Battle(new Dictionary<BattleUser, int>
                    {
                        {new BattleUser(PlayerHandler.player), 0}, 
                        {new BattleUser(EnemyFactory.CreateEnemy(dungeonType, level)), 1}
                    }, Corridor[^1]));
                    break;
            }
        }
    }
}