using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public class Inventory
{
    [JsonConverter(typeof(ItemConverter))]
    public Dictionary<IItem, int> Items { get; set; } = new();
    public int PackWeight => Items.Sum(item => item.Key.Weight * item.Value);
    public int MaxPackWeight { get; set; } = 60;
    
    public Inventory() {}

    public void AddItem(IItem item, int quantity = 1)
    {
        if (item.Weight * quantity + PackWeight > MaxPackWeight)
        {
            AnsiConsole.WriteLine(item.Stackable
                ? $"{locale.NotEnoughWeight}: {item.Name} + ({quantity})!"
                : $"{locale.NotEnoughWeight}: {item.Name}!", Stylesheet.Styles["error"]);
            return;
        }
        if (item.Stackable && Items.Keys.Any(x => x.Alias == item.Alias))
        {
            Items[Items.FirstOrDefault(x => x.Key.Alias == item.Alias).Key] += quantity;
            AnsiConsole.Write(new Text($"{locale.YouGain}: {quantity}x {item.Name} ({Items[item]})\n", item.NameStyle()));
        }
        else
        {
            Items.Add(item, quantity);
            AnsiConsole.Write(new Text($"{locale.NewItem}! {locale.YouGain}: {quantity}x {item.Name}\n", item.NameStyle()));
        }
    }
    public bool TryRemoveItem(IItem item, int amount = 1)
    {
        if (Items.ContainsKey(item))
        {
            if (Items[item] >= amount)
            {
                Items[item] -= amount;
                if (Items[item] == 0)
                {
                    Items.Remove(item);
                }
                return true;
            }
            AnsiConsole.WriteLine($"{locale.NotEnoughItem}: {item.Name}!", Stylesheet.Styles["error"]);
            return false;
        }
        AnsiConsole.WriteLine($"{locale.ItemNotFound}: {item.Name}!", Stylesheet.Styles["error"]);
        return false;
    }
    public void RemoveJunk()
    {
        foreach (var item in Items)
        {
            if (item.Key.ItemType is not (ItemType.Weapon or ItemType.Armor)) continue;
            if ((item.Key as IEquippable).RequiredClass != PlayerHandler.player.CharacterClass)
                TryRemoveItem(item.Key, item.Value);
        }
    }
    public void SortInventory(SortType sortType)
    {
        Items = sortType switch
        {
            SortType.ItemType => Items.OrderBy(item => item.Key.ItemType)
                .ToDictionary(pair => pair.Key, pair => pair.Value),
            SortType.Rarity => Items.OrderByDescending(item => item.Key.Rarity)
                .ToDictionary(pair => pair.Key, pair => pair.Value),
            SortType.Cost => Items.OrderByDescending(item => item.Key.Cost)
                .ToDictionary(pair => pair.Key, pair => pair.Value),
            SortType.Name => Items.OrderBy(item => item.Key.Name).ToDictionary(pair => pair.Key, pair => pair.Value),
            _ => Items
        };
    }

    public void UseItem(IItem item)
    {
        if (Items.ContainsKey(item))
        {
            if (item is not IUsable usable) return;
            if (usable.Use())
            {
                TryRemoveItem(item);
            }
        }
        else
        {
            AnsiConsole.WriteLine($"{locale.ItemNotFound}: {item.Name}!", Stylesheet.Styles["error"]);
        }
    }
}