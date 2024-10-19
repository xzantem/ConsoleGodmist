using System.Text.Json;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;

namespace ConsoleGodmist.Characters;

public static class EnemyFactory
{
    public static List<EnemyCharacter> EnemiesList;
    
    public static EnemyCharacter CreateEnemy(string alias, int level)
    {
        var enemy = EnemiesList.FirstOrDefault(x => x.Alias == alias);
        enemy.Level = level;
        return enemy;
    }

    public static void InitializeEnemies()
    {
        var path = "enemies.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            EnemiesList = Newtonsoft.Json.JsonConvert.DeserializeObject<List<EnemyCharacter>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
}