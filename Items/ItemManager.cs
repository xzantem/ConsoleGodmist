using System.Globalization;
using System.Text.Json;
using ConsoleGodmist.locale;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public static class ItemManager
{
    public static List<IItem> Items
    {
        get
        {
            var temp = new List<IItem>();
            temp = temp.Concat(BaseIngredients).ToList();
            return temp;
        }
    }
    public static List<BaseIngredient> BaseIngredients { get; private set; }

    public static void InitItems()
    {
        var path = "base-ingredients.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            BaseIngredients = JsonSerializer.Deserialize<List<BaseIngredient>>(json);
            foreach (var ingredient in BaseIngredients)
                ingredient.Name = locale_items.ResourceManager.GetString(ingredient.Name);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
    public static IItem GetItemById(int id)
    {
        return Items.FirstOrDefault(i => i.ID == id);
    }
}