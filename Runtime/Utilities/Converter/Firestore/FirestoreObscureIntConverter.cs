using CodeStage.AntiCheat.ObscuredTypes;
using Firebase.Firestore;

namespace Runtime.Utilities.Converter
{
    public sealed class FirestoreObscureIntConverter : FirestoreConverter<ObscuredInt>
    {
        public override object ToFirestore(ObscuredInt value)
        {
            return (int)value;
        }

        public override ObscuredInt FromFirestore(object value)
        {
            return (int)(long)value;
        }
    }
}