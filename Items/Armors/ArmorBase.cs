using ConsoleGodmist.Combat.Skills;
using Newtonsoft.Json;

namespace ConsoleGodmist.Items.Armors;

[JsonConverter(typeof(EquipmentPartConverter))]
public class ArmorBase : IEquipmentPart
{
    public double HealthBonus { get; set; }
    public int Dodge { get; set; }
    public double PhysicalDefenseBonus { get; set; }
    public double MagicDefenseBonus { get; set; }
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : locale.ResourceManager.GetString(Alias);
    public string Alias { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int GoldCost { get; set; }
    public int MaterialCost { get; set; }
    
    public ArmorBase() {}
}