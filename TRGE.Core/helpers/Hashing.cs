using System.Security.Cryptography;
using System.Text;

namespace TRGE.Core
{
    internal static class Hashing
    {
        internal static string CreateMD5(string str, Encoding encoding)
        {
            using (MD5 md5 = MD5.Create())
            {
                byte[] hash = md5.ComputeHash(encoding.GetBytes(str));
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hash.Length; i++)
                {
                    sb.Append(hash[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
    }
}