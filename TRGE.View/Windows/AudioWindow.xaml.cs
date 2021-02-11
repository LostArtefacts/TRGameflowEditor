using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Media;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using TRGE.Coord;
using TRGE.Core;
using TRGE.View.Model;
using TRGE.View.Model.Audio;
using TRGE.View.Model.Data;
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
            "AudioData", typeof(AudioData), typeof(AudioWindow)
        );

        public static readonly DependencyProperty AllAudioDataProperty = DependencyProperty.Register
        (
            "AllAudioData", typeof(IReadOnlyList<AudioTrack>), typeof(AudioWindow)
        );

        public AudioData AudioData
        {
            get => (AudioData)GetValue(AudioDataProperty);
            set => SetValue(AudioDataProperty, value);
        }

        public IReadOnlyList<AudioTrack> AllAudioData
        {
            get => (List<AudioTrack>)GetValue(AllAudioDataProperty);
            private set => SetValue(AllAudioDataProperty, value);
        }
        #endregion

        private readonly IAudioDataProvider _dataProvider;
        private PlayAudioEventArgs _audioPlayingArgs;
        private AudioPlayer _audioPlayer;

        public AudioWindow(IAudioDataProvider dataProvider)
        {
            InitializeComponent();
            Owner = WindowUtils.GetActiveWindow();
            DataContext = this;

            _dataProvider = dataProvider;
            AudioData = _dataProvider.GetAudioData();
            AllAudioData = AudioData.AllTracks;

            TRCoord.Instance.ResourceDownloading += TRCoord_ResourceDownloading;

            MinWidth = Width;
            MinHeight = Height;
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            WindowUtils.EnableMinimiseButton(this, false);
            WindowUtils.TidyMenu(this);
        }

        private void AudioControl_AudioPlayRequest(object sender, PlayAudioEventArgs e)
        {
            StopAudio();
            _audioPlayingArgs = e;
            new Thread(LoadAndPlayAudio).Start();
        }

        private void AudioControl_AudioStopRequest(object sender, PlayAudioEventArgs e)
        {
            StopAudio();
        }

        private void LoadAndPlayAudio()
        {
            byte[] trackData = _dataProvider.GetAudioTrackData(_audioPlayingArgs.Track);
            Dispatcher.Invoke(new Action(() => PlayAudio(trackData)));
        }

        private void PlayAudio(byte[] trackData)
        {
            _audioPlayer = new AudioPlayer(trackData);
            _audioPlayer.AudioStarted += delegate (object sender, EventArgs e)
            {
                _audioPlayingArgs.Callback.AudioStarted(_audioPlayingArgs.Track);
            };
            _audioPlayer.AudioCompleted += delegate (object sender, EventArgs e)
            {
                _audioPlayingArgs.Callback.AudioFinished(_audioPlayingArgs.Track);
            };
            _audioPlayer.PlayAudio();
        }

        private void StopAudio()
        {
            if (_audioPlayer != null)
            {
                _audioPlayer.StopAudio();
            }
        }

        private void TRCoord_ResourceDownloading(object sender, TRDownloadEventArgs e)
        {
            if (e.Status == TRDownloadStatus.Initialising)
            {
                Dispatcher.Invoke(delegate
                {

                });
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            StopAudio();
        }

        private void OkButton_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }
    }
}