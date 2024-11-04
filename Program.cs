using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using ConsoleGodmist;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Components;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Items;
using ConsoleGodmist.Items.Lootbags;
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
        
        while (true)
        {
            MainMenu.Menu();
            var arungard = new Town();
            arungard.EnterTown();
            AnsiConsole.Clear();
        }
    }
}

    