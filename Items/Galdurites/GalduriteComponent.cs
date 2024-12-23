using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public class GalduriteComponent
{
    public string PoolColor { get; set; }
    public string EffectTier { get; set; }
    public double EffectStrength { get; set; }
    public GalduriteEffectType EffectType { get; set; }

    public string EffectText
    {
        get
        {
            return EffectType switch
            {
                GalduriteEffectType.TotalDamageDealt => $"Total Damage Dealt: +{EffectStrength:P0}\n",
                GalduriteEffectType.PhysicalDamageDealt => $"Physical Damage Dealt: +{EffectStrength:P0}\n",
                GalduriteEffectType.MagicDamageDealt => $"Magic Damage Dealt: +{EffectStrength:P0}\n",
                GalduriteEffectType.CritChance => $"Critical Hit Chance: +{EffectStrength:P0}\n",
                GalduriteEffectType.CritDamage => $"Critical Hit Damage: +{EffectStrength:P0}\n",
                GalduriteEffectType.HitChance => $"Hit Chance: +{EffectStrength:P0}\n",
                GalduriteEffectType.SuppressionChance => $"Suppression Inflict Chance: +{EffectStrength:P0}\n",
                GalduriteEffectType.DebuffChance => $"Debuff Inflict Chance: +{EffectStrength:P0}\n",
                GalduriteEffectType.DoTChance => $"DoT Inflict Chance: +{EffectStrength:P0}\n",
                GalduriteEffectType.BleedOnHitChance => $"Inflict Bleed on Hit Chance: {EffectStrength:P0}\n",
                GalduriteEffectType.PoisonOnHitChance => $"Inflict Poison on Hit Chance: {EffectStrength:P0}\n",
                GalduriteEffectType.BurnOnHitChance => $"Inflict Burn on Hit Chance: {EffectStrength:P0}\n",
                GalduriteEffectType.HealOnHit => $"Healing on Hit (Damage Dealt): {EffectStrength:P0}\n",
                GalduriteEffectType.ResourceOnHit => $"Gain Resource on Hit (Maximal): {EffectStrength:P0}\n",
                GalduriteEffectType.MoveAdvanceOnHit => $"Advance Move on Hit: {EffectStrength:F1}\n",
                GalduriteEffectType.ItemChance => $"Item Drop Chance: +{EffectStrength:P0}\n",
            };
        }
    }

    public byte EquipmentType { get; set; }
    public GalduriteComponent() {}
}