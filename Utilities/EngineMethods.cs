using Spectre.Console;
using System.Windows.Input;

namespace ConsoleGodmist
{
    internal static class EngineMethods
    {
        public static Random Random = new();
        public static double RandomDouble(double minValue, double maxValue)
        {
            var number = Random.NextDouble();
            return number * (maxValue - minValue) + minValue;
        }
        public static float RandomFloat(float minValue, float maxValue, int round)
        {
            float number = (float)Random.NextDouble();
            return (float)Math.Round(number * (maxValue - minValue) + minValue, round);
        }
        public static double EffectChance(double resistance, double baseChance)
        {
            baseChance = Clamp(baseChance, 0, 1);
            return Math.Max(0, 1 - 2 * resistance) * (1 - baseChance) / (3 - baseChance) + baseChance * Math.Min(1, 2 * (1 - resistance));
        }
        public static double Clamp(double number, double min, double max)
        {
            return Math.Max(Math.Min(number, max), min); 
        }
        public static int Clamp(int number, int min, int max)
        {
            return Math.Max(Math.Min(number, max), min);
        }
        public static void WriteLineSlowly(string text)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Spacebar || Console.ReadKey(true).Key == ConsoleKey.Enter)
                    cancellationTokenSource.Cancel();
            });
            foreach (var c in text)
            {
                AnsiConsole.Write(c.ToString());
                if (!cancellationTokenSource.IsCancellationRequested)
                    Thread.Sleep(PauseLength(c));
            }
            AnsiConsole.WriteLine();
        }
        public static void WriteLineSlowly(string text, Style style)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Spacebar || Console.ReadKey(true).Key == ConsoleKey.Enter)
                    cancellationTokenSource.Cancel();
            });
            foreach (var c in text)
            {
                AnsiConsole.Write(c.ToString(), style);
                if (!cancellationTokenSource.IsCancellationRequested)
                    Thread.Sleep(PauseLength(c));
            }
            AnsiConsole.WriteLine();
        }
        public static void WriteSlowly(string text)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Spacebar || Console.ReadKey(true).Key == ConsoleKey.Enter)
                    cancellationTokenSource.Cancel();
            });
            foreach (var c in text)
            {
                AnsiConsole.Write(c.ToString());
                if (!cancellationTokenSource.IsCancellationRequested)
                    Thread.Sleep(PauseLength(c));
            }
        }
        public static void WriteSlowly(string text, Style style)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            Task.Run(() =>
            {
                if (Console.ReadKey(true).Key == ConsoleKey.Spacebar)
                    cancellationTokenSource.Cancel();
            });
            foreach (var c in text)
            {
                AnsiConsole.Write(c.ToString(), style);
                if (!cancellationTokenSource.IsCancellationRequested)
                    Thread.Sleep(PauseLength(c));
            }
        }
        static int PauseLength(char c)
        {
            switch (c)
            {
                case '.':
                case ';':
                case '?':
                case '!':
                case ':':
                    return 100;
                case ',':
                    return Random.Next(35, 50);
                case ' ':
                    return Random.Next(15, 25);
                default:
                    if (char.IsLetterOrDigit(c))
                        return Random.Next(25, 35);
                    return 0;
            }
        }
        public static double ScaledStat(double baseStat, double scaleFactor, int level) {
            for (int i = 1; i < Math.Min(10, level); i++)
                baseStat += scaleFactor;
            for (int i = 10; i < Math.Min(20, level); i++)
                baseStat += 2 * scaleFactor;
            for (int i = 20; i < Math.Min(30, level); i++)
                baseStat += 3 * scaleFactor;
            for (int i = 30; i < Math.Min(40, level); i++)
                baseStat += 5 * scaleFactor;
            for (int i = 40; i < Math.Min(50, level); i++)
                baseStat += 9 * scaleFactor;
            return baseStat;
        }

        public static T RandomChoice<T>(Dictionary<T, double> choices)
        {
            var result = Random.NextDouble();
            if (Math.Abs(choices.Values.Sum() - 1) > 0.00001)
            {
                throw new ArgumentOutOfRangeException(nameof(choices), "Choices chances should add up to one");
            }
            double sum = 0;
            foreach (var choice in choices)
            {
                sum += choice.Value;
                if (result < sum)
                    return choice.Key;
            }

            throw new NullReferenceException();
        }
        public static T RandomChoice<T>(Dictionary<T, int> choices)
        {
            var result = Random.Next(1, choices.Values.Sum() + 1);
            double sum = 0;
            foreach (var choice in choices)
            {
                sum += choice.Value;
                if (result <= sum)
                    return choice.Key;
            }

            return default;
        }
        public static T RandomChoice<T>(List<T> choices)
        {
            var result = Random.Next(0, choices.Count);
            return choices[result];
        }

        public static Dictionary<string, int> ChoiceSelector()
        {
            return new Dictionary<string, int>();
        }

        public static bool Confirmation(string message, bool defaultValue = false)
        {
            return AnsiConsole.Prompt(
                new TextPrompt<bool>(message)
                    .AddChoice(true)
                    .AddChoice(false)
                    .DefaultValue(defaultValue)
                    .WithConverter(choice => choice ? locale.Y : "N"));
        }

        public static void ClearConsole(int lines = 1)
        {
            for (var i = 0; i < lines; i++)
            {
                Console.SetCursorPosition(0, Console.CursorTop - 1);
                Console.Write(new string(' ', Console.WindowWidth));
                Console.SetCursorPosition(0, Console.CursorTop);
            }
        }

        public static void ClearWithLog(List<Text> logs)
        {
            AnsiConsole.Clear();
            foreach (var log in logs)
            {
                AnsiConsole.Write(log);
            }
        }
        public static void ClearWithLog(Text log)
        {
            AnsiConsole.Clear();
            AnsiConsole.Write(log);
        }
    }
}
