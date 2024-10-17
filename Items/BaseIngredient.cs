using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public class BaseIngredient : IItem
{
    public string Name { get; set; }
    public int Weight { get; set; }
    public int ID { get; set; }
    public int Cost { get; set; }
    public ItemRarity Rarity { get; set; }
    public bool Stackable { get; set; }
    public string Description { get; set; }
    
    public ItemType ItemType { get; set; }

    public BaseIngredient() // For JSON deserialization
    {
        Stackable = true;
        Weight = 0;
    }  

    public BaseIngredient(string name, int id, int cost, ItemRarity rarity, string desc, ItemType itemType)
    {
        Name = name;
        Weight = 0;
        ID = id;
        Cost = cost;
        Rarity = rarity;
        Stackable = true;
        Description = desc;
        ItemType = itemType;
    }
}