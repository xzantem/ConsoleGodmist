namespace ConsoleGodmist.Items;

public class DropTable
{
    public Dictionary<DropPool, double> Table { get; private set; }
    public DropTable() { } // For JSON deserialization
    public DropTable(Dictionary<DropPool, double> table)
    {
        Table = table;
    }
    public Dictionary<IItem, int> GetDrops()
    {
        return (from pool in Table where 
            Random.Shared.NextDouble() > pool.Value 
            select pool.Key.GetRandomDrop()).ToDictionary(
            item => item.Item, item => Random.Shared.Next(item.MinAmount, item.MaxAmount + 1));
    }
}