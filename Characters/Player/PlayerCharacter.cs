using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.Items.Armors;
using ConsoleGodmist.Items.Weapons;
using Spectre.Console;

namespace ConsoleGodmist.Characters
{
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
            foreach (var statusType in Enum.GetValues(typeof(StatusEffectType)))
            {
                Resistances.Add((StatusEffectType)statusType, new Stat(0.5, 0));
            }
        }

        public CharacterClass CharacterClass { get; private set; }
        public int Gold { get; private set;} = 100;
        public int CurrentExperience { get; private set; }
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
                    < 40 and >= -20 => HonorLevel.Recruit,
                    < 100 and >= 40 => HonorLevel.Mercenary,
                    < 150 and >= 100 => HonorLevel.Fighter,
                    < 200 and >= 150 => HonorLevel.Knight,
                    >= 200 => HonorLevel.Leader
                };
            }
        }
        public Inventory Inventory { get; private set; } = new();
        public Weapon Weapon { get; private set; }
        public Armor Armor { get; private set; }

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
                MinimalAttack += Weapon.MinimalAttack;
                MaximalAttack += Weapon.MaximalAttack;
                CritChance += Weapon.CritChance;
                CritMod += Weapon.CritMod;
                Accuracy += Weapon.Accuracy;
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
                Dodge += armor.Dodge - oldArmor.Dodge;
                PhysicalDefense += armor.PhysicalDefense - oldArmor.PhysicalDefense;
                MagicDefense += armor.MagicDefense - oldArmor.MagicDefense;
            }
            else
            {
                MaximalHealth += Armor.MaximalHealth;
                Dodge += Armor.Dodge;
                PhysicalDefense += Armor.PhysicalDefense;
                MagicDefense += Armor.MagicDefense;
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
            var experienceGained = HonorLevel switch
            {
                HonorLevel.Exile => experience,
                HonorLevel.Useless => experience,
                HonorLevel.Shameful => experience,
                HonorLevel.Uncertain => experience,
                HonorLevel.Recruit => experience,
                HonorLevel.Mercenary => experience,
                HonorLevel.Fighter => (int)(experience * 1.1),
                HonorLevel.Knight => (int)(experience * 1.2),
                HonorLevel.Leader => (int)(experience * 1.5),
                _ => throw new ArgumentOutOfRangeException()
            };
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
                value += (int)(4 * Math.Pow(i, 1.5) + 20);
            }
            return value;
        }
        public void GainHonor(int honor)
        {
            var gain = Math.Min(Honor + honor, 200);
            Honor = gain;
            CharacterEventTextService.DisplayHonorGainText(this, gain);
        }
        public void LoseHonor(int honor) {
            var loss = Math.Max(Honor - honor, -100);
            Honor = loss;
            CharacterEventTextService.DisplayHonorLossText(this, loss);
        }
    }
}