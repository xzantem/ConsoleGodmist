using System.Net.NetworkInformation;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public static class PotionManager
{
    public static List<PotionComponent> PotionComponents { get; private set; }
    
    public static void InitComponents()
    {
        var path = "json/potion-components.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            PotionComponents = JsonConvert.DeserializeObject<List<PotionComponent>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }

    public static void ProcessComponent(PotionComponent component, PotionCatalyst Catalyst)
    {
        var duration = 10 + (int)(Catalyst.Effect == PotionCatalystEffect.Duration ? Catalyst.Strength : 0);
        var strength = component.EffectStrength * duration *
                       (Catalyst.Effect == PotionCatalystEffect.Strength ? 1 + Catalyst.Strength : 1) / 10;
        var condensedDuration = duration - (int)(Catalyst.Effect == PotionCatalystEffect.Condensation
            ? Catalyst.Strength
            : 0);
        switch (component.Effect)
        {
            case PotionEffect.HealthRegain:
                PlayerHandler.player.Heal(PlayerHandler.player.MaximalHealth * strength);
                break;
            case PotionEffect.HealthRegen:
                var totalHealthRegen = PlayerHandler.player.MaximalHealth * strength;
                var healthPerTurn = totalHealthRegen / condensedDuration;
                StatusEffectHandler.AddStatusEffect(
                    new Regeneration(healthPerTurn, component.Material, condensedDuration, "Health"), 
                    PlayerHandler.player);
                break;
            case PotionEffect.ResourceRegain:
                PlayerHandler.player.RegenResource((int)(PlayerHandler.player.MaximalResource * strength));
                break;
            case PotionEffect.ResourceRegen:
                var totalResourceRegen = PlayerHandler.player.MaximalResource * strength * duration / 10;
                var resourcePerTurn = totalResourceRegen / condensedDuration;
                StatusEffectHandler.AddStatusEffect(
                    new Regeneration(resourcePerTurn, component.Material, condensedDuration, "Resource"), 
                    PlayerHandler.player);
                break;
            case PotionEffect.MaxResourceIncrease:
                PlayerHandler.player.AddModifier(StatType.MaximalResource, new StatModifier(ModifierType.Multiplicative, 
                    strength, component.Material, duration));
                break;
            case PotionEffect.DamageDealtIncrease:
                PlayerHandler.player.AddModifier(StatType.DamageDealt, new StatModifier(ModifierType.Multiplicative, 
                    strength, component.Material, duration));
                break;
            case PotionEffect.DamageTakenDecrease:
                PlayerHandler.player.AddModifier(StatType.DamageTaken, new StatModifier(ModifierType.Multiplicative, 
                    -strength, component.Material, duration));
                break;
            case PotionEffect.ResistanceIncrease:
                foreach (var resistance in PlayerHandler.player.Resistances)
                {
                    PlayerHandler.player.AddResistanceModifier(resistance.Key, new StatModifier(ModifierType.Multiplicative, 
                        strength, component.Material, duration));
                }
                break;
            case PotionEffect.AccuracyIncrease:
                PlayerHandler.player.AddModifier(StatType.Accuracy, new StatModifier(ModifierType.Multiplicative, 
                    strength, component.Material, duration));
                break;
            case PotionEffect.SpeedIncrease:
                PlayerHandler.player.AddModifier(StatType.Speed, new StatModifier(ModifierType.Additive, 
                    strength, component.Material, duration));
                break;
            case PotionEffect.CritChanceIncrese:
                PlayerHandler.player.AddModifier(StatType.CritChance, new StatModifier(ModifierType.Additive, 
                    strength, component.Material, duration));
                break;
            case PotionEffect.DodgeIncrease:
                PlayerHandler.player.AddModifier(StatType.Dodge, new StatModifier(ModifierType.Additive, 
                    strength, component.Material, duration));
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public static Potion GetRandomPotion(int tier)
    {
        var possibleComponents = PotionComponents.
            Where(c => c.StrengthTier == tier).ToList();
        var components = new List<PotionComponent>();
        for (var i = 0; i < 3; i++)
        {
            var randomized = EngineMethods.RandomChoice(possibleComponents);
            components.Add(randomized);
            possibleComponents.Remove(randomized);
        }

        return new Potion("Potion", "", components,
                new PotionCatalyst(EngineMethods.RandomChoice(Enum.GetValues<PotionCatalystEffect>().ToList()), tier));
    }

    public static string GetCatalystMaterial(PotionCatalystEffect effect, int tier)
    {
        return (effect, tier) switch
        {
            (PotionCatalystEffect.Duration, 1) => "",
            (PotionCatalystEffect.Duration, 2) => "",
            (PotionCatalystEffect.Duration, 3) => "",
            (PotionCatalystEffect.Duration, 4) => "",
            (PotionCatalystEffect.Duration, 5) => "",
            
            (PotionCatalystEffect.Strength, 1) => "",
            (PotionCatalystEffect.Strength, 2) => "",
            (PotionCatalystEffect.Strength, 3) => "",
            (PotionCatalystEffect.Strength, 4) => "",
            (PotionCatalystEffect.Strength, 5) => "",
            
            (PotionCatalystEffect.Condensation, 1) => "",
            (PotionCatalystEffect.Condensation, 2) => "",
            (PotionCatalystEffect.Condensation, 3) => "",
            (PotionCatalystEffect.Condensation, 4) => "",
            (PotionCatalystEffect.Condensation, 5) => "",
            
            (PotionCatalystEffect.Capacity, 1) => "",
            (PotionCatalystEffect.Capacity, 2) => "",
            (PotionCatalystEffect.Capacity, 3) => "",
            (PotionCatalystEffect.Capacity, 4) => "",
            (PotionCatalystEffect.Capacity, 5) => ""
        };
    }
    public static double GetCatalystStrength(PotionCatalystEffect effect, int tier)
    {
        if (effect != PotionCatalystEffect.Strength) return tier;
        return tier switch
        {
            1 => 0.1, 2 => 0.12, 3 => 0.15, 4 => 0.2, 5 => 0.3
        };
    }

    public static Potion? ChoosePotion(List<Potion> potions)
    {
        var pots = potions.Select(x => $"{x.Name} ({x.CurrentCharges}/{x.MaximalCharges})").ToArray();
        var choices = pots.Append(locale.Return).ToArray();
        var choice = AnsiConsole.Prompt(new SelectionPrompt<string>().AddChoices(choices)
            .HighlightStyle(new Style(Color.Gold3_1)));
        return choice == locale.Return ? null : potions.ElementAt(Array.IndexOf(choices, choice));
    }
}