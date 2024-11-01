using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Modifiers;

public class StatusEffect(StatusEffectType type, string source, int duration, string effect = "")
{
    public StatusEffectType Type { get; private set; } = type;
    public int Duration { get; private set; } = duration;
    public string Source { get; private set; } = source;
    public string Effect { get; private set; } = effect;
    public int RemainingDuration = duration;

    public void Handle(Character target)
    {
        if (RemainingDuration != -1 && RemainingDuration != 0)
            RemainingDuration--;
        if (RemainingDuration != 0) return;
        target.StatusEffects.Remove(this);
    }

    public void Extend(int turns)
    {
        Duration += turns;
        RemainingDuration += turns;
    }
}