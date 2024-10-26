using System.Globalization;
using System.Text;
using ConsoleGodmist;
using ConsoleGodmist.Characters;
using ConsoleGodmist.Components;
using ConsoleGodmist.Dungeons;
using ConsoleGodmist.Items;
using ConsoleGodmist.Town;
using Spectre.Console;

Console.OutputEncoding = Encoding.UTF8;
Stylesheet.InitStyles();
ItemManager.InitItems();
EquipmentPartManager.InitItems();
EnemyFactory.InitializeEnemies();
PlantDropManager.InitPlantDrops();

while (true)
{
    MainMenu.Menu();
    Town.EnterTown();
    AnsiConsole.Clear();
}
    