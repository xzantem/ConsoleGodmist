using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Combat.Skills.ActiveSkillEffects;
using ConsoleGodmist.Enums;

namespace ConsoleGodmist.Items;

public static class EquippableItemService
{
    public static double RarityPriceModifier(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Destroyed => 0.7,
            ItemRarity.Damaged => 0.85,
            ItemRarity.Uncommon => 1.15,
            ItemRarity.Rare => 1.25,
            ItemRarity.Ancient => 1.5,
            ItemRarity.Legendary => 1.75,
            ItemRarity.Mythical => 2,
            ItemRarity.Godly => 2.5,
            _ => 1
        };
    }
    public static double RarityStatModifier(ItemRarity rarity)
    {
        return rarity switch
        {
            ItemRarity.Destroyed => 0.6,
            ItemRarity.Damaged => 0.7,
            ItemRarity.Uncommon => 1.025,
            ItemRarity.Rare => 1.05,
            ItemRarity.Ancient => 1.125,
            ItemRarity.Legendary => 1.225,
            ItemRarity.Mythical => 1.35,
            ItemRarity.Godly => 1.5,
            _ => 1
        };
    }

    public static ItemRarity GetRandomRarity(int bias = 0)
    {
        var rarities = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Destroyed, bias > 1 ? 0 : 40 },
            {ItemRarity.Damaged, bias > 2 ? 0 : 60 },
            {ItemRarity.Common, bias > 3 ? 0 : 200 },
            {ItemRarity.Uncommon, bias > 4 ? 0 : 40 },
            {ItemRarity.Rare, bias > 5 ? 0 : 20 },
            {ItemRarity.Ancient, bias > 6 ? 0 : 10 },
            {ItemRarity.Legendary, bias > 7 ? 0 : 5 },
            {ItemRarity.Mythical, bias > 8 ? 0 : 2 },
            {ItemRarity.Godly, 1 }
        };
        return UtilityMethods.RandomChoice(rarities);
    }
    public static ItemRarity GetRandomGalduriteRarity(int bias = 0)
    {
        var rarities = new Dictionary<ItemRarity, int>
        {
            {ItemRarity.Common, bias > 3 ? 0 : 50 },
            {ItemRarity.Uncommon, bias > 4 ? 0 : 35 },
            {ItemRarity.Rare, bias > 5 ? 0 : 25 },
            {ItemRarity.Ancient, bias > 6 ? 0 : 20 },
            {ItemRarity.Legendary, bias > 7 ? 0 : 15 },
            {ItemRarity.Mythical, bias > 8 ? 0 : 10 },
            {ItemRarity.Godly, 5 }
        };
        return UtilityMethods.RandomChoice(rarities);
    }

    public static Weapon GetRandomWeapon(int tier, CharacterClass requiredClass)
    {
        return new Weapon(EquipmentPartManager.GetRandomPart<WeaponHead>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    public static Weapon GetRandomWeapon(int tier)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        return new Weapon(EquipmentPartManager.GetRandomPart<WeaponHead>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    public static IEquippable GetBossDrop(int tier, string bossAlias)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        var dropData = (bossAlias, requiredClass) switch {
            ("SkeletonExecutioner", CharacterClass.Warrior) => ("Armor", "MadmansHauberk"),
            ("SkeletonExecutioner", CharacterClass.Scout) => ("Armor", "PlaceholderName"),
            ("SkeletonExecutioner", CharacterClass.Sorcerer) => ("Armor", "RunicRobes"),
            ("SkeletonExecutioner", CharacterClass.Paladin) => ("Weapon", "ExecutionersHammer"),
        };
        switch (dropData.Item1)
        {
            case "Weapon":
                return new Weapon(EquipmentPartManager.GetRandomPart<WeaponHead>(tier, requiredClass),
                    EquipmentPartManager.GetRandomPart<WeaponBinder>(tier, requiredClass),
                    EquipmentPartManager.GetRandomPart<WeaponHandle>(tier, requiredClass),
                    requiredClass, Quality.Masterpiece, dropData.Item2);
            case "Armor":
                return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
                    EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
                    EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
                    requiredClass, Quality.Masterpiece, dropData.Item2);
            default:
                throw new ArgumentOutOfRangeException(nameof(dropData.Item1));
                break;
        }
    }
    public static Armor GetRandomArmor(int tier, CharacterClass requiredClass)
    {
        return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    public static Armor GetRandomArmor(int tier)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
            requiredClass,
            UtilityMethods.RandomChoice(Enum.GetValues<Quality>()
                .Where(x => x != Quality.Masterpiece).ToList()));
    }
    public static Armor GetBossArmor(int tier, string bossAlias)
    {
        var requiredClass = UtilityMethods.RandomChoice(Enum.GetValues<CharacterClass>().ToList());
        var alias = (bossAlias, requiredClass) switch
        {
            ("SkeletonExecutioner", CharacterClass.Warrior) => "MadmanHauberk"
        };
        return new Armor(EquipmentPartManager.GetRandomPart<ArmorPlate>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBinder>(tier, requiredClass),
            EquipmentPartManager.GetRandomPart<ArmorBase>(tier, requiredClass),
            requiredClass, Quality.Masterpiece, alias);
    }
    
    public static List<InnatePassiveEffect> GetInnatePassiveEffects(bool equipmentType, PlayerCharacter player,
        HashSet<GalduriteComponent> components)
    {
        var allowedTypes = new List<string>
        {
            "DamageDealtMod", "PhysicalDamageDealtMod", "MagicDamageDealtMod", "CritChanceMod", "CritModMod",
            "HitChanceMod", "SuppressionChanceMod","DebuffChanceMod", "DoTChanceMod", "ItemChanceMod", 
            "ResourceRegenMod", "UndeadDamageDealtMod", "HumanDamageDealtMod", "BeastDamageDealtMod",
            "DemonDamageDealtMod", "ExperienceGainMod", "GoldGainMod", "PhysicalDefensePen", "MagicDefensePen", 
            "MaximalHealthMod", "DodgeMod", "PhysicalDefenseMod", "MagicDefenseMod", "TotalDefenseMod", "SpeedMod", 
            "MaxActionPointsMod", "DoTResistanceMod", "SuppressionResistanceMod", "DebuffResistanceMod", 
            "TotalResistanceMod", "PhysicalDamageTakenMod", "MagicDamageTakenMod", "DamageTakenMod", "MaximalResourceMod",
            "BleedDamageTakenMod", "PoisonDamageTakenMod", "BurnDamageTakenMod", "ResourceCostMod", "CritSaveChanceMod"
        };
        return (from component in components
            where allowedTypes.Contains(component.EffectType)
            select new InnatePassiveEffect(player, equipmentType ? "ArmorGaldurites" : "WeaponGaldurites",
                component.EffectType, [component.EffectStrength, ModifierType.Multiplicative])).ToList();
    }

    public static List<ListenerPassiveEffect?> GetListenerPassiveEffects(bool equipmentType, PlayerCharacter player,
        HashSet<GalduriteComponent> components)
    {
        var source = equipmentType ? "ArmorGaldurites" : "WeaponGaldurites";
        return components.Select(component => component.EffectType switch
            {
                "BleedOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new InflictDoTStatusEffect(SkillTarget.Enemy, 2, 0.5,
                            "Bleed", component.EffectStrength)
                        .Execute(player, data.Target?.User, source), player, source),
                "PoisonOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new InflictDoTStatusEffect(SkillTarget.Enemy, 2, 0.5,
                            "Poison", component.EffectStrength)
                        .Execute(player, data.Target?.User, source), player, source),
                "BurnOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new InflictDoTStatusEffect(SkillTarget.Enemy, 2, 0.5,
                            "Burn", component.EffectStrength)
                        .Execute(player, data.Target?.User, source), player, source),
                "HealOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new HealTarget(SkillTarget.Self, component.EffectStrength, DamageBase.Random).Execute(
                        player, player, source), player, source),
                "ResourceOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new RegenResource(SkillTarget.Self, component.EffectStrength, DamageBase.CasterMaxHealth)
                        .Execute(player, player, source), player, source),
                "AdvanceMoveOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => data.Source.AdvanceMove((int)component.EffectStrength), player, source),
                "StunOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new InflictGenericStatusEffect("Stun", 1, component.EffectStrength,
                        source).Execute(player, data.Target?.User, source), player,
                    source),
                "FreezeOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new InflictGenericStatusEffect("Freeze", 1, component.EffectStrength,
                        source).Execute(player, data.Target?.User, source), player,
                    source),
                "SlowOnHit" => new ListenerPassiveEffect(data => data.EventType == "OnHit",
                    data => new DebuffStat(SkillTarget.Enemy, StatType.Speed, ModifierType.Additive, 20,
                        component.EffectStrength, 3).Execute(player, data.Target?.User, source), player,
                    source),
                "HealthRegenPerTurn" => new ListenerPassiveEffect(data => data.EventType == "PerTurn",
                    data => new HealTarget(SkillTarget.Self, component.EffectStrength, DamageBase.CasterMaxHealth)
                        .Execute(player, player, source), player, source),
                "ResourceRegenPerTurn" => new ListenerPassiveEffect(data => data.EventType == "PerTurn",
                    data => new RegenResource(SkillTarget.Self, component.EffectStrength, DamageBase.CasterMaxHealth)
                        .Execute(player, player, source), player, source),
                _ => null,
            })
            .ToList();
    }
}