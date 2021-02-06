using System;
using System.Collections.Generic;
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
using System.Windows.Shapes;
using TRGE.Coord;
using TRGE.View.Model;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty IsEditorActiveProperty = DependencyProperty.Register
        (
            "IsEditorActive", typeof(bool), typeof(MainWindow)
        );

        public static readonly DependencyProperty FolderControlVisibilityProperty = DependencyProperty.Register
        (
            "FolderControlVisibility", typeof(Visibility), typeof(MainWindow)
        );

        public static readonly DependencyProperty EditorControlVisibilityProperty = DependencyProperty.Register
        (
            "EditorControlVisibility", typeof(Visibility), typeof(MainWindow)
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
            }
        }

        public Visibility EditorControlVisibility
        {
            get => (Visibility)GetValue(EditorControlVisibilityProperty);
            set => SetValue(EditorControlVisibilityProperty, value);
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

            _folderControl.DataFolderOpened += PreloadControl_DataFolderOpened;
            RefreshHistoryMenu();
            IsEditorActive = false;
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
            RecentFolders = new RecentFolderList(_folderControl);
        }

        private void EmptyRecentFoldersMenuItem_Click(object sender, RoutedEventArgs e)
        {
            if (WindowUtils.ShowConfirm("Are you sure you want to clear the list of recent folders?"))
            {
                TRCoord.Instance.ClearHistory();
            }
        }
        #endregion

        #region Open/Close Folders
        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _folderControl.OpenDataFolder();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _editorControl.Unload();
            IsEditorActive = false;
        }

        private void PreloadControl_DataFolderOpened(object sender, DataFolderEventArgs e)
        {
            _mainStatusText.Text = e.DataFolder;

            _editorControl.Load(e);
            IsEditorActive = true;
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

        private void ExportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _editorControl.ExportSettings();
        }

        private void ImportMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _editorControl.ImportSettings();
        }
        #endregion

        #region Help Options
        private void GitHubMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://github.com/lahm86/TRGameflowEditor");
        }

        private void DiscordMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Process.Start("https://discord.com/channels/183942718630658048/738175962033684510");
        }

        private void UpdatesMenuItem_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AboutMenuItem_Click(object sender, RoutedEventArgs e)
        {
            new AboutWindow
            {
                Owner = this,
                ShowInTaskbar = false
            }.ShowDialog();
        }
        #endregion

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}