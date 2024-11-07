using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.TextService;
using ConsoleGodmist.Utilities;
using Spectre.Console;

namespace ConsoleGodmist.Town.NPCs;

public abstract class NPC
{
    public string Alias { get; set; }

    public string Name => NameAliasHelper.GetName(Alias);
    
    public NPCInventory Inventory { get; set; }
    public List<ICraftable?> CraftableItems { get; set; }
     
    public int LoyaltyLevel { get; set; }
    public int GoldSpent { get; set; }
    public int RequiredGoldSpent => CalculateGoldRequired(LoyaltyLevel);

    public double ServiceCostMod => LoyaltyLevel switch
    {
        < 2 => 1.0,
        >= 2 and < 4 => 0.99,
        >= 4 and < 7 => 0.96,
        >= 7 and < 9 => 0.91,
        >= 9 and < 12 => 0.84,
        >= 12 => 0.75
    };

    public int CalculateGoldRequired(int level)
    {
        var value = 0;
        for (var i = 1; i <= Math.Min(level, 14); i++)
        {
            value += (int)Math.Pow(4, i/3.0 + 4);
        }
        return value;
    }

    public abstract void OpenMenu();
    public abstract void Say(string message);
    public void SpendGold(int gold)
    {
        GoldSpent += gold;
        while (GoldSpent >= RequiredGoldSpent) {
            if (LoyaltyLevel < 15)
                LoyaltyLevel++;
            else
            {
                GoldSpent = RequiredGoldSpent;
                return;
            }
        }
        PlayerHandler.player.LoseGold(gold);
    }

    public void DisplayShop()
    {
        while (true)
            switch (NPCMenuHandler.OpenInventoryMenu(Inventory))
            {
                case 2:
                    AnsiConsole.Write(new Text($"{locale.InspectItem}: \n", Stylesheet.Styles["default"]));
                    InspectItem(NPCMenuHandler.ChooseItem(Inventory));
                    break;
                case 3:
                    AnsiConsole.Write(new Text($"{locale.BuyItem}: \n", Stylesheet.Styles["default"]));
                    BuyItem(NPCMenuHandler.ChooseItem(Inventory));
                    break;
                case 4:
                    AnsiConsole.Write(new Text($"{locale.SellItem}: \n", Stylesheet.Styles["default"]));
                    SellItem(InventoryMenuHandler.ChooseItem(true));
                    break;
                case 5:
                    return;
            }
    }
    public void AddWares()
    {
        Inventory.UpdateWares(LoyaltyLevel);
    }

    public void InspectItem(int index)
    {
        Inventory.ElementAt(index).Key.Inspect();
        var cont = AnsiConsole.Prompt(new TextPrompt<string>(locale.PressAnyKey).AllowEmpty());
    }
    public void BuyItem(int index)
    {
        var purchase = Inventory.ElementAt(index);
        purchase.Key.WriteName();
        AnsiConsole.Write("\n");
        var player = PlayerHandler.player;
        if (purchase.Value == 1 || !purchase.Key.Stackable)
        {
            if (player.Gold < purchase.Key.Cost)
            {
                player.Say(locale.IDontHaveEnough);
                return;
            }
            if (!EngineMethods.Confirmation(locale.WantBuyThird, true))
            {
                player.Say(locale.NoSorry);
                return;
            }
            Inventory.RemoveAt(index);
            SpendGold(purchase.Key.Cost);
            AnsiConsole.Write(new Text($"{locale.Bought}: ", Stylesheet.Styles["value-gained"]));
            purchase.Key.WriteName();
            AnsiConsole.Write("\n\n");
            player.Inventory.AddItem(purchase.Key, purchase.Value);
        }
        else
        {
            if (player.Gold < purchase.Key.Cost)
            {
                player.Say(locale.IDontHaveEnough);
                return;
            }
            var amount = AnsiConsole.Prompt(new TextPrompt<int>(locale.HowManyToBuy)
                .Validate(Validator));
            if (!EngineMethods.Confirmation(locale.WantBuyThird, true))
            {
                player.Say(locale.NoSorry);
                return;
            }
            Inventory.RemoveAt(index, amount);
            SpendGold(purchase.Key.Cost * amount);
            AnsiConsole.Write(new Text($"{locale.Bought}: ", Stylesheet.Styles["value-gained"]));
            purchase.Key.WriteName();
            AnsiConsole.Write(new Text($" ({amount})", Stylesheet.Styles["default"]));
            AnsiConsole.Write("\n\n");
            player.Inventory.AddItem(purchase.Key, amount);
        }
        return;
        
        ValidationResult Validator(int n) {
            if (n > purchase.Value) return ValidationResult.Error(locale.ChoseTooMany);
            if (player.Gold < purchase.Key.Cost * n) return ValidationResult.Error(locale.NotEnoughGold);
            return n < 0 ? ValidationResult.Error(locale.IntBelowZero) : ValidationResult.Success();
        }
    }
    public void SellItem(int index)
    {
        if (index == -1)
        {
            AnsiConsole.Write(new Text($"{locale.InventoryEmpty}\n\n", Stylesheet.Styles["error"]));
            return;
        }
        var player = PlayerHandler.player;
        var selected = player.Inventory.Items.ElementAt(index);
        selected.Key.WriteName();
        AnsiConsole.Write("\n");
        var cost = selected.Key.Cost * (Inventory.PossibleWares.Contains(selected.Key.ItemType) ? 0.5 : 0.25);
        if (selected.Value == 1 || !selected.Key.Stackable)
        {
            Say($"{locale.ICanGiveYou} {cost} {locale.CrownsGenitive}");
            if (!EngineMethods.Confirmation(locale.WantSellThird, true))
            {
                player.Say(locale.NoSorry);
                return;
            }
            player.Inventory.TryRemoveItem(selected.Key);
            player.GainGold((int)cost);
            AnsiConsole.Write(new Text($"{locale.Sold}: ", Stylesheet.Styles["value-gained"]));
            selected.Key.WriteName();
            AnsiConsole.Write("\n\n");
            Inventory.AddItem(selected.Key);
        }
        else
        {
            var amount = AnsiConsole.Prompt(new TextPrompt<int>(locale.HowManyToSell)
                .Validate(Validator));
            cost *= amount;
            Say($"{locale.ICanGiveYou} {cost} {locale.CrownsGenitive}");
            if (!EngineMethods.Confirmation(locale.WantSellThird, true))
            {
                player.Say(locale.NoSorry);
                return;
            }
            player.Inventory.TryRemoveItem(selected.Key, amount);
            player.GainGold((int)(cost * amount));
            AnsiConsole.Write(new Text($"{locale.Sold}: ", Stylesheet.Styles["value-gained"]));
            selected.Key.WriteName();
            AnsiConsole.Write(new Text($" ({amount})", Stylesheet.Styles["default"]));
            AnsiConsole.Write("\n\n");
            Inventory.AddItem(selected.Key, amount);
        }

        return;
        ValidationResult Validator(int n) {
            if (n > selected.Value) return ValidationResult.Error(locale.ChoseTooMany);
            return n < 0 ? ValidationResult.Error(locale.IntBelowZero) : ValidationResult.Success();
        }
    }
    public void CraftItem()
    {
        CraftingManager.OpenCraftingMenu(CraftableItems);
    }
}