using System.Windows;
using TRGE.View.Model.Data;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for AmmolessLevelsWindow.xaml
    /// </summary>
    public partial class AmmolessLevelsWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty LevelDataProperty = DependencyProperty.Register
        (
            "LevelData", typeof(FlaggedLevelData), typeof(AmmolessLevelsWindow)
        );

        public FlaggedLevelData LevelData
        {
            get => (FlaggedLevelData)GetValue(LevelDataProperty);
            set => SetValue(LevelDataProperty, value);
        }
        #endregion

        public AmmolessLevelsWindow(FlaggedLevelData levelData)
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
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
}