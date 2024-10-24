using System.Text.Json;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using Spectre.Console;

namespace ConsoleGodmist.Dungeons;

public static class PlantDropManager
{
    public static Dictionary<DungeonType, DropPool> DropDatabase { get; private set; }

    public static void InitPlantDrops()
    {
        var path = "json/plant-drop-table.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            DropDatabase = JsonSerializer.Deserialize<Dictionary<DungeonType, DropPool>>(json);
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }
}