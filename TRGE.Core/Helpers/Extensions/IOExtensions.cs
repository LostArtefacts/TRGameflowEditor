using System.IO.Compression;
using System.Text;
using System.Text.RegularExpressions;

namespace TRGE.Core;

public static class IOExtensions
{
    /// <summary>
    /// Copies the content of this directory to the target directory.
    /// </summary>
    internal static void Copy(this DirectoryInfo directory, DirectoryInfo targetDirectory, bool overwrite = true, string[] extensions = null, Action<FileInfo> callback = null)
    {
        targetDirectory.CopyInto(directory.GetFilteredFiles(extensions), overwrite, callback);
    }

    internal static FileInfo[] GetFilteredFiles(this DirectoryInfo directory, string[] extensions = null)
    {
        FileInfo[] farr;
        if (extensions == null)
        {
            farr = directory.GetFiles();
        }
        else
        {
            List<FileInfo> files = new();
            foreach (string ext in extensions)
            {
                files.AddRange(directory.GetFiles(ext, SearchOption.TopDirectoryOnly));
            }
            farr = files.ToArray();
        }
        return farr;
    }

    internal static void CopyInto(this DirectoryInfo targetDirectory, FileInfo[] files, bool overwrite = false, Action<FileInfo> callback = null)
    {
        targetDirectory.Create();
        foreach (FileInfo fi in files)
        {
            FileInfo targetFile = new(Path.Combine(targetDirectory.FullName, fi.Name));
            if (overwrite || !targetFile.Exists)
            {
                File.Copy(fi.FullName, targetFile.FullName, true);
                targetFile.EnsureWritable();
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
                fi.EnsureWritable();
                fi.Delete();
            }
        }
    }

    internal static void ClearExcept(this DirectoryInfo directory, List<string> fileNames, string[] extensions)
    {
        List<string> compNames = new();
        fileNames.ForEach(e => compNames.Add(e.ToLower()));

        List<FileInfo> files = new();
        foreach (string ext in extensions)
        {
            files.AddRange(directory.GetFiles(ext, SearchOption.TopDirectoryOnly));
        }

        foreach (FileInfo fi in files)
        {
            if (!compNames.Contains(fi.Name.ToLower()))
            {
                fi.EnsureWritable();
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
            file.EnsureWritable();
            file.Delete();
        }
        foreach (DirectoryInfo dir in directory.EnumerateDirectories())
        {
            dir.Delete(true);
        }
    }

    public static void CopyFile(string originalFile, DirectoryInfo targetDirectory, bool overwrite = false)
    {
        string targetFile = Path.Combine(targetDirectory.FullName, Path.GetFileName(originalFile));
        CopyFile(originalFile, targetFile, overwrite);
    }

    public static void CopyFile(string originalFile, string targetFile, bool overwrite = false)
    {
        if (overwrite || !File.Exists(targetFile))
        {
            if (File.Exists(targetFile))
            {
                EnsureWritable(targetFile);
            }
            File.Copy(originalFile, targetFile, true);
            EnsureWritable(targetFile);
        }
    }

    internal static void EnsureWritable(this FileInfo file)
    {
        EnsureWritable(file.FullName);
    }

    internal static void EnsureWritable(string file)
    {
        FileAttributes attrs = File.GetAttributes(file);
        if (attrs.HasFlag(FileAttributes.ReadOnly))
        {
            File.SetAttributes(file, attrs & ~FileAttributes.ReadOnly);
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

        using FileStream fs = fileInfo.OpenRead();
        using GZipStream zs = new(fs, CompressionMode.Decompress);
        using MemoryStream ms = new();
        zs.CopyTo(ms);
        return encoding.GetString(ms.ToArray());
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
        using FileStream fs = fileInfo.Create();
        using GZipStream zs = new(fs, CompressionMode.Compress);
        zs.Write(data, 0, data.Length);
    }

    /// <summary>
    /// Reads a compressed file and returns its contents as a string.
    /// </summary>
    internal static byte[] ReadCompressedBinary(this FileInfo fileInfo)
    {
        using FileStream fs = fileInfo.OpenRead();
        using GZipStream zs = new(fs, CompressionMode.Decompress);
        using MemoryStream ms = new();
        zs.CopyTo(ms);
        return ms.ToArray();
    }

    /// <summary>
    /// Writes text to a compressed file.
    /// </summary>
    internal static void WriteCompressedBinary(this FileInfo fileInfo, byte[] data)
    {
        using FileStream fs = fileInfo.Create();
        using GZipStream zs = new(fs, CompressionMode.Compress);
        zs.Write(data, 0, data.Length);
    }

    private static readonly string[] _sizeSuffixes =
    {
        "bytes", "KB", "MB", "GB", "TB", "PB", "EB", "ZB", "YB"
    };

    public static string ToDescriptiveSize(this long length, int decimalPlaces = 1)
    {
        if (length < 0)
        {
            return "-" + (-length).ToDescriptiveSize(decimalPlaces);
        }

        int i = 0;
        decimal d = length;
        while (Math.Round(d, decimalPlaces) >= 1000)
        {
            d /= 1024;
            i++;
        }

        return string.Format("{0:n" + decimalPlaces + "} {1}", d, _sizeSuffixes[i]);
    }

    public static string ToSafeFileName(this string str)
    {
        return new Regex("[^a-zA-Z0-9_-]").Replace(str, string.Empty);
    }
}