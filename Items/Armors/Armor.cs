using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items.Weapons;

namespace ConsoleGodmist.Items.Armors;

public class Armor : IEquippable
{
    //Base IItem implementations
    public string Alias { get; }
    public int Weight { get; }
    public int ID { get; }
    public int Cost { get; }
    public ItemRarity Rarity { get; }
    public bool Stackable { get; }
    public string Description { get; }
    public ItemType ItemType => ItemType.Armor;
    
    //Base IEquippable implementations
    public int RequiredLevel { get; }
    public CharacterClass RequiredClass { get; }
    public Quality Quality { get; }
    public double UpgradeModifier { get; set; }
    public double RarityModifier
    {
        get
        {
            return Rarity switch
            {
                ItemRarity.Destroyed => 0.7f,
                ItemRarity.Damaged => 0.85f,
                ItemRarity.Uncommon => 1.05f,
                ItemRarity.Rare => 1.1f,
                ItemRarity.Ancient => 1.2f,
                ItemRarity.Legendary => 1.3f,
                ItemRarity.Mythical => 1.5f,
                ItemRarity.Godly => 1.75f,
                _ => 1f,
            };
        }
    }
    
    //Weapon implementations
    public ArmorPlate Plate { get; private set; }
    public ArmorBinder Binder { get; private set; }
    public ArmorBase Base { get; private set; }
    public int MaximalHealth
    {
        get
        {
            double value = Binder.Health;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 10,
                CharacterClass.Scout => 6,
                CharacterClass.Sorcerer => 4,
                _ => 14
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier * (Binder.Tier * 10 - 5);
            value *= RarityModifier * (Plate.HealthBonus + Base.HealthBonus + 1);
            return (int)value;
        }
    }
    public int Dodge
    {
        get
        {
            double value = Base.Dodge;
            var multiplier = 2;
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier;
            value *= RarityModifier * (Plate.DodgeBonus + Binder.DodgeBonus + 1);
            return (int)value;
        }
    }
    public int PhysicalDefense
    {
        get
        {
            double value = Plate.PhysicalDefense;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 1.2,
                CharacterClass.Scout => 0.8,
                CharacterClass.Sorcerer => 0.2,
                _ => 1.6
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier;
            value *= UpgradeModifier * (Binder.PhysicalDefenseBonus + Base.PhysicalDefenseBonus + 1);
            return (int)value;
        }
    }
    public int MagicDefense
    {
        get
        {
            double value = Plate.MagicDefense;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 0.8,
                CharacterClass.Scout => 0.4,
                CharacterClass.Sorcerer => 1.2,
                _ => 1.6
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier;
            value *= UpgradeModifier * (Binder.MagicDefenseBonus + Base.MagicDefenseBonus + 1);
            return (int)value;
        }
    }

    public bool Use()
    {
        if (RequiredClass != PlayerHandler.player.CharacterClass)
        {
            //Maybe display some text
            return false;
        }
        if (RequiredLevel > PlayerHandler.player.Level)
        {
            //Maybe display some text
            return false;
        }
        PlayerHandler.player.SwitchArmor(this);
        return true;
    }
}