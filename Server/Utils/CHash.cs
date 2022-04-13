using System.Security.Cryptography;
using System.Text;

namespace AsbtCore.UtilsV2
{
    public static class CHash
    {
        public static string EncryptMD5(string Text)
        {
            byte[] hash = MD5.Create().ComputeHash(Encoding.Default.GetBytes(Text));
            StringBuilder stringBuilder = new StringBuilder();
            for (int index = 0; index < hash.Length; ++index)
                stringBuilder.Append(hash[index].ToString("x2"));
            return stringBuilder.ToString();
        }

        public class HashSha256
        {
            private static string Salt = "l23sdhfbiu aswiruhwi4h39@#$%284h 234 u234289  fk sj skdfdhfsldhf";

            public static string Get(string str)
            {
                var crypt = new SHA256Managed();
                var hash = new StringBuilder();

                str += Salt;
                var crypto = crypt.ComputeHash(Encoding.UTF8.GetBytes(str), 0, Encoding.UTF8.GetByteCount(str));

                foreach (byte theByte in crypto)
                {
                    hash.Append(theByte.ToString("x2"));
                }
                return hash.ToString();
            }
        }
    }
}
