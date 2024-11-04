using ConsoleGodmist.Characters;

namespace ConsoleGodmist.Items.Lootbags;

public class ArmorLootbag : Lootbag
{
    public override int ID => 572;
    public override string Alias => "ArmorLootbag";
    public ArmorLootbag(int level)
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