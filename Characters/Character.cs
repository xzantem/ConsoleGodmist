using System.Runtime;
using ConsoleGodmist.Combat.Battles;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;
using ConsoleGodmist.TextService;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Rendering;

namespace ConsoleGodmist.Characters
{
    public abstract class Character {
        public abstract string Name { get; set; }
        public Stat _maximalHealth;
        [JsonIgnore]
        public double MaximalHealth
        {
            get => _maximalHealth.Value(Level);
            protected set => _maximalHealth.BaseValue = value;
        }
        public double _currentHealth;
        [JsonIgnore]
        public double CurrentHealth {
            get => _currentHealth;
            protected set => _currentHealth = Math.Clamp(value, 0, MaximalHealth);
        }
        public Stat _minimalAttack;
        [JsonIgnore]
        public double MinimalAttack {
            get => _minimalAttack.Value(Level);
            protected set => _minimalAttack.BaseValue = value;
        }
        public Stat _maximalAttack;
        [JsonIgnore]
        public double MaximalAttack { 
            get  => _maximalAttack.Value(Level);
            protected set => _maximalAttack.BaseValue = Math.Max(_minimalAttack.BaseValue, value);
        }
        public Stat _dodge;
        [JsonIgnore]
        public double Dodge {
            get => _dodge.Value(Level);
            protected set => _dodge.BaseValue = value;
        }
        public Stat _physicalDefense;
        [JsonIgnore]
        public double PhysicalDefense {
            get => _physicalDefense.Value(Level);
            protected set => _physicalDefense.BaseValue = value;
        }
        public Stat _magicDefense;
        [JsonIgnore]
        public double MagicDefense
        {
            get => _magicDefense.Value(Level);
            protected set => _magicDefense.BaseValue = value;
        }
        public Stat _critChance;
        [JsonIgnore]
        public double CritChance
        {
            get => Math.Clamp(_critChance.Value(Level), 0, 1);
            protected set => _critChance.BaseValue = Math.Clamp(value, 0, 0.5D);
        }
        public Stat _speed;
        [JsonIgnore]
        public double Speed
        {
            get => _speed.Value(Level) + (ResourceType == ResourceType.Momentum ? CurrentResource / 10 : 0);
            protected set => _speed.BaseValue = value;
        }

        public Stat _accuracy; 
        [JsonIgnore]
        public double Accuracy
        {
            get => _accuracy.Value(Level) - (ResourceType == ResourceType.Fury ? CurrentResource / 3 : 0);
            protected set => _accuracy.BaseValue = value;
        }
        public Stat _critMod; 
        [JsonIgnore]
        public double CritMod
        {
            get => _critMod.Value(Level);
            protected set => _critMod.BaseValue = value;
        }
        public Stat _maximalResource; 
        [JsonIgnore]
        public double MaximalResource
        {
            get => _maximalResource.Value(Level);
            protected set => _maximalResource.BaseValue = value;
        }
        protected double _currentResource;
        [JsonIgnore]
        public double CurrentResource {
            get => _currentResource;
            protected set => _currentResource = Math.Clamp(value, 0, MaximalHealth);
            
        }
        public Stat _resourceRegen;
        [JsonIgnore]
        public double ResourceRegen
        {
            get
            {
                if (ResourceType == ResourceType.Momentum) return Speed >= 100 ? Speed / 10 : Speed / 5;
                return _resourceRegen.Value(Level);
            }
            set => _resourceRegen.BaseValue = value;
        }
        public Stat _damageDealt;
        [JsonIgnore]
        public double DamageDealt
        {
            get => _damageDealt.Value(Level);
            set => _damageDealt.BaseValue = value;
        }
        public Stat _damageTaken;
        [JsonIgnore]
        public double DamageTaken
        {
            get => _damageTaken.Value(Level);
            set => _damageTaken.BaseValue = value;
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
            Name = name;
            _maximalHealth = maxHealth;
            CurrentHealth = maxHealth.BaseValue;
            _minimalAttack = minimalAttack;
            _maximalAttack = maximalAttack;
            _critChance = critChance;
            _dodge = dodge;
            _physicalDefense = physicalDefense;
            _magicDefense = magicDefense;
            _resourceRegen = new Stat(0, 0);
            _damageDealt = new Stat(1, 0);
            _damageTaken = new Stat(1, 0);
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
            CharacterEventTextService.DisplayTakeDamageText(this, damageTaken
                .ToDictionary(x => x.Key, x => (int)x.Value));
            var shields = StatusEffects
                .Where(effect => effect.Type == StatusEffectType.Shield).Cast<Shield>().ToList();
            var damageSum = damageTaken.Sum(x => x.Value);
            if (shields.Count > 0)
                damageSum = StatusEffectHandler.TakeShieldsDamage(shields, this, damageSum);
            CurrentHealth -= damageSum;
        }
        public double TakeDamage(DamageType damageType, double damage)
        {
            var damageTaken = DamageMitigated(damage, damageType);
            CharacterEventTextService.DisplayTakeDamageText
                (this, new Dictionary<DamageType, int> { {damageType, (int)damageTaken}});
            var shields = StatusEffects
                .Where(effect => effect.Type == StatusEffectType.Shield).Cast<Shield>().ToList();
            if (shields.Count > 0)
                damageTaken = StatusEffectHandler.TakeShieldsDamage(shields, this, damageTaken);
            CurrentHealth -= damageTaken;
            return damageTaken;
        }

        public void UseResource(int amount)
        {
            CurrentResource -= amount;
            if (ResourceType != ResourceType.Fury)
                CurrentResource = Math.Max(CurrentResource, 0);
        }

        public void RegenResource(int amount)
        {
            CurrentResource = Math.Min(CurrentResource + amount, MaximalResource);
            //CharacterEventTextService.DisplayResourceRegenText(this, amount);
        }

        public double DamageMitigated(double damage, DamageType damageType)
        {
            damage = damageType switch
            {
                DamageType.Physical => damage * damage / (damage + PhysicalDefense),
                DamageType.Magic => damage * damage / (damage + MagicDefense),
                _ => damage
            };
            return Math.Max(DamageTaken * damage, 1);
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
                case StatType.DamageDealt:
                    _damageDealt.AddModifier(modifier);
                    break;
                case StatType.DamageTaken:
                    _damageTaken.AddModifier(modifier);
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
            _maximalResource.Decrement();
            _damageDealt.Decrement();
            _damageTaken.Decrement();
        }

        public Dictionary<StatType, StatModifier> GetModifiers()
        {
            var mods = _maximalHealth.Modifiers
                .ToDictionary(mod => StatType.MaximalHealth, mod => mod);
            foreach (var mod in _minimalAttack.Modifiers)
                mods.Add(StatType.MinimalAttack, mod);
            foreach (var mod in _maximalAttack.Modifiers)
                mods.Add(StatType.MaximalAttack, mod);
            foreach (var mod in _dodge.Modifiers)
                mods.Add(StatType.Dodge, mod);
            foreach (var mod in _physicalDefense.Modifiers)
                mods.Add(StatType.PhysicalDefense, mod);
            foreach (var mod in _magicDefense.Modifiers)
                mods.Add(StatType.MagicDefense, mod);
            foreach (var mod in _accuracy.Modifiers)
                mods.Add(StatType.Accuracy, mod);
            foreach (var mod in _speed.Modifiers)
                mods.Add(StatType.Speed, mod);
            foreach (var mod in _maximalResource.Modifiers)
                mods.Add(StatType.MaximalResource, mod);
            foreach (var mod in _damageDealt.Modifiers)
                mods.Add(StatType.DamageDealt, mod);
            foreach (var mod in _damageTaken.Modifiers)
                mods.Add(StatType.DamageTaken, mod);
            return mods;
        }
    }
}