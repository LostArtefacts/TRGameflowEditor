using System.Text;
using TRGE.Core;

namespace TRGE.Coord;

public abstract class AbstractTRLevelEditor : AbstractTRGEEditor
{
    protected readonly TRDirectoryIOArgs _io;
    protected readonly TREdition _edition;
    protected readonly Dictionary<string, ISet<TRScriptedLevelEventArgs>> _levelModifications;

    internal event EventHandler RestoreProgressChanged;

    internal override string ConfigFilePath => _io.ConfigFile.FullName;

    protected AbstractTRScriptEditor _scriptEditor;

    public AbstractTRLevelEditor(TRDirectoryIOArgs io, TREdition edition)
    {
        _io = io;
        _edition = edition;
        _levelModifications = new Dictionary<string, ISet<TRScriptedLevelEventArgs>>();
        ReadConfig(_config = Config.Read(_io.ConfigFile.FullName));
    }

    protected sealed override void ReadConfig(Config config)
    {
        config ??= new Config();
        ApplyConfig(config);
    }

    internal sealed override Config ExportConfig()
    {
        Config config = base.ExportConfig();
        StoreConfig(config);
        return config;
    }

    /// <summary>
    /// The supplied dictionary has been loaded from disk from the previous edit, so values
    /// can be assigned locally as necessary.
    /// </summary>
    /// <param name="config">The configuration dictionary loaded from disk, or an empty config if no file currently exists.</param>
    protected override void ApplyConfig(Config config) { }

    /// <summary>
    /// Any custom values to be saved between edits should be added to the supplied dictionary.
    /// </summary>
    /// <param name="config">The current configuration dictionary.</param>
    protected virtual void StoreConfig(Config config) { }

    internal void Initialise(AbstractTRScriptEditor scriptEditor)
    {
        _scriptEditor = scriptEditor;
        _levelModifications.Clear();
        if (scriptEditor.GymAvailable)
        {
            _levelModifications.Add(scriptEditor.LevelManager.AssaultLevel.ID, new HashSet<TRScriptedLevelEventArgs>());
        }
        foreach (AbstractTRScriptedLevel level in scriptEditor.LevelManager.Levels)
        {
            _levelModifications.Add(level.ID, new HashSet<TRScriptedLevelEventArgs>());
        }
    }

    /// <summary>
    /// This is called before any scripting changes are saved and so it allows the level
    /// editor to scan any level files beforehand to check if anything in the script needs
    /// to be set. This allows changes to be sent to the script editor, whereas during the
    /// save task, the script is locked at that stage.
    /// </summary>
    internal virtual void PreSave(AbstractTRScriptEditor scriptEditor) { }

    internal void Save(AbstractTRScriptEditor scriptEditor, TRSaveMonitor monitor)
    {
        _config = new Config(_config)
        {
            ["App"] = new Config
            {
                ["Tag"] = TRInterop.TaggedVersion,
                ["Version"] = TRInterop.ExecutingVersion
            }
        };

        monitor.FireSaveStateChanged(0, TRSaveCategory.LevelFile);
        foreach (string levelID in _levelModifications.Keys)
        {
            foreach (TRScriptedLevelEventArgs mod in _levelModifications[levelID])
            {
                ProcessModification(mod);
            }

            if (_levelModifications[levelID].Count > 0)
            {
                monitor.FireSaveStateChanged(1);
            }

            if (monitor.IsCancelled)
            {
                return;
            }
        }

        SaveImpl(scriptEditor, monitor);

        StoreConfig(_config);
    }

    /// <summary>
    /// Called on a successful save transaction from TREditor, so it is safe to write the current config
    /// to disk at this stage.
    /// </summary>
    internal sealed override void SaveComplete()
    {
        _config.Write(_io.ConfigFile.FullName);
    }

    internal int GetRestoreTarget()
    {
        return GetRestoreFiles().Count;
    }

    internal sealed override void Restore()
    {
        Dictionary<string, string> restoreFiles = GetRestoreFiles();
        foreach (string backup in restoreFiles.Keys)
        {
            IOExtensions.CopyFile(backup, restoreFiles[backup], true);
            RestoreProgressChanged?.Invoke(this, EventArgs.Empty);
        }

        ApplyRestore();

        while (File.Exists(_io.ConfigFile.FullName))
        {
            _io.ConfigFile.Delete(); //issue #39
        }
        _io.ConfigFile = new FileInfo(_io.ConfigFile.FullName);
        ReadConfig(_config = Config.Read(_io.ConfigFile.FullName));
    }

    protected Dictionary<string, string> GetRestoreFiles()
    {
        Dictionary<string, string> files = new();
        foreach (AbstractTRScriptedLevel level in _scriptEditor.Levels)
        {
            string backup = Path.Combine(_io.BackupDirectory.FullName, level.LevelFileBaseName);
            string restore = Path.GetFullPath(Path.Combine(_io.OriginalDirectory.FullName, @"..\", level.LevelFile)); // Supports restoring to folders outside data
            files[backup] = restore;
            if (level.HasCutScene)
            {
                string cutBackup = Path.Combine(_io.BackupDirectory.FullName, level.CutSceneLevel.LevelFileBaseName);
                string cutRestore = Path.GetFullPath(Path.Combine(_io.OriginalDirectory.FullName, @"..\", level.CutSceneLevel.LevelFile));
                files[cutBackup] = cutRestore;
            }
        }

        foreach (string additionalFile in _scriptEditor.Script.GetAdditionalBackupFiles())
        {
            string backup = Path.Combine(_io.BackupDirectory.FullName, Path.GetFileName(additionalFile));
            if (File.Exists(backup))
            {
                string restore = Path.GetFullPath(Path.Combine(_io.OriginalDirectory.FullName, @"..\", additionalFile));
                files[backup] = restore;
            }
        }

        return files;
    }

    protected virtual void ApplyRestore() { }

    internal void ScriptedLevelModified(TRScriptedLevelEventArgs e)
    {
        if (e.ScriptedLevel.Enabled && ShouldHandleModification(e))
        {
            if (!_levelModifications.ContainsKey(e.LevelID))
            {
                _levelModifications[e.LevelID] = new HashSet<TRScriptedLevelEventArgs>();
            }
            _levelModifications[e.LevelID].Add(e);
        }
    }

    public sealed override int GetSaveTargetCount()
    {
        int levelCount = _scriptEditor.EnabledScriptedLevels.Count;
        if (_scriptEditor.GymAvailable)
        {
            levelCount++;
        }
        return levelCount + GetSaveTarget(levelCount);
        //return _levelModifications.Count + GetSaveTarget(_levelModifications.Count);
    }

    /// <summary>
    /// Called when initialising a save. The returned value should represent the
    /// number of steps that will be involved in the save progress for this class.
    /// </summary>
    /// <param name="numLevels">A count of the total number of enabled levels for the current edit.</param>
    /// <returns>The number of save steps for this class.</returns>
    protected virtual int GetSaveTarget(int numLevels)
    {
        return 0;
    }

    internal abstract bool ShouldHandleModification(TRScriptedLevelEventArgs e);
    internal abstract void ProcessModification(TRScriptedLevelEventArgs e);

    /// <summary>
    /// Called after any modifications have been actioned as received from AbstractTRScriptEditor edit events.
    /// </summary>
    /// <param name="scriptEditor">A reference to the current script editor.</param>
    /// <param name="monitor">The save monitor for publishing save progress.</param>
    protected virtual void SaveImpl(AbstractTRScriptEditor scriptEditor, TRSaveMonitor monitor) { }
    
    /// <summary>
    /// All reads are done on the original backed up files in the backup directory unless there is an existing
    /// file in the WIP directory.
    /// </summary>
    protected string GetReadBasePath()
    {
        return _io.BackupDirectory.FullName;
    }

    protected virtual string GetReadLevelFilePath(string levelFileName)
    {
        string wipFile = GetWriteLevelFilePath(levelFileName);
        if (File.Exists(wipFile))
        {
            return wipFile;
        }
        return Path.Combine(GetReadBasePath(), levelFileName);
    }

    /// <summary>
    /// All writes are sent to the temporary WIP directory and are only moved to the output
    /// and target directories at the end of the save chain. See TREditor.
    /// </summary>
    protected string GetWriteBasePath()
    {
        return _io.WIPOutputDirectory.FullName;
    }

    protected virtual string GetWriteLevelFilePath(string levelFileName)
    {
        return Path.Combine(GetWriteBasePath(), levelFileName);
    }

    /// <summary>
    /// This performs a check that each level defined in the script file is available as a level
    /// file in the specified directory. So, if a folder contains a TR2 script file but TR3 level
    /// files, then an exception is thrown. Equally, if the folder contains only a subset of the
    /// expected level files, an exception is thrown.
    /// </summary>
    internal static void ValidateCompatibility(List<AbstractTRScriptedLevel> levels, string folderPath)
    {
        List<AbstractTRScriptedLevel> faults = new();
        foreach (AbstractTRScriptedLevel level in levels)
        {
            if (!File.Exists(Path.Combine(folderPath, level.LevelFileBaseName)))
            {
                faults.Add(level);
            }
        }

        if (faults.Count > 0)
        {
            StringBuilder sb = new StringBuilder("The following level files were not found in ").Append(folderPath).Append('.').AppendLine();
            foreach (AbstractTRScriptedLevel level in faults)
            {
                sb.AppendLine().Append(level.Name).Append(" (").Append(level.LevelFileBaseName).Append(')');
            }
            throw new ScriptedLevelMismatchException(sb.ToString());
        }
    }

    protected string GetResource(string filePath)
    {
        return Path.Combine("Resources", _edition.Version.ToString(), filePath);
    }

    protected string ReadResource(string filePath)
    {
        return File.ReadAllText(GetResource(filePath));
    }
}