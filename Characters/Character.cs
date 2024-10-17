using System.Runtime;
using ConsoleGodmist.Enums;
using ConsoleGodmist.locale;

namespace ConsoleGodmist.Characters
{
    public abstract class Character {
        public string Name { get; protected set; }
        protected double _maximalHealth;
        public double MaximalHealth
        {
            get
            {
                return _maximalHealth;
            }
            protected set { 
                _maximalHealth = value;
            }
        }
        protected double _currentHealth;
        public double CurrentHealth {
            get {
                return _currentHealth;
            }
            protected set {
                _currentHealth = Math.Clamp(value, 0, MaximalHealth);
            }
        }
        protected double _minimalAttack;
        public double MinimalAttack {
            get {
                return _minimalAttack;
            }
            protected set {
                _minimalAttack = value;
            }
        }
        protected double _maximalAttack;
        public double MaximalAttack {
            get {
                return _maximalAttack;
            }
            protected set {
                _maximalAttack = value;
            }
        }
        protected double _dodge;
        public double Dodge {
            get {
                return _dodge;
            }
            protected set {
                _dodge = value; 
            }
        }
        protected double _physicalDefense;
        public double PhysicalDefense {
            get {
                return _physicalDefense;
            }
            protected set {
                _physicalDefense = value;
            }
        }
        protected double _magicDefense;
        public double MagicDefense {
            get {
                return _magicDefense;
            }
            protected set {
                _magicDefense = value;
            }
        }
        protected double _critChance;
        public double CritChance {
            get {
                return _critChance;
            }
            set {
                _critChance = Math.Clamp(value, 0, 0.5D);
            }
        }
        protected double _accuracy;
        public double Accuracy {
            get {
                return _accuracy;
            }
            set {
                _accuracy = value;
            }
        }
        protected double _speed;
        public double Speed {
            get {
                return _speed;
            }
            set {
                _speed = value;
            }
        }
        public int Level {get; protected set;}
        protected Character(string name, double maxHealth, double minimalAttack, double maximalAttack, double critChance,
                            double dodge, double physicalDefense, double magicDefense, double speed, int level) 
        {
            Name = name ?? locale_main.Nameless;
            MaximalHealth = Math.Max(0, maxHealth);
            CurrentHealth = maxHealth;
            MinimalAttack = minimalAttack;
            MaximalAttack = maximalAttack;
            CritChance = critChance;
            Dodge = dodge;
            PhysicalDefense = physicalDefense;
            MagicDefense = magicDefense;
            Speed = speed;
            Level = level;
        }
        protected void TakeDamage(double damage, DamageType damageType) {
            damage = damageType switch {
                DamageType.Physical => damage * damage / (damage + PhysicalDefense),
                DamageType.Magic => damage * damage / (damage + MagicDefense),
                _ => damage
            };
            CurrentHealth -= damage;
        }
        protected void Heal(double heal) {
            CurrentHealth += heal;
        }
    }
}