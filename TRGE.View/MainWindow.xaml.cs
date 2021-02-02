using System;
using System.Collections.Generic;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            TRCoord.Instance.HistoryAdded += TRCoord_HistoryAdded;
            _preloadControl.DataFolderOpened += PreloadControl_DataFolderOpened;
            RefreshHistoryMenu();
        }

        private void TRCoord_HistoryAdded(object sender, TRHistoryEventArgs e)
        {
            RefreshHistoryMenu();
        }

        private void RefreshHistoryMenu()
        {
            _historyMenu.Items.Clear();
            for (int i = 0; i < TRCoord.Instance.History.Count; i++)
            {
                string folder = TRCoord.Instance.History[i];
                MenuItem menuItem = new MenuItem
                {
                    Tag = folder,
                    Header = (i + 1) + ". " + folder
                };
                menuItem.Click += RecentFolderMenuItem_Click;
                _historyMenu.Items.Add(menuItem);
            }
            _historyMenu.IsEnabled = _historyMenu.Items.Count > 0;
        }

        private void RecentFolderMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _preloadControl.OpenDataFolder((sender as MenuItem).Tag.ToString());
        }

        private void OpenMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _preloadControl.OpenDataFolder();
        }

        private void CloseMenuItem_Click(object sender, RoutedEventArgs e)
        {
            _editorControl.Visibility = Visibility.Hidden;
            _preloadControl.Visibility = Visibility.Visible;

            _editorControl.Unload();
        }

        private void PreloadControl_DataFolderOpened(object sender, DataFolderEventArgs e)
        {
            _preloadControl.Visibility = Visibility.Hidden;
            _editorControl.Visibility = Visibility.Visible;
            _mainStatusText.Text = e.DataFolder;

            _editorControl.Load(e);
        }

        private void ExitMenuItem_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}