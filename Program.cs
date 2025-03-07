using System.Text;
using ConsoleGodmist;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Utilities;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using ConsoleGodmist.Quests;
using ConsoleGodmist.Town;
using Spectre.Console;

class Program
{

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        DisableConsoleQuickEdit.Go();
        SetFullScreenConsole.Go();
        Stylesheet.InitStyles();
        ItemManager.InitItems();
        LootbagManager.InitItems();
        EquipmentPartManager.InitItems();
        EnemyFactory.InitializeEnemies();
        PlantDropManager.InitPlantDrops();
        PotionManager.InitComponents();
        GalduriteManager.InitComponents();
        while (true)
        {
            try
            {
                AnsiConsole.Clear();
                MainMenu.Menu();
                TownsHandler.Arungard.EnterTown();
            }
            catch(Exception ex)
            {
                AnsiConsole.WriteException(ex);
                AnsiConsole.Write(new Text("An unexpected error occurred. Make sure to send error log to developer! Press any key to continue..."));
                Console.ReadLine();
            }
        }
    }
}

    