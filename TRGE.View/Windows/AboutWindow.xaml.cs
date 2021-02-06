using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Navigation;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for AboutWindow.xaml
    /// </summary>
    public partial class AboutWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty AppTitleProperty = DependencyProperty.Register
        (
            "AppTitle", typeof(string), typeof(AboutWindow)
        );

        public static readonly DependencyProperty VersionProperty = DependencyProperty.Register
        (
            "Version", typeof(string), typeof(AboutWindow)
        );

        public static readonly DependencyProperty CopyrightProperty = DependencyProperty.Register
        (
            "Copyright", typeof(string), typeof(AboutWindow)
        );

        public string AppTitle
        {
            get => (string)GetValue(AppTitleProperty);
            private set => SetValue(AppTitleProperty, value);
        }

        public string Version
        {
            get => (string)GetValue(VersionProperty);
            private set => SetValue(VersionProperty, value);
        }

        public string Copyright
        {
            get => (string)GetValue(CopyrightProperty);
            private set => SetValue(CopyrightProperty, value);
        }
        #endregion

        public AboutWindow()
        {
            InitializeComponent();
            DataContext = this;

            Assembly assembly = Assembly.GetExecutingAssembly();
            object[] attributes = assembly.GetCustomAttributes(typeof(AssemblyTitleAttribute), false);
            if (attributes.Length > 0)
            {
                AppTitle = ((AssemblyTitleAttribute)attributes[0]).Title;
            }
            else
            {
                AppTitle = Path.GetFileNameWithoutExtension(assembly.CodeBase);
            }

            Version v = assembly.GetName().Version;
            Version = string.Format("{0}.{1}.{2}", v.Major, v.Minor, v.Build);

            attributes = assembly.GetCustomAttributes(typeof(AssemblyCopyrightAttribute), false);
            if (attributes.Length > 0)
            {
                Copyright = ((AssemblyCopyrightAttribute)attributes[0]).Copyright;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.TidyMenu(this);
        }

        private void Hyperlink_RequestNavigate(object sender, RequestNavigateEventArgs e)
        {
            Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}