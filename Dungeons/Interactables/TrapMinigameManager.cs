﻿using System.Collections.Specialized;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.Dungeons;

public static class TrapMinigameManager
{
    public const int MinigameCount = 5;

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
        AnsiConsole.Write(new Text($"{locale.CodeChallengeIntro}\n"));
        while (true)
        {
            AnsiConsole.Write(new Text($"\n{locale.CurrentCode}: "));
            foreach (var t in playerCode)
                AnsiConsole.Write(new Text($"[{t}]"));
            AnsiConsole.Write("\n");
            List<string> choices = [locale.AdjustDigit, locale.ChangeWholeCode, locale.CheckAnswer];
            var selectedAction = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title($"{locale.ChooseNextAction}:")
                    .AddChoices(choices));
            switch (choices.IndexOf(selectedAction))
            {
                case 0:
                    var selectedDigit = AnsiConsole.Prompt(
                        new TextPrompt<int>($"{locale.TypeDigitIndex}: ")
                            .AddChoices(Enumerable.Range(1, size))) - 1;
                    playerCode[selectedDigit] = AnsiConsole.Prompt(
                        new TextPrompt<int>($"{locale.TypeDigitDesiredValue}: ")
                            .AddChoices(Enumerable.Range(1, 9)));
                    break;
                case 1:
                    var str = AnsiConsole.Prompt(
                        new TextPrompt<string>($"{locale.TypeCodeDesiredValue}: ")
                            .Validate(x => x.Length == size && x
                                .All(char.IsDigit), locale.InvalidCode));
                    playerCode = str.Select(x => int.Parse(x.ToString())).ToArray();
                    break;
                case 2 when !playerCode.Where((t, i) => t != correctCode[i]).Any():
                    return true;
                case 2 when tries < 10:
                    AnsiConsole.Write($"{locale.IncorrectCode}\n{locale.Tips}: \n");
                    for (var i = 0; i < size; i++)
                    {
                        var txt = (correctCode[i] - playerCode[i]) switch
                        {
                            > 0 => $"{locale.DigitAtPosition} {i + 1} {locale.IsTooLow}.\n",
                            < 0 => $"{locale.DigitAtPosition} {i + 1} {locale.IsTooHigh}.\n",
                            _ => $"{locale.DigitAtPosition} {i + 1} {locale.IsCorrect}.\n"
                        };
                        AnsiConsole.Write(txt);
                    }
                    tries++;
                    break;
                case 2:
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
            Difficulty.Easy => 4,  
            Difficulty.Normal => 5,
            Difficulty.Hard => 6,
            Difficulty.Nightmare => 8,
            _ => 5,
        };
        var answer = new char[size];
        for (var i = 0; i < size; i++)
            answer[i] = characters[Random.Shared.Next(0, characters.Length)];
        for (var i = 0; i < 5; i++)
        {
            WaitForSeconds(1, $"{locale.MemoryChallengeIntro} {5 - i}\n").Wait();
        }
        for (var i = size - 1; i >= 0; i--)
        {
            AnsiConsole.Write(new Text($"{locale.MemorizeThisSequence}: "));
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
            WaitForSeconds(1, $"({i})\n").Wait();
        }
        while (answer.Length > playerAnswer.Count)
        {
            AnsiConsole.Write(new Text($"{locale.CurrentSequence}: "));
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
            AnsiConsole.Write(new Text(string.Join("", Enumerable
                .Repeat("? ", size - playerAnswer.Count))));
            var selectedChar = AnsiConsole.Prompt(
                new TextPrompt<char>($"{locale.TypeNextCharacter}: ")
                    .AddChoices(characters));
            if (selectedChar == answer[playerAnswer.Count])
                playerAnswer.Add(selectedChar);
            else if (tries < 5)
            {
                AnsiConsole.Write($"{locale.IncorrectCode}\n");
                tries++;
            }
            else
                return false;
        }
        return true;
    }

    static async Task WaitForSeconds(double seconds, string txt = "")
    {
        await Task.Run(() =>
        {
            AnsiConsole.Write(new Text(txt));
            Thread.Sleep((int)(seconds * 1000.0));
            UtilityMethods.ClearConsole();
        });
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
            WaitForSeconds(1, $"{locale.ReactionChallengeIntro} {5 - i}\n").Wait();
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
                AnsiConsole.Write($"{locale.ReactionTypeCharacter} [{Math.Round(remainingTime / 1000, 2)}] ");
                AnsiConsole.Write(new Text($"({desiredKey}): ", Stylesheet.Styles["level-up"]));
                if (Console.KeyAvailable)
                {
                    var keyInfo = Console.ReadKey(true);
                    if (keyInfo.KeyChar != desiredKey) return false;
                    keyPressDetected = true;
                    break;
                }
                
                Thread.Sleep(100);
                AnsiConsole.Write("\n");
                UtilityMethods.ClearConsole();
            }
            if (!keyPressDetected) return false;
            AnsiConsole.Write(new Text($"{locale.GoodAdverb}!", Stylesheet.Styles["rarity-uncommon"]));
            Thread.Sleep(1000);
        }

        return true;
    }
    private static bool ColorWordleChallenge(Difficulty difficulty)
    {
        string[] words = difficulty switch
        {
            Difficulty.Easy =>
            [
            "MALAD", "MIST", "FIRM", "GOLD", "JUMP", "WIND", "CRUX", "VAST", "ZEAL", "LUSH",
            "BOLD", "GRIT", "RISK", "CALM", "FAIR", "DEFT", "HUSH", "KNIT", "LAZY", "MEAL",
            "NEST", "OATH", "PEAR", "QUIZ", "RIFT", "SWAN", "TOMB", "VINE", "WAVE", "YARN",
            "ZEST", "BLUR", "CHAR", "DOCK", "ELMS", "FANG", "GLOW", "HORN", "IVY", "JOLT",
            "KITE", "LACE", "MINT", "NOVA", "ORCA", "PLUM", "QUAD", "RUNE", "SKIM", "TRIM",
            "UPON", "VIAL", "WISP", "XYLO", "YOLK", "ZONE", "ALMS", "BRIM", "CHOP", "DRUM",
            "ECHO", "FLUX", "GAZE", "HERO", "INCH", "JURY", "KNOB", "LUCK", "MOAT", "NUDE",
            "OPAL", "PIKE", "QUIP", "ROAM", "SNOW", "TRACE", "UMBRA", "VEXED", "WREAK",
            "XENON", "YEARN", "ABLY", "BAIT", "CREW", "DUSK", "EMBER", "FROG", "GRIM",
            "HAZE", "IVORY", "JOKER", "KNOCK", "LIGHT", "MAGIC", "NOTCH", "OZONE", "POWER",
            "QUIRK", "RHYME", "SWORD", "THRUM", "KUSEK"
            ], // 3-4 unique letters
            Difficulty.Normal =>
            [
            "PLANT", "STONE", "BRAVE", "CLOUD", "FLOUR", "SHINY", "GLIDE", "ROGUE", "CRISP", "DWELL",
            "FRUIT", "KNAVE", "QUILT", "STORM", "WAGER", "FROST", "BLAZE", "NORTH", "SHARD", "TOWER",
            "WISER", "GLORY", "MARCH", "PEARL", "MIRTH", "SCOUT", "DEALT", "EPOCH", "LANCE", "PRIME",
            "SHOUT", "VIGOR", "SWEEP", "UNITE", "WORTH", "CITRO", "DREAM", "FLOAT", "HUMOR", "JOUST",
            "KNOLL", "LIVID", "MOUNT", "NOVEL", "OCTAL", "PATIO", "RAVEN", "SLEEP", "TRUTH", "VISTA",
            "WORST", "ABYSS", "BISON", "CRAVE", "DUSKY", "ELFIN", "FLARE", "GUILD", "HAVEN", "INFER",
            "JOLLY", "KNOCK", "LOYAL", "MAGIC", "NEVER", "OCEAN", "PLUSH", "QUIRK", "RELAX", "SOLID",
            "THORN", "URBAN", "VIRAL", "WOVEN", "YIELD", "ZEBRA", "ANGEL", "BEAST", "CHARM", "DAUNT",
            "EAGLE", "FABLE", "GRASP", "HONOR", "IMAGE", "JUNIP", "KAPPA", "LEMON", "MYSTO", "NIMBL",
            "OPERA", "PLAZA", "QUIET", "REBEL", "STORM", "TORCH", "URSAE", "VIXEN", "WALTZ", "XYLAN"
            ], // 5 unique letters
            Difficulty.Hard =>
            [
            "MARKED", "JUNGLE", "HARBOR", "SILVER", "BORDER", "FROSTY", "VELVET", "THRONE", "PRANCE", "GLYPHS",
            "MELLOW", "KNIGHT", "SWERVE", "FORGED", "PLIGHT", "ABOUND", "BLITHE", "CIPHER", "DRENCH", "FIERCE",
            "GRAVEL", "HUMBLE", "INSPIRE", "JAGGED", "KERNEL", "LANTER", "MAGNET", "NOVICE", "OYSTER", "PONDER",
            "QUAINT", "RUSTIC", "SPLICE", "TRUDGE", "UPROOT", "VORTEX", "WHIRLY", "XYLOID", "YOUNGS", "ZODIAC",
            "ANGLER", "BRISKY", "CANYON", "DUSTER", "ENGULF", "FLORAL", "GROUSE", "HUNTER", "INVOKE", "JIGSAW",
            "KARMIC", "LEGACY", "MIRROR", "NESTLE", "OBLATE", "PLUNGE", "QUAVER", "RAVISH", "SUMMIT", "TWIRLS",
            "UPTAKE", "VALISE", "WREAKS", "XENOPS", "YEOMAN", "ZITHER", "AUSTIN", "BOSQUE", "CLEFTS", "DODGER",
            "ESSENT", "FOSSIL", "GLOWER", "HICKLE", "IMPACT", "JOCKEY", "KETTLE", "LURING", "MASCOT", "NIMBLE",
            "OCTAVE", "PORTAL", "QUIZZY", "REVERB", "SELDOM", "TUNICS", "URGENT", "VERBAL", "WALTZ", "XYLENE"
            ], // 6 unique letters
            Difficulty.Nightmare =>
            [
            "MANDOLINE", "SHIPWRECK", "LABYRINTH", "TURBULENT", "ADVENTURE", "SHADOWING", "NOSTALGIA", "PROMPTING", 
            "LIGHTNING", "TRANQUIL", "CHRONICLE", "RESONANCE", "WINDSWEPT", "BREATHTAKING", "MISFORTUNE", "WHIRLPOOL",
            "CONQUEST", "BREAKFAST", "TOLERANCE", "EXISTENCE", "SYNERGIZE", "AMPLITUDE", "BACKSTORY", "EPHEMERAL",
            "FORESHADOW", "GALVANIZE", "HARMONIZE", "INCENTIVE", "JOURNALING", "KINDLINESS", "LUXURIOUS", "MESMERIZE",
            "NONENTITY", "OBSIDIAN", "PANDEMONY", "QUOTATION", "RADIATION", "SIMPLICITY", "TEMPTATION", "UNFATHOMED",
            "VIBRATION", "WONDERFUL", "XYLOPHONE", "YOUNGERLY", "ZENITHAL", "ALGORITHM", "BITTERNESS", "CONVERGENCE",
            "DIVERSION", "ENHANCERS", "FRAGMENT", "GLORIOUS", "HALLMARK", "INTRICATE", "JEOPARDY", "KILOGRAMS",
            "LUMINANCE", "MELODIOUS", "NEGATIVE", "OBLIVIOUS", "PERSUADED", "QUIVERING", "REPLENISH", "SUSPICION",
            "TANGIBLES", "UPHOLSTER", "VEGETABLE", "WONDERING", "XEROGRAPH", "YESTERDAY", "ZIGZAGGED"
            ], // 7+ unique letters
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
        AnsiConsole.Write(new Text($"{locale.ColorChallengeIntro}\n"));
        var notInWord = new HashSet<char>();
        var inWord = new HashSet<char>();
        while (true)
        {
            var canvas = new Canvas(word.Length, 1);
            for (var i = 0; i < word.Length; i++)
            {
                canvas.SetPixel(i, 0, colorMap[word[i]]);
            }
            AnsiConsole.Write(new Text($"{locale.WordEncoded}: \n"));
            AnsiConsole.Write(canvas);
            if (inWord.Count != 0)
                AnsiConsole.Write($"\n{locale.InWord}: {string.Join(", ", inWord)}");
            if (notInWord.Count != 0)
                AnsiConsole.Write($"\n{locale.NotInWord}: {string.Join(", ", notInWord)}");
            AnsiConsole.Write("\n");
            var typedWord = AnsiConsole.Prompt(
                new TextPrompt<string>($"{locale.TypeDecodedWord}: ")
                    .Validate(x => x.Length == word.Length, locale.IncorrectWordLength))
                .ToUpper();
            if (typedWord == word)
                return true;
            tries++;
            if (tries >= 5)
                return false;
            AnsiConsole.Write($"{locale.IncorrectWord}\n{locale.Tips}: \n");
            for (var i = 0; i < word.Length; i++)
            {
                var txt = new Text("");
                if (word[i] == typedWord[i])
                {
                    inWord.Add(typedWord[i]);
                    txt = new Text($"{locale.LetterAtPosition} {i + 1} {locale.CorrectWordAndPosition}\n", Stylesheet.Styles["rarity-uncommon"]);
                }
                else if (word.Contains(typedWord[i]))
                {
                    inWord.Add(typedWord[i]);
                    txt =  new Text($"{locale.LetterAtPosition} {i + 1} {locale.CorrectWordWrongPosition}\n", Stylesheet.Styles["rarity-ancient"]);
                }
                else
                {
                    notInWord.Add(typedWord[i]);
                    txt =  new Text($"{locale.LetterAtPosition} {i + 1} {locale.WrongLetter}\n", Stylesheet.Styles["rarity-damaged"]);
                }
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
        AnsiConsole.Write(new Text($"{locale.GambaGridChallengeIntro} {size})\n"));
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
                        .Title(locale.GambaGridTitle)
                        .PageSize(9)
                        .InstructionsText(
                            $"{locale.Space}: {locale.Select}, " + 
                            $"Enter, Bomb Position ({bombPosition}): {locale.Accept}")
                        .AddChoices(items).Required().WrapAround());
            }

            if (choices.Any(choice => Array.IndexOf(items.ToArray(), choice) == bombPosition))
            {
                tries--;
                AnsiConsole.Write(new Text($"{locale.BombTriggered} ", Stylesheet.Styles["failure"]));
                if (tries > 0)
                    AnsiConsole.Write(new Text($"{locale.TriesLeft}: {tries}\n", Stylesheet.Styles["default"]));
                else
                {
                    AnsiConsole.Write(new Text($"{locale.BombExplodes}\n", Stylesheet.Styles["failure"]));
                    return false;
                }
            }
            else return true;
        }
    }
}