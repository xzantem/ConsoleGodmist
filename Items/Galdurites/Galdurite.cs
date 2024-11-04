using ConsoleGodmist.Enums;
using ConsoleGodmist.Utilities;

namespace ConsoleGodmist.Items;

public class Galdurite : BaseItem
{
    public new string Name => NameAliasHelper.GetName(Alias);
    public override int Weight => 1;
    public override int ID => 562;
    public override bool Stackable => false;
}