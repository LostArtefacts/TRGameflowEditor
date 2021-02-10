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
using System.Windows.Shapes;
using TRGE.Core;
using TRGE.View.Utils;

namespace TRGE.View.Windows
{
    /// <summary>
    /// Interaction logic for AudioWindow.xaml
    /// </summary>
    public partial class AudioWindow : Window
    {
        #region Dependency Properties
        public static readonly DependencyProperty AudioDataProperty = DependencyProperty.Register
        (
            "AudioData", typeof(List<MutableTuple<string, string, ushort>>), typeof(AudioWindow)
        );

        public static readonly DependencyProperty AllAudioDataProperty = DependencyProperty.Register
        (
            "AllAudioData", typeof(IReadOnlyList<Tuple<ushort, string>>), typeof(AudioWindow)
        );

        public List<MutableTuple<string, string, ushort>> AudioData
        {
            get => (List<MutableTuple<string, string, ushort>>)GetValue(AudioDataProperty);
            set => SetValue(AudioDataProperty, value);
        }

        public IReadOnlyList<Tuple<ushort, string>> AllAudioData
        {
            get => (List<Tuple<ushort, string>>)GetValue(AllAudioDataProperty);
            private set => SetValue(AllAudioDataProperty, value);
        }
        #endregion

        public AudioWindow(IReadOnlyList<MutableTuple<string, string, ushort>> audioData, IReadOnlyList<Tuple<ushort, string>> allAudioData)
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
            DataContext = this;
            AudioData = new List<MutableTuple<string, string, ushort>>(audioData);
            AllAudioData = allAudioData;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.EnableMinimiseButton(this, false);
            WindowUtils.TidyMenu(this);
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}
