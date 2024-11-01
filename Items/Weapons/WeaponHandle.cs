using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;

namespace ConsoleGodmist.Items;

[JsonConverter(typeof(EquipmentPartConverter))]
public class WeaponHandle : IEquipmentPart
{
    public double AttackBonus { get; set; }
    public double CritChanceBonus { get; set; }
    public double CritModBonus { get; set; }
    public int Accuracy { get; set; }
    public string Name => NameAliasHelper.GetName(Alias);
    public string Alias { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int GoldCost { get; set; }
    public int MaterialCost { get; set; }
    
    public WeaponHandle() {}
}