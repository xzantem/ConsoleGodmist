using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Characters;

public class BossEnemy(EnemyCharacter other, int level) : EnemyCharacter(other, level)
{
    public int CurrentPhase => CurrentHealth <= MaximalHealth / 2 ? 2 : 1;
}