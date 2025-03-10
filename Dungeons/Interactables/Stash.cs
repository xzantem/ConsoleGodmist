﻿using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;

namespace ConsoleGodmist.Dungeons;

public class Stash(int dungeonLevel, DungeonType dungeonType)
{
    public int DungeonLevel { get;} = dungeonLevel;
    public DungeonType DungeonType { get; } = dungeonType;
    public Dictionary<IItem, int> GetDrops()
    {
        var drops = new Dictionary<IItem, int>();
        // Add drops from supply lootbag pool
        var lootDrop = LootbagManager.GetSupplyBag(DungeonType, DungeonLevel);
        foreach (var item in lootDrop.DropTable.GetDrops(DungeonLevel))
        {
            if (drops.ContainsKey(item.Key))
                drops[item.Key] += item.Value;
            else
                drops.Add(item.Key, item.Value);
        }
        //Add drops from equipment lootbag pool
        if (Random.Shared.Next(0, 2) == 0)
        {
            drops.Add(EquippableItemService.GetRandomWeapon(DungeonLevel / 10 + 1), 1);
        }
        else
        {
            drops.Add(EquippableItemService.GetRandomArmor(DungeonLevel / 10 + 1), 1);
        }
        //Add drops from galdurite lootbag pool
        for (var i = 0; i < UtilityMethods.RandomChoice(new Dictionary<int, int> { { 1, 4 }, { 2, 2 }, { 3, 1 } }); i++)
        {
            var galduriteDrop = LootbagManager.GetLootbag("GalduriteBag", DungeonLevel);
            foreach (var item in galduriteDrop.DropTable.GetDrops(DungeonLevel))
            {
                if (drops.ContainsKey(item.Key))
                    drops[item.Key] += item.Value;
                else
                    drops.Add(item.Key, item.Value);
            }
        }
        return drops;
    }
}