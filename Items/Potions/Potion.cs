using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using ConsoleGodmist.Utilities;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public class Potion : BaseItem
{
    public new string Name => NameAliasHelper.GetName(Alias);
    public override int Weight => 2;
    public override int ID => 561;
    public override bool Stackable => false;
    public override ItemType ItemType => ItemType.Potion;
    
    public List<PotionComponent> Components { get; set; }
    
    public PotionCatalyst Catalyst { get; set; }
    public int CurrentCharges { get; set; }
    public int MaximalCharges { get; set; }

    public Potion(string alias, string description, List<PotionComponent> components, 
        PotionCatalyst catalyst)
    {
        Alias = alias;
        Cost = components.Sum(x => ItemManager.GetItem(x.Material).Cost) * 3; //+ 
               //ItemManager.GetItem(catalyst.Material).Cost;
        Rarity = components.Max(x => ItemManager.GetItem(x.Material).Rarity);
        Components = components;
        Catalyst = catalyst;
        MaximalCharges = CurrentCharges = 5 + (Catalyst.Effect == PotionCatalystEffect.Capacity ? Catalyst.Tier : 0);
    }
    public Potion() {}

    public bool Use()
    {
        if (CurrentCharges <= 0) return false;
        CurrentCharges--;
        //Add alcohol poisoning
        foreach (var component in Components)
        {
            PotionManager.ProcessComponent(component, Catalyst);
        }
        return false;
    }

    public void Refill(int amount)
    {
        CurrentCharges = Math.Min(MaximalCharges, CurrentCharges + amount);
    }

    public override void Inspect(int amount = 1)
    {
        base.Inspect(amount);
        AnsiConsole.Write($"Charges: {CurrentCharges}/{MaximalCharges}\n");
        foreach (var effect in Components)
        {
            var duration = 10 + (int)(Catalyst.Effect == PotionCatalystEffect.Duration ? Catalyst.Strength : 0);
            var strength = effect.EffectStrength * duration * (Catalyst.Effect == PotionCatalystEffect.Strength ? 1 + Catalyst.Strength : 1) / 10.0;
            var condensedDuration = duration - (int)(Catalyst.Effect == PotionCatalystEffect.Condensation ? Catalyst.Strength : 0);
            switch (effect.Effect)
            {
                case PotionEffect.HealthRegain:
                    AnsiConsole.Write(new Text($"- {locale.HealthC} Regain: {strength:P0} {locale.HealthC} " +
                                               $"instantly\n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.HealthRegen:
                    AnsiConsole.Write(new Text($"- {locale.HealthC} Regen: {strength:P0} {locale.HealthC} over " +
                                               $"{condensedDuration} turns \n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.ResourceRegain:
                    AnsiConsole.Write(new Text($"- {BattleTextService.ResourceShortText(PlayerHandler.player)} Regain: {strength:P0} " +
                                               $"{BattleTextService.ResourceShortText(PlayerHandler.player)} " +
                                               $"instantly\n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.ResourceRegen:
                    AnsiConsole.Write(new Text($"- {BattleTextService.ResourceShortText(PlayerHandler.player)} Regen: {strength:P0} " + 
                                               $"{BattleTextService.ResourceShortText(PlayerHandler.player)} over " +
                                               $"{condensedDuration} turns \n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.MaxResourceIncrease:
                    AnsiConsole.Write(new Text($"- Max {BattleTextService.ResourceShortText(PlayerHandler.player)} Increase: +{strength:P0} " + 
                                               $"{BattleTextService.ResourceShortText(PlayerHandler.player)} for " +
                                               $"{duration} turns \n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.DamageDealtIncrease:
                    AnsiConsole.Write(new Text($"- {locale.DamageGenitive} Dealt Increase: +{strength:P0} {locale.DamageGenitive} " +
                                               $"for {duration} turns \n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.DamageTakenDecrease:
                    AnsiConsole.Write(new Text($"- {locale.DamageGenitive} Taken Decrease: -{strength:P0} {locale.DamageGenitive} " +
                                               $"for {duration} turns \n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.ResistanceIncrease:
                    AnsiConsole.Write(new Text($"- {locale.Resistances} Increase: +{strength:P0} {locale.Resistances} " +
                                               $"for {duration} turns \n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.SpeedIncrease:
                    AnsiConsole.Write(new Text($"- {locale.Speed} Increase: +{strength:P0} {locale.Speed} " +
                                               $"for {duration} turns \n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.CritChanceIncrese:
                    AnsiConsole.Write(new Text($"- {locale.CritChance} Increase: +{strength:P0} {locale.CritChance} " +
                                               $"for {duration} turns \n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.DodgeIncrease:
                    AnsiConsole.Write(new Text($"- {locale.Dodge} Increase: +{strength:F0} {locale.Dodge} " +
                                               $"for {duration} turns \n", Stylesheet.Styles["default"]));
                    break;
                case PotionEffect.AccuracyIncrease:
                    AnsiConsole.Write(new Text($"- {locale.Accuracy} Increase: +{strength:P0} {locale.Accuracy} " +
                                               $"for {duration} turns \n", Stylesheet.Styles["default"]));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }
}