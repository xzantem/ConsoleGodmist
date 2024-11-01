using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public class PotionComponent
{
    public PotionEffect Effect { get; set; }
    public int StrengthTier { get; set; }
    public double EffectStrength { get; set; }
    public string Material { get; set; }
    
    public PotionComponent() {}
    
    
}