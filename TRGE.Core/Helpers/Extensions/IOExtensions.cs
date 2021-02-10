﻿using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Text;

namespace TRGE.Core
{
    internal static class IOExtensions
    {
        /// <summary>
        /// Copies the content of this directory to the target directory.
        /// </summary>
        internal static void Copy(this DirectoryInfo directory, DirectoryInfo targetDirectory, bool overwrite = true, string[] extensions = null, Action<FileInfo> callback = null)
        {
            FileInfo[] farr;
            if (extensions == null)
            {
                farr = directory.GetFiles();
            }
            else
            {
                List<FileInfo> files = new List<FileInfo>();
                foreach (string ext in extensions)
                {
                    files.AddRange(directory.GetFiles(ext, SearchOption.TopDirectoryOnly));
                }
                farr = files.ToArray();
            }

            targetDirectory.Create();
            foreach (FileInfo fi in farr)
            {
                FileInfo targetFile = new FileInfo(Path.Combine(targetDirectory.FullName, fi.Name));
                if (overwrite || !targetFile.Exists)
                {
                    File.Copy(fi.FullName, targetFile.FullName, true);
                }
                callback?.Invoke(fi);
            }
        }

        internal static void Copy(this DirectoryInfo directory, string targetDirectory, bool overwrite = true, string[] extensions = null, Action<FileInfo> callback = null)
        {
            Copy(directory, new DirectoryInfo(targetDirectory), overwrite, extensions, callback);
        }

        /// <summary>
        /// Remove all files in the directory except for the one with the second arg's name.
        /// </summary>
        internal static void ClearExcept(this DirectoryInfo directory, FileInfo file)
        {
            foreach (FileInfo fi in directory.GetFiles("*" + file.Extension))
            {
                if (!fi.Name.ToLower().Equals(file.Name.ToLower()))
                {
                    fi.Delete();
                }
            }
        }

        internal static void ClearExcept(this DirectoryInfo directory, string filepath)
        {
            ClearExcept(directory, new FileInfo(filepath));
        }

        /// <summary>
        /// Empties a directory entirely, but does not delete it.
        /// </summary>
        internal static void Clear(this DirectoryInfo directory)
        {
            foreach (FileInfo file in directory.EnumerateFiles())
            {
                file.Delete();
            }
            foreach (DirectoryInfo dir in directory.EnumerateDirectories())
            {
                dir.Delete(true);
            }
        }

        /// <summary>
        /// Reads a compressed file and returns its contents as a string.
        /// </summary>
        internal static string ReadCompressedText(this FileInfo fileInfo, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

            using (FileStream fs = fileInfo.OpenRead())
            using (GZipStream zs = new GZipStream(fs, CompressionMode.Decompress))
            using (MemoryStream ms = new MemoryStream())
            {
                zs.CopyTo(ms);
                return encoding.GetString(ms.ToArray());
            }
        }

        /// <summary>
        /// Writes text to a compressed file.
        /// </summary>
        internal static void WriteCompressedText(this FileInfo fileInfo, string text, Encoding encoding = null)
        {
            if (encoding == null)
            {
                encoding = Encoding.Default;
            }

            byte[] data = encoding.GetBytes(text);
            using (FileStream fs = fileInfo.OpenWrite())
            using (GZipStream zs = new GZipStream(fs, CompressionMode.Compress))
            {
                zs.Write(data, 0, data.Length);
            }
        }
    }
}