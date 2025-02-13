using ConsoleGodmist.Combat.Battles;

namespace ConsoleGodmist.Combat.Modifiers;

public class BattleEventData(string eventType, BattleUser source, BattleUser? target = null, int? value = null)
{
    public string EventType { get; } = eventType;
    public BattleUser Source { get; } = source;
    public BattleUser? Target { get; } = target;
    public double? Value { get; } = value; // Optional, used for damage, healing, etc.
}