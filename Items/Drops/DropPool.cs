namespace ConsoleGodmist.Items;

public class DropPool
{
    private Dictionary<ItemDrop, int> Pool { get; set; }

    public DropPool() { } // For JSON deserialization

    public DropPool(Dictionary<ItemDrop, int> pool) { Pool = pool; }
    public ItemDrop GetRandomDrop() => EngineMethods.RandomChoice(Pool);
}