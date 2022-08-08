using System.Collections.Generic;

namespace TRGE.Core
{
    public interface IAmmolessEditor
    {
        Organisation AmmolessLevelOrganisation { get; set; }

        RandomGenerator AmmolessLevelRNG { get; set; }

        uint RandomAmmolessLevelCount { get; set; }

        List<MutableTuple<string, string, bool>> AmmolessLevelData { get; set; }
    }
}