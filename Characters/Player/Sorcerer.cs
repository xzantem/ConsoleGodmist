using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Characters
{
    public class Sorcerer : PlayerCharacter {
        public new double MaximalHealth
        {
            get
            {
                return EngineMethods.ScaledStat(_maximalHealth, 7.5D, Level);
            }
            protected set { 
                _maximalHealth = value;
            }
        }
        public new double MinimalAttack {
            get {
                return EngineMethods.ScaledStat(_minimalAttack, 1D, Level);
            }
            protected set {
                _minimalAttack = value;
            }
        }
        public new double MaximalAttack {
            get {
                return EngineMethods.ScaledStat(_maximalAttack, 1.3D, Level);
            }
            protected set {
                _maximalAttack = value;
            }
        }
        public new double Dodge {
            get {
                return EngineMethods.ScaledStat(_dodge, 0.04D, Level);
            }
            private set {
                _dodge = value; 
            }
        }
        public new double PhysicalDefense {
            get {
                return EngineMethods.ScaledStat(_physicalDefense, 0.25D, Level);
            }
            set {
                _physicalDefense = value;
            }
        }
        public new double MagicDefense {
            get {
                return EngineMethods.ScaledStat(_magicDefense, 0.5D, Level);
            }
            set {
                _magicDefense = value;
            }
        }
        // Mana
        // Capped at 120, increased through various means, such as weapons or galdurites or potions
        // Start battle with full Mana, regenerates passively each turn by 15 (also can be increased)
        private double _maximalMana;
        public double MaximalMana
        {
            get
            {
                return _maximalMana;
            }
            protected set { 
                _maximalMana = value;
            }
        }
        private double _currentMana;
        public double CurrentMana {
            get {
                return _currentMana;
            }
            private set {
                _currentMana = Math.Clamp(value, 0, MaximalMana);
            }
        }
        private double _manaRegen;
        public double ManaRegen
        {
            get
            {
                return _manaRegen;
            }
            protected set { 
                _manaRegen = value;
            }
        }
        public Sorcerer(string name) : base(name, 250, 27, 36, 0, 6, 6, 12, 45, CharacterClass.Sorcerer) {
            MaximalMana = 120;
            CurrentMana = MaximalMana;
            ManaRegen = 15;
        }
    }
}