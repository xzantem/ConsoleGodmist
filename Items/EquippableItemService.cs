using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public static class EquippableItemService
{
    public static double RarityModifier(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Destroyed => 0.7,
            ItemRarity.Damaged => 0.85,
            ItemRarity.Uncommon => 1.15,
            ItemRarity.Rare => 1.25,
            ItemRarity.Ancient => 1.5,
            ItemRarity.Legendary => 1.75,
            ItemRarity.Mythical => 2,
            ItemRarity.Godly => 2.5,
            _ => 1
        };
    }

    public static ItemRarity GetRandomRarity()
    {
        var rarities = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Destroyed, 40 },
            {ItemRarity.Damaged, 60 },
            {ItemRarity.Common, 200 },
            {ItemRarity.Uncommon, 40 },
            {ItemRarity.Rare, 20 },
            {ItemRarity.Ancient, 10 },
            {ItemRarity.Legendary, 5 },
            {ItemRarity.Mythical, 2 },
            {ItemRarity.Godly, 1 }
        };
        return EngineMethods.RandomChoice(rarities);
    }

    public static Weapon GetRandomWeapon(int tier)
    {
        return new Weapon(EquipmentPartManager.GetRandomPart<WeaponHead>(tier),
            EquipmentPartManager.GetRandomPart<WeaponBinder>(tier),
            EquipmentPartManager.GetRandomPart<WeaponHandle>(tier),
            EngineMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList()),
            EngineMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    public static Armor GetRandomArmor(int tier)
    {
        return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier),
            EngineMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList()),
            EngineMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
}