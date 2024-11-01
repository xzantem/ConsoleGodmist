using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;

namespace ConsoleGodmist.Items;

[JsonConverter(typeof(EquipmentPartConverter))]
public class WeaponHead : IEquipmentPart
{
    public int MinimalAttack { get; set; }
    public int MaximalAttack { get; set; }
    public double CritMod { get; set; }
    public double CritChanceBonus { get; set; }
    public double AccuracyBonus { get; set; }
    public string Name => NameAliasHelper.GetName(Alias);
    public string Alias { get; set; }
    
    public string Adjective { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int GoldCost { get; set; }
    public int MaterialCost { get; set; }
    
    public WeaponHead() {}
}