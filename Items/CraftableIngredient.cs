using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public class CraftableIngredient : ICraftable, IItem
{
    public string Alias { get; set; }
    public int Weight { get; set; }
    public int ID { get; set; }
    public int Cost { get; set; }
    public ItemRarity Rarity { get; set; }
    public bool Stackable { get; set; }
    public string Description { get; set; }
    public ItemType ItemType { get; set; }
    
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