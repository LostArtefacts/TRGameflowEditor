using System.IO;

namespace TRGE.Core
{
    public class TRDirectoryIOArgs
    {
        public DirectoryInfo OriginalDirectory { get; set; }
        public DirectoryInfo BackupDirectory { get; set; }
        public FileInfo ConfigFile { get; set; }
        public DirectoryInfo OutputDirectory { get; set; }
    }
}