namespace ConsoleGodmist.Items.Weapons;

public class ArmorPlate
{
    public double HealthBonus { get; set; }
    public double DodgeBonus { get; set; }
    public int PhysicalDefense { get; set; }
    public int MagicDefense { get; set; }
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : locale.ResourceManager.GetString(Alias);
    public string Alias { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int Cost { get; set; }
    
    public ArmorPlate() {}
}