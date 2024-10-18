using ConsoleGodmist.Enums;
using ConsoleGodmist.locale;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public class Inventory
{
    public Dictionary<IItem, int> Items { get; private set; } = new();
    private int PackWeight { get {return Items.Sum(item => item.Key.Weight * item.Value); } }
    private int MaxPackWeight { get; set; } = 60;

    public void AddItem(IItem item, int quantity = 1)
    {
        if (item.Weight * quantity + PackWeight > MaxPackWeight)
        {
            AnsiConsole.WriteLine(item.Stackable
                ? $"{locale_main.NotEnoughWeight}: {item.Name} + ({quantity})!"
                : $"{locale_main.NotEnoughWeight}: {item.Name}!", Stylesheet.Styles["error"]);
        }
        if (item.Stackable && Items.ContainsKey(item))
        {
            Items[item] += quantity;
        }
        else
        {
            Items.Add(item, quantity);
        }
    }

    /*public int GetItemCount(int id)
    {
        return Items.ContainsKey(ItemManager.GetItemById(id)) ? Items[ItemManager.GetItemById(id)] : 0;
    }*/

    /*public void RemoveItemById(int id, int amount = 1)
    {
        var item = ItemManager.GetItemById(id);
        if (Items.ContainsKey(item))
        {
            if (Items[item] >= amount)
            {
                Items[item] -= amount;
                if (Items[item] == 0)
                {
                    Items.Remove(item);
                }
            }
            else
            {
                AnsiConsole.WriteLine($"{locale_main.NotEnoughItem}: {item.Name}!", Stylesheet.Styles["error"]);
            }
        }
        else
        {
            AnsiConsole.WriteLine($"{locale_main.ItemNotFound}: {item.Name}!", Stylesheet.Styles["error"]);
        } 
    }*/
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
            AnsiConsole.WriteLine($"{locale_main.NotEnoughItem}: {item.Name}!", Stylesheet.Styles["error"]);
            return false;
        }
        AnsiConsole.WriteLine($"{locale_main.ItemNotFound}: {item.Name}!", Stylesheet.Styles["error"]);
        return false;
    }
    public void RemoveJunk()
    {
        foreach (var item in Items)
        {
            if (item.Key.ItemType is ItemType.Weapon or ItemType.Armor)
            {
                //if (item.Key.RequiredClass != PlayerHandler.player.characterClass)
                    //RemoveItem(item.Key, item.Value);
            }
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
            if (item.Use())
            {
                TryRemoveItem(item);
            }
        }
        else
        {
            AnsiConsole.WriteLine($"{locale_main.ItemNotFound}: {item.Name}!", Stylesheet.Styles["error"]);
        }
    }
}