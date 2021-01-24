using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace TRGE.Core
{
    public static class HashingExtensions
    {
        public static string CreateMD5(this string str, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

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

        public static string Checksum(this FileInfo file)
        {
            using (MD5 md5 = MD5.Create())
            using (FileStream stream = File.OpenRead(file.FullName))
            {
                byte[] hash = md5.ComputeHash(stream);
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