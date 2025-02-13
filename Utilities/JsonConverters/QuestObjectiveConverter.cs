using ConsoleGodmist.Items;
using ConsoleGodmist.Quests;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Spectre.Console;

namespace ConsoleGodmist.Quests;

public class QuestObjectiveConverter : JsonConverter
{
    public override bool CanConvert(Type objectType)
    {
        return typeof(IQuestObjective).IsAssignableFrom(objectType);
    }

    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        var jsonObject = JObject.Load(reader);
        var type = jsonObject["Type"]?.ToString();
        

        IQuestObjective result = type switch
        {
            "KillQuestObjective" => new KillQuestObjective(),
            "KillInDungeonQuestObjective" => new KillInDungeonQuestObjective(),
            "TalkQuestObjective" => new TalkQuestObjective(),
            "DescendQuestObjective" => new DescendQuestObjective(),
            _ => throw new NotSupportedException($"Unknown type: {type}")
        };

        serializer.Populate(jsonObject.CreateReader(), result);
        return result;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        var jsonObject = new JObject
        {
            ["Type"] = value.GetType().Name
        };

        // Serialize the object's properties directly to the JObject.
        foreach (var property in value.GetType().GetProperties())
        {
            // Add each property to the JObject.
            if (property.CanRead)
            {
                var propertyValue = property.GetValue(value);
                jsonObject[property.Name] = propertyValue != null ? JToken.FromObject(propertyValue, serializer) : JValue.CreateNull();
            }
        }

        // Write the resulting JObject to the writer.
        jsonObject.WriteTo(writer);
    }
}