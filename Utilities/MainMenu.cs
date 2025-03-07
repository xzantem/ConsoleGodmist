using Spectre.Console;
using System.Globalization;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Items;
using ConsoleGodmist.Quests;
using ConsoleGodmist.Town;

namespace ConsoleGodmist.Utilities
{
    internal static class MainMenu
    {
        public static void Menu()
        {
            AnsiConsole.Write(new FigletText("Godmist").Color(Color.DarkViolet_1));
            while(true)
            {
                string[] choices = [locale.NewGame, locale.LoadGame, locale.DeleteSaveFile, locale.ChooseLanguage, locale.ExitGame
                ];
                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .AddChoices(choices)
                    .HighlightStyle(new Style(Color.MediumPurple3)).WrapAround());
                switch (Array.IndexOf(choices, choice))
                {
                    case 0: NewGame(); return;
                    case 1: if (DataPersistanceManager.LoadGame()) return; break;
                    case 2: DataPersistanceManager.DeleteSaveFile(); break;
                    case 3: ChooseLanguage(); break;
                    case 4: Environment.Exit(0); break;
                }
            }
        }

        private static void ChooseLanguage()
        {
            string[] choices = [locale.English__Language_, locale.Polish__Language_];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices)
                .Title(locale.ChooseLanguage)
                .HighlightStyle(new Style(Color.MediumPurple3)).WrapAround());

            Thread.CurrentThread.CurrentUICulture = Array.IndexOf(choices, choice) switch
            {
                0 => CultureInfo.GetCultureInfo("en-US"),
                1 => CultureInfo.GetCultureInfo("pl-PL"),
                _ => Thread.CurrentThread.CurrentUICulture
            };
            CultureInfo.CurrentCulture = Array.IndexOf(choices, choice) switch
            {
                0 => CultureInfo.GetCultureInfo("en-US"),
                1 => CultureInfo.GetCultureInfo("pl-PL"),
                _ => Thread.CurrentThread.CurrentCulture
            };
        }

        private static void NewGame()
        {
            TownsHandler.Arungard = new Town.Town("Arungard");
            string[] choices = [$"{locale.Easy}", $"{locale.Normal}", $"{locale.Hard}", $"{locale.Nightmare}"
            ];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices)
                .Title(locale.ChooseDifficulty)
                .HighlightStyle(new Style(Color.MediumPurple3)).WrapAround());
            GameSettings.Difficulty = Array.IndexOf(choices, choice) switch
            {
                0 => Difficulty.Easy,
                1 => Difficulty.Normal,
                2 => Difficulty.Hard,
                3 => Difficulty.Nightmare,
                _ => throw new NotImplementedException()
            };
            QuestManager.InitMainQuests();
            QuestManager.InitSideQuests(true);
            Console.WriteLine(locale.Opening_1);
            Console.WriteLine(locale.Opening_2);
            Console.WriteLine(locale.Opening_3);
            Console.WriteLine(locale.Opening_4);
            Console.WriteLine(locale.Opening_5);
            AnsiConsole.Write($"\n{locale.TownGuard_Name}: ", new Style(Color.CornflowerBlue));
            Console.WriteLine(locale.TownGuard_Line);

            string[] choices1 = [locale.WarriorLocative, locale.ScoutLocative, locale.SorcererLocative, locale.PaladinLocative
            ];
            var choice1 = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices1)
                .Title($"\n???: {locale.Iam} ")
                .HighlightStyle(new Style(Color.MediumPurple3)).WrapAround());
            var characterClass = Array.IndexOf(choices1, choice1) switch
            {
                0 => CharacterClass.Warrior,
                1 => CharacterClass.Scout,
                2 => CharacterClass.Sorcerer,
                3 => CharacterClass.Paladin,
                _ => CharacterClass.Warrior
            };
            Console.WriteLine($"\n???: {locale.Iam} {choice1}");
            var prompt = new TextPrompt<string>($"???: {locale.MyNameIs} ").Validate(n => n.Length switch
            {
                > 32 => ValidationResult.Error(locale.NameTooLong),
                <= 32 => ValidationResult.Success(),
            });
            prompt.AllowEmpty = false;
            var name = AnsiConsole.Prompt(prompt);
            PlayerHandler.player = characterClass switch {
                CharacterClass.Warrior => new Warrior(name),
                CharacterClass.Scout => new Scout(name),
                CharacterClass.Sorcerer => new Sorcerer(name),
                CharacterClass.Paladin => new Paladin(name),
                _ => throw new NotImplementedException()
            };
        }
    }
}
