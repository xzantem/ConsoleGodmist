using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Combat.Skills.ActiveSkillEffects;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Characters
{
    public class Warrior : PlayerCharacter {
        public override string Name { get; protected set; }

        public new double MaximalHealth
        {
            get => _maximalHealth.Value(Level);
            protected set =>
                _maximalHealth.BaseValue = value;
        }
        public new double MinimalAttack {
            get => _minimalAttack.Value(Level);
            protected set =>
                _minimalAttack.BaseValue = value;
        }
        public new double MaximalAttack {
            get => _maximalAttack.Value(Level);
            protected set =>
                _maximalAttack.BaseValue = value;
        }
        public new double Dodge {
            get => _dodge.Value(Level);
            protected set =>
                _dodge.BaseValue = value;
        }
        public new double PhysicalDefense {
            get => _physicalDefense.Value(Level);
            protected set =>
                _physicalDefense.BaseValue = value;
        }
        public new double MagicDefense {
            get => _magicDefense.Value(Level);
            protected set =>
                _magicDefense.BaseValue = value;
        }
        public new double Accuracy {
            get => _accuracy.Value(Level) - CurrentResource / 5; // + Weapon.Accuracy
            set => _accuracy.BaseValue = value;
        }
        // Fury
        // Maximal is capped at 50 by default
        // Using Chop (Base Attack) grants 5 Fury
        // Every 5 points reduces accuracy by 1 
        // As with all classes, used for casting skills
        // If a skill requires more than your maximum of Fury, you can cast it at maximum Fury, 
        // but you do not gain Fury until the deficit is compensated for (effectively in negative or debt)
        // Gain 1% damage for every 5 Fury (also lose if negative)
        public Warrior(string name) : base(name, new Stat(375, 12.5),
            new Stat(20, 0.7), new Stat(28, 0.95),
            new Stat(0, 0), new Stat(10, 0.05),
            new Stat(12, 0.45), new Stat(8, 0.3),
            new Stat(40, 0), new Stat(0, 0),
            new Stat(1, 0), CharacterClass.Warrior) {
            _maximalResource = new Stat(50, 0);
            CurrentResource = 0;
            ResourceType = ResourceType.Fury;
            ActiveSkills[0] = new ActiveSkill("TestDamaging", 0, false, 80,
                [new DealDamage(DamageType.Physical, DamageBase.Random, 1, true, false)]);
            ActiveSkills[1] = new ActiveSkill("TestDamaging", 0, false, 80,
                [new DealDamage(DamageType.Physical, DamageBase.Random, 1, true, false)]);
            ActiveSkills[2] = new ActiveSkill("TestDamaging", 0, false, 80,
                [new DealDamage(DamageType.Physical, DamageBase.Random, 1, true, false)]);
            ActiveSkills[3] = new ActiveSkill("TestDamaging", 0, false, 80,
                [new DealDamage(DamageType.Physical, DamageBase.Random, 1, true, false)]);
            ActiveSkills[4] = new ActiveSkill("TestDamaging", 0, false, 80,
                [new DealDamage(DamageType.Physical, DamageBase.Random, 1, true, false)]);
        }
    }
}