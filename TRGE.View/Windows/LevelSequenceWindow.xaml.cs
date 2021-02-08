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
using System.Windows.Shapes;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for LevelSequenceWindow.xaml
    /// </summary>
    public partial class LevelSequenceWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty SequencingProperty = DependencyProperty.Register
        (
            "Sequencing", typeof(List<Tuple<string, string>>), typeof(LevelSequenceWindow)
        );

        public static readonly DependencyProperty SelectionCanMoveUpProperty = DependencyProperty.Register
        (
            "CanMoveUp", typeof(bool), typeof(LevelSequenceWindow), new PropertyMetadata(false)
        );

        public static readonly DependencyProperty SelectionCanMoveDownProperty = DependencyProperty.Register
        (
            "CanMoveDown", typeof(bool), typeof(LevelSequenceWindow), new PropertyMetadata(false)
        );

        public List<Tuple<string, string>> Sequencing
        {
            get => (List<Tuple<string, string>>)GetValue(SequencingProperty);
            set => SetValue(SequencingProperty, value);
        }

        public bool CanMoveUp
        {
            get => (bool)GetValue(SelectionCanMoveUpProperty);
            set => SetValue(SelectionCanMoveUpProperty, value);
        }

        public bool CanMoveDown
        {
            get => (bool)GetValue(SelectionCanMoveDownProperty);
            set => SetValue(SelectionCanMoveDownProperty, value);
        }
        #endregion

        public LevelSequenceWindow(IReadOnlyList<Tuple<string, string>> sequencing)
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
            DataContext = this;
            Sequencing = new List<Tuple<string, string>>(sequencing);

            MinHeight = Height;
            MinWidth = Width;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.EnableMinimiseButton(this, false);
            WindowUtils.TidyMenu(this);
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateMoveStatus();
        }

        private void UpdateMoveStatus()
        {
            int selectedIndex = _listView.SelectedIndex;
            CanMoveUp = selectedIndex > 0;
            CanMoveDown = selectedIndex >= 0 && selectedIndex < Sequencing.Count - 1;
        }

        private void MoveUpButton_Click(object sender, RoutedEventArgs e)
        {
            int i = _listView.SelectedIndex;
            SwapItems(i, i - 1);
        }

        private void MoveDownButton_Click(object sender, RoutedEventArgs e)
        {
            int i = _listView.SelectedIndex;
            SwapItems(i, i + 1);
        }

        private void SwapItems(int i, int j)
        {
            List<Tuple<string, string>> sequencing = Sequencing;
            Tuple<string, string> t1 = sequencing[i];
            sequencing[i] = sequencing[j];
            sequencing[j] = t1;
            Sequencing = new List<Tuple<string, string>>(sequencing);

            _listView.SelectedIndex = j;
            UpdateMoveStatus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}