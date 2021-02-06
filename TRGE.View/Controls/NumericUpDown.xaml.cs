using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace TRGE.View.Controls
{
    /// <summary>
    /// Interaction logic for NumericUpDown.xaml
    /// </summary>
    public partial class NumericUpDown : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register
        (
            "Value", typeof(int), typeof(NumericUpDown)
        );

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register
        (
            "MinValue", typeof(int), typeof(NumericUpDown)
        );

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register
        (
            "MaxValue", typeof(int), typeof(NumericUpDown)
        );

        public int Value
        {
            get => (int)GetValue(ValueProperty);
            set => SetValue(ValueProperty, AdjustValue(value));
        }

        public int MinValue
        {
            get => (int)GetValue(MinValueProperty);
            set
            {
                SetValue(MinValueProperty, value);
                Value = Value;
            }
        }

        public int MaxValue
        {
            get => (int)GetValue(MaxValueProperty);
            set
            {
                SetValue(MaxValueProperty, value);
                Value = Value;
            }
        }
        #endregion

        public NumericUpDown()
        {
            InitializeComponent();
            _textBox.DataContext = this;

            MinValue = 1;
            MaxValue = int.MaxValue;

            Value = 1;
        }

        private void RepeatUpButton_Click(object sender, RoutedEventArgs e)
        {
            ++Value;
        }

        private void RepeatDownButton_Click(object sender, RoutedEventArgs e)
        {
            --Value;
        }

        private void TextBox_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (!ValidateInput(_textBox.Text))
            {
                e.CancelCommand();
            }
        }

        private void TextBox_TextInput(object sender, TextCompositionEventArgs e)
        {
            //e.Handled = !int.TryParse(e.Text, out int _);
            e.Handled = !ValidateInput(e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            int v = Value;
            if (ValidateInput(_textBox.Text))
            {
                v = int.Parse(_textBox.Text);
            }
            else
            {
                e.Handled = true;
                
            }
            Value = v;
        }

        private bool ValidateInput(string text)
        {
            return int.TryParse(text, out int _);
        }

        private int AdjustValue(int value)
        {
            return Math.Min(MaxValue, Math.Max(MinValue, value));
        }
    }
}