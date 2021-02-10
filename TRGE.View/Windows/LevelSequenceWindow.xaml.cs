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
using TRGE.Core;
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
            "Sequencing", typeof(List<MutableTuple<int, string, string>>), typeof(LevelSequenceWindow)
        );

        public static readonly DependencyProperty SelectionCanMoveUpProperty = DependencyProperty.Register
        (
            "CanMoveUp", typeof(bool), typeof(LevelSequenceWindow), new PropertyMetadata(false)
        );

        public static readonly DependencyProperty SelectionCanMoveDownProperty = DependencyProperty.Register
        (
            "CanMoveDown", typeof(bool), typeof(LevelSequenceWindow), new PropertyMetadata(false)
        );

        public List<MutableTuple<int, string, string>> Sequencing
        {
            get => (List<MutableTuple<int, string, string>>)GetValue(SequencingProperty);
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

            List<MutableTuple<int, string, string>> indexedSquencing = new List<MutableTuple<int, string, string>>();
            for (int i = 0; i < sequencing.Count; i++)
            {
                indexedSquencing.Add(new MutableTuple<int, string, string>(i + 1, sequencing[i].Item1, sequencing[i].Item2));
            }
            Sequencing = indexedSquencing;

            MinHeight = Height;
            MinWidth = Width;
        }

        public IReadOnlyList<Tuple<string, string>> GetSequencing()
        {
            List<Tuple<string, string>> result = new List<Tuple<string, string>>();
            foreach (MutableTuple<int, string, string> data in Sequencing)
            {
                result.Add(new Tuple<string, string>(data.Item2, data.Item3));
            }
            return result;
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
            List<MutableTuple<int, string, string>> sequencing = Sequencing;
            MutableTuple<int, string, string> t1 = sequencing[i];
            int seq1 = t1.Item1;
            int seq2 = sequencing[j].Item1;

            sequencing[i] = sequencing[j];
            sequencing[j] = t1;

            sequencing[i].Item1 = seq1;
            sequencing[j].Item1 = seq2;

            Sequencing = new List<MutableTuple<int, string, string>>(sequencing);

            _listView.SelectedIndex = j;
            _listView.Focus();
            _listView.ScrollIntoView(_listView.SelectedItem);
            UpdateMoveStatus();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}