using ConsoleGodmist;
using ConsoleGodmist.Components;
using ConsoleGodmist.Items;
using ConsoleGodmist.Town;
using Spectre.Console;

Stylesheet.InitStyles();
ItemManager.InitItems();
while (true)
{
    MainMenu.Menu();
    Town.EnterTown();
    AnsiConsole.Clear();
}
    