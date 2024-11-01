namespace ConsoleGodmist.Utilities;

public static class NameAliasHelper
{
    public static string GetName(string alias)
    {
        return (locale.ResourceManager.GetString(alias) == null ? alias : locale.ResourceManager.GetString(alias))!;
    }
}