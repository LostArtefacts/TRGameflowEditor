using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TRGE.Coord;
using TRGE.Core;
using TRGE.View.Model;
using TRGE.View.Utils;
using TRGE.View.Windows;
using SWF = System.Windows.Forms;

namespace TRGE.View.Controls
{
    /// <summary>
    /// Interaction logic for FolderLoadControl.xaml
    /// </summary>
    public partial class FolderLoadControl : UserControl, IRecentFolderOpener
    {
        #region Dependency Properties
        public static readonly DependencyProperty RecentFoldersProperty = DependencyProperty.Register
        (
            "RecentFolders", typeof(RecentFolderList), typeof(FolderLoadControl)
        );

        public static readonly DependencyProperty RecentFoldersVisibilityProperty = DependencyProperty.Register
        (
            "RecentFoldersVisibility", typeof(Visibility), typeof(FolderLoadControl)
        );

        public RecentFolderList RecentFolders
        {
            get => (RecentFolderList)GetValue(RecentFoldersProperty);
            set
            {
                SetValue(RecentFoldersProperty, value);
                RecentFoldersVisibility = RecentFolders.IsEmpty ? Visibility.Collapsed : Visibility.Visible;
            }
        }

        public Visibility RecentFoldersVisibility
        {
            get => (Visibility)GetValue(RecentFoldersVisibilityProperty);
            set => SetValue(RecentFoldersVisibilityProperty, value);
        }
        #endregion

        public event EventHandler<DataFolderEventArgs> DataFolderOpened;

        public FolderLoadControl()
        {
            InitializeComponent();
            _historyBox.DataContext = this;
            _historyIcon.Source = ControlUtils.DefaultIcons.FolderSmall.ToImageSource();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenDataFolder();
        }

        private void HistoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_historyListView.SelectedItem != null)
            {
                OpenDataFolder(_historyListView.SelectedItem as RecentFolder);
                _historyListView.UnselectAll();
            }
        }

        private void HistoryListView_MouseMove(object sender, MouseEventArgs e)
        {
            ListViewItem item = _historyListView.GetItemAt(e.GetPosition(_historyListView));
            _historyListView.Cursor = item == null ? Cursors.Arrow : Cursors.Hand;
        }

        public void OpenDataFolder()
        {
            using SWF.FolderBrowserDialog dlg = new()
            {
                Description = "TRGE : Select Data Folder",
                UseDescriptionForTitle = true,
            };
            if (dlg.ShowDialog() == SWF.DialogResult.OK)
            {
                OpenDataFolder(dlg.SelectedPath);
            }
        }

        public void OpenDataFolder(RecentFolder folder)
        {
            OpenDataFolder(folder.FolderPath);
        }

        public void OpenDataFolder(string folderPath, TRScriptOpenOption openOption = TRScriptOpenOption.Default)
        {
            OpenProgressWindow opw = new OpenProgressWindow(folderPath, openOption);
            try
            {
                if (opw.ShowDialog() ?? false)
                {
                    DataFolderOpened?.Invoke(this, new DataFolderEventArgs(folderPath, opw.OpenedEditor));
                }
                else if (opw.OpenException != null)
                {
                    throw opw.OpenException;
                }
            }
            catch (ChecksumMismatchException)
            {
                if (openOption != TRScriptOpenOption.Default)
                {
                    throw opw.OpenException;
                }

                HandleChecksumMismatch(folderPath);
            }
            catch (Exception e)
            {
                MessageWindow.ShowError(e.Message);
            }
        }

        private void HandleChecksumMismatch(string folderPath)
        {
            TRScriptOpenOption option = TRScriptOpenOption.Default;
            ScriptOptionWindow sow = new ScriptOptionWindow();
            if (sow.ShowDialog() ?? false)
            {
                option = sow.Option;
            }
            
            if (option != TRScriptOpenOption.Default)
            {
                OpenDataFolder(folderPath, option);
            }
        }

        public void EmptyRecentFolders()
        {
            string msg = string.Format
            (
                "All backup files and edit configuration files will be removed from the directory below. Make sure to backup any files you want to keep.\n\n{0}\n\nDo you wish to proceed?",
                TRCoord.Instance.ConfigDirectory
            );

            if (MessageWindow.ShowConfirm(msg))
            {
                try
                {
                    TRCoord.Instance.ClearHistory();
                }
                catch (Exception e)
                {
                    MessageWindow.ShowError(e.Message);
                }
            }
        }
    }
}