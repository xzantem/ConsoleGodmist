using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public class Lootbag : BaseItem, IUsable
{
    public new string Name => NameAliasHelper.GetName(Alias);
    public override int Weight => 0;
    public override bool Stackable => true;
    public override ItemType ItemType => ItemType.LootBag;
    
    public override int ID => Alias switch
    {
        "BonySupplyBag" => 563,
        "LeafySupplyBag" => 564,
        "DemonicSupplyBag" => 565,
        "PirateSupplyBag" => 566,
        "SandySupplyBag" => 567,
        "TempleSupplyBag" => 568,
        "MountainousSupplyBag" => 569,
        "WeaponBag" => 570,
        "ArmorBag" => 571,
        "GalduriteBag" => 572,
        "SkeletonExecutionerBag" => 573,
    };
    public DropTable DropTable { get; set; }
    public Lootbag(string alias, int level, DropTable dropTable)
    {
        Alias = alias;
        Level = level;
        DropTable = dropTable;
    }
    public override ItemRarity Rarity => Level switch
    {
        <=10 => ItemRarity.Common,
        >10 and <= 20 => ItemRarity.Uncommon,
        >20 and <= 30 => ItemRarity.Rare,
        >30 and <= 40 => ItemRarity.Ancient,
        _ => ItemRarity.Legendary
    };
    public int Level { get; set; }
    public bool Use()
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
            AnsiConsole.Write(new Text(locale.PressAnyKey, Stylesheet.Styles["highlight-good"]));
            Console.ReadKey();
            AnsiConsole.Write("\n");
        }
        PlayerHandler.player.Inventory.TryRemoveItem(this, toOpen);
        return false;
        
        ValidationResult Validator(int n) {
            if (n > inInventory) return ValidationResult.Error(locale.ChoseTooMany);
            return n < 0 ? ValidationResult.Error(locale.IntBelowZero) : ValidationResult.Success();
        }
    }
}