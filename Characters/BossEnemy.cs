using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Characters;

public class BossEnemy(EnemyCharacter other, int level) : EnemyCharacter(other, level)
{
    public int CurrentPhase => CurrentHealth <= MaximalHealth / 2 ? 2 : 1;

    public ActiveSkill[] CurrentSkills => CurrentPhase == 1
        ? [ActiveSkills[0], ActiveSkills[1], ActiveSkills[2]]
        : [ActiveSkills[3], ActiveSkills[4], ActiveSkills[5]];
}