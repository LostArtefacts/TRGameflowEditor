using TRGE.View.Model.Data;

namespace TRGE.View.Model.Audio;

public interface IPlayAudioCallback
{
    void AudioStarted(AudioTrack track);
    void AudioFinished(AudioTrack track);
    void AudioExportComplete(AudioTrack track);
}