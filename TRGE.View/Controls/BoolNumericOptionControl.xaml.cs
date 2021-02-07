using System.Windows;
using System.Windows.Controls;

namespace TRGE.View.Controls
{
    /// <summary>
    /// Interaction logic for BoolNumericOptionControl.xaml
    /// </summary>
    public partial class BoolNumericOptionControl : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty TitleProperty = DependencyProperty.Register
        (
            "Title", typeof(string), typeof(BoolNumericOptionControl)
        );

        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
        (
            "Text", typeof(string), typeof(BoolNumericOptionControl)
        );

        public static readonly DependencyProperty IsActiveProperty = DependencyProperty.Register
        (
            "IsActive", typeof(bool), typeof(BoolNumericOptionControl)
        );

        public static readonly DependencyProperty NumericTitleProperty = DependencyProperty.Register
        (
            "NumericTitle", typeof(string), typeof(BoolNumericOptionControl)
        );

        public static readonly DependencyProperty NumericTextProperty = DependencyProperty.Register
        (
            "NumericText", typeof(string), typeof(BoolNumericOptionControl)
        );

        public static readonly DependencyProperty NumericValueProperty = DependencyProperty.Register
        (
            "NumericValue", typeof(int), typeof(BoolNumericOptionControl)
        );

        public static readonly DependencyProperty NumericMinValueProperty = DependencyProperty.Register
        (
            "NumericMinValue", typeof(int), typeof(BoolNumericOptionControl), new PropertyMetadata(1)
        );

        public static readonly DependencyProperty NumericMaxValueProperty = DependencyProperty.Register
        (
            "NumericMaxValue", typeof(int), typeof(BoolNumericOptionControl), new PropertyMetadata(int.MaxValue)
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

        public string NumericTitle
        {
            get => (string)GetValue(NumericTitleProperty);
            set => SetValue(NumericTitleProperty, value);
        }

        public string NumericText
        {
            get => (string)GetValue(NumericTextProperty);
            set => SetValue(NumericTextProperty, value);
        }

        public int NumericValue
        {
            get => (int)GetValue(NumericMinValueProperty);
            set => SetValue(NumericMinValueProperty, value);
        }

        public int NumericMinValue
        {
            get => (int)GetValue(NumericMinValueProperty);
            set => SetValue(NumericMinValueProperty, value);
        }

        public int NumericMaxValue
        {
            get => (int)GetValue(NumericMaxValueProperty);
            set => SetValue(NumericMaxValueProperty, value);
        }
        #endregion

        public BoolNumericOptionControl()
        {
            InitializeComponent();
            _content.DataContext = this;
        }
    }
}