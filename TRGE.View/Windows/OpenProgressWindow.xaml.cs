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
    /// Interaction logic for BackupWindow.xaml
    /// </summary>
    public partial class OpenProgressWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register
        (
            "ProgressValue", typeof(int), typeof(OpenProgressWindow), new PropertyMetadata(0)
        );

        public static readonly DependencyProperty ProgressTargetProperty = DependencyProperty.Register
        (
            "ProgressTarget", typeof(int), typeof(OpenProgressWindow), new PropertyMetadata(100)
        );

        public static readonly DependencyProperty ProgressDescriptionProperty = DependencyProperty.Register
        (
            "ProgressDescription", typeof(string), typeof(OpenProgressWindow), new PropertyMetadata("Checking backup status")
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

        private volatile bool _complete;
        private readonly string _folderPath;
        private readonly TRScriptOpenOption _openOption;

        public Exception OpenException { get; private set; }
        public TREditor OpenedEditor { get; private set; }

        public OpenProgressWindow(string folderPath, TRScriptOpenOption openOption)
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
            DataContext = this;
            _complete = false;
            _folderPath = folderPath;
            _openOption = openOption;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.EnableCloseButton(this, false);
            WindowUtils.TidyMenu(this);
            new Thread(Open).Start();
        }

        private void Open()
        {
            TRCoord.Instance.BackupProgressChanged += TRCoord_BackupProgressChanged;

            try
            {
                OpenedEditor = TRCoord.Instance.Open(_folderPath, _openOption);
            }
            catch (Exception e)
            {
                OpenException = e;
            }
            finally
            {
                TRCoord.Instance.BackupProgressChanged -= TRCoord_BackupProgressChanged;

                Dispatcher.Invoke(delegate
                {
                    _complete = true;
                    WindowUtils.EnableCloseButton(this, true);
                    DialogResult = OpenException == null;
                });
            }
        }

        private void TRCoord_BackupProgressChanged(object sender, TRBackupRestoreEventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                ProgressTarget = e.ProgressTarget;
                ProgressValue = e.ProgressValue;
            });
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_complete)
            {
                e.Cancel = true;
            }
        }
    }
}