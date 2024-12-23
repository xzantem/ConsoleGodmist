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
    /*[DllImport("kernel32.dll", ExactSpelling = true)]
    private static extern IntPtr GetConsoleWindow();

    [DllImport("user32.dll")]
    private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

    private const int SW_MAXIMIZE = 3;*/

    static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;
        //IntPtr handle = GetConsoleWindow();
        //ShowWindow(handle, SW_MAXIMIZE);
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
            MainMenu.Menu();
            TownsHandler.Arungard.EnterTown();
            AnsiConsole.Clear();
        }
    }
}

    