using System.Windows;
using TRGE.View.Model.Data;
using TRGE.View.Utils;

namespace TRGE.View.Windows;

/// <summary>
/// Interaction logic for SunsetLevelsWindow.xaml
/// </summary>
public partial class SunsetLevelsWindow : Window
{
    #region Dependency Properties
    public static readonly DependencyProperty LevelDataProperty = DependencyProperty.Register
    (
        "LevelData", typeof(FlaggedLevelData), typeof(SunsetLevelsWindow)
    );

    public FlaggedLevelData LevelData
    {
        get => (FlaggedLevelData)GetValue(LevelDataProperty);
        set => SetValue(LevelDataProperty, value);
    }
    #endregion

    public SunsetLevelsWindow(FlaggedLevelData levelData)
    {
        InitializeComponent();
        Owner = WindowUtils.GetActiveWindow(this);
        DataContext = this;

        LevelData = levelData;

        MinHeight = Height;
        MinWidth = Width;
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
    {
        WindowUtils.EnableMinimiseButton(this, false);
        WindowUtils.TidyMenu(this);
    }

    private void OkButton_Click(object sender, RoutedEventArgs e)
    {
        DialogResult = true;
    }
}