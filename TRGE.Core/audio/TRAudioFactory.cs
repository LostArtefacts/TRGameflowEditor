namespace TRGE.Core
{
    internal static class TRAudioFactory
    {
        internal static AbstractTRAudioProvider GetAudioProvider(TREdition edition)
        {
            return GetAudioProvider(edition.Version);
        }

        internal static AbstractTRAudioProvider GetAudioProvider(TRVersion version)
        {
            switch (version)
            {
                case TRVersion.TR2:
                case TRVersion.TR2G:
                    return new TR2AudioProvider();
                case TRVersion.TR3:
                case TRVersion.TR3G:
                    return new TR3AudioProvider();
                default:
                    return null;
            }
        }
    }
}