using System.Runtime;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleGodmist.Characters
{
    public abstract class Character {
        public string Name { get; protected set; }
        protected Stat _maximalHealth;
        public double MaximalHealth
        {
            get => _maximalHealth.Value;
            protected set => _maximalHealth.BaseValue = value;
        }
        protected double _currentHealth;
        public double CurrentHealth {
            get => _currentHealth;
            protected set => _currentHealth = Math.Clamp(value, 0, MaximalHealth);
        }
        protected Stat _minimalAttack;
        public double MinimalAttack {
            get => _minimalAttack.Value;
            protected set => _minimalAttack.BaseValue = value;
        }
        protected Stat _maximalAttack;
        public double MaximalAttack { 
            get  => _minimalAttack.Value;
            protected set => _maximalAttack.BaseValue = Math.Min(_minimalAttack.BaseValue, value);
        }
        protected Stat _dodge;
        public double Dodge {
            get => _dodge.Value;
            protected set => _dodge.BaseValue = value;
        }
        protected Stat _physicalDefense;
        public double PhysicalDefense {
            get => _physicalDefense.Value;
            protected set => _physicalDefense.BaseValue = value;
        }
        protected Stat _magicDefense;
        public double MagicDefense
        {
            get => _magicDefense.Value;
            protected set => _magicDefense.BaseValue = value;
        }
        protected Stat _critChance;
        public double CritChance
        {
            get => Math.Clamp(_critChance.Value, 0, 1);
            protected set => _critChance.BaseValue = Math.Clamp(value, 0, 0.5D);
        }
        protected Stat _accuracy;
        public double Accuracy
        {
            get => _accuracy.Value;
            protected set => _accuracy.BaseValue = value;
        }
        protected Stat _speed;
        public double Speed
        {
            get => _speed.Value;
            set => _speed.BaseValue = value;
        }
        public int Level {get; protected set;}
        protected Character(string name, double maxHealth, double minimalAttack, double maximalAttack, double critChance,
                            double dodge, double physicalDefense, double magicDefense, double speed, int level) 
        {
            Name = name ?? locale.Nameless;
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
        public List<Text> TakeDamage(Dictionary<DamageType, double> damage)
        {
            var segments = new List<Text> { new($"{Name} {locale.Takes} ", Stylesheet.Styles["default"]) };
            var damageSegments = new List<Text>();
            foreach (var damageType in damage)
            {
                var damageTaken = DamageMitigated(damageType.Value, damageType.Key);
                CurrentHealth -= damageTaken;
                var style = damageType.Key switch
                {
                    DamageType.Physical => Stylesheet.Styles["damage-physical"],
                    DamageType.Magic => Stylesheet.Styles["damage-magic"],
                    DamageType.Bleed => Stylesheet.Styles["damage-bleed"],
                    DamageType.Poison => Stylesheet.Styles["damage-poison"],
                    DamageType.Burn => Stylesheet.Styles["damage-burn"],
                    DamageType.True => Stylesheet.Styles["damage-true"],
                    _ => Stylesheet.Styles["default"]
                };
                damageSegments.Add(new Text($"{damageTaken}", style));
                if (damageSegments.Count < damage.Count)
                    damageSegments.Add(new Text("+", Stylesheet.Styles["default"]));
            }

            segments = segments.Concat(damageSegments).ToList();
            segments.Add(new Text($" {locale.DamageGenitive}\n", Stylesheet.Styles["default"]));
            return segments;
        }
        public List<Text> TakeDamage(DamageType damageType, double damage)
        {
            var segments = new List<Text> { new($"{locale.YouTake} ", Stylesheet.Styles["default"]) };
            var damageTaken = DamageMitigated(damage, damageType);
            CurrentHealth -= damageTaken;
            var style = damageType switch
            {
                DamageType.Physical => Stylesheet.Styles["damage-physical"],
                DamageType.Magic => Stylesheet.Styles["damage-magic"],
                DamageType.Bleed => Stylesheet.Styles["damage-bleed"],
                DamageType.Poison => Stylesheet.Styles["damage-poison"],
                DamageType.Burn => Stylesheet.Styles["damage-burn"],
                DamageType.True => Stylesheet.Styles["damage-true"],
                _ => Stylesheet.Styles["default"]
            };
            segments.Add(new Text($"{damageTaken}", style));
            segments.Add(new Text($" {locale.DamageGenitive}", Stylesheet.Styles["default"]));
            return segments;
        }

        public int DamageMitigated(double damage, DamageType damageType)
        {
            return (int)(damageType switch {
                DamageType.Physical => damage * damage / (damage + PhysicalDefense),
                DamageType.Magic => damage * damage / (damage + MagicDefense),
                _ => damage
            });
        }
        protected void Heal(double heal) {
            CurrentHealth += heal;
        }

        public List<Stat> Stats() => new List<Stat>()
        {
            _maximalHealth, _minimalAttack, _maximalAttack, _critChance,
            _accuracy, _dodge, _physicalDefense, _magicDefense, _speed
        };
    }
}