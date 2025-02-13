﻿using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public class Bandage : BaseItem, ICraftable, IUsable
{
    public override string Alias => "Bandage";
    public override int Weight => 0;
    public override int ID => 590;
 
    public override int Cost => (int)Math.Floor((double)CraftingRecipe
        .Sum(x => x.Value * ItemManager.GetItem(x.Key).Cost) / CraftedAmount);

    public override ItemRarity Rarity => ItemRarity.Common;
    public override bool Stackable => true;
    public override string Description => "";
    public override ItemType ItemType => ItemType.Alchemy;

    public Dictionary<string, int> CraftingRecipe {
        get => new() { { "WeakAlcohol", 1 }, { "CottonFabric", 1 } };
        set => throw new InvalidOperationException(); }

    public int CraftedAmount { get => 2; set => throw new InvalidOperationException(); }

    public bool Use()
    {
        PlayerHandler.player.StatusEffects.RemoveAll(x => x.Type == StatusEffectType.Bleed);
        return true;
    }
}