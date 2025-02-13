using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class StatusEffect(StatusEffectType type, string source, int duration, string effect = "")
{
    public StatusEffectType Type { get; private set; } = type;
    public int Duration { get; private set; } = duration;
    public string Source { get; private set; } = source;
    public string Effect { get; private set; } = effect;

    public void Tick(Character target)
    {
        Duration--;
        if (Duration <= 0) target.StatusEffects.Remove(this);
    }

    public void Extend(int turns)
    {
        Duration += turns;
    }
}