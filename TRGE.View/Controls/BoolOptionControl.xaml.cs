using System.Windows;
using System.Windows.Controls;

namespace TRGE.View.Controls
{
    /// <summary>
    /// Interaction logic for GeneralOptionControl.xaml
    /// </summary>
    public partial class BoolOptionControl : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register
        (
            "Title", typeof(string), typeof(BoolOptionControl)
        );

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
        (
            "Text", typeof(string), typeof(BoolOptionControl)
        );

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register
        (
            "IsActive", typeof(bool), typeof(BoolOptionControl)
        );

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public bool IsActive
        {
            get => (bool)GetValue(IsActiveProperty);
            set => SetValue(IsActiveProperty, value);
        }
        #endregion

        public BoolOptionControl()
        {
            InitializeComponent();
            _content.DataContext = this;
        }
    }
}