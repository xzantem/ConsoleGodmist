using ConsoleGodmist.Items;
using ConsoleGodmist.Items.Armors;
using ConsoleGodmist.Items.Weapons;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleGodmist.Combat.Skills;

public class EquipmentPartConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IEquipmentPart).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var type = jsonObject["Type"]?.ToString();

        IEquipmentPart result = type switch
        {
            "WeaponHead" => new WeaponHead(),
            "WeaponBinder" => new WeaponBinder(),
            "WeaponHandle" => new WeaponHandle(),
            "ArmorPlate" => new ArmorPlate(),
            "ArmorBinder" => new ArmorBinder(),
            "ArmorBase" => new ArmorBase(),
            _ => throw new NotSupportedException($"Unknown type: {type}")
        };

        serializer.Populate(jsonObject.CreateReader(), result);
        return result;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var obj = JToken.FromObject(value);
        obj["Type"] = value.GetType().Name;
        obj.WriteTo(writer);
    }
}