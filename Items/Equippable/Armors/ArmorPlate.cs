using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;

namespace ConsoleGodmist.Items;

[JsonConverter(typeof(EquipmentPartConverter))]
public class ArmorPlate : IEquipmentPart
{
    public double HealthBonus { get; set; }
    public double DodgeBonus { get; set; }
    public int PhysicalDefense { get; set; }
    public int MagicDefense { get; set; }
    public string Name => NameAliasHelper.GetName(Alias);
    public string Alias { get; set; }
    public CharacterClass IntendedClass { get; set; }
    public string Adjective { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int MaterialCost { get; set; }
    
    public ArmorPlate() {}
    
    public string DescriptionText(double costMultiplier)
    {
        return $"{Name}, {locale.Level} {Tier * 10 - 5} " +
               $"({MaterialCost * costMultiplier}x {NameAliasHelper.GetName(Material)} ({PlayerHandler.player.Inventory.
                   Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n{locale.HealthC}: " +
               $"*{1+HealthBonus:P0} | {locale.Dodge}: *{1 + DodgeBonus:P0} | {locale.Defense}: {PhysicalDefense}:{MagicDefense}\n";
    }
}