﻿using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using TRGE.Coord;
using TRGE.View.Model;
using TRGE.View.Utils;

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
            using (CommonOpenFileDialog dlg = new CommonOpenFileDialog())
            {
                dlg.IsFolderPicker = true;
                dlg.Title = "TRGE : Select Data Folder";
                if (dlg.ShowDialog(WindowUtils.GetActiveWindowHandle()) == CommonFileDialogResult.Ok)
                {
                    OpenDataFolder(dlg.FileName);
                }
            }
        }

        public void OpenDataFolder(RecentFolder folder)
        {
            OpenDataFolder(folder.FolderPath);
        }

        public void OpenDataFolder(string folderPath)
        {
            try
            {
                WindowUtils.GetActiveWindow().Cursor = Cursors.Wait;

                TREditor editor = TRCoord.Instance.Open(folderPath);
                DataFolderEventArgs e = new DataFolderEventArgs(folderPath, editor);
                DataFolderOpened?.Invoke(this, e);
            }
            catch (Exception e)
            {
                WindowUtils.ShowError(e.Message);
            }
            finally
            {
                WindowUtils.GetActiveWindow().Cursor = Cursors.Arrow;
            }
        }

        public void EmptyRecentFolders()
        {
            string msg = string.Format
            (
                "All backup files and edit configuration files will be removed from the directory below. Make sure to backup any files you want to keep.\n\n{0}\n\nDo you wish to proceed?",
                TRCoord.Instance.ConfigDirectory
            );

            if (WindowUtils.ShowConfirm(msg))
            {
                try
                {
                    TRCoord.Instance.ClearHistory();
                }
                catch (Exception e)
                {
                    WindowUtils.ShowError(e.Message);
                }
            }
        }
    }
}