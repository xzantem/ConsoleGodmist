using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;

namespace ConsoleGodmist.Characters;

public class EnemyCharacter : Character
{
    public EnemyType EnemyType { get; set; }
    public string Alias { get; set; }
    public override string Name
    {
        get => locale.ResourceManager.GetString(Alias) == null ? Alias : locale.ResourceManager.GetString(Alias);
        set {}
    }

    public DungeonType DefaultLocation { get; set; }
    
    public DropTable DropTable { get; set; }

    public EnemyCharacter()
    {
        StatusEffects = [];
    } // For JSON Serialization

    public EnemyCharacter(EnemyCharacter other, int level) // Deep Copy for initializing new monsters
    {
        Alias = other.Alias;
        EnemyType = other.EnemyType;
        DefaultLocation = other.DefaultLocation;
        DropTable = new DropTable(other.DropTable.Table.ToList());
        _maximalHealth = new Stat(other._maximalHealth.BaseValue, other._maximalHealth.ScalingFactor);
        _minimalAttack = new Stat(other._minimalAttack.BaseValue, other._minimalAttack.ScalingFactor);
        _maximalAttack = new Stat(other._maximalAttack.BaseValue, other._maximalAttack.ScalingFactor);
        CurrentHealth = MaximalHealth;
        _critChance = new Stat(other._critChance.BaseValue, other._critChance.ScalingFactor);
        _dodge = new Stat(other._dodge.BaseValue, other._dodge.ScalingFactor);
        _physicalDefense = new Stat(other._physicalDefense.BaseValue, other._physicalDefense.ScalingFactor);
        _magicDefense = new Stat(other._magicDefense.BaseValue, other._magicDefense.ScalingFactor);
        _resourceRegen = new Stat(other._resourceRegen.BaseValue, other._resourceRegen.ScalingFactor);
        _maximalResource = new Stat(other._maximalResource.BaseValue, other._maximalResource.ScalingFactor);
        _currentResource = 0;
        ResourceType = other.ResourceType;
        _damageDealt = new Stat(other._damageDealt.BaseValue, other._damageDealt.ScalingFactor);
        _damageTaken = new Stat(other._damageTaken.BaseValue, other._damageTaken.ScalingFactor);
        _speed = new Stat(other._speed.BaseValue, other._speed.ScalingFactor);
        _accuracy = new Stat(other._accuracy.BaseValue, other._accuracy.ScalingFactor);
        _critMod = new Stat(other._critMod.BaseValue, other._critMod.ScalingFactor);
        StatusEffects = [];
        Resistances = other.Resistances.ToDictionary(x => x.Key, x => 
            new Stat(x.Value.BaseValue, x.Value.ScalingFactor));
        ActiveSkills = (ActiveSkill[])other.ActiveSkills.Clone();
        Level = level;
    }
}