using ConsoleGodmist.Combat.Skills;
using ConsoleGodmist.Combat.Skills.ActiveSkillEffects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace ConsoleGodmist.Characters;

public class PlayerJsonConverter : JsonConverter
{
        public override bool CanConvert(Type objectType)
        {
            return typeof(PlayerCharacter).IsAssignableFrom(objectType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jsonObject = JObject.Load(reader);
            Console.WriteLine("JSON Object: " + jsonObject.ToString()); // Log JSON content

            var characterClass = jsonObject["CharacterClass"]?.ToObject<int>(); // Read as integer
            Console.WriteLine($"Character Class: {characterClass}"); // Log class number

            PlayerCharacter result = characterClass switch
            {
                0 => new Warrior(),
                // Add cases for other classes here
                1 => throw new NotSupportedException($"Unknown class: {characterClass}"),
                2 => throw new NotSupportedException($"Unknown class: {characterClass}"),
                3 => throw new NotSupportedException($"Unknown class: {characterClass}"),
                _ => throw new NotSupportedException($"Unknown class: {characterClass}")
            };

            // Populate the resulting object with the JSON properties.
            serializer.Populate(jsonObject.CreateReader(), result);

            // Log the populated properties to verify
            if (result is Warrior warrior)
            {
                Console.WriteLine($"Populated Warrior - Health: {warrior.MaximalHealth}, Strength: {warrior.MaximalAttack}, Level: {warrior.Level}");
            }

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
                if (property.CanRead)
                {
                    var propertyValue = property.GetValue(value);
                    jsonObject[property.Name] = propertyValue != null ? JToken.FromObject(propertyValue, serializer) : JValue.CreateNull();
                }
            }

            jsonObject.WriteTo(writer);
        }
    }