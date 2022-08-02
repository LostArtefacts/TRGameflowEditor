using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    public interface IUnarmedEditor
    {
        Organisation UnarmedLevelOrganisation { get; set; }
        RandomGenerator UnarmedLevelRNG { get; set; }
        uint RandomUnarmedLevelCount { get; set; }

        List<MutableTuple<string, string, bool>> UnarmedLevelData { get; set; }
    }
}