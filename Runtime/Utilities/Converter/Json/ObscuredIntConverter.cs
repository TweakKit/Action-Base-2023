using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;

namespace Runtime.Utilities.Converter
{
    public sealed class ObscuredIntConverter : JsonConverter<ObscuredInt>
    {
        public override void WriteJson(JsonWriter writer, ObscuredInt value, JsonSerializer serializer)
        {
            writer.WriteValue(value.GetDecrypted());
        }

        public override ObscuredInt ReadJson(JsonReader reader, Type objectType, ObscuredInt existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<int>(reader);
        }
    }
}