using ConsoleGodmist.Enums;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public class NPCInventory
{
    [JsonConverter(typeof(ItemConverter))]
    public Dictionary<IItem, int> RotatingShop { get; set; }
    [JsonConverter(typeof(ItemConverter))]
    public Dictionary<IItem, int> BoughtFromPlayer { get; set; }
    public List<ItemType> PossibleWares { get; set; }

    public NPCInventory(List<ItemType> itemTypesInShop)
    {
        RotatingShop = new Dictionary<IItem, int>();
        BoughtFromPlayer = new Dictionary<IItem, int>();
        PossibleWares = itemTypesInShop;
        UpdateWares(1);
    }
    public NPCInventory() {}

    public void UpdateWares(int loyaltyLevel)
    {
        RotatingShop.Clear();
        var tier = loyaltyLevel switch
        {
            <= 1 => 1,
            > 1 and <= 5 => 2,
            > 5 and <= 8 => 3,
            > 8 and <= 10 => 4,
            > 10 and <= 13 => 5
        };
        foreach (var type in PossibleWares)
        {
            var initial = RotatingShop.Count;
            switch (type)
            {
                case ItemType.Weapon:
                    while (RotatingShop.Count - initial < 5)
                        RotatingShop.TryAdd(EquippableItemService.GetRandomWeapon(tier), 1);
                    break;
                case ItemType.Armor:
                    while (RotatingShop.Count - initial < 5)
                        RotatingShop.TryAdd(EquippableItemService.GetRandomArmor(tier), 1);
                    break;
                case ItemType.Smithing:
                case ItemType.Alchemy:
                case ItemType.Runeforging:
                    var items = ItemManager.Items
                        .Where(x => x.ItemType == type &&
                                    (int)x.Rarity == tier + 2 && !RotatingShop.ContainsKey(x)).ToList(); 
                    while (RotatingShop.Count - initial < 15 && RotatingShop.Count - initial < items.Count)
                        RotatingShop.TryAdd(UtilityMethods.RandomChoice(items), Random.Shared.Next(10, 21));
                    break;
                case ItemType.Potion:
                    while (RotatingShop.Count - initial < 5)
                        RotatingShop.TryAdd(PotionManager.GetRandomPotion(tier), 1);
                    break;
                case ItemType.WeaponGaldurite:
                    // Generate 5 random weapon galdurites adjusted to level
                    break;
                case ItemType.ArmorGaldurite:
                    // Generate 5 random armor galdurites adjusted to level
                    break;
                case ItemType.LootBag:
                    throw new ArgumentOutOfRangeException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
    }

    public void AddItem(IItem item, int quantity = 1)
    {
        if (item.Stackable && BoughtFromPlayer.ContainsKey(item))
        {
            BoughtFromPlayer[item] += quantity;
        }
        else
        {
            BoughtFromPlayer.Add(item, quantity);
        }
    }
    public void RemoveAt(int index, int amount = 1)
    {
        var items = RotatingShop.Concat(BoughtFromPlayer).ToList();
        if (index <= RotatingShop.Count - 1)
        {
            RotatingShop[RotatingShop.ElementAt(index).Key] -= amount;
            if (RotatingShop[RotatingShop.ElementAt(index).Key] <= 0)
                RotatingShop.Remove(RotatingShop.ElementAt(index).Key);
        }
        else
        {
            BoughtFromPlayer[BoughtFromPlayer.ElementAt(index - RotatingShop.Count + 1).Key] -= amount;
            if (BoughtFromPlayer[BoughtFromPlayer.ElementAt(index - RotatingShop.Count + 1).Key] <= 0)
                BoughtFromPlayer.Remove(BoughtFromPlayer.ElementAt(index - RotatingShop.Count + 1).Key);
        }
    }

    public KeyValuePair<IItem, int> ElementAt(int index)
    {
        var items = RotatingShop.Concat(BoughtFromPlayer).ToList();
        return index <= RotatingShop.Count + 1 ? RotatingShop.ElementAt(index) : 
            BoughtFromPlayer.ElementAt(index - RotatingShop.Count);
    }
}