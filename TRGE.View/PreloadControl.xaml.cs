using Microsoft.WindowsAPICodePack.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace TRGE.View
{
    /// <summary>
    /// Interaction logic for PreloadControl.xaml
    /// </summary>
    public partial class PreloadControl : UserControl
    {
        public event EventHandler<DataFolderEventArgs> DataFolderOpened;

        public PreloadControl()
        {
            InitializeComponent();
            TRCoord.Instance.HistoryAdded += TRCoord_HistoryAdded;
            RefreshHistoryList();

            
            _historyIcon.Source = WindowUtils.DefaultIcons.FolderSmall.ToImageSource();

            _openButton.Click += OpenButton_Click;
        }

        private void TRCoord_HistoryAdded(object sender, TRHistoryEventArgs e)
        {
            RefreshHistoryList();
        }

        private void RefreshHistoryList()
        {
            ObservableCollection<string> items = new ObservableCollection<string>();
            _historyListView.ItemsSource = items;
            foreach (string history in TRCoord.Instance.History)
            {
                items.Add(history);
            }
            _historyBox.Visibility = items.Count > 0 ? Visibility.Visible : Visibility.Hidden;
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            OpenDataFolder();
        }

        private void HistoryListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_historyListView.SelectedItem != null)
            {
                OpenDataFolder(_historyListView.SelectedItem.ToString());
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

        public void OpenDataFolder(string folderPath)
        {
            TREditor editor = TRCoord.Instance.Open(folderPath);
            DataFolderEventArgs e = new DataFolderEventArgs(folderPath, editor);
            DataFolderOpened?.Invoke(this, e);
        }
    }
}