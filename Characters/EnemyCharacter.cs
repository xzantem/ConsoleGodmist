using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.Combat.Modifiers;

namespace ConsoleGodmist.Characters;

public class EnemyCharacter : Character
{
    public EnemyType EnemyType { get; set; }
    public string Alias { get; set; }
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : 
        locale.ResourceManager.GetString(Alias);
    
    public DungeonType DefaultLocation { get; set; }
    
    public DropTable DropTable { get; set; }

    public EnemyCharacter()
    {
        StatusEffects = [];
    } // For JSON Serialization

    public void Initialize()
    {
        CurrentHealth = MaximalHealth;
    }
}