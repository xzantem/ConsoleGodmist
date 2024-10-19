using Spectre.Console;
using System.Threading;
using System.Globalization;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Items;

namespace ConsoleGodmist.Components
{
    internal static class MainMenu
    {
        public static void Menu()
        {
            AnsiConsole.Write(new FigletText("Godmist").Centered().Color(Color.DarkViolet_1));
            while(true)
            {
                string[] choices = [locale.NewGame, locale.LoadGame, locale.DeleteSaveFile, locale.ChooseLanguage, locale.ExitGame
                ];
                var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                    .AddChoices(choices)
                    .HighlightStyle(new Style(Color.MediumPurple3)));
                switch (Array.IndexOf(choices, choice))
                {
                    case 0: NewGame(); return;
                    case 1: AnsiConsole.WriteException(new NotImplementedException()); return;
                    case 2: AnsiConsole.WriteException(new NotImplementedException()); break;
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
                .HighlightStyle(new Style(Color.MediumPurple3)));

            Thread.CurrentThread.CurrentUICulture = Array.IndexOf(choices, choice) switch
            {
                0 => CultureInfo.GetCultureInfo("en-US"),
                1 => CultureInfo.GetCultureInfo("pl-PL"),
                _ => Thread.CurrentThread.CurrentUICulture
            };
            System.Globalization.CultureInfo.CurrentCulture = Array.IndexOf(choices, choice) switch
            {
                0 => CultureInfo.GetCultureInfo("en-US"),
                1 => CultureInfo.GetCultureInfo("pl-PL"),
                _ => Thread.CurrentThread.CurrentCulture
            };
        }

        private static async void NewGame()
        {
            string[] choices = [$"{locale.Easy} (50%)", $"{locale.Normal} (100%)", $"{locale.Hard} (150%)", $"{locale.Nightmare} (200%)"
            ];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices)
                .Title(locale.ChooseDifficulty)
                .HighlightStyle(new Style(Color.MediumPurple3)));
            GameSettings.Difficulty = Array.IndexOf(choices, choice) switch
            {
                0 => Difficulty.Easy,
                1 => Difficulty.Normal,
                2 => Difficulty.Hard,
                3 => Difficulty.Nightmare,
                _ => throw new NotImplementedException()
            };
            Console.WriteLine(locale.Opening_1);
            Console.WriteLine(locale.Opening_2);
            Console.WriteLine(locale.Opening_3);
            Console.WriteLine(locale.Opening_4);
            Console.WriteLine(locale.Opening_5);
            AnsiConsole.Write($"\n{locale.TownGuard_Name}: ", new Style(Color.CornflowerBlue));
            Console.WriteLine(locale.TownGuard_Line);

            string[] choices1 = { locale.WarriorLocative, locale.ScoutLocative, locale.SorcererLocative, locale.PaladinLocative };
            var choice1 = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices1)
                .Title($"\n...: {locale.Iam} ")
                .HighlightStyle(new Style(Color.MediumPurple3)));
            CharacterClass characterClass = Array.IndexOf(choices1, choice1) switch
            {
                0 => CharacterClass.Warrior,
                1 => CharacterClass.Scout,
                2 => CharacterClass.Sorcerer,
                3 => CharacterClass.Paladin,
                _ => CharacterClass.Warrior
            };
            Console.WriteLine($"\n...: {locale.Iam} {choice1}");
            var name = "";
            do
            {
                Console.Write($"...: {locale.MyNameIs} ");
                name = Console.ReadLine();
            } while (name == "");
            PlayerHandler.player = characterClass switch {
                CharacterClass.Warrior => new Warrior(name),
                CharacterClass.Scout => new Warrior(name),
                CharacterClass.Sorcerer => new Warrior(name),
                CharacterClass.Paladin => new Warrior(name),
                _ => throw new NotImplementedException()
            };
        }
    }
}
