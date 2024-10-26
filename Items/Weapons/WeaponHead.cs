namespace ConsoleGodmist.Items.Weapons;

public class WeaponHead
{
    public int MinimalAttack { get; set; }
    public int MaximalAttack { get; set; }
    public double CritMod { get; set; }
    public double CritChanceBonus { get; set; }
    public double AccuracyBonus { get; set; }
    public string Name => locale.ResourceManager.GetString(Alias) == null ? Alias : locale.ResourceManager.GetString(Alias);
    public string Alias { get; set; }
    public int Tier { get; set; }
    public string Material { get; set; }
    public int Cost { get; set; }
    
    public WeaponHead() {}
}