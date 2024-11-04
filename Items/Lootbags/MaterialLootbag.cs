using ConsoleGodmist.Characters;

namespace ConsoleGodmist.Items.Lootbags;

public class MaterialLootbag : Lootbag
{
    public override int ID => Alias switch
    {
        "BonySupplyBag" => 563,
        "LeafySupplyBag" => 564,
        "DemonicSupplyBag" => 565,
        "PirateSupplyBag" => 566,
        "SandySupplyBag" => 567,
        "TempleSupplyBag" => 568,
        "MountainousSupplyBag" => 569,
        "MurkySupplyBag" => 570,
    };
    public DropTable DropTable { get; set; }
    public MaterialLootbag(string alias, int level, DropTable dropTable)
    {
        Alias = alias;
        Level = level;
        DropTable = dropTable;
    }

    public override bool Use()
    {
        var drops = DropTable.GetDrops(Level);
        foreach (var drop in drops)
            PlayerHandler.player.Inventory.AddItem(drop.Key, drop.Value);
        return true;
    }
}