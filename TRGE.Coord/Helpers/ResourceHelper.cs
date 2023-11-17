using System.IO.Compression;

namespace TRGE.Coord
{
    internal static class ResourceHelper
    {
        internal static byte[] Decompress(byte[] compressedBytes)
        {
            using MemoryStream ms = new(compressedBytes);
            using GZipStream stream = new(ms, CompressionMode.Decompress);
            byte[] buffer = new byte[4096];
            using MemoryStream memory = new();
            int i;
            while ((i = stream.Read(buffer, 0, buffer.Length)) > 0)
            {
                memory.Write(buffer, 0, i);
            }
            return memory.ToArray();
        }
    }
}