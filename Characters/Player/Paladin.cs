using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Characters
{
    public class Paladin : PlayerCharacter {
        public new double MaximalHealth
        {
            get
            {
                return EngineMethods.ScaledStat(_maximalHealth, 20D, Level);
            }
            protected set { 
                _maximalHealth = value;
            }
        }
        public new double MinimalAttack {
            get {
                return EngineMethods.ScaledStat(_minimalAttack, 0.6D, Level);
            }
            protected set {
                _minimalAttack = value;
            }
        }
        public new double MaximalAttack {
            get {
                return EngineMethods.ScaledStat(_maximalAttack, 0.8D, Level);
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
                return EngineMethods.ScaledStat(_physicalDefense, 0.6D, Level);
            }
            set {
                _physicalDefense = value;
            }
        }
        public new double MagicDefense {
            get {
                return EngineMethods.ScaledStat(_magicDefense, 0.6D, Level);
            }
            set {
                _magicDefense = value;
            }
        }
        // Mana
        // Capped at 120, cannot be increased
        // Start battle with full Mana, regenerates passively each turn by 20
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
        public Paladin(string name) : base(name, 450, 21, 30, 0.08D, 8, 16, 16, 35, CharacterClass.Paladin) {
            MaximalMana = 120;
            CurrentMana = MaximalMana;
            ManaRegen = 20;
        }
    }
}