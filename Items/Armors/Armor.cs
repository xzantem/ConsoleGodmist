using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public class Armor : BaseItem, IEquippable
{
    //BaseItem implementations
    public override string Name { get; set; }
    public override int Weight => 5;
    public override int ID => 560;
    public int BaseCost { get; set; }
    public override int Cost => (int)(BaseCost * EquippableItemService.RarityModifier(Rarity));
    public override bool Stackable => false;
    public override ItemType ItemType => ItemType.Armor;
    
    //Base IEquippable implementations
    public int RequiredLevel { get; set; }
    public CharacterClass RequiredClass { get; }
    public Quality Quality { get; set; }
    public double UpgradeModifier { get; set; }
    
    //Armor implementations
    public ArmorPlate Plate { get; set; }
    public ArmorBinder Binder { get; set; }
    public ArmorBase Base { get; set; }
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
        } + quality switch
        {
            Quality.Weak => $" ({locale.Weak})",
            Quality.Excellent => $" ({locale.Excellent})",
            _ => ""
        };
        Alias = $"{plate.Alias}.{binder.Alias}.{armBase.Alias}";
        BaseCost = plate.GoldCost + binder.GoldCost + armBase.GoldCost;
        Rarity = EquippableItemService.GetRandomRarity();
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
    
    /// <summary>
    /// Gets Starter weapon for the specified class
    /// </summary>
    /// <param name="requiredClass"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Armor(CharacterClass requiredClass)
    {
        switch (requiredClass)
        {
            case CharacterClass.Warrior:
                Plate = EquipmentPartManager.GetPart<ArmorPlate>("ScratchedPlate");
                Binder = EquipmentPartManager.GetPart<ArmorBinder>("ScratchedBinder");
                Base = EquipmentPartManager.GetPart<ArmorBase>("ScratchedBase");
                Name = locale.ResourceManager.GetString(Plate.Adjective) + " " + locale.Hauberk;
                break;
            case CharacterClass.Scout:
                Plate = EquipmentPartManager.GetPart<ArmorPlate>("HoleyPlate");
                Binder = EquipmentPartManager.GetPart<ArmorBinder>("HoleyBinder");
                Base = EquipmentPartManager.GetPart<ArmorBase>("HoleyBase");
                Name = locale.ResourceManager.GetString(Plate.Adjective) + " " + locale.Tunic;
                break;
            case CharacterClass.Sorcerer:
                Plate = EquipmentPartManager.GetPart<ArmorPlate>("TornPlate");
                Binder = EquipmentPartManager.GetPart<ArmorBinder>("TornBinder");
                Base = EquipmentPartManager.GetPart<ArmorBase>("TornBase");
                Name = locale.ResourceManager.GetString(Plate.Adjective) + " " + locale.Robe;
                break;
            case CharacterClass.Paladin:
                Plate = EquipmentPartManager.GetPart<ArmorPlate>("PiercedPlate");
                Binder = EquipmentPartManager.GetPart<ArmorBinder>("PiercedBinder");
                Base = EquipmentPartManager.GetPart<ArmorBase>("PiercedBase");
                Name = locale.ResourceManager.GetString(Plate.Adjective) + " " + locale.Cuirass;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(requiredClass), requiredClass, null);
        }
        Alias = $"StarterWeapon.{requiredClass.ToString()}";
        BaseCost = 15;
        Rarity = ItemRarity.Junk;
        RequiredLevel = 1;
        RequiredClass = requiredClass;
        Quality = Quality.Normal;
        UpgradeModifier = 1;
    }
    public Armor() {}

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
    
    public override void Inspect(int amount = 1)
    {
        base.Inspect(amount);
        var playerArmor = PlayerHandler.player.Armor;
        AnsiConsole.Write(new Text($"{locale.Level} {RequiredLevel}, + {UpgradeModifier:P0}\n", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"{locale.Defense}: {PhysicalDefense}", Stylesheet.Styles["default"]));
        WriteComparator(PhysicalDefense, playerArmor.PhysicalDefense);
        AnsiConsole.Write(new Text($" | {MagicDefense}", Stylesheet.Styles["default"]));
        WriteComparator(MagicDefense, playerArmor.MagicDefense);
        AnsiConsole.Write(new Text($"\n{locale.Dodge}: {Dodge}", Stylesheet.Styles["default"]));
        WriteComparator(Dodge, playerArmor.Dodge);
        AnsiConsole.Write(new Text($"\n{locale.HealthC}: {MaximalHealth}", Stylesheet.Styles["default"]));
        WriteComparator(MaximalHealth, playerArmor.MaximalHealth);
        AnsiConsole.Write(new Text("\n"));
        return;
        void WriteComparator(double value1, double value2)
        {
            if (value1 > value2)
                AnsiConsole.Write(new Text(" ( ^ )", Stylesheet.Styles["success"]));
            else if (value1 == value2)
                AnsiConsole.Write(new Text(" ( ~ )", Stylesheet.Styles["default"]));
            else
                AnsiConsole.Write(new Text(" ( v )", Stylesheet.Styles["failure"]));
        }
    }
}