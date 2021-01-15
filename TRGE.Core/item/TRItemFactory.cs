using System.Collections.Generic;

namespace TRGE.Core
{
    internal static class TRItemFactory
    {
        internal static AbstractTRItemProvider GetProvider(TRVersion version, IReadOnlyList<string> gameStrings)
        {
            switch (version)
            {
                case TRVersion.TR2:
                case TRVersion.TR2G:
                    return new TR2ItemProvider(gameStrings);
                case TRVersion.TR3:
                case TRVersion.TR3G:
                    return new TR3ItemProvider(gameStrings);
                default:
                    return null;
            }
        }
    }
}