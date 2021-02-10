using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using TRGE.View.Model;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for BonusItemsWindow.xaml
    /// </summary>
    public partial class BonusItemsWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty SecretBonusDataProperty = DependencyProperty.Register
        (
            "SecretBonusData", typeof(GlobalSecretBonusData), typeof(BonusItemsWindow)
        );

        public static readonly DependencyProperty LevelWeaponsProperty = DependencyProperty.Register
        (
            "LevelWeapons", typeof(List<SecretBonusItem>), typeof(BonusItemsWindow)
        );

        public static readonly DependencyProperty LevelAmmoProperty = DependencyProperty.Register
        (
            "LevelAmmo", typeof(List<SecretBonusItem>), typeof(BonusItemsWindow)
        );

        public static readonly DependencyProperty LevelOtherItemsProperty = DependencyProperty.Register
        (
            "LevelOtherItems", typeof(List<SecretBonusItem>), typeof(BonusItemsWindow)
        );

        public GlobalSecretBonusData SecretBonusData
        {
            get => (GlobalSecretBonusData)GetValue(SecretBonusDataProperty);
            set => SetValue(SecretBonusDataProperty, value);
        }

        public List<SecretBonusItem> LevelWeapons
        {
            get => (List<SecretBonusItem>)GetValue(LevelWeaponsProperty);
            set => SetValue(LevelWeaponsProperty, value);
        }

        public List<SecretBonusItem> LevelAmmo
        {
            get => (List<SecretBonusItem>)GetValue(LevelAmmoProperty);
            set => SetValue(LevelAmmoProperty, value);
        }

        public List<SecretBonusItem> LevelOtherItems
        {
            get => (List<SecretBonusItem>)GetValue(LevelOtherItemsProperty);
            set => SetValue(LevelOtherItemsProperty, value);
        }
        #endregion

        private int _selectedLevelIndex;

        public BonusItemsWindow(GlobalSecretBonusData bonusData)
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
            DataContext = this;

            SecretBonusData = bonusData;
            _levelListView.SelectedIndex = 0;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.TidyMenu(this);
        }

        private void LevelListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (_levelListView.SelectedIndex == -1)
            {
                _levelListView.SelectedIndex = _selectedLevelIndex;
                return;
            }

            _selectedLevelIndex = _levelListView.SelectedIndex;
            LevelWeapons = SecretBonusData[_levelListView.SelectedIndex].BonusData.WeaponItems;
            LevelAmmo = SecretBonusData[_levelListView.SelectedIndex].BonusData.AmmoItems;
            LevelOtherItems = SecretBonusData[_levelListView.SelectedIndex].BonusData.OtherItems;
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}