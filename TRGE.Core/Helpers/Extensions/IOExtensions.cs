using System.Collections.Generic;
using System.IO;

namespace TRGE.Core
{
    internal static class IOExtensions
    {
        internal static void Copy(this DirectoryInfo directory, DirectoryInfo targetDirectory, bool overwrite = true, string[] extensions = null)
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
                    File.Copy(fi.FullName, targetFile.FullName);
                }
            }
        }

        internal static void Copy(this DirectoryInfo directory, string targetDirectory, bool overwrite = true, string[] extensions = null)
        {
            Copy(directory, new DirectoryInfo(targetDirectory), overwrite, extensions);
        }

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
    }
}