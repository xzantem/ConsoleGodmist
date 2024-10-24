using ConsoleGodmist.Combat.Skills.ActiveSkillEffects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleGodmist.Combat.Skills;

public class ActiveSkillEffectConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IActiveSkillEffect).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var type = jsonObject["Type"]?.ToString();

        IActiveSkillEffect result = type switch
        {
            "DealDamage" => new DealDamage(),
            "InflictStatusEffect" => new InflictGenericStatusEffect(),
            "InflictDoTStatusEffect" => new InflictDoTStatusEffect(),
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