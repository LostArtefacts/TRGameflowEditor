using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal abstract class AbstractTRLevelManager
    {
        internal virtual ITRLevel GetLevel(string id)
        {
            foreach (ITRLevel level in GetLevels())
            {
                if (level.GetID().Equals(id))
                {
                    return level;
                }
            }
            return null;
        }

        internal abstract List<ITRLevel> GetLevels();
        internal abstract void Save();
    }
}