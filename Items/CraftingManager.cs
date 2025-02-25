using System.Reflection.Metadata.Ecma335;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
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
        var quality = Quality.Normal;
        var player = PlayerHandler.player;
        WeaponHead head = null;
        WeaponBinder binder = null;
        WeaponHandle handle = null;
        while (true)
        {
            var costMultiplier = quality switch
            {
                Quality.Weak => 0.5,
                Quality.Normal => 1,
                Quality.Excellent => 2,
                Quality.Masterpiece => 2
            }; 
            AnsiConsole.Write($"{locale.Part} 1 - ");
            AnsiConsole.Write(head == null ? $"{locale.None}\n" : head.DescriptionText(costMultiplier));
            AnsiConsole.Write($"\n{locale.Part} 2 - ");
            AnsiConsole.Write(binder == null ? $"{locale.None}\n" : binder.DescriptionText(costMultiplier));
            AnsiConsole.Write($"\n{locale.Part} 3 - ");
            AnsiConsole.Write(handle == null ? $"{locale.None}\n" : handle.DescriptionText(costMultiplier));

            var levelMultiplier = quality switch
            {
                Quality.Weak => "-3",
                Quality.Normal => locale.Unchanged,
                Quality.Excellent => "+3",
                Quality.Masterpiece => "+5"
            };
            AnsiConsole.Write($"\n{locale.Quality}: {NameAliasHelper.GetName(quality.ToString())} " +
                              $"({locale.Level} {levelMultiplier})\n\n");
            string[] choices = [locale.CreateWeapon, locale.ChangePart + " 1", locale.ChangePart + " 2", 
                locale.ChangePart + " 3", locale.ChangeQuality, locale.Return];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
                .HighlightStyle(Stylesheet.Styles["npc-blacksmith"]));
            switch (Array.IndexOf(choices, choice))
            {
                case 0:
                    if (head != null && binder != null && handle != null)
                    {
                        if (player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == head.Material).Value <
                            head.MaterialCost * costMultiplier ||
                            player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == binder.Material).Value <
                            binder.MaterialCost * costMultiplier ||
                            player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == handle.Material).Value <
                            handle.MaterialCost * costMultiplier)
                            AnsiConsole.Write(new Text($"{locale.NotEnoughMaterials}\n", Stylesheet.Styles["error"]));
                        else
                        {
                            var prompt = new TextPrompt<string>($"{locale.ChooseWeaponName}: ").Validate(n => n.Length switch
                            {
                                > 32 => ValidationResult.Error(locale.NameTooLong),
                                <= 32 => ValidationResult.Success(),
                            });
                            prompt.AllowEmpty = false;
                            var name = AnsiConsole.Prompt(prompt);
                            var weapon = new Weapon(head, binder, handle, player.CharacterClass, quality, name);
                            weapon.Inspect();
                            if (!UtilityMethods.Confirmation(locale.WantCraftThird, true)) return;
                            player.Inventory.TryRemoveItem(ItemManager.GetItem(head.Material),
                                (int)(head.MaterialCost * costMultiplier));
                            player.Inventory.TryRemoveItem(ItemManager.GetItem(binder.Material),
                                (int)(binder.MaterialCost * costMultiplier));
                            player.Inventory.TryRemoveItem(ItemManager.GetItem(handle.Material),
                                (int)(handle.MaterialCost * costMultiplier));
                            player.Inventory.AddItem(weapon);
                        }
                    }
                    else
                    {
                        AnsiConsole.Write(new Text($"{locale.PartsNotSelected}\n", Stylesheet.Styles["error"]));
                    }
                    break;
                case 1:
                    var heads = EquipmentPartManager.WeaponHeads
                        .Where(x => x.IntendedClass == player.CharacterClass && x.Material != "None").ToArray();
                    var headsChoices = heads.Select(h => h.DescriptionText(costMultiplier)).ToArray();
                    head = heads[Array.IndexOf(headsChoices, AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(headsChoices)
                        .HighlightStyle(Stylesheet.Styles["npc-blacksmith"])))];
                    break;
                case 2: 
                    var binders = EquipmentPartManager.WeaponBinders
                        .Where(x => x.IntendedClass == player.CharacterClass && x.Material != "None").ToArray();
                    var bindersChoices = binders.Select(b => b.DescriptionText(costMultiplier)).ToArray();
                    binder = binders[Array.IndexOf(bindersChoices, AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(bindersChoices)
                        .HighlightStyle(Stylesheet.Styles["npc-blacksmith"])))];
                    break;
                case 3: 
                    var handles = EquipmentPartManager.WeaponHandles
                        .Where(x => x.IntendedClass == player.CharacterClass && x.Material != "None").ToArray();
                    var handleChoices = handles.Select(h => h.DescriptionText(costMultiplier)).ToArray();
                    handle = handles[Array.IndexOf(handleChoices, AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(handleChoices)
                        .HighlightStyle(Stylesheet.Styles["npc-blacksmith"])))];
                    break;
                case 4:
                    var qualities = Enum.GetNames(typeof(Quality)).Select(NameAliasHelper.GetName).ToArray();
                    quality = Enum.GetValues<Quality>()[
                        Array.IndexOf(qualities, AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(qualities)
                        .HighlightStyle(Stylesheet.Styles["npc-blacksmith"])))];
                    break;
                case 5: return;
            }
        }
    }
    
    public static void CraftArmor()
    {
        var quality = Quality.Normal;
        var player = PlayerHandler.player;
        ArmorPlate? plate = null;
        ArmorBinder? binder = null;
        ArmorBase? armorBase = null;
        while (true)
        {
            var costMultiplier = quality switch
            {
                Quality.Weak => 0.5,
                Quality.Normal => 1,
                Quality.Excellent => 2,
                Quality.Masterpiece => 2,
                _ => throw new ArgumentOutOfRangeException()
            }; 
            AnsiConsole.Write($"{locale.Part} 1 - ");
            AnsiConsole.Write(plate == null ? $"{locale.None}\n" : plate.DescriptionText(costMultiplier));
            AnsiConsole.Write($"{locale.Part} 2 - ");
            AnsiConsole.Write(binder == null ? $"{locale.None}\n" : binder.DescriptionText(costMultiplier));
            AnsiConsole.Write($"{locale.Part} 3 - ");
            AnsiConsole.Write(armorBase == null ? $"{locale.None}\n" : armorBase.DescriptionText(costMultiplier));

            var levelMultiplier = quality switch
            {
                Quality.Weak => "-3",
                Quality.Normal => locale.Unchanged,
                Quality.Excellent => "+3",
                Quality.Masterpiece => "+5",
                _ => throw new ArgumentOutOfRangeException()
            };
            AnsiConsole.Write($"{locale.Quality}: {NameAliasHelper.GetName(quality.ToString())} " +
                              $"({locale.Level} {levelMultiplier})\n\n");
            string[] choices = [locale.CreateArmor, locale.ChangePart + " 1", locale.ChangePart + " 2", 
                locale.ChangePart + " 3", locale.ChangeQuality, locale.Return];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
                .HighlightStyle(Stylesheet.Styles["npc-blacksmith"]));
            switch (Array.IndexOf(choices, choice))
            {
                case 0:
                    if (plate != null && binder != null && armorBase != null)
                    {
                        if (player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == plate.Material).Value <
                            plate.MaterialCost * costMultiplier ||
                            player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == binder.Material).Value <
                            binder.MaterialCost * costMultiplier ||
                            player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == armorBase.Material).Value <
                            armorBase.MaterialCost * costMultiplier)
                            AnsiConsole.Write(new Text($"{locale.NotEnoughMaterials}\n", Stylesheet.Styles["error"]));
                        else
                        {
                            var prompt = new TextPrompt<string>($"{locale.ChooseArmorName}: ").Validate(n => n.Length switch
                            {
                                > 32 => ValidationResult.Error(locale.NameTooLong),
                                <= 32 => ValidationResult.Success(),
                            });
                            prompt.AllowEmpty = false;
                            var name = AnsiConsole.Prompt(prompt);
                            var armor = new Armor(plate, binder, armorBase, player.CharacterClass, quality, name);
                            armor.Inspect();
                            if (!UtilityMethods.Confirmation(locale.WantCraftThird, true)) return;
                            player.Inventory.TryRemoveItem(ItemManager.GetItem(plate.Material),
                                (int)(plate.MaterialCost * costMultiplier));
                            player.Inventory.TryRemoveItem(ItemManager.GetItem(binder.Material),
                                (int)(binder.MaterialCost * costMultiplier));
                            player.Inventory.TryRemoveItem(ItemManager.GetItem(armorBase.Material),
                                (int)(armorBase.MaterialCost * costMultiplier));
                            player.Inventory.AddItem(armor);
                            return;
                        }
                    }
                    else
                    {
                        AnsiConsole.Write(new Text($"{locale.PartsNotSelected}\n", Stylesheet.Styles["error"]));
                    }
                    break;
                case 1:
                    var plates = EquipmentPartManager.ArmorPlates
                        .Where(x => x.IntendedClass == player.CharacterClass && x.Material != "None").ToArray();
                    var platesChoices = plates.Select(p => p.DescriptionText(costMultiplier)).ToArray();
                    plate = plates[Array.IndexOf(platesChoices, AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(platesChoices)
                        .HighlightStyle(Stylesheet.Styles["npc-blacksmith"])))];
                    break;
                case 2: 
                    var binders = EquipmentPartManager.ArmorBinders
                        .Where(x => x.IntendedClass == player.CharacterClass && x.Material != "None").ToArray();
                    var bindersChoices = binders.Select(b => b.DescriptionText(costMultiplier)).ToArray();
                    binder = binders[Array.IndexOf(bindersChoices, AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(bindersChoices)
                        .HighlightStyle(Stylesheet.Styles["npc-blacksmith"])))];
                    break;
                case 3: 
                    var bases = EquipmentPartManager.ArmorBases
                        .Where(x => x.IntendedClass == player.CharacterClass && x.Material != "None").ToArray();
                    var basesChoices = bases.Select(b => b.DescriptionText(costMultiplier)).ToArray();
                    armorBase = bases[Array.IndexOf(basesChoices, AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(basesChoices)
                        .HighlightStyle(Stylesheet.Styles["npc-blacksmith"])))];
                    break;
                case 4:
                    var qualities = Enum.GetNames(typeof(Quality)).Select(NameAliasHelper.GetName).ToArray();
                    quality = Enum.GetValues<Quality>()[
                        Array.IndexOf(qualities, AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(qualities)
                        .HighlightStyle(Stylesheet.Styles["npc-blacksmith"])))];
                    break;
                case 5: return;
            }
        }
    }

    public static void CraftPotion()
    {
        var player = PlayerHandler.player;
        PotionCatalyst? catalyst = null;
        var components = new PotionComponent?[3];
        while (true)
        {
            var alcohol = components.All(x => x== null) ? locale.None : 
                components.Max(x => x == null ? ItemRarity.Damaged : ItemManager.GetItem(x.Material).Rarity) switch
            {
                ItemRarity.Common => "WeakAlcohol",
                ItemRarity.Uncommon => "Alcohol",
                ItemRarity.Rare => "GoodAlcohol",
                ItemRarity.Ancient => "StrongAlcohol",
                ItemRarity.Legendary => "PerfectAlcohol",
                _ => throw new ArgumentOutOfRangeException()
            };
            AnsiConsole.Write($"{locale.Base}: {NameAliasHelper.GetName(alcohol)}\n");
            for (var i = 0; i < components.Length; i++)
                AnsiConsole.Write(components[i] == null ? $"{locale.Effect} {i+1} - {locale.None}\n" : 
                    $"{locale.Effect} {i+1} {components[i]!.EffectDescription(catalyst, true, 3)}");
            AnsiConsole.Write(catalyst == null ? $"{locale.Catalyst}: {locale.None}\n" : 
                $"{locale.Catalyst}: {catalyst.DescriptionText()}");
            string[] choices = [locale.CreatePotion, locale.ChangeEffect + " 1", locale.ChangeEffect + " 2", 
                locale.ChangeEffect + " 3", locale.ChangeCatalyst, locale.Return];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
                .HighlightStyle(Stylesheet.Styles["npc-alchemist"]));
            switch (Array.IndexOf(choices, choice))
            {
                case 0:
                    if (components.All(x => x != null))
                    {
                        if (!components.All(c => player.Inventory.Items
                                .Any(i => i.Key.Alias == c.Material && i.Value >= 3)) || (catalyst != null && player.Inventory.Items
                                .All(i => i.Key.Alias != catalyst?.Material)) || player.Inventory.Items
                                .All(i => i.Key.Alias != alcohol))
                        {
                            AnsiConsole.Write(new Text($"{locale.NotEnoughMaterials}\n", Stylesheet.Styles["error"]));
                            break;
                        }
                        var prompt = new TextPrompt<string>($"{locale.ChoosePotionName}: ").Validate(n => n.Length switch
                        {
                            > 32 => ValidationResult.Error(locale.NameTooLong),
                            <= 32 => ValidationResult.Success(),
                        });
                        prompt.AllowEmpty = false;
                        var name = AnsiConsole.Prompt(prompt);
                        var potion = new Potion(name, "", components.ToList(), catalyst);
                        potion.Inspect();
                        if (!UtilityMethods.Confirmation(locale.WantCraftThird, true)) return;
                        foreach (var component in components)
                            player.Inventory.TryRemoveItem(ItemManager.GetItem(component.Material), 3);
                        player.Inventory.TryRemoveItem(ItemManager.GetItem(alcohol));
                        if (catalyst != null) player.Inventory.TryRemoveItem(ItemManager.GetItem(catalyst.Material));
                        player.Inventory.AddItem(potion);
                        return;
                    }
                    AnsiConsole.Write(new Text($"{locale.EffectsNotSelected}\n", Stylesheet.Styles["error"]));
                    break;
                case 1:
                case 2: 
                case 3: 
                    var effects = PotionManager.PotionComponents.Where(x => !components.Contains(x)).ToArray();
                    var effectsChoices = effects.Select(e => e.EffectDescription(catalyst, true, 3)).ToArray();
                    components[Array.IndexOf(choices, choice) - 1] = effects[Array.IndexOf(effectsChoices, AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(effectsChoices)
                        .HighlightStyle(Stylesheet.Styles["npc-alchemist"])))];
                    break;
                case 4:
                    var catalysts = new List<PotionCatalyst>();
                    var catalystsTexts = new List<string>();
                    var catalystEffects = Enum.GetValues<PotionCatalystEffect>();
                    foreach (var effect in catalystEffects)
                    {
                        for (var i = 1; i < 6; i++)
                        {
                            var c = new PotionCatalyst(effect, i);
                            catalysts.Add(c);
                            catalystsTexts.Add($"{c.DescriptionText()}");
                        }
                    }
                    catalyst = catalysts.ElementAt(Array.IndexOf(catalystsTexts.ToArray(), AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(catalystsTexts)
                        .HighlightStyle(Stylesheet.Styles["npc-alchemist"]))));
                    break;
                case 5: return;
            }
        }
    }

    public static void CraftGaldurite()
    {
        var player = PlayerHandler.player;
        var galduriteType = false;
        var color = "None";
        var tier = 1;
        while (true)
        {
            var stone = ItemManager.GetItem(tier switch
            {
                1 => "LimestoneGalduriteStone",
                2 => "GraniteGalduriteStone",
                3 => "MarbleGalduriteStone",
                _ => throw new ArgumentOutOfRangeException()
            });
            var powder = GalduriteManager.GetColorMaterial(tier, color);
            if (powder == "")
                AnsiConsole.Write($"{locale.Color}: {NameAliasHelper.GetName(color)}\n");
            else
                AnsiConsole.Write($"{locale.Color}: {NameAliasHelper.GetName(color + "MAdj")} ({NameAliasHelper.GetName(powder)} " +
                                  $"({player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == powder).Value}))\n");
            AnsiConsole.Write(galduriteType ? $"{locale.Type}: {locale.ArmorGaldurite}\n" : $"{locale.Type}: {locale.WeaponGaldurite}\n");
            
            AnsiConsole.Write(tier switch
            {
                1 => $"{locale.Level}: 11 ({stone.Name} ({player.Inventory.Items.FirstOrDefault
                    (x => x.Key.Alias == stone.Alias).Value}))\n",
                2 => $"{locale.Level}: 21 ({stone.Name} ({player.Inventory.Items.FirstOrDefault
                    (x => x.Key.Alias == stone.Alias).Value}))\n",
                3 => $"{locale.Level}: 41 ({stone.Name} ({player.Inventory.Items.FirstOrDefault
                    (x => x.Key.Alias == stone.Alias).Value}))\n",
                _ => throw new ArgumentOutOfRangeException()
            });
            string[] choices = [locale.CreateGaldurite, locale.ChangeType, locale.ChangeColor, 
                locale.ChangeLevel, locale.Return];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
                .HighlightStyle(Stylesheet.Styles["npc-enchanter"]));
            switch (Array.IndexOf(choices, choice))
            {
                case 0:
                    if (color == "None")
                    {
                        AnsiConsole.Write(new Text($"{locale.ColorNotSelected}\n", Stylesheet.Styles["error"]));
                        break;
                    }
                    if (!(player.Inventory.Items.All(x => x.Key.Alias != stone.Alias) || 
                        player.Inventory.Items.All(x => x.Key.Alias != powder)))
                    {
                        var galdurite = new Galdurite(galduriteType, tier, 0, color);
                        if (!UtilityMethods.Confirmation(locale.WantCraftThird, true)) return;
                        galdurite.Reveal();
                        galdurite.Inspect();
                        player.Inventory.TryRemoveItem(stone);
                        player.Inventory.TryRemoveItem(ItemManager.GetItem(powder));
                        player.Inventory.AddItem(galdurite);
                        return;
                    }
                    AnsiConsole.Write(new Text($"{locale.NotEnoughMaterials}\n", Stylesheet.Styles["error"]));
                    break;
                case 1:
                    galduriteType = !galduriteType;
                    break;
                case 2:
                    var colors = new Dictionary<string, string>();
                    foreach (var component in GalduriteManager.GalduriteComponents
                                 .Where(component => !colors.ContainsValue(component.PoolColor))) 
                        colors.Add(NameAliasHelper.GetName(component.PoolColor + "MAdj"), component.PoolColor);
                    var colorChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(colors.Keys)
                        .HighlightStyle(Stylesheet.Styles["npc-enchanter"]));
                    color = colors[colorChoice];
                    break;
                case 3: 
                    string[] tierChoices = [
                        $"11 ({NameAliasHelper.GetName("LimestoneGalduriteStone")} ({player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == "LimestoneGalduriteStone").Value}))", 
                        $"21 ({NameAliasHelper.GetName("GraniteGalduriteStone")} ({player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == "GraniteGalduriteStone").Value}))", 
                        $"41 ({NameAliasHelper.GetName("MarbleGalduriteStone")} ({player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == "MarbleGalduriteStone").Value}))"];
                    var tierChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                        .AddChoices(tierChoices)
                        .HighlightStyle(Stylesheet.Styles["npc-enchanter"]));
                    tier = Array.IndexOf(tierChoices, tierChoice) + 1;
                    break;
                case 4: return;
            }
        }
    }
}