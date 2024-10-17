using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.locale;
using Spectre.Console;

namespace ConsoleGodmist.Characters;

public static class InventoryMenuHandler
{
    private static readonly Inventory Inventory = PlayerHandler.player.Inventory;
    public static void OpenInventoryMenu()
    {
        const int scrollAmount = 10;
        const int pageSize = 10;
        var index = 0;
        
        foreach (var item in ItemManager.Items)
            Inventory.AddItem(item);
        while (true)
        {
            var tempIndex = 0;
            AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText(locale_main.Inventory).Centered().Color(Color.Gold3_1));
            var rows = Inventory.Items.Select(item => 
                new Text(
                    item.Key.Stackable ? 
                        $"{1 + tempIndex++}. {item.Key.Name} - {item.Value}x" : 
                        $"{1 + tempIndex++}. {item.Key.Name}", item.Key.NameStyle())).ToList();
            AnsiConsole.Write(new Rows(rows.GetRange(index, Math.Min(pageSize, rows.Count - index))));
            AnsiConsole.Write($"\n\n");
            Dictionary<string, int> choices = [];
            if (index < rows.Count - scrollAmount)
                choices.Add(locale_main.GoDown, 0);
            if (index >= scrollAmount)
                choices.Add(locale_main.GoUp, 1);
            choices.Add(locale_main.InspectItem, 2);
            choices.Add(locale_main.DeleteItem, 3);
            choices.Add(locale_main.DeleteJunk, 4);
            choices.Add(locale_main.SortInventory, 5);
            choices.Add(locale_main.UseItem, 6);
            choices.Add(locale_main.GoBack, 7);
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices.Keys)
                .HighlightStyle(new Style(Color.Gold3_1)));
            switch (choices[choice])
            {
                case 0:
                    index += scrollAmount;
                    break;
                case 1:
                    index -= scrollAmount;
                    break;
                case 2:
                    AnsiConsole.Write(new Text($"{locale.locale_main.ChooseItem} {locale_main.ToInspect}:\n", 
                        Stylesheet.Styles["default"]));
                    Inventory.Items.ElementAt(ChooseItem()).Key.Inspect();
                    break;
                case 3:
                    AnsiConsole.Write(new Text($"{locale.locale_main.ChooseItem} {locale_main.ToRemove}:\n", 
                        Stylesheet.Styles["default"]));
                    DeleteItem();
                    break;
                case 4:
                    if (!EngineMethods.Confirmation(locale_main.DeleteJunkConfirmation))
                        return;
                    Inventory.RemoveJunk();
                    break;
                case 5:
                    SortInventory();
                    break;
                case 6:
                    AnsiConsole.Write(new Text($"{locale.locale_main.ChooseItem} {locale_main.ToUse}:\n", 
                        Stylesheet.Styles["default"]));
                    Inventory.Items.ElementAt(ChooseItem()).Key.Use();
                    break;
                case 7:
                    return;
            }
        }
    }

    private static int ChooseItem()
    {
        
        var tempIndex = 0;
        var choices = Inventory.Items.Select(item =>
            (item.Key.Stackable
                ? $"{1 + tempIndex++}. {item.Key.Name} - {item.Value}x"
                : $"{1 + tempIndex++}. {item.Key.Name}").ToString()).ToArray();
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        return Array.IndexOf(choices, choice);
    }
    private static void DeleteItem()
    {
        var Item = Inventory.Items.ElementAt(ChooseItem());
        if (Item.Key.Stackable && Item.Value > 1)
        {
            var number = AnsiConsole.Prompt(
                new TextPrompt<int>($"{locale_main.HowManyToDelete} (max {Item.Value}): ")
                    .Validate((Func<int, ValidationResult>)Validator));
            if (!EngineMethods.Confirmation($"{locale_main.DeleteItemConfirmation}: {Item.Key.Name} ({number})?"))
                return;
            Inventory.RemoveItem(Item.Key, number);
        }
        else
        {
            if (!EngineMethods.Confirmation($"{locale_main.DeleteItemConfirmation}: {Item.Key.Name}?"))
                return;
            Inventory.RemoveItem(Item.Key);
        }
        return;

        ValidationResult Validator(int n) {
            if (n > Item.Value) return ValidationResult.Error(locale.locale_main.ChoseTooMany);
            else if (n < 0) return ValidationResult.Error(locale.locale_main.IntBelowZero);
            else return ValidationResult.Success(); }
    }
    private static void SortInventory()
    {
        AnsiConsole.Write(new Text($"{locale.locale_main.ChooseSortingMethod}\n", Stylesheet.Styles["default"]));
        string[] choices =
        [
            locale_items.Type, locale_items.Rarity, locale_items.Price, locale_items.Name
        ];
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        Inventory.SortInventory((SortType)Array.IndexOf(choices, choice));
    }
    private static void UseItem()
    {
        
    }
}