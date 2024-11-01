using ConsoleGodmist.Items;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class NPCMenuHandler
{
    public static int OpenInventoryMenu(NPCInventory inventory)
    {
        const int scrollAmount = 10;
        const int pageSize = 10;
        var index = 0;
        
        while (true)
        {
            var tempIndex = 0;
            var items = inventory.RotatingShop.Concat(inventory.BoughtFromPlayer).ToList();
            var rows = items.Select(item => 
                new Text(
                    item.Key.Stackable ? 
                        $"{1 + tempIndex++}. {item.Key.Name} - {item.Value}x " +
                        $"[{item.Key.Cost} ({item.Key.Cost * item.Value}) cr]" : 
                        $"{1 + tempIndex++}. {item.Key.Name} [{item.Key.Cost} cr]", item.Key.NameStyle())).ToList();
            AnsiConsole.Write(new Rows(rows.GetRange(index, Math.Min(pageSize, rows.Count - index))));
            AnsiConsole.Write("\n\n");
            Dictionary<string, int> choices = [];
            if (index < rows.Count - scrollAmount)
                choices.Add(locale.GoDown, 0);
            if (index >= scrollAmount)
                choices.Add(locale.GoUp, 1);
            choices.Add(locale.InspectItem, 2);
            choices.Add(locale.BuyItem, 3);
            choices.Add(locale.SellItem, 4);
            choices.Add(locale.Return, 5);
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
                default:
                    return choices[choice];
            }
        }
    }
    public static int ChooseItem(NPCInventory inventory)
    {
        var items = inventory.RotatingShop.Concat(inventory.BoughtFromPlayer).ToList();
        var tempIndex = 0;
        var choices = items.Select(item =>
            (item.Key.Stackable ? 
                $"{1 + tempIndex++}. {item.Key.Name} - {item.Value}x " +
                $"[[{item.Key.Cost} ({item.Key.Cost * item.Value}) cr]]" : 
                $"{1 + tempIndex++}. {item.Key.Name} [[{item.Key.Cost} cr]]").ToString()).ToArray();
        if (choices.Length <= 1)
            return 0;
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        return Array.IndexOf(choices, choice);
    }
}