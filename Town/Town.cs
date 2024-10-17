using ConsoleGodmist.Characters;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.locale;
using Spectre.Console;

namespace ConsoleGodmist.Town
{
    internal static class Town
    {
        public static void EnterTown()
        {
            while (true)
            {
                AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Arungard").Centered().Color(Color.Gold3_1));
                string[] choices =
                [
                    locale_main.StartExpedition, locale_main.Blacksmith, locale_main.Alchemist,
                    locale_main.Enchanter, "Druid", locale_main.QuestLog,
                    locale_main.OpenInventory, locale_main.ShowCharacter, locale_main.ExitToMenu
                ];
                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .AddChoices(choices)
                    .HighlightStyle(new Style(Color.Gold3_1)));
                switch (Array.IndexOf(choices, choice))
                {
                    case 0:
                        DungeonMovementManager.EnterDungeon(new Dungeon(1, ChooseDungeon()));
                        DungeonMovementManager.TraverseDungeon();
                        break;
                    case 1:
                        PlayerHandler.player.GainExperience(100);
                        break;
                    case 6:
                        InventoryMenuHandler.OpenInventoryMenu();
                        break;
                    case 7: AnsiConsole.Write(PlayerHandler.player.Name + ", Poziom " + PlayerHandler.player.Level + " " + PlayerHandler.player.CharacterClass); break;
                    case 8: return;
                }
            }
        }
        private static DungeonType ChooseDungeon() {
            AnsiConsole.WriteLine(locale_main.SelectDestination);
            string[] dungeonChoices = [locale_main.Catacombs, locale_main.Forest, locale_main.ElvishRuins, locale_main.Cove, locale_main.Desert, locale_main.Temple, locale_main.Mountains, locale_main.Swamp
            ];
            var dungeonChoice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(dungeonChoices)
                .HighlightStyle(new Style(Color.Gold3_1)));
            return Array.IndexOf(dungeonChoices, dungeonChoice) switch
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
        }
    }
}
