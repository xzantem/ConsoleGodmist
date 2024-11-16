using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Town.NPCs;

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
            string[] choices = [locale.OpenShop, locale.CreateEnchanting, locale.CreateGaldurite, 
                locale.RevealGaldurite, locale.ApplyWeaponGaldurite, locale.ApplyArmorGaldurite, 
                locale.RemoveWeaponGaldurite, locale.RemoveArmorGaldurite, locale.Return];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
                .HighlightStyle(Stylesheet.Styles["npc-enchanter"]));
            switch (Array.IndexOf(choices, choice))
            {
                case 0: DisplayShop(); break;
                case 1: CraftItem(); break;
                case 2: CraftGaldurite(); break;
                case 3: ExamineGaldurite(); break;
                case 4: InsertWeaponGaldurite(); break;
                case 5: InsertArmorGaldurite(); break;
                case 6: RemoveWeaponGaldurite(); break;
                case 7: RemoveArmorGaldurite(); break;
                case 8: return;
            }
            AnsiConsole.Write(new FigletText(locale.Enchanter).Centered()
                .Color(Stylesheet.Styles["npc-enchanter"].Foreground));
        }
    }
    public void CraftGaldurite()
    {
        throw new NotImplementedException();
    }
    public void ExamineGaldurite()
    {
        throw new NotImplementedException();
    }
    public void InsertWeaponGaldurite()
    {
        throw new NotImplementedException();
    }
    public void InsertArmorGaldurite()
    {
        throw new NotImplementedException();
    }
    public void RemoveWeaponGaldurite()
    {
        throw new NotImplementedException();
    }
    public void RemoveArmorGaldurite()
    {
        throw new NotImplementedException();
    }

    public override void Say(string message)
    {
        AnsiConsole.Write(new Text($"{Name}: ", Stylesheet.Styles["npc-enchanter"]));
        AnsiConsole.Write(new Text($"{message}\n", Stylesheet.Styles["dialogue"]));
    }
}