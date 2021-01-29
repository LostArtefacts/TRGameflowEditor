using System.IO;
using TRGE.Core;

namespace TRGE.Coord
{
    public abstract class AbstractTRLevelEditor
    {
        protected readonly TRDirectoryIOArgs _io;
        
        public bool AllowSuccessiveEdits { get; set; }

        internal AbstractTRLevelEditor(TRDirectoryIOArgs io)
        {
            _io = io;
            AllowSuccessiveEdits = false;
        }

        internal abstract void ScriptedLevelModified(TRScriptedLevelEventArgs e);
        internal abstract void Save(AbstractTRScriptEditor scriptEditor);
        internal abstract void Restore();

        /// <summary>
        /// Depending on wheter AllowSuccessiveEdits is set this will either return the current
        /// file in the target directory or the file that was originally backed up.
        /// </summary>
        protected string GetReadLevelFilePath(string levelFileName)
        {
            return Path.Combine(AllowSuccessiveEdits ?_io.OutputDirectory.FullName : _io.BackupDirectory.FullName, levelFileName);
        }

        protected string GetWriteLevelFilePath(string levelFileName)
        {
            return Path.Combine(_io.OutputDirectory.FullName, levelFileName);
        }
    }
}