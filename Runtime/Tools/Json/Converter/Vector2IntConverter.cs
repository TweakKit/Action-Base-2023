using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Runtime.Tool.JsonConverter
{
    public sealed class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override void WriteJson(JsonWriter writer, Vector2Int value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.x);
            writer.WriteValue(value.y);
            writer.WriteEndArray();
        }

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue,
            bool hasExistingValue, JsonSerializer serializer)
        {
            var values = serializer.Deserialize<int[]>(reader);
            if (values == null)
            {
                return Vector2Int.zero;
            }

            return new Vector2Int(values[0], values[1]);
        }
    }
}