using System.Globalization;
using System.Text.Json;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items.Armors;
using ConsoleGodmist.Items.Weapons;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public static class ItemManager
{
    public static List<IItem> Items
    {
        get
        {
            var temp = new List<IItem>();
            temp = temp.Concat(BaseIngredients).ToList();
            temp = temp.Concat(CraftableIngredients).ToList();
            return temp;
        }
    }
    public static List<BaseIngredient> BaseIngredients { get; private set; }
    public static List<CraftableIngredient> CraftableIngredients { get; private set; }

    public static void InitItems()
    {
        var path = "json/base-ingredients.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            BaseIngredients = JsonSerializer.Deserialize<List<BaseIngredient>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
        path = "json/craftable-ingredients.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            CraftableIngredients = JsonSerializer.Deserialize<List<CraftableIngredient>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }

    public static IItem GetItem(int id)
    {
        return Items.FirstOrDefault(i => i.ID == id);
    }
    public static IItem GetItem(string alias)
    {
        return Items.FirstOrDefault(i => i.Alias == alias);
    }

    public static Weapon GetRandomWeapon(int tier, bool randomClass)
    {
        var requiredClass =
            randomClass ? EngineMethods.RandomChoice(Enum
                .GetValues(typeof(CharacterClass)).OfType<CharacterClass>().ToList()) 
                : PlayerHandler.player.CharacterClass;
        return new Weapon(
            EquipmentPartManager.GetRandomPart<WeaponHead>(tier),
            EquipmentPartManager.GetRandomPart<WeaponBinder>(tier),
            EquipmentPartManager.GetRandomPart<WeaponHandle>(tier),
            requiredClass, EngineMethods.RandomChoice(Enum
                .GetValues(typeof(Quality)).OfType<Quality>().ToList())
        );
    }
    public static Armor GetRandomArmor(int tier, bool randomClass)
    {
        var requiredClass =
            randomClass ? EngineMethods.RandomChoice(Enum
                    .GetValues(typeof(CharacterClass)).OfType<CharacterClass>().ToList()) 
                : PlayerHandler.player.CharacterClass;
        return new Armor(
            EquipmentPartManager.GetRandomPart<ArmorPlate>(tier),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier),
            requiredClass, EngineMethods.RandomChoice(Enum
                .GetValues(typeof(Quality)).OfType<Quality>().ToList())
        );
    }
}