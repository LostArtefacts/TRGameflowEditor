using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Threading;
using System.Windows;
using TRGE.Core;
using TRGE.View.Model.Audio;
using TRGE.View.Model.Data;
using TRGE.View.Utils;

namespace TRGE.View.Windows;

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
        Owner = WindowUtils.GetActiveWindow(this);
        DataContext = this;

        _dataProvider = dataProvider;
        AudioData = _dataProvider.GetAudioData();
        AllAudioData = AudioData.AllTracks;

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
        //get track data may invoke a download from GitHub, so this is performed
        //in a separate thread to allow a download window to show - see App.xaml.cs
        byte[] trackData = _dataProvider.GetAudioTrackData(_audioPlayingArgs.Track);
        Dispatcher.Invoke(new Action(() => PlayAudio(trackData)));
    }

    private void PlayAudio(byte[] trackData)
    {
        if (trackData == null || trackData.Length == 0)
        {
            //MessageWindow.ShowError("Failed to load track data.");
            _audioPlayingArgs.Callback.AudioFinished(_audioPlayingArgs.Track);
            return;
        }

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

    private void AudioControl_AudioSaveRequest(object sender, PlayAudioEventArgs e)
    {
        new Thread(() => LoadAndExportAudio(e)).Start();
    }

    private void LoadAndExportAudio(PlayAudioEventArgs e)
    {
        //get track data may invoke a download from GitHub, so this is performed
        //in a separate thread to allow a download window to show - see App.xaml.cs
        byte[] trackData = _dataProvider.GetAudioTrackData(e.Track);
        Dispatcher.Invoke(new Action(() => ExportAudio(e, trackData)));
    }

    private static void ExportAudio(PlayAudioEventArgs e, byte[] trackData)
    {
        if (trackData != null && trackData.Length > 0)
        {
            SaveFileDialog dlg = new()
            {
                Filter = "WAV Files|*.wav",
                Title = "TRGE : Save Audio Track",
                FileName = e.Track.Name.ToSafeFileName() + ".wav",
            };
            if (dlg.ShowDialog(WindowUtils.GetActiveWindow()) ?? false)
            {
                File.WriteAllBytes(dlg.FileName, trackData);
            }
        }

        e.Callback.AudioExportComplete(e.Track);
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