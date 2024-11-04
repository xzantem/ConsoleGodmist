using ConsoleGodmist.Characters;

namespace ConsoleGodmist.Items.Lootbags;

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
        var amount = EngineMethods.RandomChoice(new Dictionary<int, int> { {1, 3}, {2, 4}, {3, 1} });
        for (var i = 0; i < amount; i++)
        {
            var weapon = EquippableItemService.GetRandomWeapon(Level / 10 + 1);
            PlayerHandler.player.Inventory.AddItem(weapon);
        }
        return true;
    }
}