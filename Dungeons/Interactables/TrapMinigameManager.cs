using System.Collections.Specialized;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Dungeons;

public static class TrapMinigameManager
{
    public static readonly int MinigameCount = 5;
    public static bool StartMinigame(Difficulty difficulty, int TrapType)
    {
        switch (TrapType)
        {
            case 0: return GambaGridChallenge(difficulty);
            case 1: return CodeChallenge(difficulty);
            case 2: return MemoryChallenge(difficulty);
            case 3: return ReactionChallenge(difficulty);
            case 4: return ColorWordleChallenge(difficulty);
        }
        return false;
    }

    private static bool CodeChallenge(Difficulty difficulty)
    {
        var size = difficulty switch
        {
            Difficulty.Easy => 3,
            Difficulty.Normal => 4,
            Difficulty.Hard => 5,
            Difficulty.Nightmare => 6,
            _ => 3,
        };
        var correctCode = new int[size];
        var playerCode = new int[size];
        var tries = 0;
        for (var i = 0; i < size; i++)
            correctCode[i] = Random.Shared.Next(0, 9);
        AnsiConsole.Write(new Text($"Code challenge: Try to match the secret digit sequence\n"));
        while (true)
        {
            AnsiConsole.Write(new Text("\nCurrent code: "));
            foreach (var t in playerCode)
                AnsiConsole.Write(new Text($"[{t}]"));
            AnsiConsole.Write("\n");
            
            var selectedAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Select action:")
                    .AddChoices("Adjust digit", "Change whole code", "Check answer"));
            switch (selectedAction)
            {
                case "Adjust digit":
                    var selectedDigit = AnsiConsole.Prompt(
                        new TextPrompt<int>("Type the digit's index: ")
                            .AddChoices(Enumerable.Range(1, size))) - 1;
                    playerCode[selectedDigit] = AnsiConsole.Prompt(
                        new TextPrompt<int>("Type the digit's desired value: ")
                            .AddChoices(Enumerable.Range(1, 9)));
                    break;
                case "Change whole code":
                    var str = AnsiConsole.Prompt(
                        new TextPrompt<string>("Type the code desired value: ")
                            .Validate(x => x.Length == size && x
                                .All(char.IsDigit), "Invalid code"));
                    playerCode = str.Select(x => int.Parse(x.ToString())).ToArray();
                    break;
                case "Check answer" when !playerCode.Where((t, i) => t != correctCode[i]).Any():
                    return true;
                case "Check answer" when tries < 10:
                    AnsiConsole.Write("Incorrect code. Try again!\nTips: \n");
                    for (var i = 0; i < size; i++)
                    {
                        var txt = (correctCode[i] - playerCode[i]) switch
                        {
                            > 0 => $"The number at position {i + 1} is too low.\n",
                            < 0 => $"The number at position {i + 1} is too high.\n",
                            _ => $"The number at position {i + 1} is correct.\n"
                        };
                        AnsiConsole.Write(txt);
                    }
                    tries++;
                    break;
                case "Check answer":
                    return false;
            }
        }
    }

    private static bool MemoryChallenge(Difficulty difficulty)
    {
        char[] characters = ['@', '#', '!', '%', '$'];
        var tries = 0;
        var playerAnswer = new List<char>();
        var size = difficulty switch
        {
            Difficulty.Easy => 5,  
            Difficulty.Normal => 7,
            Difficulty.Hard => 10,
            Difficulty.Nightmare => 12,
            _ => 3,
        };
        var answer = new char[size];
        for (var i = 0; i < size; i++)
            answer[i] = characters[Random.Shared.Next(0, characters.Length)];
        for (var i = 0; i < 5; i++)
        {
            AnsiConsole.Write(new Text($"Memory challenge: Remember the sequence that will show up in {5 - i}\n"));
            Thread.Sleep(1000);
            UtilityMethods.ClearConsole();
        }
        for (var i = size - 1; i >= 0; i--)
        {
            AnsiConsole.Write(new Text($"Memorize this sequence: "));
            for (var j = 0; j < size; j++)
            {
                var color = answer[j] switch
                {
                    '!' => Color.Red,
                    '@' => Color.Yellow,
                    '#' => Color.Green,
                    '$' => Color.Aqua,
                    '%' => Color.Fuchsia,
                    _ => Color.White,
                };
                AnsiConsole.Write(new Text($"{answer[j]} ", color));
            }
            AnsiConsole.Write(new Text($"({i})\n"));
            Thread.Sleep(1000);
            UtilityMethods.ClearConsole();
        }
        AnsiConsole.Write(new Text($"Current sequence: "));
        foreach (var t in playerAnswer)
        {
            var color = t switch
            {
                '!' => Color.Red,
                '@' => Color.Yellow,
                '#' => Color.Green,
                '$' => Color.Aqua,
                '%' => Color.Fuchsia,
                _ => Color.White,
            };
            AnsiConsole.Write(new Text($"{t} ", color));
        }

        while (answer.Length > playerAnswer.Count)
        {
            var selectedChar = AnsiConsole.Prompt(
                new TextPrompt<char>("Type the next character: ")
                    .AddChoices(characters));
            if (selectedChar == answer[playerAnswer.Count])
                playerAnswer.Add(selectedChar);
            else if (tries < 5)
            {
                AnsiConsole.Write("Incorrect code. Try again!\n");
                tries++;
            }
            else
                return false;
        }
        return true;
    }
    
    private static bool ReactionChallenge(Difficulty difficulty)
    {
        var timeLimit = difficulty switch
        {
            Difficulty.Easy => 2,
            Difficulty.Normal => 1.5,
            Difficulty.Hard => 1,
            Difficulty.Nightmare => 0.75,
            _ => 1,
        };
        var keys = new char[] { 'a', 's', 'd', 'q', 'w', 'e', 'z', 'x', 'c'};
        for (var i = 0; i < 5; i++)
        {
            AnsiConsole.Write(new Text($"Reaction challenge: Quickly press the buttons that will start appearing in {5 - i}\n"));
            Thread.Sleep(1000);
            UtilityMethods.ClearConsole();
        }
        for (var i = 0; i < 5; i++)
        {
            var desiredKey = keys[Random.Shared.Next(0, keys.Length)];
            var startTime = DateTime.UtcNow;
            var keyPressDetected = false;
            while (true)
            {
                var currentTime = DateTime.UtcNow;
                var elapsedMiliSeconds = (currentTime - startTime).TotalMilliseconds;
                if (elapsedMiliSeconds >= timeLimit * 1000)
                {
                    break;
                }
                double remainingTime = timeLimit * 1000 - elapsedMiliSeconds;
                AnsiConsole.Write($"Quick! Type the character [{Math.Round(remainingTime / 1000, 2)}] ");
                AnsiConsole.Write(new Text($"({desiredKey}): ", Stylesheet.Styles["level-up"]));
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    if (keyInfo.KeyChar == desiredKey)
                    {
                        keyPressDetected = true;
                        break;
                    }
                    else return false;
                }
                
                Thread.Sleep(100);
                AnsiConsole.Write("\n");
                UtilityMethods.ClearConsole();
            }
            if (!keyPressDetected) return false;
            AnsiConsole.Write(new Text("Good!", Stylesheet.Styles["rarity-uncommon"]));
            Thread.Sleep(1000);
        }

        return true;
    }
    private static bool ColorWordleChallenge(Difficulty difficulty)
    {
        string[] words = difficulty switch
        {
            Difficulty.Easy => ["MALAD", "MIST"], // 3-4 unique letters
            Difficulty.Normal => ["GHORN", "ARUNGARD", "CERIA", "NYLIA", "GAERASAR"], // 5 unique letters
            Difficulty.Hard => ["ARUNGARD", "SIDRIEL", "KALIGAR", "TYWYLLYCH", "MELEAUX", ], // 6 unique letters
            Difficulty.Nightmare => ["DIMONEHRIN", "TREGANDA", "SHELLANIA", "KHILRAM", "THALONIA"], // 7 unique letters
            _ => [""]
        };
        var word = words[Random.Shared.Next(0, words.Length)];
        var tries = 0;
        List<Color> colors = [Color.Red, Color.Yellow, Color.Blue, Color.Aqua, Color.Green, Color.Purple, Color.Fuchsia];
        Dictionary<char, Color> colorMap = new();
        foreach (var character in word)
        {
            if (colorMap.ContainsKey(character))
                continue;
            var color = colors[Random.Shared.Next(0, colors.Count)];
            colorMap.Add(character, color);
            colors.Remove(color);
        }
        AnsiConsole.Write(new Text("Color code challenge: Try to decipher what word is hidden behind the colors!\n"));
        while (true)
        {
            var canvas = new Canvas(word.Length, 1);
            for (var i = 0; i < word.Length; i++)
            {
                canvas.SetPixel(i, 0, colorMap[word[i]]);
            }
            AnsiConsole.Write(new Text($"Word (encoded): \n"));
            AnsiConsole.Write(canvas);
            AnsiConsole.Write("\n");
            var typedWord = AnsiConsole.Prompt(
                new TextPrompt<string>("Type in the decoded word: ")
                    .Validate(x => x.Length == word.Length, "The word's length does not match"))
                .ToUpper();
            if (typedWord == word)
                return true;
            tries++;
            if (tries >= 5)
                return false;
            AnsiConsole.Write("Incorrect word. Try again!\nTips: \n");
            for (var i = 0; i < word.Length; i++)
            {
                var txt = new Text("");
                if (word[i] == typedWord[i])
                    txt = new Text($"The letter at position {i + 1} is in the word, and at the correct position.\n", Stylesheet.Styles["rarity-uncommon"]);
                else if (word.Contains(typedWord[i]))
                    txt =  new Text($"The letter at position {i + 1} is in the word, but in another position.\n", Stylesheet.Styles["rarity-ancient"]);
                else
                    txt =  new Text($"The letter at position {i + 1} is not in the word.\n", Stylesheet.Styles["rarity-damaged"]);
                AnsiConsole.Write(txt);
            }
        }
    }
    private static bool GambaGridChallenge(Difficulty difficulty)
    {
        var size = difficulty switch
        {
            Difficulty.Easy => 5,
            Difficulty.Normal => 5,
            Difficulty.Hard => 6,
            Difficulty.Nightmare => 6,
            _ => 3,
        };
        var items = new List<string>
        {
            "A1", "A2", "A3",
            "B1", "B2", "B3",
            "C1", "C2", "C3"
        };
        var tries = difficulty switch
        {
            Difficulty.Easy => 5,
            Difficulty.Normal => 4,
            Difficulty.Hard => 4,
            Difficulty.Nightmare => 3,
            _ => 3,
        };
        var bombPosition = Random.Shared.Next(0, 9);
        AnsiConsole.Write(new Text($"Gamba grid challenge: Try to guess which squares don't contain the bomb! (You must select exactly {size} fields)\n"));
        var table = new Grid();
        table.AddColumn();
        table.AddColumn();
        table.AddColumn();
        for (var i = 0; i < items.Count; i += 3)
        {
            table.AddRow(items[i], items[i + 1], items[i + 2]);
        }
        while (true)
        {
            AnsiConsole.Write(table);
            var choices = new List<string>();
            while (choices.Count != size)
            {
                choices = AnsiConsole.Prompt(
                    new MultiSelectionPrompt<string>()
                        .Title("Feeling lucky? Choose a tile, any tile")
                        .PageSize(9)
                        .InstructionsText(
                            "Space: Select, " + 
                            "Enter: Accept")
                        .AddChoices(items).Required());
            }

            if (choices.Any(choice => Array.IndexOf(choices.ToArray(), choice) == bombPosition))
            {
                tries--;
                AnsiConsole.Write(new Text("Bomb was triggered on one of the chosen squares! ", Stylesheet.Styles["failure"]));
                if (tries > 0)
                    AnsiConsole.Write(new Text($"Tries left: {tries}\n", Stylesheet.Styles["default"]));
                else
                {
                    AnsiConsole.Write(new Text("The bomb explodes!\n", Stylesheet.Styles["failure"]));
                    return false;
                }
            }
            else return true;
        }
    }
}