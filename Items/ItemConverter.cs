using ConsoleGodmist.Enums;
using ConsoleGodmist.Items;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

public class ItemConverter : JsonConverter<Dictionary<IItem, int>>
{
    public override void WriteJson(JsonWriter writer, Dictionary<IItem, int> value, JsonSerializer serializer)
    {
        writer.WriteStartObject();
        foreach (var kvp in value)
        {
            // Serialize the item type and its properties
            var itemType = kvp.Key.GetType().Name; // Get the type name for polymorphism
            writer.WritePropertyName(itemType);
            serializer.Serialize(writer, kvp.Key); // Serialize the item
            writer.WritePropertyName("Quantity");
            writer.WriteValue(kvp.Value); // Serialize the quantity
        }
        writer.WriteEndObject();
    }

    public override Dictionary<IItem, int> ReadJson(JsonReader reader, Type objectType, Dictionary<IItem, int> existingValue, bool hasExistingValue, JsonSerializer serializer)
    {
        var items = new Dictionary<IItem, int>();
        var jsonObject = JObject.Load(reader);

        foreach (var property in jsonObject.Properties())
        {
            // Get the item type from the property name
            var itemType = Type.GetType($"ConsoleGodmist.Items.{property.Name}");
            if (itemType == null) continue;
            // Deserialize the item
            var item = (IItem)property.Value.ToObject(itemType, serializer);
            // Get the quantity
            var quantity = (int)jsonObject["Quantity"];
            items.Add(item, quantity);
        }
        return items;
    }
}