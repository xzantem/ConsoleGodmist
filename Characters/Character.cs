using System.Runtime;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleGodmist.Characters
{
    public abstract class Character {
        public abstract string Name { get; protected set; }
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
            get  => _maximalAttack.Value(Level);
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
        public Stat _maximalResource; 
        public double MaximalResource
        {
            get => _maximalResource.Value(Level);
            set => _maximalResource.BaseValue = value;
        }
        protected double _currentResource;
        public double CurrentResource {
            get => _currentResource;
            protected set => _currentResource = Math.Clamp(value, 0, MaximalHealth);
        }
        public ResourceType ResourceType { get; set; }
        public List<StatusEffect> StatusEffects { get; set; }
        
        public Dictionary<StatusEffectType, Stat> Resistances { get; set; }
        public int Level {get; set;}
        
        public ActiveSkill[] ActiveSkills { get; set; }

        protected Character() { }
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
            ActiveSkills = new ActiveSkill[5];
        }
        public void TakeDamage(Dictionary<DamageType, double> damage)
        {
            var damageTaken = damage
                .ToDictionary(damageType => damageType.Key, 
                    damageType => DamageMitigated(damageType.Value, damageType.Key));
            CurrentHealth -= damageTaken.Sum(x => x.Value);
            CharacterEventTextService.DisplayTakeDamageText(this, damageTaken
                .ToDictionary(x => x.Key, x => (int)x.Value));
        }
        public void TakeDamage(DamageType damageType, double damage)
        {
            var damageTaken = DamageMitigated(damage, damageType);
            CurrentHealth -= damageTaken;
            CharacterEventTextService.DisplayTakeDamageText
                (this, new Dictionary<DamageType, int> { {damageType, (int)damageTaken}});
        }

        public void UseResource(int amount)
        {
            CurrentResource -= amount;
            if (ResourceType != ResourceType.Fury)
                CurrentResource = Math.Max(CurrentResource, 0);
        }

        public double DamageMitigated(double damage, DamageType damageType)
        {
            damage = damageType switch
            {
                DamageType.Physical => damage * damage / (damage + PhysicalDefense),
                DamageType.Magic => damage * damage / (damage + MagicDefense),
                _ => damage
            };
            var shields = StatusEffects
                .Where(effect => effect.Type == StatusEffectType.Shield).Cast<Shield>().ToList();
            if (shields.Count > 0)
                damage = StatusEffectHandler.TakeShieldsDamage(shields, this, damage);
            return Math.Max(damage, 1);
        }
        public void Heal(double heal) {
            CurrentHealth += heal;
            CharacterEventTextService.DisplayHealText(this, (int)heal);
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
                case StatType.MaximalResource:
                    _maximalResource.AddModifier(modifier);
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(stat), stat, null);
            }
        }

        public void AddResistanceModifier(StatusEffectType stat, StatModifier modifier)
        {
            Resistances[stat].AddModifier(modifier);
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