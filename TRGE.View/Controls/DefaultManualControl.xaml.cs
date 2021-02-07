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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace TRGE.View.Controls
{
    /// <summary>
    /// Interaction logic for DefaultManualControl.xaml
    /// </summary>
    public partial class DefaultManualControl : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty TextProperty = DependencyProperty.Register
        (
            "Text", typeof(string), typeof(DefaultManualControl)
        );

        public static readonly DependencyProperty DefaultLabelProperty = DependencyProperty.Register
        (
            "DefaultLabel", typeof(string), typeof(DefaultManualControl), new PropertyMetadata("Use the default configuration")
        );

        public static readonly DependencyProperty ManualLabelProperty = DependencyProperty.Register
        (
            "ManualLabel", typeof(string), typeof(DefaultManualControl), new PropertyMetadata("Configure manually")
        );

        public static readonly DependencyProperty IsDefaultProperty = DependencyProperty.Register
        (
            "IsDefault", typeof(bool), typeof(DefaultManualControl), new PropertyMetadata(true)
        );

        public static readonly DependencyProperty IsManualProperty = DependencyProperty.Register
        (
            "IsManual", typeof(bool), typeof(DefaultManualControl), new PropertyMetadata(false)
        );

        public static readonly DependencyProperty ManualButtonTextProperty = DependencyProperty.Register
        (
            "ManualButtonText", typeof(string), typeof(DefaultManualControl), new PropertyMetadata("Organise")
        );

        public string Text
        {
            get => (string)GetValue(TextProperty);
            set => SetValue(TextProperty, value);
        }

        public string DefaultLabel
        {
            get => (string)GetValue(DefaultLabelProperty);
            set => SetValue(DefaultLabelProperty, value);
        }

        public string ManualLabel
        {
            get => (string)GetValue(ManualLabelProperty);
            set => SetValue(ManualLabelProperty, value);
        }

        public bool IsDefault
        {
            get => (bool)GetValue(IsDefaultProperty);
            set => SetValue(IsDefaultProperty, value);
        }

        public bool IsManual
        {
            get => (bool)GetValue(IsManualProperty);
            set => SetValue(IsManualProperty, value);
        }
        #endregion

        public DefaultManualControl()
        {
            InitializeComponent();
            _content.DataContext = this;
        }
    }
}
