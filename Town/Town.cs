﻿using ConsoleGodmist.Characters;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Town
{
    internal static class Town
    {
        public static void EnterTown()
        {
            //AnsiConsole.Clear();
            AnsiConsole.Write(new FigletText("Arungard").Centered().Color(Color.Gold3_1));
            string[] choices = { locale.StartExpedition, locale.Blacksmith, locale.Alchemist,
                                 locale.Enchanter, locale.Druid, locale.QuestLog,
                                 locale.OpenInventory, locale.ExitGame};
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices)
                .HighlightStyle(new Style(Color.Gold3_1)));
            switch (Array.IndexOf(choices, choice))
            {
                case 0:
                    var dungeonType = ChooseDungeon();
                    Dungeon dungeon = new(dungeonType);
                    dungeon.TraverseDungeon();
                break;
                case 6: AnsiConsole.Write(PlayerHandler.player.Name + ", Poziom " + PlayerHandler.player.Level + " " + PlayerHandler.player.CharacterClass); break;
                case 7: Environment.Exit(0); break;
            }
        }
        public static DungeonType ChooseDungeon() {
            AnsiConsole.WriteLine(locale.SelectDestination);
            string[] dungeonChoices = { locale.Catacombs, locale.Forest, locale.ElvishRuins, locale.Cove, locale.Desert, locale.Temple, locale.Mountains, locale.Swamp };
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
