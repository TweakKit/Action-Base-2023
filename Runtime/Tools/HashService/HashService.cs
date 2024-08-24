using System.Security.Cryptography;
using System.Text;

namespace Runtime.Tool.Hash
{
    public class HashService
    {
        public const string STRING_FORMAT = "x2";

        public string Hash(string input)
        {
            return Hash(HashType.SHA_256, input);
        }

        public string Hash(HashType type, string input)
        {
            using HashAlgorithm hashAlgorithm = CreateHashAlgorithm(type);
            byte[] array = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < array.Length; i++)
            {
                stringBuilder.Append(array[i].ToString("x2"));
            }

            return stringBuilder.ToString();
        }

        public HashAlgorithm CreateHashAlgorithm(HashType type)
        {
            return type switch {
                HashType.MD5 => MD5.Create(),
                HashType.SHA_256 => SHA256.Create(),
                HashType.SHA_512 => SHA512.Create(),
                _ => null,
            };
        }
    }

    public enum HashType
    {
        MD5,
        SHA_256,
        SHA_512
    }
}