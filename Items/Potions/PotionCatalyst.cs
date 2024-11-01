using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public class PotionCatalyst(PotionCatalystEffect effect, int tier)
{
    public PotionCatalystEffect Effect { get; private set; } = effect;
    public int Tier { get; private set; } = tier;
    public double Strength { get; private set; } = PotionManager.GetCatalystStrength(effect, tier);
    public string Material { get; private set; } = PotionManager.GetCatalystMaterial(effect, tier);
}