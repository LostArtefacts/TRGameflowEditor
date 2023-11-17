using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using TRGE.View.Model.Audio;
using TRGE.View.Model.Data;

namespace TRGE.View.Controls;

/// <summary>
/// Interaction logic for AudioControl.xaml
/// </summary>
public partial class AudioControl : UserControl, IPlayAudioCallback
{
    #region Dependency Properties
    public static readonly DependencyProperty AudioDataProperty = DependencyProperty.Register
    (
        "AudioData", typeof(List<AudioTrack>), typeof(AudioControl)
    );

    public static readonly DependencyProperty AudioLevelDataProperty = DependencyProperty.Register
    (
        "AudioLevelData", typeof(AudioLevelData), typeof(AudioControl)
    );

    public static readonly DependencyProperty LevelNameProperty = DependencyProperty.Register
    (
        "LevelName", typeof(string), typeof(AudioControl)
    );

    public static readonly DependencyProperty LevelTrackProperty = DependencyProperty.Register
    (
        "LevelTrack", typeof(AudioTrack), typeof(AudioControl)
    );        

    public List<AudioTrack> AudioData
    {
        get => (List<AudioTrack>)GetValue(AudioDataProperty);
        set => SetValue(AudioDataProperty, value);
    }

    public AudioLevelData AudioLevelData
    {
        get => (AudioLevelData)GetValue(AudioLevelDataProperty);
        set => SetValue(AudioLevelDataProperty, value);
    }

    public string LevelName
    {
        get => (string)GetValue(LevelNameProperty);
        set => SetValue(LevelNameProperty, value);
    }

    public AudioTrack LevelTrack
    {
        get => (AudioTrack)GetValue(LevelTrackProperty);
        set => SetValue(LevelTrackProperty, value);
    }
    #endregion

    private static readonly Brush _standardBackground = Brushes.Transparent;
    private static readonly Brush _playingBackground = new SolidColorBrush(Color.FromArgb(0xFF, 0xE5, 0xF3, 0xFB));

    public event EventHandler<PlayAudioEventArgs> AudioPlayRequest;
    public event EventHandler<PlayAudioEventArgs> AudioSaveRequest;
    public event EventHandler<PlayAudioEventArgs> AudioStopRequest;
    private PlayAudioEventArgs _audioPlayingArgs;

    public AudioControl()
    {
        InitializeComponent();
        _content.DataContext = this;
    }

    private void AudioComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if ((e.Source as ComboBox).SelectedItem is AudioTrack track)
        {
            LevelTrack = AudioLevelData.Track = track;
            if (_stopButton.IsEnabled)
            {
                if (LevelTrack.ID == 0)
                {
                    StopSelectedAudio();
                }
                else
                {
                    PlaySelectedAudio();
                }
            }
            _playButton.IsEnabled = _saveButton.IsEnabled = LevelTrack.ID != 0; //TODO: check for blank track better than this
        }
    }

    private void PlayButton_Click(object sender, RoutedEventArgs e)
    {
        PlaySelectedAudio();
    }

    private void PlaySelectedAudio()
    {
        AudioPlayRequest?.Invoke(this, _audioPlayingArgs = new PlayAudioEventArgs(this, LevelTrack));
        _playButton.IsEnabled = false;
    }

    private void StopButton_Click(object sender, RoutedEventArgs e)
    {
        StopSelectedAudio();
    }

    private void StopSelectedAudio()
    {
        AudioStopRequest?.Invoke(this, _audioPlayingArgs);
        _stopButton.IsEnabled = false;
    }

    private void SaveButton_Click(object sender, RoutedEventArgs e)
    {
        AudioSaveRequest?.Invoke(this, new PlayAudioEventArgs(this, LevelTrack));
        _saveButton.IsEnabled = false;
    }

    public void AudioStarted(AudioTrack track)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(new Action(() => AudioStarted(track)));
        }
        else
        {
            _content.Background = _playingBackground;
            _playButton.Visibility = Visibility.Collapsed;
            _stopButton.Visibility = Visibility.Visible;
            _stopButton.IsEnabled = true;
        }
    }

    public void AudioFinished(AudioTrack track)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(new Action(() => AudioFinished(track)));
        }
        else
        {
            _content.Background = _standardBackground;
            _playButton.Visibility = Visibility.Visible;
            _stopButton.Visibility = Visibility.Collapsed;
            _stopButton.IsEnabled = false;
            _playButton.IsEnabled = LevelTrack.ID != 0;
        }
    }

    public void AudioExportComplete(AudioTrack track)
    {
        if (!Dispatcher.CheckAccess())
        {
            Dispatcher.Invoke(new Action(() => AudioFinished(track)));
        }
        else
        {
            _saveButton.IsEnabled = LevelTrack.ID != 0;
        }
    }
}