using System.Runtime;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleGodmist.Characters
{
    public abstract class Character {
        public string Name { get; protected set; }
        public Stat _maximalHealth;
        public double MaximalHealth
        {
            get => _maximalHealth.Value(Level);
            set => _maximalHealth.BaseValue = value;
        }
        protected double _currentHealth;
        public double CurrentHealth {
            get => _currentHealth;
            protected set => _currentHealth = Math.Clamp(value, 0, MaximalHealth);
        }
        public Stat _minimalAttack;
        public double MinimalAttack {
            get => _minimalAttack.Value(Level);
            set => _minimalAttack.BaseValue = value;
        }
        public Stat _maximalAttack;
        public double MaximalAttack { 
            get  => _minimalAttack.Value(Level);
            set => _maximalAttack.BaseValue = Math.Max(_minimalAttack.BaseValue, value);
        }
        public Stat _dodge;
        public double Dodge {
            get => _dodge.Value(Level);
            set => _dodge.BaseValue = value;
        }
        public Stat _physicalDefense;
        public double PhysicalDefense {
            get => _physicalDefense.Value(Level);
            set => _physicalDefense.BaseValue = value;
        }
        public Stat _magicDefense;
        public double MagicDefense
        {
            get => _magicDefense.Value(Level);
            set => _magicDefense.BaseValue = value;
        }
        public Stat _critChance;

        public double CritChance
        {
            get => Math.Clamp(_critChance.Value(Level), 0, 1);
            set => _critChance.BaseValue = Math.Clamp(value, 0, 0.5D);
        }
        public Stat _speed;
        public double Speed
        {
            get => _speed.Value(Level);
            set => _speed.BaseValue = value;
        }
        public Stat _accuracy; 
        public double Accuracy
        {
            get => _accuracy.Value(Level);
            set => _accuracy.BaseValue = value;
        }
        public Stat _critMod; 
        public double CritMod
        {
            get => _critMod.Value(Level);
            set => _critMod.BaseValue = value;
        }
        public List<StatusEffect> StatusEffects { get; set; }
        public int Level {get; set;}
        
        protected Character() {}
        protected Character(string name, Stat maxHealth, Stat minimalAttack, Stat maximalAttack, 
            Stat critChance, Stat dodge, Stat physicalDefense, Stat magicDefense, Stat speed, Stat accuracy,
            Stat critMod, int level = 1) 
        {
            Name = name ?? locale.Nameless;
            _maximalHealth = maxHealth;
            CurrentHealth = maxHealth.BaseValue;
            _minimalAttack = minimalAttack;
            _maximalAttack = maximalAttack;
            _critChance = critChance;
            _dodge = dodge;
            _physicalDefense = physicalDefense;
            _magicDefense = magicDefense;
            _speed = speed;
            _accuracy = accuracy;
            _critMod = critMod;
            Level = level;
            StatusEffects = [];
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
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        public void HandleStatusEffects()
        {
            var statusDict = StatusEffects
                .ToDictionary(x => x.Type, x => StatusEffects
                    .Where(s => s.Type == x.Type).Sum(s => s.Strength));
            foreach (var status in statusDict)
            {
                var damageType = status.Key switch
                {
                    StatusEffectType.Bleed => DamageType.Bleed,
                    StatusEffectType.Poison => DamageType.Poison,
                    StatusEffectType.Burn => DamageType.Burn,
                };
                AnsiConsole.Write(new Text(string.Join("", TakeDamage(damageType, status.Value))));
            }
            foreach (var statusEffect in StatusEffects.ToList())
            {
                statusEffect.RemainingDuration--;
                if (statusEffect.RemainingDuration == 0)
                    StatusEffects.Remove(statusEffect);
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
        }
    }
}