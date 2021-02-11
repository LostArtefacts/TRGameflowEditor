using TRGE.View.Model.Data;

namespace TRGE.View.Model.Audio
{
    public interface IAudioDataProvider
    {
        AudioData GetAudioData();
        byte[] GetAudioTrackData(AudioTrack track);
    }
}