namespace ConsoleGodmist.Items;

public class ItemDrop
{
    private string ItemAlias { get; set; }
    public int MinAmount { get; private set; }
    public int MaxAmount { get; private set; }
    public IItem Item => ItemManager.GetItem(ItemAlias);
    public ItemDrop() { } // For JSON deserialization
    public ItemDrop(string itemAlias, int minAmount = 1, int maxAmount = 1)
    {
        ItemAlias = itemAlias;
        MinAmount = minAmount;
        MaxAmount = maxAmount;
    }
}