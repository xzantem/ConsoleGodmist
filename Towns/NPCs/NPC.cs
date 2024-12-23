using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.TextService;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;
using Spectre.Console;

/*[
  { "Alias": "Szkielet Miecznik", "ResourceType": "Fury" },
  { "Alias": "Umarlak", "ResourceType": "Fury" },
  { "Alias": "Upiór", "ResourceType": "Mana" },
  { "Alias": "Szkielet Okultysta", "ResourceType": "Mana" },
  { "Alias": "Szkielet Łucznik", "ResourceType": "Momentum" },
  { "Alias": "Ghul", "ResourceType": "Fury" },
  { "Alias": "Szkielet Obrońca", "ResourceType": "Fury" },
  { "Alias": "Szkielet Zabójca", "ResourceType": "Fury" },
  { "Alias": "Duch Umarłego Króla", "ResourceType": "Mana" },
  { "Alias": "Zarażone Drzewo", "ResourceType": "Mana" },
  { "Alias": "Wściekły Wilk", "ResourceType": "Momentum" },
  { "Alias": "Bandyta Rzezimieszek", "ResourceType": "Momentum" },
  { "Alias": "Bandyta Strzelec", "ResourceType": "Momentum" },
  { "Alias": "Elfi Łucznik", "ResourceType": "Momentum" },
  { "Alias": "Elfi Kłusownik", "ResourceType": "Momentum" },
  { "Alias": "Elfi Druid", "ResourceType": "Mana" },
  { "Alias": "Zarażony Ent", "ResourceType": "Mana" },
  { "Alias": "Przywódca Bandytów", "ResourceType": "Momentum" },
  { "Alias": "Niewolnik Demonów", "ResourceType": "Mana" },
  { "Alias": "Demoniczny Zbrojny", "ResourceType": "Fury" },
  { "Alias": "Demoniczny Czarownik", "ResourceType": "Mana" },
  { "Alias": "Chochlik", "ResourceType": "Mana" },
  { "Alias": "Hawia'kalb", "ResourceType": "Momentum" },
  { "Alias": "Demoniczny Iluzjonista", "ResourceType": "Mana" },
  { "Alias": "Eayan'masakh", "ResourceType": "Mana" },
  { "Alias": "Demoniczny Jarl", "ResourceType": "Fury" },
  { "Alias": "Cerber", "ResourceType": "Fury" },
  { "Alias": "Pirat Kombatant", "ResourceType": "Fury" },
  { "Alias": "Pirat Harpunnik", "ResourceType": "Momentum" },
  { "Alias": "Piaszczysty Feniks", "ResourceType": "Mana" },
  { "Alias": "Papuga", "ResourceType": "Momentum" },
  { "Alias": "Człowiek-rekin", "ResourceType": "Fury" },
  { "Alias": "Pirat Konsyliarz", "ResourceType": "Mana" },
  { "Alias": "Pirat Kanonier", "ResourceType": "Momentum" },
  { "Alias": "Alfa-Rekin", "ResourceType": "Fury" },
  { "Alias": "Kapitan Piratów", "ResourceType": "Fury" },
  { "Alias": "Awazar Weteran", "ResourceType": "Fury" },
  { "Alias": "Awazar Łucznik", "ResourceType": "Momentum" },
  { "Alias": "Awazar Medyk", "ResourceType": "Mana" },
  { "Alias": "Awazar Buławnik", "ResourceType": "Fury" },
  { "Alias": "Trujący Skorpion", "ResourceType": "Mana" },
  { "Alias": "Ognisty Skorpion", "ResourceType": "Mana" },
  { "Alias": "Piaszczyste Tornado", "ResourceType": "Mana" },
  { "Alias": "Przywódca Awazarów", "ResourceType": "Fury" },
  { "Alias": "Jeździeć Alfa-Skorpiona", "ResourceType": "Fury" },
  { "Alias": "Alfa-Skorpion", "ResourceType": "Fury" },
  { "Alias": "Kapłan Malada", "ResourceType": "Mana" },
  { "Alias": "Czarny Rycerz Malada", "ResourceType": "Fury" },
  { "Alias": "Plugawiciel Malada", "ResourceType": "Mana" },
  { "Alias": "Strażnik Świątynny", "ResourceType": "Mana" },
  { "Alias": "Mumia", "ResourceType": "Fury" },
  { "Alias": "Nekromanta Malada", "ResourceType": "Mana" },
  { "Alias": "Posąg Obronny", "ResourceType": "Mana" },
  { "Alias": "Hierofant", "ResourceType": "Mana" },
  { "Alias": "Jawa", "ResourceType": "Mana" },
  { "Alias": "Troll Górski", "ResourceType": "Fury" },
  { "Alias": "Ruk Tregandzki", "ResourceType": "Momentum" },
  { "Alias": "Włócznik Skruszonej Czaszki", "ResourceType": "Fury" },
  { "Alias": "Szaman Skruszonej Czaszki", "ResourceType": "Mana" },
  { "Alias": "Histynowy Golem", "ResourceType": "Fury" },
  { "Alias": "Górski Bandyta", "ResourceType": "Momentum" },
  { "Alias": "Tajemniczy Wędrowiec", "ResourceType": "Mana" },
  { "Alias": "Berserk Skruszonej Czaszki", "ResourceType": "Fury" },
  { "Alias": "Wiwerna", "ResourceType": "Fury" },
  { "Alias": "Mięsożerna Roślina", "ResourceType": "Mana" },
  { "Alias": "Aligator", "ResourceType": "Fury" },
  { "Alias": "Pasożytnicza Roślina", "ResourceType": "Mana" },
  { "Alias": "Topielec", "ResourceType": "Momentum" },
  { "Alias": "Dezerter", "ResourceType": "Fury" },
  { "Alias": "Strażnik Bagien", "ResourceType": "Momentum" },
  { "Alias": "Plująca Roślina", "ResourceType": "Mana" },
  { "Alias": "Bagnisty Ent", "ResourceType": "Mana" },
  { "Alias": "Ludożerna Roślina", "ResourceType": "Mana" }
]
*/

namespace ConsoleGodmist.Town.NPCs;

public abstract class NPC
{
    [JsonIgnore]
    public string Alias { get; set; }

    public string Name => NameAliasHelper.GetName(Alias);
    
    public NPCInventory Inventory { get; set; }
    [JsonIgnore]
    public List<ICraftable?> CraftableItems { get; set; }
     
    public int LoyaltyLevel { get; set; }
    public int GoldSpent { get; set; }
    public int RequiredGoldSpent => CalculateGoldRequired(LoyaltyLevel);

    public double ServiceCostMod => LoyaltyLevel switch
    {
        < 2 => 1.0,
        < 4 => 0.99,
        < 7 => 0.96,
        < 9 => 0.91,
        < 12 => 0.84,
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
            if (!UtilityMethods.Confirmation(locale.WantBuyThird, true))
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
            if (!UtilityMethods.Confirmation(locale.WantBuyThird, true))
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
            Say($"{locale.ICanGiveYou} {(int)cost} {locale.CrownsGenitive}");
            if (!UtilityMethods.Confirmation(locale.WantSellThird, true))
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
            Say($"{locale.ICanGiveYou} {(int)cost} {locale.CrownsGenitive}");
            if (!UtilityMethods.Confirmation(locale.WantSellThird, true))
            {
                player.Say(locale.NoSorry);
                return;
            }
            player.Inventory.TryRemoveItem(selected.Key, amount);
            player.GainGold((int)cost);
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