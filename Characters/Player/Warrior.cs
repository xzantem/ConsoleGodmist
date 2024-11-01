using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Combat.Skills.ActiveSkillEffects;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.Items;
using Newtonsoft.Json;

namespace ConsoleGodmist.Characters
{
    [JsonConverter(typeof(PlayerJsonConverter))]
    public class Warrior : PlayerCharacter {
        public override string Name { get; set; }
        // Fury
        // Maximal is capped at 50 by default
        // Using Chop (Base Attack) grants 5 Fury
        // Every point reduces accuracy by 1/3
        // As with all classes, used for casting skills
        // If a skill requires more than your maximum of Fury, you can cast it at maximum Fury, 
        // but you do not gain Fury until the deficit is compensated for (effectively in negative or debt)
        // Gain 1% damage for every 5 Fury (also lose if negative)
        public Warrior(string name) : base(name, new Stat(375, 12.5),
            new Stat(20, 0.7), new Stat(28, 0.95),
            new Stat(0, 0), new Stat(10, 0.05),
            new Stat(12, 0.45), new Stat(8, 0.3),
            new Stat(40, 0), new Stat(0, 0),
            new Stat(1, 0), CharacterClass.Warrior) {
            Resistances.Add(StatusEffectType.Debuff, new Stat(0.6, 0));
            Resistances.Add(StatusEffectType.Bleed, new Stat(0.7, 0));
            Resistances.Add(StatusEffectType.Poison, new Stat(0.5, 0));
            Resistances.Add(StatusEffectType.Burn, new Stat(0.4, 0));
            Resistances.Add(StatusEffectType.Stun, new Stat(0.4, 0));
            Resistances.Add(StatusEffectType.Freeze, new Stat(0.6, 0));
            Resistances.Add(StatusEffectType.Frostbite, new Stat(0.6, 0));
            Resistances.Add(StatusEffectType.Sleep, new Stat(0.6, 0));
            Resistances.Add(StatusEffectType.Paralysis, new Stat(0.6, 0));
            Resistances.Add(StatusEffectType.Provocation, new Stat(0.6, 0));
            _maximalResource = new Stat(50, 0);
            CurrentResource = 0;
            ResourceType = ResourceType.Fury;
            SwitchWeapon(new Weapon(CharacterClass.Warrior));
            SwitchArmor(new Armor(CharacterClass.Warrior));
            ActiveSkills[0] = new ActiveSkill("Chop", 0, false, 80,
            [new DealDamage(DamageType.Physical, DamageBase.Random, 1, true, false, 0),
                new RegenResource(SkillTarget.Self, 10, DamageBase.Flat)]);
            ActiveSkills[1] = new ActiveSkill("Kick", 20, false, 73,
                [new DebuffStat(SkillTarget.Enemy, StatType.Dodge, ModifierType.Additive, 15, 0.8, 3)]);
            ActiveSkills[2] = new ActiveSkill("WarCry", 30, true, 100,
                [new BuffStat(SkillTarget.Self, StatType.MinimalAttack, ModifierType.Multiplicative, 0.2, 1, 3),
                    new BuffStat(SkillTarget.Self, StatType.MaximalAttack, ModifierType.Multiplicative, 0.2, 1, 3)]);
            ActiveSkills[3] = new ActiveSkill("RightHook", 35, false, 72,
            [new DealDamage(DamageType.Physical, DamageBase.Minimal, 1, true, false, 0),
                new InflictGenericStatusEffect(new StatusEffect(StatusEffectType.Stun, "RightHook", 3), 0.8)]);
            ActiveSkills[4] = new ActiveSkill("FieldBandage", 40, true, 100,
                [new HealTarget(SkillTarget.Self, 0.08, DamageBase.CasterMissingHealth),
                new ClearStatusEffect(SkillTarget.Self, StatusEffectType.Bleed)]);
        }
        public Warrior() {}
    }
}