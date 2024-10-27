using ConsoleGodmist.Combat.Skills;
using Newtonsoft.Json;

namespace ConsoleGodmist.Items.Weapons;

[JsonConverter(typeof(EquipmentPartConverter))]
public class WeaponBinder : IEquipmentPart
{
    public double AttackBonus { get; set; }
    public double CritChance { get; set; }
    public double CritModBonus { get; set; }
    public double AccuracyBonus { get; set; }
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : locale.ResourceManager.GetString(Alias);
    public string Alias { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int GoldCost { get; set; }
    public int MaterialCost { get; set; }
    
    public WeaponBinder() {}
}