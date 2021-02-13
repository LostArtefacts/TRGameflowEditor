using System;
using System.Threading;
using System.Windows;
using TRGE.Coord;
using TRGE.Core;
using TRGE.View.Model;
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
        private readonly EditorOptions _options;

        public SaveProgressWindow(TREditor editor, EditorOptions options)
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
            DataContext = this;
            _editor = editor;
            _options = options;
        }

        private void Editor_SaveProgressChanged(object sender, TRSaveEventArgs e)
        {
            Dispatcher.Invoke(delegate
            {
                ProgressTarget = e.ProgressTarget;
                ProgressValue = e.ProgressValue;
                switch (e.Category)
                {
                    case TRSaveCategory.Scripting:
                        ProgressDescription = "Saving script data";
                        break;
                    case TRSaveCategory.LevelFile:
                        ProgressDescription = "Saving level file modifications";
                        break;
                    case TRSaveCategory.Commit:
                        ProgressDescription = "Committing changes";
                        break;
                }
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
            _editor.SaveProgressChanged += Editor_SaveProgressChanged;
            Exception error = null;

            try
            {
                _options.Save();
                _editor.Save();
            }
            catch (Exception e)
            {
                error = e;
            }
            finally
            {
                _editor.SaveProgressChanged -= Editor_SaveProgressChanged;

                Dispatcher.Invoke(delegate
                {
                    WindowUtils.EnableCloseButton(this, true);
                    if (error != null)
                    {
                        MessageWindow.ShowError(error.Message);
                        DialogResult = false;
                    }
                    else
                    {
                        DialogResult = true;
                    }
                });
            }
        }
    }
}