using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Characters
{
    public abstract class PlayerCharacter : Character
    {
        public CharacterClass CharacterClass { get; private set; }
        public int Gold { get; private set;}
        public int CurrentExperience { get; private set; }
        public int RequiredExperience {
            get {
                int value = 0;
                for (int i = 1; i <= Level; i++)
                {
                    value += (int)(4 * Math.Pow(i, 1.5) + 20);
                }
                return value;
            }
        }
        public int PackWeight {
            get {
                // count each item in inventory
                return 0;
            }
        }
        public int Honor {get; private set;}
        public HonorLevel HonorLevel {
            get {
                return Honor switch {
                    <-100 => HonorLevel.Exile,
                    <-75 and >=-100 => HonorLevel.Useless,
                    <-50 and >= -75 => HonorLevel.Shameful,
                    <-20 and >= -50 => HonorLevel.Uncertain,
                    <40 and >= -20 => HonorLevel.Citizen,
                    <100 and >= 40 => HonorLevel.Mercenary,
                    <150 and >= 100 => HonorLevel.Fighter,
                    <200 and >= 150 => HonorLevel.Knight,
                    >=200 => HonorLevel.Leader
                };
            }
        }

        protected PlayerCharacter(string name, double maxHealth, double minimalAttack, double maximalAttack, double critChance,
                            double dodge, double physicalDefense, double magicDefense, double speed,
                            CharacterClass characterClass) : base(name, maxHealth, minimalAttack, maximalAttack, critChance,
                                                                  dodge, physicalDefense, magicDefense, speed, 1) {
            CharacterClass = characterClass;
            Gold = 100;
            CurrentExperience = 0;
        }
        public void GainGold(int gold) {
            Gold += gold;
        }
        public void GainExperience(int experience) {
            CurrentExperience += experience;
            while (CurrentExperience >= RequiredExperience) {
                if (Level < 50)
                {
                    Level++;
                    CurrentHealth = MaximalHealth;
                }
                else
                {
                    CurrentExperience = RequiredExperience;
                    return;
                }
            }
        }
        public void GainHonor(int honor) {
            Honor = Math.Min(Honor + honor, 100);
        }
        public void LoseHonor(int honor) {
            Honor = Math.Min(Honor - honor, 100);
        }
    }
}