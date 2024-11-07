using ConsoleGodmist.Characters;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public class WeaponLootbag : Lootbag
{
    public override int ID => 571;
    public override string Alias => "WeaponLootbag";
    public WeaponLootbag(int level)
    {
        Level = level;
    }
    
    public override bool Use()
    {
        var inInventory = PlayerHandler.player.Inventory.Items.FirstOrDefault(x => x.Key.Alias == Alias).Value;
        var toOpen = AnsiConsole.Prompt(new TextPrompt<int>(locale.HowManyToOpen + $" Up to {inInventory}: ")
            .DefaultValue(inInventory)
            .Validate(Validator));
        var amount = EngineMethods.RandomChoice(new Dictionary<int, int> { {1, 3}, {2, 4}, {3, 1} });
        for (var i = 0; i < amount * toOpen; i++)
        {
            var weapon = EquippableItemService.GetRandomWeapon(Level / 10 + 1);
            PlayerHandler.player.Inventory.AddItem(weapon);
        }
        PlayerHandler.player.Inventory.TryRemoveItem(this, toOpen);
        return false;
        ValidationResult Validator(int n) {
            if (n > inInventory) return ValidationResult.Error(locale.ChoseTooMany);
            return n < 0 ? ValidationResult.Error(locale.IntBelowZero) : ValidationResult.Success();
        }
    }
}