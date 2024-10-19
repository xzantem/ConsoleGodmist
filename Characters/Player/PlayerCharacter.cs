using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using Spectre.Console;

namespace ConsoleGodmist.Characters
{
    public abstract class PlayerCharacter(
        string name,
        Stat maxHealth,
        Stat minimalAttack,
        Stat maximalAttack,
        Stat critChance,
        Stat dodge,
        Stat physicalDefense,
        Stat magicDefense,
        Stat speed,
        Stat accuracy,
        Stat critMod,
        CharacterClass characterClass)
        : Character(name, maxHealth, minimalAttack, maximalAttack, critChance,
            dodge, physicalDefense, magicDefense, speed, accuracy, critMod, 1)
    {
        public CharacterClass CharacterClass { get; private set; } = characterClass;
        public int Gold { get; private set;} = 100;
        public int CurrentExperience { get; private set; } = 0;
        public int RequiredExperience => CalculateExperience(Level);

        public int Honor {get; private set;}

        public HonorLevel HonorLevel
        {
            get
            {
                return Honor switch
                {
                    < -100 => HonorLevel.Exile,
                    < -75 and >= -100 => HonorLevel.Useless,
                    < -50 and >= -75 => HonorLevel.Shameful,
                    < -20 and >= -50 => HonorLevel.Uncertain,
                    < 40 and >= -20 => HonorLevel.Citizen,
                    < 100 and >= 40 => HonorLevel.Mercenary,
                    < 150 and >= 100 => HonorLevel.Fighter,
                    < 200 and >= 150 => HonorLevel.Knight,
                    >= 200 => HonorLevel.Leader
                };
            }
        }
        public Inventory Inventory { get; private set; } = new();

        public void GainGold(int gold) {
            Gold += gold;
            AnsiConsole.Write(new Text($"{locale.CurrentGold}: ", Stylesheet.Styles["default"]));
            AnsiConsole.Write(new Text($"{gold}cr", Stylesheet.Styles["gold"]));
            AnsiConsole.Write(new Text($" (+{gold}cr)\n", Stylesheet.Styles["value-gained"]));
        }
        public void LoseGold(int gold) {
            Gold -= gold;
            AnsiConsole.Write(new Text($"{locale.CurrentGold}: ", Stylesheet.Styles["default"]));
            AnsiConsole.Write(new Text($"{gold}cr", Stylesheet.Styles["gold"]));
            AnsiConsole.Write(new Text($" (-{gold}cr)", Stylesheet.Styles["value-lost"]));
        }
        public void GainExperience(int experience) {
            CurrentExperience += experience;
            AnsiConsole.Write(new Text($"{locale.YouGain} {experience} {locale.ExperienceGenitive}!\n",
                Stylesheet.Styles["default"]));
            while (CurrentExperience >= RequiredExperience) {
                if (Level < 50)
                {
                    Level++;
                    AnsiConsole.Write(new Text($"{locale.LevelUp} {Level}!\n", Stylesheet.Styles["level-up"]));
                    CurrentHealth = MaximalHealth;
                }
                else
                {
                    CurrentExperience = RequiredExperience;
                    AnsiConsole.Write(new Text($"{locale.CurrentLevel}: {Level}[{CurrentExperience}", 
                        Stylesheet.Styles["default"]));
                    AnsiConsole.Write(new Text($"(+{experience})", Stylesheet.Styles["value-gained"]));
                    AnsiConsole.Write(new Text($"/{RequiredExperience}]\n", Stylesheet.Styles["default"]));
                    return;
                }
            }
            AnsiConsole.Write(new Text($"{locale.CurrentLevel}: {Level}[{CurrentExperience}", 
                Stylesheet.Styles["default"]));
            AnsiConsole.Write(new Text($"(+{experience})", Stylesheet.Styles["value-gained"]));
            AnsiConsole.Write(new Text($"/{RequiredExperience}]\n", Stylesheet.Styles["default"]));
            AnsiConsole.Write(new BreakdownChart().Width(40).HideTags()
                .AddItem("", CurrentExperience - CalculateExperience(Level - 1), Color.Aqua)
                .AddItem("", RequiredExperience - CurrentExperience, Color.DeepSkyBlue4));
        }

        private int CalculateExperience(int level)
        {
            var value = 0;
            for (var i = 1; i <= Math.Min(level, 49); i++)
            {
                value += (int)(4 * Math.Pow(i, 1.5) + 20);
            }
            return value;
        }
        public void GainHonor(int honor) {
            Honor = Math.Min(Honor + honor, 200);
        }
        public void LoseHonor(int honor) {
            Honor = Math.Max(Honor - honor, -100);
        }
    }
}