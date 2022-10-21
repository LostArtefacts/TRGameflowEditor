using System.IO;

namespace TRGE.Core
{
    public class TRScriptIOArgs
    {
        public FileInfo TRScriptFile { get; set; }
        public FileInfo TRScriptBackupFile { get; set; }
        public FileInfo TRConfigFile { get; set; }
        public FileInfo TRConfigBackupFile { get; set; }
        public FileInfo InternalConfigFile { get; set; }
        public DirectoryInfo WIPOutputDirectory { get; set; }
        public DirectoryInfo OutputDirectory { get; set; }
        public DirectoryInfo OriginalDirectory { get; set; }
        public DirectoryInfo BackupDirectory { get; set; }
    }
}