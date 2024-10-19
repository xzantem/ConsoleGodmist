namespace ConsoleGodmist.Items;

public class DropPool
{
    public Dictionary<string, ItemDrop> Pool { get; set; }
    public double[] Chances { get; set; }

    public DropPool() { } // For JSON deserialization

    public DropPool(DropPool other)
    {
        Pool = other.Pool;
        Chances = other.Chances;
    }

    public DropPool(Dictionary<string, ItemDrop> pool) { Pool = pool; }

    public KeyValuePair<IItem, ItemDrop> Choice(int level)
    {
        var choice = EngineMethods.RandomChoice(Pool
            .Where(item => item.Value.MinLevel <= level && item.Value.MaxLevel >= level)
            .ToDictionary(item => item.Key, item => item.Value)
            .ToDictionary(x => x, x => x.Value.Weight));
        return new KeyValuePair<IItem, ItemDrop>(ItemManager.GetItem(choice.Key), choice.Value);
    }

    public KeyValuePair<IItem, int> GetDrop(int level)
    {
        var drop = Choice(level);
        return new KeyValuePair<IItem, int>(drop.Key, 
            Random.Shared.Next(drop.Value.MinAmount, drop.Value.MaxAmount + 1));
    }
}