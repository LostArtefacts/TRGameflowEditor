using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using TRGE.View.Model.Data;
using TRGE.View.Utils;

namespace TRGE.View.Windows;

/// <summary>
/// Interaction logic for LevelSequenceWindow.xaml
/// </summary>
public partial class LevelSequenceWindow : Window
{
    #region Dependency Properties
    public static readonly DependencyProperty SelectionCanMoveUpProperty = DependencyProperty.Register
    (
        "CanMoveUp", typeof(bool), typeof(LevelSequenceWindow), new PropertyMetadata(false)
    );

    public static readonly DependencyProperty SelectionCanMoveDownProperty = DependencyProperty.Register
    (
        "CanMoveDown", typeof(bool), typeof(LevelSequenceWindow), new PropertyMetadata(false)
    );

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

    public LevelSequencingData LevelSequencingData { get; private set; }
    private readonly ObservableCollection<SequencedLevel> _levels;

    public LevelSequenceWindow(LevelSequencingData levelSequencing)
    {
        InitializeComponent();
        Owner = WindowUtils.GetActiveWindow(this);
        DataContext = this;

        _levels = new ObservableCollection<SequencedLevel>(LevelSequencingData = levelSequencing);
        _listView.ItemsSource = _levels;
        
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
        CanMoveDown = selectedIndex >= 0 && selectedIndex < _levels.Count - 1;
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
        SequencedLevel level1 = _levels[i];
        SequencedLevel level2 = _levels[j];

        int seq1 = level1.DisplaySequence;
        int seq2 = level2.DisplaySequence;
        level1.DisplaySequence = seq2;
        level2.DisplaySequence = seq1;

        _levels[i] = level2;
        _levels[j] = level1;

        _listView.SelectedIndex = j;
        _listView.Focus();
        _listView.ScrollIntoView(_listView.SelectedItem);

        LevelSequencingData.Sort();
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}