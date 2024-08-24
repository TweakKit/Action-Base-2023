using System.Collections.Generic;
using CodeStage.AntiCheat.ObscuredTypes;
using Firebase.Firestore;
using Runtime.Tracking;

namespace Runtime.Utilities.Converter
{
    public sealed class FirestoreObscuredDictionaryIntLongConverter : FirestoreConverter<Dictionary<int, ObscuredLong>>
    {
        public override object ToFirestore(Dictionary<int, ObscuredLong> value)
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (var entry in value)
            {
                result.Add(entry.Key.ToString(), (long)entry.Value);
            }

            return result;
        }

        public override Dictionary<int, ObscuredLong> FromFirestore(object value)
        {
            Dictionary<string, object> input = (Dictionary<string, object>)value;
            Dictionary<int, ObscuredLong> result = new Dictionary<int, ObscuredLong>();
            foreach (var entry in input)
            {
                int key = int.Parse(entry.Key);
                result.Add(key, input.ToLong(entry.Key));
            }

            return result;
        }
    }
}