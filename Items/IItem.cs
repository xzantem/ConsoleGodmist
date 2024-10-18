using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public interface IItem
{
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : 
        locale.ResourceManager.GetString(Alias);
    public string Alias { get; }
    public int Weight { get; }
    public int ID { get; }
    public int Cost { get; }
    public ItemRarity Rarity { get; }
    public bool Stackable { get; }
    public string Description { get; }
    public ItemType ItemType { get; }
    public bool Use() { return false; }

    public void Inspect(int amount = 1)
    {
        AnsiConsole.Write(new Text($"\n{Name}", NameStyle()));
        if (Stackable)
            AnsiConsole.Write(new Text($" {amount}x", NameStyle()));
        AnsiConsole.Write(new Text($"\n{RarityName()}, ", NameStyle()));
        AnsiConsole.Write(new Text($"{ItemTypeName()}", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"\n{Cost}cr", Stylesheet.Styles["gold"]));
        if (Stackable)
            AnsiConsole.Write(new Text($" ({Cost*amount}cr)", Stylesheet.Styles["gold"]));
        AnsiConsole.Write(new Text($" | {Weight}kg", Stylesheet.Styles["default"]));
        if (Stackable)
            AnsiConsole.Write(new Text($" ({Weight*amount}kg)", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"\n{Description}\n", Stylesheet.Styles["default-cursive"]));
    }

    public void WriteName()
    {
        AnsiConsole.Write(new Text(Name,NameStyle()));
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
        AnsiConsole.Write(new Text($"[{ItemTypeName()}]", Stylesheet.Styles["default"]));
    }

    public void WriteRarity()
    {
        AnsiConsole.Write(new Text($" ({RarityName})", NameStyle()));
    }

    public string RarityName()
    {
        return Rarity switch
        {
            ItemRarity.Common => locale.Common,
            ItemRarity.Uncommon => locale.Uncommon,
            ItemRarity.Rare => locale.Rare,
            ItemRarity.Ancient => locale.Ancient,
            ItemRarity.Legendary => locale.Legendary,
            ItemRarity.Mythical => locale.Mythical,
            ItemRarity.Godly => locale.Godly,
            ItemRarity.Destroyed => locale.Destroyed,
            ItemRarity.Damaged => locale.Damaged,
            ItemRarity.Junk => locale.Junk,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public string ItemTypeName()
    {
        return ItemType switch
        {
            ItemType.Weapon => locale.Weapon,
            ItemType.Armor => locale.Armor,
            ItemType.Smithing => locale.Smithing,
            ItemType.Alchemy => locale.Alchemy,
            ItemType.Potion => locale.Potion,
            ItemType.Runeforging => locale.Runeforging,
            ItemType.WeaponGaldurite => locale.WeaponGaldurite,
            ItemType.ArmorGaldurite => locale.ArmorGaldurite,
            ItemType.LootBag => locale.LootBag,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}