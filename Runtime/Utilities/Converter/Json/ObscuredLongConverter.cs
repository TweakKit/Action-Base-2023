using System;
using CodeStage.AntiCheat.ObscuredTypes;
using Newtonsoft.Json;

namespace Runtime.Utilities.Converter
{
    public sealed class ObscuredLongConverter : JsonConverter<ObscuredLong>
    {
        public override void WriteJson(JsonWriter writer, ObscuredLong value, JsonSerializer serializer)
        {
            writer.WriteValue(value.GetDecrypted());
        }

        public override ObscuredLong ReadJson(JsonReader reader, Type objectType, ObscuredLong existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            return serializer.Deserialize<long>(reader);
        }
    }
}
