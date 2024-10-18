namespace ConsoleGodmist.Items;

public class DropTable
{
    public Dictionary<DropPool, double> Table { get; private set; }
    public DropTable() { } // For JSON deserialization
    public DropTable(Dictionary<DropPool, double> table)
    {
        Table = table;
    }
    public Dictionary<IItem, int> GetDrops(int level)
    {
        return (from pool in Table where 
            Random.Shared.NextDouble() > pool.Value 
            select pool.Key.Choice(level)).ToDictionary(
            item => item.Key, 
            item => Random.Shared.Next(item.Value.MinAmount, item.Value.MaxAmount + 1));
    }
}