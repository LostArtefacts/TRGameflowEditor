using System.IO;

namespace TRGE.Core
{
    public class TRScriptIOArgs
    {
        public FileInfo OriginalFile { get; set; }
        public FileInfo BackupFile { get; set; }
        public FileInfo ConfigFile { get; set; }
        public DirectoryInfo OutputDirectory { get; set; }
    }
}