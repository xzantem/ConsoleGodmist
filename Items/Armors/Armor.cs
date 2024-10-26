﻿using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items.Weapons;

namespace ConsoleGodmist.Items.Armors;

public class Armor : IEquippable
{
    //Base IItem implementations
    public string Name { get; }
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
            value *= EquippableItemService.RarityModifier(Rarity) * (Plate.HealthBonus + Base.HealthBonus + 1);
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
            value *= EquippableItemService.RarityModifier(Rarity) * (Plate.DodgeBonus + Binder.DodgeBonus + 1);
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
    
    public Armor(ArmorPlate plate, ArmorBinder binder, ArmorBase armBase, CharacterClass requiredClass, Quality quality, int requiredLevel = 0)
    {
        Plate = plate;
        Binder = binder;
        Base = armBase;
        Name = Plate.Adjective + requiredClass switch
        {
            CharacterClass.Warrior => locale.Hauberk,
            CharacterClass.Scout => locale.Tunic,
            CharacterClass.Sorcerer => locale.Robe,
            _ => locale.Cuirass
        };
        Alias = $"{plate.Alias}.{binder.Alias}.{armBase.Alias}";
        Weight = 8;
        ID = 560;
        Cost = plate.Cost + binder.Cost + armBase.Cost;
        Rarity = EquippableItemService.GetRandomRarity();
        Stackable = false;
        RequiredLevel = requiredLevel == 0
            ? Math.Max(Math.Max(plate.Tier, binder.Tier), armBase.Tier) * 10 - 5 + Quality switch
            {
                Quality.Weak => -3,
                Quality.Normal => 0,
                Quality.Excellent => 3,
                Quality.Masterpiece => 5,
                _ => 0
            }
            : requiredLevel;
        RequiredClass = requiredClass;
        Quality = quality;
        UpgradeModifier = 1;
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