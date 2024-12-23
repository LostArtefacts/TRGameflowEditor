﻿using Newtonsoft.Json;
using TRGE.Coord.Helpers;
using TRGE.Core;
using TRLevelControl.Helpers;

namespace TRGE.Coord;

internal class TRIOCoord : ITRConfigProvider
{
    internal enum OperationMode
    {
        File, Directory
    }

    /// <summary>
    /// The main folder name in the application settings folder that houses all edit information.
    /// </summary>
    protected const string _editDirectoryName = "Editors";
    /// <summary>
    /// The folder name under a specific edit folder where backups are stored.
    /// </summary>
    protected const string _backupDirectoryName = "Backup";
    /// <summary>
    /// The folder name under a specific edit folder where outputs are stored.
    /// </summary>
    protected const string _outputDirectoryName = "Output";
    /// <summary>
    /// The folder name under the outputs folder where temporary files are written to. Handlers should
    /// move these files to Output on successful completion of a save transaction.
    /// </summary>
    protected const string _wipDirectoryName = "WIP";
    /// <summary>
    /// The name of the config file that stores script editing information.
    /// </summary>
    protected const string _scriptConfigFileName = "trge.csf";
    /// <summary>
    /// The name of the config file that stores level editing information.
    /// </summary>
    protected const string _dirConfigFileName = "trle.csf";

    protected OperationMode _mode;

    protected string _editDirectory;
    protected string _orignalScriptFile, _backupScriptFile, _orignalGoldScriptFile,_backupGoldScriptFile, _scriptConfigFile, _originalTRConfigFile, _backupTRConfigFile;
    protected string _originalDirectory, _directoryConfigFile;

    internal string OriginalDirectory => _originalDirectory;
    internal string OutputDirectory => GetOutputDirectory();
    internal string WIPOutputDirectory => GetWIPOutputDirectory();

    #region History Vars and Events
    protected readonly List<string> _history;
    internal IReadOnlyList<string> History => _history;

    internal event EventHandler<TRHistoryEventArgs> HistoryAdded;
    internal event EventHandler HistoryChanged;
    #endregion

    internal event EventHandler<TRBackupRestoreEventArgs> BackupProgressChanged;
    private TRBackupRestoreEventArgs _backupArgs;

    internal TRIOCoord()
    {
        _history = new List<string>();
    }

    internal TREditor Open(string path, TRScriptOpenOption openOption, TRBackupChecksumOption checksumOption)
    {
        _mode = Directory.Exists(path) ? OperationMode.Directory : OperationMode.File;
        _orignalScriptFile = _mode == OperationMode.Directory ? FindScriptFile(path, false) : path;
        _orignalGoldScriptFile = _mode == OperationMode.Directory ? FindScriptFile(path, true) : path;
        _originalTRConfigFile = _mode == OperationMode.Directory ? FindConfigFile(path) : path;
        _originalDirectory = _mode == OperationMode.Directory ? path : new FileInfo(_orignalScriptFile).DirectoryName;

        // Verify that the script and level editors are compatible - this
        // should be done before any backups are performed.
        if (_mode == OperationMode.Directory && _orignalScriptFile != null)
        {
            TREditor.ValidateCompatibility(TRScriptFactory.OpenScript(_orignalScriptFile), _originalDirectory);
        }

        _editDirectory = GetEditDirectory();

        _backupArgs = new TRBackupRestoreEventArgs()
        {
            ProgressValue = 0,
            ProgressTarget = 1
        };
        CreateBackup(checksumOption);

        AbstractTRScriptEditor scriptEditor = GetScriptEditor(openOption);
        TidyBackup(scriptEditor);

        AbstractTRLevelEditor levelEditor = GetLevelEditor(scriptEditor);

        FireBackupProgressChanged(1);

        return new TREditor(WIPOutputDirectory, OutputDirectory, OriginalDirectory)
        {
            ScriptEditor = scriptEditor,
            LevelEditor = levelEditor
        };
    }

    internal AbstractTRScriptEditor GetScriptEditor(TRScriptOpenOption openOption)
    {
        TRScriptIOArgs io = new()
        {
            TRScriptFile = _orignalScriptFile == null ? null : new FileInfo(_orignalScriptFile),
            TRScriptBackupFile = _backupScriptFile == null ? null : new FileInfo(_backupScriptFile),
            TRConfigFile = _originalTRConfigFile == null ? null : new FileInfo(_originalTRConfigFile),
            TRConfigBackupFile = _backupTRConfigFile == null ? null : new FileInfo(_backupTRConfigFile),
            InternalConfigFile = new FileInfo(_scriptConfigFile),
            WIPOutputDirectory = new DirectoryInfo(GetWIPOutputDirectory()),
            OutputDirectory = new DirectoryInfo(GetOutputDirectory()),
            OriginalDirectory = new DirectoryInfo(_originalDirectory),
            BackupDirectory = new DirectoryInfo(GetBackupDirectory())
        };
        AbstractTRScriptEditor scriptMan = TRScriptFactory.GetScriptEditor(io, openOption);

        if (scriptMan.Edition.HasGold && _orignalGoldScriptFile != null)
        {
            TRScriptIOArgs goldIO = new()
            {
                TRScriptFile = new FileInfo(_orignalGoldScriptFile),
                TRScriptBackupFile = _backupGoldScriptFile == null ? null : new FileInfo(_backupGoldScriptFile),
                TRConfigFile = _originalTRConfigFile == null ? null : new FileInfo(_originalTRConfigFile),
                TRConfigBackupFile = _backupTRConfigFile == null ? null : new FileInfo(_backupTRConfigFile),
                InternalConfigFile = null,
                WIPOutputDirectory = new DirectoryInfo(GetWIPOutputDirectory()),
                OutputDirectory = new DirectoryInfo(GetOutputDirectory()),
                OriginalDirectory = new DirectoryInfo(_originalDirectory),
                BackupDirectory = new DirectoryInfo(GetBackupDirectory()),
            };
            scriptMan.GoldEditor = TRScriptFactory.GetScriptEditor(goldIO, openOption);
        }

        UpdateFileHistory();

        return scriptMan;
    }

    internal AbstractTRLevelEditor GetLevelEditor(AbstractTRScriptEditor scriptEditor)
    {
        if (_mode == OperationMode.File)
        {
            return null;
        }

        TRDirectoryIOArgs io = new()
        {
            OriginalDirectory = new DirectoryInfo(_originalDirectory),
            BackupDirectory = new DirectoryInfo(GetBackupDirectory()),
            ConfigFile = new FileInfo(_directoryConfigFile),
            WIPOutputDirectory = new DirectoryInfo(GetWIPOutputDirectory()),
            OutputDirectory = new DirectoryInfo(GetOutputDirectory())
        };

        return TRLevelEditorFactory.GetLevelEditor(io, scriptEditor.Edition);
    }

    private static string FindScriptFile(string path, bool gold)
    {
        FileInfo fi = TRScriptFactory.FindScriptFile(new DirectoryInfo(path), gold);
        if (fi == null)
        {
            // Test Remasters and use fake scripts. This is ugly.
            bool isTR1 = TR1LevelNames.AsList.All(l => File.Exists(Path.Combine(path, l)));
            if (isTR1)
            {
                if (Directory.Exists(Path.Combine(path, "UB")) && File.Exists(Path.Combine(path, "../tomb1.dll")))
                {
                    return gold ? null : TRRScript.TR1PlaceholderName;
                }

                // Guess that it's TombATI
                throw new PlatformNotSupportedException("The use of TombATI is not supported. Please upgrade to TR1X - https://github.com/LostArtefacts/TR1X/");
            }

            bool isTR2 = TR2LevelNames.AsList.All(l => File.Exists(Path.Combine(path, l)));
            if (isTR2 && Directory.Exists(Path.Combine(path, "GM")) && File.Exists(Path.Combine(path, "../tomb2.dll")))
            {
                return gold ? null : TRRScript.TR2PlaceholderName;
            }

            bool isTR3 = TR3LevelNames.AsList.All(l => File.Exists(Path.Combine(path, l)));
            if (isTR3 && Directory.Exists(Path.Combine(path, "LA")) && File.Exists(Path.Combine(path, "../tomb3.dll")))
            {
                return gold ? null : TRRScript.TR3PlaceholderName;
            }

            throw new MissingScriptException(string.Format("No valid Tomb Raider script file was found in {0}.", path));
        }
        return fi?.FullName;
    }

    private static string FindConfigFile(string path)
    {
        FileInfo fi = TRScriptFactory.FindConfigFile(new DirectoryInfo(path));
        return fi?.FullName;
    }

    protected void CreateBackup(TRBackupChecksumOption checksumOption)
    {
        string backupDirectory = GetBackupDirectory();
        string outputDirectory = GetOutputDirectory();
        if (_mode == OperationMode.Directory)
        {
            AbstractTRScript script = TRScriptFactory.OpenScript(_orignalScriptFile);

            DirectoryInfo backupDI = new(backupDirectory);
            DirectoryInfo outputDI = new(outputDirectory);

            List<string> filesToBackup = new();
            if (!script.Edition.Remastered)
            {
                if (_orignalScriptFile != null)
                {
                    filesToBackup.Add(_orignalScriptFile);
                }
                if (_originalTRConfigFile != null)
                {
                    filesToBackup.Add(_originalTRConfigFile);
                }
            }

            void backupLevels(List<AbstractTRScriptedLevel> levels)
            {
                foreach (AbstractTRScriptedLevel level in levels)
                {
                    if (level is TRRScriptedLevel remasteredLevel)
                    {
                        filesToBackup.AddRange(remasteredLevel.AllFiles.Select(f => GetOriginalFilePath(f)));
                        if (level.HasCutScene)
                        {
                            filesToBackup.AddRange((remasteredLevel.CutSceneLevel as TRRScriptedLevel).AllFiles.Select(f => GetOriginalFilePath(f)));
                        }
                    }
                    else
                    {
                        filesToBackup.Add(GetOriginalFilePath(level.LevelFile));
                        if (level.HasCutScene)
                        {
                            filesToBackup.Add(GetOriginalFilePath(level.CutSceneLevel.LevelFile));
                        }
                    }
                }
            }

            // Open the original script and determine which files we need to copy. Merge the level files
            // with the original paths as some may not be in the current directory (e.g. TR3 cutscene files).
            backupLevels(script.Levels);
            if (script is TRRScript trrscript)
            {
                backupLevels(trrscript.GoldLevels);
            }

            AbstractTRScriptedLevel assaultLevel = script.AssaultLevel;
            if (assaultLevel != null)
            {
                backupLevels(new() { assaultLevel });
            }

            AbstractTRScript goldScript = null;
            if (_orignalGoldScriptFile != null)
            {
                filesToBackup.Add(_orignalGoldScriptFile);
                goldScript = TRScriptFactory.OpenScript(_orignalGoldScriptFile);
                backupLevels(goldScript.Levels);
            }

            // Perform a checksum against the level files unless the option to ignore issues has already been passed.
            // This only happens during the initial backup of a file, otherwise future edits would be flagged.
            if (checksumOption == TRBackupChecksumOption.PerformCheck && TRInterop.ChecksumTester != null)
            {
                _backupArgs.ProgressTarget += filesToBackup.Count;
                FireBackupProgressChanged();

                List<string> failures = new();
                List<string> exts = new() { ".PHD", ".TR2", ".TR4", ".TRC" };
                foreach (string file in filesToBackup)
                {
                    if (!exts.Contains(Path.GetExtension(file).ToUpper()))
                    {
                        FireBackupProgressChanged(1);
                        continue;
                    }
                    if (!File.Exists(Path.Combine(backupDI.FullName, Path.GetFileName(file))) && !TRInterop.ChecksumTester.Test(file))
                    {
                        failures.Add(file);
                    }
                    FireBackupProgressChanged(1);
                }
                if (failures.Count > 0)
                {
                    throw new ChecksumMismatchException();
                }
            }

            filesToBackup.AddRange(script.GetAdditionalBackupFiles()
                .Select(f => GetOriginalFilePath(f)));
            if (goldScript != null)
            {
                filesToBackup.AddRange(goldScript.GetAdditionalBackupFiles()
                    .Select(f => GetOriginalFilePath(f)));
            }

            TestTRRCommon(filesToBackup, script);

            _backupArgs.ProgressTarget += filesToBackup.Count * 2;
            FireBackupProgressChanged();

            foreach (string file in filesToBackup)
            {
                IOExtensions.CopyFile(file, backupDI, false);
                FireBackupProgressChanged(1);

                IOExtensions.CopyFile(file, outputDI, false);
                FireBackupProgressChanged(1);
            }

            if (_orignalScriptFile != null)
            {
                _backupScriptFile = Path.Combine(backupDirectory, new FileInfo(_orignalScriptFile).Name);
            }
            if (_orignalGoldScriptFile != null)
            {
                _backupGoldScriptFile = Path.Combine(backupDirectory, new FileInfo(_orignalGoldScriptFile).Name);
            }
            if (_originalTRConfigFile != null)
            {
                _backupTRConfigFile = Path.Combine(backupDirectory, new FileInfo(_originalTRConfigFile).Name);
            }
            else
            {
                _backupTRConfigFile = null;
            }
        }
        else
        {
            FileInfo fi = new(_orignalScriptFile);
            _backupScriptFile = Path.Combine(backupDirectory, fi.Name);
            if (!File.Exists(_backupScriptFile))
            {
                File.Copy(_orignalScriptFile, _backupScriptFile);
            }

            string outputFile = Path.Combine(outputDirectory, fi.Name);
            if (!File.Exists(outputFile))
            {
                File.Copy(_orignalScriptFile, outputFile);
            }
        }

        _scriptConfigFile = Path.Combine(_editDirectory, _scriptConfigFileName);
        _directoryConfigFile = Path.Combine(_editDirectory, _dirConfigFileName);
    }

    private void TestTRRCommon(List<string> backupFiles, AbstractTRScript script)
    {
        if (script is not TRRScript
            || backupFiles.Find(f => Path.GetFileName(f).ToUpper() == "COMMON.TXT") is not string commonFile)
        {
            return;
        }

        string commonFolder = Path.GetFullPath(Path.Combine(_editDirectory, "../TRR"));
        Directory.CreateDirectory(commonFolder);
        string commonBackupFile = Path.Combine(commonFolder, Path.GetFileName(commonFile));
        IOExtensions.CopyFile(commonFile, commonBackupFile, false);

        backupFiles.Remove(commonFile);
        backupFiles.Add(commonBackupFile);
    }

    private string GetOriginalFilePath(string fileName)
    {
        return Path.GetFullPath(Path.Combine(_originalDirectory, "../", fileName));
    }

    private void FireBackupProgressChanged(int progress = 0)
    {
        _backupArgs.ProgressValue += progress;
        BackupProgressChanged?.Invoke(this, _backupArgs);
    }

    protected void TidyBackup(AbstractTRScriptEditor scriptEditor)
    {
        DirectoryInfo backupDI = new(GetBackupDirectory());
        DirectoryInfo outputDI = new(GetOutputDirectory());
        List<string> expectedFiles = new();
        if (_orignalScriptFile != null)
        {
            expectedFiles.Add(scriptEditor.BackupFile.Name);
        }
        if (scriptEditor.GoldEditor != null)
        {
            expectedFiles.Add(scriptEditor.GoldEditor.BackupFile.Name);
        }
        if (_originalTRConfigFile != null)
        {
            expectedFiles.Add(scriptEditor.BackupTRConfigFile.Name);
        }

        if (scriptEditor.Edition.AssaultCourseSupported)
        {
            expectedFiles.Add(scriptEditor.LevelManager.AssaultLevel.LevelFileBaseName);
        }
        foreach (AbstractTRScriptedLevel level in scriptEditor.Levels)
        {
            expectedFiles.Add(level.LevelFileBaseName);
            if (level.HasCutScene)
            {
                expectedFiles.Add(level.CutSceneLevel.LevelFileBaseName);
            }
        }

        if (scriptEditor.GoldEditor != null)
        {
            foreach (AbstractTRScriptedLevel level in scriptEditor.GoldEditor.Levels)
            {
                expectedFiles.Add(level.LevelFileBaseName);
                if (level.HasCutScene)
                {
                    expectedFiles.Add(level.LevelFileBaseName);
                }
            }
        }
        else if (scriptEditor.Script is TRRScript trrscript)
        {
            expectedFiles.AddRange(trrscript.GoldLevels.Select(l => l.LevelFileBaseName));
        }

        expectedFiles.AddRange(scriptEditor.Script.GetAdditionalBackupFiles()
                .Select(f => Path.GetFileName(f)));
        if (scriptEditor.GoldEditor != null)
        {
            expectedFiles.AddRange(scriptEditor.GoldEditor.Script.GetAdditionalBackupFiles()
                .Select(f => Path.GetFileName(f)));
        }

        backupDI.ClearExcept(expectedFiles, TREditor.TargetFileExtensions);
        outputDI.ClearExcept(expectedFiles, TREditor.TargetFileExtensions);
    }

    #region Directory Management
    internal string GetEditDirectory()
    {
        DirectoryInfo topLevelEditDirectory = Directory.CreateDirectory(Path.Combine(TRCoord.Instance.ConfigDirectory, _editDirectoryName));
        string hashBase = Path.GetFullPath(_originalDirectory);
        if (_orignalScriptFile != null)
        {
            hashBase += $@"\{_orignalScriptFile}";
        }

        return topLevelEditDirectory.CreateSubdirectory(HashingExtensions.CreateMD5(hashBase.ToUpper())).FullName;
    }

    internal string GetBackupDirectory()
    {
        DirectoryInfo editDirectory = new(GetEditDirectory());
        return editDirectory.CreateSubdirectory(_backupDirectoryName).FullName;
    }

    internal string GetWIPOutputDirectory()
    {
        DirectoryInfo outputDirectory = new(GetOutputDirectory());
        return outputDirectory.CreateSubdirectory(_wipDirectoryName).FullName;
    }

    internal string GetOutputDirectory()
    {
        DirectoryInfo editDirectory = new(GetEditDirectory());
        return editDirectory.CreateSubdirectory(_outputDirectoryName).FullName;
    }

    internal string GetOriginalDirectory()
    {
        return _originalDirectory;
    }
    #endregion

    #region History Methods
    protected void FireHistoryChanged()
    {
        HistoryChanged?.Invoke(this, EventArgs.Empty);
    }

    protected void FireHistoryAdded(string filePath)
    {
        HistoryAdded?.Invoke(this, new TRHistoryEventArgs(filePath));
    }

    protected void UpdateFileHistory()
    {
        string target = _mode == OperationMode.Directory ? _originalDirectory : _orignalScriptFile;

        int j = GetFileHistoryIndex(target);
        _history.Insert(0, target);
        if (j != -1)
        {
            _history.RemoveAt(j + 1);
        }

        while (_history.Count > 10)
        {
            _history.RemoveAt(_history.Count - 1);
        }
        FireHistoryAdded(target);
    }

    protected int GetFileHistoryIndex(string filePath)
    {
        filePath = filePath.ToLower();
        for (int i = 0; i < _history.Count; i++)
        {
            if (_history[i].ToLower().Equals(filePath))
            {
                return i;
            }
        }
        return -1;
    }

    internal void ClearHistory()
    {
        string mainEditDirectory = Path.Combine(TRCoord.Instance.ConfigDirectory, _editDirectoryName);
        if (Directory.Exists(mainEditDirectory))
        {
            Directory.Delete(mainEditDirectory, true);
        }
        _history.Clear();
        FireHistoryChanged();
    }

    internal void ClearBackup()
    {
        string backupDirectory = GetEditDirectory();
        if (Directory.Exists(backupDirectory))
        {
            Directory.Delete(backupDirectory, true);
        }

        if (_orignalScriptFile == TRRScript.TR1PlaceholderName
            || _orignalScriptFile == TRRScript.TR2PlaceholderName
            || _orignalScriptFile == TRRScript.TR3PlaceholderName)
        {
            string commonFolder = Path.GetFullPath(Path.Combine(_editDirectory, "../TRR"));
            if (Directory.Exists(commonFolder))
            {
                Directory.Delete(commonFolder, true);
            }
        }
    }

    internal void CheckBackupIntegrity()
    {
        if (TRInterop.ChecksumTester == null)
        {
            return;
        }

        List<string> failures = new();
        List<string> exts = new() { ".PHD", ".TR2", ".TR4", ".TRC" };
        foreach (string backupFile in Directory.GetFiles(GetBackupDirectory()))
        {
            if (!exts.Contains(Path.GetExtension(backupFile).ToUpper()))
            {
                continue;
            }
            if (!TRInterop.ChecksumTester.Test(backupFile))
            {
                failures.Add(Path.GetFileName(backupFile));
            }
        }

        if (failures.Count == 0)
        {
            return;
        }

        throw new ChecksumMismatchException
        {
            FailedFiles = failures,
        };
    }
    #endregion

    #region ITRConfigProvider
    public void SetConfig(object config, string configDirectory)
    {
        if (config == null || !Directory.Exists(Path.Combine(configDirectory, _editDirectoryName)))
        {
            _history.Clear();
        }
        else
        {
            string[] history = JsonConvert.DeserializeObject<string[]>(config.ToString());
            foreach (string item in history)
            {
                if (File.Exists(item) || Directory.Exists(item))
                {
                    _history.Add(item);
                    FireHistoryAdded(item);
                }
            }

            if (history.Length == 0)
            {
                FireHistoryChanged();
            }
        }
    }

    public object GetConfig()
    {
        return _history.ToArray();
    }
    #endregion
}