using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TRGE.Core
{
    internal class TR23LevelManager : AbstractTRLevelManager
    {
        private TR23Script _script;
        internal TR23LevelManager(TR23Script script)
        {
            _script = script;
        }

        internal override List<ITRLevel> GetLevels()
        {
            throw new NotImplementedException();
        }

        internal override void Save()
        {
            throw new NotImplementedException();
        }
    }
}