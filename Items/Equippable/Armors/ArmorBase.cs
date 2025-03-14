﻿using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;

namespace ConsoleGodmist.Items;

[JsonConverter(typeof(EquipmentPartConverter))]
public class ArmorBase : IEquipmentPart
{
    public double HealthBonus { get; set; }
    public int Dodge { get; set; }
    public double PhysicalDefenseBonus { get; set; }
    public double MagicDefenseBonus { get; set; }
    public string Name => NameAliasHelper.GetName(Alias[..^4]);
    public string Alias { get; set; }
    public CharacterClass IntendedClass { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int MaterialCost { get; set; }
    
    public ArmorBase() {}
    
    public string DescriptionText(double costMultiplier)
    {
        return $"{Name}, {locale.Level} {Tier * 10 - 5} " +
               $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                   Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.HealthC}: " +
               $"*{1+HealthBonus:P0} | {locale.Dodge}: {Dodge} | {locale.Defense}: " +
               $"*{1+PhysicalDefenseBonus:P0}:{1+MagicDefenseBonus:P0}\n";
    }
}