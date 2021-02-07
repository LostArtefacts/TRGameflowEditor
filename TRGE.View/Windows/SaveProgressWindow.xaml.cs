using System;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using TRGE.Coord;
using TRGE.Core;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for SaveProgressWindow.xaml
    /// </summary>
    public partial class SaveProgressWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register
        (
            "ProgressValue", typeof(int), typeof(SaveProgressWindow), new PropertyMetadata(0)
        );

        public static readonly DependencyProperty ProgressTargetProperty = DependencyProperty.Register
        (
            "ProgressTarget", typeof(int), typeof(SaveProgressWindow), new PropertyMetadata(100)
        );

        public static readonly DependencyProperty ProgressDescriptionProperty = DependencyProperty.Register
        (
            "ProgressDescription", typeof(string), typeof(SaveProgressWindow), new PropertyMetadata("Saving - please wait")
        );

        public int ProgressValue
        {
            get => (int)GetValue(ProgressValueProperty);
            set => SetValue(ProgressValueProperty, value);
        }

        public int ProgressTarget
        {
            get => (int)GetValue(ProgressTargetProperty);
            set => SetValue(ProgressTargetProperty, value);
        }

        public string ProgressDescription
        {
            get => (string)GetValue(ProgressDescriptionProperty);
            set => SetValue(ProgressDescriptionProperty, value);
        }
        #endregion

        private readonly TREditor _editor;

        public Exception SaveException { get; private set; }

        public SaveProgressWindow(TREditor editor)
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
            DataContext = this;
            _editor = editor;
            _editor.SaveProgressChanged += Editor_SaveProgressChanged;
        }

        private void Editor_SaveProgressChanged(object sender, TRSaveEventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                ProgressTarget = e.ProgressTarget;
                ProgressValue = e.ProgressValue;
                ProgressDescription = e.ProgressDescription;
            });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.EnableCloseButton(this, false);
            WindowUtils.TidyMenu(this);

            new Thread(Save).Start();
        }

        private void Save()
        {
            try
            {
                _editor.Save();
            }
            catch (Exception e)
            {
                SaveException = e;
            }
            finally
            {
                _editor.SaveProgressChanged -= Editor_SaveProgressChanged;
                Dispatcher.Invoke(delegate
                {
                    WindowUtils.EnableCloseButton(this, true);
                    Close();
                });
            }
        }
    }
}