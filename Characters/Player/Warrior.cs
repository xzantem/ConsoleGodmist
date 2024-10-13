using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Characters
{
    public class Warrior : PlayerCharacter {
        public new double MaximalHealth
        {
            get
            {
                return EngineMethods.ScaledStat(_maximalHealth, 12.5D, Level);
            }
            protected set { 
                _maximalHealth = value;
            }
        }
        public new double MinimalAttack {
            get {
                return EngineMethods.ScaledStat(_minimalAttack, 0.7D, Level);
            }
            protected set {
                _minimalAttack = value;
            }
        }
        public new double MaximalAttack {
            get {
                return EngineMethods.ScaledStat(_maximalAttack, 0.95D, Level);
            }
            protected set {
                _maximalAttack = value;
            }
        }
        public new double Dodge {
            get {
                return EngineMethods.ScaledStat(_dodge, 0.05D, Level);
            }
            private set {
                _dodge = value; 
            }
        }
        public new double PhysicalDefense {
            get {
                return EngineMethods.ScaledStat(_physicalDefense, 0.45D, Level);
            }
            set {
                _physicalDefense = value;
            }
        }
        public new double MagicDefense {
            get {
                return EngineMethods.ScaledStat(_magicDefense, 0.3D, Level);
            }
            set {
                _magicDefense = value;
            }
        }
        private double _maximalFury;
        public double MaximalFury
        {
            get
            {
                return _maximalFury;
            }
            protected set { 
                _maximalFury = value;
            }
        }

        public new double Accuracy {
            get {
                return _accuracy - CurrentFury / 5;
            }
            set {
                _accuracy = value;
            }
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
            get {
                return _currentFury;
            }
            private set {
                _currentFury = Math.Max(value, MaximalFury);
            }
        }
        public Warrior(string name) : base(name, 375, 20, 28, 0.12D, 10, 12, 8, 40, CharacterClass.Warrior) {
            MaximalFury = 50;
            CurrentFury = 0;
        }
    }
}