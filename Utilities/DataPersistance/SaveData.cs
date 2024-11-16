using ConsoleGodmist.Characters;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Quests;
using ConsoleGodmist.Town;

namespace ConsoleGodmist.Utilities;

public class SaveData
{
    public PlayerCharacter Player { get; set; }
    public Difficulty Difficulty { get; set; }
    public List<Quest> Quests { get; set; }
    public Town.Town Town { get; set; }
    
    
    public SaveData() {}

    public SaveData(PlayerCharacter player, Difficulty difficulty, List<Quest> quests, Town.Town town)
    {
        Player = player;
        Difficulty = difficulty;
        Quests = quests;
        Town = town;
    }
}