using ConsoleGodmist.Characters;

namespace ConsoleGodmist.Combat.Modifiers;

public abstract class PassiveEffect(Character owner, string source)
{
    public Character Owner { get; } = owner;
    public string Source { get; } = source; // Could be "Weapon", "Skill Tree: Berserker's Wrath", etc.
}