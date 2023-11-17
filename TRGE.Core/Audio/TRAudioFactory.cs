namespace TRGE.Core;

public static class TRAudioFactory
{
    public static AbstractTRAudioProvider GetAudioProvider(TREdition edition)
    {
        return GetAudioProvider(edition.Version);
    }

    public static AbstractTRAudioProvider GetAudioProvider(TRVersion version)
    {
        return version switch
        {
            TRVersion.TR1 => new TR1AudioProvider(),
            TRVersion.TR2 or TRVersion.TR2G => new TR2AudioProvider(),
            TRVersion.TR3 or TRVersion.TR3G => new TR3AudioProvider(),
            _ => null,
        };
    }
}