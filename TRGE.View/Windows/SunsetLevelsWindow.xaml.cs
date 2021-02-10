using System.Collections.Generic;
using System.Windows;
using TRGE.Core;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for SunsetLevelsWindow.xaml
    /// </summary>
    public partial class SunsetLevelsWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty LevelDataProperty = DependencyProperty.Register
        (
            "LevelData", typeof(List<MutableTuple<string, string, bool>>), typeof(SunsetLevelsWindow)
        );

        public List<MutableTuple<string, string, bool>> LevelData
        {
            get => (List<MutableTuple<string, string, bool>>)GetValue(LevelDataProperty);
            set => SetValue(LevelDataProperty, value);
        }
        #endregion

        public SunsetLevelsWindow(IReadOnlyList<MutableTuple<string, string, bool>> levelData)
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
            DataContext = this;

            LevelData = new List<MutableTuple<string, string, bool>>(levelData);

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