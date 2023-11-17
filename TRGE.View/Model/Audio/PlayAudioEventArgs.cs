using System;
using TRGE.View.Model.Data;

namespace TRGE.View.Model.Audio;

public class PlayAudioEventArgs : EventArgs
{
    public IPlayAudioCallback Callback { get; private set; }
    public AudioTrack Track { get; private set; }

    public PlayAudioEventArgs(IPlayAudioCallback callback, AudioTrack track)
    {
        Callback = callback;
        Track = track;
    }
}