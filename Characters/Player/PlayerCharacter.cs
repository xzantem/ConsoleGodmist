using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.TextService;
using ConsoleGodmist.Utilities;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Characters
{
    [JsonConverter(typeof(PlayerJsonConverter))]
    public abstract class PlayerCharacter : Character
    {
        protected PlayerCharacter(string name,
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
            CharacterClass characterClass) : base(name, maxHealth, minimalAttack, maximalAttack, critChance,
            dodge, physicalDefense, magicDefense, speed, accuracy, critMod, 1)
        {
            CharacterClass = characterClass;
            Resistances = new Dictionary<StatusEffectType, Stat>();
        }
        public PlayerCharacter() {}

        public CharacterClass CharacterClass { get; set; }
        public int Gold { get; set;} = 100;
        public int CurrentExperience { get; set; }
        public int RequiredExperience => CalculateExperience(Level);

        public int Honor {get; set;}

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
                    < 40 and >= -20 => HonorLevel.Recruit,
                    < 100 and >= 40 => HonorLevel.Mercenary,
                    < 150 and >= 100 => HonorLevel.Fighter,
                    < 200 and >= 150 => HonorLevel.Knight,
                    >= 200 => HonorLevel.Leader
                };
            }
        }
        public Inventory Inventory { get; set; } = new();
        public Weapon Weapon { get; set; }
        public Armor Armor { get; set; }

        public void SwitchWeapon(Weapon weapon)
        {
            if (Weapon != null)
            {
                var oldWeapon = Weapon;
                Inventory.AddItem(oldWeapon);
                MinimalAttack += weapon.MinimalAttack - oldWeapon.MinimalAttack;
                MaximalAttack += weapon.MaximalAttack - oldWeapon.MaximalAttack;
                CritChance += weapon.CritChance - oldWeapon.CritChance;
                CritMod += weapon.CritMod - oldWeapon.CritMod;
                Accuracy += weapon.Accuracy - oldWeapon.Accuracy;
            }
            else
            {
                MinimalAttack += weapon.MinimalAttack;
                MaximalAttack += weapon.MaximalAttack;
                CritChance += weapon.CritChance;
                CritMod += weapon.CritMod;
                Accuracy += weapon.Accuracy;
            }
            Weapon = weapon;
        }
        public void SwitchArmor(Armor armor)
        {
            if (Armor != null)
            {
                var oldArmor = Armor;
                Inventory.AddItem(oldArmor);
                MaximalHealth += armor.MaximalHealth - oldArmor.MaximalHealth;
                CurrentHealth += armor.MaximalHealth - oldArmor.MaximalHealth;
                Dodge += armor.Dodge - oldArmor.Dodge;
                PhysicalDefense += armor.PhysicalDefense - oldArmor.PhysicalDefense;
                MagicDefense += armor.MagicDefense - oldArmor.MagicDefense;
            }
            else
            {
                MaximalHealth += armor.MaximalHealth;
                CurrentHealth += armor.MaximalHealth;
                Dodge += armor.Dodge;
                PhysicalDefense += armor.PhysicalDefense;
                MagicDefense += armor.MagicDefense;
            }
            
            Armor = armor;
        }

        public void GainGold(int gold) {
            Gold += gold;
            CharacterEventTextService.DisplayGoldGainText(this, gold);
        }
        public void LoseGold(int gold) {
            Gold -= gold;
            CharacterEventTextService.DisplayGoldLossText(this, gold);
        }
        public void GainExperience(int experience)
        {
            var experienceGained = (int)(experience * PlayerHandler.HonorExperienceModifier);
            CurrentExperience += experienceGained;
            CharacterEventTextService.DisplayExperienceGainText(experienceGained);
            while (CurrentExperience >= RequiredExperience) {
                if (Level < 50)
                {
                    Level++;
                    CharacterEventTextService.DisplayLevelUpText(Level);
                    CurrentHealth = MaximalHealth;
                }
                else
                {
                    CurrentExperience = RequiredExperience;
                    CharacterEventTextService.DisplayCurrentLevelText
                        (this, experienceGained, CalculateExperience(Level - 1));
                    return;
                }
            }
            CharacterEventTextService.DisplayCurrentLevelText
                (this, experienceGained, CalculateExperience(Level - 1));
        }
        private int CalculateExperience(int level)
        {
            var value = 0;
            for (var i = 1; i <= Math.Min(level, 49); i++)
            {
                value += (int)(12 * Math.Pow(i, 1.5) + 60);
            }
            return value;
        }
        public void GainHonor(int honor)
        {
            var gain = Math.Min(200 - Honor, honor);
            Honor += gain;
            CharacterEventTextService.DisplayHonorGainText(this, gain);
        }
        public void LoseHonor(int honor) {
            var loss = Math.Min(Honor + 100, honor);
            Honor -= loss;
            CharacterEventTextService.DisplayHonorLossText(this, loss);
        }

        public void Say(string message)
        {
            AnsiConsole.Write(new Text($"{Name}: ", Stylesheet.Styles["npc-player"]));
            AnsiConsole.Write(new Text($"{message}\n", Stylesheet.Styles["dialogue"]));
        }
    }
}