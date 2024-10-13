using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Characters
{
    public class Scout : PlayerCharacter {
        public new double MaximalHealth
        {
            get
            {
                return EngineMethods.ScaledStat(_maximalHealth, 10D, Level);
            }
            protected set { 
                _maximalHealth = value;
            }
        }
        public new double MinimalAttack {
            get {
                return EngineMethods.ScaledStat(_minimalAttack, 0.8D, Level);
            }
            protected set {
                _minimalAttack = value;
            }
        }
        public new double MaximalAttack {
            get {
                return EngineMethods.ScaledStat(_maximalAttack, 1.1D, Level);
            }
            protected set {
                _maximalAttack = value;
            }
        }
        public new double Dodge {
            get {
                return EngineMethods.ScaledStat(_dodge, 0.08D, Level);
            }
            private set {
                _dodge = value; 
            }
        }
        public new double PhysicalDefense {
            get {
                return EngineMethods.ScaledStat(_physicalDefense, 0.33D, Level);
            }
            set {
                _physicalDefense = value;
            }
        }
        public new double MagicDefense {
            get {
                return EngineMethods.ScaledStat(_magicDefense, 0.2D, Level);
            }
            set {
                _magicDefense = value;
            }
        }
        // Momentum
        // Maximal is capped at 200
        // Each turn gain momentum equal to 20% of your speed, 10% if at 100 or above
        // Gain 1 speed for every 10 Momentum (up to 20)
        // As with all classes, used for casting skills, downside to using is losing bonus speed
        private double _maximalMomentum;
        public double MaximalMomentum
        {
            get
            {
                return _maximalMomentum;
            }
            protected set { 
                _maximalMomentum = value;
            }
        }
        private double _currentMomentum;
        public double CurrentMomentum {
            get {
                return _currentMomentum;
            }
            private set {
                _currentMomentum = Math.Clamp(value, 0, MaximalMomentum);
            }
        }
        public Scout(string name) : base(name, 300, 18, 36, 0.24D, 17, 8, 4, 55, CharacterClass.Scout) {
            MaximalMomentum = 200;
            CurrentMomentum = 0;
        }
    }
}