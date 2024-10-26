namespace ConsoleGodmist.Items.Weapons;

public class WeaponBinder
{
    public double AttackBonus { get; set; }
    public double CritChance { get; set; }
    public double CritModBonus { get; set; }
    public double AccuracyBonus { get; set; }
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : locale.ResourceManager.GetString(Alias);
    public string Alias { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int Cost { get; set; }
    
    public WeaponBinder() {}
}