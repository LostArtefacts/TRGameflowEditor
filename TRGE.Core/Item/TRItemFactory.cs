namespace TRGE.Core;

internal static class TRItemFactory
{
    internal static AbstractTRItemProvider GetProvider(TREdition edition, IReadOnlyList<string> gameStrings)
    {
        return edition.Version switch
        {
            TRVersion.TR2 or TRVersion.TR2G => new TR2ItemProvider(edition, gameStrings),
            TRVersion.TR3 or TRVersion.TR3G => new TR3ItemProvider(edition, gameStrings),
            _ => null,
        };
    }
}