using ConsoleGodmist.Characters;
using Spectre.Console;

namespace ConsoleGodmist.Items;

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
        var inInventory = PlayerHandler.player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == Alias).Value;
        var toOpen = AnsiConsole.Prompt(new TextPrompt<int>(locale.HowManyToOpen + $" Up to {inInventory}: ")
            .DefaultValue(inInventory)
            .Validate(Validator));
        for (var i = 0; i < toOpen; i++)
        {
            var drops = DropTable.GetDrops(Level);
            foreach (var drop in drops)
                PlayerHandler.player.Inventory.AddItem(drop.Key, drop.Value);
        }
        PlayerHandler.player.Inventory.TryRemoveItem(this, toOpen);
        return false;
        
        ValidationResult Validator(int n) {
            if (n > inInventory) return ValidationResult.Error(locale.ChoseTooMany);
            return n < 0 ? ValidationResult.Error(locale.IntBelowZero) : ValidationResult.Success();
        }
    }
}