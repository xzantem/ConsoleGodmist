using ConsoleGodmist.Characters;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleGodmist.Items;

public static class CraftingManager
{
    private static readonly Inventory Inventory = PlayerHandler.player.Inventory;

    public static void CraftItem(List<ICraftable> possibleItems)
    {
        ChooseItem(possibleItems);
    }

    public static int ChooseItem(List<ICraftable> possibleItems)
    {
        const int scrollAmount = 10;
        const int pageSize = 10;
        var index = 0;
        
        while (true)
        {
            var tempIndex = 0;
            /*var grid = new Grid();
            while (grid.Columns.Count < 2 * possibleItems.Max(x => x.CraftingRecipe.Count) + 1)
                grid.AddColumn();
            foreach (var possibleItem in possibleItems)
            {
                while (grid.Columns.Count < 2 * possibleItem.CraftingRecipe.Count + 1)
                    grid.AddColumn();
                var row = new IRenderable[2 * possibleItem.CraftingRecipe.Count + 1];
                row[0] = new Text($"{tempIndex++}. {possibleItem.Name}");
                for (var i = 0; i < possibleItem.CraftingRecipe.Count; i++)
                {
                    var item = ItemManager.GetItem(possibleItem.CraftingRecipe.ElementAt(i).Key);
                    var itemCount = Inventory.Items.GetValueOrDefault(item, 0);
                    row[2 * i + 1] = new Text($"{item.Name}");
                    row[2 * i + 2] = new Text($"({itemCount}/{possibleItem.CraftingRecipe.ElementAt(i).Value})x", 
                        itemCount >= possibleItem.CraftingRecipe.ElementAt(i).Value ? 
                            Stylesheet.Styles["success"] : Stylesheet.Styles["failure"]);
                }
                grid.AddRow(row);
            }*/
            for (var i = index; i < Math.Min(pageSize, possibleItems.Count - index); i++)
            {
                AnsiConsole.Write(new Text($"\n{1 + tempIndex++}. {possibleItems[i].CraftedAmount}x {possibleItems[i].Name}: ", 
                    possibleItems[i].NameStyle()));
                for (var j = 0; j < possibleItems[i].CraftingRecipe.Count; j++)
                {
                    var item = ItemManager.GetItem(possibleItems[i].CraftingRecipe.ElementAt(j).Key);
                    var itemCount = Inventory.Items.GetValueOrDefault(item, 0);
                    AnsiConsole.Write(j != 0 ? new Text($", {item.Name}") : new Text($"{item.Name}"));
                    AnsiConsole.Write(new Text($" ({itemCount}/{possibleItems[i].CraftingRecipe.ElementAt(j).Value})", 
                        itemCount >= possibleItems[i].CraftingRecipe.ElementAt(j).Value ? 
                            Stylesheet.Styles["success"] : Stylesheet.Styles["failure"]));
                }
            }
            AnsiConsole.Write("\n\n");
            Dictionary<string, int> choices = [];
            if (index < possibleItems.Count - scrollAmount)
                choices.Add(locale.GoDown, 0);
            if (index >= scrollAmount)
                choices.Add(locale.GoUp, 1);
            choices.Add(locale.SelectItem, 2);
            choices.Add(locale.Return, 3);
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
}