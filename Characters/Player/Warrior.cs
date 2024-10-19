using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Characters
{
    public class Warrior : PlayerCharacter {
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
        private Stat _maximalFury;
        public double MaximalFury
        {
            get => _maximalFury.Value(Level);
            protected set => _maximalFury.BaseValue = value;
        }

        public new double Accuracy {
            get => _accuracy.Value(Level) - CurrentFury / 5; // + Weapon.Accuracy
            set => _accuracy.BaseValue = value;
        }
        // Fury
        // Maximal is capped at 50 by default
        // Using Chop (Base Attack) grants 5 Fury
        // Every 5 points reduces accuracy by 1 (up to 10)
        // As with all classes, used for casting skills
        // If a skill requires more than your maximum of Fury, you can cast it at maximum Fury, 
        // but you do not gain Fury until the deficit is compensated for (effectively in negative or debt)
        // Gain 1% damage for every 5 Fury (also lose if negative)
        private double _currentFury;
        public double CurrentFury {
            get => _currentFury;
            private set => _currentFury = Math.Clamp(value, 0, MaximalFury);
        }
        public Warrior(string name) : base(name, new Stat(375, 12.5),
            new Stat(20, 0.7), new Stat(28, 0.95),
            new Stat(0, 0), new Stat(10, 0.05),
            new Stat(12, 0.45), new Stat(8, 0.3),
            new Stat(40, 0), new Stat(0, 0),
            new Stat(1, 0), CharacterClass.Warrior) {
            _maximalFury = new Stat(50, 0);
            CurrentFury = 0;
        }
        public void AddModifier(StatType stat, StatModifier modifier)
        {
            switch (stat)
            {
                case StatType.MaximalHealth:
                    _maximalHealth.AddModifier(modifier);
                    break;
                case StatType.MinimalAttack:
                    _minimalAttack.AddModifier(modifier);
                    break;
                case StatType.MaximalAttack:
                    _maximalAttack.AddModifier(modifier);
                    break;
                case StatType.Dodge:
                    _dodge.AddModifier(modifier);
                    break;
                case StatType.PhysicalDefense:
                    _physicalDefense.AddModifier(modifier);
                    break;
                case StatType.MagicDefense:
                    _magicDefense.AddModifier(modifier);
                    break;
                case StatType.CritChance:
                    _critChance.AddModifier(modifier);
                    break;
                case StatType.Speed:
                    _speed.AddModifier(modifier);
                    break;
                case StatType.Accuracy:
                    _accuracy.AddModifier(modifier);
                    break;
                case StatType.MaximalFury:
                    _maximalFury.AddModifier(modifier);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }
        public void HandleModifiers()
        {
            _maximalHealth.Decrement();
            _minimalAttack.Decrement();
            _maximalAttack.Decrement();
            _dodge.Decrement();
            _physicalDefense.Decrement();
            _magicDefense.Decrement();
            _accuracy.Decrement();
            _speed.Decrement();
            _maximalFury.Decrement();
        }
    }
}