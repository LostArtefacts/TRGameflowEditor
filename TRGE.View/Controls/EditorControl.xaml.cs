using Microsoft.Win32;
using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using TRGE.Coord;
using TRGE.Core;
using TRGE.View.Model;
using TRGE.View.Utils;
using TRGE.View.Windows;

namespace TRGE.View.Controls
{
    /// <summary>
    /// Interaction logic for EditorControl.xaml
    /// </summary>
    public partial class EditorControl : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty TREditionProperty = DependencyProperty.Register
        (
            "Edition", typeof(string), typeof(EditorControl)
        );

        public static readonly DependencyProperty DataFolderProperty = DependencyProperty.Register
        (
            "DataFolder", typeof(string), typeof(EditorControl), new PropertyMetadata(string.Empty)
        );

        public string Edition
        {
            get => (string)GetValue(TREditionProperty);
            private set => SetValue(TREditionProperty, value);
        }

        public string DataFolder
        {
            get => (string)GetValue(DataFolderProperty);
            private set => SetValue(DataFolderProperty, value);
        }
        #endregion

        private readonly EditorOptions _options;
        private bool _dirty, _reloadRequested;
        private volatile bool _showExternalModPrompt;

        public event EventHandler<EditorEventArgs> EditorStateChanged;

        public TREditor Editor { get; private set; }

        public EditorControl()
        {
            InitializeComponent();
            _editorGrid.DataContext = _options = new EditorOptions();
            _options.PropertyChanged += Editor_PropertyChanged;
            _dirty = false;
            _showExternalModPrompt = true;
            _reloadRequested = false;
        }

        private void Editor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _dirty = true;
            FireEditorStateChanged();
        }

        private void FireEditorStateChanged()
        {
            EditorStateChanged?.Invoke(this, new EditorEventArgs
            {
                IsDirty = _dirty,
                CanExport = Editor != null && Editor.IsExportPossible,
                ReloadRequested = _reloadRequested
            });
        }

        public void Load(DataFolderEventArgs e)
        {
            Editor = e.Editor;
            Edition = Editor.Edition.Title;
            DataFolder = e.DataFolder;

            Editor.ConfigExternallyChanged += Editor_ConfigExternallyChanged;

            Reload();
        }

        private void Editor_ConfigExternallyChanged(object sender, FileSystemEventArgs e)
        {
            // Prevent several message boxes appearing i.e. in case the initial box is displayed and
            // several external edits are carried out.
            if (_showExternalModPrompt)
            {
                _showExternalModPrompt = false;
                new Thread(() => Dispatcher.Invoke(() => HandleConfigExternallyChanged(e))).Start();
            }
        }

        private void HandleConfigExternallyChanged(FileSystemEventArgs _)
        {
            string message = _dirty ? 
                "The configuration file has been modified by an external program and you have unsaved changes.\n\nDo you want to reload the configuration and lose your changes?" :
                "The configuration file has been modified by an external program. Do you want to reload the configuration?";

            try
            {
                if (MessageWindow.ShowConfirm(message))
                {
                    _dirty = false;
                    _reloadRequested = true;
                    Editor.ConfigExternallyChanged -= Editor_ConfigExternallyChanged;
                    FireEditorStateChanged();
                }
                else
                {
                    _dirty = true;
                    FireEditorStateChanged();
                }
            }
            finally
            {
                _showExternalModPrompt = true;
            }
        }

        private void Reload()
        {
            _options.Load(Editor.ScriptEditor as TR23ScriptEditor);
            _dirty = false;
            FireEditorStateChanged();
        }

        public bool Save()
        {
            SaveProgressWindow spw = new(Editor, _options);
            if (spw.ShowDialog() ?? false)
            {
                _dirty = false;
                FireEditorStateChanged();
                return true;
            }
            return false;
        }

        public void Unload()
        {
            if (Editor != null)
            {
                Editor.ConfigExternallyChanged -= Editor_ConfigExternallyChanged;
                Editor.Unload();
                Editor = null;
            }

            _options.Unload();

            _dirty = false;
            _reloadRequested = false;
            FireEditorStateChanged();
        }

        public void OpenBackupFolder()
        {
            Process.Start("explorer.exe", Editor.BackupDirectory);
        }

        public void RestoreDefaults()
        {
            if (MessageWindow.ShowConfirm("The files that were backed up when this folder was first opened will be copied back to the original directory.\n\nDo you wish to proceed?"))
            {
                RestoreProgressWindow rpw = new(Editor);
                try
                {
                    if (rpw.ShowDialog() ?? false)
                    {
                        Reload();
                        MessageWindow.ShowMessage("The restore completed successfully.");
                    }
                    else if (rpw.RestoreException != null)
                    {
                        throw rpw.RestoreException;
                    }
                }
                catch (Exception e)
                {
                    MessageWindow.ShowError(e.Message);
                }
            }
        }

        public void ImportSettings()
        {
            OpenFileDialog dlg = new()
            {
                Filter = "TRGE Files|*.trge",
                Title = "TRGE : Import Settings"
            };
            if (dlg.ShowDialog(WindowUtils.GetActiveWindow()) ?? false)
            {
                try
                {
                    Editor.ImportSettings(dlg.FileName);
                    _options.Load(Editor.ScriptEditor as TR23ScriptEditor);
                }
                catch (Exception e)
                {
                    MessageWindow.ShowError(e.Message);
                }
            }
        }

        public void ExportSettings()
        {
            if (!ConfirmExport())
            {
                return;
            }

            SaveFileDialog dlg = new()
            {
                Filter = "TRGE Files|*.trge",
                Title = "TRGE : Export Settings",
                FileName = Edition.ToSafeFileName() + ".trge",
            };
            if (dlg.ShowDialog(WindowUtils.GetActiveWindow()) ?? false)
            {
                try
                {
                    Editor.ExportSettings(dlg.FileName);
                }
                catch (Exception e)
                {
                    MessageWindow.ShowError(e.Message);
                }
            }
        }

        private bool ConfirmExport()
        {
            int unviableCount = _options.GetUnviableCount();
            if (unviableCount > 0)
            {
                StringBuilder sb = new("As the following items have been edited externally, they will be reset to default in the exported file.");
                sb.Append(Environment.NewLine);
                if (!_options.LevelSequencingViable)
                {
                    sb.Append(Environment.NewLine).Append("Level Sequencing");
                }
                if (!_options.UnarmedLevelsViable)
                {
                    sb.Append(Environment.NewLine).Append("Unarmed Levels");
                }
                if (!_options.AmmolessLevelsViable)
                {
                    sb.Append(Environment.NewLine).Append("Ammoless Levels");
                }
                if (_options.SecretRewardsSupported && !_options.SecretRewardsViable)
                {
                    sb.Append(Environment.NewLine).Append("Secret Rewards");
                }
                if (_options.SunsetsSupported && !_options.SunsetsViable)
                {
                    sb.Append(Environment.NewLine).Append("Sunsets");
                }
                if (!_options.AudioViable)
                {
                    sb.Append(Environment.NewLine).Append("Audio");
                }

                return MessageWindow.ShowWarningWithCancel(sb.ToString());
            }

            return true;
        }

        private void LevelSequencing_ManualConfigure(object sender, RoutedEventArgs e)
        {
            LevelSequenceWindow lsw = new(_options.LevelSequencing);
            if (lsw.ShowDialog() ?? false)
            {
                _options.LevelSequencing = lsw.LevelSequencingData;
            }
        }

        private void UnarmedLevels_ManualConfigure(object sender, RoutedEventArgs e)
        {
            UnarmedLevelsWindow ulw = new(_options.UnarmedLevelData);
            if (ulw.ShowDialog() ?? false)
            {
                _options.UnarmedLevelData = ulw.LevelData;
            }
        }

        private void AmmolessLevels_ManualConfigure(object sender, RoutedEventArgs e)
        {
            AmmolessLevelsWindow alw = new(_options.AmmolessLevelData);
            if (alw.ShowDialog() ?? false)
            {
                _options.AmmolessLevelData = alw.LevelData;
            }
        }

        private void SecretRewards_ManualConfigure(object sender, RoutedEventArgs e)
        {
            BonusItemsWindow biw = new(_options.SecretBonusData);
            if (biw.ShowDialog() ?? false)
            {
                _options.SecretBonusData = biw.SecretBonusData;
            }
        }

        private void Sunsets_ManualConfigure(object sender, RoutedEventArgs e)
        {
            SunsetLevelsWindow slw = new(_options.SunsetLevelData);
            if (slw.ShowDialog() ?? false)
            {
                _options.SunsetLevelData = slw.LevelData;
            }
        }

        private void Audio_ManualConfigure(object sender, RoutedEventArgs e)
        {
            AudioWindow aw = new(_options);
            if (aw.ShowDialog() ?? false)
            {
                _options.GlobalAudioData = aw.AudioData;
            }
        }

        private void LevelSequencing_ChangeViability(object sender, RoutedEventArgs e)
        {
            if (MessageWindow.ShowConfirm("Any external level sequencing changes made to the current configuration will be lost during the next save.\n\nDo you wish to proceed?"))
            {
                _options.SetLevelSequencingViable();
            }
        }

        private void UnarmedLevels_ChangeViability(object sender, RoutedEventArgs e)
        {
            if (MessageWindow.ShowConfirm("Any external unarmed level changes made to the current configuration will be lost during the next save.\n\nDo you wish to proceed?"))
            {
                _options.SetUnarmedLevelsViable();
            }
        }

        private void AmmolessLevels_ChangeViability(object sender, RoutedEventArgs e)
        {
            if (MessageWindow.ShowConfirm("Any external ammoless level changes made to the current configuration will be lost during the next save.\n\nDo you wish to proceed?"))
            {
                _options.SetAmmolessLevelsViable();
            }
        }

        private void SecretRewards_ChangeViability(object sender, RoutedEventArgs e)
        {
            if (MessageWindow.ShowConfirm("Any external secret reward changes made to the current configuration will be lost during the next save.\n\nDo you wish to proceed?"))
            {
                _options.SetSecretRewardsViable();
            }
        }

        private void Sunsets_ChangeViability(object sender, RoutedEventArgs e)
        {
            if (MessageWindow.ShowConfirm("Any external sunsets changes made to the current configuration will be lost during the next save.\n\nDo you wish to proceed?"))
            {
                _options.SetSunsetsViable();
            }
        }

        private void Audio_ChangeViability(object sender, RoutedEventArgs e)
        {
            if (MessageWindow.ShowConfirm("Any external audio changes made to the current configuration will be lost during the next save.\n\nDo you wish to proceed?"))
            {
                _options.SetAudioViable();
            }
        }
    }
}