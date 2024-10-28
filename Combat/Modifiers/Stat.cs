using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class Stat
{
    public double BaseValue { get; set; }
    public double ScalingFactor { get; set; }
    public List<StatModifier> Modifiers { get; private set; } = [];
    
    public Stat(double baseValue, double scalingFactor)
    {
        BaseValue = baseValue;
        ScalingFactor = scalingFactor;
    }
    //public Stat(double baseValue, double scalingFactor)
    //{
     //   BaseValue = baseValue;
     //   ScalingFactor = scalingFactor;
    //}

    public double Value(int level = 1)
    {
        var value = EngineMethods.ScaledStat(BaseValue, ScalingFactor, level);
        // Order of modifiers:
        // 1. Multiply by all relative modifiers
        // 2. Add all additive modifiers
        // 3. Multiply by all multiplicative modifiers
        // 4. Add all absolute modifiers
        value = Modifiers.Where(modifier => modifier.Type == ModifierType.Relative)
            .Aggregate(value, (current, modifier) => current * (1 + modifier.Mod)) + Modifiers
            .Where(modifier => modifier.Type == ModifierType.Additive).Sum(modifier => modifier.Mod);
        return Modifiers.Where(modifier => modifier.Type == ModifierType.Multiplicative)
            .Aggregate(value, (current, modifier) => current * (1 + modifier.Mod)) + Modifiers
            .Where(modifier => modifier.Type == ModifierType.Absolute).Sum(modifier => modifier.Mod);
    }

    public void Decrement()
    {
        foreach (var modifier in Modifiers.ToList())
        {
            if (modifier.RemainingDuration != -1)
                modifier.RemainingDuration--;
            if (modifier.RemainingDuration == 0)
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