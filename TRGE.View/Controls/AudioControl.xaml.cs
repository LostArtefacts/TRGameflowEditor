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
    /// Interaction logic for AudioControl.xaml
    /// </summary>
    public partial class AudioControl : UserControl
    {
        #region Dependency Properties
        public static readonly DependencyProperty LevelNameProperty = DependencyProperty.Register
        (
            "LevelName", typeof(string), typeof(AudioControl)
        );

        public static readonly DependencyProperty LevelTrackProperty = DependencyProperty.Register
        (
            "LevelTrack", typeof(ushort), typeof(AudioControl)
        );

        public static readonly DependencyProperty AudioDataProperty = DependencyProperty.Register
        (
            "AudioData", typeof(List<Tuple<ushort, string>>), typeof(AudioControl)
        );

        public string LevelName
        {
            get => (string)GetValue(LevelNameProperty);
            set => SetValue(LevelNameProperty, value);
        }

        public ushort LevelTrack
        {
            get => (ushort)GetValue(LevelTrackProperty);
            set => SetValue(LevelTrackProperty, value);
        }

        public List<Tuple<ushort, string>> AudioData
        {
            get => (List<Tuple<ushort, string>>)GetValue(AudioDataProperty);
            set => SetValue(AudioDataProperty, value);
        }
        #endregion

        public AudioControl()
        {
            InitializeComponent();
            _content.DataContext = this;
        }
    }
}
