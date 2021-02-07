using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
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
        private bool _dirty;

        public event EventHandler<EditorEventArgs> EditorStateChanged;

        public TREditor Editor { get; private set; }

        public EditorControl()
        {
            InitializeComponent();
            _editorGrid.DataContext = _options = new EditorOptions();
            _options.PropertyChanged += Editor_PropertyChanged;
            _dirty = false;
        }

        private void Editor_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            _dirty = true;
            FireEditorStateChanged();
        }

        private void FireEditorStateChanged()
        {
            EditorStateChanged?.Invoke(this, new EditorEventArgs { IsDirty = _dirty });
        }

        public void Load(DataFolderEventArgs e)
        {
            Editor = e.Editor;
            Edition = Editor.Edition.Title;
            DataFolder = e.DataFolder;

            _options.Load(Editor.ScriptEditor as TR23ScriptEditor);

            _dirty = false;
            FireEditorStateChanged();
        }

        public void Save()
        {
            _options.Save(Editor.ScriptEditor as TR23ScriptEditor);

            SaveProgressWindow spw = new SaveProgressWindow(Editor);
            spw.ShowDialog();
            if (spw.SaveException != null)
            {
                WindowUtils.ShowError(spw.SaveException.Message);
            }
            else
            {
                _dirty = false;
                FireEditorStateChanged();
            }
        }

        public void Unload()
        {
            Editor = null;
        }

        public void OpenBackupFolder()
        {
            Process.Start("explorer.exe", Editor.BackupDirectory);
        }

        public void RestoreDefaults()
        {
            if (WindowUtils.ShowConfirm("The files that were backed up when this folder was first opened will be copied back to the original directory.\n\nDo you wish to proceed?"))
            {
                Editor.Restore();
            }
        }

        public void ExportSettings()
        {

        }

        public void ImportSettings()
        {

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            new AudioWindow().ShowDialog();
        }
    }
}