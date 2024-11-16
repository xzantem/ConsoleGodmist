using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Town.NPCs;

public class Blacksmith : NPC
{
    public Blacksmith(string alias)
    {
        Alias = alias;
        Inventory = new NPCInventory([ItemType.Smithing, ItemType.Weapon, ItemType.Armor]);
        CraftableItems = ItemManager.CraftableIngredients.Where(x => x.ItemType == ItemType.Smithing)
            .Cast<ICraftable>().ToList();
        LoyaltyLevel = 1;
        GoldSpent = 0;
    }
    [JsonConstructor]
    public Blacksmith()
    {
        Alias = "Blacksmith";
    }

    public override void OpenMenu()
    {
        AnsiConsole.Write(new FigletText(locale.Blacksmith).Centered()
            .Color(Stylesheet.Styles["npc-blacksmith"].Foreground));
        Say($"{locale.BlacksmithGreeting1}, {PlayerHandler.player.Name}. {locale.BlacksmithGreeting2}\n");
        while (true)
        {
            AnsiConsole.Write(new Text($"{locale.LoyaltyLevel}: [{LoyaltyLevel}/15]\n", Stylesheet.Styles["npc-blacksmith"]));
            string[] choices = [locale.OpenShop, locale.CreateSmithing, locale.CreateWeapon, 
                locale.CreateArmor, locale.UpgradeWeapon, locale.UpgradeArmor, locale.ReforgeWeapon, 
                locale.ReforgeArmor, locale.Return];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
                .HighlightStyle(Stylesheet.Styles["npc-blacksmith"]));
            switch (Array.IndexOf(choices, choice))
            {
                case 0: DisplayShop(); break;
                case 1: CraftItem(); break;
                case 2: CraftWeapon(); break;
                case 3: CraftArmor(); break;
                case 4: UpgradeWeapon(); break;
                case 5: UpgradeArmor(); break;
                case 6: ReforgeWeapon(); break;
                case 7: ReforgeArmor(); break;
                case 8: return;
            }
            AnsiConsole.Write(new FigletText(locale.Blacksmith).Centered()
                .Color(Stylesheet.Styles["npc-blacksmith"].Foreground));
        }
    }
    public void CraftWeapon()
    {
        throw new NotImplementedException();
    }
    public void CraftArmor()
    {
        throw new NotImplementedException();
    }

    public void UpgradeWeapon()
    {
        var player = PlayerHandler.player;
        var chosenModifier = player.Weapon.UpgradeModifier - 1;
        var upgradeChance = 0.5;
        while (true)
        {
            switch (player.Weapon.UpgradeModifier)
            {
                case >= 2:
                    Say(locale.MaxUpgradeAlready); return;
                case >= 1.6:
                    //Add quest condition
                    Say(locale.ToolsTooWeak); return;
            }
            var cost = PlayerHandler.HonorDiscountModifier * ServiceCostMod * (1 + player.Weapon.Cost) / 2.0 * 
                ((7 * chosenModifier + 3) / (12 - 11 * chosenModifier) * ((57 - 37 * upgradeChance) / (76 - 75 * upgradeChance)));
            AnsiConsole.Write(new Text($"{locale.CurrentChosenModifier}: {player.Weapon.UpgradeModifier - 1:P0} " +
                                        $"-> {chosenModifier:P0}\n", Stylesheet.Styles["default"]));
            AnsiConsole.Write(new Text($"{locale.ChosenUpgradeChance}: {upgradeChance:P0}\n", Stylesheet.Styles["default"]));
            AnsiConsole.Write(new Text($"{locale.ServiceCost}: {cost:F0} cr\n", Stylesheet.Styles["default"]));
            string[] choices = [locale.UpgradeWeapon, locale.ChangeModifier, 
                locale.ChangeUpgradeChance, locale.Return];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
                .HighlightStyle(Stylesheet.Styles["npc-blacksmith"]));
            switch (Array.IndexOf(choices, choice))
            {
                case 0: 
                    if (player.Gold < cost)
                    {
                        player.Say(locale.NotEnoughGold);
                        continue;
                    }
                    Say(locale.WantUpgrade);
                    if (!UtilityMethods.Confirmation(locale.WantUpgradeThird, true)) continue;
                    SpendGold((int)cost);
                    if (Random.Shared.NextDouble() < upgradeChance)
                    {
                        player.Weapon.UpgradeModifier = chosenModifier + 1;
                        AnsiConsole.Write(new Text($"{locale.SuccessC}! {locale.CurrentWeaponModifier} {chosenModifier:P0}\n", Stylesheet.Styles["success"]));
                    }
                    else
                        AnsiConsole.Write(new Text($"{locale.FailureC}! {locale.CurrentWeaponModifier} {player.Weapon.UpgradeModifier - 1:P2}\n", Stylesheet.Styles["failure"]));
                    break;
                case 1:
                    chosenModifier = AnsiConsole.Prompt(
                        new TextPrompt<int>(
                                $"{locale.ChooseModifier} {player.Weapon.UpgradeModifier - 1:P0} {locale.And2} 60%): ")
                            .Validate(ModValidator)) * 0.01;
                    break;
                case 2:
                    upgradeChance = AnsiConsole.Prompt(
                        new TextPrompt<int>(
                                $"{locale.ChooseUpgradeChance}: ")
                            .Validate(ChanceValidator)) * 0.01;
                    break;
                case 3: return;
            }
        }
    }

    public void UpgradeArmor()
    {
        var player = PlayerHandler.player;
        var chosenModifier = player.Armor.UpgradeModifier - 1;
        var upgradeChance = 0.5;
        while (true)
        {
            switch (player.Armor.UpgradeModifier)
            {
                case >= 2:
                    Say(locale.MaxUpgradeAlready); return;
                case >= 1.6:
                    //Add quest condition
                    Say(locale.ToolsTooWeak); return;
            }
            var cost = PlayerHandler.HonorDiscountModifier * ServiceCostMod * (1 + player.Armor.Cost) / 2.0 * 
                ((7 * chosenModifier + 3) / (12 - 11 * chosenModifier) * ((57 - 37 * upgradeChance) / (76 - 75 * upgradeChance)));
            AnsiConsole.Write(new Text($"{locale.CurrentChosenModifier}: {player.Armor.UpgradeModifier - 1:P0} " +
                                        $"-> {chosenModifier:P0}\n", Stylesheet.Styles["default"]));
            AnsiConsole.Write(new Text($"{locale.ChosenUpgradeChance}: {upgradeChance:P0}\n", Stylesheet.Styles["default"]));
            AnsiConsole.Write(new Text($"{locale.ServiceCost}: {cost:F0} cr\n", Stylesheet.Styles["default"]));
            string[] choices = [locale.UpgradeArmor, locale.ChangeModifier, 
                locale.ChangeUpgradeChance, locale.Return];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
                .HighlightStyle(Stylesheet.Styles["npc-blacksmith"]));
            switch (Array.IndexOf(choices, choice))
            {
                case 0: 
                    if (player.Gold < cost)
                    {
                        player.Say(locale.NotEnoughGold);
                        continue;
                    }
                    Say(locale.WantUpgrade);
                    if (!UtilityMethods.Confirmation(locale.WantUpgradeThird, true)) continue;
                    SpendGold((int)cost);
                    if (Random.Shared.NextDouble() < upgradeChance)
                    {
                        player.Armor.UpgradeModifier = chosenModifier + 1;
                        AnsiConsole.Write(new Text($"{locale.SuccessC}! {locale.CurrentArmorModifier} {chosenModifier:P0}\n", Stylesheet.Styles["success"]));
                    }
                    else
                        AnsiConsole.Write(new Text($"{locale.FailureC}! {locale.CurrentArmorModifier} {player.Armor.UpgradeModifier - 1:P2}\n", Stylesheet.Styles["failure"]));
                    break;
                case 1:
                    chosenModifier = AnsiConsole.Prompt(
                        new TextPrompt<int>(
                                $"{locale.ChooseModifier} {player.Armor.UpgradeModifier - 1:P0} {locale.And2} 60%): ")
                            .Validate(ModValidator)) * 0.01;
                    break;
                case 2:
                    upgradeChance = AnsiConsole.Prompt(
                        new TextPrompt<int>(
                                $"{locale.ChooseUpgradeChance}: ")
                            .Validate(ChanceValidator)) * 0.01;
                    break;
                case 3: return;
            }
        }
    }
    public void ReforgeWeapon()
    {
        var player = PlayerHandler.player;
        switch (player.Weapon.Rarity)
        {
            case ItemRarity.Godly:
                Say(locale.MaxUpgradeAlready); return;
            case ItemRarity.Junk:
                Say(locale.TryReforgeJunk); return;
        }
        var cost = (int)(player.Weapon.Cost * ServiceCostMod * PlayerHandler.HonorDiscountModifier / 10.0);
        Say($"{locale.ICanReforge} {cost} {locale.CrownsGenitive}");
        if (player.Gold < cost)
        {
            player.Say(locale.IDontHaveEnough);
            return;
        }
        Say(locale.WantReforge);
        if (!UtilityMethods.Confirmation(locale.WantReforgeThird, true)) return;
        SpendGold(cost);
        var success = UtilityMethods.RandomChoice(new Dictionary<int, double>
        { { -1, 0.1 }, { 0, 0.2 }, { 1, 0.5 }, { 2, 0.2 } });
        switch (success)
        {
            case -1:
                player.Weapon.Rarity = ItemRarity.Destroyed;
                AnsiConsole.Write(new Text($"{locale.CriticalM} {locale.Failure}! {locale.WeaponDestroyed}\n", Stylesheet.Styles["error"]));
                break;
            case 0:
                AnsiConsole.Write(new Text($"{locale.FailureC}. {locale.ReforgeNoChange}\n", Stylesheet.Styles["failure"]));
                break;
            case 1:
                player.Weapon.Rarity += 1;
                AnsiConsole.Write(new Text($"{locale.SuccessC}.\n", Stylesheet.Styles["success"]));
                break;
            case 2:
                player.Weapon.Rarity += player.Weapon.Rarity == ItemRarity.Legendary ? 1 : 2;
                AnsiConsole.Write(new Text($"{locale.CriticalM} {locale.Success}!\n", Stylesheet.Styles["highlight-good"]));
                break;
        }
        AnsiConsole.Write(new Text($"{locale.CurrentRarity}:", Stylesheet.Styles["default"]));
        (player.Weapon as IItem).WriteRarity();
        AnsiConsole.Write("\n");
    }
    public void ReforgeArmor()
    {
        var player = PlayerHandler.player;
        switch (player.Armor.Rarity)
        {
            case ItemRarity.Godly:
                Say(locale.MaxUpgradeAlready); return;
            case ItemRarity.Junk:
                Say(locale.TryReforgeJunk); return;
        }
        var cost = (int)(player.Armor.Cost * ServiceCostMod * PlayerHandler.HonorDiscountModifier / 10.0);
        Say($"{locale.ICanReforge} {cost} {locale.CrownsGenitive}");
        if (player.Gold < cost)
        {
            player.Say(locale.IDontHaveEnough);
            return;
        }
        Say(locale.WantReforge);
        if (!UtilityMethods.Confirmation(locale.WantReforgeThird, true)) return;
        SpendGold(cost);
        var success = UtilityMethods.RandomChoice(new Dictionary<int, double>
            { { -1, 0.1 }, { 0, 0.2 }, { 1, 0.5 }, { 2, 0.2 } });
        switch (success)
        {
            case -1:
                player.Armor.Rarity = ItemRarity.Destroyed;
                AnsiConsole.Write(new Text($"{locale.CriticalM} {locale.Failure}! {locale.ArmorDestroyed}\n", Stylesheet.Styles["error"]));
                break;
            case 0:
                AnsiConsole.Write(new Text($"{locale.FailureC}. {locale.ReforgeNoChange}\n", Stylesheet.Styles["failure"]));
                break;
            case 1:
                player.Armor.Rarity += 1;
                AnsiConsole.Write(new Text($"{locale.SuccessC}.\n", Stylesheet.Styles["success"]));
                break;
            case 2:
                player.Armor.Rarity += player.Armor.Rarity == ItemRarity.Legendary ? 1 : 2;
                AnsiConsole.Write(new Text($"{locale.CriticalM} {locale.Success}!\n", Stylesheet.Styles["highlight-good"]));
                break;
        }
        AnsiConsole.Write(new Text($"{locale.CurrentRarity}:", Stylesheet.Styles["default"]));
        (player.Armor as IItem).WriteRarity();
        AnsiConsole.Write("\n");
    }
    ValidationResult ModValidator(int m)
    { 
        if (m > 60) return ValidationResult.Error(locale.ChoseTooMany);
        return m < 1 ? ValidationResult.Error(locale.IntBelowZero) : ValidationResult.Success();
    }
    ValidationResult ChanceValidator(int c)
    { 
        if (c > 100) return ValidationResult.Error(locale.ChoseTooMany);

        return c < 0 ? ValidationResult.Error(locale.IntBelowZero) : ValidationResult.Success();
    }
    public override void Say(string message)
    {
        AnsiConsole.Write(new Text($"{Name}: ", Stylesheet.Styles["npc-blacksmith"]));
        AnsiConsole.Write(new Text($"{message}\n", Stylesheet.Styles["dialogue"]));
    }
}