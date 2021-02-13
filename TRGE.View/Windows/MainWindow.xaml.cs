using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using TRGE.Coord;
using TRGE.View.Model;
using TRGE.View.Updates;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, IRecentFolderOpener
    {
        #region Dependency Properties
        public static readonly DependencyProperty IsEditorActiveProperty = DependencyProperty.Register
        (
            "IsEditorActive", typeof(bool), typeof(MainWindow)
        );

        public static readonly DependencyProperty IsEditorDirtyProperty = DependencyProperty.Register
        (
            "IsEditorDirty", typeof(bool), typeof(MainWindow)
        );

        public static readonly DependencyProperty EditorCanExportProperty = DependencyProperty.Register
        (
            "EditorCanExport", typeof(bool), typeof(MainWindow)
        );

        public static readonly DependencyProperty CanEmptyRecentFoldersProperty = DependencyProperty.Register
        (
            "CanEmptyRecentFolders", typeof(bool), typeof(MainWindow)
        );

        public static readonly DependencyProperty FolderControlVisibilityProperty = DependencyProperty.Register
        (
            "FolderControlVisibility", typeof(Visibility), typeof(MainWindow)
        );

        public static readonly DependencyProperty EditorControlVisibilityProperty = DependencyProperty.Register
        (
            "EditorControlVisibility", typeof(Visibility), typeof(MainWindow)
        );

        public static readonly DependencyProperty EditorStatusVisibilityProperty = DependencyProperty.Register
        (
            "EditorStatusVisibility", typeof(Visibility), typeof(MainWindow)
        );

        public static readonly DependencyProperty EditorSavedStatusVisibilityProperty = DependencyProperty.Register
        (
            "EditorSavedStatusVisibility", typeof(Visibility), typeof(MainWindow)
        );

        public static readonly DependencyProperty EditorUnsavedStatusVisibilityProperty = DependencyProperty.Register
        (
            "EditorUnsavedStatusVisibility", typeof(Visibility), typeof(MainWindow)
        );

        public static readonly DependencyProperty RecentFoldersProperty = DependencyProperty.Register
        (
            "RecentFolders", typeof(RecentFolderList), typeof(MainWindow)
        );

        public static readonly DependencyProperty HasRecentFoldersProperty = DependencyProperty.Register
        (
            "HasRecentFolders", typeof(bool), typeof(MainWindow)
        );

        public bool IsEditorActive
        {
            get => (bool)GetValue(IsEditorActiveProperty);
            set
            {
                SetValue(IsEditorActiveProperty, value);
                FolderControlVisibility = value ? Visibility.Collapsed : Visibility.Visible;
                EditorControlVisibility = value ? Visibility.Visible : Visibility.Collapsed;
                EditorStatusVisibility = value ? Visibility.Visible : Visibility.Hidden;
                CanEmptyRecentFolders = !value;
            }
        }

        public bool IsEditorDirty
        {
            get => (bool)GetValue(IsEditorDirtyProperty);
            set
            {
                SetValue(IsEditorDirtyProperty, value);
                EditorSavedStatusVisibility = !value ? Visibility.Visible : Visibility.Collapsed;
                EditorUnsavedStatusVisibility = value ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public bool EditorCanExport
        {
            get => (bool)GetValue(EditorCanExportProperty);
            set => SetValue(EditorCanExportProperty, value);
        }

        public Visibility EditorControlVisibility
        {
            get => (Visibility)GetValue(EditorControlVisibilityProperty);
            set => SetValue(EditorControlVisibilityProperty, value);
        }

        public Visibility EditorStatusVisibility
        {
            get => (Visibility)GetValue(EditorStatusVisibilityProperty);
            set => SetValue(EditorStatusVisibilityProperty, value);
        }

        public Visibility EditorSavedStatusVisibility
        {
            get => (Visibility)GetValue(EditorSavedStatusVisibilityProperty);
            set => SetValue(EditorSavedStatusVisibilityProperty, value);
        }

        public Visibility EditorUnsavedStatusVisibility
        {
            get => (Visibility)GetValue(EditorUnsavedStatusVisibilityProperty);
            set => SetValue(EditorUnsavedStatusVisibilityProperty, value);
        }

        public bool CanEmptyRecentFolders
        {
            get => (bool)GetValue(CanEmptyRecentFoldersProperty);
            set => SetValue(CanEmptyRecentFoldersProperty, HasRecentFolders && value);
        }

        public Visibility FolderControlVisibility
        {
            get => (Visibility)GetValue(FolderControlVisibilityProperty);
            set => SetValue(FolderControlVisibilityProperty, value);
        }

        public RecentFolderList RecentFolders
        {
            get => (RecentFolderList)GetValue(RecentFoldersProperty);
            set
            {
                SetValue(RecentFoldersProperty, value);
                HasRecentFolders = !RecentFolders.IsEmpty;
                _folderControl.RecentFolders = RecentFolders;
            }
        }

        public bool HasRecentFolders
        {
            get => (bool)GetValue(HasRecentFoldersProperty);
            set => SetValue(HasRecentFoldersProperty, value);
        }
        #endregion

        public MainWindow()
        {
            InitializeComponent();
            DataContext = this;

            TRCoord.Instance.HistoryAdded += TRCoord_HistoryAdded;
            TRCoord.Instance.HistoryChanged += TRCoord_HistoryChanged;

            _folderControl.DataFolderOpened += FolderControl_DataFolderOpened;
            _editorControl.EditorStateChanged += EditorControl_EditorStateChanged;
            RefreshHistoryMenu();

            _editionStatusText.DataContext = _folderStatusText.DataContext = _editorControl;
            IsEditorActive = false;

            UpdateChecker.Instance.UpdateAvailable += UpdateChecker_UpdateAvailable;

            MinHeight = Height;
            MinWidth = Width;
        }

        #region History Updates
        private void TRCoord_HistoryChanged(object sender, EventArgs e)
        {
            RefreshHistoryMenu();
        }
        
        private void TRCoord_HistoryAdded(object sender, TRHistoryEventArgs e)
        {
            RefreshHistoryMenu();
        }

        private void RefreshHistoryMenu()
        {
            RecentFolders = new RecentFolderList(this);
        }

        public void OpenDataFolder(RecentFolder folder)
        {
            if (ConfirmEditorSaveState())
            {
                _folderControl.OpenDataFolder(folder);
            }
        }

        private void EmptyRecentFoldersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _folderControl.EmptyRecentFolders();
        }
        #endregion

        #region Open/Save/Close

        private void OpenCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ConfirmEditorSaveState())
            {
                _folderControl.OpenDataFolder();
            }
        }

        private void SaveCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            _editorControl.Save();
        }

        private void CloseCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            if (ConfirmEditorSaveState())
            {
                _editorControl.Unload();
                IsEditorActive = false;
            }
        }

        private void FolderControl_DataFolderOpened(object sender, DataFolderEventArgs e)
        {
            _editorControl.Load(e);
            IsEditorActive = true;
        }

        private void EditorControl_EditorStateChanged(object sender, EditorEventArgs e)
        {
            IsEditorDirty = e.IsDirty;
            EditorCanExport = e.CanExport;
        }
        #endregion

        #region Edit Options
        private void ShowBackupMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _editorControl.OpenBackupFolder();
        }

        private void RestoreDefaultsMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _editorControl.RestoreDefaults();
        }

        private void ImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _editorControl.ImportSettings();
        }

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (ConfirmEditorSaveState())
            {
                _editorControl.ExportSettings();
            }
        }        

        private void EditorFolder_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
        #endregion

        #region Help Options
        private void HelpCommandBinding_Executed(object sender, ExecutedRoutedEventArgs e)
        {
            Process.Start("https://github.com/lahm86/TRGameflowEditor");
        }

        private void DiscordMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.com/channels/183942718630658048/738175962033684510");
        }

        private void UpdatesMenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (UpdateChecker.Instance.CheckForUpdates())
                {
                    ShowUpdateWindow();
                }
                else
                {
                    MessageWindow.ShowMessage("The current version of TRGE is up to date.");
                }
            }
            catch (Exception ex)
            {
                MessageWindow.ShowError(ex.Message);
            }
        }

        private void UpdateChecker_UpdateAvailable(object sender, UpdateEventArgs e)
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(() => UpdateChecker_UpdateAvailable(sender, e));
            }
            else
            {
                _updateAvailableMenu.Visibility = Visibility.Visible;
            }
        }

        private void UpdateAvailableMenuItem_Click(object sender, RoutedEventArgs e)
        {
            ShowUpdateWindow();
        }

        private void ShowUpdateWindow()
        {
            new UpdateAvailableWindow().ShowDialog();
        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow().ShowDialog();
        }
        #endregion

        #region Exiting
        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!ConfirmEditorSaveState())
            {
                e.Cancel = true;
            }
        }

        private bool ConfirmEditorSaveState()
        {
            if (IsEditorDirty)
            {
                switch (MessageWindow.ShowConfirmCancel("Do you want to save the changes you have made?"))
                {
                    case MessageBoxResult.Yes:
                        _editorControl.Save();
                        break;
                    case MessageBoxResult.Cancel:
                        return false;
                }
            }
            return true;
        }
        #endregion
    }
}