using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Components;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.TextService;
using ConsoleGodmist.Town.NPCs;
using Spectre.Console;

namespace ConsoleGodmist.Town
{
    internal class Town
    {
        public Alchemist Alchemist { get; private set; } = new();
        public Blacksmith Blacksmith { get; private set; } = new();
        public Enchanter Enchanter { get; private set; } = new();

        public void EnterTown()
        {
            while (true)
            {
                //AnsiConsole.Clear();
                AnsiConsole.Write(new FigletText("Arungard").Centered().Color(Color.Gold3_1));
                string[] choices =
                [
                    locale.StartExpedition, locale.Blacksmith, locale.Alchemist,
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
                        Blacksmith.OpenMenu();
                        break;
                    case 2:
                        Alchemist.OpenMenu();
                        break;
                    case 3:
                        Enchanter.OpenMenu();
                        break;
                    case 6:
                        InventoryMenuHandler.OpenInventoryMenu();
                        break;
                    case 7: AnsiConsole.Write(PlayerHandler.player.Name + ", Poziom " + PlayerHandler.player.Level + " " + PlayerHandler.player.CharacterClass); break;
                    case 8: DataPersistanceManager.SaveGame(); break;
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
