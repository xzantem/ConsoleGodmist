using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public interface IItem
{
    public string Name { get; set; }
    public int Weight { get; set; }
    public int ID { get; set; }
    public int Cost { get; set; }
    public ItemRarity Rarity { get; set; }
    public bool Stackable { get; set; }
    public string Description { get; set; }
    public ItemType ItemType { get; set; }
    public bool Use() { return false; }

    public void Inspect()
    {
        
    }

    public void WriteName()
    {
        var style = Rarity switch
        {
            ItemRarity.Destroyed => Stylesheet.Styles["rarity-destroyed"],
            ItemRarity.Damaged => Stylesheet.Styles["rarity-damaged"],
            ItemRarity.Junk => Stylesheet.Styles["rarity-junk"],
            ItemRarity.Common => Stylesheet.Styles["rarity-common"],
            ItemRarity.Uncommon => Stylesheet.Styles["rarity-uncommon"],
            ItemRarity.Rare => Stylesheet.Styles["rarity-rare"],
            ItemRarity.Ancient => Stylesheet.Styles["rarity-ancient"],
            ItemRarity.Legendary => Stylesheet.Styles["rarity-legendary"],
            ItemRarity.Mythical => Stylesheet.Styles["rarity-mythical"],
            ItemRarity.Godly => Stylesheet.Styles["rarity-godly"],
            _ => throw new ArgumentOutOfRangeException()
        };
        AnsiConsole.Write(new Text(Name,style));
    }
    public Style NameStyle()
    {
        return Rarity switch
        {
            ItemRarity.Destroyed => Stylesheet.Styles["rarity-destroyed"],
            ItemRarity.Damaged => Stylesheet.Styles["rarity-damaged"],
            ItemRarity.Junk => Stylesheet.Styles["rarity-junk"],
            ItemRarity.Common => Stylesheet.Styles["rarity-common"],
            ItemRarity.Uncommon => Stylesheet.Styles["rarity-uncommon"],
            ItemRarity.Rare => Stylesheet.Styles["rarity-rare"],
            ItemRarity.Ancient => Stylesheet.Styles["rarity-ancient"],
            ItemRarity.Legendary => Stylesheet.Styles["rarity-legendary"],
            ItemRarity.Mythical => Stylesheet.Styles["rarity-mythical"],
            ItemRarity.Godly => Stylesheet.Styles["rarity-godly"],
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public void WriteItemType()
    {
        var txt = ItemType switch
        {
            ItemType.Weapon => locale.locale_items.Weapon,
            ItemType.Armor => locale.locale_items.Armor,
            ItemType.Smithing => locale.locale_items.Smithing,
            ItemType.Alchemy => locale.locale_items.Alchemy,
            ItemType.Potion => locale.locale_items.Potion,
            ItemType.Runeforging => locale.locale_items.Runeforging,
            ItemType.WeaponGaldurite => locale.locale_items.WeaponGaldurite,
            ItemType.ArmorGaldurite => locale.locale_items.ArmorGaldurite,
            ItemType.LootBag => locale.locale_items.LootBag,
            _ => throw new ArgumentOutOfRangeException()
        };
        AnsiConsole.Write(new Text($"[{txt}]", Stylesheet.Styles["default"]));
    }

    public void WriteRarity()
    {
        var style = Rarity switch
        {
            ItemRarity.Destroyed => Stylesheet.Styles["rarity-destroyed"],
            ItemRarity.Damaged => Stylesheet.Styles["rarity-damaged"],
            ItemRarity.Junk => Stylesheet.Styles["rarity-junk"],
            ItemRarity.Common => Stylesheet.Styles["rarity-common"],
            ItemRarity.Uncommon => Stylesheet.Styles["rarity-uncommon"],
            ItemRarity.Rare => Stylesheet.Styles["rarity-rare"],
            ItemRarity.Ancient => Stylesheet.Styles["rarity-ancient"],
            ItemRarity.Legendary => Stylesheet.Styles["rarity-legendary"],
            ItemRarity.Mythical => Stylesheet.Styles["rarity-mythical"],
            ItemRarity.Godly => Stylesheet.Styles["rarity-godly"],
            _ => throw new ArgumentOutOfRangeException()
        };
        var txt = Rarity switch
        {
            ItemRarity.Common => locale.locale_items.Common,
            ItemRarity.Uncommon => locale.locale_items.Uncommon,
            ItemRarity.Rare => locale.locale_items.Rare,
            ItemRarity.Ancient => locale.locale_items.Ancient,
            ItemRarity.Legendary => locale.locale_items.Legendary,
            ItemRarity.Mythical => locale.locale_items.Mythical,
            ItemRarity.Godly => locale.locale_items.Godly,
            ItemRarity.Destroyed => locale.locale_items.Destroyed,
            ItemRarity.Damaged => locale.locale_items.Damaged,
            ItemRarity.Junk => locale.locale_items.Junk,
            _ => throw new ArgumentOutOfRangeException()
        };
        AnsiConsole.Write(new Text($" ({txt})", style));
    }
}