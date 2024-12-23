using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.Quests;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Town.NPCs;

public class Alchemist : NPC
{
    public Alchemist(string alias)
    {
        Alias = alias;
        Inventory = new NPCInventory([ItemType.Alchemy, ItemType.Potion]);
        CraftableItems = ItemManager.CraftableIngredients.Where(x => x.ItemType == ItemType.Alchemy)
            .Cast<ICraftable>().ToList();
        LoyaltyLevel = 1;
        GoldSpent = 0;
    }
    [JsonConstructor]
    public Alchemist()
    {
        Alias = "Alchemist";
    }

    public override void OpenMenu()
    {
        AnsiConsole.Write(new FigletText(locale.Alchemist).Centered()
            .Color(Stylesheet.Styles["npc-alchemist"].Foreground));
        Say($"{locale.AlchemistGreeting1}, {PlayerHandler.player.Name}. {locale.AlchemistGreeting2}\n");
        while (true)
        {
            AnsiConsole.Write(new Text($"{locale.LoyaltyLevel}: [{LoyaltyLevel}/15]\n", Stylesheet.Styles["npc-alchemist"]));
            Dictionary<string, int> choices = new() {
                {locale.OpenShop, 0}, {locale.CureWounds, 1}, {locale.CreateAlchemy, 2}, 
                {locale.CreatePotion, 3}, {locale.RefillPotion, 4} };
            if (QuestNPCHandler.GetAvailableQuests(Name).Count > 0) choices.Add(locale.AcceptQuest, 5);
            if (QuestNPCHandler.GetReturnableQuests(Name).Count > 0) choices.Add(locale.ReturnQuest, 6);
            choices.Add( locale.Return, 7 );
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices.Keys)
                .HighlightStyle(Stylesheet.Styles["npc-alchemist"]));
            switch (choices[choice])
            {
                case 0: DisplayShop(); break;
                case 1: Treat(); break;
                case 2: CraftItem(); break;
                case 3: CraftPotion(); break;
                case 4: RefillPotion(); break;
                case 5: QuestNPCHandler.SelectQuestToAccept(Name); break;
                case 6: QuestNPCHandler.SelectQuestToReturn(Name); break;
                case 7: return;
            }
            AnsiConsole.Write(new FigletText(locale.Alchemist).Centered()
                .Color(Stylesheet.Styles["npc-alchemist"].Foreground));
        }
    }

    public void Treat()
    {
        var player = PlayerHandler.player;
        var cost = (int)(ServiceCostMod * PlayerHandler.HonorDiscountModifier * 
            Math.Pow(4, (player.Level - 1) / 10.0) * 0.1 * player.MaximalHealth * 
            (player.MaximalHealth - player.CurrentHealth) / (2 * player.MaximalHealth - player.CurrentHealth));
        Say($"{locale.ICanTreat} {cost} {locale.CrownsGenitive}");
        if (player.Gold < cost)
        {
            player.Say(locale.IDontHaveEnough);
            return;
        }
        Say(locale.WantTreatment);
        if (!UtilityMethods.Confirmation(locale.WantTreatmentThird, true)) return;
        SpendGold(cost);
        player.Heal(player.MaximalHealth - player.CurrentHealth);
    }
    public void CraftPotion()
    {
        throw new NotImplementedException();
    }
    public void RefillPotion()
    {
        var player = PlayerHandler.player;
        var potions = player.Inventory.Items
            .Where(x => x.Key.ItemType == ItemType.Potion)
            .Select(x => x.Key).Cast<Potion>().ToList();
        if (potions.Count == 0 || potions.All(x => x.MaximalCharges == x.CurrentCharges))
        {
            Say(locale.NoPotions);
            return;
        }
        var cost = 6 * PlayerHandler.HonorDiscountModifier * ServiceCostMod * Math.Pow(4, (player.Level - 1)/10.0);
        Say(locale.ChooseRefillPotion);
        var potion = PotionManager.ChoosePotion(potions);
        if (potion == null) return;
        var amount = Math.Min(potion.MaximalCharges - potion.CurrentCharges,
            AnsiConsole.Prompt(new TextPrompt<int>(locale.ChooseRefillAmount)
                .DefaultValue(potion.MaximalCharges - potion.CurrentCharges).Validate(i => i switch
                { <= 0 => ValidationResult.Error(locale.IntBelowZero), _ => ValidationResult.Success() })));
        Say($"{locale.ICanRefill} {amount * cost} {locale.CrownsGenitive}");
        if (player.Gold < amount * cost)
        {
            player.Say(locale.IDontHaveEnough);
            return;
        }
        Say(locale.WantRefill);
        if (!UtilityMethods.Confirmation(locale.WantRefillThird, true)) return;
        SpendGold((int)(amount * cost));
        potion.Refill(amount);
    }
    public override void Say(string message)
    {
        AnsiConsole.Write(new Text($"{Name}: ", Stylesheet.Styles["npc-alchemist"]));
        AnsiConsole.Write(new Text($"{message}\n", Stylesheet.Styles["dialogue"]));
    }
}