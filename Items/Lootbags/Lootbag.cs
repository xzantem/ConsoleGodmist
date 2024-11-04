using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Items.Lootbags;

public abstract class Lootbag : BaseItem
{
    public new string Name => NameAliasHelper.GetName(Alias);
    public override int Weight => 0;
    public override bool Stackable => true;
    public override ItemType ItemType => ItemType.LootBag;

    public override ItemRarity Rarity => Level switch
    {
        <=10 => ItemRarity.Common,
        >10 and <= 20 => ItemRarity.Uncommon,
        >20 and <= 30 => ItemRarity.Rare,
        >30 and <= 40 => ItemRarity.Ancient,
        _ => ItemRarity.Legendary
    };

    public int Level { get; set; }
}