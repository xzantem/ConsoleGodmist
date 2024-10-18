using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class Stat(double baseValue, List<StatModifier> modifiers)
{
    public double BaseValue { get; set; } = baseValue;
    public List<StatModifier> Modifiers { get; private set; } = modifiers;

    public Stat(double baseValue) : this(baseValue, []) { }

    public double Value
    {
        get
        {
            // Order of modifiers:
            // 1. Multiply by all relative modifiers
            // 2. Add all additive modifiers
            // 3. Multiply by all multiplicative modifiers
            // 4. Add all absolute modifiers
            var value = Modifiers.Where(modifier => modifier.Type == ModifierType.Relative)
                            .Aggregate(BaseValue, (current, modifier) => current * TotalMod(ModifierType.Relative)) + 
                        Modifiers.Where(modifier => modifier.Type == ModifierType.Additive)
                            .Sum(modifier => TotalMod(ModifierType.Additive));
            return Modifiers.Where(modifier => modifier.Type == ModifierType.Multiplicative)
                       .Aggregate(value, (current, modifier) => current * TotalMod(ModifierType.Multiplicative)) + 
                   Modifiers.Where(modifier => modifier.Type == ModifierType.Absolute)
                       .Sum(modifier => TotalMod(ModifierType.Absolute));
        }
    }

    public double ScaledValue
    {
        get
        {
            
        }
    }

    public void Decrement(bool removeInfiniteModifiers)
    {
        foreach (var modifier in Modifiers.ToList())
        {
            if (removeInfiniteModifiers && modifier.remainingDuration == -1)
                Modifiers.Remove(modifier);
            if (modifier.remainingDuration != -1)
                modifier.remainingDuration--;
            if (modifier.remainingDuration == 0)
                Modifiers.Remove(modifier);
        }
    }
    public double TotalMod(ModifierType modType)
    {
        switch (modType)
        {
            case ModifierType.Additive:
            case ModifierType.Absolute:
                return Modifiers.Sum(m => m.Type == modType? m.Mod : 0);
            case ModifierType.Multiplicative:
            case ModifierType.Relative:
                return Modifiers
                    .Where(modifier => modifier.Type == ModifierType.Multiplicative)
                    .Aggregate(1.0, (current, modifier) => current * (1 + modifier.Mod));
            default:
                throw new ArgumentOutOfRangeException(modType.ToString());
            
        }
    }
    public void AddModifier( ModifierType modType, double baseValue, int duration)
    {
        Modifiers.Add(new StatModifier(modType, baseValue, duration));
    }
    public void AddModifier( ModifierType modType, double baseValue)
    {
        Modifiers.Add(new StatModifier(modType, baseValue));
    }
    public void AddModifier(StatModifier modifier)
    {
        Modifiers.Add(modifier);
    }
}