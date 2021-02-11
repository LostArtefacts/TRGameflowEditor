using TRGE.View.Model.Data;

namespace TRGE.View.Model
{
    public interface IPlayAudioCallback
    {
        void AudioStarted(AudioTrack track);
        void AudioFinished(AudioTrack track);
    }
}