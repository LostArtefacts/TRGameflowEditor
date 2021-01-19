using System.Collections.Generic;

namespace TRGE.Core
{
    internal static class TRItemFactory
    {
        internal static AbstractTRItemProvider GetProvider(TREdition edition, IReadOnlyList<string> gameStrings)
        {
            switch (edition.Version)
            {
                case TRVersion.TR2:
                case TRVersion.TR2G:
                    return new TR2ItemProvider(edition, gameStrings);
                case TRVersion.TR3:
                case TRVersion.TR3G:
                    return new TR3ItemProvider(edition, gameStrings);
                default:
                    return null;
            }
        }
    }
}