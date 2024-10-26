namespace ConsoleGodmist.Items.Armors;

public class ArmorBinder
{
    public int Health { get; set; }
    public double DodgeBonus { get; set; }
    public double PhysicalDefenseBonus { get; set; }
    public double MagicDefenseBonus { get; set; }
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : locale.ResourceManager.GetString(Alias);
    public string Alias { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int Cost { get; set; }
    
    public ArmorBinder() {}
}