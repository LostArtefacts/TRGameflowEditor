using System.Collections.Generic;

namespace TRGE.Core
{
    public interface IMedilessEditor
    {
        Organisation MedilessLevelOrganisation { get; set; }

        RandomGenerator MedilessLevelRNG { get; set; }

        uint RandomMedilessLevelCount { get; set; }

        List<MutableTuple<string, string, bool>> MedilessLevelData { get; set; }
    }
}