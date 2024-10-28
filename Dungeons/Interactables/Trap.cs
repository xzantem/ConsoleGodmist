using System.Data.Common;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
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

    public void Trigger()
    {
        switch (DungeonType)
        {
            case DungeonType.Catacombs:
                PlayerHandler.player.TakeDamage(new Dictionary<DamageType, double>
                {
                    { DamageType.True, PlayerHandler.player.MaximalHealth / 10 },
                    { DamageType.Physical, PlayerHandler.player.MaximalHealth / 5 }
                });
                break;
            case DungeonType.Forest:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
                StatusEffectHandler.AddStatusEffect(new DoTStatusEffect(PlayerHandler.player.MaximalHealth / 25, 
                    StatusEffectType.Poison, "Trap", 5), PlayerHandler.player);
                break;
            case DungeonType.ElvishRuins:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
                break;
            case DungeonType.Cove:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
                StatusEffectHandler.AddStatusEffect(new DoTStatusEffect(PlayerHandler.player.MaximalHealth / 25, 
                    StatusEffectType.Burn, "Trap", 5), PlayerHandler.player);
                break;
            case DungeonType.Desert:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
                PlayerHandler.player.AddModifier(StatType.Dodge, new StatModifier(ModifierType.Additive, 15, "Trap", 5));
                break;
            case DungeonType.Temple:
                PlayerHandler.player.TakeDamage(new Dictionary<DamageType, double>
                {
                    { DamageType.True, PlayerHandler.player.MaximalHealth / 10 },
                    { DamageType.Magic, PlayerHandler.player.MaximalHealth / 5 }
                });
                break;
            case DungeonType.Mountains:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
                StatusEffectHandler.AddStatusEffect(new DoTStatusEffect(PlayerHandler.player.MaximalHealth / 25, 
                    StatusEffectType.Bleed, "Trap", 5), PlayerHandler.player);
                break;
            case DungeonType.Swamp:
                PlayerHandler.player.TakeDamage(DamageType.True, PlayerHandler.player.MaximalHealth / 10);
                PlayerHandler.player.AddModifier(StatType.Speed, new StatModifier(ModifierType.Additive, 10, "Trap", 5));
                break;
        }
    }
}