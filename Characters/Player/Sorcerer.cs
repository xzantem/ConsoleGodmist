using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Combat.Skills.ActiveSkillEffects;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using Newtonsoft.Json;

namespace ConsoleGodmist.Characters
{
    public class Sorcerer : PlayerCharacter {
        public override string Name { get; set; }
        // Mana
        // Capped at 120, increased through various means, such as weapons or galdurites or potions
        // Start battle with full Mana, regenerates passively each turn by 15 (also can be increased)
        public Sorcerer(string name) : base(name, new Stat(250, 7.5),
            new Stat(27, 1), new Stat(36, 1.3),
            new Stat(0, 0), new Stat(6, 0.04),
            new Stat(6, 0.25), new Stat(12, 0.5),
            new Stat(45, 0), new Stat(100, 0),
            new Stat(1, 0), CharacterClass.Sorcerer) {
            Resistances.Add(StatusEffectType.Debuff, new Stat(0.7, 0));
            Resistances.Add(StatusEffectType.Bleed, new Stat(0.25, 0));
            Resistances.Add(StatusEffectType.Poison, new Stat(0.25, 0));
            Resistances.Add(StatusEffectType.Burn, new Stat(0.8, 0));
            Resistances.Add(StatusEffectType.Stun, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Freeze, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Frostbite, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Sleep, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Paralysis, new Stat(0.3, 0));
            Resistances.Add(StatusEffectType.Provocation, new Stat(0.7, 0));
            _maximalResource = new Stat(120, 0);
            _resourceRegen = new Stat(15, 0);
            CurrentResource = MaximalResource;
            ResourceType = ResourceType.Mana;
            SwitchWeapon(new Weapon(CharacterClass.Sorcerer));
            SwitchArmor(new Armor(CharacterClass.Sorcerer));
            ActiveSkills[0] = new ActiveSkill("EnergyOrb", 5, 0.75, true, 100,
            [new DealDamage(DamageType.Magic, DamageBase.Random, 1, false, false, 0)]);
            ActiveSkills[1] = new ActiveSkill("Fireball", 60, 0.75, true, 100,
                [new DealDamage(DamageType.Magic, DamageBase.Random, 1, false, false, 0),
                new InflictDoTStatusEffect(SkillTarget.Enemy, 3, 0.8, "Fireball", StatusEffectType.Burn, 0.75)]);
            ActiveSkills[2] = new ActiveSkill("Focus", 0, 0.85, true, 100,
            [new TradeHealthForResource(SkillTarget.Self, 0.1, 2)]);
            ActiveSkills[3] = new ActiveSkill("MagicShield", 80, 0.4, true, 100,
            [new GainShield(SkillTarget.Self, "MagicShield", DamageBase.Random, 2, 1, -1)]);
            ActiveSkills[4] = new ActiveSkill("ExhaustingSpells", -1, 0, true, 100,
            [new InflictGenericStatusEffect(new StatusEffect(StatusEffectType.Buff, "ExhaustingSpells", -1, "Halts mana regen, but attacks have a 70% chance to slow by 12 for 3 turns"), 1, SkillTarget.Self)]);
        }
        public Sorcerer() {}

        public void SwitchWeapon(Weapon weapon)
        {
            if (Weapon != null)
            {
                var oldWeapon = Weapon;
                Inventory.AddItem(oldWeapon);
                MinimalAttack += weapon.MinimalAttack - oldWeapon.MinimalAttack;
                MaximalAttack += weapon.MaximalAttack - oldWeapon.MaximalAttack;
                ResourceRegen += weapon.CritChance - oldWeapon.CritChance;
                CritMod += weapon.CritMod - oldWeapon.CritMod;
                MaximalResource += weapon.Accuracy - oldWeapon.Accuracy;
            }
            else
            {
                MinimalAttack += weapon.MinimalAttack;
                MaximalAttack += weapon.MaximalAttack;
                ResourceRegen += weapon.CritChance;
                CritMod += weapon.CritMod;
                MaximalResource += weapon.Accuracy;
            }
            Weapon = weapon;
        }
    }
}