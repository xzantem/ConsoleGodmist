using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.Quests;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Town.NPCs;

[JsonConverter(typeof(NPCConverter))]
public class Enchanter : NPC
{
    public Enchanter(string alias)
    {
        Alias = alias;
        Inventory = new NPCInventory([ItemType.Runeforging, ItemType.WeaponGaldurite, ItemType.ArmorGaldurite]);
        CraftableItems = ItemManager.CraftableIngredients.Where(x => x.ItemType == ItemType.Runeforging)
            .Cast<ICraftable>().ToList();
        LoyaltyLevel = 1;
        GoldSpent = 0;
    }
    [JsonConstructor]
    public Enchanter()
    {
        Alias = "Enchanter";
    }

    public override void OpenMenu()
    {
        AnsiConsole.Write(new FigletText(locale.Enchanter).Centered()
            .Color(Stylesheet.Styles["npc-enchanter"].Foreground));
        Say($"{locale.EnchanterGreeting}, {PlayerHandler.player.Name}?\n");
        while (true)
        {
            AnsiConsole.Write(new Text($"{locale.LoyaltyLevel}: [{LoyaltyLevel}/15]\n", Stylesheet.Styles["npc-enchanter"]));
            Dictionary<string, int> choices = new() {{locale.OpenShop, 0}, {locale.CreateEnchanting, 1}, 
                {locale.CreateGaldurite, 2}, {locale.RevealGaldurite, 3}, {locale.ApplyWeaponGaldurite, 4}, 
                {locale.ApplyArmorGaldurite, 5}, {locale.RemoveWeaponGaldurite, 6}, {locale.RemoveArmorGaldurite, 7}};
            if (QuestNPCHandler.GetAvailableQuests(Alias).Count > 0) choices.Add(locale.AcceptQuest, 8);
            if (QuestNPCHandler.GetReturnableQuests(Alias).Count > 0) choices.Add(locale.ReturnQuest, 9);
            choices.Add( locale.Return, 10 );
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices.Keys)
                .HighlightStyle(Stylesheet.Styles["npc-enchanter"]));
            switch (choices[choice])
            {
                case 0: DisplayShop(); break;
                case 1: CraftItem(); break;
                case 2: CraftingManager.CraftGaldurite(); break;
                case 3: ExamineGaldurite(); break;
                case 4: InsertWeaponGaldurite(); break;
                case 5: InsertArmorGaldurite(); break;
                case 6: RemoveWeaponGaldurite(); break;
                case 7: RemoveArmorGaldurite(); break;
                case 8: QuestNPCHandler.SelectQuestToAccept(Alias); break;
                case 9: QuestNPCHandler.SelectQuestToReturn(Alias); break;
                case 10: return;
            }
            AnsiConsole.Write(new FigletText(locale.Enchanter).Centered()
                .Color(Stylesheet.Styles["npc-enchanter"].Foreground));
        }
    }
    public void ExamineGaldurite()
    {
        var player = PlayerHandler.player;
        var galdurites = player.Inventory.Items
            .Where(x => x.Key.ItemType is ItemType.WeaponGaldurite or ItemType.ArmorGaldurite && 
                        !(x.Key as Galdurite).Revealed)
            .Select(x => x.Key).Cast<Galdurite>().ToList();
        if (galdurites.Count == 0)
        {
            Say(locale.NoGalduritesReveal);
            return;
        }
        Say(locale.ChooseGalduriteReveal);
        var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
        if (galdurite == null) return;
        var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * 0.2 * galdurite.Cost);
        Say($"{locale.ICanReveal} {cost} {locale.CrownsGenitive}");
        if (player.Gold < cost)
        {
            player.Say(locale.IDontHaveEnough);
            return;
        }
        Say(locale.WantReveal);
        if (!UtilityMethods.Confirmation(locale.WantRevealThird, true)) return;
        SpendGold(cost);
        galdurite.Reveal();
        galdurite.Inspect();
    }
    public void InsertWeaponGaldurite()
    {
        var player = PlayerHandler.player;
        if (player.Weapon.Galdurites.Count < player.Weapon.GalduriteSlots)
        {
            var galdurites = player.Inventory.Items.Keys
            .Where(x => x.ItemType == ItemType.WeaponGaldurite)
            .Cast<Galdurite>()
            .Where(x => player.Weapon.Rarity >= x.Rarity && player.Weapon.RequiredLevel >= x.RequiredLevel)
            .ToList();
            if (galdurites.Count == 0)
            {
                Say(locale.NoGalduritesApply);
                return;
            }
            Say(locale.ChooseGalduriteApply);
            var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
            if (galdurite == null) return;
            var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * galdurite.Cost * 0.75);
            Say($"{locale.ICanApply} {cost} {locale.CrownsGenitive}");
            if (player.Gold < cost)
            {
                player.Say(locale.IDontHaveEnough);
                return;
            }
            Say(locale.WantApply);
            galdurite.Inspect();
            if (!UtilityMethods.Confirmation(locale.WantApplyThird, true)) return;
            SpendGold(cost);
            player.Weapon.AddGaldurite(galdurite);
            player.Inventory.TryRemoveItem(galdurite);
        }
        else
        {
            Say(locale.NoGalduriteSlotsWeapon);
        }
    }
    public void InsertArmorGaldurite()
    {
        var player = PlayerHandler.player;
        if (player.Armor.Galdurites.Count < player.Armor.GalduriteSlots)
        {
            var galdurites = player.Inventory.Items.Keys
                .Where(x => x.ItemType == ItemType.ArmorGaldurite)
                .Cast<Galdurite>()
                .Where(x => player.Armor.Rarity >= x.Rarity && player.Armor.RequiredLevel >= x.RequiredLevel)
                .ToList();
            if (galdurites.Count == 0)
            {
                Say(locale.NoGalduritesApply);
                return;
            }
            Say(locale.ChooseGalduriteApply);
            var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
            if (galdurite == null) return;
            var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * galdurite.Cost * 0.75);
            Say($"{locale.ICanApply} {cost} {locale.CrownsGenitive}");
            if (player.Gold < cost)
            {
                player.Say(locale.IDontHaveEnough);
                return;
            }
            Say(locale.WantApply);
            galdurite.Inspect();
            if (!UtilityMethods.Confirmation(locale.WantApplyThird, true)) return;
            SpendGold(cost);
            player.Armor.AddGaldurite(galdurite);
            player.Inventory.TryRemoveItem(galdurite);
        }
        else
        {
            Say(locale.NoGalduriteSlotsArmor);
        }
    }
    public void RemoveWeaponGaldurite()
    {
        var player = PlayerHandler.player;
        var galdurites = player.Weapon.Galdurites;
        if (galdurites.Count == 0)
        {
            Say(locale.NoGalduritesInWeapon);
            return;
        }
        Say(locale.ChooseGalduriteRemove);
        var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
        if (galdurite == null) return;
        var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * galdurite.Cost * 4);
        Say($"{locale.ICanRemove} {cost} {locale.CrownsGenitive}");
        if (player.Gold < cost)
        {
            player.Say(locale.IDontHaveEnough);
            return;
        }
        Say(locale.WantRemove);
        galdurite.Inspect();
        if (!UtilityMethods.Confirmation(locale.WantRemoveThird, true)) return;
        SpendGold(cost);
        player.Weapon.RemoveGaldurite(galdurite);
    }
    public void RemoveArmorGaldurite()
    {
        var player = PlayerHandler.player;
        var galdurites = player.Armor.Galdurites;
        if (galdurites.Count == 0)
        {
            Say(locale.NoGalduritesInArmor);
            return;
        }
        Say(locale.ChooseGalduriteRemove);
        var galdurite = GalduriteManager.ChooseGaldurite(galdurites);
        if (galdurite == null) return;
        var cost = (int)(PlayerHandler.HonorDiscountModifier * ServiceCostMod * galdurite.Cost * 4);
        Say($"{locale.ICanRemove} {cost} {locale.CrownsGenitive}");
        if (player.Gold < cost)
        {
            player.Say(locale.IDontHaveEnough);
            return;
        }
        Say(locale.WantRemove);
        galdurite.Inspect();
        if (!UtilityMethods.Confirmation(locale.WantRemoveThird, true)) return;
        SpendGold(cost);
        player.Armor.RemoveGaldurite(galdurite);
    }

    public override void Say(string message)
    {
        AnsiConsole.Write(new Text($"{Name}: ", Stylesheet.Styles["npc-enchanter"]));
        AnsiConsole.Write(new Text($"{message}\n", Stylesheet.Styles["dialogue"]));
    }
}