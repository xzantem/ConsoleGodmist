using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleGodmist.Items;

public class Weapon : BaseItem, IEquippable
{
    //Base IItem implementations
    public override string Name { get; set; }
    [JsonIgnore]
    public override int Weight => 5;
    [JsonIgnore]
    public override int ID => 559;
    public int BaseCost { get; set; }

    [JsonIgnore]
    public override int Cost => (int)(BaseCost * EquippableItemService.RarityModifier(Rarity));
    [JsonIgnore]
    public override bool Stackable => false;
    [JsonIgnore]
    public override ItemType ItemType => ItemType.Weapon;
    
    //Base IEquippable implementations
    public int RequiredLevel { get; set; }
    public CharacterClass RequiredClass { get; set;  }
    public Quality Quality { get; set; }
    public double UpgradeModifier { get; set; }
    public List<Galdurite> Galdurites { get; set; }

    //Weapon implementations
    
    public WeaponHead Head { get; set; }
    public WeaponBinder Binder { get; set; }
    public WeaponHandle Handle { get; set; }
    [JsonIgnore]
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
    [JsonIgnore]
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
    [JsonIgnore]
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
            value *= EquippableItemService.RarityModifier(Rarity) * (Head.CritChanceBonus + Handle.CritChanceBonus + 1);
            return value;
        }
    }
    [JsonIgnore]
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
            value *= EquippableItemService.RarityModifier(Rarity) * (Binder.CritModBonus + Handle.CritModBonus + 1);
            return value;
        }
    }
    [JsonIgnore]
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
            value *= EquippableItemService.RarityModifier(Rarity) * (Head.AccuracyBonus + Binder.AccuracyBonus + 1);
            return (int)value;
        }
    }

    public Weapon(WeaponHead head, WeaponBinder binder, WeaponHandle handle, CharacterClass requiredClass, Quality quality)
    {
        Head = head;
        Binder = binder;
        Handle = handle;
        Name = NameAliasHelper.GetName(Head.Adjective) + " " + requiredClass switch
        {
            CharacterClass.Warrior => locale.Longsword,
            CharacterClass.Scout => locale.SwordAndDagger,
            CharacterClass.Sorcerer => locale.Wand,
            _ => locale.Hammer
        } + quality switch
        {
            Quality.Weak => $" ({locale.Weak})",
            Quality.Excellent => $" ({locale.Excellent})",
            _ => ""
        };
        Alias = $"{head.Alias}.{binder.Alias}.{handle.Alias}";
        BaseCost = head.MaterialCost * ItemManager.GetItem(head.Material).Cost + 
                   binder.MaterialCost * ItemManager.GetItem(binder.Material).Cost + 
                   handle.MaterialCost * ItemManager.GetItem(handle.Material).Cost;
        Rarity = EquippableItemService.GetRandomRarity();
        RequiredLevel = Math.Max(Math.Max(head.Tier, binder.Tier), handle.Tier) * 10 - 5 + Quality switch
        {
            Quality.Weak => -3,
            Quality.Normal => 0,
            Quality.Excellent => 3,
            Quality.Masterpiece => 5,
            _ => 0
        };
        RequiredClass = requiredClass;
        Quality = quality;
        UpgradeModifier = 1;
        Galdurites = new List<Galdurite>();
    }
    /// <summary>
    /// Gets Starter weapon for the specified class
    /// </summary>
    /// <param name="requiredClass"></param>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    public Weapon(CharacterClass requiredClass)
    {
        switch (requiredClass)
        {
            case CharacterClass.Warrior:
                Head = EquipmentPartManager.GetPart<WeaponHead>("BrokenHead");
                Binder = EquipmentPartManager.GetPart<WeaponBinder>("BrokenBinder");
                Handle = EquipmentPartManager.GetPart<WeaponHandle>("BrokenHandle");
                Name = NameAliasHelper.GetName(Head.Adjective) + " " + locale.Longsword;
                break;
            case CharacterClass.Scout:
                Head = EquipmentPartManager.GetPart<WeaponHead>("RustyHead");
                Binder = EquipmentPartManager.GetPart<WeaponBinder>("RustyBinder");
                Handle = EquipmentPartManager.GetPart<WeaponHandle>("RustyHandle");
                Name = NameAliasHelper.GetName(Head.Adjective) + " " + locale.SwordAndDagger;
                break;
            case CharacterClass.Sorcerer:
                Head = EquipmentPartManager.GetPart<WeaponHead>("SplinteryHead");
                Binder = EquipmentPartManager.GetPart<WeaponBinder>("SplinteryBinder");
                Handle = EquipmentPartManager.GetPart<WeaponHandle>("SplinteryHandle");
                Name = NameAliasHelper.GetName(Head.Adjective) + " " + locale.Wand;
                break;
            case CharacterClass.Paladin:
                Head = EquipmentPartManager.GetPart<WeaponHead>("MisshapenHead");
                Binder = EquipmentPartManager.GetPart<WeaponBinder>("MisshapenBinder");
                Handle = EquipmentPartManager.GetPart<WeaponHandle>("MisshapenHandle");
                Name = NameAliasHelper.GetName(Head.Adjective) + " " + locale.Hammer;
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
        Galdurites = new List<Galdurite>();
    }
    public Weapon() {}

    public bool Use()
    {
        if (RequiredClass != PlayerHandler.player.CharacterClass)
        {
            AnsiConsole.Write(new Text($"{locale.WrongClass} ({NameAliasHelper.GetName(RequiredClass.ToString())})", 
                Stylesheet.Styles["error"]));
            return false;
        }
        if (RequiredLevel > PlayerHandler.player.Level)
        {
            AnsiConsole.Write(new Text($"{locale.LevelTooLow} ({RequiredLevel})", Stylesheet.Styles["error"]));
            return false;
        }
        PlayerHandler.player.SwitchWeapon(this);
        return true;
    }

    public override void Inspect(int amount = 1)
    {
        base.Inspect(amount);
        var playerWeapon = PlayerHandler.player.Weapon;
        var averagePlayerDamage = (playerWeapon.MinimalAttack + playerWeapon.MaximalAttack) / 2;
        var averageDamage = (MinimalAttack + MaximalAttack) / 2;
        AnsiConsole.Write(new Text($"{locale.Level} {RequiredLevel}, +{UpgradeModifier-1:P0}\n", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"{locale.Attack}: {MinimalAttack}-{MaximalAttack}", Stylesheet.Styles["default"]));
        WriteComparator(averageDamage, averagePlayerDamage);
        if (RequiredClass == CharacterClass.Sorcerer)
        {
            AnsiConsole.Write(new Text($"\n{locale.ManaShort}: {Accuracy:F0}"));
            WriteComparator(Accuracy, playerWeapon.Accuracy);
            AnsiConsole.Write(new Text($" ({CritChance:F0}/t)"));
            WriteComparator(CritChance, playerWeapon.CritChance);
            AnsiConsole.Write(new Text($"\n{locale.Crit}: {CritMod:P2}"));
            WriteComparator(CritMod, playerWeapon.CritMod);
        }
        else
        {
            AnsiConsole.Write(new Text($"\n{locale.Crit}: {CritChance:P2}"));
            WriteComparator(CritChance, playerWeapon.CritChance);
            AnsiConsole.Write(new Text($" ({CritMod:F2}x)"));
            WriteComparator(CritMod, playerWeapon.CritMod);
            AnsiConsole.Write(new Text($"\n{locale.Accuracy}: {Accuracy}"));
            WriteComparator(Accuracy, playerWeapon.Accuracy);
        }
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