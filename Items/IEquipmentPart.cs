using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Items;

public interface IEquipmentPart
{
    public string Name => NameAliasHelper.GetName(Alias);
    public string Alias { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int GoldCost { get; set; }
    public int MaterialCost { get; set; }
}