using System.Data.Common;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Dungeons;

public class Trap(Difficulty difficulty, DungeonField location, int trapType, DungeonType dungeonType)
{
    public Difficulty Difficulty { get; private set; } = difficulty;
    public int TrapType { get; private set; } = trapType;

    public DungeonField Location { get; private set; } = location;
    public DungeonType DungeonType { get; private set; } = dungeonType;

    public Trap(Difficulty difficulty, DungeonField location, DungeonType dungeonType) : 
        this(difficulty, location, Random.Shared.Next(0, TrapMinigameManager.MinigameCount), dungeonType)
    {
    }

    public bool Activate()
    {
        return TrapMinigameManager.StartMinigame(Difficulty, TrapType);
    }

    public void Disarm()
    {
        Location.Clear();
    }

    public List<Text> Trigger()
    {
        switch (DungeonType)
        {
            case DungeonType.Catacombs:
                return PlayerHandler.player.TakeDamage(new Dictionary<DamageType, double>
                {
                    { DamageType.True, PlayerHandler.player.MaximalHealth / 10 },
                    { DamageType.Physical, PlayerHandler.player.MaximalHealth / 5 }
                });
            case DungeonType.Forest: // Add 20% Poison
                return PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
            case DungeonType.ElvishRuins:
                return PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
            case DungeonType.Cove: // Add 20% Burn
                return PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
            case DungeonType.Desert: // Add Dodge Debuff
                return PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
            case DungeonType.Temple:
                return PlayerHandler.player.TakeDamage(new Dictionary<DamageType, double>
                {
                    { DamageType.True, PlayerHandler.player.MaximalHealth / 10 },
                    { DamageType.Physical, PlayerHandler.player.MaximalHealth / 5 }
                });
            case DungeonType.Mountains: // Add 20% Bleed
                return PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
            case DungeonType.Swamp: // Add Slow
                return PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
        }

        return [];
    }
}