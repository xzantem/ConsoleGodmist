using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class Stat
{
    public double BaseValue { get; set; }
    public double ScalingFactor { get; set; }
    public List<StatModifier> Modifiers { get; private set; } = [];
    
    public Stat() {}
    
    public Stat(double baseValue, double scalingFactor)
    {
        BaseValue = baseValue;
        ScalingFactor = scalingFactor;
    }
    public Stat(double baseValue, double scalingFactor, List<StatModifier> modifiers)
    {
        BaseValue = baseValue;
        ScalingFactor = scalingFactor;
        Modifiers = modifiers;
    }

    public double Value(Character owner, string statName)
    {
        var value = UtilityMethods.ScaledStat(BaseValue, ScalingFactor, owner.Level);
        var mods = new List<StatModifier>();
        mods.AddRange(Modifiers);
        mods.AddRange(owner.PassiveEffects.GetModifiers(statName));
        if (statName is "PhysicalDefense" or "MagicDefense")
            mods.AddRange(owner.PassiveEffects.GetModifiers("TotalDefense"));
        if (statName is "BleedResistance" or "PoisonResistance" or "BurnResistance")
        { mods.AddRange(owner.PassiveEffects.GetModifiers("DoTResistanceMod"));
            mods.AddRange(owner.PassiveEffects.GetModifiers("TotalResistanceMod")); }
        else if (statName.EndsWith("Resistance") && statName.StartsWith("Debuff"))
        { mods.AddRange(owner.PassiveEffects.GetModifiers("DebuffResistanceMod"));
            mods.AddRange(owner.PassiveEffects.GetModifiers("TotalResistanceMod")); }
        else if(statName.EndsWith("Resistance"))
        { mods.AddRange(owner.PassiveEffects.GetModifiers("SuppressionResistanceMod")); 
            mods.AddRange(owner.PassiveEffects.GetModifiers("TotalResistanceMod")); }
        return UtilityMethods.CalculateModValue(value, mods);
    }

    public void Tick()
    {
        foreach (var modifier in Modifiers.ToList())
        {
            modifier.Tick();
            if (modifier.Duration <= 0)
                Modifiers.Remove(modifier);
        }
    }

    public double TotalMod(ModifierType modType)
    {
        switch (modType)
        {
            case ModifierType.Additive:
            case ModifierType.Absolute:
                return Modifiers.Sum(m => m.Type == modType ? m.Mod : 0);
            case ModifierType.Multiplicative:
            case ModifierType.Relative:
                return Modifiers
                    .Where(modifier => modifier.Type == ModifierType.Multiplicative)
                    .Aggregate(1.0, (current, modifier) => current * (1 + modifier.Mod));
            default:
                throw new ArgumentOutOfRangeException(modType.ToString());
        }
    }

    public void AddModifier(StatModifier modifier)
    {
        Modifiers.Add(modifier);
    }
}