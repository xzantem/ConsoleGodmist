﻿using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Quests;

public class ActivateQuestObjective : IQuestObjective
{
    public bool IsComplete { get; set; }

    public DungeonType Target { get; set; }
    public int AmountToActivate { get; set; }
    public int QuestProgress { get; private set; }
    public void Progress(QuestObjectiveContext context)
    {
        if (context.ActivateInDungeonTarget != Target) return;
        QuestProgress++;
        if (QuestProgress >= AmountToActivate)
            IsComplete = true;
    }
}