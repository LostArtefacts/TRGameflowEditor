using System.ComponentModel;
using System.Windows;
using TRGE.Core;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for DownloadingWindow.xaml
    /// </summary>
    public partial class DownloadingWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty ProgressValueProperty = DependencyProperty.Register
        (
            "ProgressValue", typeof(int), typeof(DownloadingWindow), new PropertyMetadata(0)
        );

        public static readonly DependencyProperty ProgressTargetProperty = DependencyProperty.Register
        (
            "ProgressTarget", typeof(int), typeof(DownloadingWindow), new PropertyMetadata(100)
        );

        public static readonly DependencyProperty ProgressDescriptionProperty = DependencyProperty.Register
        (
            "ProgressDescription", typeof(string), typeof(DownloadingWindow), new PropertyMetadata("Downloading - please wait")
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

        private bool _cancelPending;

        public DownloadingWindow()
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
            DataContext = this;
            _cancelPending = false;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.TidyMenu(this);
        }

        public void HandleDownloadEvent(TRDownloadEventArgs e)
        {
            if (e.Status == TRDownloadStatus.Failed)
            {
                WindowUtils.ShowError(e.Exception.Message);
                WindowUtils.EnableCloseButton(this, true);
                DialogResult = false;
            }
            else if (e.Status == TRDownloadStatus.Completed)
            {
                WindowUtils.EnableCloseButton(this, true);
                DialogResult = true;
            }
            else if (e.Status == TRDownloadStatus.Downloading)
            {
                if (_cancelPending)
                {
                    e.IsCancelled = true;
                    _cancelPending = false;
                }
                else
                {
                    ProgressTarget = (int)e.DownloadLength;
                    ProgressValue = (int)e.DownloadProgress;
                    ProgressDescription = e.URL;
                }
            }
            else if (e.IsCancelled)
            {
                WindowUtils.EnableCloseButton(this, true);
                DialogResult = false;
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Cancel();
        }

        private void Cancel()
        {
            _cancelPending = true;
            _cancelButton.IsEnabled = false;
            WindowUtils.EnableCloseButton(this, false);
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!_cancelPending && DialogResult == null)
            {
                Cancel();
                e.Cancel = true;
            }
        }
    }
}