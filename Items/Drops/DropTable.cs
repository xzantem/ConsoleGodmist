namespace ConsoleGodmist.Items;

public class DropTable
{
    public List<DropPool> Table { get; set; }
    public DropTable() { } // For JSON deserialization
    public DropTable(List<DropPool> table)
    {
        Table = table;
    }
    public Dictionary<IItem, int> GetDrops(int level)
    {
        Dictionary<IItem, int> drops = new();
        foreach (var pool in Table)
        {
            if (pool.Pool == null) continue;
            var poolCopy = new DropPool(pool);
            foreach (var t in poolCopy.Chances)
            {
                if (!poolCopy.Pool.Any(x => x.Value.MinLevel <= level && x.Value.MaxLevel >= level)) break;
                if (!(Random.Shared.NextDouble() < t)) continue;
                var pair = poolCopy.Choice(level);
                drops.Add(pair.Key, Random.Shared.Next(pair.Value.MinAmount, pair.Value.MaxAmount + 1));
                poolCopy.Pool.Remove(pair.Key.Alias);
            }
        }
        return drops;
    }
}