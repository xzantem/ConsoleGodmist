using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text.Json;
using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Items.Armors;
using ConsoleGodmist.Items.Weapons;
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

    public static IEquipmentPart GetPart<T>(string alias)
    {
        if (typeof(T) == typeof(WeaponHead))
            return WeaponHeads.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(WeaponBinder))
            return WeaponBinders.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(WeaponHandle))
            return WeaponHandles.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(ArmorPlate))
            return ArmorPlates.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(ArmorBinder))
            return ArmorBinders.FirstOrDefault(x => x.Alias == alias)!;
        if (typeof(T) == typeof(ArmorBase))
            return ArmorBases.FirstOrDefault(x => x.Alias == alias)!;
        return null!;
    }
}