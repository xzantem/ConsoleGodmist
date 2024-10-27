using ConsoleGodmist.Combat.Skills;
using Newtonsoft.Json;

namespace ConsoleGodmist.Items.Weapons;

[JsonConverter(typeof(EquipmentPartConverter))]
public class WeaponHandle : IEquipmentPart
{
    public double AttackBonus { get; set; }
    public double CritChanceBonus { get; set; }
    public double CritModBonus { get; set; }
    public int Accuracy { get; set; }
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : locale.ResourceManager.GetString(Alias);
    public string Alias { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int GoldCost { get; set; }
    public int MaterialCost { get; set; }
    
    public WeaponHandle() {}
}