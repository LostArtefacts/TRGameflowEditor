using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.ExceptionServices;
using TRGE.Core;

namespace TRGE.Coord
{
    public class TREditor
    {
        public static readonly string[] TargetFileExtensions = new string[] { "*.dat", "*.tr2", "*.psx" };

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
        public string BackupDirectory => _scriptEditor.BackupFile.DirectoryName;
        public string ErrorDirectory => Path.GetFullPath(Path.Combine(BackupDirectory, @"..\Errors"));
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
            if (LevelEditor != null)
            {
                LevelEditor.ScriptedLevelModified(e);
            }
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
            //TRSaveMonitor monitor = new TRSaveMonitor(new TRSaveEventArgs
            //{
            //    ProgressTarget = ScriptEditor.GetSaveTargetCount() + 
            //    (LevelEditor == null ? 0 : LevelEditor.GetSaveTargetCount())
            //});
            //monitor.SaveStateChanged += Editor_SaveStateChanged;

            DirectoryInfo wipDirectory = new DirectoryInfo(_wipOutputDirectory);
            wipDirectory.Create();
            wipDirectory.Clear();

            _watcher.Enabled = false;

            try
            {
                if (LevelEditor != null)
                {
                    LevelEditor.PreSave(ScriptEditor);
                }

                ScriptEditor.Save(/*monitor*/);

                TRSaveMonitor monitor = new TRSaveMonitor(new TRSaveEventArgs
                {
                    ProgressTarget = ScriptEditor.GetSaveTargetCount() +
                    (LevelEditor == null ? 0 : LevelEditor.GetSaveTargetCount())
                });
                monitor.SaveStateChanged += Editor_SaveStateChanged;
                monitor.FireSaveStateBeginning(TRSaveCategory.Scripting);
                monitor.FireSaveStateChanged(1);

                if (LevelEditor != null)
                {
                    LevelEditor.Save(ScriptEditor, monitor);
                }

                if (!monitor.IsCancelled)
                {
                    monitor.FireSaveStateChanged(0, TRSaveCategory.Commit);

                    ScriptEditor.SaveComplete();
                    if (LevelEditor != null)
                    {
                        LevelEditor.SaveComplete();
                    }

                    // Copy everything from WIP into the Output folder.
                    DirectoryInfo outputDirectory = new DirectoryInfo(_outputDirectory);
                    wipDirectory.Copy(outputDirectory, true, TargetFileExtensions);

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
                if (LevelEditor != null)
                {
                    LevelEditor.Initialise(ScriptEditor);
                }

                _watcher.Enabled = true;
                wipDirectory.Clear();
            }
        }

        private void LogException(Exception e)
        {
            Config config = new Config
            {
                ["Trace"] = e.ToString(),
                ["TRGE"] = ScriptEditor.ExportConfig()
            };
            if (LevelEditor != null)
            {
                config["TRLE"] = LevelEditor.ExportConfig();
            }

            Dictionary<string, string> checksums = new Dictionary<string, string>();
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
            DirectoryInfo targetDataDirectory = new DirectoryInfo(_targetDirectory);
            string outputScript = ScriptEditor.GetScriptOutputPath();
            IOExtensions.CopyFile(outputScript, targetDataDirectory, true);

            List<AbstractTRScriptedLevel> levels = new List<AbstractTRScriptedLevel>(ScriptEditor.Levels);
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
        }

        private void CopyLevelToTarget(AbstractTRScriptedLevel level)
        {
            string outputLevel = Path.Combine(_outputDirectory, level.LevelFileBaseName);
            if (File.Exists(outputLevel))
            {
                string targetFile = Path.GetFullPath(Path.Combine(_targetDirectory, @"..\", level.LevelFile));
                IOExtensions.CopyFile(outputLevel, targetFile, true);
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

                if (LevelEditor != null) LevelEditor.Restore();
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

            Config config = new Config
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
    }
}