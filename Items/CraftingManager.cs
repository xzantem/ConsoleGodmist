using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleGodmist.Items;

public static class CraftingManager
{
    private static readonly Inventory Inventory = PlayerHandler.player.Inventory;

    public static void OpenCraftingMenu(List<ICraftable> possibleItems)
    {
        const int scrollAmount = 10;
        const int pageSize = 10;
        var index = 0;
        
        while (true)
        {
            var tempIndex = 0;
            for (var i = index; i < Math.Min(pageSize + index, possibleItems.Count); i++)
            {
                AnsiConsole.Write(new Text($"\n{1 + i}. {possibleItems[i].CraftedAmount}x {possibleItems[i].Name}: ", 
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
                case 2:
                    var item = ChooseItem(possibleItems.Where(x => x.CraftingRecipe
                        .All(s => s.Value <= Inventory.Items
                            .GetValueOrDefault(ItemManager.GetItem(s.Key)))).ToList());
                    if (item != null)
                        CraftItem(item);
                    break;
                default:
                    return;
            }
        }
    }

    public static ICraftable? ChooseItem(List<ICraftable> possibleItems)
    {
        var list = new List<string>();
        foreach (var item in possibleItems)
        {
            var mainStr = $"{item.CraftedAmount}x {item.Name}: ";
            var ingredientStr = new List<string>();
            foreach (var ingredient in item.CraftingRecipe)
            {
                var ingredientItem = new KeyValuePair<IItem, int>(ItemManager.GetItem(ingredient.Key), ingredient.Value);
                var itemCount = Inventory.Items.GetValueOrDefault(ingredientItem.Key, 0);
                ingredientStr.Add($"{ingredientItem.Key.Name} " + $"({itemCount}/{ingredientItem.Value})");
            }
            list.Add(mainStr + string.Join(", ", ingredientStr));
        }
        var choices = list.ToArray();
        if (choices.Length <= 1)
            return null;
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
            .AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        var chosenItem = possibleItems[Array.IndexOf(choices, choice)];
        return chosenItem;
    }

    public static void CraftItem(ICraftable item)
    {
        var maxAmount = item.CraftingRecipe.Min(x => Inventory.Items
            .GetValueOrDefault(ItemManager.GetItem(x.Key)) / x.Value);
        var amount = AnsiConsole.Prompt(new TextPrompt<int>(locale.HowManyToCraft + $" (x{item.CraftedAmount}) Up to {maxAmount}: ")
            .Validate(Validator));
        if (!UtilityMethods.Confirmation(locale.WantCraftThird, true))
        {
            PlayerHandler.player.Say(locale.NoSorry);
            return;
        }
        foreach (var ingredient in item.CraftingRecipe)
            Inventory.TryRemoveItem(ItemManager.GetItem(ingredient.Key), ingredient.Value * amount);
        Inventory.AddItem(item, item.CraftedAmount * amount);
        AnsiConsole.Write(new Text($"{locale.Crafted}: ", Stylesheet.Styles["value-gained"]));
        item.WriteName();
        AnsiConsole.Write(new Text($" ({amount})", Stylesheet.Styles["default"]));
        AnsiConsole.Write("\n\n");
        return;
        
        ValidationResult Validator(int n) {
            if (n > maxAmount) return ValidationResult.Error(locale.ChoseTooMany);
            return n < 0 ? ValidationResult.Error(locale.IntBelowZero) : ValidationResult.Success();
        }
    }

    public static void CraftWeapon()
    {
        var chosenQuality = Quality.Normal;
        WeaponHead head = null;
        WeaponBinder binder = null;
        WeaponHandle handle = null;
        while (true)
        {
            return;
        }
    }
}