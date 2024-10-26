using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public static class EquippableItemService
{
    public static double RarityModifier(ItemRarity rarity)
    {
        return rarity switch
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

    public static ItemRarity GetRandomRarity()
    {
        var rarities = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Destroyed, 15 },
            {ItemRarity.Damaged, 25 },
            {ItemRarity.Uncommon, 40 },
            {ItemRarity.Rare, 25 },
            {ItemRarity.Ancient, 10 },
            {ItemRarity.Legendary, 5 },
            {ItemRarity.Mythical, 2 },
            {ItemRarity.Godly, 1 },
        };
        return EngineMethods.RandomChoice(rarities);
    }
}