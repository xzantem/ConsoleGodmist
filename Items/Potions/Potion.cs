﻿using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using ConsoleGodmist.Utilities;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public class Potion : BaseItem, IUsable
{
    public override string Name => PotionManager
        .GetPotionName(Components.Select(x => x.Effect).ToList(), 
            Components.Max(x => x.StrengthTier));
    public override int Weight => 2;
    public override int ID => 561;
    public override bool Stackable => false;

    public override int Cost =>
        (int)(0.5 * (1 + (double)CurrentCharges / MaximalCharges) * (Components.Sum(x => ItemManager
            .GetItem(x.Material).Cost) * 3 + (Catalyst == null ? 0 : ItemManager.GetItem(Catalyst.Material).Cost)));
    public override ItemType ItemType => ItemType.Potion;
    
    public List<PotionComponent> Components { get; set; }
    
    public PotionCatalyst? Catalyst { get; set; }
    public int CurrentCharges { get; set; }
    public int MaximalCharges { get; set; }

    public Potion(string alias, string description, List<PotionComponent> components, 
        PotionCatalyst catalyst)
    {
        Alias = alias;
        Rarity = components.Max(x => ItemManager.GetItem(x.Material).Rarity);
        Components = components;
        Catalyst = catalyst;
        MaximalCharges = CurrentCharges = 5 + (catalyst == null ? 0 :(Catalyst.Effect == PotionCatalystEffect.Capacity ? Catalyst.Tier : 0));
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
        AnsiConsole.Write($"{locale.Charges}: {CurrentCharges}/{MaximalCharges}\n");
        foreach (var effect in Components)
            AnsiConsole.Write(effect.EffectDescription(Catalyst, false));
    }
}