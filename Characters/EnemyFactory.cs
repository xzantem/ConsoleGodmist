using System.Text.Json;
using ConsoleGodmist.Combat.Modifiers;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;

namespace ConsoleGodmist.Characters;

public static class EnemyFactory
{
    public static List<EnemyCharacter> EnemiesList;
    
    public static EnemyCharacter CreateEnemy(string alias, int level)
    {
        var enemy = EnemiesList.FirstOrDefault(x => x.Alias == alias);
        return new EnemyCharacter(enemy, level);
    }
    public static EnemyCharacter CreateEnemy(DungeonType dungeonType, int level)
    {
        var enemy = UtilityMethods.RandomChoice(EnemiesList
            .Where(x => x.DefaultLocation == dungeonType)
            .ToDictionary(s => s, enemy => 1));
        return new EnemyCharacter(enemy, level);
    }

    public static void InitializeEnemies()
    {
        var path = "json/enemies.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            EnemiesList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EnemyCharacter>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
}