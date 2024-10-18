using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Dungeons;

public class Trap(Difficulty difficulty, int trapType)
{
    public Difficulty Difficulty { get; private set; } = difficulty;
    public int TrapType { get; private set; } = trapType;

    public Trap(Difficulty difficulty) : this(difficulty, Random.Shared.Next(0, 4))
    {
    }

    public bool Activate()
    {
        return TrapMinigameManager.StartMinigame(Difficulty, TrapType);
    }
}