using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public class CraftableIngredient : BaseItem, ICraftable
{
    public Dictionary<string, int> CraftingRecipe { get; set; }
    public int CraftedAmount { get; set; }

    public CraftableIngredient() // For JSON deserialization
    {
        Stackable = true;
        Weight = 0;
    }

    public CraftableIngredient(string alias, int id, int cost, ItemRarity rarity,
        string desc, ItemType itemType, Dictionary<string, int> craftingRecipe, int craftedAmount)
    {
        Alias = alias;
        ID = id;
        Cost = cost;
        Rarity = rarity;
        Description = desc;
        ItemType = itemType;
        CraftingRecipe = craftingRecipe;
        Stackable = true;
        Weight = 0;
        CraftedAmount = craftedAmount;
    }
}