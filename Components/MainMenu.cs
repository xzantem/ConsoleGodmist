using Spectre.Console;
using System.Threading;
using System.Globalization;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Items;
using ConsoleGodmist.locale;

namespace ConsoleGodmist.Components
{
    internal static class MainMenu
    {
        public static void Menu()
        {
            AnsiConsole.Write(new FigletText("Godmist").Centered().Color(Color.DarkViolet_1));
            while(true)
            {
                string[] choices = [locale_main.NewGame, locale_main.LoadGame, locale_main.DeleteSaveFile, locale_main.ChooseLanguage, locale_main.ExitGame
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
            string[] choices = [locale_main.English__Language_, locale_main.Polish__Language_];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices)
                .Title(locale_main.ChooseLanguage)
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
            string[] choices = [$"{locale_main.Easy} (50%)", $"{locale_main.Normal} (100%)", $"{locale_main.Hard} (150%)", $"{locale_main.Nightmare} (200%)"
            ];
            var choice = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices)
                .Title(locale_main.ChooseDifficulty)
                .HighlightStyle(new Style(Color.MediumPurple3)));
            var difficulty = Array.IndexOf(choices, choice) switch
            {
                0 => Difficulty.Easy,
                1 => Difficulty.Normal,
                2 => Difficulty.Hard,
                3 => Difficulty.Nightmare,
                _ => throw new NotImplementedException()
            };
            Console.WriteLine(locale_main.Opening_1);
            Console.WriteLine(locale_main.Opening_2);
            Console.WriteLine(locale_main.Opening_3);
            Console.WriteLine(locale_main.Opening_4);
            Console.WriteLine(locale_main.Opening_5);
            AnsiConsole.Write($"\n{locale_main.TownGuard_Name}: ", new Style(Color.CornflowerBlue));
            Console.WriteLine(locale_main.TownGuard_Line);

            string[] choices1 = { locale_main.WarriorLocative, locale_main.ScoutLocative, locale_main.SorcererLocative, locale_main.PaladinLocative };
            var choice1 = AnsiConsole.Prompt(new SelectionPrompt<string>()
                .AddChoices(choices1)
                .Title($"\n...: {locale_main.Iam} ")
                .HighlightStyle(new Style(Color.MediumPurple3)));
            CharacterClass characterClass = Array.IndexOf(choices1, choice1) switch
            {
                0 => CharacterClass.Warrior,
                1 => CharacterClass.Scout,
                2 => CharacterClass.Sorcerer,
                3 => CharacterClass.Paladin,
                _ => CharacterClass.Warrior
            };
            Console.WriteLine($"\n...: {locale_main.Iam} {choice1}");
            var name = "";
            do
            {
                Console.Write($"...: {locale_main.MyNameIs} ");
                name = Console.ReadLine();
            } while (name == "");
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
