using ConsoleGodmist.Combat.Skills;
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
    public string Adjective { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int GoldCost { get; set; }
    public int MaterialCost { get; set; }
    
    public ArmorPlate() {}
}