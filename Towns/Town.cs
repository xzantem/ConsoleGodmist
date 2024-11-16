using ConsoleGodmist.Characters;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Quests;
using ConsoleGodmist.TextService;
using ConsoleGodmist.Town.NPCs;
using ConsoleGodmist.Utilities;
using Spectre.Console;

namespace ConsoleGodmist.Town
{
    public class Town
    {
        public List<NPC> NPCs { get; set; }
        public string TownName { get; set; }

        public Town(string name)
        {
            NPCs =
            [
                new Alchemist("Alchemist"),
                new Blacksmith("Blacksmith"),
                new Enchanter("Enchanter")
            ];
            TownName = name;
        }
        public Town() {}

        public void EnterTown()
        {
            while (true)
            {
                //AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText(TownName).Centered().Color(Color.Gold3_1));
                string[] choices =
                [
                    locale.StartExpedition, locale.Alchemist, locale.Blacksmith,
                    locale.Enchanter, "Druid", locale.QuestLog,
                    locale.OpenInventory, locale.ShowCharacter, locale.SaveGame, locale.ExitToMenu
                ];
                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .AddChoices(choices)
                    .HighlightStyle(new Style(Color.Gold3_1)));
                switch (Array.IndexOf(choices, choice))
                {
                    case 0:
                        DungeonMovementManager.EnterDungeon(ChooseDungeon());
                        DungeonMovementManager.TraverseDungeon();
                        break;
                    case 1:
                        NPCs[0].OpenMenu();
                        break;
                    case 2:
                        NPCs[1].OpenMenu();
                        break;
                    case 3:
                        NPCs[2].OpenMenu();
                        break;
                    case 6:
                        InventoryMenuHandler.OpenInventoryMenu();
                        break;
                    case 7: AnsiConsole.Write(PlayerHandler.player.Name + ", Poziom " + PlayerHandler.player.Level + " " + PlayerHandler.player.CharacterClass); break;
                    case 8: DataPersistanceManager.SaveGame(new SaveData(PlayerHandler.player, GameSettings.Difficulty, 
                        [QuestManager.MainQuests, QuestManager.RandomizedSideQuests, QuestManager.BossSideQuests]
                        ,QuestManager.BossProgress, this)); break;
                    case 9: return;
                }
            }
        }
        private Dungeon ChooseDungeon() {
            AnsiConsole.WriteLine(locale.SelectDestination);
            string[] dungeonChoices = [locale.Catacombs, locale.Forest, locale.ElvishRuins, locale.Cove, locale.Desert, locale.Temple, locale.Mountains, locale.Swamp
            ];
            var dungeonChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(dungeonChoices)
                .HighlightStyle(new Style(Color.Gold3_1)));
            var dungeonType = Array.IndexOf(dungeonChoices, dungeonChoice) switch
            {
                0 => DungeonType.Catacombs,
                1 => DungeonType.Forest,
                2 => DungeonType.ElvishRuins,
                3 => DungeonType.Cove,
                4 => DungeonType.Desert,
                5 => DungeonType.Temple,
                6 => DungeonType.Mountains,
                7 => DungeonType.Swamp,
                _ => DungeonType.Catacombs,
            };
            var level = AnsiConsole.Prompt(
                new TextPrompt<int>($"{locale.SelectDungeonLevel} [[1-50]] ")
                    .DefaultValue(PlayerHandler.player.Level)
                    .Validate((n) => n switch
                        {
                            <1 or >50 => ValidationResult.Error(locale.InvalidLevel),
                            _ => ValidationResult.Success()
                        }
                        ));
            return new Dungeon(level, dungeonType);
        }
    }
}
