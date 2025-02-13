using System.Net.Security;
using ConsoleGodmist.Characters;

namespace ConsoleGodmist.Combat.Modifiers;

public class TimedPassiveEffect(Character owner, string source, string type, int duration, 
    dynamic[] effects, Action? onTick = null) : 
    InnatePassiveEffect(owner, source, type, effects)
{
    public int Duration { get; private set; } = duration;

    public void Tick()
    {
        onTick?.Invoke();
        Duration--;
        if (Duration <= 0) Owner.PassiveEffects.Remove(this);
    }

    public void Extend(int turns)
    {
        Duration += turns;
    }
}