using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Enums;
using Newtonsoft.Json;
using Spectre.Console;

namespace ConsoleGodmist.Items;

public static class EquipmentPartManager
{
    private static List<WeaponHead> WeaponHeads { get; set; } = null!;
    private static List<WeaponBinder> WeaponBinders { get; set; } = null!;
    private static List<WeaponHandle> WeaponHandles { get; set; } = null!;

    private static List<ArmorPlate> ArmorPlates { get; set; } = null!;
    private static List<ArmorBinder> ArmorBinders { get; set; } = null!;
    private static List<ArmorBase> ArmorBases { get; set; } = null!;

    public static void InitItems()
    {
        var path = "json/equipment-parts.json";
        if (File.Exists(path))
        {
            var json = File.ReadAllText(path);
            var equipmentParts = JsonConvert.DeserializeObject<List<IEquipmentPart>>(json, new EquipmentPartConverter());
            WeaponHeads = equipmentParts.Where(x => x.GetType() == typeof(WeaponHead)).Cast<WeaponHead>().ToList();
            WeaponBinders = equipmentParts.Where(x => x.GetType() == typeof(WeaponBinder)).Cast<WeaponBinder>()
                .ToList();
            WeaponHandles = equipmentParts.Where(x => x.GetType() == typeof(WeaponHandle)).Cast<WeaponHandle>()
                .ToList();

            ArmorPlates = equipmentParts.Where(x => x.GetType() == typeof(ArmorPlate)).Cast<ArmorPlate>().ToList();
            ArmorBinders = equipmentParts.Where(x => x.GetType() == typeof(ArmorBinder)).Cast<ArmorBinder>().ToList();
            ArmorBases = equipmentParts.Where(x => x.GetType() == typeof(ArmorBase)).Cast<ArmorBase>().ToList();
        }
        else
            throw new FileNotFoundException($"JSON file not found in {path}");
    }

    public static T GetPart<T>(string alias) where T : IEquipmentPart
    {
        if (typeof(T) == typeof(WeaponHead))
            return (T)(object)WeaponHeads.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(WeaponBinder))
            return (T)(object)WeaponBinders.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(WeaponHandle))
            return (T)(object)WeaponHandles.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(ArmorPlate))
            return (T)(object)ArmorPlates.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(ArmorBinder))
            return (T)(object)ArmorBinders.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(ArmorBase))
            return (T)(object)ArmorBases.FirstOrDefault(x => x.Alias == alias)!;
        throw new NotSupportedException();
    }

    public static T GetRandomPart<T>(int tier, CharacterClass intendedClass) where T : IEquipmentPart
    {
        if (typeof(T) == typeof(WeaponHead))
            return (T)(object)UtilityMethods.RandomChoice(WeaponHeads
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(WeaponBinder))
            return (T)(object)UtilityMethods.RandomChoice(WeaponBinders
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(WeaponHandle))
            return (T)(object)UtilityMethods.RandomChoice(WeaponHandles
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(ArmorPlate))
            return (T)(object)UtilityMethods.RandomChoice(ArmorPlates
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(ArmorBinder))
            return (T)(object)UtilityMethods.RandomChoice(ArmorBinders
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        if (typeof(T) == typeof(ArmorBase))
            return (T)(object)UtilityMethods.RandomChoice(ArmorBases
                .Where(x => x.Tier == tier && x.Material != "None" && x.IntendedClass == intendedClass).ToList());
        throw new NotSupportedException();
    }
}