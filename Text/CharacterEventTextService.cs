using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class CharacterEventTextService
{
    public static void DisplayTakeDamageText(Character character, Dictionary<DamageType, int> damage, 
        bool isFirstPerson = false)
    {
        AnsiConsole.Write(new Text($"{character.Name} {locale.Takes} ", Stylesheet.Styles["default"]));
        foreach (var damageType in damage)
        {
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
            AnsiConsole.Write(new Text($"{damageType.Value}", style));
            if (damageType.Key != damage.Keys.Last())
                AnsiConsole.Write(new Text("+", Stylesheet.Styles["default"]));
        }
        AnsiConsole.Write(new Text($" {locale.DamageGenitive}\n", Stylesheet.Styles["default"]));
    }

    public static void DisplayHealText(Character character, int heal)
    {
        AnsiConsole.Write(new Text($"{character.Name} {locale.Heals} {heal} {locale.HealthGenitive}\n", 
            Stylesheet.Styles["default"]));
    }
    public static void DisplayResourceRegenText(Character character, int regen)
    {
        var resource = character.ResourceType switch
        {
            ResourceType.Fury => locale.FuryGenitive,
            ResourceType.Mana => locale.ManaGenitive,
            ResourceType.Momentum => locale.MomentumGenitive
        };
        AnsiConsole.Write(new Text($"{character.Name} {locale.Regenerates} {regen} {resource}\n", 
            Stylesheet.Styles["default"]));
    }
    public static void DisplayGoldGainText(PlayerCharacter character, int gold)
    {
        AnsiConsole.Write(new Text($"{locale.CurrentGold}: ", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"{character.Gold}cr", Stylesheet.Styles["gold"]));
        AnsiConsole.Write(new Text($" (+{gold}cr)\n", Stylesheet.Styles["value-gained"]));
    }
    public static void DisplayGoldLossText(PlayerCharacter character, int gold)
    {
        AnsiConsole.Write(new Text($"{locale.CurrentGold}: ", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"{character.Gold}cr", Stylesheet.Styles["gold"]));
        AnsiConsole.Write(new Text($" (-{gold}cr)\n", Stylesheet.Styles["value-lost"]));
    }
    public static void DisplayHonorGainText(PlayerCharacter character, int honor)
    {
        AnsiConsole.Write(new Text($"{locale.CurrentHonor}: ", Stylesheet.Styles["default"]));
        DisplayHonorText(character.HonorLevel);
        AnsiConsole.Write(new Text(" [", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"{character.Honor}", Stylesheet.Styles["honor"]));
        AnsiConsole.Write(new Text($" (+{honor})", Stylesheet.Styles["value-gained"]));
        AnsiConsole.Write(new Text("]\n", Stylesheet.Styles["default"]));
    }
    public static void DisplayHonorLossText(PlayerCharacter character, int honor)
    {
        AnsiConsole.Write(new Text($"{locale.CurrentHonor}: ", Stylesheet.Styles["default"]));
        DisplayHonorText(character.HonorLevel);
        AnsiConsole.Write(new Text(" [", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"{character.Honor}", Stylesheet.Styles["honor"]));
        AnsiConsole.Write(new Text($" ({honor})", Stylesheet.Styles["value-lost"]));
        AnsiConsole.Write(new Text("]\n", Stylesheet.Styles["default"]));
    }

    public static void DisplayExperienceGainText(int experience)
    {
        AnsiConsole.Write(new Text($"{locale.YouGain} {experience} {locale.ExperienceGenitive}!\n",
            Stylesheet.Styles["default"]));
    }
    public static void DisplayLevelUpText(int level)
    {
        AnsiConsole.Write(new Text($"{locale.LevelUp} {level}!\n", Stylesheet.Styles["level-up"]));
    }

    public static void DisplayCurrentLevelText(PlayerCharacter character, int experience, int calculatedExperience)
    {
        AnsiConsole.Write(new Text($"{locale.CurrentLevel}: {character.Level}[{character.CurrentExperience}", 
            Stylesheet.Styles["default"]));
        AnsiConsole.Write(new Text($"(+{experience})", Stylesheet.Styles["value-gained"]));
        AnsiConsole.Write(new Text($"/{character.RequiredExperience}]\n", Stylesheet.Styles["default"]));
        AnsiConsole.Write(new BreakdownChart().Width(40).HideTags()
            .AddItem("", character.CurrentExperience - calculatedExperience, Color.Aqua)
            .AddItem("", character.RequiredExperience - character.CurrentExperience, Color.DeepSkyBlue4));
    }
    private static void DisplayHonorText(HonorLevel honorLevel)
    {
        AnsiConsole.Write(honorLevel switch
        {
            HonorLevel.Exile => new Text($"{locale.Exile}", Stylesheet.Styles["honor-exile"]),
            HonorLevel.Useless => new Text($"{locale.Useless}", Stylesheet.Styles["honor-useless"]),
            HonorLevel.Shameful => new Text($"{locale.Shameful}", Stylesheet.Styles["honor-shameful"]),
            HonorLevel.Uncertain => new Text($"{locale.Uncertain}", Stylesheet.Styles["honor-uncertain"]),
            HonorLevel.Recruit => new Text($"{locale.Recruit}", Stylesheet.Styles["honor-citizen"]),
            HonorLevel.Mercenary => new Text($"{locale.Mercenary}", Stylesheet.Styles["honor-mercenary"]),
            HonorLevel.Fighter => new Text($"{locale.Fighter}", Stylesheet.Styles["honor-fighter"]),
            HonorLevel.Knight => new Text($"{locale.Knight}", Stylesheet.Styles["honor-knight"]),
            HonorLevel.Leader => new Text($"{locale.Leader}", Stylesheet.Styles["honor-leader"]),
            _ => throw new ArgumentOutOfRangeException(nameof(honorLevel), honorLevel, null)
        });
    }
    public static void DisplayStatusEffectText(Character target, StatusEffect statusEffect)
    {
        switch (statusEffect.Type)
        {
            case StatusEffectType.Bleed:
                AnsiConsole.Write(new Text($"{target.Name} {locale.Bleeds} {locale.And1} {locale.Takes}", Stylesheet.Styles["default"]));
                AnsiConsole.Write(new Text($" {(int)((DoTStatusEffect)statusEffect).Strength}", Stylesheet.Styles["damage-bleed"]));
                AnsiConsole.Write(new Text($" {locale.DamageGenitive} {locale.ForTheNext} " +
                                           $"{statusEffect.Duration} {locale.Turns}\n", Stylesheet.Styles["default"]));
                break;
            case StatusEffectType.Buff:
                break;
            case StatusEffectType.Debuff:
                break;
            case StatusEffectType.Poison:
                AnsiConsole.Write(new Text($"{target.Name} {locale.IsPoisoned} {locale.And1} {locale.Takes}", Stylesheet.Styles["default"]));
                AnsiConsole.Write(new Text($" {(int)((DoTStatusEffect)statusEffect).Strength}", Stylesheet.Styles["damage-poison"]));
                AnsiConsole.Write(new Text($" {locale.DamageGenitive} {locale.ForTheNext} " +
                                           $"{statusEffect.Duration} {locale.Turns}\n", Stylesheet.Styles["default"]));
                break;
            case StatusEffectType.Burn:
                AnsiConsole.Write(new Text($"{target.Name} {locale.Burns} {locale.And1} {locale.Takes}", Stylesheet.Styles["default"]));
                AnsiConsole.Write(new Text($" {(int)((DoTStatusEffect)statusEffect).Strength}", Stylesheet.Styles["damage-burn"]));
                AnsiConsole.Write(new Text($" {locale.DamageGenitive} {locale.ForTheNext} " +
                                           $"{statusEffect.Duration} {locale.Turns}\n", Stylesheet.Styles["default"]));
                break;
            case StatusEffectType.Stun:
                AnsiConsole.Write(new Text($"{target.Name} is stunned {locale.And1} cannot move {locale.ForTheNext} " +
                                           $"{statusEffect.Duration} {locale.Turns}\n", Stylesheet.Styles["default"]));
                break;
            case StatusEffectType.Freeze:
                break;
            case StatusEffectType.Frostbite:
                break;
            case StatusEffectType.Sleep:
                break;
            case StatusEffectType.Invisible:
                break;
            case StatusEffectType.Paralysis:
                break;
            case StatusEffectType.Provocation:
                break;
            case StatusEffectType.Shield:
                break;
            case StatusEffectType.Regeneration:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}