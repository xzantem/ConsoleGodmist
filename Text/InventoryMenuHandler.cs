﻿using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class InventoryMenuHandler
{
    private static readonly Inventory Inventory = PlayerHandler.player.Inventory;
    public static void OpenInventoryMenu()
    {
        const int scrollAmount = 10;
        const int pageSize = 10;
        var index = 0;
        
        while (true)
        {
            var tempIndex = 0;
            //AnsiConsole.Write(new FigletText(locale.Inventory).Centered().Color(Color.Gold3_1));
            var rows = Inventory.Items.Select(item => 
                new Text(
                    item.Key.Stackable ? 
                        $"{1 + tempIndex++}. {item.Key.Name} - {item.Value}x" : 
                        $"{1 + tempIndex++}. {item.Key.Name}", item.Key.NameStyle())).ToList();
            AnsiConsole.Write(new Rows(rows.GetRange(index, Math.Min(pageSize, rows.Count - index))));
            AnsiConsole.Write(new Text($"{locale.Weight}: {Inventory.PackWeight}/{Inventory.MaxPackWeight}" +
                                       $" | ----- | {locale.Crowns}: {PlayerHandler.player.Gold} cr\n\n"));
            if (Inventory.Items.Count == 0)
            {
                AnsiConsole.Write(new Text(locale.InventoryEmpty + "\n"));
                return;
            }
            Dictionary<string, int> choices = [];
            if (index < rows.Count - scrollAmount)
                choices.Add(locale.GoDown, 0);
            if (index >= scrollAmount)
                choices.Add(locale.GoUp, 1);
            choices.Add(locale.InspectItem, 2);
            choices.Add(locale.DeleteItem, 3);
            choices.Add(locale.DeleteJunk, 4);
            choices.Add(locale.SortInventory, 5);
            choices.Add(locale.UseItem, 6);
            choices.Add(locale.Return, 7);
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices.Keys)
                .HighlightStyle(new Style(Color.Gold3_1)).WrapAround());
            switch (choices[choice])
            {
                case 0:
                    index += scrollAmount;
                    break;
                case 1:
                    index -= scrollAmount;
                    break;
                case 2:
                    AnsiConsole.Write(new Text($"{locale.ChooseItem} {locale.ToInspect}:\n\n", 
                        Stylesheet.Styles["default"]));
                    InspectItem();
                    break;
                case 3:
                    AnsiConsole.Write(new Text($"{locale.ChooseItem} {locale.ToRemove}:\n\n", 
                        Stylesheet.Styles["default"]));
                    DeleteItem();
                    break;
                case 4:
                    if (!UtilityMethods.Confirmation(locale.DeleteJunkConfirmation))
                        break;
                    Inventory.RemoveJunk();
                    break;
                case 5:
                    SortInventory();
                    break;
                case 6:
                    AnsiConsole.Write(new Text($"{locale.ChooseItem} {locale.ToUse}:\n\n", 
                        Stylesheet.Styles["default"]));
                    Inventory.UseItem(ChooseUsableItem());
                    break;
                case 7:
                    return;
            }
        }
    }

    public static int ChooseItem(bool showPrices)
    {
        var tempIndex = 0;
        var choices = Inventory.Items.Select(item =>
            (!showPrices ? item.Key.Stackable
                ? $"{1 + tempIndex++}. {item.Key.Name} - {item.Value}x"
                : $"{1 + tempIndex++}. {item.Key.Name}" 
                : item.Key.Stackable ? 
                    $"{1 + tempIndex++}. {item.Key.Name} - {item.Value}x " +
                    $"[[{item.Key.Cost} ({item.Key.Cost * item.Value}) cr]]" : 
                    $"{1 + tempIndex++}. {item.Key.Name} [[{item.Key.Cost} cr]]" ).ToString()).ToArray();
        if (choices.Length < 1)
            return -1;
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)).WrapAround());
        return Array.IndexOf(choices, choice);
    }

    public static IItem? ChooseUsableItem()
    {
        var tempIndex = 0;
        var items = Inventory.Items.Where(x => x.Key is IUsable).ToList();
        var choices = items.Select(item =>
            (item.Key.Stackable
                ? $"{1 + tempIndex++}. {item.Key.Name} - {item.Value}x"
                : $"{1 + tempIndex++}. {item.Key.Name}").ToString()).ToArray();
        if (choices.Length < 1)
            return null;
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)).WrapAround());
        return items[Array.IndexOf(choices, choice)].Key;
    }
    private static void DeleteItem()
    {
        var item = Inventory.Items.ElementAt(ChooseItem(false));
        if (item.Key.Stackable && item.Value > 1)
        {
            var number = AnsiConsole.Prompt(
                new TextPrompt<int>($"{locale.HowManyToDelete} [[max {item.Value}]] ")
                    .DefaultValue(1)
                    .Validate(Validator));
            if (!UtilityMethods.Confirmation($"{locale.DeleteItemConfirmation}: {item.Key.Name} ({number})?"))
                return;
            Inventory.TryRemoveItem(item.Key, number);
        }
        else
        {
            if (!UtilityMethods.Confirmation($"{locale.DeleteItemConfirmation}: {item.Key.Name}?"))
                return;
            Inventory.TryRemoveItem(item.Key);
        }
        return;

        ValidationResult Validator(int n) {
            if (n > item.Value) return ValidationResult.Error(locale.ChoseTooMany);
            return n < 0 ? ValidationResult.Error(locale.IntBelowZero) : ValidationResult.Success();
        }
    }
    private static void SortInventory()
    {
        AnsiConsole.Write(new Text($"{locale.ChooseSortingMethod}\n", Stylesheet.Styles["default"]));
        string[] choices =
        [
            locale.Type, locale.Rarity, locale.Price, locale.Name
        ];
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)).WrapAround());
        Inventory.SortInventory((SortType)Array.IndexOf(choices, choice));
    }

    private static void InspectItem()
    {
        var item = Inventory.Items.ElementAt(ChooseItem(false));
        AnsiConsole.Write("\n\n");
        item.Key.Inspect(item.Value);
        var cont = AnsiConsole.Prompt(
            new TextPrompt<string>(locale.PressAnyKey)
                .AllowEmpty());
    }
}