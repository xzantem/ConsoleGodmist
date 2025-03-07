using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;
using Spectre.Console;

namespace ConsoleGodmist.TextService;

public static class PassiveEffectTextService
{
    public static Text PassiveEffectShortDescription(PassiveEffect effect)
    {
        return new Text("");
    }
    public static Text PassiveEffectDescription(PassiveEffect effect)
    {
        return new Text("");
    }
    
    public static Text ModifierText(StatModifier modifier, StatType statType)
    {
        return modifier.Type switch
        {
            ModifierType.Absolute or ModifierType.Additive => 
                new Text($"{NameAliasHelper.GetName(statType.ToString())} ({modifier.Source}) - " +
                         $"{modifier.Mod:+#;-#;0} [{modifier.Duration}]"),
            ModifierType.Relative or ModifierType.Multiplicative =>
                new Text($"{NameAliasHelper.GetName(statType.ToString())} ({modifier.Source}) - " +
                         $"{modifier.Mod:*#} [{modifier.Duration}]")
        };
    }
}