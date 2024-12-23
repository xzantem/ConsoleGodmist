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

    public static ItemRarity GetRandomRarity(int bias = 0)
    {
        var rarities = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Destroyed, bias > 1 ? 0 : 40 },
            {ItemRarity.Damaged, bias > 2 ? 0 : 60 },
            {ItemRarity.Common, bias > 3 ? 0 : 200 },
            {ItemRarity.Uncommon, bias > 4 ? 0 : 40 },
            {ItemRarity.Rare, bias > 5 ? 0 : 20 },
            {ItemRarity.Ancient, bias > 6 ? 0 : 10 },
            {ItemRarity.Legendary, bias > 7 ? 0 : 5 },
            {ItemRarity.Mythical, bias > 8 ? 0 : 2 },
            {ItemRarity.Godly, 1 }
        };
        return UtilityMethods.RandomChoice(rarities);
    }
    public static ItemRarity GetRandomGalduriteRarity(int bias = 0)
    {
        var rarities = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Common, bias > 3 ? 0 : 50 },
            {ItemRarity.Uncommon, bias > 4 ? 0 : 35 },
            {ItemRarity.Rare, bias > 5 ? 0 : 25 },
            {ItemRarity.Ancient, bias > 6 ? 0 : 20 },
            {ItemRarity.Legendary, bias > 7 ? 0 : 15 },
            {ItemRarity.Mythical, bias > 8 ? 0 : 10 },
            {ItemRarity.Godly, 5 }
        };
        return UtilityMethods.RandomChoice(rarities);
    }

    public static Weapon GetRandomWeapon(int tier, CharacterClass requiredClass)
    {
        return new Weapon(EquipmentPartManager.GetRandomPart<WeaponHead>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    public static Weapon GetRandomWeapon(int tier)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        return new Weapon(EquipmentPartManager.GetRandomPart<WeaponHead>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    public static IEquippable GetBossDrop(int tier, string bossAlias)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        var dropData = (bossAlias, requiredClass) switch {
            ("SkeletonExecutioner", CharacterClass.Warrior) => ("Armor", "MadmansHauberk"),
            ("SkeletonExecutioner", CharacterClass.Scout) => ("Armor", "PlaceholderName"),
            ("SkeletonExecutioner", CharacterClass.Sorcerer) => ("Armor", "RunicRobes"),
            ("SkeletonExecutioner", CharacterClass.Paladin) => ("Weapon", "ExecutionersHammer"),
        };
        switch (dropData.Item1)
        {
            case "Weapon":
                return new Weapon(EquipmentPartManager.GetRandomPart<WeaponHead>(tier, requiredClass),
                    EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, requiredClass),
                    EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, requiredClass),
                    requiredClass, Quality.Masterpiece, dropData.Item2);
            case "Armor":
                return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
                    EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
                    EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
                    requiredClass, Quality.Masterpiece, dropData.Item2);
            default:
                throw new ArgumentOutOfRangeException(nameof(dropData.Item1));
                break;
        }
    }
    public static Armor GetRandomArmor(int tier, CharacterClass requiredClass)
    {
        return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    public static Armor GetRandomArmor(int tier)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    public static Armor GetBossArmor(int tier, string bossAlias)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        var alias = (bossAlias, requiredClass) switch
        {
            ("SkeletonExecutioner", CharacterClass.Warrior) => "MadmanHauberk"
        };
        return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
            requiredClass, Quality.Masterpiece, alias);
    }
}