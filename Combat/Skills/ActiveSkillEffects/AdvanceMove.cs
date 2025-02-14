using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Combat.Skills.ActiveSkillEffects;

public class AdvanceMove : IActiveSkillEffect
{
    public SkillTarget Target { get; set; }
    public double Amount { get; set; }

    public AdvanceMove(SkillTarget target, double amount)
    {
        Target = target;
        Amount = amount;
    }
    public AdvanceMove() {}

    public void Execute(Character caster, Character enemy, string source)
    {
        BattleUser? target = null;
        switch (Target)
        {
            case SkillTarget.Self:
                target = BattleManager.CurrentBattle!.Users
                    .FirstOrDefault(x => x.Key.User == caster).Key;
                break;
            case SkillTarget.Enemy:
                target = BattleManager.CurrentBattle!.Users
                    .FirstOrDefault(x => x.Key.User == enemy).Key;
                break;
        }
        target?.AdvanceMove((int)(target.ActionPointer / target.User.Speed * Amount));
    }
}