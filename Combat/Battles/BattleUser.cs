using ConsoleGodmist.Characters;

namespace ConsoleGodmist.Combat.Battles;

public class BattleUser
{
    public Character User { get; private set; }
    public bool MovedThisTurn { get; private set; }
    public double ActionPointer { get; private set; }
    public int ActionValue { get; private set; }

    private const double ActionPointerInitial = 4;

    public BattleUser(Character user)
    {
        User = user;
        ResetAction();
    }

    public void ResetAction()
    {
        ActionPointer = Math.Pow(10, ActionPointerInitial);
        ActionValue = (int)(ActionPointer / User.Speed);
    }

    public bool TryMove()
    {
        ActionValue--;
        if (ActionValue != 0) return false;
        ResetAction();
        MovedThisTurn = true;
        return true;
    }

    public void StartNewTurn()
    {
        MovedThisTurn = false;
    }
}