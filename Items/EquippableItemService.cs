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
            ItemRarity.Uncommon => 1.15f,
            ItemRarity.Rare => 1.25f,
            ItemRarity.Ancient => 1.5f,
            ItemRarity.Legendary => 1.75f,
            ItemRarity.Mythical => 2f,
            ItemRarity.Godly => 2.5f,
            _ => 1f,
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
            {ItemRarity.Godly, 1 },
        };
        return EngineMethods.RandomChoice(rarities);
    }
}