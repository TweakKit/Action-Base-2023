using CodeStage.AntiCheat.ObscuredTypes;
using Firebase.Firestore;

namespace Runtime.Utilities.Converter
{
    public sealed class FirestoreObscuredLongConverter : FirestoreConverter<ObscuredLong>
    {
        public override object ToFirestore(ObscuredLong value)
        {
            return (long)value;
        }

        public override ObscuredLong FromFirestore(object value)
        {
            return (long)value;
        }
    }

}