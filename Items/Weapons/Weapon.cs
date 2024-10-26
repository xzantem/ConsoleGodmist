using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items.Weapons;

public class Weapon : IEquippable
{
    //Base IItem implementations
    public string Alias { get; }
    public int Weight { get; }
    public int ID { get; }
    public int Cost { get; }
    public ItemRarity Rarity { get; }
    public bool Stackable { get; }
    public string Description { get; }
    public ItemType ItemType => ItemType.Weapon;
    
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
    public WeaponHead Head { get; private set; }
    public WeaponBinder Binder { get; private set; }
    public WeaponHandle Handle { get; private set; }
    public int MinimalAttack
    {
        get
        {
            double value = Head.MinimalAttack;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 1,
                CharacterClass.Scout => 0.4,
                CharacterClass.Sorcerer => 0.8,
                _ => 0.4
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier * (Head.Tier * 10 - 5);
            value *= UpgradeModifier * (Handle.AttackBonus + Binder.AttackBonus + 1);
            return (int)value;
        }
    }
    public int MaximalAttack
    {
        get
        {
            double value = Head.MaximalAttack;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 2,
                CharacterClass.Scout => 2,
                CharacterClass.Sorcerer => 2.4,
                _ => 1.2
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier * (Head.Tier * 10 - 5);
            value *= UpgradeModifier * (Handle.AttackBonus + Binder.AttackBonus + 1);
            return (int)value;
        }
    }
    public double CritChance // Mage gains Mana regen instead of CritChance
    {
        get
        {
            var value = Binder.CritChance;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 0.01,
                CharacterClass.Scout => 0.01,
                CharacterClass.Sorcerer => 0.05,
                _ => 0.01
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            if (RequiredClass == CharacterClass.Sorcerer)
                multiplier *= Binder.Tier * 10 - 5;
            value += multiplier;
            value *= RarityModifier * (Head.CritChanceBonus + Handle.CritChanceBonus + 1);
            return value;
        }
    }
    public double CritMod
    {
        get
        {
            var value = Head.CritMod;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 0.02,
                CharacterClass.Scout => 0.03,
                CharacterClass.Sorcerer => 0.02,
                _ => 0.02
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            value += multiplier * (Head.Tier * 10 - 5);
            value *= RarityModifier * (Binder.CritModBonus + Handle.CritModBonus + 1);
            return value;
        }
    }
    public int Accuracy // Mage gains Maximal Mana instead of Accuracy
    {
        get
        {
            double value = Handle.Accuracy;
            var multiplier = RequiredClass switch
            {
                CharacterClass.Warrior => 1,
                CharacterClass.Scout => 1,
                CharacterClass.Sorcerer => 0.3,
                _ => 2
            };
            multiplier *= Quality switch
            {
                Quality.Weak => -1,
                Quality.Normal => 0,
                Quality.Excellent => 1,
                Quality.Masterpiece => 4,
                _ => 0
            };
            if (RequiredClass == CharacterClass.Sorcerer)
                multiplier *= Handle.Tier * 10 - 5;
            value += (int)multiplier;
            value *= RarityModifier * (Head.AccuracyBonus + Binder.AccuracyBonus + 1);
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
        PlayerHandler.player.SwitchWeapon(this);
        return true;
    }
}