﻿using Newtonsoft.Json;
using System.Runtime.ExceptionServices;
using TRGE.Core;

namespace TRGE.Coord;

public class TREditor
{
    public static readonly string[] TargetFileExtensions = new string[] { "*.dat", "*.tr2", "*.psx", "*.phd", "*.json5", "*.png" };

    private AbstractTRScriptEditor _scriptEditor;
    public AbstractTRScriptEditor ScriptEditor
    {
        get => _scriptEditor;
        internal set
        {
            if ((_scriptEditor = value) != null)
            {
                _scriptEditor.LevelModified += ScriptEditorLevelModified;
                ConfigureWatcher();
            }
        }
    }

    private AbstractTRLevelEditor _levelEditor;
    public AbstractTRLevelEditor LevelEditor
    {
        get => _levelEditor;
        internal set
        {
            if ((_levelEditor = value) != null)
            {
                _levelEditor.Initialise(ScriptEditor);
                ConfigureWatcher();
            }
        }
    }

    public TREdition Edition => _scriptEditor.Edition;
    public string BackupDirectory => _scriptEditor.BackupDirectory.FullName;
    public string ErrorDirectory => Path.GetFullPath(Path.Combine(BackupDirectory, "../Errors"));
    public string OutputDirectory => _outputDirectory;
    public string TargetDirectory => _targetDirectory;
    private readonly string _wipOutputDirectory;
    private readonly string _outputDirectory;
    private readonly string _targetDirectory;

    public event EventHandler<TRSaveEventArgs> SaveProgressChanged;

    private ConfigFileWatcher _watcher;
    public event EventHandler<FileSystemEventArgs> ConfigExternallyChanged;

    public event EventHandler<TRBackupRestoreEventArgs> RestoreProgressChanged;
    private TRBackupRestoreEventArgs _restoreArgs;

    public bool IsExportPossible => ScriptEditor.IsExportPossible && (LevelEditor == null || LevelEditor.IsExportPossible);

    internal TREditor(string wipOutputDirectory, string outputDirectory, string targetDirectory)
    {
        _wipOutputDirectory = wipOutputDirectory;
        _outputDirectory = outputDirectory;
        _targetDirectory = targetDirectory;
    }

    internal static void ValidateCompatibility(AbstractTRScript script, string directoryPath)
    {
        if (TRLevelEditorFactory.EditionSupportsLevelEditing(script.Edition))
        {
            AbstractTRLevelEditor.ValidateCompatibility(script.Levels, directoryPath);
        }
    }

    private void ScriptEditorLevelModified(object sender, TRScriptedLevelEventArgs e)
    {
        LevelEditor?.ScriptedLevelModified(e);
    }

    private void Editor_SaveStateChanged(object sender, TRSaveEventArgs e)
    {
        FireSaveProgressChanged(e);
    }

    private void FireSaveProgressChanged(TRSaveEventArgs e)
    {
        SaveProgressChanged?.Invoke(this, e);
    }

    /// <summary>
    /// The ScriptEditor and LevelEditor will be directed to save all output to the temporary
    /// WIP directory. Provided no errors occur, or the save transaction is not cancelled, the
    /// contents of the WIP directory will be moved to the Output and Target directories. Both
    /// editors will then be re-initialised. Any subscribers to SaveProgressChanged can monitor
    /// save progress and optionally cancel the task before the Commit stage is reached.
    /// </summary>
    public void Save()
    {
        DirectoryInfo wipDirectory = new(_wipOutputDirectory);
        wipDirectory.Create();
        wipDirectory.Clear();

        _watcher.Enabled = false;

        try
        {
            LevelEditor?.PreSave(ScriptEditor);

            ScriptEditor.Save();

            TRSaveMonitor monitor = new(new TRSaveEventArgs
            {
                ProgressTarget = ScriptEditor.GetSaveTargetCount() +
                (LevelEditor == null ? 0 : LevelEditor.GetSaveTargetCount())
            });
            monitor.SaveStateChanged += Editor_SaveStateChanged;
            monitor.FireSaveStateBeginning(TRSaveCategory.Scripting);
            monitor.FireSaveStateChanged(1);

            LevelEditor?.Save(ScriptEditor, monitor);

            if (!monitor.IsCancelled)
            {
                monitor.FireSaveStateChanged(0, TRSaveCategory.Commit);

                ScriptEditor.SaveComplete();
                LevelEditor?.SaveComplete();

                // Copy everything from WIP into the Output folder.
                DirectoryInfo outputDirectory = new(_outputDirectory);
                wipDirectory.Copy(outputDirectory, true);

                // Finally, copy everything to the target folder.
                CopyOutputToTarget();
            }
        }
        catch (Exception e)
        {
            LogException(e);
            ExceptionDispatchInfo.Capture(e).Throw();
        }
        finally
        {
            // Reinitialise regardless of whether the process completed or not
            ScriptEditor.Initialise();
            LevelEditor?.Initialise(ScriptEditor);

            _watcher.Enabled = true;
            wipDirectory.Clear();
        }
    }

    private void LogException(Exception e)
    {
        Config config = new()
        {
            ["Trace"] = e.ToString(),
            ["TRGE"] = ScriptEditor.ExportConfig()
        };
        if (LevelEditor != null)
        {
            config["TRLE"] = LevelEditor.ExportConfig();
        }

        Dictionary<string, string> checksums = new();
        foreach (FileInfo fi in new DirectoryInfo(BackupDirectory).GetFiles())
        {
            checksums[fi.Name] = fi.Checksum();
        }
        config["BackupChecksums"] = checksums;

        Directory.CreateDirectory(ErrorDirectory);
        config.Write(Path.Combine(ErrorDirectory, DateTime.Now.ToString("yyyyMMdd_HHmmss") + ".err"), true, Formatting.Indented);
    }

    private void CopyOutputToTarget()
    {
        if (ScriptEditor.Edition.HasScript)
        {
            string targetScriptFolder = Path.GetFullPath(Path.GetDirectoryName(Path.Combine(_targetDirectory, ScriptEditor.Edition.ScriptName)));
            IOExtensions.CopyFile(ScriptEditor.GetScriptOutputPath(), new DirectoryInfo(targetScriptFolder), true);
            if (ScriptEditor.Edition.HasGold)
            {
                if (ScriptEditor.GameMode == GameMode.Normal)
                {
                    IOExtensions.CopyFile(ScriptEditor.GoldEditor.GetScriptOutputPath(), new DirectoryInfo(targetScriptFolder), true);
                }
                else
                {
                    string targetFile = Path.Combine(targetScriptFolder, Path.GetFileName(ScriptEditor.GoldEditor.GetScriptOutputPath()));
                    IOExtensions.CopyFile(ScriptEditor.GetScriptOutputPath(), targetFile, true);
                }
            }
        }

        if (ScriptEditor.Edition.HasConfig)
        {
            string targetScriptFolder = Path.GetFullPath(Path.GetDirectoryName(Path.Combine(_targetDirectory, ScriptEditor.Edition.ConfigName)));
            IOExtensions.CopyFile(ScriptEditor.GetConfigOutputPath(), new DirectoryInfo(targetScriptFolder), true);
        }

        List<AbstractTRScriptedLevel> levels = new(ScriptEditor.Levels);

        if (_scriptEditor.Edition.Remastered && _scriptEditor.LevelSequencingOrganisation != Organisation.Default)
        {
            // No actual sequencing defined for the game, so rewrite the output levels
            List<AbstractTRScriptedLevel> originalLevels = ScriptEditor.GetOriginalLevels();

            foreach (AbstractTRScriptedLevel level in levels)
            {
                TRRScriptedLevel trrLevel = level as TRRScriptedLevel;
                RenameOutputFile(Path.Combine(_outputDirectory, trrLevel.LevelFileBaseName), Path.Combine(_outputDirectory, trrLevel.Sequence + Path.GetExtension(trrLevel.LevelFileBaseName)));
                RenameOutputFile(Path.Combine(_outputDirectory, trrLevel.MapFileBaseName), Path.Combine(_outputDirectory, level.Sequence + Path.GetExtension(trrLevel.MapFile)));
                RenameOutputFile(Path.Combine(_outputDirectory, trrLevel.PdpFileBaseName), Path.Combine(_outputDirectory, level.Sequence + Path.GetExtension(trrLevel.PdpFile)));
                RenameOutputFile(Path.Combine(_outputDirectory, trrLevel.TexFileBaseName), Path.Combine(_outputDirectory, level.Sequence + Path.GetExtension(trrLevel.TexFile)));
                RenameOutputFile(Path.Combine(_outputDirectory, trrLevel.TrgFileBaseName), Path.Combine(_outputDirectory, level.Sequence + Path.GetExtension(trrLevel.TrgFileBaseName)));
            }

            foreach (AbstractTRScriptedLevel level in levels)
            {
                TRRScriptedLevel trrLevel = level as TRRScriptedLevel;
                TRRScriptedLevel targetSequenceLevel = originalLevels.Find(l => l.Sequence == trrLevel.Sequence) as TRRScriptedLevel;
                string outputLevel = Path.Combine(_outputDirectory, trrLevel.Sequence + Path.GetExtension(trrLevel.LevelFileBaseName));
                RenameOutputFile(outputLevel, Path.Combine(_outputDirectory, targetSequenceLevel.LevelFileBaseName));
                RenameOutputFile(Path.ChangeExtension(outputLevel, Path.GetExtension(trrLevel.MapFile)), Path.Combine(_outputDirectory, targetSequenceLevel.MapFileBaseName));
                RenameOutputFile(Path.ChangeExtension(outputLevel, Path.GetExtension(trrLevel.PdpFile)), Path.Combine(_outputDirectory, targetSequenceLevel.PdpFileBaseName));
                RenameOutputFile(Path.ChangeExtension(outputLevel, Path.GetExtension(trrLevel.TexFile)), Path.Combine(_outputDirectory, targetSequenceLevel.TexFileBaseName));
                RenameOutputFile(Path.ChangeExtension(outputLevel, Path.GetExtension(trrLevel.TrgFile)), Path.Combine(_outputDirectory, targetSequenceLevel.TrgFileBaseName));
            }
        }

        if (ScriptEditor.Edition.AssaultCourseSupported)
        {
            levels.Add(ScriptEditor.AssaultLevel);
        }

        foreach (AbstractTRScriptedLevel level in levels)
        {
            // Check if it's been generated in the output folder
            // If so, move it to its target directory
            CopyLevelToTarget(level);
            if (level.HasCutScene)
            {
                CopyLevelToTarget(level.CutSceneLevel);
            }
        }

        List<string> additionalFiles = ScriptEditor.Script.GetAdditionalBackupFiles();
        if (ScriptEditor.GoldEditor != null)
        {
            additionalFiles.AddRange(ScriptEditor.GoldEditor.Script.GetAdditionalBackupFiles());
        }

        foreach (string additionalFile in additionalFiles.Distinct())
        {
            string targetFolder = Path.GetFullPath(Path.GetDirectoryName(Path.Combine(_targetDirectory, "../", additionalFile)));
            string outputFile = Path.Combine(_outputDirectory, Path.GetFileName(additionalFile));
            IOExtensions.CopyFile(outputFile, new DirectoryInfo(targetFolder), true);
        }

        if (_scriptEditor.Edition.Remastered && _scriptEditor.LevelSequencingOrganisation != Organisation.Default)
        {
            // Add some map data for any external programs
            List<TRRSequence> sequences = GenerateTRRSequenceOutput();
            File.WriteAllText(Path.Combine(_targetDirectory, "trge.json"), JsonConvert.SerializeObject(sequences));
        }
    }

    private static void RenameOutputFile(string baseFile, string targetFile)
    {
        if (!File.Exists(baseFile))
        {
            return;
        }

        File.Move(baseFile, targetFile, true);
    }

    private void CopyLevelToTarget(AbstractTRScriptedLevel level)
    {
        List<string> files = level is TRRScriptedLevel trrLevel 
            ? trrLevel.AllFiles
            : new() { level.LevelFile };

        foreach (string file in files)
        {
            string outputFile = Path.Combine(_outputDirectory, Path.GetFileName(file));
            if (File.Exists(outputFile))
            {
                string targetFile = Path.GetFullPath(Path.Combine(_targetDirectory, "../", file));
                IOExtensions.CopyFile(outputFile, targetFile, true);
            }
        }
    }

    public void Restore()
    {
        _restoreArgs = new TRBackupRestoreEventArgs
        {
            ProgressValue = 0,
            ProgressTarget = 1
        };

        if (LevelEditor != null)
        {
            _restoreArgs.ProgressTarget += LevelEditor.GetRestoreTarget();
            LevelEditor.RestoreProgressChanged += LevelEditor_RestoreProgressChanged;
        }

        FireRestoreProgressChanged();

        try
        {
            ScriptEditor.Restore();
            FireRestoreProgressChanged(1);

            LevelEditor?.Restore();
        }
        finally
        {
            if (LevelEditor != null) LevelEditor.RestoreProgressChanged -= LevelEditor_RestoreProgressChanged;
        }
    }

    private void LevelEditor_RestoreProgressChanged(object sender, EventArgs e)
    {
        FireRestoreProgressChanged(1);
    }

    private void FireRestoreProgressChanged(int progress = 0)
    {
        _restoreArgs.ProgressValue += progress;
        RestoreProgressChanged?.Invoke(this, _restoreArgs);
    }

    public void ExportSettings(string filePath)
    {
        if (!IsExportPossible)
        {
            throw new InvalidOperationException();
        }

        Config config = new()
        {
            ["TRGE"] = ScriptEditor.ExportConfig()
        };
        if (LevelEditor != null)
        {
            config["TRLE"] = LevelEditor.ExportConfig();
        }

        config.Write(filePath);
    }

    public void ImportSettings(string filePath)
    {
        Config config = Config.Read(filePath);
        if (config.ContainsKey("TRGE"))
        {
            ScriptEditor.ImportConfig(config.GetSubConfig("TRGE"));
        }
        if (config.ContainsKey("TRLE") && LevelEditor != null)
        {
            LevelEditor.ImportConfig(config.GetSubConfig("TRLE"));
        }
    }

    public void ResetSettings()
    {
        ScriptEditor.ResetConfig();
        LevelEditor?.ResetConfig();
    }

    private void ConfigureWatcher()
    {
        if (_watcher != null || ScriptEditor == null)
        {
            return;
        }

        _watcher = new ConfigFileWatcher(ScriptEditor.ConfigFilePath);
        _watcher.Changed += delegate (object sender, FileSystemEventArgs e)
        {
            ConfigExternallyChanged?.Invoke(this, e);
        };
    }

    public void Unload()
    {
        if (_watcher != null)
        {
            _watcher.Enabled = false;
            _watcher = null;
        }

        ScriptEditor = null;
        LevelEditor = null;
    }

    private List<TRRSequence> GenerateTRRSequenceOutput()
    {
        List<AbstractTRScriptedLevel> levels = new(ScriptEditor.Levels);
        return new(levels.Select(l => new TRRSequence
        {
            Index = l.Sequence,
            Name = l.Name,
            Ammoless = l.RemovesAmmo,
            Unarmed = l.RemovesWeapons,
            HasCutScene = levels.Find(lvl => lvl.OriginalSequence == l.Sequence).HasCutScene,
        }));
    }

    private class TRRSequence
    {
        public int Index { get; set; }
        public string Name { get; set; }
        public bool Unarmed { get; set; }
        public bool Ammoless { get; set; }
        public bool HasCutScene { get; set; }
    }
}