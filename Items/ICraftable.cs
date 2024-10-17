namespace ConsoleGodmist.Items;

public interface ICraftable : IItem
{
    public Dictionary<IItem, int> CraftingRecipe { get; set; }
    public int CraftedAmount { get; set; }
}