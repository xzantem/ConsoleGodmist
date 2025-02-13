using System.Security.Cryptography;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Items;

public class PotionCatalyst(PotionCatalystEffect effect, int tier)
{
    public PotionCatalystEffect Effect { get; private set; } = effect;
    public int Tier { get; private set; } = tier;
    public double Strength { get; private set; } = PotionManager.GetCatalystStrength(effect, tier);
    public string Material { get; private set; } = PotionManager.GetCatalystMaterial(effect, tier);

    public string DescriptionText()
    {
        return effect switch
        {
            PotionCatalystEffect.Capacity => $"- {locale.PotionCapacity}: +{Strength}x ({NameAliasHelper.GetName(Material)} " +
            $"({PlayerHandler.player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n",
            PotionCatalystEffect.Strength => $"- {locale.EffectStrength}: +{Strength:P0} ({NameAliasHelper.GetName(Material)} " +
            $"({PlayerHandler.player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n",
            PotionCatalystEffect.Duration => $"- {locale.Duration}: +{Strength}t ({NameAliasHelper.GetName(Material)} " +
            $"({PlayerHandler.player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n",
            PotionCatalystEffect.Condensation => $"- {locale.Condensation}: -{Strength}t ({NameAliasHelper.GetName(Material)} " +
            $"({PlayerHandler.player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == Material).Value}))\n",
            _ => throw new ArgumentOutOfRangeException(nameof(effect), effect, null)
        };
    }
}